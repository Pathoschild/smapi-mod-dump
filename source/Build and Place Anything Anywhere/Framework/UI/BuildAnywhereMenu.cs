/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;

namespace AnythingAnywhere.Framework.UI
{
    internal sealed class BuildAnywhereMenu(string builder, GameLocation? targetLocation = null) : CarpenterMenu(builder, targetLocation)
    {
        // Prevents Better Juminos from spamming errors
        #pragma warning disable CS0649
        public bool magicalConstruction;

        // Check if a building is valid for a location
        public override bool IsValidBuildingForLocation(string typeId, BuildingData data, GameLocation targetLocation)
        {
            return typeId != "Cabin" || TargetLocation.Name == "Farm" || !Game1.IsMultiplayer;
        }
    }
}