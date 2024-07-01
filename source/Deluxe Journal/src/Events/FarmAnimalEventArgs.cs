/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewValley.GameData.FarmAnimals;

namespace DeluxeJournal.Events
{
    public class FarmAnimalEventArgs(Farmer player, string farmAnimalType, FarmAnimalData farmAnimalData) : EventArgs
    {
        /// <summary>The player interacting with the <see cref="FarmAnimal"/>.</summary>
        public Farmer Player { get; } = player;

        /// <summary>
        /// The farm animal type. Also the key for the associated data object in
        /// <see cref="Game1.farmAnimalData"/>.
        /// </summary>
        public string FarmAnimalType { get; } = farmAnimalType;

        /// <summary>
        /// The <see cref="StardewValley.GameData.FarmAnimals.FarmAnimalData"/> of the target farm animal.
        /// </summary>
        public FarmAnimalData FarmAnimalData { get; } = farmAnimalData;
    }
}
