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
using System.IO;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreImage;

using AquissUsageChecker.Util;

namespace AquissUsageChecker.StatusPanel
{
    /// <summary>
    /// The status icon controller (not very well named...)
    /// </summary>
    /// <remarks>More info can be found here: http://dan.clarke.name/2012/08/cocoa-popup-window-in-the-status-bar-monomac-port/</remarks>
    public class StatusPanelController : IDisposable
    {
        public const float StatusItemViewWidth = 24.0f;
		private const string ImagePath = "Status.png";
		private const string HighlightImagePath = "StatusHighlighted.png";

        private NSStatusItem _statusItem;
        private StatusItemView _statusItemView;
   
        private PanelController _panelController;
        private bool _disposed = false;

        public StatusPanelController(PanelController panelController)
        {
            _panelController = panelController;
            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(StatusItemViewWidth);
			_statusItemView = new StatusItemView(_statusItem)
			{
			    Image = NSImage.ImageNamed(ImagePath),
			    AlternateImage = NSImage.ImageNamed(HighlightImagePath)
			};
            _statusItemView.StatusItemClicked += OnStatusItemClicked;
            _panelController.StatusItemView = _statusItemView;
			_panelController.StatusController = this;
        }

        protected NSStatusItem StatusItem { get { return _statusItem; } }

		public void ResetIcon()
		{
			_statusItemView.Image = NSImage.ImageNamed(ImagePath);
		}

        /// <summary>
        /// Tint the icon with the specified colour
        /// </summary>
        /// <param name="colour">Colour to tint icon with</param>
		public void TintIcon(CIColor colour)
		{
            // Use CoreImage to tint the icon
			var statusImage = CIImage.FromUrl(NSUrl.FromFilename(NSBundle.MainBundle.PathForResource(
				Path.GetFileNameWithoutExtension(HighlightImagePath), Path.GetExtension(HighlightImagePath))));
			var tintImage = CIImage.ImageWithColor(colour);
			var filter = CIFilter.FromName("CIMultiplyCompositing");

			filter.SetValueForKey(tintImage, (NSString)"inputImage");
			filter.SetValueForKey(statusImage, (NSString)"inputBackgroundImage");

			var processedImage = (CIImage)filter.ValueForKey((NSString)"outputImage");
		 	var outputImage = processedImage.ToNSImage();

			_statusItemView.Image = outputImage;
		}

        /// <summary>
        /// Gets or sets the panel controller, this is the panel that will be displayed when the user clicks the icon
        /// </summary>
        /// <value>
        /// The panel controller.
        /// </value>
        public virtual PanelController PanelController
        {
            get { return _panelController; }
            set
            {
                if (_panelController == value)
                    return;

                _panelController.ClosePanel();
				_panelController.StatusItemView = null;
				_panelController.StatusController = null;

                _panelController = value;
                _panelController.StatusItemView = _statusItemView;
				_panelController.StatusController = this;
                _panelController.OpenPanel();
            }
        }

        protected virtual void OnStatusItemClicked(object sender, EventArgs e)
        {
            _statusItemView.IsHighlighted = !_statusItemView.IsHighlighted;

            if (_statusItemView.IsHighlighted)
                PanelController.OpenPanel();
            else
                PanelController.ClosePanel();
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Clear managed resources here
            }

            if (_statusItem != null)
            {
                NSStatusBar.SystemStatusBar.RemoveStatusItem(_statusItem);
                _statusItem = null;
            }

            _disposed = true;
        }

        ~StatusPanelController()
        {
            Dispose(false);
        }

        #endregion

    }
}

