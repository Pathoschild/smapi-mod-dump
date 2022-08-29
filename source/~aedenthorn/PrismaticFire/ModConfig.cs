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
using System.Collections.Generic;

namespace PrismaticFire
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public float PrismaticSpeed { get; set; } = 3;
        public string TriggerSound{ get; set; } = "fireball";
        public Color AmethystColor { get; set; } = Color.Purple;
        public Color AquamarineColor { get; set; } = Color.LightBlue;
        public Color EmeraldColor { get; set; } = Color.Green;
        public Color RubyColor { get; set; } = Color.Red;
        public Color TopazColor { get; set; } = Color.Yellow;
        public Color DiamondColor { get; set; } = Color.PaleTurquoise;
    }
}
