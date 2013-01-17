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

            // If we don't have the hashcode saved, we need to request it from the user
            if (string.IsNullOrWhiteSpace(hashCode))
            {
                SetLoginPanel();
                return;
            }

            // Display the status panel (if user is logged in)
            SetStatusPanel();
        }

        /// <summary>
        /// Panel to display should be the login panel
        /// </summary>
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

        /// <summary>
        /// Panel to display should be the usage status
        /// </summary>
        private void SetStatusPanel()
		{
			var hashCode = SettingsManager.GetSetting(SettingsManager.KeyHashCode);

			if (string.IsNullOrWhiteSpace(hashCode))
				SetLoginPanel();

            // Since this is init, we may have a different hash code, so create a new usage checker
            // TODO: Ideally UsageChecker needs to be refactored out and put into the UsagePanel
			if (_usageChecker != null)
				_usageChecker.Dispose();
            
			_usageChecker = new UsageChecker(hashCode);

            // Instantiate the usage panel and hook up the neccessary events
            var panelController = new UsagePanelController();

            panelController.RefreshButtonClicked += (sender, e) => _usageChecker.UpdateUsageInformation();
            panelController.QuitButtonClicked += (sender, e) => NSApplication.SharedApplication.Terminate(sender as UsagePanelController);
            panelController.LogoutButtonClicked += (sender, e) => Logout();

            // Whenever the usage checker gets a new value, update the UI
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
            // Clean up all of the panels, and disposable resources
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

