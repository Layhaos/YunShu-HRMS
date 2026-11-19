using Mass.Cms.IService;
using Mass.Cms.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mass.Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController:ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IConfiguration _configuration;
        public LoginController(IUserInfoService userInfoService, IConfiguration configuration)
        {
            _userInfoService = userInfoService;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userInfoService.LoadEntities(a => a.UserPhone == loginDto.UserPhone).FirstOrDefaultAsync();
            if (user == null || user.UserPassword != loginDto.UserPassword)
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = "账号或密码错误" });
            }
            // 登录成功，可以生成 token 等操作

            //JWT生成
            var secretKey = _configuration["Jwt:SecretKey"];
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserPhone),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);



            return Ok(new ApiResult<string> { IsSuccess = true, Message = "登录成功", Data = tokenString });
        }
        
    }
}
