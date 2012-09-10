
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using AquissUsageChecker.StatusPanel;

namespace AquissUsageChecker.UsagePanel
{
	public partial class UsagePanelController : PanelController
	{
		private const string InitialValue = "Checking...";
		
		public event EventHandler RefreshButtonClicked;
		public event EventHandler LogoutButtonClicked;
		public event EventHandler QuitButtonClicked;

		#region Constructors
		
		// Called when created from unmanaged code
		public UsagePanelController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public UsagePanelController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public UsagePanelController() : base("UsagePanel")
		{
			Initialize();
		}
		
		// Shared initialization code
		private void Initialize()
		{
			// Default values
			ProgressIndicator.StartAnimation(this);
			PeakUsageLabel.StringValue = InitialValue;
			OffPeakUsageLabel.StringValue = InitialValue;
			TotalUsageLabel.StringValue = InitialValue;
			PeriodBeganLabel.StringValue = InitialValue;
			PeriodEndedLabel.StringValue = InitialValue;
		}
		
		#endregion

		public void UpdateUsage(UsageInformation usageInformation)
		{
			PeakUsageLabel.StringValue = string.Format("{0:f2} GB", usageInformation.PeakUsage);
			OffPeakUsageLabel.StringValue = string.Format("{0:f2} GB", usageInformation.OffPeakUsage);
			TotalUsageLabel.StringValue = string.Format("{0:f2} GB", usageInformation.Usage);
			
			PeriodBeganLabel.StringValue = usageInformation.StartDate.ToLongDateString();
			PeriodEndedLabel.StringValue = usageInformation.EndDate.ToLongDateString();
			
			ProgressIndicator.StopAnimation(this);
		}

		partial void RefreshClicked(NSButton sender)
		{
			if (RefreshButtonClicked != null)
			{
				ProgressIndicator.StartAnimation(this);
				RefreshButtonClicked(this, EventArgs.Empty);
			}
		}
		
		partial void LogoutClicked(NSButton sender)
		{
			if (LogoutButtonClicked != null)
				LogoutButtonClicked(this, EventArgs.Empty);
		}
		
		partial void QuitClicked(NSButton sender)
		{
			if (QuitButtonClicked != null)
				QuitButtonClicked(this, EventArgs.Empty);
		}
		
		//strongly typed window accessor
		public new UsagePanel Window
		{
			get { return (UsagePanel)base.Window; }
		}
	}
}

