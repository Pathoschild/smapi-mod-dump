/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;

namespace StardewArchipelago.GameModifications.Buildings
{   
    public abstract class BuildingMenuArchipelago : CarpenterMenu
    {
        private List<string> ExcludedBuildings = new List<string>{
            "Stone Cabin", "Plank Cabin", "Log Cabin", "Greenhouse", "Mine Elevator"
        };

        private const string _stableName = "Stable";
        private const string _tractorGarageName = "Tractor Garage";
        private const int _tractorMaxOccupantValue = -794739;

        protected IModHelper _modHelper;
        protected ArchipelagoClient _archipelago;
        private readonly IReflectedField<List<BluePrint>> _blueprintsField;
        private bool _hasCleanedGarage = false;

        protected BuildingMenuArchipelago(IModHelper modHelper, ArchipelagoClient archipelago, bool magicalConstruction) : base(magicalConstruction)
        {
            _modHelper = modHelper;
            _archipelago = archipelago;

            _blueprintsField = _modHelper.Reflection.GetField<List<BluePrint>>(this, "blueprints");
            var blueprints = GetAvailableBlueprints();

            if (blueprints == null || !blueprints.Any())
            {
                return;
            }

            _blueprintsField.SetValue(blueprints);

            setNewActiveBlueprint();
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            snapToDefaultClickableComponent();

            _hasCleanedGarage = false;
        }

        // This override exists pretty much only because the Tractor Garage tries to sneakily add the real garage and stable for no reason, AFTER initializing this menu
        public override void update(GameTime time)
        {
            base.update(time);

            CleanExtraStables();
        }

        private void CleanExtraStables()
        {
            if (_hasCleanedGarage)
            {
                return;
            }
            
            var blueprints = _blueprintsField.GetValue();
            for (var i = blueprints.Count - 1; i >= 0; i--)
            {
                if (blueprints[i].name != _stableName && blueprints[i].name != _tractorGarageName)
                {
                    continue;
                }

                if (!blueprints[i].displayName.Contains("Free", StringComparison.OrdinalIgnoreCase))
                {
                    blueprints.RemoveAt(i);
                }
            }

            _hasCleanedGarage = true;
        }

        public abstract List<BluePrint> GetAvailableBlueprints();

        public List<BluePrint> FullBlueprintData()
        {
            var fullBlueprintData = new List<BluePrint>();
            var rawBlueprintData = Game1.content.Load<Dictionary<string, string>>("Data\\blueprints");
            foreach (var blueprintPair in rawBlueprintData)
            {
                if (ExcludedBuildings.Contains(blueprintPair.Key))
                {
                    continue;
                }
                var blueprintDataArray = blueprintPair.Value.Split('/');
                if (blueprintDataArray[0] == "animal")
                {
                    continue;
                }
                fullBlueprintData.Add(new BluePrint(blueprintPair.Key));
            }
            
            return fullBlueprintData;
        }

        protected virtual void AddBuildingBlueprint(List<BluePrint> blueprints, string buildingName, string sendingPlayer, bool onlyOne = false, string requiredBuilding = null)
        {
            var farm = Game1.getFarm();
            BluePrint blueprintToAdd = null;
            var isConstructedAlready = IsBuildingConstructed(farm, buildingName);
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
                //Tractor Mod implementation utilizes an odd building type...
                if (buildingName == _tractorGarageName)
                {
                    blueprintToAdd = CreateTractorGarageBlueprint(false);

                }
                else
                {
                    blueprintToAdd = new BluePrint(buildingName);
                }

            }
            else
            {
                if (buildingName == _tractorGarageName)
                {
                    blueprintToAdd = CreateTractorGarageBlueprint(true, sendingPlayer);
                }
                else
                {
                    blueprintToAdd = new FreeBlueprint(buildingName, sendingPlayer);
                }

            }
            blueprints.Add(blueprintToAdd);
        }

        private static BluePrint CreateTractorGarageBlueprint(bool free, string sendingPlayerName = null)
        {
            const string tractorDescription = "A garage to store your tractor. Tractor included!";
            var garageBlueprint = new BluePrint(_stableName)
            {
                displayName = _tractorGarageName,
                description = tractorDescription,
                moneyRequired = 150000,
                itemsRequired = new Dictionary<int, int>() { { 355, 20 }, { 337, 5 }, { 787, 5 } },
            };

            if (free)
            {
                var freeGarageBlueprint = new FreeBlueprint(_stableName, sendingPlayerName);
                freeGarageBlueprint.SetDisplayFields(_tractorGarageName, tractorDescription, sendingPlayerName);
                garageBlueprint = freeGarageBlueprint;
            }

            garageBlueprint.maxOccupants = _tractorMaxOccupantValue;
            garageBlueprint.tilesWidth = 4;
            garageBlueprint.tilesHeight = 2;
            garageBlueprint.sourceRectForMenuView = new Rectangle(0, 0, 64, 96);
            garageBlueprint.magical = false;
            return garageBlueprint;
        }

        private bool IsBuildingConstructed(Farm farm, string name)
        {
            foreach (var building in farm.buildings)
            {
                if (IsCorrectType(building, name) && building.daysOfConstructionLeft.Value <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsCorrectType(Building building, string name)
        {
            if (name == _stableName || name == _tractorGarageName)
            {
                return building.buildingType.Value.Equals(_stableName, StringComparison.OrdinalIgnoreCase) && IsCorrectTypeOfStable(building, name);
            }

            return building.buildingType.Value.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCorrectTypeOfStable(Building building, string name)
        {
            if (name.Equals(_stableName, StringComparison.OrdinalIgnoreCase) && building.maxOccupants.Value != _tractorMaxOccupantValue)
            {
                return true;
            }

            if (name.Equals(_tractorGarageName, StringComparison.OrdinalIgnoreCase) && building.maxOccupants.Value == _tractorMaxOccupantValue)
            {
                return true;
            }

            return false;
        }
    }
}
