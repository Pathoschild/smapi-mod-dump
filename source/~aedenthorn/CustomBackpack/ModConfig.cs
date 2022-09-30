/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace CustomBackpack
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public Vector2 BackpackPosition { get; set; } = new Vector2(456f, 1088f);
        public bool ShowRowNumbers { get; set; } = true;
        public bool ShowArrows { get; set; } = true;
        public int MinHandleHeight { get; set; } = 16;
        public Color HandleColor { get; set; } = new Color(233, 84, 32);
        public Color BackgroundColor { get; set; } = new Color(174, 167, 159);
        public SButton ShowExpandedButton { get; set; } = SButton.RightShoulder;
    }
}
