/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using MoonShared.Asset;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace ShovelToolUpgrades
{
    [AssetClass]
    public class Assets
    {
        [AssetProperty("assets/tool_sprites.png", Priority = AssetLoadPriority.Medium)]
        public Texture2D ToolSprites { get; set; }
        public string ToolSpritesPath { get; set; }
    }
}
