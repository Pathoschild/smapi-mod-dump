/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/EarlyCommunityUpgrades
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace EarlyCommunityUpgrades
{
	public class CommunityUpgradesPatches
	{

		public static bool customRequirementsMet = false;
		private static bool pamsHouseBuilt;
		private static bool shortcutsBuilt;

		///<summary>
		///Attempts to Harmony patch the following:
		///<list type="bullet">
		///	<item>
		///		<description>GameLocation.communityUpgradeAccept -> CommunityUpgradePatches.CommunityUpgradeAccept_Prefix</description>
		///	</item>
		///	<item>
		///		<description>GameLocation.carpenters -> CommunityUpgradePatches.Carpenters_Prefix</description>
		///	</item>
		///</list>
		///</summary>
		/// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
		public static bool ApplyHarmonyPatches()
		{
			try
			{
				HarmonyInstance harmony = HarmonyInstance.Create(Globals.Manifest.UniqueID);
				harmony.Patch(
					original: AccessTools.Method(typeof(GameLocation), "communityUpgradeAccept"),
					prefix: new HarmonyMethod(typeof(CommunityUpgradesPatches), nameof(CommunityUpgradesPatches.CommunityUpgradeAccept_Prefix))
					);

				harmony.Patch(
					original: typeof(GameLocation).GetMethod("carpenters"),
					prefix: new HarmonyMethod(typeof(CommunityUpgradesPatches).GetMethod("Carpenters_Prefix"))
					);
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log(e.ToString(), LogLevel.Error);
				return false;
			}
		}

		/// <summary>
		/// Harmony patch for <c>GameLocation.carpenters</c>
		/// </summary>
		/// <param name="__instance" />
		/// <param name="tileLocation" />
		/// <returns>
		/// <c>True</c> if unable to patch, so that the original method runs instead.
		/// <c>False</c> if successfully patched, in order to skip the original method.
		/// </returns>
		public static bool Carpenters_Prefix(GameLocation __instance, ref Location tileLocation)
		{
			try
			{
				// flags should be set independently of route (CC or Joja)
				int numRoomsCompleted = new[] { "ccFishTank", "ccPantry", "ccCraftsRoom", "ccBoilerRoom", "ccVault", "ccBulletin" }.Count(mail => Game1.MasterPlayer.hasOrWillReceiveMail(mail));
				int houseUpgradeLevel = Game1.MasterPlayer.houseUpgradeLevel.Value;
				int friendshipHeartsGained = Game1.player.friendshipData.Any() ? Game1.player.friendshipData.Keys.Sum(name => Game1.player.getFriendshipHeartLevelForNPC(name)) : 0;

				customRequirementsMet = numRoomsCompleted >= Globals.Config.Requirements.numRoomsCompleted &&
					houseUpgradeLevel >= Globals.Config.Requirements.numFarmhouseUpgrades &&
					friendshipHeartsGained >= Globals.Config.Requirements.numFriendshipHeartsGained;

				if (Game1.player.currentUpgrade == null)
				{
					foreach (NPC i in __instance.characters)
					{
						if (!i.Name.Equals("Robin"))
							continue;

						if (Vector2.Distance(i.getTileLocation(), new Vector2(tileLocation.X, tileLocation.Y)) > 3f)
							return false;

						i.faceDirection(2);
						if (Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
						{
							List<Response> options = new List<Response>
							{
								new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop"))
							};
							// remove requirement that only the host can perform community upgrades 
							//if (Game1.IsMasterGame)
							//{
							if (Game1.IsMasterGame && Game1.MasterPlayer.houseUpgradeLevel < 3)
								options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));

							else if (Game1.player.houseUpgradeLevel < 3)
								options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));

							// replace vanilla requirements with custom defined ones
							if (customRequirementsMet)
							{
								if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
									options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));

								else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
									options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
							}

							if (Game1.player.houseUpgradeLevel >= 2)
							{
								if (Game1.IsMasterGame)
									options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));

								else
									options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
							}

							options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
							options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));

							__instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
						}
						else
							Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");

						return false;
					}
					if (__instance.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
					{
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
						Game1.afterDialogues = delegate
						{
							Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
						};
						return false;
					}
					if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
					{
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
						return false;
					}
					return false;
				}
				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(Carpenters_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}

		/// <summary>
		/// Harmony patch for <c>GameLocation.communityUpgradeAccept</c>
		/// </summary>
		/// <param name="__instance" />
		/// <param name="tileLocation" />
		/// <returns>
		/// <c>True</c> if unable to patch, so that the original method runs instead.
		/// <c>False</c> if successfully patched, in order to skip the original method.
		/// </returns>
		public static bool CommunityUpgradeAccept_Prefix()
		{
			try
			{
				Multiplayer multiplayer = Globals.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

				if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
				{
					if (Game1.player.Money >= Globals.Config.Costs.pamCostGold && Game1.player.hasItemInInventory(388, Globals.Config.Costs.pamCostWood))
					{
						Game1.player.Money -= Globals.Config.Costs.pamCostGold;
						Game1.player.removeItemsFromInventory(388, Globals.Config.Costs.pamCostWood);
						Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_PamUpgrade_Accepted"));
						Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
						(Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value = 3;
						multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
					}
					else if (Game1.player.Money < Globals.Config.Costs.pamCostGold)
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));

					else
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood3"));

				}
				else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
				{
					if (Game1.player.Money >= Globals.Config.Costs.shortcutCostGold)
					{
						Game1.player.Money -= Globals.Config.Costs.shortcutCostGold;
						Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
						Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
						(Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value = 3;
						multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
					}
					else if (Game1.player.Money < Globals.Config.Costs.shortcutCostGold)
						Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));

				}

				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(CommunityUpgradeAccept_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}

		public static void CheckInstantUnlocks()
		{
			if (pamsHouseBuilt && shortcutsBuilt) return;

			if (Globals.Config.InstantUnlocks.pamsHouse && !pamsHouseBuilt)
			{
				Game1.MasterPlayer.mailReceived.Add("pamHouseUpgrade");
				Game1.player.eventsSeen.Add(611173);
				pamsHouseBuilt = true;
			}

			if (Globals.Config.InstantUnlocks.shortcuts && !shortcutsBuilt)
			{
				Game1.MasterPlayer.mailReceived.Add("communityUpgradeShortcuts");
				shortcutsBuilt = true;
			}
		}

	}
}
