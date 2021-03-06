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
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreImage;

using AquissUsageChecker.StatusPanel;
using AquissUsageChecker.Util;

namespace AquissUsageChecker.UsagePanel
{
    /// <summary>
    /// Controller for the usage status panel
    /// </summary>
	public partial class UsagePanelController : PanelController
	{
		private readonly double Allowance = double.Parse(SettingsManager.GetSetting(SettingsManager.KeyAllowance));
		private const float ColourCap = 150;
		private const string InitialValue = "Checking...";
		private const string KeyLastChecked = "lastchecked";
		private const string KeyLastWarning = "lastwarning";

		private static readonly float[] Warnings = new[] { 0.75f, 0.95f }; // Warn at 75% and 95% usage
		private static readonly string[] WarningMessages = new[]
		{
			"Broadband usage is nearing the usage cap",
			"Broadband usage is about to hit the usage cap"
		};
		
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
			UsageIndicator.StartAnimation(this);
			PeakUsageLabel.StringValue = InitialValue;
			OffPeakUsageLabel.StringValue = InitialValue;
			TotalUsageLabel.StringValue = InitialValue;
			PeriodBeganLabel.StringValue = InitialValue;
			PeriodEndedLabel.StringValue = InitialValue;
		}
		
		#endregion

		public void UpdateUsage(UsageInformation usageInformation)
        {
            float usage = usageInformation.PeakUsage / (float)Allowance;

            PeakUsageLabel.StringValue = string.Format("{0:f2} GB / {1:f2} GB", usageInformation.PeakUsage, Allowance);
            OffPeakUsageLabel.StringValue = string.Format("{0:f2} GB", usageInformation.OffPeakUsage);
            TotalUsageLabel.StringValue = string.Format("{0:f2} GB", usageInformation.Usage);
		
            PeriodBeganLabel.StringValue = usageInformation.StartDate.ToLongDateString();
            PeriodEndedLabel.StringValue = usageInformation.EndDate.ToLongDateString();

            UsageIndicator.StopAnimation(this);

            UpdateIconTint(usage);
            NotifyUser(usageInformation);
		}

        /// <summary>
        /// Tint the icon from green to red as the usage limit is approached
        /// </summary>
        /// <param name="usage">Usage as a percentage (0...1)</param>
		private void UpdateIconTint(float usage)
		{
			if (StatusController == null)
				return;

			float red, green;

			if (usage > 1f)
				usage = 1f;
			if (usage < 0f)
				usage = 0f;

			if (usage <= 0.5f)
			{
				green = ColourCap;
				//red = ColourCap * usage * 2f;
				red = 0f;
			}
			else
			{
				green = ((1f - usage) * 2f) * ColourCap;
				red = ColourCap;
			}

			red = red / 255f;
			green = green / 255f;

			StatusController.TintIcon(CIColor.FromRgb(red, green, 0f));
		}

        /// <summary>
        /// Notify user if neccesary that they're approaching their usage limit
        /// </summary>
        /// <param name="usageInformation">Usage information.</param>
		private void NotifyUser(UsageInformation usageInformation)
		{
			var usage = usageInformation.PeakUsage / (float)Allowance;
			var lastWarning = GetLastWarning();
			var lastChecked = GetLastCheckedDate();

			for (int i = Warnings.Length - 1; i > 0; --i)
			{
				if (usage < Warnings[i])
					continue;

				// Check if we've already warned at this level (or above)
				if (lastChecked <= usageInformation.EndDate && lastWarning >= i)
					break;

				NSAlert.WithMessage("Broadband Usage Warning", "OK", null, null, WarningMessages[i]).RunModal();

				SettingsManager.SetSetting(KeyLastWarning, i.ToString());
				break;
			}

			SettingsManager.SetSetting(KeyLastChecked, DateTime.UtcNow.ToTimestamp().ToString());
		}

		private DateTime GetLastCheckedDate()
		{
			var lastCheckedString = SettingsManager.GetSetting(KeyLastChecked);

			if (string.IsNullOrWhiteSpace(lastCheckedString))
				return DateTime.MinValue;

			Int64 timestamp;
			
			if (Int64.TryParse(lastCheckedString, out timestamp))
				return timestamp.ToDateTime();
			else
				return DateTime.MinValue;
		}

		private int GetLastWarning()
		{
			var lastWarning = SettingsManager.GetSetting(KeyLastWarning);

			if (string.IsNullOrWhiteSpace(lastWarning))
				return -1;

			int warning;

			if (int.TryParse(lastWarning, out warning))
				return warning;
			else
				return -1;
		}

		partial void RefreshClicked(NSButton sender)
		{
			if (RefreshButtonClicked != null)
			{
				UsageIndicator.StartAnimation(this);
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

