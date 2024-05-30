/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SplitscreenImproved.Layout
{
    public class SplitscreenLayoutData
    {
        public SplitscreenLayoutData(byte numScreens, LayoutPreset layoutPreset = LayoutPreset.Default)
        {
            if (numScreens < 1)
            {
                numScreens = 1;
            }

            NumScreens = numScreens;
            ScreenSplits = layoutPreset switch
            {
                LayoutPreset.SwapSides => GetSwapSidesScreenSplits(),
                LayoutPreset.Custom => GetCustomScreenSplits(),
                _ => GetDefaultScreenSplits(),
            };
        }

        public Vector4[] ScreenSplits { get; set; }

        private byte NumScreens { get; }

        private Vector4[] GetDefaultScreenSplits()
        {
            var defaultScreenSplits = new Vector4[NumScreens];

            switch (NumScreens)
            {
                case 1:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 1f);
                    break;
                case 2:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 0.5f, 1f);
                    defaultScreenSplits[1] = new Vector4(0.5f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                default:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[3] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            return defaultScreenSplits;
        }

        private Vector4[] GetSwapSidesScreenSplits()
        {
            var swapScreenSplits = new Vector4[NumScreens];

            switch (NumScreens)
            {
                case 1:
                    swapScreenSplits[0] = new Vector4(0f, 0f, 1f, 1f);
                    break;
                case 2:
                    swapScreenSplits[0] = new Vector4(0.5f, 0f, 0.5f, 1f);
                    swapScreenSplits[1] = new Vector4(0f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    swapScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    swapScreenSplits[1] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    swapScreenSplits[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    break;
                default:
                    swapScreenSplits[0] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
                    swapScreenSplits[1] = new Vector4(0f, 0f, 0.5f, 0.5f);
                    swapScreenSplits[2] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    swapScreenSplits[3] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            return swapScreenSplits;
        }

        // Default values for Custom preset is a horizontal version of the vanilla splitscreen.
        private Vector4[] GetCustomScreenSplits()
        {
            var customScreenSplits = new Vector4[NumScreens];

            switch (NumScreens)
            {
                case 1:
                    customScreenSplits[0] = new Vector4(0f, 0f, 1f, 1f);
                    break;
                case 2:
                    customScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    customScreenSplits[1] = new Vector4(0f, 0.5f, 1f, 0.5f);
                    break;
                case 3:
                    customScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    customScreenSplits[1] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    customScreenSplits[2] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                default:
                    customScreenSplits[0] = new Vector4(0f, 0f, 0.5f, 0.5f);
                    customScreenSplits[1] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
                    customScreenSplits[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    customScreenSplits[3] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            return customScreenSplits;
        }
    }
}
