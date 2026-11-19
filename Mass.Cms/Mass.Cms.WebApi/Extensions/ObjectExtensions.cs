using System.Dynamic;
using System.Reflection;

namespace Mass.Cms.WebApi.Extensions
{
    /// <summary>
    /// 单对象塑形：按照前端 fields 参数，把实体的"指定字段子集"打成一个 ExpandoObject 返回。
    /// fields 为空 = 不塑形，全量属性都带出来。
    /// </summary>
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<T>(this T source, string fields)
        {
            var expandoObject = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expandoObject;

            if (string.IsNullOrEmpty(fields))
            {
                //不塑形：所有公共实例属性按原样放进 Expando
                var properties = typeof(T).GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                foreach (var property in properties)
                {
                    expandoDict.Add(property.Name, property.GetValue(source));
                }
            }
            else
            {
                //塑形：fields="id,title" → 只放这两个属性（逗号、空格、空段都容忍）
                var fieldsArr = fields.Split(',');
                foreach (var field in fieldsArr)
                {
                    if (string.IsNullOrEmpty(field))
                    {
                        continue;
                    }

                    var propertyInfo = typeof(T).GetProperty(
                        field.Trim(),
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"没有找到{field.Trim()}对应的属性");
                    }

                    expandoDict.Add(propertyInfo.Name, propertyInfo.GetValue(source));
                }
            }

            return expandoObject;
        }
    }
}
