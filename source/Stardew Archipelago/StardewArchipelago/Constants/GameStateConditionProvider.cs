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
using System.Reflection;
using StardewValley;

namespace StardewArchipelago.Constants
{
    public static class GameStateConditionProvider
    {
        public static readonly string HAS_RECEIVED_ITEM = CreateId("HasReceivedItem");
        public static readonly string HAS_STOCK_SIZE = CreateId("HasCartStockSize");
        public static readonly string FOUND_ARTIFACT = CreateId("FoundArtifact");
        public static readonly string FOUND_MINERAL = CreateId("FoundMineral");

        public static string CreateHasReceivedItemCondition(string itemName, int amount = 1)
        {
            if (amount < 1)
            {
                return string.Empty;
            }

            var arguments = new[] { amount.ToString(), itemName };
            return CreateCondition(HAS_RECEIVED_ITEM, arguments);
        }

        public static string CreateHasBuildingAnywhereCondition(string buildingName, bool hasBuilding)
        {
            if (buildingName.Contains(" "))
            {
                buildingName = $"\"{buildingName}\"";
            }

            if (hasBuilding)
            {
                return $"BUILDINGS_CONSTRUCTED ALL {buildingName} 1";
            }
            return $"BUILDINGS_CONSTRUCTED ALL {buildingName} 0 0";
        }

        public static string CreateHasStockSizeCondition(double minimumStock)
        {
            var arguments = new[] { minimumStock.ToString() };
            return CreateCondition(HAS_STOCK_SIZE, arguments);
        }

        public static string CreateSeasonsCondition(string[] seasons)
        {
            return CreateCondition("SEASON", seasons);
        }

        public static string CreateArtifactsCondition(string[] artifacts)
        {
            return CreateCondition(FOUND_ARTIFACT, artifacts);
        }

        public static string CreateMineralsCondition(string[] minerals)
        {
            return CreateCondition(FOUND_MINERAL, minerals);
        }

        public static string CreateCondition(string condition, string[] arguments)
        {
            return !arguments.Any() ? condition : $"{condition} {string.Join(' ', arguments)}";
        }

        public static string RemoveCondition(string condition, string conditionToRemove)
        {
            return FilterCondition(condition, x => !x.Contains(conditionToRemove));
        }

        public static string FilterCondition(string condition, Func<string, bool> filter)
        {
            return string.Join(',', condition.Split(',').Where(filter));
        }

        private static string CreateId(string name)
        {
            return $"{ModEntry.Instance.ModManifest.UniqueID}.GameStateCondition.{name}";
        }

        private static string CreateId(MemberInfo t)
        {
            return CreateId(t.Name);
        }

        private static string CreateId<T>()
        {
            return CreateId(typeof(T));
        }
    }
}
