using System.Net;
using System.Web;

namespace TSFramework.Libs.Helpers
{
    public static class ClientHelper
    {
        /// <summary>
        /// Lấy IPv4 từ máy tính
        /// </summary>
        /// <returns></returns>
        public static string GetIPv4OfComputer()
        {
             
            var myIP = string.Empty;
            myIP = HttpContext.Current.Request.UserHostAddress;
            if (myIP != null)
            {
                return myIP;
            }
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostEntry(hostName).AddressList;

            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    myIP = address.ToString();
                }
            }
            return myIP;
        } 

        public static string GetBrowserInfo()
        {
            var userAgent = HttpContext.Current.Request.UserAgent;

            return userAgent;
        }
    }
}