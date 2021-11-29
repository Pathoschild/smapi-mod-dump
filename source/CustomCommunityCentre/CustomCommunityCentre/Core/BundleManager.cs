/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomCommunityCentre
{
    public static class BundleManager
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;
		private static Config Config => ModEntry.Config;


		internal static void RegisterEvents()
		{
			Helper.Events.GameLoop.ReturnedToTitle += BundleManager.GameLoop_ReturnedToTitle;
			Helper.Events.GameLoop.DayEnding += BundleManager.GameLoop_DayEnding;
            Helper.Events.GameLoop.Saving += BundleManager.GameLoop_Saving;
            Helper.Events.GameLoop.Saved += BundleManager.GameLoop_Saved;
		}

		internal static void AddConsoleCommands(string cmd)
		{
			// . . .
		}

		private static void GameLoop_Saved(object sender, SavedEventArgs e)
        {
			Log.T("Saved");
        }

        private static void GameLoop_Saving(object sender, SavingEventArgs e)
		{
			Log.T("Saving");
		}

        private static void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// Local co-op farmhands should not clear custom area-bundle data
			if (!Context.IsSplitScreen || Context.IsMainPlayer)
			{
				BundleManager.Clear();
			}
		}

		private static void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
		{
			Log.T("DayEnding");

			// Save local (and/or persistent) community centre data
			BundleManager.Save(Bundles.CC);
		}

		internal static void SaveLoadedBehaviours(CommunityCenter cc)
		{
			Log.D($"Loaded save: {Game1.player.Name} ({Game1.player.farmName}).",
				Config.DebugMode);
		}

		internal static void DayStartedBehaviours(CommunityCenter cc)
		{
			// Load in new community centre area-bundle data if ready
			if (Bundles.IsAbandonedJojaMartBundleAvailableOrComplete())
			{
				Log.D("Community centre and abandoned joja-mart complete, doing nothing.",
					ModEntry.Config.DebugMode);
			}
			else if (Bundles.IsCommunityCentreDefinitelyComplete(cc))
			{
				Log.D("Community centre complete, unloading any bundle data.",
					ModEntry.Config.DebugMode);
				BundleManager.Save(cc);
			}
			else
			{
				Log.D("Community centre incomplete, loading bundle data.",
					ModEntry.Config.DebugMode);
				BundleManager.Load(cc);
			}
		}

		internal static void Clear()
		{
			Bundles.CustomBundleDonations.Clear();
			Bundles.CustomAreaBundleKeys.Clear();
			Bundles.CustomAreaNamesAndNumbers.Clear();
			Bundles.CustomAreasComplete.Clear();

			Bundles.CustomAreaInitialIndex = 0;
			Bundles.CustomBundleInitialIndex = 0;
			Bundles.DefaultMaxArea = 0;
			Bundles.DefaultMaxBundle = 0;

			Bundles.SetCC(cc: null);
		}

		internal static void Generate(bool isLoadingCustomContent)
		{
			// Fetch initial area-bundle values if not yet set
			if (Bundles.DefaultMaxArea == 0)
			{
				var bundleData = Game1.content.Load
					<Dictionary<string, string>>
					(@"Data/Bundles");

				// Area count is inclusive of Abandoned Joja Mart area to avoid conflicting logic and cases
				Bundles.DefaultMaxArea = bundleData.Keys
					.Select(key => key.Split(Bundles.BundleKeyDelim).First())
					.Distinct()
					.Count() - 1;

				// Bundle count is inclusive of Abandoned Joja Mart bundles, as each requires a unique ID
				Bundles.DefaultMaxBundle = bundleData.Keys
					.Select(key => key.Split(Bundles.BundleKeyDelim).Last())
					.ToList()
					.ConvertAll(int.Parse)
					.Max();

				// Starting index for our custom bundles' unique IDs is after the highest base game bundle ID
				Bundles.CustomBundleInitialIndex = Bundles.DefaultMaxBundle + 1;

				// Starting index for our custom areas' numbers is after the Abandoned Joja Mart area
				// The game will usually consider area 7 as the bulletin board extras, and area 8 as the Junimo Hut, so skip those too
				// Skip 9 to bring us up to a round 10, leaving room for 1 new base game area.
				Bundles.CustomAreaInitialIndex = 10;
			}

			// Reassign world state with or without custom values
			Random r = new ((int)Game1.uniqueIDForThisGame * 9); // copied from StardewValley.Game1.GenerateBundles(...)
			if (isLoadingCustomContent)
			{
				Helper.Content.InvalidateCache(CustomCommunityCentre.AssetManager.BundleCacheAssetKey); // Farmhand idiocy
				Dictionary<string, string> bundleData = new StardewValley.BundleGenerator().Generate(
					bundle_data_path: CustomCommunityCentre.AssetManager.BundleCacheAssetKey, // Internal sneaky asset business
					rng: r);

				// Add bundle data entries, ignoring existing values for mod compatibility
				Dictionary<string, string> oldBundleData = Game1.netWorldState.Value.BundleData;
				bundleData = oldBundleData
					.Union(bundleData.Where(pair => !oldBundleData.ContainsKey(pair.Key)))
					.ToDictionary(pair => pair.Key, pair => pair.Value);

				// Reassign bundle data
				Game1.netWorldState.Value.SetBundleData(bundleData);
			}
			else
			{
				if (Context.IsMainPlayer && Bundles.CustomAreaBundleKeys != null)
				{
					var netBundleData = Reflection.GetField
						<NetStringDictionary<string, NetString>>
						(obj: Game1.netWorldState.Value, name: "netBundleData")
						.GetValue();

					IEnumerable<int> bundleNumbers = Bundles.GetAllCustomBundleNumbers();

					foreach (int bundleNumber in bundleNumbers.Where(num => Bundles.IsCustomBundle(num)))
					{
						netBundleData.Remove(netBundleData.Keys
							.FirstOrDefault(b => bundleNumber == int.Parse(b.Split(Bundles.BundleKeyDelim).Last())));
						Game1.netWorldState.Value.Bundles.Remove(bundleNumber);
						Game1.netWorldState.Value.BundleRewards.Remove(bundleNumber);
					}
				}
			}
		}

        internal static List<StardewValley.GameData.RandomBundleData> Parse()
		{
            static List<StardewValley.GameData.BundleData> bundleEntriesToData(Dictionary<string, CustomCommunityCentre.Data.Bundle> entries)
            {
				return entries
					.Select(bsd => new StardewValley.GameData.BundleData()
					{
						Name = bsd.Key,
						Color = bsd.Value.Colour,
						Index = bsd.Value.Index,
						Items = bsd.Value.Items,
						Pick = bsd.Value.Pick,
						RequiredItems = bsd.Value.RequiredItems,
						Reward = bsd.Value.Reward,
						Sprite = bsd.Value.Sprite
					})
					.ToList();
			}

			StringBuilder errorMessage = new();

			// Reset custom area-bundle dictionaries
			Bundles.CustomAreaNamesAndNumbers.Clear();
			Bundles.CustomAreaBundleKeys.Clear();
			Bundles.CustomAreasComplete.Clear();

			int areaSum = Bundles.CustomAreaInitialIndex;
			int bundleSum = Bundles.CustomBundleInitialIndex;

			List<StardewValley.GameData.RandomBundleData> parsedBundleDefinitions = new();
			foreach (CustomCommunityCentre.Data.ContentPack contentPack in CustomCommunityCentre.ModEntry.ContentPacks)
			{
				foreach (string areaName in contentPack.Definitions.Keys.ToList())
				{
					Log.D($"Checking bundle definitions from '{contentPack.Source.Manifest.UniqueID}'",
						ModEntry.Config.DebugMode);

					// Validate areas
					IEnumerable<char> badChars = areaName
						.Where(c => CustomCommunityCentre.AssetManager.ForbiddenAssetNameCharacters.Contains(c))
						.Distinct();
					if (badChars.Any())
					{
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Area name contains forbidden characters '{string.Join(", ", badChars)}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}
					if (!areaName.StartsWith(CustomCommunityCentre.AssetManager.RequiredAssetNamePrefix, StringComparison.InvariantCulture))
					{
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Area name does not start with required asset name prefix '{CustomCommunityCentre.AssetManager.RequiredAssetNamePrefix}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}
					if (areaName.Split(CustomCommunityCentre.AssetManager.RequiredAssetNameDivider).Length < 3)
					{
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Area does not split name with '{CustomCommunityCentre.AssetManager.RequiredAssetNameDivider}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}

					// Validate bundles
					List<StardewValley.GameData.BundleData> bundles =
						// Default bundles
						bundleEntriesToData(entries: contentPack.Definitions[areaName].BundleSets
							.SelectMany(dict => dict)
							.ToDictionary(pair => pair.Key, pair => pair.Value))
						// Random bundles
						.Concat(bundleEntriesToData(entries: contentPack.Definitions[areaName].Bundles))
						.ToList();
					if (bundles.Any(bundle => bundle?.Name == null))
					{
						errorMessage.AppendLine($"Area {areaName} was not loaded: Bundle or {nameof(StardewValley.GameData.BundleData.Name)} was null.");
						contentPack.Definitions.Remove(areaName);
						continue;
					}
					badChars = bundles
						.SelectMany(b => b.Name.Where(c => CustomCommunityCentre.AssetManager.ForbiddenAssetNameCharacters.Contains(c)))
						.Distinct();
					IEnumerable<string> badNames;
					if (badChars.Any())
					{
						badNames = bundles
							.Where(b => b.Name.Any(c => CustomCommunityCentre.AssetManager.ForbiddenAssetNameCharacters.Contains(c)))
							.Select(b => b.Name);
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Bundle '{string.Join(",", badNames)}' contains forbidden characters '{string.Join(", ", badChars)}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}
					badNames = bundles
						.Where(b => !b.Name.StartsWith(CustomCommunityCentre.AssetManager.RequiredAssetNamePrefix, StringComparison.InvariantCulture))
						.Select(b => b.Name);
					if (badNames.Any())
					{
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Bundle '{string.Join(",", badNames)}' does not start with required asset name prefix '{CustomCommunityCentre.AssetManager.RequiredAssetNamePrefix}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}
					badNames = bundles
						.Where(b => b.Name.Split(CustomCommunityCentre.AssetManager.RequiredAssetNameDivider).Length < 4)
						.Select(b => b.Name);
					if (badNames.Any())
					{
						errorMessage.AppendLine($"Area '{areaName}' was not loaded: Bundle '{string.Join(",", badNames)}' does not split name with '{CustomCommunityCentre.AssetManager.RequiredAssetNameDivider}'");
						contentPack.Definitions.Remove(areaName);
						continue;
					}

					// Set unique area-bundle index keys
					Dictionary<string, int> bundleNamesAndNumbers = bundles.ToDictionary(
						keySelector: bundle => bundle.Name,
						elementSelector: bundle => bundleSum + bundle.Index);

					string[] bundleKeys = bundleNamesAndNumbers
						.Select(pair => $"{pair.Key}{Bundles.BundleKeyDelim}{pair.Value}")
						.ToArray();

					int[] bundleNumbers = bundleNamesAndNumbers.Values
						.Distinct()
						.ToArray();

					// Add parsed definition to list
					StardewValley.GameData.RandomBundleData area = new()
					{
						AreaName = areaName,
						Keys = string.Join(" ", bundleNumbers),
						Bundles = bundleEntriesToData(entries: contentPack.Definitions[areaName].Bundles),
						BundleSets = contentPack.Definitions[areaName].BundleSets
							.Select(dict => new StardewValley.GameData.BundleSetData()
							{
								Bundles = bundleEntriesToData(entries: dict)
							})
							.ToList()
					};
					parsedBundleDefinitions.Add(area);

					// Set custom area-bundle dictionary values
					Bundles.CustomAreaBundleKeys[areaName] = bundleKeys;
					Bundles.CustomAreasComplete[areaSum] = false;
					Bundles.CustomAreaNamesAndNumbers[areaName] = areaSum;

					// Increment counters used for area numbers and bundle keys
					bundleSum += bundleNumbers.Length;
					areaSum++;

					Log.D($"Added bundles for area '{areaName}':{Environment.NewLine}{string.Join(Environment.NewLine, bundleKeys)}",
						ModEntry.Config.DebugMode);
				}
			}

			foreach (CustomCommunityCentre.Data.ContentPack contentPack in CustomCommunityCentre.ModEntry.ContentPacks)
			{
				// Replace bundle entries with substitute values
				Log.D($"Checking substitute bundles from '{contentPack.Source.Manifest.UniqueID}'",
					ModEntry.Config.DebugMode);

				foreach (string targetId in contentPack.Substitutes.Keys.ToList())
				{
					if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded(uniqueID: targetId))
					{
						Log.D($"Skipping substitutes for bundle '{targetId}': Not loaded.",
							ModEntry.Config.DebugMode);
						continue;
					}

					Log.D($"Loading substitutes for bundle '{targetId}'...",
						ModEntry.Config.DebugMode);

					foreach (string bundleName in contentPack.Substitutes[targetId].Keys.ToList())
					{
						CustomCommunityCentre.Data.BundleSubstitute bundleSubstitute
							= contentPack.Substitutes[targetId][bundleName];

						// Validate bundle substitutes
						IEnumerable<char> badChars = bundleName.Where(c => AssetManager.ForbiddenAssetNameCharacters.Contains(c)).Distinct();
						if (badChars.Any())
						{
							errorMessage.AppendLine($"Substitute for bundle '{bundleName}' was not loaded: Bundle name contains forbidden characters '{string.Join(", ", badChars)}'");
							contentPack.Substitutes[targetId].Remove(bundleName);
							continue;
						}

						// Find matching bundle for substitute bundle
						IEnumerable<StardewValley.GameData.BundleData> bundles = parsedBundleDefinitions
							// Default bundles
							.SelectMany(rbd => rbd.BundleSets.SelectMany(bd => bd.Bundles))
							// Random bundles
							.Concat(parsedBundleDefinitions.SelectMany(rbd => rbd.Bundles));
						StardewValley.GameData.BundleData bundle = bundles
							.FirstOrDefault(bd => bd.Name == bundleName);

						if (bundle == null)
						{
							Log.D($"Skipping substitute for bundle '{bundleName}': Match not found.",
								ModEntry.Config.DebugMode);
							continue;
						}

						Log.D($"Substituting bundle: '{bundleName}'",
							ModEntry.Config.DebugMode);
						
						// Substitute entries in bundle with provided values
						if (!string.IsNullOrWhiteSpace(bundleSubstitute.Items))
							bundle.Items = bundleSubstitute.Items;
						if (bundleSubstitute.Pick.HasValue)
							bundle.Pick = bundleSubstitute.Pick.Value;
						if (bundleSubstitute.RequiredItems.HasValue)
							bundle.RequiredItems = bundleSubstitute.RequiredItems.Value;
						if (!string.IsNullOrWhiteSpace(bundleSubstitute.Reward))
							bundle.Reward = bundleSubstitute.Reward;
					}
				}
			}

			if (errorMessage.Length > 0)
			{
				errorMessage.Insert(0, $"Encountered errors while loading area-bundle data:{Environment.NewLine}");
				Log.E(errorMessage.ToString());
			}

			// Append custom bundle data to default random bundle data after modifications
			var randomBundleData = Game1.content.Load
				<List<StardewValley.GameData.RandomBundleData>>
				(@"Data/RandomBundles");
			parsedBundleDefinitions = randomBundleData
				.Union(parsedBundleDefinitions)
				.ToList();

			return parsedBundleDefinitions;
		}

		internal static void Load(CommunityCenter cc)
		{
			// Set world state to include custom content:

			BundleManager.Generate(isLoadingCustomContent: true);

			IEnumerable<string> areaNames = Bundles.CustomAreaBundleKeys.Keys;
			IEnumerable<(string name, int number)> bundles = Bundles.GetBundleNamesAndNumbersFromBundleKeys(
				Bundles.CustomAreaBundleKeys.Values
					.SelectMany(s => s));

			Dictionary<string, bool> areasCompleteData = null;
			IEnumerable<(string name, bool isComplete)> bundleRewardsData = null;
			// lord forgive me for what i am about to do

			// Host player loads preserved/persistent data from world:
			if (Context.IsMainPlayer)
			{
				// Deserialise saved mod data:

				if (cc.modData.TryGetValue(Bundles.KeyAreasComplete, out string rawAreasComplete) && !string.IsNullOrWhiteSpace(rawAreasComplete))
				{
					// Deserialise saved area-bundle mod data
					Dictionary<string, bool> savedAreasComplete = rawAreasComplete
					.Split(Bundles.ModDataKeyDelim)
					.ToDictionary(
						keySelector: s => s.Split(Bundles.ModDataValueDelim).First(),
						elementSelector: s => bool.Parse(s.Split(Bundles.ModDataValueDelim).Last()));

					// Read saved area-bundle setups

					areasCompleteData = areaNames
						// Include newly-added area-bundles
						.Where(name => !savedAreasComplete.ContainsKey(name))
						.ToDictionary(name => name, name => false)
						// Include saved area-bundles if their data is loaded
						// Saved area-bundles are excluded if data is missing
						.Concat(savedAreasComplete.Where(pair => areaNames.Contains(pair.Key)))
						.ToDictionary(pair => pair.Key, pair => pair.Value);

					// Check for saved data with no matching data
					IEnumerable<string> excludedAreas = savedAreasComplete.Select(pair => pair.Key).Except(areasCompleteData.Keys);
					if (excludedAreas.Any())
					{
						string s1 = string.Join(", ", excludedAreas);
						string message = $"Removing saved data for area-bundles with missing data:{Environment.NewLine}{s1}";
						Log.W(message);
					}
				}

				// Same as above code for deserialising areas complete, but for bundle rewards
				if (cc.modData.TryGetValue(Bundles.KeyBundleRewards, out string rawBundleRewards) && !string.IsNullOrWhiteSpace(rawBundleRewards))
				{
					Dictionary<string, bool> savedBundleRewards = rawBundleRewards
						.Split(Bundles.ModDataKeyDelim)
						.ToDictionary(
							keySelector: s => s.Split(Bundles.ModDataValueDelim).First(),
							elementSelector: s => bool.Parse(s.Split(Bundles.ModDataValueDelim).Last()));

					bundleRewardsData = bundles
						.Where(bundle => !savedBundleRewards.Keys.Any(key => key == bundle.name))
						.Select(bundle => (name: bundle.name, isComplete: false))
						.Concat(savedBundleRewards.Where(pair => bundles.Any(bundle => bundle.name == pair.Key))
							.Select(pair => (name: pair.Key, isComplete: pair.Value)));

					IEnumerable<string> excludedBundles = savedBundleRewards
						.Select(pair => pair.Key)
						.Except(bundleRewardsData.Select(bundleReward => bundleReward.name));
					if (excludedBundles.Any())
					{
						string s = string.Join(", ", excludedBundles);
						string message = $"Removing saved data for bundle rewards with missing data:{Environment.NewLine}{s}";
						Log.W(message);
					}
				}

				// Load donated bundle items from world storage, populating bundle progress dictionary:

				// Fetch world storage chest
				Vector2 tileLocation = Utility.PointToVector2(Bundles.CustomBundleDonationsChestTile);
				if (cc.Objects.TryGetValue(tileLocation, out StardewValley.Object o) && o is Chest chest)
				{
					// Compare items against bundle requirements fields in bundle data
					IEnumerable<string> customBundleDataKeys = Game1.netWorldState.Value.BundleData
						.Where(pair => int.Parse(pair.Key.Split(Bundles.BundleKeyDelim).Last()) > Bundles.DefaultMaxBundle)
						.Select(pair => pair.Key);
					foreach (string bundleKey in customBundleDataKeys)
					{
						const int fields = 3;
						int bundleId = int.Parse(bundleKey.Split(Bundles.BundleKeyDelim).Last());
						string rawBundleInfo = Game1.netWorldState.Value.BundleData[bundleKey];
						string[] split = rawBundleInfo.Split(Bundles.BundleKeyDelim);
						string[] ingredientsSplit = split[2].Split(' ');
						bool[] ingredientsComplete = new bool[ingredientsSplit.Length];
						cc.bundles[bundleId].CopyTo(array: ingredientsComplete, index: 0);

						for (int ingredient = 0; ingredient < ingredientsSplit.Length; ingredient += fields)
						{
							int ingredientIndex = ingredient / fields;
							StardewValley.Menus.BundleIngredientDescription bundleIngredientDescription
								= new (
									index: Convert.ToInt32(ingredientsSplit[ingredient]),
									stack: Convert.ToInt32(ingredientsSplit[ingredient + 1]),
									quality: Convert.ToInt32(ingredientsSplit[ingredient + 2]),
									completed: true);

							// Check chest items for use in bundles and repopulate bundle progress data
							foreach (Item item in chest.items.ToList())
							{
								if (item is StardewValley.Object o1
									&& (o1.ParentSheetIndex == bundleIngredientDescription.index
										|| (bundleIngredientDescription.index < 0 && o1.Category == bundleIngredientDescription.index))
									&& o1.Quality >= bundleIngredientDescription.quality
									&& o1.Stack == bundleIngredientDescription.stack)
								{
									ingredientsComplete[ingredientIndex] = true;
									chest.items.Remove(item);
									Bundles.CustomBundleDonations.Add(item);
									break;
								}
							}
						}

						// If the first (len/3) elements are true, the bundle should be marked as complete (mark all other elements true)
						if (ingredientsComplete.Take(ingredientsSplit.Length / 3).All(isComplete => isComplete))
						{
							for (int i = 0; i < ingredientsComplete.Length; ++i)
							{
								ingredientsComplete[i] = true;
							}
						}

						cc.bundles[bundleId] = ingredientsComplete;
					}

					chest.clearNulls();

					// Remove chest if empty
					if (!chest.items.Any(i => i != null))
					{
						cc.Objects.Remove(tileLocation);
					}
				}
			}

			// With no previous saved data, load current area-bundle setup as-is
			if (areasCompleteData == null || !areasCompleteData.Any())
			{
				areasCompleteData = areaNames.ToDictionary(name => name, name => false);
			}
			if (bundleRewardsData == null || !bundleRewardsData.Any())
			{
				bundleRewardsData = bundles.Select(bundle => (name: bundle.name, isComplete: false));
			}

			// Set CC state to include custom content:

			// Set areas complete array to reflect saved data length and contents
			if (areasCompleteData.Any())
			{
				foreach (string areaName in areasCompleteData.Keys)
				{
					int areaNumber = Bundles.GetCustomAreaNumberFromName(areaName);
					Bundles.CustomAreasComplete[areaNumber] = areasCompleteData[areaName];
				}
			}

			BundleManager.ReplaceAreaBundleConversions(cc: cc);
			cc.refreshBundlesIngredientsInfo();

			// Set bundle rewards dictionary to reflect saved data
			foreach ((string name, bool isComplete) in bundleRewardsData)
			{
				int bundleNumber = bundles.FirstOrDefault(b => b.name == name).number;
				cc.bundleRewards[bundleNumber] = isComplete;
			}

			Log.D("Loaded bundle data.",
				Config.DebugMode);
		}

		internal static void Save(CommunityCenter cc)
		{
			Log.D("Unloading bundle data.",
				ModEntry.Config.DebugMode);

			// Host player saves preserved/persistent data to world:
			if (Context.IsMainPlayer)
			{
				// Save donated bundle items to world storage
				{
					Vector2 tileLocation = Utility.PointToVector2(Bundles.CustomBundleDonationsChestTile);
					Chest existingChest = cc.Objects.TryGetValue(tileLocation, out StardewValley.Object o) && o is Chest
						? o as Chest
						: null;
					Chest chest = existingChest ?? new(playerChest: true, tileLocation: tileLocation);
					chest.clearNulls();

					if (Bundles.CustomBundleDonations.Any())
					{
						if (o != null && existingChest == null)
						{
							// Replace any existing object with this chest
							Log.I($"Bundle donations safekeeping chest now contains {o.Name}.");
							chest.items.Add(cc.Objects[tileLocation]);
						}
						foreach (Item item in Bundles.CustomBundleDonations.ToList())
						{
							// Add all bundle donations to the chest
							chest.items.Add(item);
							Bundles.CustomBundleDonations.Remove(item);
						}
						cc.Objects[tileLocation] = chest;
					}
				}

				if (Bundles.CustomAreaBundleKeys != null)
				{
					// Serialise mod data to be saved

					Dictionary<string, bool> areasCompleteData =
						Bundles.CustomAreasComplete
						.ToDictionary(
							keySelector: pair => Bundles.GetCustomAreaNameFromNumber(pair.Key),
							elementSelector: pair => pair.Value);
					Dictionary<string, bool> bundleRewardsData =
						Bundles.CustomAreaBundleKeys.Values
						.SelectMany(s => s)
						.ToDictionary(
							keySelector: bundleKey => bundleKey.Split(Bundles.BundleKeyDelim).First(),
							elementSelector: bundleKey => cc.bundleRewards.TryGetValue(key: int.Parse(bundleKey.Split(Bundles.BundleKeyDelim).Last()), out bool isComplete) && isComplete);

					string serialisedAreasCompleteData = string.Join(
						Bundles.ModDataKeyDelim.ToString(),
						areasCompleteData.Select(pair => $"{pair.Key}{Bundles.ModDataValueDelim}{pair.Value}"));
					string serialisedBundleRewardsData = string.Join(
						Bundles.ModDataKeyDelim.ToString(),
						bundleRewardsData.Select(pair => $"{pair.Key}{Bundles.ModDataValueDelim}{pair.Value}"));

					cc.modData[Bundles.KeyAreasComplete] = serialisedAreasCompleteData;
					cc.modData[Bundles.KeyBundleRewards] = serialisedBundleRewardsData;
				}
			}

			// Reset world state to exclude custom content
			try
			{
				BundleManager.Generate(isLoadingCustomContent: false);
			}
			catch (Exception e)
            {
				Log.E($"Error while unloading area-bundle data: {e}");
            }
			finally
			{
				BundleManager.ReplaceAreaBundleConversions(cc: cc);
				cc.refreshBundlesIngredientsInfo();
			}

			Log.D("Unloaded bundle data.",
				Config.DebugMode);
		}

		internal static void ReplaceAreaBundleConversions(CommunityCenter cc)
		{
			IReflectedField<Dictionary<int, List<int>>> areaBundleDictField = Reflection.GetField
				<Dictionary<int, List<int>>>
				(cc, "areaToBundleDictionary");
			IReflectedField<Dictionary<int, int>> bundleAreaDictField = Reflection.GetField
				<Dictionary<int, int>>
				(cc, "bundleToAreaDictionary");

			Dictionary<int, List<int>> areaBundleDict = areaBundleDictField.GetValue() ?? new();
			Dictionary<int, int> bundleAreaDict = bundleAreaDictField.GetValue() ?? new();

			Dictionary<string, int> areaNamesAndNumbers = Game1.netWorldState.Value.BundleData.Keys
				.Select(bundleKey => bundleKey.Split(Bundles.BundleKeyDelim).First())
				.Distinct()
				.ToDictionary(
					keySelector: areaName => areaName,
					elementSelector: areaName => CommunityCenter.getAreaNumberFromName(areaName) is int i && i >= 0
						? i
						: Bundles.GetCustomAreaNumberFromName(areaName));

			if (!cc.bundleMutexes.Any())
			{
				// Load custom area-bundle pairs (and base game data if not yet loaded)
				for (int i = 0; i < Bundles.DefaultMaxArea + 1; ++i)
				{
					NetMutex netMutex = new();
					cc.bundleMutexes.Add(netMutex);
					cc.NetFields.AddField(netMutex.NetFields);
				}
			}

			int areaDefaultCount = Bundles.DefaultMaxArea + 1;
			int areaTotalCount = Bundles.IsCommunityCentreDefinitelyComplete(cc) || !Bundles.AreAnyCustomAreasLoaded()
				? areaDefaultCount
				: areaNamesAndNumbers.Count + (Bundles.CustomAreaInitialIndex - areaDefaultCount);
			for (int i = 0; i < areaDefaultCount; ++i)
			{
				// Load bundle lists for default areas
				areaBundleDict[i] = new();
			}
			for (int i = areaDefaultCount; i < areaTotalCount; ++i)
			{
				// Load bundle lists for blank and custom areas
				// These area lists are loaded separately as loading blank bundle lists
				// without custom areas will cause issues
				areaBundleDict[i] = new();
			}
			foreach (string bundleKey in Game1.netWorldState.Value.BundleData.Keys)
			{
				// Populate bundles forareas
				string areaName = bundleKey.Split(Bundles.BundleKeyDelim).First();
				int areaNumber = areaNamesAndNumbers[areaName];
				int bundleNumber = Convert.ToInt32(bundleKey.Split(Bundles.BundleKeyDelim).Last());

				if (string.IsNullOrWhiteSpace(areaName) || areaNumber < 0 || bundleNumber < 0)
					continue;

				bundleAreaDict[bundleNumber] = areaNumber;
				if (!areaBundleDict[areaNumber].Contains(bundleNumber))
				{
					areaBundleDict[areaNumber].Add(bundleNumber);
				}
			}

			areaBundleDictField.SetValue(areaBundleDict);
			bundleAreaDictField.SetValue(bundleAreaDict);
		}
	}
}
