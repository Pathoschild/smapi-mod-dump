using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Microsoft.Xna.Framework.Graphics;
using WColor = System.Drawing.Color;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public sealed class DominantColorFinder
    {
        private static readonly Dictionary<int, XColor> ColorCache = new Dictionary<int, XColor>();

        public static unsafe XColor GetDominantColor(int spriteIndex, Texture2D texture, int width, int height)
        {
            XColor cachedColor;
            if (ColorCache.TryGetValue(spriteIndex, out cachedColor)) return cachedColor;

            var rect = TextureInfo.Default[TextureType.Items].GetSourceRect(spriteIndex);
            var size = width * height;
            var data = new XColor[size];
            texture.GetData(0, rect, data, 0, data.Length);

            const byte empty = 0;
            const byte inner = 1;
            const byte edge = 2;

            var alpha = new byte[size];
            var colors = new Dictionary<int, int>();
            fixed (byte* pAlpha0 = alpha)
            fixed (XColor* pData0 = data)
            {
                var pAlpha = pAlpha0;
                var pData = pData0;

                for (var i = 0; i < size; ++i)
                {
                    var c = *pData;
                    *pAlpha = (c.A >= 128)? inner : empty;

                    ++pData;
                    ++pAlpha;
                }

                pAlpha = pAlpha0 + width;

                var hi = height - 1;
                var wi = width - 1;
                for (var y = 1; y < hi; ++y)
                {
                    ++pAlpha;
                    for (var x = 1; x < wi; ++x, ++pAlpha)
                    {
                        if (*(pAlpha - width) == empty || *(pAlpha + width) == empty || *(pAlpha - 1) == empty || *(pAlpha + width) == empty)
                        {
                            *pAlpha = edge;
                        }
                    }
                    ++pAlpha;
                }

                pAlpha = pAlpha0 + width;
                pData = pData0 + width;

                for (var y = 1; y < hi; ++y)
                {
                    ++pAlpha;
                    ++pData;

                    for (var x = 1; x < wi; ++x, ++pAlpha, ++pData)
                    {
                        if (*pAlpha != 1) continue;

                        var xColor = *pData;
                        var c = WColor.FromArgb(xColor.R, xColor.G, xColor.B);
                        var l = c.GetBrightness();
                        if (l <= 0.15f || 0.85f <= l) continue;

                        var s = c.GetSaturation();
                        if (s <= 0.25f) continue;

                        var h = c.GetHue();
                        var kh = (int)((h + 5) / 10); // 0..36
                        var ks = (int)((s + 0.17f) * 3); // 0..3
                        var kl = (int)((l + 0.17f) * 3); // 0..3
                        var key = (kh << 4) | (ks << 2) | kl;

                        int value;
                        if (colors.TryGetValue(key, out value))
                        {
                            colors[key] = value + 1;
                        }
                        else
                        {
                            colors.Add(key, 1);
                        }
                    }
                }
            }

            if (!colors.Any())
            {
                Log.Error("No dominant color found!");
                cachedColor = XColor.Gray;
            }
            else
            {
                var dominantColors = colors.OrderByDescending(x => x.Value).Take(3).ToArray();
                var dominantColor = dominantColors[0];

                if (IsGreenColor(dominantColor))
                {
                    var notGreenColors = dominantColors.Where(c => !IsGreenColor(c)).Take(2).ToArray();
                    if (notGreenColors.Any())
                    {
                        var altColor1 = notGreenColors[0];
                        var altColor2 = (notGreenColors.Length > 2)? notGreenColors[1] : altColor1;

                        if ((altColor1.Value + altColor2.Value) * 4 >= dominantColor.Value * 3)
                        {
                            dominantColor = altColor1;
                        }
                    }
                }

                var dominantColorKey = dominantColor.Key;
                var finalH = Math.Min(Math.Max(0, (dominantColorKey >> 4) * 10 - 5), 359);
                var finalL = Math.Min(Math.Max(0, 0.40 + (dominantColorKey & 3) * 0.1), 1);
                cachedColor = RawColor.FromHSL(finalH, 0.80, finalL).ToXnaColor();
            }

            ColorCache.Add(spriteIndex, cachedColor);
            return cachedColor;
        }

        private static bool IsGreenColor(KeyValuePair<int, int> colorInfo)
        {
            var h = colorInfo.Key >> 4;
            return (9 <= h && h <= 15);
        }
    }
}
