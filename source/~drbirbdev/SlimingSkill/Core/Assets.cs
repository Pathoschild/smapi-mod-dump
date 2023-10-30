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

namespace SlimingSkill
{
    [AssetClass]
    internal class Assets
    {

        [AssetProperty("assets/slimingiconA.png")]
        public Texture2D IconA { get; set; }
        [AssetProperty("assets/slimingiconB.png")]
        public Texture2D IconB { get; set; }

        [AssetProperty("assets/rancher.png")]
        public Texture2D Rancher { get; set; }
        [AssetProperty("assets/breeder.png")]
        public Texture2D Breeder { get; set; }
        [AssetProperty("assets/hatcher.png")]
        public Texture2D Hatcher { get; set; }
        [AssetProperty("assets/hunter.png")]
        public Texture2D Hunter { get; set; }
        [AssetProperty("assets/poacher.png")]
        public Texture2D Poacher { get; set; }
        [AssetProperty("assets/tamer.png")]
        public Texture2D Tamer { get; set; }

        [AssetProperty("assets/rancherP.png")]
        public Texture2D RancherP { get; set; }
        [AssetProperty("assets/breederP.png")]
        public Texture2D BreederP { get; set; }
        [AssetProperty("assets/hatcherP.png")]
        public Texture2D HatcherP { get; set; }
        [AssetProperty("assets/hunterP.png")]
        public Texture2D HunterP { get; set; }
        [AssetProperty("assets/poacherP.png")]
        public Texture2D PoacherP { get; set; }
        [AssetProperty("assets/tamerP.png")]
        public Texture2D TamerP { get; set; }
    }
}
