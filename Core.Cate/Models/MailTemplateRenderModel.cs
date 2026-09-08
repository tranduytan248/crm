using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Cate.Models
{
    /// <summary>
    /// Model dùng chung để render mẫu email từ dữ liệu JSON.
    /// </summary>
    public class MailTemplateRenderModel
    {
        #region Declaration

        private Dictionary<string, object> _parameters;

        #endregion

        #region Property

        public string TemplateCode { get; set; }

        public string JsonData { get; set; }

        public Dictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region Constructor

        public MailTemplateRenderModel()
        {
            Parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gán chuỗi JSON cho model và parse sang dictionary.
        /// </summary>
        public void SetJsonData(string jsonData)
        {
            JsonData = jsonData ?? string.Empty;
            Parameters = ParseJsonToDictionary(JsonData);
        }


        /// <summary>
        /// Gán trực tiếp tập tham số cho model.
        /// </summary>
        public void SetParameters(Dictionary<string, object> parameters)
        {
            Parameters = parameters ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            JsonData = JsonConvert.SerializeObject(Parameters);
        }


        /// <summary>
        /// Lấy giá trị chuỗi theo key.
        /// </summary>
        public string GetString(string key, string defaultValue = "")
        {
            object value = GetValue(key);
            if (value == null)
            {
                return defaultValue;
            }

            string stringValue = Convert.ToString(value);
            return stringValue ?? defaultValue;
        }


        /// <summary>
        /// Lấy giá trị kiểu bool theo key.
        /// </summary>
        public bool GetBoolean(string key, bool defaultValue = false)
        {
            object value = GetValue(key);
            if (value == null)
            {
                return defaultValue;
            }

            bool parsedValue;
            if (bool.TryParse(Convert.ToString(value), out parsedValue))
            {
                return parsedValue;
            }

            return defaultValue;
        }


        /// <summary>
        /// Lấy giá trị kiểu DateTime theo key.
        /// </summary>
        public DateTime? GetDateTime(string key)
        {
            object value = GetValue(key);
            if (value == null)
            {
                return null;
            }

            DateTime parsedValue;
            if (DateTime.TryParse(Convert.ToString(value), out parsedValue))
            {
                return parsedValue;
            }

            return null;
        }


        /// <summary>
        /// Lấy giá trị object theo key.
        /// </summary>
        public object GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (Parameters == null || !Parameters.ContainsKey(key))
            {
                return null;
            }

            return Parameters[key];
        }


        /// <summary>
        /// Kiểm tra key có tồn tại trong dữ liệu render hay không.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key)
                && Parameters != null
                && Parameters.ContainsKey(key);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse JSON phẳng thành dictionary dùng cho render.
        /// </summary>
        private Dictionary<string, object> ParseJsonToDictionary(string jsonData)
        {
            Dictionary<string, object> result =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return result;
            }

            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
            if (jsonObject == null)
            {
                return result;
            }

            foreach (JProperty property in jsonObject.Properties())
            {
                JToken token = property.Value;
                result[property.Name] = token == null || token.Type == JTokenType.Null
                    ? string.Empty
                    : token.ToObject<object>();
            }

            return result;
        }

        #endregion 
    }
}
