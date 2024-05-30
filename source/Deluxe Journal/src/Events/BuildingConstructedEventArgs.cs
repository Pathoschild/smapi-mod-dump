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
using StardewValley.Buildings;

namespace DeluxeJournal.Events
{
    public class BuildingConstructedEventArgs : EventArgs
    {
        /// <summary>Building location.</summary>
        public GameLocation Location { get; }

        /// <summary>The building being constructed.</summary>
        public Building Building { get; }

        /// <summary>Is this an upgrade? False if the building is new.</summary>
        public bool IsUpgrade { get; }

        /// <summary>The building name after construction is complete.</summary>
        public string NameAfterConstruction => IsUpgrade ? Building.upgradeName.Value : Building.buildingType.Value;

        public BuildingConstructedEventArgs(GameLocation location, Building building, bool isUpgrade)
        {
            Location = location;
            Building = building;
            IsUpgrade = isUpgrade;
        }
    }
}
