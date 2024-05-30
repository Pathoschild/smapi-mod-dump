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
    public class SplitscreenLayout
    {
        public SplitscreenLayout(LayoutPreset layoutPreset = LayoutPreset.Default)
        {
            Preset = layoutPreset;

            TwoPlayerLayout = new SplitscreenLayoutData(2, layoutPreset);
            ThreePlayerLayout = new SplitscreenLayoutData(3, layoutPreset);
            FourPlayerLayout = new SplitscreenLayoutData(4, layoutPreset);
        }

        public SplitscreenLayoutData TwoPlayerLayout { get; set; }

        public SplitscreenLayoutData ThreePlayerLayout { get; set; }

        public SplitscreenLayoutData FourPlayerLayout { get; set; }

        // Singleplayer layout should not be configurable.
        private SplitscreenLayoutData SinglePlayerLayout { get; } = new(1);

        // This Preset property is useful for debugging purposes to determine which layout preset this is for.
        private LayoutPreset Preset { get; }

        public Vector4[] GetScreenSplits(int numScreens)
        {
            if (numScreens < 1)
            {
                numScreens = 1;
            }

            return numScreens switch
            {
                1 => SinglePlayerLayout.ScreenSplits,
                2 => TwoPlayerLayout.ScreenSplits,
                3 => ThreePlayerLayout.ScreenSplits,
                _ => FourPlayerLayout.ScreenSplits,
            };
        }
    }
}
