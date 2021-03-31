/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Linq;

namespace LoveOfCooking
{
	public class Tools
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Instance.Config;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		// Add Cooking Tool
		internal const int CookingToolSheetIndex = 17;
		internal const string CookingToolBaseName = ModEntry.ObjectPrefix + "cookingtool";

		internal static void RegisterEvents()
		{
			Helper.Events.Display.MenuChanged += Display_MenuChanged;
			Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
		}

		internal static void AddConsoleCommands(string cmd)
		{
			Helper.ConsoleCommands.Add(cmd + "spawnpan", "Add a broken frying pan object to inventory. Not very useful.", (s, args) =>
			{
				var level = args.Length > 0 ? int.Parse(args[0]) : 0;
				level = Math.Max(0, Math.Min(3, level));
				var tool = GenerateCookingTool(level);
				Game1.player.addItemByMenuIfNecessary(tool);
			});
			Helper.ConsoleCommands.Add(cmd + "purgepan", "Remove broken frying pan objects from inventories and chests.", (s, args) =>
			{
				Log.D($"Purging frying pans{(args.Length > 0 ? " and sending mail." : ".")}");
				PurgeBrokenFryingPans(sendMail: args.Length > 0);
			});
		}

		internal static void SaveLoadedBehaviours()
		{
			PurgeBrokenFryingPans(sendMail: true);
		}

		private static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			// Checks for purchasing a cooking tool upgrade from Clint's upgrade menu
			if (Game1.currentLocation?.Name == "Blacksmith")
			{
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu menu
					&& menu.heldItem != null && menu.heldItem is StardewValley.Tools.GenericTool tool)
				{
					Log.D($"Checking tool upgrade for {tool?.Name}",
						Config.DebugMode);
					if (IsThisCookingTool(tool))
					{
						Log.D($"Upgrading {tool?.Name}",
							Config.DebugMode);
						Game1.player.toolBeingUpgraded.Value = tool;
						Game1.player.daysLeftForToolUpgrade.Value = Config.DebugMode ? 0 : 2;
						Game1.playSound("parry");
						Game1.exitActiveMenu();
						Game1.drawDialogue(Game1.getCharacterFromName("Clint"),
							Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14317"));
					}
				}
			}
		}

		private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.OldMenu is TitleMenu || e.NewMenu is TitleMenu || Game1.currentLocation == null || Game1.player == null)
				return;

			// Add new objects to shop menus and edit shop stock
			if (e.NewMenu is ShopMenu menu && menu != null)
			{
				if (Config.AddCookingToolProgression && Game1.currentLocation?.Name == "Blacksmith")
				{
					// Upgrade cooking equipment at the blacksmith
					var canUpgrade = CanFarmerUpgradeCookingEquipment();
					var level = ModEntry.Instance.States.Value.CookingToolLevel;
					if (canUpgrade)
					{
						if (e.NewMenu is ShopMenu upgradeMenu && upgradeMenu.itemPriceAndStock.Keys.All(key => key.Name != "Coal"))
						{
							var cookingTool = GenerateCookingTool(level);
							var price = Helper.Reflection.GetMethod(
								typeof(Utility), "priceForToolUpgradeLevel").Invoke<int>(level + 1);
							var index = Helper.Reflection.GetMethod(
								typeof(Utility), "indexOfExtraMaterialForToolUpgrade").Invoke<int>(level + 1);
							upgradeMenu.itemPriceAndStock.Add(cookingTool, new int[3] { price / 2, 1, index });
							upgradeMenu.forSale.Add(cookingTool);
						}
					}
				}

				return;
			}
		}

		public static StardewValley.Tools.GenericTool GenerateCookingTool(int level)
		{
			var name = GetCookingToolDisplayName(level);
			var description = i18n.Get("menu.cooking_equipment.description", new { level = level + 2 }).ToString();
			var tool = new StardewValley.Tools.GenericTool(name, description, level + 1, CookingToolSheetIndex + level, CookingToolSheetIndex + level);
			return tool;
		}

		public static string GetCookingToolDisplayName(int level)
		{
			var localisedName = i18n.Get("menu.cooking_equipment.name").ToString();
			var displayName = string.Format(Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs." + (14299 + level)), localisedName);
			return displayName;
		}

		public static bool IsThisCookingTool(Tool tool)
		{
			var index = tool.IndexOfMenuItemView - CookingToolSheetIndex;
			return index >= 0 && index < 4;
		}

		public static bool CanFarmerUpgradeCookingEquipment()
		{
			var hasMail = Game1.player.mailReceived.Contains(ModEntry.MailCookbookUnlocked);
			var level = ModEntry.Instance.States.Value.CookingToolLevel < 4;
			return hasMail && level;
		}

		public static void PurgeBrokenFryingPans(bool sendMail)
		{
			Log.D("Checking for broken cooking tools.",
				Config.DebugMode);

			var name = i18n.Get("menu.cooking_equipment.name");
			var found = 0;

			for (var i = Game1.player.Items.Count - 1; i >= 0; --i)
			{
				if (Game1.player.Items[i] == null
					|| (!Game1.player.Items[i].Name.EndsWith(name) && !Game1.player.Items[i].Name.EndsWith(ModEntry.AssetPrefix + "tool")))
					continue;

				Log.D($"Removing a broken Cooking tool in {Game1.player.Name}'s inventory slot {i}.",
					Config.DebugMode);

				++found;
				Game1.player.removeItemFromInventory(i);
			}

			foreach (var location in Game1.locations)
			{
				foreach (var chest in location.Objects.Values.Where(o => o != null && o is Chest chest && chest.items.Count > 0))
				{
					for (var i = ((Chest)chest).items.Count - 1; i >= 0; --i)
					{
						if (((Chest)chest).items[i] == null
							|| (!((Chest)chest).items[i].Name.EndsWith(name) && !((Chest)chest).items[i].Name.EndsWith(ModEntry.AssetPrefix + "tool")))
							continue;

						Log.D($"Removing a broken Cooking tool in chest at {location.Name} {chest.TileLocation.ToString()} item slot {i}.",
							Config.DebugMode);

						++found;
						((Chest)chest).items.RemoveAt(i);
					}
				}
			}

			if (found > 0 && sendMail)
			{
				if (!Game1.player.mailReceived.Contains(ModEntry.MailFryingPanWhoops))
				{
					Game1.player.mailbox.Add(ModEntry.MailFryingPanWhoops);
				}
			}
		}
	}
}
