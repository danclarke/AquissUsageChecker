using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

using AquissUsageChecker.StatusPanel;

namespace AquissUsageChecker.UsagePanel
{
    public partial class UsagePanel : Panel
    {
		#region Constructors
		
        // Called when created from unmanaged code
        public UsagePanel(IntPtr handle) : base(handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public UsagePanel(NSCoder coder) : base(coder)
        {
            Initialize();
        }
		
        // Shared initialization code
        private void Initialize()
        {
        }
		
		#endregion


    }
}

