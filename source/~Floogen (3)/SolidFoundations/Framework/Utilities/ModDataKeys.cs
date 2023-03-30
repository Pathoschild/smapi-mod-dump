/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Utilities
{
    internal class ModDataKeys
    {
        // Core keys
        internal const string GENERIC_BLUEPRINT = "SolidFoundations.GenericBlueprint";
        internal const string GENERIC_BUILDING_ID = "SolidFoundations.GenericBuilding.Id";
        internal const string LOCATION_CUSTOM_BUILDINGS = "SolidFoundations.Location.CustomBuildings";
        internal const string CUSTOM_CHEST_CAPACITY = "SolidFoundations.Chest.Capacity";

        // Messages
        internal const string MESSAGE_BUILDING_BUILT = "SolidFoundations.Messages.BuildingBuilt";

        // Flags
        internal const string FLAG_BASE = "SolidFoundations.Flag";
    }
}
