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
using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using SailorStyles.Core;

namespace SailorStyles.Editors
{
	public static class ObjectDisplayNameEditor
	{
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

        internal static bool TryEdit(AssetRequestedEventArgs e)
        {
            if (ModEntry.JsonAssets is null)
                return false;

            if (e.NameWithoutLocale.IsEquivalentTo("Data/ClothingInformation"))
            {
                e.Edit((asset) =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    // Add localised names and descriptions for new clothes
                    Dictionary<string, bool> packs = ModConsts.ClothingPacks
                        .ToDictionary(pack => pack, isHat => false);
                    LocaliseItems(
                        source: ref data,
                        packs: packs,
                        nameIndex: 1,
                        descriptionIndex: 2,
                        packSelector: ModEntry.JsonAssets.GetAllClothingFromContentPack,
                        idSelector: ModEntry.JsonAssets.GetClothingId);
                    asset.AsDictionary<int, string>().ReplaceWith(data);
                });
                return true;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/hats"))
            {
                e.Edit((asset) =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    // Add localised names and descriptions for new hats
                    Dictionary<string, bool> packs = ModConsts.HatPacks
                        .ToDictionary(pack => pack, isHat => true);
                    packs.Add("Tuxedo Top Hats", true);
                    LocaliseItems(
                        source: ref data,
                        packs: packs,
                        nameIndex: 5,
                        descriptionIndex: 1, // JA items inexplicably add a blank field at index 4
                        packSelector: ModEntry.JsonAssets.GetAllHatsFromContentPack,
                        idSelector: ModEntry.JsonAssets.GetHatId);

                    asset.AsDictionary<int, string>().ReplaceWith(data);
                });
                return true;
            }
            return false;
        }

        private static void LocaliseItems(
            ref IDictionary<int, string> source,
            Dictionary<string, bool> packs,
            int nameIndex,
            int descriptionIndex,
            Func<string, List<string>> packSelector,
            Func<string, int> idSelector)
        {
            IEnumerable<string> items = packs
                .SelectMany(pack => packSelector(ModEntry.GetIdFromContentPackName(pack.Key, pack.Value)));
            Dictionary<string, int> itemsGrouped = items
                .ToDictionary(key => key, value => idSelector(value));

            foreach ((string name, int id) in itemsGrouped)
            {
                string normalisedName = name
                    .GetNthChunk('/', 0)
                    .ToString()
                    .ToLowerInvariant()
                    .Replace(" ", "");
                string[] entry = source[id]
                    .Split('/', Math.Max(nameIndex, descriptionIndex) + 2);

                if (entry.Length < Math.Max(nameIndex, descriptionIndex))
                {
                    int initialLength = entry.Length;
                    Array.Resize(ref entry, Math.Max(nameIndex, descriptionIndex));
                    for (int i = initialLength; i < entry.Length; i++)
                        entry[i] = string.Empty;
                }

                // Localise names and descriptions
                entry[nameIndex] = i18n.Get($"item.{normalisedName}.name");
                entry[descriptionIndex] = i18n.Get($"item.{normalisedName}.description");

                // Update data asset
                source[id] = string.Join('/', entry);
            }
        }
	}
}
