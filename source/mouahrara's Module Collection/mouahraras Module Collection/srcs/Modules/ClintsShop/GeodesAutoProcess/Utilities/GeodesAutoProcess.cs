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
using mouahrarasModuleCollection.ClintsShop.GeodesAutoProcess.Hooks;

namespace mouahrarasModuleCollection.ClintsShop.GeodesAutoProcess.Utilities
{
	internal class GeodesAutoProcessUtility
	{
		private static readonly PerScreen<GeodeMenu>	geodeMenu = new(() => null);
		private static readonly PerScreen<Item>			geodeBeingProcessed = new(() => null);
		private static readonly PerScreen<Item>			foundArtifact = new(() => null);

		internal static void SetGeodeMenu(GeodeMenu value)
		{
			geodeMenu.Value = value;
		}

		internal static GeodeMenu GetGeodeMenu()
		{
			return geodeMenu.Value;
		}

		internal static void SetGeodeBeingProcessed(Item value)
		{
			geodeBeingProcessed.Value = value;
		}

		internal static Item GetGeodeBeingProcessed()
		{
			return geodeBeingProcessed.Value;
		}

		internal static void SetFoundArtifact(Item value)
		{
			foundArtifact.Value = value;
		}

		internal static Item GetFoundArtifact()
		{
			return foundArtifact.Value;
		}

		internal static void InitializeAfterOpeningGeodeMenu(GeodeMenu __instance)
		{
			SetGeodeMenu(__instance);
			SetFoundArtifact(null);
		}

		internal static void CleanBeforeClosingGeodeMenu()
		{
			EndGeodeProcessing();
			SetFoundArtifact(null);
			SetGeodeMenu(null);
		}

		internal static void StartGeodeProcessing()
		{
			SetGeodeBeingProcessed(GetGeodeMenu().heldItem);
			GetGeodeMenu().heldItem = null;
			GetGeodeMenu().inventory.highlightMethod = (Item _) => false;
			ModEntry.Helper.Events.GameLoop.UpdateTicking += UpdateTickingHook.Apply;
		}

		internal static void EndGeodeProcessing()
		{
			ModEntry.Helper.Events.GameLoop.UpdateTicking -= UpdateTickingHook.Apply;
			GetGeodeMenu().inventory.highlightMethod = GetGeodeMenu().highlightGeodes;
			if (IsProcessing())
				Game1.player.addItemToInventory(GetGeodeBeingProcessed());
			SetGeodeBeingProcessed(null);
		}

		internal static bool IsProcessing()
		{
			return GetGeodeBeingProcessed() != null && GetGeodeBeingProcessed().Stack > 0;
		}

		internal static void CrackGeodeSecure()
		{
			if (!CanProcess())
				return;

			GeodeMenu geodeMenu = GetGeodeMenu();
			Item geodeBeingProcessed = GetGeodeBeingProcessed();

			geodeMenu.geodeSpot.item = new Object(geodeBeingProcessed.ParentSheetIndex, geodeBeingProcessed.Stack);
			if (geodeMenu.geodeSpot.item.ParentSheetIndex == 791 && !Game1.netWorldState.Value.GoldenCoconutCracked.Value)
			{
				geodeMenu.waitingForServerResponse = true;
				Game1.player.team.goldenCoconutMutex.RequestLock(delegate
				{
					geodeMenu.waitingForServerResponse = false;
					geodeMenu.geodeTreasureOverride = new Object(73, 1);
					CrackGeode();
				}, delegate
				{
					geodeMenu.waitingForServerResponse = false;
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
			GeodeMenu geodeMenu = GetGeodeMenu();
			Item geodeBeingProcessed = GetGeodeBeingProcessed();

			if (geodeMenu.waitingForServerResponse)
			{
				return false;
			}
			if (geodeBeingProcessed.Stack <= 0 || Game1.player.Money < 25)
			{
				EndGeodeProcessing();
				return false;
			}
			if (!(Game1.player.freeSpotsInInventory() > 1 || (Game1.player.freeSpotsInInventory() == 1 && geodeBeingProcessed.Stack == 1)))
			{
				EndGeodeProcessing();
				geodeMenu.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
				geodeMenu.wiggleWordsTimer = 500;
				geodeMenu.alertTimer = 1500;
				return false;
			}
			return true;
		}

		private static void CrackGeode()
		{
			GeodeMenu geodeMenu = GetGeodeMenu();
			Item geodeBeingProcessed = GetGeodeBeingProcessed();

			geodeBeingProcessed.Stack--;
			Game1.player.Money -= 25;
			Game1.playSound("stoneStep");
			geodeMenu.geodeAnimationTimer = 2700;
			geodeMenu.clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
			{
				new(8, 300),
				new(9, 200),
				new(10, 80),
				new(11, 200),
				new(12, 100),
				new(8, 300)
			});
			geodeMenu.clint.loop = false;
		}
	}
}
