/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace GiftWrapper
{
	class AssetManager : IAssetLoader, IAssetEditor
	{
		private IModHelper Helper => ModEntry.Instance.Helper;
		private ITranslationHelper i18n => ModEntry.Instance.i18n;

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModEntry.GameContentTexturePath);
		}

		public T Load<T>(IAssetInfo asset)
		{
			return (T)(object)Helper.Content.Load<Texture2D>($"{ModEntry.LocalTexturePath}.png");
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Data/ObjectInformation")
				|| asset.AssetNameEquals(@"Strings/UI")
				//|| asset.AssetNameEquals(@"TileSheets/tools") // Tool-based method for wrapped gifts
				;
		}

		public void Edit<T>(IAssetData asset)
		{
			this.Edit(asset);
		}

		public void Edit(IAssetData asset)
		{
			if (asset.AssetNameEquals(@"Data/ObjectInformation"))
			{
				if (ModEntry.JsonAssets == null || Game1.currentLocation == null)
					return;

				IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

				// Add localised names and descriptions for new objects
				foreach (var pair in data.Where(pair => pair.Value.Split('/') is string[] split && split[0].StartsWith(ModEntry.AssetPrefix)).ToList())
				{
					string[] itemData = pair.Value.Split('/');
					string itemName = itemData[0].Split(new[] { '.' }, 3)[2];
					itemData[4] = i18n.Get("item." + itemName + ".name").ToString();
					itemData[5] = i18n.Get("item." + itemName + ".description").ToString();
					data[pair.Key] = string.Join("/", itemData);
				}

				asset.AsDictionary<int, string>().ReplaceWith(data);
				return;
			}
			if (asset.AssetNameEquals(@"Strings/UI"))
			{
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

				// Add global chat message for gifts opened
				// Format message tokens so that they can be later tokenised by the game in multiplayer.globalChatInfoMessage()
				const string i18nKey = "message.giftopened";
				data.Add("Chat_" + ModEntry.AssetPrefix + i18nKey,
					i18n.Get(i18nKey, new
					{
						Recipient = "{0}",
						Sender = "{1}",
						OneOrMany = "{2}",
						ItemName = "{3}"
					}));

				asset.AsDictionary<string, string>().ReplaceWith(data);
				return;
			}
			/*
			if (asset.AssetNameEquals(@"TileSheets/tools"))
			{
				// Insert wrapped gift icon into the tools sheet in some unused area
				int objectIndex = ModEntry.JsonAssets.GetObjectId(ModEntry.AssetPrefix + ModEntry.WrappedGiftName);
				if (objectIndex < 0)
					return;
				asset.AsImage().PatchImage(
					source: Game1.objectSpriteSheet,
					sourceArea: Game1.getSourceRectForStandardTileSheet(
						tileSheet: Game1.objectSpriteSheet,
						tilePosition: objectIndex,
						width: 16, height: 16),
					targetArea: Game1.getSourceRectForStandardTileSheet(
						tileSheet: asset.AsImage().Data,
						tilePosition: ModEntry.WrappedGiftToolsSheetIndex,
						width: 16, height: 16),
					patchMode: PatchMode.Replace);
				return;
			}
			*/
		}
	}
}
