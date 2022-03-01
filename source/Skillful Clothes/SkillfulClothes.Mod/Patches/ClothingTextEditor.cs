/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Patches
{
    class ClothingTextEditor : IAssetEditor
    {        
        string clothingAssetName = "Data\\ClothingInformation";
        string hatAssetName = "Data\\hats";

        public bool CanEdit<T1>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(clothingAssetName) || asset.AssetNameEquals(hatAssetName);
        }

        public void Edit<T1>(IAssetData asset)
        {
            Logger.Debug($"Editing {asset.AssetName}");
            var dict = asset.AsDictionary<int, string>().Data;

            if (asset.AssetNameEquals(clothingAssetName))
            {
                // Shirts and Hats
                UpdateTexts(ItemDefinitions.ShirtEffects, dict, 2);
                UpdateTexts(ItemDefinitions.PantsEffects, dict, 2);
            }
            else
            {
                // Hats
                UpdateTexts(ItemDefinitions.HatEffects, dict, 1);
            }
        }

        private void UpdateTexts<T>(Dictionary<T, ExtItemInfo> itemDefinitions, IDictionary<int, string> dict, int descriptionIndex)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an Enum");
            }

            foreach (var itemDef in itemDefinitions)
            {
                int index = Convert.ToInt32(itemDef.Key);
                if (itemDef.Value.ShouldDescriptionBePatched && dict.ContainsKey(index))
                {                    
                    // replace the item's description text
                    string value = dict[index];
                    var pp = value.Split('/');
                    pp[descriptionIndex] = itemDef.Value.NewItemDescription;                    
                    dict[index] = String.Join("/", pp);
                }
            }
        }
    }
}
