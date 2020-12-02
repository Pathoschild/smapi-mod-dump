/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/ImagEd
**
*************************************************/

using System;
using System.Linq;

using Microsoft.Xna.Framework;


namespace ImagEd.Framework {
    /// <summary>
    /// This class provides several algorithms to desaturate RGB.
    /// See https://en.wikipedia.org/wiki/HSL_and_HSV for details.
    /// </summary>
    internal class Desaturation {
        public enum Mode {
            // No desaturation.
            None,
            // HSV method (max(R, G, B))
            DesaturateHSV,
            // HSL method ((max(R, G, B) + min(R, G, B)) / 2)
            DesaturateHSL,
            // HSI method ((R + G + B) / 3)
            DesaturateHSI,
            // Luma (SDTV) method. (0.3 * R + 0.59 * G + 0.11 * B)
            DesaturateLuma
        }

        public static Mode ParseEnum(string desaturation) {
            return Enum.TryParse<Mode>(desaturation, true, out Mode mode) ? mode : Mode.None;
        }

        /// <summary>
        /// Applies the algorithm on the given pixel.
        /// </summary>
        public static Color Desaturate(Color pixel, Mode mode) {
            // Several methods to calculate lightness.
            // https://en.wikipedia.org/wiki/HSL_and_HSV
            if (mode == Mode.DesaturateHSV) {
                // HSV method (max(R, G, B))
                byte max = (new[] { pixel.R, pixel.G, pixel.B }).Max();

                return new Color(max, max, max);
            }
            else if (mode == Mode.DesaturateHSL) {
                // HSL method ((max(R, G, B) + min(R, G, B)) / 2)
                int max = (new[] { pixel.R, pixel.G, pixel.B }).Max();
                int min = (new[] { pixel.R, pixel.G, pixel.B }).Min();
                int mid = (byte) ((max + min) / 2);

                return new Color(mid, mid, mid);
            }
            else if (mode == Mode.DesaturateHSI) {
                // HSI method ((R + G + B) / 3)
                byte avg = (byte) ((pixel.R + pixel.G + pixel.B) / 3);

                return new Color(avg, avg, avg);
            }
            else if (mode == Mode.DesaturateLuma) {
                // Luma (SDTV) method. (0.3 * R + 0.59 * G + 0.11 * B)
                byte luma = (byte) (0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);

                return new Color(luma, luma, luma);
            }
            else {
                return pixel;
            }
        }
    }
}
