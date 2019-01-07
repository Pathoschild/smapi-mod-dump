using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;

namespace AllFishAllSeasons
{
    public class ModEntry : Mod, IAssetEditor
    {
        public override void Entry(IModHelper helper)
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Locations"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Data/Fish"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Fish"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int fishID in data.Keys)
                {
                    string[] fields = data[fishID].Split('/');
                    fields[5] = "600 200";
                    data[fishID] = string.Join("/", fields);
                }
            }

            if (asset.AssetNameEquals("Data/Locations"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int locationsID in data.Keys)
                {
                    string[] fields = data[locationsID].Split('/');
                    fields[4] = "//todo";
                    fields[5] = "//todo";
                    fields[6] = "//todo";
                    fields[7] = "//todo";
                    data[locationsID] = string.Join("/", fields);
                }
            }
        }
    }
}
