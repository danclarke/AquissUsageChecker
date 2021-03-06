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

using AquissUsageChecker.StatusPanel;

namespace AquissUsageChecker.LoginPanel
{
    /// <summary>
    /// The panel/view itself for logging in
    /// </summary>
    public partial class LoginPanel : Panel
    {
		#region Constructors
		
        // Called when created from unmanaged code
        public LoginPanel(IntPtr handle) : base (handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public LoginPanel(NSCoder coder) : base (coder)
        {
            Initialize();
        }
		
        // Shared initialization code
        void Initialize()
        {
        }
		
		#endregion
    }
}

