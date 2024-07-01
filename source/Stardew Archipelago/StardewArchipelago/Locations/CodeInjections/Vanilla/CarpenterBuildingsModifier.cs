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
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Buildings;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class CarpenterBuildingsModifier
    {
        private const string bigPrefix = "Big ";
        private const string deluxePrefix = "Deluxe ";
        private static readonly string[] _progressiveBuildings = new[] { "Coop", "Barn", "Shed" };
        private static readonly string[] _progressiveBuildingPrefixes = new[] { string.Empty, bigPrefix, deluxePrefix };

        protected static IMonitor _monitor;
        protected static IModHelper _helper;
        protected static ArchipelagoClient _archipelago;

        public CarpenterBuildingsModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
        }

        public void OnBuildingsRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var buildingsData = asset.AsDictionary<string, BuildingData>().Data;
                    ChangePrices(buildingsData);
                    AddFreeBuildings(buildingsData);
                },
                AssetEditPriority.Late
            );
        }

        private void ChangePrices(IDictionary<string, BuildingData> buildingsData)
        {
            var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
            if (Math.Abs(priceMultiplier - 1.0) < 0.01)
            {
                return;
            }

            foreach (var (_, buildingData) in buildingsData)
            {
                var finalCost = (int)Math.Round(buildingData.BuildCost * priceMultiplier);
                buildingData.BuildCost = finalCost;
                if (buildingData.BuildMaterials == null)
                {
                    continue;
                }

                foreach (var buildingMaterial in buildingData.BuildMaterials)
                {
                    var amount = Math.Max(1, (int)Math.Round(buildingMaterial.Amount * priceMultiplier));
                    buildingMaterial.Amount = amount;
                }
            }
        }

        private void AddFreeBuildings(IDictionary<string, BuildingData> buildingsData)
        {
            foreach (var buildingName in buildingsData.Keys.ToArray())
            {
                var buildingData = buildingsData[buildingName];

                if (!_archipelago.SlotData.BuildingProgression.HasFlag(BuildingProgression.Progressive) && !buildingData.MagicalConstruction)
                {
                    continue;
                }

                var freebuildingData = buildingData.DeepClone();
                var archipelagoCondition = GetReceivedBuildingCondition(buildingName);
                var hasBuildingCondition = CreateHasBuildingOrHigherCondition(buildingName, true);
                var doesNotHaveBuildingCondition = CreateHasBuildingOrHigherCondition(buildingName, false);

                freebuildingData.BuildCost = 0;
                freebuildingData.BuildMaterials?.Clear();
                freebuildingData.Description = $"A gift from a friend. {freebuildingData.Description}";
                freebuildingData.BuildCondition = $"{archipelagoCondition}, {doesNotHaveBuildingCondition}";
                buildingData.BuildCondition = $"{archipelagoCondition}, {hasBuildingCondition}";

                buildingsData.Add($"Free {buildingName}", freebuildingData);
            }
        }

        private static string CreateHasBuildingOrHigherCondition(string buildingName, bool hasBuilding)
        {
            var noBuildingConditions = new List<string>();
            noBuildingConditions.Add(GameStateConditionProvider.CreateHasBuildingAnywhereCondition(buildingName, false));

            if (!_progressiveBuildings.Any(x => buildingName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                return ConcatenateConditions(noBuildingConditions, hasBuilding);
            }

            if (buildingName.StartsWith(deluxePrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return ConcatenateConditions(noBuildingConditions, hasBuilding);
            }
            if (buildingName.StartsWith(bigPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                var deluxeBuildingName = buildingName.Replace(bigPrefix, deluxePrefix);
                noBuildingConditions.Add(GameStateConditionProvider.CreateHasBuildingAnywhereCondition(deluxeBuildingName, false));
                return ConcatenateConditions(noBuildingConditions, hasBuilding);
            }
            if (_progressiveBuildings.Contains(buildingName))
            {
                var bigBuildingName = $"{bigPrefix}{buildingName}";
                var deluxeBuildingName = $"{deluxePrefix}{buildingName}";
                noBuildingConditions.Add(GameStateConditionProvider.CreateHasBuildingAnywhereCondition(bigBuildingName, false));
                noBuildingConditions.Add(GameStateConditionProvider.CreateHasBuildingAnywhereCondition(deluxeBuildingName, false));
                return ConcatenateConditions(noBuildingConditions, hasBuilding);
            }

            return ConcatenateConditions(noBuildingConditions, hasBuilding);
        }

        private static string ConcatenateConditions(IReadOnlyList<string> conditions, bool invert)
        {
            if (conditions == null || !conditions.Any())
            {
                return "";
            }

            if (invert)
            {
                if (conditions.Count == 1)
                {
                    return InvertCondition(conditions[0]);
                }

                return $"ANY {string.Join(' ', conditions.Select(x => SurroundWithQuotes(InvertCondition(x))))}";
            }
            else
            {
                return string.Join(", ", conditions);
            }
        }

        private static string InvertCondition(string condition)
        {
            return $"!{condition}";
        }

        private static string SurroundWithQuotes(string condition)
        {
            return $"\"{condition.Replace("\"", "\\\"")}\"";
        }

        private static string GetReceivedBuildingCondition(string buildingName)
        {
            var itemName = buildingName;
            var amount = 1;
            if (buildingName.StartsWith(bigPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                amount = 2;
                itemName = $"Progressive {itemName[bigPrefix.Length..]}";
            }
            else if (buildingName.StartsWith(deluxePrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                amount = 3;
                itemName = $"Progressive {itemName[deluxePrefix.Length..]}";
            }
            else if (_progressiveBuildings.Contains(buildingName))
            {
                itemName = $"Progressive {itemName}";
            }
            else if (_buildingNameReplacements.ContainsKey(buildingName))
            {
                itemName = _buildingNameReplacements[buildingName];
            }
            return GameStateConditionProvider.CreateHasReceivedItemCondition(itemName, amount);
        }

        private static Dictionary<string, string> _buildingNameReplacements = new()
        {
            { "Pathoschild.TractorMod_Stable", "Tractor Garage" }
        };
    }
}
