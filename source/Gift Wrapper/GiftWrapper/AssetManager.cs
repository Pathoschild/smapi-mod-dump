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
using StardewModdingAPI.Events;

namespace GiftWrapper
{
	public static class AssetManager
	{
		public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameContentDataPath))
			{
				e.LoadFromModFile<Data.Data>(relativePath: $"{ModEntry.LocalDataPath}.json", priority: AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameContentGiftTexturePath))
			{
				e.LoadFromModFile<Texture2D>(relativePath: $"{ModEntry.LocalGiftTexturePath}.png", priority: AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameContentWrapTexturePath))
			{
				e.LoadFromModFile<Texture2D>(relativePath: $"{ModEntry.LocalWrapTexturePath}.png", priority: AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameContentMenuTexturePath))
			{
				e.LoadFromModFile<Texture2D>(relativePath: $"{ModEntry.GetThemedTexturePath()}.png", priority: AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(ModEntry.GameContentCardTexturePath))
			{
				e.LoadFromModFile<Texture2D>(relativePath: $"{ModEntry.LocalCardTexturePath}.png", priority: AssetLoadPriority.Medium);
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(@"Strings/UI"))
			{
				e.Edit(apply: AssetManager.EditStrings);
			}
		}

		private static void EditStrings(IAssetData asset)
		{
			var data = asset.AsDictionary<string, string>().Data;

			// Add global chat message for gifts opened
			// Format message tokens so that they can be later tokenised by the game in multiplayer.globalChatInfoMessage()
			foreach (string key in new [] { "message.giftopened", "message.giftopened.quantity" })
			{
				data.Add($"Chat_{ModEntry.ItemPrefix}{key}",
					ModEntry.I18n.Get(key, new
					{
						Recipient = "{0}",
						Sender = "{1}",
						ItemName = "{2}",
						ItemQuantity = "{3}"
					}));
			}

			asset.AsDictionary<string, string>().ReplaceWith(data);
		}
	}
}
