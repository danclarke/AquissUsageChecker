
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

using AquissUsageChecker.StatusPanel;

namespace AquissUsageChecker.LoginPanel
{
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

