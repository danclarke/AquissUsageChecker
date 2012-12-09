using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

using AquissUsageChecker.StatusPanel;
using AquissUsageChecker.UsagePanel;
using AquissUsageChecker.LoginPanel;
using AquissUsageChecker.Util;
using System.Diagnostics;

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
            var hashCode = SettingsManager.GetSetting(SettingsManager.KeyHashCode);

            if (string.IsNullOrWhiteSpace(hashCode))
            {
                SetLoginPanel();
                return;
            }

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
			var hashCode = SettingsManager.GetSetting(SettingsManager.KeyHashCode);

			if (string.IsNullOrWhiteSpace(hashCode))
				SetLoginPanel();

			if (_usageChecker != null)
				_usageChecker.Dispose();
            
			_usageChecker = new UsageChecker(hashCode);

            var panelController = new UsagePanelController();

            panelController.RefreshButtonClicked += (sender, e) => _usageChecker.UpdateUsageInformation();
            panelController.QuitButtonClicked += (sender, e) => NSApplication.SharedApplication.Terminate(sender as UsagePanelController);
            panelController.LogoutButtonClicked += (sender, e) => Logout();

            _usageChecker.UsageUpdated += (sender, e) =>
            {
                if (e.UsageInformation != null)
                    panelController.UpdateUsage(e.UsageInformation);
            };

            if (_statusPanelController != null)
                _statusPanelController.PanelController = panelController;
            else
                _statusPanelController = new StatusPanelController(panelController);
        }

        private void Logout()
        {
            _usageChecker.Dispose();
            _usageChecker = null;

            SetLoginPanel();
			_statusPanelController.ResetIcon();
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

