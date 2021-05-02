/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using StardewModdingAPI;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SailorStyles.Editors
{
	public class ObjectDisplayNameEditor : IAssetEditor
	{
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return ModEntry.JsonAssets != null && Context.IsWorldReady
				&& (asset.AssetNameEquals(Path.Combine("Data", "ClothingInformation"))
					|| asset.AssetNameEquals(Path.Combine("Data", "hats")));
		}

		public void Edit<T>(IAssetData asset)
		{
			Edit(asset);
		}

		private static void Edit(IAssetData asset)
		{
			void localiseNames(
				ref IDictionary<int, string> source, List<string> packs,
				int nameIndex, int descriptionIndex,
				Func<string, List<string>> packSelector, Func<string, int> idSelector)
			{
				const char delimiter = ':';
				List<string> items = packs
					.SelectMany(pack => packSelector(ModEntry.ContentPackNameToId(pack)))
					.ToList();
				Dictionary<string, int> itemsGrouped = items.Zip(items
					.Select(item => idSelector(item)), (name, id) => name + delimiter + id)
					.ToDictionary(str => str.Split(delimiter)[0], str => int.Parse(str.Split(delimiter)[1]));
				foreach (KeyValuePair<string, int> nameAndId in itemsGrouped)
				{
					string name = nameAndId.Key.Split('/')[0].ToLower().Replace(" ", "");
					List<string> entry = source[nameAndId.Value].Split('/').ToList();
					while (entry.Count < Math.Max(nameIndex, descriptionIndex))
						entry.Add("");
					entry[nameIndex] = i18n.Get($"item.{name}.name").ToString();
					entry[descriptionIndex] = i18n.Get($"item.{name}.description").ToString();
					source[nameAndId.Value] = string.Join("/", entry);
				}
			}

			if (asset.AssetNameEquals(Path.Combine("Data", "ClothingInformation")))
			{
				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names and descriptions for new clothes
				localiseNames(
					source: ref data, packs: ModConsts.ClothingPacks,
					nameIndex: 1, descriptionIndex: 2,
					packSelector: ModEntry.JsonAssets.GetAllClothingFromContentPack,
					idSelector: ModEntry.JsonAssets.GetClothingId);
				asset.AsDictionary<int, string>().ReplaceWith(data);
				return;
			}

			if (asset.AssetNameEquals(Path.Combine("Data", "hats")))
			{
				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names and descriptions for new hats
				localiseNames(
					source: ref data, packs: ModConsts.HatPacks,
					nameIndex: 5, descriptionIndex: 1, // JA items inexplicably add a blank field at index 4
					packSelector: ModEntry.JsonAssets.GetAllHatsFromContentPack,
					idSelector: ModEntry.JsonAssets.GetHatId);

				asset.AsDictionary<int, string>().ReplaceWith(data);
				return;
			}
		}
	}
}
