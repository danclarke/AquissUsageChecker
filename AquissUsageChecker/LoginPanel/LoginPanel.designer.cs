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
		MonoMac.AppKit.NSTextField UsernameField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSecureTextField PasswordField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSComboBox AllowanceCombo { get; set; }

		[Action ("LoginClicked:")]
		partial void LoginClicked (MonoMac.AppKit.NSButton sender);

		[Action ("QuitClicked:")]
		partial void QuitClicked (MonoMac.AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (UsernameField != null) {
				UsernameField.Dispose ();
				UsernameField = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}

			if (AllowanceCombo != null) {
				AllowanceCombo.Dispose ();
				AllowanceCombo = null;
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
