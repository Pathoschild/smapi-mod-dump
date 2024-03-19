/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Handlers;

namespace mouahrarasModuleCollection.TweaksAndFeatures.Shops.GeodesAutoProcess.Utilities
{
	internal class GeodesAutoProcessUtility
	{
		private static readonly PerScreen<GeodeMenu>	geodeMenu = new(() => null);
		private static readonly PerScreen<Item>			geodeBeingProcessed = new(() => null);
		private static readonly PerScreen<Item>			foundArtifact = new(() => null);

		internal static GeodeMenu GeodeMenu
		{
			get => geodeMenu.Value;
			set => geodeMenu.Value = value;
		}

		internal static Item GeodeBeingProcessed
		{
			get => geodeBeingProcessed.Value;
			set => geodeBeingProcessed.Value = value;
		}

		internal static Item FoundArtifact
		{
			get => foundArtifact.Value;
			set => foundArtifact.Value = value;
		}

		internal static void InitializeAfterOpeningGeodeMenu(GeodeMenu __instance)
		{
			GeodeMenu = __instance;
			FoundArtifact = null;
		}

		internal static void CleanBeforeClosingGeodeMenu()
		{
			EndGeodeProcessing();
			FoundArtifact = null;
			GeodeMenu = null;
		}

		internal static void StartGeodeProcessing()
		{
			GeodeBeingProcessed = GeodeMenu.heldItem;
			GeodeMenu.heldItem = null;
			GeodeMenu.inventory.highlightMethod = (Item _) => false;
			ModEntry.Helper.Events.GameLoop.UpdateTicking += UpdateTickingHandler.Apply;
		}

		internal static void EndGeodeProcessing()
		{
			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHandler.Apply;
			GeodeMenu.inventory.highlightMethod = GeodeMenu.highlightGeodes;
			if (IsProcessing())
				Game1.player.addItemToInventory(GeodeBeingProcessed);
			GeodeBeingProcessed = null;
		}

		internal static bool IsProcessing()
		{
			return GeodeBeingProcessed != null && GeodeBeingProcessed.Stack > 0;
		}

		internal static void CrackGeodeSecure()
		{
			if (!CanProcess())
				return;

			GeodeMenu.geodeSpot.item = ItemRegistry.Create(GeodeBeingProcessed.QualifiedItemId);
			if (GeodeMenu.geodeSpot.item.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
			{
				GeodeMenu.waitingForServerResponse = true;
				Game1.player.team.goldenCoconutMutex.RequestLock(delegate
				{
					GeodeMenu.waitingForServerResponse = false;
					GeodeMenu.geodeTreasureOverride = ItemRegistry.Create("(O)73");
					CrackGeode();
				}, delegate
				{
					GeodeMenu.waitingForServerResponse = false;
					CrackGeode();
				});
			}
			else
			{
				CrackGeode();
			}
			CanProcess();
		}

		private static bool CanProcess()
		{
			if (GeodeMenu.waitingForServerResponse)
			{
				return false;
			}
			if (GeodeBeingProcessed.Stack <= 0 || Game1.player.Money < 25)
			{
				EndGeodeProcessing();
				return false;
			}
			if (!(Game1.player.freeSpotsInInventory() > 1 || (Game1.player.freeSpotsInInventory() == 1 && GeodeBeingProcessed.Stack == 1)))
			{
				EndGeodeProcessing();
				GeodeMenu.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
				GeodeMenu.wiggleWordsTimer = 500;
				GeodeMenu.alertTimer = 1500;
				return false;
			}
			return true;
		}

		private static void CrackGeode()
		{
			GeodeBeingProcessed.Stack--;
			Game1.player.Money -= 25;
			Game1.playSound("stoneStep");
			GeodeMenu.geodeAnimationTimer = 2700;
			GeodeMenu.clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
			{
				new(8, 300),
				new(9, 200),
				new(10, 80),
				new(11, 200),
				new(12, 100),
				new(8, 300)
			});
			GeodeMenu.clint.loop = false;
		}
	}
}
