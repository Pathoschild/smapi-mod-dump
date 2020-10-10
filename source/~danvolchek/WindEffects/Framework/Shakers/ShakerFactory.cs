/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;

namespace WindEffects.Framework.Shakers
{
    internal class ShakerFactory
    {
        public bool TryGetShaker(TerrainFeature feature, Wave wave, GameLocation location, out IShaker shaker)
        {
            shaker = null;

            if (feature is Grass grass)
            {
                shaker = new GrassShaker(grass, wave.IsLeft());
            }

            if (feature is Tree tree)
            {
                shaker = new TreeShaker(tree, location, wave.IsLeft());
            }

            if (feature is FruitTree fruitTree)
            {
                shaker = new FruitTreeShaker(fruitTree, wave.IsLeft());
            }

            if (feature is HoeDirt dirt)
            {
                shaker = new HoeDirtShaker(dirt, wave.IsLeft());
            }

            if (feature is Bush bush)
            {
                shaker = new BushShaker(bush, wave.IsLeft());
            }

            return shaker != null;
        }
    }
}
