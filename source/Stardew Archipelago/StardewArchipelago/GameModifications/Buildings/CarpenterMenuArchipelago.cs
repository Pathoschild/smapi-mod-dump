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
using StardewArchipelago.Locations.CodeInjections;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.Buildings
{
    public class CarpenterMenuArchipelago : BuildingMenuArchipelago
    {
        public CarpenterMenuArchipelago(ArchipelagoClient archipelago) : base(archipelago)
        {
        }

        public CarpenterMenuArchipelago(IModHelper modHelper, ArchipelagoClient archipelago) : base(modHelper, archipelago, false)
        {
        }

        public override List<BluePrint> GetAvailableBlueprints()
        {
            var blueprints = new List<BluePrint>();

            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_COOP);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_BARN);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_WELL);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_SILO);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_MILL);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_SHED);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_FISH_POND);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_STABLE, true);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_SLIME_HUTCH);

            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_BIG_COOP, requiredBuilding: CarpenterInjections.BUILDING_COOP);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_DELUXE_COOP, requiredBuilding: CarpenterInjections.BUILDING_BIG_COOP);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_BIG_BARN, requiredBuilding: CarpenterInjections.BUILDING_BARN);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_DELUXE_BARN, requiredBuilding: CarpenterInjections.BUILDING_BIG_BARN);
            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_BIG_SHED, requiredBuilding: CarpenterInjections.BUILDING_SHED);

            AddBuildingBlueprintIfReceived(blueprints, CarpenterInjections.BUILDING_SHIPPING_BIN);
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
