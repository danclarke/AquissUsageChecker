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
        private const string HashKey = "ylCYEAPkwtkGlohRJw7LGfvuW2MROayc";
        private const double UpdateInterval = 1000 * 60 * 30; // Every 30 minutes

        private readonly object _lockObject = new object();

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

        public UsageChecker()
        {
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

        public void UpdateUsageInformation()
        {
            AquissService.GetUsage(HashKey, (val) =>
            {
                lock (_lockObject)
                    _currentUsage = new UsageInformation(val);
                OnUsageUpdated();
            }, (resp) =>
            {
                using (var pool = new NSAutoreleasePool())
                {
                    pool.InvokeOnMainThread(() =>
                    {
                        var alert = NSAlert.WithMessage("Error", "OK", null, null, "Could not get usage information: " + resp.ErrorMessage);
                        alert.AlertStyle = NSAlertStyle.Warning;
                        alert.Icon = null;
                        alert.RunModal();
                    });
                }
            });
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

    public struct UsageInformation
    {
        public UsageInformation(UsageReturnValue val)
        {
            Usage = val.UsageTotal;
            OffPeakUsage = val.UsageOffPeak;
            PeakUsage = val.UsagePeak;
            StartDate = val.UsageStartDateString.ToDateTime().ToLocalTime();
            EndDate = val.UsageEndDateString.ToDateTime().ToLocalTime();
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

