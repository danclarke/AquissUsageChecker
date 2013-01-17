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
using System.Drawing;
using MonoMac.CoreImage;

namespace AquissUsageChecker.Util
{
    /// <summary>
    /// Image & colour manipulation methods
    /// </summary>
	public static class ImageUtil
	{
        /// <summary>
        /// Transform a colour's [H]ue, [S]aturation and [V]Value
        /// </summary>
        /// <returns>The transformed colour</returns>
        /// <param name="input">Colour to transform</param>
        /// <param name="hue">Hue adjustment</param>
        /// <param name="saturation">Saturation adjustment</param>
        /// <param name="value">Value adjustment</param>
        /// <remarks>Based on: http://beesbuzz.biz/code/hsv_color_transforms.php</remarks>
		public static CIColor TransformHSV(this CIColor input, double hue, double saturation, double value)
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

