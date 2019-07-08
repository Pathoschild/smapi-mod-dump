using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Characters;

namespace AdoptSkin.Framework
{
    /// <summary>The representation of a skin within Adopt & Skin.</summary>
    public class AnimalSkin
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The animal, pet, or horse type for the skin.</summary>
        public string CreatureType { get; }

        /// <summary>The unique ID assigned to the skin. This is used code-internally and for user reference.</summary>
        public int ID { get; }

        /// <summary>The internal asset key of the skin, associated with the sprite file within the directory.</summary>
        public string AssetKey { get; }

        /*********
        ** Public methods
        *********/
        public AnimalSkin(string creatureType, int id, string assetKey)
        {
            CreatureType = creatureType;
            ID = id;
            AssetKey = assetKey;
        }

        public class Comparer : IComparer<AnimalSkin>
        {
            public int Compare(AnimalSkin skin1, AnimalSkin skin2)
            {
                return skin1.ID.CompareTo(skin2.ID);
            }
        }
    }
}
