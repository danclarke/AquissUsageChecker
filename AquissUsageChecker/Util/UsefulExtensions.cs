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

namespace AquissUsageChecker.Util
{
    public static class UsefulExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert a unix timestamp to a DateTime
        /// </summary>
        /// <returns>
        /// Timestamp expressed as a DateTime, UTC
        /// </returns>
        /// <param name='timestamp'>
        /// Timestamp to convert
        /// </param>
        public static DateTime ToDateTime(this int timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Convert a unix timestamp to a DateTime
        /// </summary>
        /// <returns>
        /// Timestamp expressed as a DateTime, UTC
        /// </returns>
        /// <param name='timestamp'>
        /// Timestamp to convert
        /// </param>
        public static DateTime ToDateTime(this Int64 timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Convert a unix timestamp to a DateTime
        /// </summary>
        /// <returns>
        /// Timestamp expressed as a DateTime, UTC
        /// </returns>
        /// <param name='timestamp'>
        /// Timestamp to convert
        /// </param>
        public static DateTime ToDateTime(this double timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts a DateTime to unix timestamp
        /// </summary>
        /// <returns>
        /// DateTime expressed as a unix timestamp
        /// </returns>
        /// <param name='date'>
        /// Date to convert
        /// </param>
        public static Int64 ToTimestamp(this DateTime date)
        {
            var utcDate = (date.Kind == DateTimeKind.Utc) ? date : date.ToUniversalTime();
            return (Int64)(utcDate - UnixEpoch).TotalSeconds;
        }
    }
}

