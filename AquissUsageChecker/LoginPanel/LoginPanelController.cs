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
using System.Diagnostics;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.Security;

using AquissUsageChecker.StatusPanel;
using AquissUsageChecker.Util;

namespace AquissUsageChecker.LoginPanel
{
    /// <summary>
    /// Controller for login panel
    /// </summary>
    public partial class LoginPanelController : PanelController
    {
        public event EventHandler<LoggedInEventArgs> LoggedIn;
        public event EventHandler QuitButtonClicked;

        // Strings used for selecting the allowance in the UI
		protected static readonly string[] UsageStrings = new[] { "3 GB", "30 GB", "60 GB", "90 GB" };

        // Export the usage strings to Interface Builder & OSX dropdown widged
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

            // Save verified valid login details
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
			
            // Try and validate the hash code, in the background so we don't lock the UI
			UsageChecker.ValidateHashCode(HashCode.StringValue, (success) => 
			{
                BeginInvokeOnMainThread(() =>
                {
                    // Update UI with what we found out
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

