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

using MonoMac.AppKit;

namespace AquissUsageChecker.StatusPanel
{
    /// <summary>
    /// The view displayed in the menu bar (status bar). This will be an image with an alternate 'highlight' image when the user has clicked on the icon.
    /// </summary>
    /// <remarks>More info can be found here: http://dan.clarke.name/2012/08/cocoa-popup-window-in-the-status-bar-monomac-port/</remarks>
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

