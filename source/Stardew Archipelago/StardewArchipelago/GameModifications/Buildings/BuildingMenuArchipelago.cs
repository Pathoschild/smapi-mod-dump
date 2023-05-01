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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications.Buildings
{
    public abstract class BuildingMenuArchipelago : CarpenterMenu
    {
        protected IModHelper _modHelper;
        protected ArchipelagoClient _archipelago;

        public BuildingMenuArchipelago(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
        }

        protected BuildingMenuArchipelago(IModHelper modHelper, ArchipelagoClient archipelago, bool magicalConstruction) : base(magicalConstruction)
        {
            _modHelper = modHelper;
            _archipelago = archipelago;

            var blueprintsField = _modHelper.Reflection.GetField<List<BluePrint>>(this, "blueprints");
            var blueprints = GetAvailableBlueprints();

            if (blueprints == null || !blueprints.Any())
            {
                return;
            }

            blueprintsField.SetValue(blueprints);

            setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public abstract List<BluePrint> GetAvailableBlueprints();

        protected virtual void AddBuildingBlueprint(List<BluePrint> blueprints, string buildingName, string sendingPlayer, bool onlyOne = false, string requiredBuilding = null)
        {
            var farm = Game1.getFarm();
            var isConstructedAlready = farm.isBuildingConstructed(buildingName);
            if (onlyOne && isConstructedAlready)
            {
                return;
            }

            if (requiredBuilding != null)
            {
                var requiredBuildingExists = farm.isBuildingConstructed(requiredBuilding);
                if (!requiredBuildingExists)
                {
                    return;
                }
            }

            var shouldBePaid = isConstructedAlready;
            if (!shouldBePaid && (buildingName.EndsWith("Coop") || buildingName.EndsWith("Barn") || buildingName.EndsWith("Shed")))
            {
                if (buildingName.StartsWith("Big"))
                {
                    shouldBePaid |= farm.isBuildingConstructed(buildingName.Replace("Big", "Deluxe"));
                }
                else
                {
                    shouldBePaid |= farm.isBuildingConstructed($"Big {buildingName}");
                    shouldBePaid |= farm.isBuildingConstructed($"Deluxe {buildingName}");
                }
            }
            if (shouldBePaid)
            {
                blueprints.Add(new BluePrint(buildingName));
            }
            else
            {
                blueprints.Add(new FreeBlueprint(buildingName, sendingPlayer));
            }
        }
    }
}
