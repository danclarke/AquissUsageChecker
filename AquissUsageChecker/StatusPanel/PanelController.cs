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
using System.Drawing;
using System.Diagnostics;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace AquissUsageChecker.StatusPanel
{
    /// <summary>
    /// Base class for custom menu bar panels
    /// </summary>
    /// <remarks>More info can be found here: http://dan.clarke.name/2012/08/cocoa-popup-window-in-the-status-bar-monomac-port/</remarks>
    public partial class PanelController : NSWindowController
    {
        private const float OpenDuration = 0.15f;
        private const float CloseDuration = 0.1f;
        private const float MenuAnimationDuration = 0.1f;

		#region Constructors
		
        // Called when created from unmanaged code
        public PanelController(IntPtr handle) : base(handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public PanelController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from a XIB/NIB file
        public PanelController(string windowNibName) : base(windowNibName)
        {
            Initialize();
        }
		
        // Call to load from the XIB/NIB file
        public PanelController() : base("Panel")
        {
            Initialize();
        }
		
        // Shared initialization code
        private void Initialize()
        {
            // Additional validation for inheriting implementations
            if (!(base.Window is Panel))
                throw new InvalidOperationException("Window must inherit from Panel");
            if (BackgroundView == null)
                throw new InvalidOperationException("BackgroundView cannot be NULL");

            // Perform styling
            Window.AcceptsMouseMovedEvents = true;
            Window.Level = NSWindowLevel.PopUpMenu;
            Window.IsOpaque = false;
            Window.BackgroundColor = NSColor.Clear;

            // Hookup Events
            Window.WillClose += (sender, e) => ClosePanel();
            Window.DidResignKey += (sender, e) =>
            {
                if (Window.IsVisible)
                    ClosePanel();
            };
            Window.DidResize += HandleWindowDidResize;
        }
		
		#endregion

        /// <summary>
        /// The StatusItemView that toggles this panel
        /// </summary>
        public StatusItemView StatusItemView { get; set; }

		/// <summary>
		/// The controller that's managing the status view and this controller
		/// </summary>
		/// <value>
		/// The status controller.
		/// </value>
		public StatusPanelController StatusController { get; set; }

        protected virtual void HandleWindowDidResize (object sender, EventArgs e)
        {
            var statusRect = StatusRectForWindow(Window);
            var panelRect = Window.Frame;

            float statusX = (float)Math.Round(statusRect.Right - (statusRect.Width / 2.0f));
            float panelX = statusX - panelRect.Left;

            BackgroundView.ArrowX = (int)panelX;
        }

        protected RectangleF StatusRectForWindow(NSWindow window)
        {
            RectangleF statusRect;

            if (StatusItemView != null)
            {
                statusRect = StatusItemView.GlobalRect;
            }
            else
            {
                var screenRect = NSScreen.Screens[0].Frame;
                var originX = (screenRect.Width - statusRect.Width) / 2.0f;
                var originY = screenRect.Height - statusRect.Height * 2.0f;
                statusRect = new RectangleF(originX, originY, StatusPanelController.StatusItemViewWidth, NSStatusBar.SystemStatusBar.Thickness);
            }

            return statusRect;
        }

        /// <summary>
        /// Gets whether the panel is currently 'open'
        /// </summary>
        /// <value>
        /// <c>true</c> if this panel is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; protected set; }

        /// <summary>
        /// Opens the panel
        /// </summary>
        public virtual void OpenPanel()
        {
            if (IsOpen)
                return;

            var screenRect = NSScreen.Screens[0].Frame;
            var statusRect = StatusRectForWindow(Window);
           
            var panelRect = Window.Frame;
            panelRect.X = (float)Math.Round((statusRect.Right - (statusRect.Width)) - panelRect.Width / 2f);
            panelRect.Y = statusRect.Top - panelRect.Height;

            if (panelRect.Right > (screenRect.Right - BackgroundView.ArrowHeight))
                panelRect.X -= panelRect.Right - (screenRect.Right - BackgroundView.ArrowHeight);

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            Window.AlphaValue = 0f;
            Window.SetFrame(statusRect, true);
            Window.MakeKeyAndOrderFront(this);

            float openDuration = OpenDuration;

#if DEBUG
            var currentEvent = NSApplication.SharedApplication.CurrentEvent;
            if (currentEvent.Type == NSEventType.LeftMouseUp)
            {
                var clearFlags = currentEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask;
                bool shiftPressed = clearFlags == NSEventModifierMask.ShiftKeyMask;
                bool shiftOptionPressed = clearFlags == (NSEventModifierMask.ShiftKeyMask | NSEventModifierMask.AlternateKeyMask);
                if (shiftPressed || shiftOptionPressed)
                {
                    openDuration *= 10f;

                    if (shiftOptionPressed)
                        Debug.WriteLine("Icon is at {0}\n\tMenu is on screen {1}\n\tWill be animated to {2}", statusRect, screenRect, panelRect);
                }
            }
#endif

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = openDuration;
            var animator = (NSWindow)Window.Animator; // Note the cast neccesary for MonoMac
            animator.SetFrame(panelRect, true);
            animator.AlphaValue = 1f;
            NSAnimationContext.EndGrouping();

            IsOpen = true;
            StatusItemView.IsHighlighted = true;
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(false);
        }

        /// <summary>
        /// Closes the panel
        /// </summary>
        public virtual void ClosePanel()
        {
            if (!IsOpen)
                return;

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = CloseDuration;
            var animator = (NSWindow)Window.Animator; // Note the cast neccesary for MonoMac
            animator.AlphaValue = 0f;
            NSAnimationContext.EndGrouping();

            IsOpen = false;
            StatusItemView.IsHighlighted = false;
        }
    }
}

