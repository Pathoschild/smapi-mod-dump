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
