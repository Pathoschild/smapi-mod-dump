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
    static class ClothingTextPatches
    {        
        const string clothingAssetName = "Data/ClothingInformation";
        const string hatAssetName = "Data/Hats";

        public static void Apply(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        static private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (CanEdit(e.NameWithoutLocale.BaseName))
            {
                e.Edit((assetData) => EditAsset(assetData));
            }
        }

        static bool CanEdit(string assetNameWithoutLocale)
        {
            return assetNameWithoutLocale == clothingAssetName || assetNameWithoutLocale == hatAssetName;
        }

        static void EditAsset(IAssetData asset)
        {
            Logger.Debug($"Editing {asset.NameWithoutLocale}");
            var dict = asset.AsDictionary<int, string>().Data;

            if (asset.NameWithoutLocale.IsEquivalentTo(clothingAssetName))
            {
                // Shirts and Pants
                UpdateTexts(ItemDefinitions.ShirtEffects, dict, 2);
                UpdateTexts(ItemDefinitions.PantsEffects, dict, 2);
            }
            else
            {
                // Hats
                UpdateTexts(ItemDefinitions.HatEffects, dict, 1);
            }
        }

        static void UpdateTexts<T>(Dictionary<T, ExtItemInfo> itemDefinitions, IDictionary<int, string> dict, int descriptionIndex)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an Enum");
            }

            foreach (var itemDef in itemDefinitions)
            {
                int index = Convert.ToInt32(itemDef.Key);
                if (itemDef.Value.ShouldDescriptionBePatched && dict.TryGetValue(index, out string value))
                {
                    // replace the item's description text
                    var pp = value.Split('/');
                    pp[descriptionIndex] = itemDef.Value.NewItemDescription;
                    dict[index] = String.Join("/", pp);
                }
            }
        }
    }
}
