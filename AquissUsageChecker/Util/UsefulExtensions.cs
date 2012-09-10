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

