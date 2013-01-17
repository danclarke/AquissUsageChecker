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

namespace AquissUsageChecker.Service.ReturnTypes
{
    public sealed class UsageReturnValue
    {
        /// <summary>
        /// Version of the API called
        /// </summary>
        /// <value>
        /// API version
        /// </value>
        public string Version { get; set; }

        /// <summary>
        /// Free text response from server. 'Valid' if all OK, otherwise error string
        /// </summary>
        /// <value>
        /// Text response
        /// </value>
        public string Response { get; set; }

        /// <summary>
        /// Hashkey used for the request
        /// </summary>
        /// <value>
        /// The hash key for this user
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Friendly start date string, en-GB locale
        /// </summary>
        /// <value>
        /// The usage start date
        /// </value>
        public string UsageStartDate { get; set; }

        /// <summary>
        /// Friendly end date string, en-GB locale
        /// </summary>
        /// <value>
        /// The usage end date
        /// </value>
        public string UsageEndDate { get; set; }

        /// <summary>
        /// Usage start date as a Unix timestamp
        /// </summary>
        /// <value>
        /// Usage start date timestamp
        /// </value>
        public Int64 UsageStartDateString { get; set; }

        /// <summary>
        /// Usage end date as a Unix timestamp
        /// </summary>
        /// <value>
        /// Usage end date timestamp
        /// </value>
        public Int64 UsageEndDateString { get; set; }

        /// <summary>
        /// Peak usage in GiB
        /// </summary>
        public float UsagePeak { get; set; }

        /// <summary>
        /// Off peal usage in GiB
        /// </summary>
        public float UsageOffPeak { get; set; }

        /// <summary>
        /// Total usage in GiB
        /// </summary>
        public float UsageTotal { get; set; }
    }
}

