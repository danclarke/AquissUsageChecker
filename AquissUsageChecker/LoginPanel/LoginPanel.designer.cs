// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace AquissUsageChecker.LoginPanel
{
	[Register ("LoginPanelController")]
	partial class LoginPanelController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField HashCode { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton AllowancePopup { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator ActivityIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton ViewUsageButton { get; set; }

		[Action ("WhatIsHashCodeClicked:")]
		partial void WhatIsHashCodeClicked (MonoMac.AppKit.NSButton sender);

		[Action ("LoginClicked:")]
		partial void LoginClicked (MonoMac.AppKit.NSButton sender);

		[Action ("QuitClicked:")]
		partial void QuitClicked (MonoMac.AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (HashCode != null) {
				HashCode.Dispose ();
				HashCode = null;
			}

			if (AllowancePopup != null) {
				AllowancePopup.Dispose ();
				AllowancePopup = null;
			}

			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (ViewUsageButton != null) {
				ViewUsageButton.Dispose ();
				ViewUsageButton = null;
			}
		}
	}

	[Register ("LoginPanel")]
	partial class LoginPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
