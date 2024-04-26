/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace PassableCrops {
    internal class ModEntry : Mod {
        public Config? Config { get; private set; }

        public override void Entry(IModHelper helper) {
            this.Config = Config.Register(helper);
            Patches.Crops.Register(this);
            Patches.Objects.Register(this);
            Patches.Bushes.Register(this);
            Patches.Trees.Register(this);
            Patches.FruitTrees.Register(this);
        }

        public void PlayRustleSound(Vector2 tile, GameLocation location) {
            if ((Config?.ShakeWhenPassing ?? false) && (Config?.PlaySoundWhenPassing ?? false) && Utility.isOnScreen(new Point((int)tile.X, (int)tile.Y), 2, location)) {
                Grass.PlayGrassSound();
            }
        }
    }
}
