using System;

using RestSharp;

using AquissUsageChecker.Service.ReturnTypes;

namespace AquissUsageChecker.Service
{
    public static class AquissService
    {
        private const string UrlBase = "http://api.aquiss.net/stable";
        private const string UrlUsage = "usage-xml.php";

        /// <summary>
        /// Gets the current bandwidth usage.
        /// </summary>
        /// <param name='hashKey'>
        /// Hash key
        /// </param>
        /// <param name='success'>
        /// Success delegate
        /// </param>
        /// <param name='failure'>
        /// Failure delegate
        /// </param>
        public static void GetUsage(string hashKey, Action<UsageReturnValue> success, Action<IRestResponse<UsageReturnValue>> failure)
        {
            var client = new RestClient(UrlBase);
            var request = new RestRequest(UrlUsage, Method.GET);
            request.AddParameter("hashkey", hashKey);

            client.ExecuteAsync<UsageReturnValue>(request, response =>
            {
                if (string.IsNullOrWhiteSpace(response.ErrorMessage) && response.ErrorException == null)
                    success(response.Data);
                else
                    failure(response);
            });
        }
    }
}

