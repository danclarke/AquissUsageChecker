using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.Security;

using AquissUsageChecker.StatusPanel;
using AquissUsageChecker.Util;

namespace AquissUsageChecker.LoginPanel
{
    public partial class LoginPanelController : PanelController
    {
        public event EventHandler<LoggedInEventArgs> LoggedIn;
        public event EventHandler QuitButtonClicked;

		protected static readonly string[] UsageStrings = new[] { "3 GB", "30 GB", "60 GB", "90 GB" };

		[Export("usageStrings")]
		public string[] AllowanceUsageStrings { get { return UsageStrings; } }

		#region Constructors
		
        // Called when created from unmanaged code
        public LoginPanelController(IntPtr handle) : base(handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public LoginPanelController(NSCoder coder) : base(coder)
        {
            Initialize();
        }
		
        // Call to load from the XIB/NIB file
        public LoginPanelController() : base("LoginPanel")
        {
            Initialize();
        }
		
        // Shared initialization code
        private void Initialize()
        {
            var hashCode = SettingsManager.GetSetting(SettingsManager.KeyHashCode);
            var usage = SettingsManager.GetSetting(SettingsManager.KeyAllowance);

            if (!string.IsNullOrWhiteSpace(hashCode))
                HashCode.StringValue = hashCode;

            if (!string.IsNullOrWhiteSpace(usage))
            {
                var usageString = usage + " GB";
				AllowancePopup.SelectItem(((IList<string>)UsageStrings).IndexOf(usageString));
            }
        }
		
		#endregion

        protected virtual void OnLoggedIn(string hashCode)
        {
			var allowanceString = AllowancePopup.SelectedItem.Title;
			var allowance = allowanceString.Substring(0, allowanceString.Length - 3);

            SettingsManager.SetSetting(SettingsManager.KeyHashCode, hashCode);
            SettingsManager.SetSetting(SettingsManager.KeyAllowance, allowance);

            if (LoggedIn != null)
                LoggedIn(this, new LoggedInEventArgs(hashCode));
        }

		partial void WhatIsHashCodeClicked(NSButton sender)
		{
			NSAlert.WithMessage("About Hash Code", "OK", null, null, "The Hash Code is available in your control panel at http://www.aquiss.net").RunModal();
		}

        partial void LoginClicked(NSButton sender)
        {
			ActivityIndicator.StartAnimation(this);
			ViewUsageButton.Enabled = false;
			
			UsageChecker.ValidateHashCode(HashCode.StringValue, (success) => 
			{
                BeginInvokeOnMainThread(() =>
                {
                    ActivityIndicator.StopAnimation(this);
                    ViewUsageButton.Enabled = true;
                    
                    if (success)
                        OnLoggedIn(HashCode.StringValue);
                    else
                        NSAlert.WithMessage("Error", "OK", null, null, "The Hash Code you entered is incorrect, please double-check the code you entered.").RunModal();
                });
			});
        }

        partial void QuitClicked(NSButton sender)
        {
            if (QuitButtonClicked != null)
                QuitButtonClicked(this, EventArgs.Empty);
        }
		
        //strongly typed window accessor
        public new LoginPanel Window
        {
            get
            {
                return (LoginPanel)base.Window;
            }
        }
    }

    public class LoggedInEventArgs : EventArgs
    {
        private readonly string _hashCode;

        public LoggedInEventArgs(string hashCode)
        {
            _hashCode = hashCode;
        }

        public string Hashcode { get { return _hashCode; } }
    }
}

