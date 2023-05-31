/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.Buildings
{
    public class CarpenterMenuArchipelago : BuildingMenuArchipelago
    {

        public CarpenterMenuArchipelago(IModHelper modHelper, ArchipelagoClient archipelago) : base(modHelper, archipelago, false)
        {
        }

        public override List<BluePrint> GetAvailableBlueprints()
        {
            var blueprints = new List<BluePrint>();
            var blueprintData = FullBlueprintData();
            foreach (var blueprint in blueprintData)
            {
                var blueprintMagical = blueprint.magical;
                var blueprintUpgrade = blueprint.nameOfBuildingToUpgrade;

                if (blueprintMagical)
                {
                    continue;
                }
                if (blueprint.name == "Stable")
                {
                    AddBuildingBlueprintIfReceived(blueprints, blueprint.name, true);
                    continue;
                }
                if (blueprintUpgrade == "none")
                {
                    AddBuildingBlueprintIfReceived(blueprints, blueprint.name, requiredBuilding: null);
                    continue;
                }
                AddBuildingBlueprintIfReceived(blueprints, blueprint.name, requiredBuilding: blueprintUpgrade);
            }

            if (_archipelago.SlotData.Mods.HasMod(ModNames.TRACTOR))
            {
                AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_TRACTOR_GARAGE, true);
            }
            return blueprints;
        }

        private void AddBuildingBlueprintIfReceived(List<BluePrint> blueprints, string buildingName, bool onlyOne = false, string requiredBuilding = null)
        {
            var hasReceivedBuilding = CarpenterInjections.HasReceivedBuilding(buildingName, out var sendingPlayer);
            if (!hasReceivedBuilding)
            {
                return;
            }

            AddBuildingBlueprint(blueprints, buildingName, sendingPlayer, onlyOne, requiredBuilding);
        }
    }
}
