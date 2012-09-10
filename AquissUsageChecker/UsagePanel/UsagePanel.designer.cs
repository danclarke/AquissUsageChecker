// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace AquissUsageChecker.UsagePanel
{
	[Register ("UsagePanelController")]
	partial class UsagePanelController
	{
		[Outlet]
		MonoMac.AppKit.NSProgressIndicator ProgressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField PeakUsageLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField OffPeakUsageLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField TotalUsageLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField PeriodBeganLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField PeriodEndedLabel { get; set; }

		[Action ("RefreshClicked:")]
		partial void RefreshClicked (MonoMac.AppKit.NSButton sender);

		[Action ("LogoutClicked:")]
		partial void LogoutClicked (MonoMac.AppKit.NSButton sender);

		[Action ("QuitClicked:")]
		partial void QuitClicked (MonoMac.AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ProgressIndicator != null) {
				ProgressIndicator.Dispose ();
				ProgressIndicator = null;
			}

			if (PeakUsageLabel != null) {
				PeakUsageLabel.Dispose ();
				PeakUsageLabel = null;
			}

			if (OffPeakUsageLabel != null) {
				OffPeakUsageLabel.Dispose ();
				OffPeakUsageLabel = null;
			}

			if (TotalUsageLabel != null) {
				TotalUsageLabel.Dispose ();
				TotalUsageLabel = null;
			}

			if (PeriodBeganLabel != null) {
				PeriodBeganLabel.Dispose ();
				PeriodBeganLabel = null;
			}

			if (PeriodEndedLabel != null) {
				PeriodEndedLabel.Dispose ();
				PeriodEndedLabel = null;
			}
		}
	}

	[Register ("UsagePanel")]
	partial class UsagePanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
