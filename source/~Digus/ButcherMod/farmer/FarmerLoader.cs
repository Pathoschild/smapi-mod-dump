/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using StardewModdingAPI.Utilities;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerLoader
    {
        public static FarmerData FarmerData = null;

        public static void LoadData()
        {
            FarmerData = DataLoader.Helper.Data.ReadJsonFile<FarmerData>($"data/farmers/{Constants.SaveFolderName}.json") ?? new FarmerData();
        }

        public static void SaveData()
        {
            if (Context.IsMainPlayer)
            {
                DataLoader.Helper.Data.WriteJsonFile<FarmerData>($"data/farmers/{Constants.SaveFolderName}.json", FarmerData);
            }
        }
    }
}
