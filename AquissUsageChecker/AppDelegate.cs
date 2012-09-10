using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

using AquissUsageChecker.StatusPanel;
using AquissUsageChecker.UsagePanel;
using AquissUsageChecker.LoginPanel;
using AquissUsageChecker.Util;

namespace AquissUsageChecker
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        private StatusPanelController _statusPanelController;
        private UsageChecker _usageChecker;

        public AppDelegate()
        {
        }

        public override void AwakeFromNib()
        {
            /*var username = SettingsManager.GetSetting(SettingsManager.KeyUsername);

            if (string.IsNullOrWhiteSpace(username))
            {
                SetLoginPanel();
                return;
            }

            string password;
            if (!Auth.GetPassword(username, out password))
            {
                SetLoginPanel();
                return;
            }*/

            SetStatusPanel();
        }

        private void SetLoginPanel()
        {
            var loginController = new LoginPanelController();

            loginController.LoggedIn += (sender, e) => SetStatusPanel();
            loginController.QuitButtonClicked += (sender, e) => NSApplication.SharedApplication.Terminate(sender as LoginPanelController);

            if (_statusPanelController == null)
                _statusPanelController = new StatusPanelController(loginController);
            else
                _statusPanelController.PanelController = loginController;
        }

        private void SetStatusPanel()
        {
            if (_usageChecker == null)
                _usageChecker = new UsageChecker();

            var panelController = new UsagePanelController();

            panelController.RefreshButtonClicked += (sender, e) => _usageChecker.UpdateUsageInformation();
            panelController.QuitButtonClicked += (sender, e) => NSApplication.SharedApplication.Terminate(sender as UsagePanelController);
            panelController.LogoutButtonClicked += (sender, e) => Logout();

            _usageChecker.UsageUpdated += (sender, e) => panelController.UpdateUsage(e.UsageInformation);

            if (_statusPanelController != null)
                _statusPanelController.PanelController = panelController;
            else
                _statusPanelController = new StatusPanelController(panelController);
        }

        private void Logout()
        {
            var username = SettingsManager.GetSetting(SettingsManager.KeyUsername);
            _usageChecker.Dispose();
            _usageChecker = null;

            Auth.ClearPassword(username);
            SetLoginPanel();
        }

        public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            if (_usageChecker != null)
            {
                _usageChecker.Dispose();
                _usageChecker = null;
            }

            if (_statusPanelController != null)
            {
                _statusPanelController.Dispose();
                _statusPanelController = null;
            }

            return NSApplicationTerminateReply.Now;
        }
    }
}

