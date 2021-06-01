/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Outerwear.Models.Converted;
using StardewValley;

namespace Outerwear
{
    /// <summary>Provides basic outerwear apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Gets whether a specified object id is an outerwear.</summary>
        /// <param name="objectId">The id of the object to check.</param>
        /// <returns><see langword="true"/>, if the object is an outerwear; otherwise, <see langword="false"/>.</returns>
        public bool IsOuterwear(int objectId);

        /// <summary>Gets the data of an outerwear with a specified object id.</summary>
        /// <param name="objectId">The object id of the outerwear data to get.</param>
        /// <returns>The outerwear data with an object id of <paramref name="objectId"/>, if one exists; otherwise, <see langword="null"/>.</returns>
        public OuterwearData GetOuterwearData(int objectId);

        /// <summary>Gets the equipped outerwear.</summary>
        /// <returns>The outerwear item that's equipped, if an outerwear is equipped; otherwise, <see langword="null"/>.</returns>
        public Item GetEquippedOuterwear();

        /// <summary>Gets the id of the equipped outerwear.</summary>
        /// <param name="farmer">The farmer to get the equipped outerwear id of, if <see langword="null"/> is specified, <see cref="Game1.player"/> is used.</param>
        /// <returns>The id of the outerwear currently equipped by <paramref name="player"/>, if one is equipped; otherwise, <see langword="null"/>.</returns>
        public int GetEquippedOuterwearId(Farmer player = null);

        /// <summary>Equips an outerwear.</summary>
        /// <param name="outerwear">The outerwear item to equip.</param>
        public void EquipOuterwear(Item outerwear);

        /// <summary>Unequips the currently equip outerwear.</summary>
        public void UnequipOuterwear();
    }
}
