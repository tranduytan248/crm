using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.ClientScript;
using System.Collections;

namespace Modules.API.Helpers
{
    public class IgnorePropsResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _propsToIgnore;

        public IgnorePropsResolver(IEnumerable<string> propNames)
        {
            _propsToIgnore = new HashSet<string>(propNames, StringComparer.OrdinalIgnoreCase);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => !_propsToIgnore.Contains(p.PropertyName)).ToList();
        }
    }

    /// <summary>
    /// Xóa mấy thuộc tính dư thừa trong API
    /// </summary>
    public static class CustomDataApi
    {
        public static List<object> RemvoveProperties<T>(List<T> obj, string[] propNames = null)
        {
            // Các prop mặc định cần bỏ
            var defaultProp = new[] { "CanDelete", "CanEdit", "OrderDir", "RowIndex", "Reason", "UpdatedBy", "TotalRow" };

            // Gộp lại: nếu propNames null thì dùng defaultProp, nếu có thì nối thêm
            var propsToIgnore = (propNames == null || propNames.Length == 0)
                ? defaultProp
                : defaultProp.Concat(propNames).ToArray();

            // Config bỏ các prop đó khi serialize
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropsResolver(propsToIgnore)
            };

            var json = JsonConvert.SerializeObject(obj, settings);
            return JsonConvert.DeserializeObject<List<object>>(json);
        }

        //public static List<object> RemvoveProperties<T>(List<T> obj, string[] propNames = null)
        //{
        //    // Các prop mặc định cần bỏ
        //    var defaultProp = new[] { "Search", "Order", "OrderDir", "StartIndex", "PageSize", "TotalRow" };

        //    // Gộp lại: nếu propNames null thì dùng defaultProp, nếu có thì nối thêm
        //    var propsToIgnore = (propNames == null || propNames.Length == 0)
        //        ? defaultProp
        //        : defaultProp.Concat(propNames).ToArray();

        //    // Config bỏ các prop đó khi serialize
        //    var settings = new JsonSerializerSettings
        //    {
        //        ContractResolver = new IgnorePropsResolver(propsToIgnore)
        //    };

        //    var json = JsonConvert.SerializeObject(obj, settings);
        //    return JsonConvert.DeserializeObject<List<object>>(json);
        //}

        public static object RemvovePropertiesAuto(object data, string[] propNames = null)
        {
            var defaultProp = new[] { "CanDelete", "CanEdit", "OrderDir", "RowIndex", "Reason", "UpdatedBy", "TotalRow" };
            var propsToIgnore = (propNames == null || propNames.Length == 0)
                ? defaultProp
                : defaultProp.Concat(propNames).ToArray();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropsResolver(propsToIgnore)
            };

            var json = JsonConvert.SerializeObject(data, settings);

            if (data is IEnumerable)
            {
                return JsonConvert.DeserializeObject<List<object>>(json);
            }
            else
            {
                return JsonConvert.DeserializeObject<object>(json);
            }
        }



    }
}