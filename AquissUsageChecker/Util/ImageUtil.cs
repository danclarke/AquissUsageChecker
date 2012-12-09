using System;
using System.Drawing;
using MonoMac.CoreImage;

namespace AquissUsageChecker.Util
{
	public static class ImageUtil
	{
		public static CIColor TransformHSV(CIColor input, double hue, double saturation, double value)
		{
			double VSU = value * saturation * Math.Cos(hue * Math.PI / 180.0);
			double VSW = value * saturation * Math.Sin(hue * Math.PI / 180.0);

			double red = (.299 * value + 0.701 * VSU + 0.168 * VSW) * input.Red
				  	   + (0.587 * value - 0.587 * VSU + 0.330 * VSW) * input.Green
				  	   + (0.114 * value - 0.114 * VSU - 0.497 * VSW) * input.Blue;

			double green = (0.299 * value - 0.299 * VSU - 0.328 * VSW) * input.Red
				   	  	 + (0.587 * value + 0.413 * VSU + 0.035 * VSW) * input.Green
				  	  	 + (0.114 * value - 0.114 * VSU + 0.292 * VSW) * input.Blue;

			double blue = (0.299 * value - 0.3 * VSU + 0.125 * VSW) * input.Red
				  	   	+ (0.587 * value - 0.588 * VSU - 1.05 * VSW) * input.Green
				  	   	+ (0.114 * value + 0.886 * VSU - 0.203 * VSW) * input.Blue;

			if (red > 1.0)
				red = 1.0;
			if (red < 0.0)
				red = 0.0;
			
			if (green > 1.0)
				green = 1.0;
			if (green < 0.0)
				green = 0.0;
			
			if (blue > 1.0)
				blue = 1.0;
			if (blue < 0.0)
				blue = 0.0;

			var returnValue = CIColor.FromRgb((float)red, (float)green, (float)blue);

			return returnValue;
		}
	}
}

