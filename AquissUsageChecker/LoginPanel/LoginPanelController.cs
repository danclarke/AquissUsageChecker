using System;
using System.Collections.Generic;
using System.Linq;

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
            var username = SettingsManager.GetSetting(SettingsManager.KeyUsername);
            var usage = SettingsManager.GetSetting(SettingsManager.KeyAllowance);

            if (!string.IsNullOrWhiteSpace(username))
                UsernameField.StringValue = username;

            if (!string.IsNullOrWhiteSpace(usage))
            {
                var usageString = usage + " GB";
                AllowanceCombo.StringValue = usageString;
            }
        }
		
		#endregion

        protected virtual void OnLoggedIn(string username, string password)
        {
            string allowance;

            if (AllowanceCombo.StringValue.Length > 0)
                allowance = AllowanceCombo.StringValue.Substring(0, AllowanceCombo.StringValue.Length - 3);
            else
                allowance = string.Empty;

            SettingsManager.SetSetting(SettingsManager.KeyUsername, username);
            SettingsManager.SetSetting(SettingsManager.KeyAllowance, allowance);

            Auth.SetPassword(username, password);

            if (LoggedIn != null)
                LoggedIn(this, new LoggedInEventArgs(username, password));
        }

        partial void LoginClicked(MonoMac.AppKit.NSButton sender)
        {
            OnLoggedIn(UsernameField.StringValue, PasswordField.StringValue);
        }

        partial void QuitClicked(MonoMac.AppKit.NSButton sender)
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
        private readonly string _username;
        private readonly string _password;

        public LoggedInEventArgs(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string Username { get { return _username; } }
        public string Password { get { return _password; } }
    }
}

