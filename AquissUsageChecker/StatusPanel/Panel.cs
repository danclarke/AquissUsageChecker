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
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace AquissUsageChecker.StatusPanel
{
    /// <summary>
    /// Base class for the panel view used in menu bar panels
    /// </summary>
    /// <remarks>More info: http://dan.clarke.name/2012/08/cocoa-popup-window-in-the-status-bar-monomac-port/</remarks>
    public partial class Panel : NSPanel
    {
		#region Constructors
		
        // Called when created from unmanaged code
        public Panel(IntPtr handle) : base(handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public Panel(NSCoder coder) : base(coder)
        {
            Initialize();
        }
		
        // Shared initialization code
        private void Initialize()
        {
        }
		
		#endregion

        // Required so that we can hide the window if the user clicks away
        public override bool CanBecomeKeyWindow { get { return true; } }
    }
}

