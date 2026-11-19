using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.WebApi.AutofaceDI;
using Mass.Cms.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

// ========== ① 服务注册区 ==========
var builder = WebApplication.CreateBuilder(args);

//API 控制器体系（注意：不是 AddControllersWithViews，因为没有视图）
builder.Services.AddControllers();

// 1. 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        // 允许任意来源（开发环境用，生产环境最好写死前端地址）
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              //CORS 默认只让前端 JS 读 6 个标准响应头，自定义头必须在这里"报备"才能被 JS 读到
              .WithExposedHeaders("act");
    });
});

//JWT 鉴权：告诉后端"哪些 token 才是我们签发的"——校验密钥/签发者/受众/有效期，都从 appsettings 的 Jwt 段读
//和 LoginController 生成 token 时用同一套参数，两边对不上，token 就验不过
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"] ?? "")),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(10)  //默认过期后还宽限5分钟，课上为了快速看到401压缩成10秒
        };

        //OnChallenge：验票失败（没票/假票/过期票）即将回 401 的最后一刻触发。
        //在这里补写自定义响应头 act=expired，给前端一个"这次 401 是票失效了"的信号
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.Response.Headers["act"] = "expired";
                return Task.CompletedTask;
            }
        };
    });

//Swagger 的两件套：探测器 + 生成器
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //JWT 授权头配置：给 Swagger 右上角加"Authorize"按钮
    //没有这三段，[Authorize] 的接口在 Swagger 里连输 token 的地方都没有（之前一直靠脚本/前端带 token 测试）
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "粘贴登录接口返回的 token（不用手写 Bearer 前缀）"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//数据库：每个 HTTP 请求从容器领一个全新的 MyDbContext（注入的是 DbContextOptions，里面带着连接串）
builder.Services.AddDbContext<MyDbContext>(sp =>
{
    var connStr = builder.Configuration.GetConnectionString("StrConn");
    sp.UseSqlServer(connStr);
});

//内存缓存：塑形校验的"反射属性列表"住在这里（每请求同一个缓存实例，进程级复用）
builder.Services.AddMemoryCache();

//⑮ Redis 分布式缓存：注册 IConnectionMultiplexer 单例（连接工厂，全局一个，线程安全）
//ConnectionMultiplexer 是重量级对象——创建贵、创建后复用终身，所以必须是单例不能是 Scoped
//连接串里 defaultDatabase=3：我们用 db3 存权限编码，和别的用途隔离开（db0~15 物理隔离的库）
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));

//把 UnitOfWorkFilter 注册为"全局过滤器"：所有 Action 执行完都会经过它，
//只有标了 [UnitOfWork] 的才真正 SaveChanges —— 你 Web 项目里写的那套在这里第一次上岗
builder.Services.Configure<MvcOptions>(opt =>
{
    opt.Filters.Add<UnitOfWorkFilter>();
});

//========== Autofac：替换默认 DI 容器 ==========
//默认容器是微软自己的，功能够用但"一个个注册"太啰嗦；
//Autofac 能按程序集扫描、按命名约定批量注册（见 AutofacModuleRegister）
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacModuleRegister());
    });

//AutoMapper：扫描 WebApi 项目下所有继承 Profile 的类，让 CreateMap 生效
builder.Services.AddAutoMapper(g =>
{
    g.AddMaps(new[] { "Mass.Cms.WebApi" });
});

var app = builder.Build();
app.UseCors("MyCorsPolicy");

// ========== ② 管道区 ==========
//Swagger 只在开发环境出现：给后端自己调试用，生产环境此页自动消失
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//鉴权在前、授权在后：先把"你是谁"验明白（能看懂 token 的 claims），再谈"你能不能干这事"
app.UseAuthentication();
app.UseAuthorization();

// ========== ③ 路由映射区 ==========
//让控制器上的 [Route("api/[controller]")] 和 [HttpGet]/[HttpPost] 生效。
//注意最关键的一点："请求方式 + 路由地址"的组合必须唯一，组合重复直接启动/请求报错
app.MapControllers();

app.Run();
