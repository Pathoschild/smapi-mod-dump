/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace TransparentObjects
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool MakeTransparent { get; set; } = true;
        public bool RequireButtonDown { get; set; } = false;
        public SButton ToggleButton { get; set; } = SButton.NumPad0;
        public float MinTransparency { get; set; } = 0.1f;
        public int TransparencyMaxDistance { get; set; } = 192;
        public string[] Exceptions { get; set; } = {
            "Ornamental Hay Bale",
            "Log Section",
            "Campfire",
        };
        public string[] Allowed { get; set; } = new string[0];
    }
}