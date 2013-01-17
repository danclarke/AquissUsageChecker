/*
    AquissUsageChecker - Realtime display of broadband usage on Aquiss
    Copyright (C) 2013  Dan Clarke

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

using RestSharp;

using AquissUsageChecker.Service.ReturnTypes;

namespace AquissUsageChecker.Service
{
    /// <summary>
    /// Class to access the Aquiss API
    /// </summary>
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

