/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace SailorStyles.Editors
{
	internal static class NpcManager
	{
        internal static bool TryLoad(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentCatSchedulePath))
                e.LoadFromModFile<Dictionary<string, string>>(ModConsts.LocalCatSchedulePath + ".json", AssetLoadPriority.Exclusive);
            else if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentCatSpritesPath))
                e.LoadFromModFile<Texture2D>(ModConsts.LocalCatSpritesPath + ".png", AssetLoadPriority.Exclusive);
            else if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentCatPortraitPath))
                e.LoadFromModFile<Texture2D>(ModConsts.LocalCatPortraitPath + ".png", AssetLoadPriority.Exclusive);
            else
                return false;
            return true;
        }

        internal static bool TryEdit(AssetRequestedEventArgs e, IModContentHelper helper)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentAnimationsPath))
                e.Edit((IAssetData asset) => Edit(asset, helper), AssetEditPriority.Default);
            else if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentCatSchedulePath))
                e.Edit((IAssetData asset) => Edit(asset, helper), AssetEditPriority.Default);
            else
                return false;
            return true;
        }

		private static void Edit(IAssetData asset, IModContentHelper helper)
        {
            if (asset.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentAnimationsPath))
            {
                // Add CatShop character animation sequence definitions to data asset
                var data = asset.AsDictionary<string, string>().Data;
                var json = helper.Load
                    <Dictionary<string, string>>
                    (ModConsts.LocalAnimationsPath + ".json");

                foreach ((string key, string value) in json)
                {
                    _ = data.TryAdd(key, value);
                }
                asset.ReplaceWith(data);
            }
            else if (asset.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentCatSchedulePath))
            {
                // Add CatShop game and tile location values to character schedule data asset
                var data = asset.AsDictionary<string, string>().Data;
                data["spring"] = string.Format(
                    data["spring"],
                    ModConsts.CatLocationId,
                    ModConsts.CatTileLocation.X,
                    ModConsts.CatTileLocation.Y,
                    Game1.down);
                asset.ReplaceWith(data);
            }
        }
	}
}
