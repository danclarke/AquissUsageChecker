using System;
using System.Text;

using MonoMac.Security;
using MonoMac.Foundation;

namespace AquissUsageChecker.Util
{
    public static class Auth
    {
        private const string ServiceName = "Aquiss Usage Checker";

        /// <summary>
        /// Gets the Aquiss password from the OSX keychain
        /// </summary>
        /// <returns>
        /// Password is present in the keychain
        /// </returns>
        /// <param name='username'>
        /// The username
        /// </param>
        /// <param name='password'>
        /// The stored password
        /// </param>
        public static bool GetPassword(string username, out string password)
        {
            SecRecord searchRecord;
            var record = FetchRecord(username, out searchRecord);

            if (record == null)
            {
                password = string.Empty;
                return false;
            }

            password = NSString.FromData(record.ValueData, NSStringEncoding.UTF8);
            return true;
        }

        public static void SetPassword(string username, string password)
        {
            SecRecord searchRecord;
            var record = FetchRecord(username, out searchRecord);

            if (record == null)
            {
                record = new SecRecord(SecKind.InternetPassword)
                {
                    Service = ServiceName,
                    Label = ServiceName,
                    Account = username,
                    ValueData = NSData.FromString(password)
                };

                SecKeyChain.Add(record);
                return;
            }

            record.ValueData = NSData.FromString(password);
            SecKeyChain.Update(searchRecord, record);
        }

        private static SecRecord FetchRecord(string username, out SecRecord searchRecord)
        {
            searchRecord = new SecRecord(SecKind.InternetPassword)
            {
                Service = ServiceName,
                Account = username
            };

            SecStatusCode code;
            var data = SecKeyChain.QueryAsRecord(searchRecord, out code);

            if (code == SecStatusCode.Success)
                return data;
            else
                return null;
        }

        public static void ClearPassword(string username)
        {
            var searchRecord = new SecRecord(SecKind.InternetPassword)
            {
                Service = ServiceName,
                Account = username
            };

            SecKeyChain.Remove(searchRecord);
        }
    }
}

