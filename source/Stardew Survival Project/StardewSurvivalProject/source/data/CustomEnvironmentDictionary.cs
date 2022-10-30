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
using System.IO;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace StardewSurvivalProject.source.data
{
    public class LocationEnvironmentData
    {
        public String name { get; set; } = "";
        public double tempModifierAdditive { get; set; } = 0;
        public double tempModifierMultiplicative { get; set; } = 1;
        public double tempModifierFixedValue { get; set; } = -274; //ignore value
        public double tempModifierTimeDependentScale { get; set; } = 3;
        public double tempModifierFluctuationScale { get; set; } = 1;
        public int area { get; set; } = 400;
    }

    public class CustomEnvironmentDictionary
    {
        public static Dictionary<string, LocationEnvironmentData> value_list = new Dictionary<string, LocationEnvironmentData>();

        public static void loadList(Mod context)
        {
            String RelativePath = Path.Combine(context.Helper.DirectoryPath, "locationEnvironmentData.json");
            String jsonData = File.ReadAllText(RelativePath);
            LocationEnvironmentData[] tempArray = JsonConvert.DeserializeObject<LocationEnvironmentData[]>(jsonData);

            if (tempArray == null)
            {
                LogHelper.Warn("No environment entry is found");
                return;
            }
            for (int i = 0; i < tempArray.Length; i++)
                value_list.Add(tempArray[i].name, tempArray[i]);
            LogHelper.Debug("Environment list loaded");
        }

        public static LocationEnvironmentData GetEnvironmentData(string locationName)
        {
            if (value_list.ContainsKey(locationName))
            {
                return value_list[locationName];
            }
            else
            {
                return null;
            }
        }
    }
}
