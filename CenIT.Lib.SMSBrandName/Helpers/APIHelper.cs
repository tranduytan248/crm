using CenIT.Lib.SMSBrandName.Provider;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using TSFramework.Libs.Interfaces;

namespace CenIT.Lib.SMSBrandName.Helpers
{
    public class APIHelper
    {
        
        // Ký tự đặc biệt của XML: Có 5 ký tự đặc biệt của XML, khi gửi theo kiểu XML, nếu
        // gặp 5 ký tự đặc biệt là “, ‘, <, >, & thì phải thay thế tương ứng như sau:
        //  " &quot;
        //  ' &apos;
        //  < &lt;
        //  > &gt;
        //  & &amp;
        
        private string baseURL = "";
        public APIHelper()
        {
            baseURL = ConfigurationManager.AppSettings["BASE_URL_SMS_API"];
        }

        private string URLAPI(string name)
        {
            string urlConvert = ConfigurationManager.AppSettings[name];
            urlConvert = urlConvert.Replace("SSOAPIAND", "&");
            return urlConvert;
        }

        private Dictionary<string, string> HeaderParams()
        {
            var user = HttpContext.Current.User as IBasePrincipal;
            return new Dictionary<string, string>
            {
            };
        }

        /// <summary>
        /// Gọi API của SMS BrandName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T CallAPI<T>(string data)
        {
            string url = baseURL + URLAPI("URL_SMS_BRANDNAME");
            return WebApiProviders.PostRaw<T>(url, data, HeaderParams());
        }
        

    }
}
