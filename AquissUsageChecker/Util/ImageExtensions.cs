using System;

using MonoMac.Foundation;
using MonoMac.CoreImage;
using MonoMac.AppKit;

namespace AquissUsageChecker.Util
{
	public static class ImageExtensions
	{
		public static NSImage ToNSImage(this CIImage image)
		{
			return ToNSImage(image, image.Extent.Size);
		}

		public static NSImage ToNSImage(this CIImage image, System.Drawing.SizeF size)
		{
			var imageRep = NSCIImageRep.FromCIImage(image);
			var nsImage = new NSImage(size);
			nsImage.AddRepresentation(imageRep);
			
			return nsImage;
		}
	}
}

