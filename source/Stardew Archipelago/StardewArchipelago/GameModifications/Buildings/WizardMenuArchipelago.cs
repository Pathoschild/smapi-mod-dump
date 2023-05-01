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
    public class WizardMenuArchipelago : BuildingMenuArchipelago
    {
        public WizardMenuArchipelago(IModHelper modHelper, ArchipelagoClient archipelago) : base(modHelper, archipelago, true)
        {
        }

        public override List<BluePrint> GetAvailableBlueprints()
        {
            var blueprints = new List<BluePrint>();

            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_EARTH_OBELISK);
            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_WATER_OBELISK);
            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_DESERT_OBELISK);
            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_ISLAND_OBELISK);
            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_JUNIMO_HUT);
            AddBuildingBlueprintIfReceived(blueprints, WizardInjections.BUILDING_GOLD_CLOCK);

            return blueprints;
        }

        private void AddBuildingBlueprintIfReceived(List<BluePrint> blueprints, string buildingName, bool onlyOne = false, string requiredBuilding = null)
        {
            var hasReceivedBuilding = _archipelago.HasReceivedItem(buildingName,  out var sendingPlayer);
            if (!hasReceivedBuilding)
            {
                return;
            }

            AddBuildingBlueprint(blueprints, buildingName, sendingPlayer, onlyOne, requiredBuilding);
        }
    }
}
