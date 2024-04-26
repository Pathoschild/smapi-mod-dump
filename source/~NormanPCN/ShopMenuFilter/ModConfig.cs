/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace ShopMenuFilter
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public Color LabelColor { get; set; } = Color.White; // Color for the text that says "Filter" in the shop menu
    }
}
