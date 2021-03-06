/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System;
using System.Linq;
using ImJustMatt.ExpandedStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ImJustMatt.ExpandedStorage.Framework.Extensions
{
    public static class ItemExtensions
    {
        private const string CategoryFurniture = "category_furniture";
        private const string CategoryArtifact = "category_artifact";
        private const string DonateMuseum = "donate_museum";
        private const string DonateBundle = "donate_bundle";

        public static Chest ToChest(this Item item, Storage storage = null)
        {
            // Get config for chest
            storage ??= ExpandedStorage.GetStorage(item);

            // Create Chest from Item
            var chest = new Chest(true, Vector2.Zero, item.ParentSheetIndex)
            {
                name = item.Name,
                SpecialChestType = Enum.TryParse(storage.SpecialChestType, out Chest.SpecialChestTypes specialChestType)
                    ? specialChestType
                    : Chest.SpecialChestTypes.None
            };
            chest.fridge.Value = storage.IsFridge;

            if (string.IsNullOrWhiteSpace(storage.Image))
                chest.lidFrameCount.Value = Math.Max(storage.Frames, 1);
            else if (item.ParentSheetIndex == 216)
                chest.lidFrameCount.Value = 2;

            // Copy modData from original item
            foreach (var modData in item.modData)
                chest.modData.CopyFrom(modData);

            // Copy modData from config
            foreach (var modData in storage.ModData)
            {
                if (!chest.modData.ContainsKey(modData.Key))
                    chest.modData.Add(modData.Key, modData.Value);
            }

            if (item is not Chest oldChest)
                return chest;

            chest.playerChoiceColor.Value = oldChest.playerChoiceColor.Value;
            if (oldChest.items.Any())
                chest.items.CopyFrom(oldChest.items);

            return chest;
        }

        public static bool MatchesTagExt(this Item item, string search)
        {
            return item.MatchesTagExt(search, true);
        }

        public static bool MatchesTagExt(this Item item, string search, bool exactMatch)
        {
            return item switch
            {
                Furniture when TagEquals(search, CategoryFurniture, exactMatch) => true,
                Object {Type: "Arch"} when TagEquals(search, CategoryArtifact, exactMatch) => true,
                Object {Type: "Arch"} when TagEquals(search, DonateMuseum, exactMatch) => CanDonateToMuseum(item),
                Object {Type: "Minerals"} when TagEquals(search, DonateMuseum, exactMatch) => CanDonateToMuseum(item),
                Object obj when TagEquals(search, DonateBundle, exactMatch) => CanDonateToBundle(obj),
                _ => item.GetContextTags().Any(tag => TagEquals(search, tag, exactMatch))
            };
        }

        private static bool TagEquals(string search, string match, bool exact)
        {
            return exact && search.Equals(match) || match.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        private static bool CanDonateToMuseum(Item item)
        {
            return Game1.locations
                       .OfType<LibraryMuseum>()
                       .FirstOrDefault()?.isItemSuitableForDonation(item)
                   ?? false;
        }

        private static bool CanDonateToBundle(Object obj)
        {
            return Game1.locations
                       .OfType<CommunityCenter>()
                       .FirstOrDefault()?.couldThisIngredienteBeUsedInABundle(obj)
                   ?? false;
        }
    }
}