/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace LightMod
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int AlphaAmount { get; set; } = 50;
        public int Alpha1Amount { get; set; } = 25;
        public int Alpha2Amount { get; set; } = 5;
        public float RadiusAmount { get; set; } = 1;
        public float Radius1Amount { get; set; } = 0.5f;
        public float Radius2Amount { get; set; } = 0.25f;
        public SButton ModButton1 { get; set; } = SButton.LeftShift;
        public SButton ModButton2 { get; set; } = SButton.LeftControl;
        public SButton RadiusModButton { get; set; } = SButton.LeftAlt;
    }
}
