using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using RestSharp;

namespace TSFramework.Libs.Providers
{
    public static class RestServiceProvider
    {
        public static IRestResponse PostFile(string urlRequest, Dictionary<string, string> headerData, string contentType,
            Dictionary<string, string> bodyData, Dictionary<string, HttpPostedFileBase> dictAttachFiles)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient(urlRequest);
            var request = new RestRequest(Method.POST)
            {
                AlwaysMultipartFormData = true,
            };
            request.AddHeader("content-type", contentType);
            if (dictAttachFiles != null)
            {
                foreach (var dictKey in dictAttachFiles.Keys)
                {
                    var attachFile = dictAttachFiles[dictKey];
                    if (attachFile != null)
                    {
                        byte[] dataFile;
                        using (Stream inputStream = attachFile.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            dataFile = memoryStream.ToArray();
                        }
                        request.AddFile(dictKey, dataFile, attachFile.FileName, attachFile.ContentType);
                    }
                }
            }

            foreach (var key in headerData.Keys) request.AddHeader(key, headerData[key]);

            if (bodyData != null && bodyData.Count > 0)
            {
                foreach (var key in bodyData.Keys)
                {
                    request.AddParameter(key, bodyData[key]);
                }
            }

            return client.Execute(request);
        }

        /// <summary>
        /// Request dạng POST kèm file
        /// </summary>
        /// <param name="urlRequest"></param>
        /// <param name="headerData"></param>
        /// <param name="contentType"></param>
        /// <param name="bodyData"></param>
        /// <param name="dictAttachFiles"></param>
        /// <returns></returns>
        public static IRestResponse PostFile(string urlRequest, Dictionary<string, string> headerData, string contentType,
            string bodyData, Dictionary<string, HttpPostedFileBase> dictAttachFiles)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient(urlRequest);
            var request = new RestRequest(Method.POST)
            {
                AlwaysMultipartFormData = true,
            };
            request.AddHeader("content-type", contentType);
            if (dictAttachFiles != null)
            {
                foreach (var dictKey in dictAttachFiles.Keys)
                {
                    var attachFile = dictAttachFiles[dictKey];
                    if (attachFile != null)
                    {
                        byte[] dataFile;
                        using (Stream inputStream = attachFile.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            dataFile = memoryStream.ToArray();
                        }
                        request.AddFile(dictKey, dataFile, attachFile.FileName, attachFile.ContentType);
                    }
                }
            }

            foreach (var key in headerData.Keys) request.AddHeader(key, headerData[key]);

            if (!string.IsNullOrEmpty(bodyData))
            {
                request.AddParameter(contentType, bodyData, ParameterType.RequestBody);
            }

            return client.Execute(request);
        }

        /// <summary>
        /// Request dạng POST không kèm file
        /// </summary>
        /// <param name="urlRequest"></param>
        /// <param name="headerData"></param>
        /// <param name="contentType"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public static IRestResponse Post(string urlRequest, Dictionary<string, string> headerData, string contentType,
            string bodyData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient(urlRequest);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", contentType);

            foreach (var key in headerData.Keys) request.AddHeader(key, headerData[key]);

            request.AddParameter(contentType, bodyData, ParameterType.RequestBody);

            return client.Execute(request);
        }

        /// <summary>
        /// Request dạng Get
        /// </summary>
        /// <param name="urlRequest"></param>
        /// <param name="headerData"></param>
        /// <returns></returns>
        public static IRestResponse Get(string urlRequest, Dictionary<string, string> headerData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient(urlRequest);
            var request = new RestRequest(Method.GET);
            foreach (var key in headerData.Keys) request.AddHeader(key, headerData[key]);
            return client.Execute(request);
        }

        /// <summary>
        /// Tải file
        /// </summary>
        /// <param name="urlRequest"></param>
        /// <param name="headerData"></param>
        /// <returns></returns>
        public static byte[] DownloadFile(string urlRequest, Dictionary<string, string> headerData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new RestClient(urlRequest);
            var request = new RestRequest(Method.GET);
            foreach (var key in headerData.Keys) request.AddHeader(key, headerData[key]);
            var response = client.Execute(request);

            return response.RawBytes;
        }
    }
}