/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;

namespace MultiFertilizer.Framework
{
    /// <summary>The metadata for a supported fertilizer.</summary>
    internal class FertilizerData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The fertilizer's object ID.</summary>
        public int Id { get; }

        /// <summary>The <see cref="TerrainFeature.modData"/> key which identifies the fertilizer type.</summary>
        public string Key { get; }

        /// <summary>The fertilizer upgrade level (e.g. 1 = basic fertilizer, 2 = quality fertilizer, 3 = deluxe fertilizer).</summary>
        public int Level { get; }

        /// <summary>The fertilizer's index in the <see cref="Game1.mouseCursors"/> spritesheet.</summary>
        public int SpriteIndex { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">The fertilizer's object ID.</param>
        /// <param name="key">The <see cref="TerrainFeature.modData"/> key which identifies the fertilizer type.</param>
        /// <param name="level">The fertilizer upgrade level (e.g. 1 = basic fertilizer, 2 = quality fertilizer, 3 = deluxe fertilizer).</param>
        /// <param name="spriteIndex">The fertilizer's index in the spritesheet.</param>
        public FertilizerData(int id, string key, int level, int spriteIndex)
        {
            this.Id = id;
            this.Key = key;
            this.Level = level;
            this.SpriteIndex = spriteIndex;
        }
    }
}
