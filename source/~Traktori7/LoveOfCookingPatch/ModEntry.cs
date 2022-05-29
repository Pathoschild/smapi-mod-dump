/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace LoveOfCookingPatch
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			HarmonyPatches.Initialize(Monitor, helper.Translation);

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
		{
			IModInfo modInfo = Helper.ModRegistry.Get("blueberry.LoveOfCooking")!;

			// Is the version new enough to contain the calendar exclusion
			if (modInfo.Manifest.Version.CompareTo(new SemanticVersion("1.0.27")) == 0)
			{
				Monitor.Log("Found Love of Cooking version 1.0.27, applying harmony patch...", LogLevel.Info);

				var harmony = new Harmony(ModManifest.UniqueID);

				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.showHoldingItem))
							?? throw new InvalidOperationException("Can't find Farmer.showHoldingItem to patch"),
						postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.ShowHoldingItem_postfix))
					);
			}
			else
			{
				Monitor.Log("Your Love of Cooking is not version 1.0.27, and this patch is not needed. Please remove it.", LogLevel.Alert);
			}
		}
	}

	public class HarmonyPatches
	{
		private static IMonitor Monitor = null!;
		private static ITranslationHelper Translator = null!;


		public static void Initialize(IMonitor monitor, ITranslationHelper translation)
		{
			Monitor = monitor;
			Translator = translation;
		}


		public static void ShowHoldingItem_postfix(ref Farmer who)
		{
			try
			{
				if (!LoveOfCooking.Objects.CookingTool.IsItemCookingTool(who.mostRecentlyGrabbedItem))
				{
					Monitor.Log("The held item wasn't the cooking tool, running the base game's logic", LogLevel.Info);

					if (who.mostRecentlyGrabbedItem is SpecialItem)
					{
						TemporaryAnimatedSprite t = (who.mostRecentlyGrabbedItem as SpecialItem)!.getTemporarySpriteForHoldingUp(who.Position + new Vector2(0f, -124f));
						t.motion = new Vector2(0f, -0.1f);
						t.scale = 4f;
						t.interval = 2500f;
						t.totalNumberOfLoops = 0;
						t.animationLength = 1;
						Game1.currentLocation.temporarySprites.Add(t);
					}
					else if (who.mostRecentlyGrabbedItem is Slingshot)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", Game1.getSquareSourceRectForNonStandardTileSheet(Tool.weaponsTexture, 16, 16, (who.mostRecentlyGrabbedItem as Slingshot)!.IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
					else if (who.mostRecentlyGrabbedItem is MeleeWeapon)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", Game1.getSquareSourceRectForNonStandardTileSheet(Tool.weaponsTexture, 16, 16, (who.mostRecentlyGrabbedItem as MeleeWeapon)!.IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
					else if (who.mostRecentlyGrabbedItem is Boots)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSquareSourceRectForNonStandardTileSheet(Game1.objectSpriteSheet, 16, 16, (who.mostRecentlyGrabbedItem as Boots)!.indexInTileSheet.Value), 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
					else if (who.mostRecentlyGrabbedItem is Hat)
					{
						Hat hat = (who.mostRecentlyGrabbedItem as Hat)!;
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Farmer\\hats", new Rectangle(hat.which.Value * 20 % FarmerRenderer.hatsTexture.Width, hat.which.Value * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20), 2500f, 1, 0, who.Position + new Vector2(-8f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
					else if (who.mostRecentlyGrabbedItem is Tool)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\tools", Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, (who.mostRecentlyGrabbedItem as Tool)!.IndexOfMenuItemView), 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
					else if (who.mostRecentlyGrabbedItem is Furniture)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\furniture", (who.mostRecentlyGrabbedItem as Furniture)!.sourceRect.Value, 2500f, 1, 0, who.Position + new Vector2(32 - (who.mostRecentlyGrabbedItem as Furniture)!.sourceRect.Width / 2 * 4, -188f), flicker: false, flipped: false)
						{
							motion = new Vector2(0f, -0.1f),
							scale = 4f,
							layerDepth = 1f
						});
					}
					else if (who.mostRecentlyGrabbedItem is SObject && !(who.mostRecentlyGrabbedItem as SObject)!.bigCraftable.Value)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, who.mostRecentlyGrabbedItem.ParentSheetIndex, 16, 16), 2500f, 1, 0, who.Position + new Vector2(0f, -124f), flicker: false, flipped: false)
						{
							motion = new Vector2(0f, -0.1f),
							scale = 4f,
							layerDepth = 1f
						});
						if (who.IsLocalPlayer && Utility.IsNormalObjectAtParentSheetIndex(who.mostRecentlyGrabbedItem, 434))
						{
							who.eatHeldObject();
						}
					}
					else if (who.mostRecentlyGrabbedItem is SObject)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\Craftables", Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, who.mostRecentlyGrabbedItem.ParentSheetIndex, 16, 32), 2500f, 1, 0, who.Position + new Vector2(0f, -188f), flicker: false, flipped: false)
						{
							motion = new Vector2(0f, -0.1f),
							scale = 4f,
							layerDepth = 1f
						});
					}
					else if (who.mostRecentlyGrabbedItem is Ring)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, who.mostRecentlyGrabbedItem.ParentSheetIndex, 16, 16), 2500f, 1, 0, who.Position + new Vector2(-4f, -124f), flicker: false, flipped: false)
						{
							motion = new Vector2(0f, -0.1f),
							scale = 4f,
							layerDepth = 1f
						});
					}
					if (who.mostRecentlyGrabbedItem == null)
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(420, 489, 25, 18), 2500f, 1, 0, who.Position + new Vector2(-20f, -152f), flicker: false, flipped: false)
						{
							motion = new Vector2(0f, -0.1f),
							scale = 4f,
							layerDepth = 1f
						});
					}
					else
					{
						Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(10, who.Position + new Vector2(32f, -96f), Color.White)
						{
							motion = new Vector2(0f, -0.1f)
						});
					}
				}
			}
			catch (Exception e)
			{
				Monitor.Log("Mod failed at prefixing Farmer.showHeldItem", LogLevel.Error);
				Monitor.Log(e.ToString(), LogLevel.Error);
			}
		}
	}
}
