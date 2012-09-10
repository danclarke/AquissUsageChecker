using System;

using MonoMac.AppKit;

namespace AquissUsageChecker.StatusPanel
{
    public class StatusPanelController : IDisposable
    {
        public const float StatusItemViewWidth = 24.0f;

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
                Image = NSImage.ImageNamed("Status.png"),
                AlternateImage = NSImage.ImageNamed("StatusHighlighted.png")
            };
            _statusItemView.StatusItemClicked += HandleStatusItemClicked;
            _panelController.StatusItemView = _statusItemView;
        }

        protected NSStatusItem StatusItem { get { return _statusItem; } }

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

                _panelController = value;
                _panelController.StatusItemView = _statusItemView;
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
                // Clear managed resourced here
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

