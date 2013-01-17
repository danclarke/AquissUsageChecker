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
using System.Timers;

using MonoMac.Foundation;
using MonoMac.AppKit;

using AquissUsageChecker.Util;
using AquissUsageChecker.Service;
using AquissUsageChecker.Service.ReturnTypes;

namespace AquissUsageChecker
{
    /// <summary>
    /// The actual usage checker, checks usage periodically
    /// </summary>
    public class UsageChecker : IDisposable
    {
        private const double UpdateInterval = 1000 * 60 * 30; // Every 30 minutes

        private readonly object _lockObject = new object();
		private readonly string _hashCode;

        private UsageInformation _currentUsage;
        private Timer _autoUpdateTimer;

        /// <summary>
        /// New usage information has been fetched from the server
        /// </summary>
        public event EventHandler<UsageUpdatedEventArgs> UsageUpdated;

        public UsageInformation CurrentUsage
        {
            get
            {
                lock (_lockObject)
                    return _currentUsage;
            }
        }

        /// <summary>
        /// Instantiate a new usage checker
        /// </summary>
        /// <param name="hashCode">The user's hashcode - this is the code that uniquely identifies them</param>
        public UsageChecker(string hashCode)
        {
			_hashCode = hashCode;

            SetupAutoFetch();
            UpdateUsageInformation();
        }

        private void SetupAutoFetch()
        {
            StopAutoFetch();

            _autoUpdateTimer = new Timer(UpdateInterval);
            _autoUpdateTimer.Elapsed += (sender, e) => UpdateUsageInformation();
            _autoUpdateTimer.Start();
        }

        private void StopAutoFetch()
        {
            if (_autoUpdateTimer != null)
            {
                _autoUpdateTimer.Stop();
                _autoUpdateTimer = null;
            }
        }

        protected virtual void OnUsageUpdated()
        {
            // Usage info comes from an async request, it could be on any thread
            // So we need to make sure we have a release pool for ObjC code in the thread
            // We also need to ensure we get back on the UI thread to prevent potential cross-thread issues
            using (var pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(() =>
                {
                    UsageInformation usage;
                    lock (_lockObject)
                        usage = _currentUsage;

                    if (UsageUpdated != null)
                        UsageUpdated(this, new UsageUpdatedEventArgs(usage));
                });
            }
        }

		public static void ValidateHashCode(string hashCode, Action<bool> callback)
		{
			AquissService.GetUsage(hashCode, (val) => callback(val.Response == "Valid"), (resp) => callback(false));
		}

        /// <summary>
        /// Immediately request new usage data
        /// </summary>
        public void UpdateUsageInformation()
        {
            AquissService.GetUsage(_hashCode, (val) =>
            {
                lock (_lockObject)
				{
					var usage = UsageInformation.CreateUsageInformation(val);
					if (usage != null)
						_currentUsage = usage;
					else
                    {
                        _currentUsage = null;
						UsageFetchError("Invalid Hash Code");
                    }
				}
                OnUsageUpdated();
            }, (resp) => UsageFetchError(resp.ErrorMessage));
        }

		private void UsageFetchError(string errorMessage)
		{
			using (var pool = new NSAutoreleasePool())
			{
				pool.BeginInvokeOnMainThread(() =>
				{
					var alert = NSAlert.WithMessage("Error", "OK", null, null, "Could not get usage information: " + errorMessage);
					alert.AlertStyle = NSAlertStyle.Warning;
					alert.RunModal();
				});
			}
		}

        #region IDisposable implementation

        ~UsageChecker()
        {
            StopAutoFetch();
        }

        public void Dispose()
        {
            StopAutoFetch();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Detailed information about the current usage as returned by Aquiss
    /// </summary>
    public sealed class UsageInformation
    {
		public static UsageInformation CreateUsageInformation(UsageReturnValue val)
        {
			if (val.Response != "Valid")
				return null;

			return new UsageInformation
			{
	            Usage = val.UsageTotal,
	            OffPeakUsage = val.UsageOffPeak,
	            PeakUsage = val.UsagePeak,
	            StartDate = val.UsageStartDateString.ToDateTime().ToLocalTime(),
	            EndDate = val.UsageEndDateString.ToDateTime().ToLocalTime()
			};
        }

        /// <summary>
        /// The current usage in GiB
        /// </summary>
        public float Usage;

        /// <summary>
        /// The off peak usage in GiB
        /// </summary>
        public float OffPeakUsage;

        /// <summary>
        /// The peak usage in GiB
        /// </summary>
        public float PeakUsage;

        /// <summary>
        /// The start date for the current usage period, in local time
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// The end date for the current usage period, in local time
        /// </summary>
        public DateTime EndDate;
    }

    public class UsageUpdatedEventArgs : EventArgs
    {
        private readonly UsageInformation _usageInformation;

        public UsageUpdatedEventArgs(UsageInformation usageInformation)
        {
            _usageInformation = usageInformation;
        }

        /// <summary>
        /// The new usage information
        /// </summary>
        /// <value>The usage information.</value>
        public UsageInformation UsageInformation { get { return _usageInformation; } }
    }
}

