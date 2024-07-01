/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using HoneyHarvestSync.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyHarvestSync
{
    /// <summary>Contains logic and values to increase compatibility with other mods.</summary>
    internal class ModCompat
	{
		/// <summary>
		/// Whether we have a reason to try to track non-HoeDirt "flower" locations for updates.
		/// Basically whether or not there is a mod loaded that allows honey to be flavored by things that aren't crops-associated-to-dirt.
		/// </summary>
		public bool ShouldTrackNonDirtCrops
		{
			get { return IsCurrentBetterBeehousesModLoaded; }
		}

		private const bool canVanillaBeeHousesProduceIndoors = false;
		/// <summary>Whether we should check indoors for bee houses.</summary>
		public bool SyncIndoorBeeHouses
		{
			get { return IsCurrentBetterBeehousesModLoaded ? canBetterBeehousesAllowIndoorBeehouses : canVanillaBeeHousesProduceIndoors; }
		}

		private const int vanillaFlowerRange = 5;
		/// <summary>The max tile range that flowers interact with bee houses at.</summary>
		public int FlowerRange
		{
			get { return BetterBeehousesAPI != null ? BetterBeehousesAPI.GetSearchRadius() : vanillaFlowerRange; }
		}

		/// <summary>Whether a mod is allowing *any* item to qualify as a honey flavor source.</summary>
		public bool IsAnythingHoney
		{
			get { return BetterBeehousesAPI != null && BetterBeehousesAPI.UsingAnythingHoney(); }
		}

		/* -- Better Beehouses compat notes --
		 * It patches the base game's `Utility.findCloseFlower()` method directly, so we shouldn't need to do anything different to determine the crop/honey-affector (if any)
		 * currently affecting a bee house's honey flavor. The only time that what we get back won't match what the farmer will harvest is if the mod's "random" feature
		 * is enabled, but there's not much we can do about that.
		 * We'll need to keep in mind that we might not get back a *flower* crop, necessarily, when dealing with the `Crop` object we get back when BB is enabled.
		 * Also, unfortunately, since not all the "flowers"/honey-affectors the mod allows are crops, that means the `Crop` instance we get back can have
		 * minimal properties set on it. We'll have to support specific tracking for most of the additional "honey flavor" source types.
		 * Ref: https://github.com/tlitookilakin/BetterBeehouses
		 * Ref: https://www.nexusmods.com/stardewvalley/mods/10996
		 */
		private const string betterBeehousesUniqueID = "tlitookilakin.BetterBeehouses";
		private const string minimumBetterBeehousesVersion = "2.1.1";

		private const bool canBetterBeehousesAllowIndoorBeehouses = true;
		public const string betterBeehousesModDataSourceTypeKey = "tlitookilakin.BetterBeehouses.SourceType";
		public const string betterBeehousesModDataFromPotKey = "tlitookilakin.BetterBeehouses.FromPot";

		private IModInfo BetterBeehousesModInfo { get; set; } = null;
		private bool IsCurrentBetterBeehousesModLoaded { get; set; } = false;
		private IBetterBeehousesAPI BetterBeehousesAPI { get; set; } = null;

		/// <summary>
		/// Call this to set up compatibility values and APIs. We have to wait until after `Entry()` before attempting to access their APIs, though,
		/// so all mods will be loaded; usually the `GameLaunched` event is a good time.
		/// </summary>
		public void Init()
		{
			// See if the Better Beehouses mod is even installed/loaded
			BetterBeehousesModInfo = ModEntry.Context.Helper.ModRegistry.Get(betterBeehousesUniqueID);

			if (BetterBeehousesModInfo == null)
			{
				ModEntry.Logger.Log($"{nameof(ModCompat)}.{nameof(Init)} - Mod '{betterBeehousesUniqueID}' not found; Not attempting to get {nameof(IBetterBeehousesAPI)}");

				return;
			}

			IsCurrentBetterBeehousesModLoaded = !BetterBeehousesModInfo.Manifest.Version.IsOlderThan(minimumBetterBeehousesVersion);

			// The API they had changed for SDV v1.6 / BB v2.0.0, so make sure its a current version.
			if (!IsCurrentBetterBeehousesModLoaded)
			{
				ModEntry.Logger.Log($"{nameof(ModCompat)}.{nameof(Init)} - Mod '{betterBeehousesUniqueID}' was found, "
					+ $"but is older than our required minimum version of {minimumBetterBeehousesVersion} to interact with its API");

				return;
			}

			// Try to get Better Beehouses's API
			BetterBeehousesAPI = ModEntry.Context.Helper.ModRegistry.GetApi<IBetterBeehousesAPI>(betterBeehousesUniqueID);

			if (BetterBeehousesAPI == null)
			{
				ModEntry.Logger.Log($"{nameof(ModCompat)}.{nameof(Init)} - Failed to get {nameof(IBetterBeehousesAPI)} even though found mod '{betterBeehousesUniqueID}'", LogLevel.Info);

				return;
			}
			
			ModEntry.Logger.Log($"{nameof(ModCompat)}.{nameof(Init)} - Got {nameof(IBetterBeehousesAPI)}");

			previousFlowerRange = FlowerRange;
			previousIsAnythingHoney = IsAnythingHoney;
		}

		// These are used to track other mods updating config values we care about in `DidCompatModConfigChange()`
		private int previousFlowerRange;
		private bool previousIsAnythingHoney;

		/// <summary>
		/// Every time this is run it will check if a mod we care about for compatibility reasons has changed any settings we care about.
		/// </summary>
		/// <returns>Whether or not another mod we attempt compatibility with has updated config values we care about.</returns>
		public bool DidCompatModConfigChange()
		{
			if (!ShouldTrackNonDirtCrops)
			{
				return false;
			}

			bool didChange = false;

			int flowerRange = FlowerRange;

			if (previousFlowerRange != flowerRange)
			{
				previousFlowerRange = flowerRange;
				didChange = true;
			}

			bool isAnythingHoney = IsAnythingHoney;

			if (previousIsAnythingHoney != isAnythingHoney)
			{
				previousIsAnythingHoney = isAnythingHoney;
				didChange = true;
			}

			return didChange;
		}
	}
}
