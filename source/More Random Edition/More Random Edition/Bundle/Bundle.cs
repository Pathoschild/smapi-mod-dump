/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Randomizer
{
	/// <summary>
	/// Represents a bundle
	/// </summary>
	public abstract class Bundle
	{
		public CommunityCenterRooms Room { get; set; }
		public int Id { get; set; }
		public string Key
		{
			get
			{
                string roomName = Room == CommunityCenterRooms.Joja
					? "Abandoned Joja Mart"
					: Regex.Replace(Room.ToString(), @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

				return $"{roomName}/{Id}";
			}
		}
		/// <summary>
		/// The name to display to users
		/// It is important for this to NEVER be set outside of SetBundleName
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Sets the bundle display name by looking up the translation
		/// </summary>
		/// <param name="translationKey">The key that's in the i18n files</param>
		/// <param name="obj">The anonymous object used in the translation</param>
		public void SetBundleName(string translationKey, dynamic obj = null)
		{
            DisplayName = Globals.GetTranslation(translationKey, obj);
        }

		/// <summary>
		/// Sets the display and english names for the vault
		/// </summary>
		/// <param name="moneyAmount">The amount of mone the bundle is worth</param>
		/// <param name="bundleFlavorId">The ID of the flavor text</param>
		public void SetVaultBundleName(int moneyAmount, int bundleFlavorId)
		{
			string moneyFormatKey = "vault-money-format";
            string moneyAmountStringLocalized = moneyAmount.ToString("N0", new CultureInfo(Globals.ModRef.Helper.Translation.Locale));
            string bundleNameFlavorLocalized = Globals.GetTranslation($"{BundleType}-{bundleFlavorId}");
            string moneyStringLocalized = Globals.GetTranslation(moneyFormatKey, new { moneyString = moneyAmountStringLocalized });
            DisplayName = $"{moneyStringLocalized}: {bundleNameFlavorLocalized}";
        }

		public RequiredBundleItem Reward { get; set; }
		public List<RequiredBundleItem> RequiredItems { get; set; }
		public BundleColors Color { get; set; }
		public int? MinimumRequiredItems { get; set; }
		public BundleTypes BundleType { get; set; } = BundleTypes.None;

		public string ImageNameSuffix { get; set; }
		public string ImageName
		{
			get
			{
				return $"{BundleType}{ImageNameSuffix}";
			}
		}

		private static List<BundleTypes> _randomBundleTypes { get; set; }

		/// <summary>
		/// Re-set up the static properties so that if this is ran again
		/// </summary>
		public static void InitializeAllBundleTypes()
		{
			_randomBundleTypes = GetBundleTypeList("All");
			CraftingRoomBundle.RoomBundleTypes = GetBundleTypeList("Crafting");
			PantryBundle.RoomBundleTypes = GetBundleTypeList("Pantry");
			FishTankBundle.RoomBundleTypes = GetBundleTypeList("FishTank");
			BulletinBoardBundle.RoomBundleTypes = GetBundleTypeList("Bulletin");
			BoilerRoomBundle.RoomBundleTypes = GetBundleTypeList("Boiler");
			VaultBundle.RoomBundleTypes = GetBundleTypeList("Vault");
			JojaBundle.RoomBundleTypes = GetBundleTypeList("Joja");
		}

		/// <summary>
		/// Gets the bundle type list that starts with the given string
		/// </summary>
		/// <param name="BundleTypeString">The string to match</param>
		/// <returns>The list of all matching bundle types</returns>
		private static List<BundleTypes> GetBundleTypeList(string BundleTypeString)
		{
			return Enum.GetValues(typeof(BundleTypes))
				.Cast<BundleTypes>()
				.Where(x => x.ToString().StartsWith(BundleTypeString))
				.ToList();
		}

		/// <summary>
		/// The factory call for this bundle - creates one of the appropriate type
		/// If there's no bundle types left, will default to a random bundle
		/// Has a 10% chance of generating a random bundle, and a 10% chance of a random reward if not the Joja room
		/// </summary>
		/// <param name="room">The room the bundle is in</param>
		/// <param name="id">The id of the bundle</param>
		public static Bundle Create(CommunityCenterRooms room, int id)
		{
            Bundle createdBundle;
            switch (room)
			{
				case CommunityCenterRooms.CraftsRoom:
					createdBundle = new CraftingRoomBundle();
					break;
				case CommunityCenterRooms.Pantry:
					createdBundle = new PantryBundle();
					break;
				case CommunityCenterRooms.FishTank:
					createdBundle = new FishTankBundle();
					break;
				case CommunityCenterRooms.BoilerRoom:
					createdBundle = new BoilerRoomBundle();
					break;
				case CommunityCenterRooms.Vault:
					createdBundle = new VaultBundle();
					break;
				case CommunityCenterRooms.BulletinBoard:
					createdBundle = new BulletinBoardBundle();
					break;
				case CommunityCenterRooms.Joja:
					createdBundle = new JojaBundle();
					break;
				default:
					Globals.ConsoleError($"Cannot create bundle for room: {room}");
					return null;
			}

			createdBundle.Room = room;
			createdBundle.Id = id;

			if (!createdBundle.TryGenerateRandomBundle()) { createdBundle.Populate(); }
			if (!createdBundle.TryGenerateRandomReward()) { createdBundle.GenerateReward(); }

			if (createdBundle.RequiredItems == null || createdBundle.RequiredItems.Count == 0)
			{
				createdBundle.GenerateRandomBundleFailsafe();
			}

			return createdBundle;
		}

		/// <summary>
		/// Populate the bundle's name, required items, number required, color
		/// </summary>
		protected abstract void Populate();

		/// <summary>
		/// Populate the bundle's reward
		/// </summary>
		protected abstract void GenerateReward();

		/// <summary>
		/// Gets the string to be used in the bundle dictionary
		/// </summary>
		/// <returns>
		/// bundle name/reward/possible items required/color/min items needed (optional)
		/// </returns>
		public override string ToString()
		{
			string rewardString = "";
			if (Room != CommunityCenterRooms.Joja) // Joja doesn't actually have an item reward
			{
				string rewardStringPrefix = GetRewardStringPrefix();
				string itemId = Reward.Item.Id;
				rewardString = $"{rewardStringPrefix} {itemId} {Reward.NumberOfItems}";
			}

			string minRequiredItemsString = "";
			if (Room != CommunityCenterRooms.Vault && MinimumRequiredItems != null && MinimumRequiredItems > 0)
			{
				minRequiredItemsString = MinimumRequiredItems.ToString();
			}

			string[] originalBundleData = DataLoader.Bundles(Game1.content)[Key].Split("/");
			originalBundleData[(int)BundleIndexes.Reward] = rewardString;
			originalBundleData[(int)BundleIndexes.RequiredItems] = GetRequiredItemString();
			originalBundleData[(int)BundleIndexes.ColorIndex] = $"{Color:D}";
			originalBundleData[(int)BundleIndexes.MinimumRequiredItems] = minRequiredItemsString;
			originalBundleData[(int)BundleIndexes.OldDisplayName] = "";
            originalBundleData[(int)BundleIndexes.DisplayName] = DisplayName;

			return string.Join("/", originalBundleData);
		}

		/// <summary>
		/// Gets the prefix used before the reward string
		/// </summary>
		/// <returns>
		/// R if a ring; BO if a BigCraftableObject; O otherwise
		/// Yes, even after the 1.6 rework, these are the correct prefixes
		/// </returns>
		private string GetRewardStringPrefix()
		{
			if (Reward?.Item == null)
			{
				Globals.ConsoleError($"No reward item defined for bundle: {DisplayName}");
				return "O 388 1";
			}

			if (Reward.Item.IsRing)
			{
				return "R";
			}

			if (Reward.Item.IsBigCraftable)
			{
				return "BO";
			}

			return "O";
		}

		/// <summary>
		/// Gets the string for all the required items
		/// </summary>
		/// <returns>The reward string</returns>
		private string GetRequiredItemString()
		{
			if (RequiredItems.Count == 0)
			{
				Globals.ConsoleError($"No items defined for bundle {DisplayName}");
				return "";
			}

			if (Room == CommunityCenterRooms.Vault)
			{
				return RequiredItems.First().GetStringForBundles(true);
			}

			else
			{
				for (int i = 0; i < RequiredItems.Count; i++)
				{
					Item item = RequiredItems[i]?.Item ?? null;
					if (item == null)
					{
						Globals.ConsoleError($"Null item found during bundle creation. Bundle: {DisplayName}; Index: {i}");
					}
				}

				return string.Join(" ", RequiredItems.Select(item => item.GetStringForBundles(false)));
			}
		}

		/// <summary>
		/// Attempt to generate a random bundle - 10% chance
		/// </summary>
		/// <returns>True if successful, false otherwise</returns>
		protected bool TryGenerateRandomBundle()
		{
			if (Room != CommunityCenterRooms.Vault &&
				Room != CommunityCenterRooms.Joja && 
				BundleRandomizer.Rng.NextBoolean(10))
			{
				PopulateRandomBundle();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Attempt to generate a random reward - 10% chance
		/// </summary>
		/// /// <returns>True if successful, false otherwise</returns>
		protected bool TryGenerateRandomReward()
		{
			if (Room != CommunityCenterRooms.Joja && BundleRandomizer.Rng.NextBoolean(10))
			{
				GenerateRandomReward();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Force generate a random bundle
		/// Failsafe in case we run out of bundles
		/// </summary>
		protected void GenerateRandomBundleFailsafe()
		{
			Globals.ConsoleWarn($"Had to generate random bundle for {Room} as a fallback for this bundle: {BundleType.ToString()}");
			PopulateRandomBundle();
			GenerateRandomReward();
		}

		/// <summary>
		/// Creates a bundle with random items
		/// </summary>
		protected void PopulateRandomBundle()
		{
			RNG rng = BundleRandomizer.Rng;

            BundleType = rng.GetRandomValueFromList(_randomBundleTypes);
			List<RequiredBundleItem> potentialItems = new List<RequiredBundleItem>();
			switch (BundleType)
			{
				case BundleTypes.AllRandom:
					SetBundleName("bundle-random-all");
					potentialItems = RequiredBundleItem.CreateList(ItemList.Items.Values.Where(x =>
						x.DifficultyToObtain < ObtainingDifficulties.Impossible)
					.ToList());
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 4;
					break;
				case BundleTypes.AllLetter:
					string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					string randomLetter;
					do
					{
						randomLetter = letters[rng.NextIntWithinRange(0, letters.Length - 1)].ToString();
                        letters = letters.Replace(randomLetter, "");
						potentialItems = RequiredBundleItem.CreateList(
							ItemList.Items.Values.Where(x =>
								ShouldIncludeInLetterBundle(x) &&
                                // Prioritiy is: OverrideName > EnglishName
                                (x.OverrideName ?? x.EnglishName)
                                    .StartsWith(randomLetter, StringComparison.InvariantCultureIgnoreCase)
							).ToList()
						);
					} while (potentialItems.Count < 4);

                    SetBundleName("bundle-random-letter", new { letter = randomLetter });
					ImageNameSuffix = randomLetter;
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 3;
					break;
			}

			Color = rng.GetRandomValueFromList(
				Enum.GetValues(typeof(BundleColors)).Cast<BundleColors>().ToList());
		}

		/// <summary>
		/// Whether we should include the item in a letter bundle - this is necessary because some names are
		/// changed during localization; we do NOT want to have a different set of items for each letter
		/// depending on the language
		/// 
		/// Logic:
		/// - Disallow all items with any kind of override name
		/// - Exclude crops and fish, since their override names are the same across languages
		/// - Also, exclude impossible items (including legendary fish)
		/// </summary>
		/// <param name="item">The item to check</param>
		/// <returns>True if the item should be included, false otherwise</returns>
		private static bool ShouldIncludeInLetterBundle(Item item)
		{
			// Exclude all items that...
			// - Have an override display name
			// - Are impossible to get
			// - Are legendary fish
			if (item.DifficultyToObtain >= ObtainingDifficulties.Impossible ||
                !string.IsNullOrWhiteSpace(item.OverrideDisplayName) ||
				(item is FishItem fishItem && fishItem.IsLegendaryFish))
			{
				return false;
			}

			// Only accept items with an override name if they are crops or fish
			// Note that Unmilled Rice is covered above since it has an OverrideDisplayName
			bool overrideNamesAreConstant = item is CropItem || item is FishItem;
			bool hasNoOverrideName = string.IsNullOrWhiteSpace(item.OverrideName);
			return overrideNamesAreConstant || hasNoOverrideName;
        }

        /// <summary>
        /// Generates a random reward out of all of the items
        /// </summary>
        protected void GenerateRandomReward()
		{
			RNG rng = BundleRandomizer.Rng;

            Item reward = rng.GetRandomValueFromList(
				ItemList.Items.Values
					.Concat(ItemList.BigCraftableItems.Values)
					.ToList()
			);
			int numberToGive = rng.NextIntWithinRange(1, 25);
			if (!reward.CanStack) { numberToGive = 1; }

			Reward = reward.IsBigCraftable
				? new RequiredBundleItem(reward.BigCraftableIndex, numberToGive)
				: new RequiredBundleItem(reward.ObjectIndex, numberToGive);
        }
	}
}
