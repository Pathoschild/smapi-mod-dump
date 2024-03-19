/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using System.IO;
using Newtonsoft.Json;

namespace StardewSurvivalProject.source.data
{
    public class HungerItemData
    {
        public String name { get; set; } = "";
        public double value { get; set; } = 1;
        public double coolingModifier { get; set; } = 1;
    }

    public class CustomHungerDictionary
    {
        //load a whitelist of item that can be used to heal player (healing value is separated from edibility)
        public static Dictionary<String, HungerItemData> value_list = new Dictionary<string, HungerItemData>();
        public static Dictionary<String, HungerItemData> wildcard_value_list = new Dictionary<string, HungerItemData>();

        public static void loadList(Mod context)
        {
            String RelativePath = Path.Combine("customHungerData.json");
            HungerItemData[] tempArray = context.Helper.Data.ReadJsonFile<HungerItemData[]>(RelativePath);
            if (tempArray == null)
            {
                LogHelper.Warn("No hunger item entry is found");
                return;
            }
            for (int i = 0; i < tempArray.Length; i++)
                if (tempArray[i].name.Contains("*"))
                    wildcard_value_list.TryAdd(tempArray[i].name, tempArray[i]);
                else
                    value_list.TryAdd(tempArray[i].name, tempArray[i]);
            LogHelper.Debug("Hunger Item Data loaded");
        }

        public static (double, double) getHungerModifierAndCoolingModifierValue(StardewValley.Object item, bool isDrinkable)
        {
            HungerItemData data = getItemData(item.Name);
            double addHunger = ((item.Edibility >= 0) ? (item.Edibility * ModConfig.GetInstance().HungerGainMultiplierFromItemEdibility) + (item.Quality / 2.5 * item.Edibility) : 0) * (data?.value ?? 1);

            if (isDrinkable)
            {
                // TODO: make this configurable
                addHunger /= 2;
            }

            return (addHunger, data?.coolingModifier ?? 0);
        }

        public static HungerItemData getItemData(string itemName)
        {
            if (value_list.ContainsKey(itemName))
            {
                return value_list[itemName];
            }
            else
            {
                // iterate through wildcard list
                foreach (KeyValuePair<string, HungerItemData> entry in wildcard_value_list)
                {
                    // contain match
                    if (entry.Key.StartsWith("*") && entry.Key.EndsWith("*") && itemName.Contains(entry.Key.Substring(1, entry.Key.Length - 2)))
                    {
                        return entry.Value;
                    }
                    // prefix match
                    else if (entry.Key.StartsWith("*") && itemName.EndsWith(entry.Key.Substring(1)))
                    {
                        return entry.Value;
                    }
                    // postfix match
                    else if (entry.Key.EndsWith("*") && itemName.StartsWith(entry.Key.Substring(0, entry.Key.Length - 1)))
                    {
                        return entry.Value;
                    }
                }
                return null;
            }
        }
    }
}
