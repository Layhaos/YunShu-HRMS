using System.Dynamic;
using System.Reflection;

namespace Mass.Cms.WebApi.Extensions
{
    /// <summary>
    /// 集合塑形：ShapeData 的集合版。关键区别——
    /// 属性列表只解析一次，然后对每条数据循环复用（不复用的话 N 条数据就反射 N 次）。
    /// </summary>
    public static class EnumableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeDatas<T>(this IEnumerable<T> source, string? fields)
        {
            //第 1 步：确定"要带哪些属性"——只做一次
            var propertyInfosList = new List<PropertyInfo>();

            if (string.IsNullOrEmpty(fields))
            {
                //不塑形：全部属性
                var properties = typeof(T).GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                propertyInfosList.AddRange(properties);
            }
            else
            {
                //fields="title,,id" → 解析掉空段后逐个找属性；找不到直接抛异常
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
                        throw new Exception($"没有找到{field.Trim()}对应的属性信息");
                    }

                    propertyInfosList.Add(propertyInfo);
                }
            }

            //第 2 步：对每条数据，用这份属性列表装配一个 ExpandoObject
            var expandoObjectList = new List<ExpandoObject>();
            foreach (var sourceObject in source)
            {
                var expandoObject = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expandoObject;

                foreach (var propertyInfo in propertyInfosList)
                {
                    expandoDict.Add(propertyInfo.Name, propertyInfo.GetValue(sourceObject));
                }

                expandoObjectList.Add(expandoObject);
            }

            return expandoObjectList;
        }
    }
}
