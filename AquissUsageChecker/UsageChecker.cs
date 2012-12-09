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
    /// The actual usage checker
    /// </summary>
    public class UsageChecker : IDisposable
    {
        private const double UpdateInterval = 1000 * 60 * 30; // Every 30 minutes

        private readonly object _lockObject = new object();
		private readonly string _hashCode;

        private UsageInformation _currentUsage;
        private Timer _autoUpdateTimer;

        public event EventHandler<UsageUpdatedEventArgs> UsageUpdated;

        public UsageInformation CurrentUsage
        {
            get
            {
                lock (_lockObject)
                    return _currentUsage;
            }
        }

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

        public UsageInformation UsageInformation { get { return _usageInformation; } }
    }
}

