/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SailorStyles
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal static IJsonAssetsApi JsonAssets;
		internal ITranslationHelper i18n => Helper.Translation;

		private static NPC CatNpc = null;
		private static readonly Dictionary<ISalable, int[]> CatShopStock = new Dictionary<ISalable, int[]>();


		public override object GetApi()
		{
			return new Api();
		}

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.GameLoop.DayEnding += this.OnDayEnding;
			helper.Events.Input.ButtonReleased += this.OnButtonReleased;

			var npcloader = new Editors.NpcManager();
			helper.Content.AssetLoaders.Add(npcloader);
			helper.Content.AssetEditors.Add(npcloader);
			
			var objecteditor = new Editors.ObjectDisplayNameEditor();
			helper.Content.AssetEditors.Add(objecteditor);

			if (Config.EnableHairstyles)
			{
				var hairloader = new Editors.HairstylesManager();
				helper.Content.AssetLoaders.Add(hairloader);
				helper.Content.AssetEditors.Add(hairloader);
			}
		}

		#region Game Events

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			GenerateCat();

			JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
			if (JsonAssets == null)
			{
				Log.E("Can't access the Json Assets API. Is the mod installed correctly?");
				return;
			}
			
			foreach (string pack in ModConsts.HatPacks)
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.HatsDir, pack));
			foreach (string pack in ModConsts.ClothingPacks)
				JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", ModConsts.ClothingDir, pack));
		}

		private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			Helper.Content.InvalidateCache(Path.Combine("Data", "hats"));
			Helper.Content.InvalidateCache(Path.Combine("Data", "ClothingInformation"));
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (ShouldAddCatShopToday())
			{
				GameLocation location = Game1.getLocationFromName(ModConsts.CatLocation);
				AddCatToLocation(location);
				RestockCatShop();
			}
		}
		
		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			GameLocation location = Game1.getLocationFromName(ModConsts.CatLocation);
			RemoveCatFromLocation(location);
		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			if (e.Button.IsActionButton()
				&& Game1.game1.IsActive && Context.IsPlayerFree
				&& Game1.currentLocation?.isCharacterAtTile(e.Cursor.GrabTile) is NPC npc
				&& npc?.Name == ModConsts.CatId)
			{
				this.UseCatShop();
			}
		}

		#endregion

		#region Cat Methods

		private static void GenerateCat()
		{
			if (CatNpc != null)
				return;

			CatNpc = new NPC(
				sprite: new AnimatedSprite(ModConsts.GameContentCatSpritesPath, 0, 16, 32),
				position: Utility.PointToVector2(ModConsts.CatPosition) * 64.0f,
				defaultMap: ModConsts.CatLocation,
				facingDirection: 2,
				name: ModConsts.CatId,
				datable: false,
				schedule: null,
				portrait: Game1.content.Load
					<Texture2D>
					(ModConsts.GameContentCatPortraitPath))
			{
				Age = 2,
				Breather = false
			};
		}

		public static bool ShouldAddCatShopToday()
		{
			bool goodDay = Game1.stats.DaysPlayed > 3 && Game1.dayOfMonth % 7 <= 1;
			bool debugging = Config.DebugMode && Config.DebugCaturday;
			return goodDay || debugging;
		}

		private static void AddCatToLocation(GameLocation location)
		{
			if (!Context.IsMainPlayer || location == null || location.getCharacterFromName(ModConsts.CatId) != null)
				return;
			CatNpc.modData.Remove(ModConsts.CatMutexKey);
			ForceNpcSchedule(CatNpc);
			location.addCharacter(CatNpc);
		}

		private static void ForceNpcSchedule(NPC npc)
		{
			try
			{
				npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
				npc.ignoreScheduleToday = false;
				npc.followSchedule = true;
			}
			catch (Exception e)
			{
				Log.D($"Caught exception in ForceNpcSchedule:\n{e}",
					Config.DebugMode);
			}
		}

		private static void RemoveCatFromLocation(GameLocation location)
		{
			if (Context.IsMainPlayer && CatNpc != null)
			{
				CatNpc.modData.Remove(ModConsts.CatMutexKey);
				if (location?.getCharacterFromName(CatNpc.Name) != null)
				{
					location.characters.Remove(CatNpc);
				}
			}
		}

		#endregion

		#region CatShop Methods

		internal static string ContentPackNameToId(string name)
		{
			return Regex.Replace(ModConsts.ContentPackPrefix + name,
				"[^a-zA-Z0-9_.]", "");
		}

		private static int GetContentPackCost(string name)
		{
			return ModConsts.ClothingPackCosts.ContainsKey(name)
				? ModConsts.ClothingPackCosts[name]
				: ModConsts.DefaultClothingCost;
		}

		private static void RestockCatShop()
		{
			CatShopStock.Clear();
			PopulateCatShop(isHat: true);
			PopulateCatShop(isHat: false);
		}

		private static void PopulateCatShop(bool isHat)
		{
			try
			{
				List<string> stock = new List<string>();
				IEnumerable<string> contentPacks = isHat
					? ModConsts.HatPacks
					: ModConsts.ClothingPacks.Concat(Config.ExtraContentPacksToSellInTheShop);
				
				foreach (string contentPack in contentPacks)
				{
					string contentPackId = !ModConsts.HatPacks.Contains(contentPack) && !ModConsts.ClothingPacks.Contains(contentPack)
						? contentPack 
						: ContentPackNameToId(contentPack);
					List<string> contentNames = isHat
						? JsonAssets.GetAllHatsFromContentPack(contentPackId)
						: JsonAssets.GetAllClothingFromContentPack(contentPackId);
					if (contentNames == null || contentNames.Count < 1)
					{
						if (!ModConsts.HatPacks.Contains(contentPack) && !ModConsts.ClothingPacks.Contains(contentPack))
						{
							Log.D($"Did not add content from {contentPack} (as {contentPackId}): Not found.",
								Config.DebugMode);
							continue;
						}
						throw new NullReferenceException($"Failed to add content from {contentPack}\n({contentPackId}).");
					}
					if (contentPack.EndsWith("Kimono"))
					{
						contentNames.RemoveAll(name => name.EndsWith("wer"));
					}

					stock.Clear();
					int currentQty = 0;
					int goalQty = Math.Max(1, contentNames.Count / ModConsts.CatShopQtyRatio);
					while (currentQty < goalQty)
					{
						string name = contentNames[Game1.random.Next(contentNames.Count - 1)];

						if (stock.Contains(name))
							continue;
						++currentQty;
						stock.Add(name);

						if (!contentPack.EndsWith("Kimono"))
							continue;
						++currentQty;
						stock.Add(name.Replace("Upp", "Low"));
					}

					foreach (string name in stock)
					{
						if (isHat)
							CatShopStock.Add(new StardewValley.Objects.Hat(
								JsonAssets.GetHatId(name)), new[] { GetContentPackCost(contentPack), 1 });
						else
							CatShopStock.Add(new StardewValley.Objects.Clothing(
								JsonAssets.GetClothingId(name)), new[] { GetContentPackCost(contentPack), 1 });
					}
				}
			}
			catch (Exception ex)
			{
				Log.E("Sailor Styles failed to populate the clothes shop."
					+ " Did you remove the clothing folders, or did I break something?");
				Log.E("Exception logged:\n" + ex);
			}
		}

		private void UseCatShop()
		{
			// Choose a shop dialogue visible in large viewports
			Random random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
			string whichDialogue = ModConsts.ShopDialogueRoot + (1 + random.Next(5));
			if (whichDialogue.EndsWith("5")) // Dialogue 5 is seasonal
				whichDialogue += $".{Game1.currentSeason}";
			Translation text = i18n.Get(whichDialogue);

			Game1.playSound("cat");
			if (CatNpc.modData.ContainsKey(ModConsts.CatMutexKey))
			{
				//Game1.playSound("cancel");
			}
			else
			{
				// Open the cat shop
				CatNpc.modData[ModConsts.CatMutexKey] = "";
				ShopMenu shopMenu = new ShopMenu(itemPriceAndStock: CatShopStock)
				{
					portraitPerson = CatNpc,
					potraitPersonDialogue = Game1.parseText(text: text, whichFont: Game1.dialogueFont, width: 304)
				};
				Game1.activeClickableMenu = shopMenu;
				Game1.activeClickableMenu.exitFunction = delegate
				{
					CatNpc.modData.Remove(ModConsts.CatMutexKey);
				};
			}
		}

		#endregion
	}
}
