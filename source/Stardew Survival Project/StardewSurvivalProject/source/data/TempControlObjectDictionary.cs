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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSurvivalProject.source.data
{
    public class TempControlObject
    {
        public string name { get; set; } = "";
        public double coreTemp { get; set; } = 0.0;
        //in tile
        public double effectiveRange { get; set; } = 1.0;
        public bool needActive { get; set; } = false;
        //0 for normal detection (for furniture), 1 for machine detection (need power)
        public int activeType { get; set; } = 0;
        //ambient temp manipulation
        public double ambientCoefficient { get; set; } = 1;
        
        public string deviceType { get; set; } = "general";
        //operational range, only valid with general devices
        public double operationalRange { get; set; } = 0;
    }
    public class TempControlObjectDictionary
    {
        public static Dictionary<string, TempControlObject> value_list = new Dictionary<string, TempControlObject>();
        public static double maxEffectiveRange = 0;

        public static void loadList(Mod context)
        {
            String RelativePath = Path.Combine(context.Helper.DirectoryPath, "tempControlObjectData.json");
            String jsonData = File.ReadAllText(RelativePath);
            TempControlObject[] tempArray = JsonConvert.DeserializeObject<TempControlObject[]>(jsonData);

            if (tempArray == null)
            {
                LogHelper.Warn("No temperature control entry is found");
                return;
            }
            for (int i = 0; i < tempArray.Length; i++)
            {
                value_list.Add(tempArray[i].name, tempArray[i]);
                if (tempArray[i].effectiveRange > maxEffectiveRange) maxEffectiveRange = tempArray[i].effectiveRange;
            }
            LogHelper.Debug("Temperature control list loaded");
        }

        public static TempControlObject GetTempControlData(string objectName)
        {
            if (value_list.ContainsKey(objectName))
            {
                return value_list[objectName];
            }
            else
            {
                return null;
            }
        }
    }
}
