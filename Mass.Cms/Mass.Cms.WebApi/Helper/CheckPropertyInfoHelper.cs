using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Mass.Cms.WebApi.Helper
{
    /// <summary>
    /// 塑形参数合法性校验：前端 fields 里每个字段名，都必须在 DTO 里真实存在，
    /// 有一个对不上就整体拦下（返回 false）→ 控制器单独 BadRequest，不让脏参数漏进塑形。
    /// fields 为空 = 前端不做塑形，直接放行（true）。
    /// </summary>
    public class CheckPropertyInfoHelper
    {
        public static bool IsCheckPropertyInfo<T>(string? fields, IMemoryCache memoryCache, string key)
        {
            //fields 没传 = 不塑形，直接全量放行
            if (string.IsNullOrEmpty(fields))
            {
                return true;
            }

            //========== 基线版（能跑，但每个字段都单独走一次反射查找）==========
            var fieldsArr = fields.Trim().Split(',');

            //引入缓存：DTO 的 PropertyInfo[] 属性列表，按 key 缓存起来，避免每次请求都 GetProperty
            var properties = (PropertyInfo[])memoryCache.Get(key+ ":Properties");
            if (properties == null)
            {
                //缓存没命中，走反射
                properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                //范围搜索只要公共实例属性，静态/私有/保护的都不管
                //还能提高性能，不做无用查找

                //这玩意要改必定重启，所以没必要设置过期时间
                memoryCache.Set(key + ":Properties", properties);
            }
            //遍历前端传来的字段
            foreach(var field in fieldsArr)
            {
                //空行跳过
                if (string.IsNullOrEmpty(field)) continue;

                if (!properties.Any(p => p.Name.Equals(field.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                    //在数组中找不到已有的字段
                }

            }

            return true;

        }
    }
}
