using System;
using System.Drawing;

using MonoMac.AppKit;

namespace AquissUsageChecker.StatusPanel
{
    public class StatusItemView : NSView
    {
        private readonly NSStatusItem _statusItem;
        private bool _mouseDown;

        public StatusItemView(NSStatusItem statusItem)
            : base(new RectangleF(0f, 0f, statusItem.Length, NSStatusBar.SystemStatusBar.Thickness))
        {
            _statusItem = statusItem;
            _statusItem.View = this;
        }

        public event EventHandler StatusItemClicked;

        private NSImage _image;
        public NSImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NeedsDisplay = true;
            }
        }

        private NSImage _alternateImage;
        public NSImage AlternateImage
        {
            get { return _alternateImage; }
            set
            {
                _alternateImage = value;
                NeedsDisplay = true;
            }
        }

        public RectangleF GlobalRect
        {
            get
            {
                return new RectangleF(Window.ConvertBaseToScreen(Frame.Location), Frame.Size);
            }
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted == value)
                    return;

                _isHighlighted = value;
                NeedsDisplay = true;
            }
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            _statusItem.DrawStatusBarBackgroundInRectwithHighlight(dirtyRect, IsHighlighted);

            var icon = IsHighlighted ? AlternateImage : Image;

            if (icon == null)
                return;

            float iconX = (float)Math.Round((Bounds.Width - icon.Size.Width) / 2f);
            float iconY = (float)Math.Round((Bounds.Height - icon.Size.Height) / 2f);
            icon.Draw(new PointF(iconX, iconY), RectangleF.Empty, NSCompositingOperation.SourceOver, 1.0f);
        }

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);

            _mouseDown = true;
        }

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);

            if (StatusItemClicked != null && _mouseDown)
                StatusItemClicked(this, EventArgs.Empty);

            _mouseDown = false;
        }
    }
}

