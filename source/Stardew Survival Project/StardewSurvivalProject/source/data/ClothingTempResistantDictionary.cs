/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewSurvivalProject.source.data
{
    //TODO: This is very ineffecient to iterate (espeically on rendering hover text), should read every clothing info
    //on game load and add it to a dictionary by the rule dictated in the json file
    public class ClothingTempResistantData
    {
        public string name { get; set; } = "";
        //matching pattern (match, prefix, postfix, contain)
        public string pattern { get; set; } = "match";
        public double heatInsulationModifier = 0;
        public double coldInsulationModifier = 0;
    }
    public class ClothingTempResistantDictionary
    {
        public static LoadedClothingData data = null;

        public class LoadedClothingData
        {
            public ClothingTempResistantData[] hat_data ;
            public ClothingTempResistantData[] shirt_data ;
            public ClothingTempResistantData[] pants_data ;
            public ClothingTempResistantData[] boots_data ;

        }

        public static void loadList(Mod context)
        {
            String RelativePath = Path.Combine(context.Helper.DirectoryPath, "clothingTempResistantData.json");
            String jsonData = File.ReadAllText(RelativePath);

            data = JsonConvert.DeserializeObject<LoadedClothingData>(jsonData);

            if (data == null)
            {
                LogHelper.Warn("No clothing entry is found");
                return;
            }

            LogHelper.Debug("Clothing list loaded");
        }

        public static ClothingTempResistantData GetClothingData(string clothingName, string type = "")
        {
            if (type.Equals("hat"))
                return GetDataByIteration(clothingName, data.hat_data);
            else if (type.Equals("shirt"))
                return GetDataByIteration(clothingName, data.shirt_data);
            else if (type.Equals("pants"))
                return GetDataByIteration(clothingName, data.pants_data);
            else if (type.Equals("boots"))
                return GetDataByIteration(clothingName, data.boots_data);

            return null;
        }

        //TODO: modify the data structure
        public static ClothingTempResistantData GetDataByIteration(string clothingName, ClothingTempResistantData[] arr)
        {
            ClothingTempResistantData res = null;
            foreach (ClothingTempResistantData x in arr)
            {
                if (x.pattern.Equals("match"))
                    if (clothingName.Equals(x.name)) { res = x; break; }
                if (x.pattern.Equals("prefix"))
                    if (clothingName.StartsWith(x.name)) { res = x; break; }
                if (x.pattern.Equals("postfix"))
                    if (clothingName.EndsWith(x.name)) { res = x; break; }
                if (x.pattern.Equals("contain"))
                    if (clothingName.Contains(x.name)) { res = x; break; }
            }
            return res;
        }
    }
}
