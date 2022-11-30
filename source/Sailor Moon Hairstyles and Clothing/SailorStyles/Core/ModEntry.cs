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
using SailorStyles.Core;
using SailorStyles.Editors;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SailorStyles
{
    public sealed class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal static IJsonAssetsApi JsonAssets;
        internal static bool IsJsonAssetsReady;

		internal ITranslationHelper i18n => Helper.Translation;

		private static NPC CatNpc;
		private static readonly Dictionary<ISalable, int[]> CatShopStock = new();


		public override object GetApi()
		{
			return new Api();
		}

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
			helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.GameLoop.DayEnding += this.OnDayEnding;
			helper.Events.Input.ButtonReleased += this.OnButtonReleased;
		}

		#region Game Events

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// CatShop is not loaded without Json Assets installed
			JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (JsonAssets == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}
			
			// Cat is created once Json Assets is loaded
			GenerateCat();

			// Register all content packs packaged with this mod
			JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.HatsDir, "Tuxedo Top Hats"));
			foreach (string contentPack in ModConsts.HatPacks)
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.HatsDir, contentPack));
			foreach (string contentPack in ModConsts.ClothingPacks)
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.ClothingDir, contentPack));

			// Set hook for Json Assets resolving item IDs in data assets
			// Our item data changes should only be applied after this is raised
            JsonAssets.IdsFixed += (_, _) => IsJsonAssetsReady = true;
		}

		[EventPriority(EventPriority.High)]
		private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			IsJsonAssetsReady = false;
		}

		[EventPriority(EventPriority.Low)]
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			// Character changes are always applied as the CatShop character is loaded globally on game launched
			NpcManager.TryLoad(e);
			NpcManager.TryEdit(e, this.Helper.ModContent);

			// Object changes are only applied once Json Assets has resolved item IDs in data assets
			if (IsJsonAssetsReady)
			{
				ObjectDisplayNameEditor.TryEdit(e);
			}
			// Hair changes are only applied if not disabled in config file
            if (Config.EnableHairstyles)
            {
                _ = HairstylesManager.TryLoad(e)
                    || HairstylesManager.TryEdit(e);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// Apply data asset changes to update localised strings for clothing items
			Helper.GameContent.InvalidateCacheAndLocalized(Path.Combine("Data", "hats"));
			Helper.GameContent.InvalidateCacheAndLocalized(Path.Combine("Data", "ClothingInformation"));
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// Add CatShop character and shop stock for the day if available
			// This is done only once on day started as the shop stock should only
			// randomise its selection of items each day
			if (ShouldAddCatShopToday())
			{
				GameLocation location = Game1.getLocationFromName(ModConsts.CatLocationId);
				AddCatToLocation(location);
				RestockCatShop();
			}
		}
		
		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			// Remove CatShop character before game saved
			GameLocation location = Game1.getLocationFromName(ModConsts.CatLocationId);
			RemoveCatFromLocation(location);
		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			// Check for player interactions with CatShop character
			if (e.Button.IsActionButton()
				&& Game1.game1.IsActive
				&& Context.CanPlayerMove
				&& Game1.currentLocation.isCharacterAtTile(e.Cursor.GrabTile) is NPC npc
				&& npc.Name == ModConsts.CatCharacterId)
			{
				// Open CatShop menu
				this.UseCatShop();
			}
		}

		#endregion

		#region Cat Methods

		private static void GenerateCat()
		{
			// Create a new NPC for the CatShop targeting the designated location,
			// with some standard values for character and sprite
			CatNpc ??= new NPC(
				sprite: new AnimatedSprite(
					textureName: ModConsts.GameContentCatSpritesPath,
					currentFrame: 0,
					spriteWidth: 16,
					spriteHeight: 32),
				position: Utility.PointToVector2(ModConsts.CatTileLocation) * Game1.tileSize,
				defaultMap: ModConsts.CatLocationId,
				facingDirection: Game1.down,
				name: ModConsts.CatCharacterId, // Unique internal name does not appear in usual play
				datable: false, // Bad
				schedule: null, // Schedule is forced elsewhere
				portrait: Game1.content.Load<Texture2D>(ModConsts.GameContentCatPortraitPath))
			{
				Age = 2, // Age set to baby/toddler/child value to use a child-height sprite mugshot where available
				Breather = false, // Breathing looks not good
				HideShadow = false, // This is fine
				Gender = 0, // Male
				displayName = ModEntry.Instance.i18n.Get("catshop.text.name"), // Display name does not appear in usual play
				ignoreMovementAnimation = true, // Ensure movement animations aren't played
				farmerPassesThrough = false, // Ensure players collide
				willDestroyObjectsUnderfoot = false // Ensure objects aren't destroyed if in the CatShop tile location
			};
		}

		public static bool ShouldAddCatShopToday()
		{
			// Cat does not appear in the first few days of the game
			bool goodCat = Game1.stats.DaysPlayed > ModConsts.CatAppearsAfterPlayedDays;
			// Cat appears every Sunday and Monday
			bool goodDay = Game1.dayOfMonth % 7 <= 1;
			// Cat appears every day if enabled in config, regardless of context
			bool everyDay = Config.DebugCaturday;

			return (goodCat && goodDay) || everyDay;
		}

		private static void AddCatToLocation(GameLocation location)
		{
			// Multiplayer farmhands are ignored by this method,
			// as as having multiple players add the CatShop character
			// would duplicate the NPC in the location and break our mutex
			if (!Context.IsMainPlayer
				|| location == null
				|| location.getCharacterFromName(ModConsts.CatCharacterId) != null)
				return;

			// Set up CatShop character to appear in location
			CatNpc.modData.Remove(ModConsts.CatMutexKey);
            location.addCharacter(CatNpc);
            ForceNpcSchedule(CatNpc);
		}

		private static void ForceNpcSchedule(NPC npc)
		{
			// (2019) Android platform raises errors on schedule assigned
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				return;
			}

			// CatShop character always follows schedule,
			// as even though it doesn't move or interact at all,
			// it holds the day-to-day idle animations
			npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
			npc.ignoreScheduleToday = false;
			npc.followSchedule = true;
		}

		private static void RemoveCatFromLocation(GameLocation location)
		{
			// Multiplayer farmhands are ignored by this method,
			// as they aren't allowed to add the CatShop character in the first place
			if (Context.IsMainPlayer && CatNpc != null)
			{
				CatNpc.modData.Remove(ModConsts.CatMutexKey);
				location?.characters.Remove(CatNpc);
			}
		}

		#endregion

		#region ContentPack Methods

		public static string GetFullContentPackName(string name, bool isHat)
		{
			return ModConsts.ContentPackPrefix + (isHat ? ModConsts.HatPackPrefix : ModConsts.ClothingPackPrefix) + name;
		}

		public static string GetIdFromContentPackName(string name, bool isHat)
		{
			return ModConsts.IsNotAscii.Replace(
				input: GetFullContentPackName(name: name, isHat: isHat),
				replacement: "");
		}

		public static int GetContentPackCost(string name)
		{
			return ModConsts.ClothingPackCosts.TryGetValue(name, out int value) ? value : ModConsts.DefaultClothingCost;
		}

		#endregion

		#region CatShop Methods

		public static void RestockCatShop()
		{
			CatShopStock.Clear();
			PopulateCatShop(isHat: false);
			PopulateCatShop(isHat: true);
		}

		private static void PopulateCatShop(bool isHat)
		{
			try
			{
				// Populate randomised shop stock with provided content packs
				// Fetch extra content packs from IDs given in config file
				IEnumerable<string> contentPacks = isHat
					? ModConsts.HatPacks
					: ModConsts.ClothingPacks.Concat(Config.ExtraContentPacksToSellInTheShop);
				
				// All content packs have their items represented in the shop on any given day,
				// though only a selection of items are used each time
				foreach (string contentPack in contentPacks)
				{
					// Unpack clothing items from this content pack
					Dictionary<string, bool> stock = new();
					string contentPackId = !ModConsts.HatPacks.Contains(contentPack) && !ModConsts.ClothingPacks.Contains(contentPack)
						? contentPack
						: GetIdFromContentPackName(name: contentPack, isHat: isHat);
					List<string> contentNames = isHat
						? JsonAssets.GetAllHatsFromContentPack(contentPackId)
						: JsonAssets.GetAllClothingFromContentPack(contentPackId);

					// Catch malformed content packs if any exist
					if (contentNames == null || contentNames.Count < 1)
					{
						if (!ModConsts.HatPacks.Contains(contentPack) && !ModConsts.ClothingPacks.Contains(contentPack))
						{
							Log.D($"Did not add content from {contentPack} (as {contentPackId}): Not found.",
								Config.DebugMode);
							continue;
						}
						throw new NullReferenceException($"Failed to add content from content pack: {contentPack} ({contentPackId})");
					}

					// Sakura Kimono upper and lower clothing items are paired to avoid halving quantity added
					if (contentPack.EndsWith("Kimono"))
					{
						contentNames.RemoveAll(name => name.EndsWith("wer"));
					}

					// Add contextual content packs for bundled items if adding a paired content pack
					string pairedContentPack = contentPack == "Stylish Rogue"
						? GetIdFromContentPackName(name: "Tuxedo Top Hats", isHat: true)
						: null;
					List<string> pairedContentNames = !string.IsNullOrEmpty(pairedContentPack)
						? JsonAssets.GetAllHatsFromContentPack(cp: pairedContentPack)
						: null;

					// Shuffle list of items in content pack to randomise selection of clothing in shop
					Utility.Shuffle(Game1.random, contentNames);

					// Limit the number of items from each content pack added each day
					// to avoid the shop being filled with hundreds of items all at once,
					// and keep players coming back to the shop throughout the game
					int goalQuantity = Math.Max(1, contentNames.Count / ModConsts.CatShopQuantityRatio);

					// Add clothing items from the shuffled content pack until goal quantity is reached
					for (int i = 0; i < goalQuantity; ++i)
					{
						string name = contentNames[i];

						// Before:

						// Add hats from Tuxedo Top Hats before tuxedo tops
						if (contentPack.Equals("Stylish Rogue"))
						{
							const string tuxedoHatKey = "Chapeau ";
							string hatVariant = name.Split(new[] { ' ' }, 2)[1];
							string hatName = pairedContentNames.Contains(tuxedoHatKey + hatVariant)
								? tuxedoHatKey + hatVariant
								: new string[] { "De Luxe", "Mystique", "Blonde" }.Contains(hatVariant)
									? tuxedoHatKey + "Blanc"
									: null;
							if (!string.IsNullOrEmpty(hatName))
								stock[hatName] = true;
						}

						// This item:
						stock[name] = isHat;

						// After:

						// Add pants from Sakura Kimono after kimono tops
						if (contentPack.EndsWith("Kimono"))
						{
							stock[name.Replace("Upper", "Lower")] = false;
						}
					}

					// Populate shop stock with items from this content pack
					int cost = GetContentPackCost(contentPack);
					foreach (KeyValuePair<string, bool> nameAndHatFlag in stock)
					{
						// Check each item individually for hat regardless of content pack hat flag,
						// as Stylish Rogue and Tuxedo Top Hats items are added together,
						// and config-added content packs may include a mix of both
						if (nameAndHatFlag.Value)
							CatShopStock[new Hat(JsonAssets.GetHatId(nameAndHatFlag.Key))] = new[] { cost, 1 };
						else
							CatShopStock[new Clothing(JsonAssets.GetClothingId(nameAndHatFlag.Key))] = new[] { cost, 1 };
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("Sailor Styles failed to populate the clothes shop."
					+ " Did you remove some clothing folders, or did I break something?");
				Log.E(ex.ToString());
			}
		}

		public void UseCatShop()
		{
			// Open the CatShop if not in use by another player
			if (!CatNpc.modData.ContainsKey(ModConsts.CatMutexKey))
			{
				// Choose a shop dialogue visible in large viewports
				Random random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
				string whichDialogue = ModConsts.ShopDialogueRoot + (1 + random.Next(5));
				if (whichDialogue.EndsWith("5")) // Dialogue 5 is seasonal
					whichDialogue += $".{Game1.currentSeason}";
				Translation text = i18n.Get(whichDialogue);

				// Meow
				Game1.playSound("cat");

				// Lock mutex on shop menu opening
				// This prevents multiple players from modifying the shop stock at once,
				// which can lead to critical game errors
				CatNpc.modData[ModConsts.CatMutexKey] = "";

				// Open the cat shop
				ShopMenu shopMenu = new ShopMenu(itemPriceAndStock: CatShopStock)
				{
					portraitPerson = CatNpc,
					potraitPersonDialogue = Game1.parseText(text: text, whichFont: Game1.dialogueFont, width: 304)
				};
				Game1.activeClickableMenu = shopMenu;
				Game1.activeClickableMenu.exitFunction = delegate
				{
					// Free mutex on shop menu closed
					CatNpc.modData.Remove(ModConsts.CatMutexKey);
				};
			}
		}

		#endregion
	}
}
