using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Libs.VNPTMoney.Payment.Models;
using TSFramework.Libs.Providers;
using TSFramework.Libs.Utils;
using static Libs.VNPTMoney.Payment.Consts.VNPTMoneyQRPaymentConst;

namespace Libs.VNPTMoney.Payment.Providers
{
    public class VNPTMoneyServiceProvider
    {
        private readonly string _apiBaseUrl;
        private readonly string _secretKey;
        private readonly string _apiKey;

        public VNPTMoneyServiceProvider(string apiBaseUrl, string secretKey, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
                throw new ArgumentNullException(nameof(apiBaseUrl), "API base URL cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");

            _apiBaseUrl = apiBaseUrl;
            _secretKey = secretKey;
            _apiKey = apiKey;
        }

        /// <summary>
        /// URL: {api_base_url}/create_vietqr
        /// Request method: POST
        /// Content-Type: application/json
        /// Parameter-Type: body
        /// </summary>
        /// <param name="actionUrl">create_vietqr</param>
        /// <param name="createQRCode"></param>
        /// <returns></returns>
        public Task<ResCreateQRCode> CreateQRCode(string actionUrl, ReqCreateQRCode createQRCode)
        {
            if (string.IsNullOrWhiteSpace(actionUrl))
                throw new ArgumentNullException(nameof(actionUrl), "Action URL cannot be null or empty.");

            if (createQRCode == null)
                throw new ArgumentNullException(nameof(createQRCode), "Create QR code request cannot be null.");
            createQRCode.SecretKey = _secretKey;

            var urlRequest = $"{_apiBaseUrl}/{actionUrl}";
            var bodyRequest = createQRCode.ToJson();

            var response = RestServiceProvider.Post(urlRequest, new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {_apiKey}" },
            }, ConstsContentTypes.JSON, bodyRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content;
                return Task.FromResult(responseContent.FromJson<ResCreateQRCode>());
            }

            return Task.FromResult<ResCreateQRCode>(null);
        }

        /// <summary>
        /// URL: {api_base_url}/query_transaction_by_billNumber
        /// Request method: POST
        /// Content-Type: application/json
        /// Parameter-Type: body
        /// </summary>
        /// <param name="actionUrl">query_transaction_by_billNumber</param>
        /// <param name="queryTransactionStatus"></param>
        /// <returns></returns>
        public Task<ResQueryTransactionBill> QueryTransactionBill(string actionUrl, ReqQueryTransactionBill queryTransactionStatus)
        {
            if (string.IsNullOrWhiteSpace(actionUrl))
                throw new ArgumentNullException(nameof(actionUrl), "Action URL cannot be null or empty.");

            if (queryTransactionStatus == null)
                throw new ArgumentNullException(nameof(queryTransactionStatus),
                    "Query transaction request cannot be null.");
            queryTransactionStatus.SecretKey = _secretKey;

            var urlRequest = $"{_apiBaseUrl}/{actionUrl}";
            var bodyRequest = queryTransactionStatus.ToJson();

            var response = RestServiceProvider.Post(urlRequest, new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", $"Bearer {_apiKey}" },
            }, ConstsContentTypes.JSON, bodyRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content;
                return Task.FromResult(responseContent.FromJson<ResQueryTransactionBill>());
            }

            return Task.FromResult<ResQueryTransactionBill>(null);
        }
    }
}