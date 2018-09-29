using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class EventsLoader : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Events\\Town");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\Events\\Town"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                string key = "2677835/z spring summer winter/u 26/t 600 900";
                if (!data.ContainsKey(key))
                {
                    data[key] = "none/-100 -100/farmer 25 65 1 Lewis 28 63 2 dog 29 63 2/faceDirection Dog 2/showFrame Dog 23/specificTemporarySprite animalCompetition/addTemporaryActor White_Chicken 16 16 29 65 0 false Animal farmChicken/showFrame farmChicken 8/skippable/viewport 28 65 true/move farmer 3 0 1/faceDirection farmer 0/speak Lewis \"@! You're here!#$b#Okay... I guess I'd better introduce my pieces. Wish me luck!\"/pause 300/speak Lewis \"Umm... Okay everyone!\"/pause 20000/globalFade/viewport -1000 -1000/end";
                }
            }
        }

        public static void CheckEventDay()
        {
            if (SDate.Now() == AnimalContestController.GetNextContestDate())
            {
                DataLoader.Helper.Content.InvalidateCache("Data\\Events\\Town");
            }
        }
    }
}
