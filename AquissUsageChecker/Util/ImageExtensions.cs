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

