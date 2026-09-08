using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TSFramework.Libs.Processors;

namespace CenIT.Lib.SMSBrandName.Provider
{
    public class WebApiProviders
    {
        public static T Get<T>(string url, Dictionary<string, string> postParams, Dictionary<string, string> headerParams = null)
        {
            return MakeRequest<T>("GET", url, headerParams, postParams);
        }

        public static T Post<T>(string url, Dictionary<string, string> postParams, Dictionary<string, string> headerParams = null )
        {
            return MakeRequest<T>("POST", url, headerParams, postParams);
        }

        public static T PostRaw<T>(string url, string postParams, Dictionary<string, string> headerParams = null)
        {
            return MakeRequestRaw<T>("POST", url, headerParams, postParams);
        }

        private static T MakeRequestRaw<T>(string httpMethod, string url, Dictionary<string, string> headerParams = null, string postParams = null)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), url);
            requestMessage.Content = new StringContent(postParams,
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header

            if (headerParams != null)
            {
                foreach (KeyValuePair<string, string> entry in headerParams)
                {
                    requestMessage.Headers.Add(entry.Key, entry.Value);
                }
            }

            HttpResponseMessage response = client.SendAsync(requestMessage).Result;
            string apiResponse = response.Content.ReadAsStringAsync().Result;
            try
            {
                // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                if (apiResponse != "")
                    return JsonConvert.DeserializeObject<T>(apiResponse);
                else
                    return default(T);
            }
            catch (Exception ex)
            {
                // CLogger.Error(ex);
                AppProcessor.Logger.Error(new Exception(ex.ToString()));
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
            }
            
        }

        private static T MakeRequest<T>(string httpMethod, string url, Dictionary<string, string> headerParams = null, Dictionary<string, string> postParams = null)
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), url);

                if (headerParams != null)
                {
                    foreach (KeyValuePair<string, string> entry in headerParams)
                    {
                        requestMessage.Headers.Add(entry.Key, entry.Value);
                    }
                }
                

                if (postParams != null)
                    requestMessage.Content = new FormUrlEncodedContent(postParams);   // This is where your content gets added to the request body


                HttpResponseMessage response = client.SendAsync(requestMessage).Result;

                string apiResponse = response.Content.ReadAsStringAsync().Result;
                try
                {
                    // Attempt to deserialise the reponse to the desired type, otherwise throw an expetion with the response from the api.
                    if (apiResponse != "")
                        return JsonConvert.DeserializeObject<T>(apiResponse);
                    else
                        return default(T);
                }
                catch (Exception ex)
                {
                    AppProcessor.Logger.Error(new Exception(ex.ToString()));
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }
    }
}

