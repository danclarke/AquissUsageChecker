using System;
using System.IO;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreImage;

using AquissUsageChecker.Util;

namespace AquissUsageChecker.StatusPanel
{
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
            _statusItemView.StatusItemClicked += HandleStatusItemClicked;
            _panelController.StatusItemView = _statusItemView;
			_panelController.StatusController = this;
        }

        protected NSStatusItem StatusItem { get { return _statusItem; } }

		public void ResetIcon()
		{
			_statusItemView.Image = NSImage.ImageNamed(ImagePath);
		}

		public void TintIcon(CIColor colour)
		{
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
        /// Gets or sets the panel controller.
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

        protected virtual void HandleStatusItemClicked (object sender, EventArgs e)
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

