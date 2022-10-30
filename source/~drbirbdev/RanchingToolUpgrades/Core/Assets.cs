/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared.Asset;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace RanchingToolUpgrades
{
    [AssetClass]
    public class Assets
    {
        [AssetProperty("assets/sprites.png", Priority = AssetLoadPriority.Medium)]
        public Texture2D Sprites { get; set; }
        public string SpritesPath { get; set; }
    }
}
