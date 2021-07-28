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
	public static class Tools
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Config;
		private static IReflectionHelper Reflection => ModEntry.Instance.Helper.Reflection;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		// Add Cooking Tool
		internal const int CookingToolSheetIndex = 17;
		internal const string CookingToolBaseName = ModEntry.ObjectPrefix + "cookingtool";

		internal static void RegisterEvents()
		{
			Helper.Events.Display.MenuChanged += Display_MenuChanged;
			Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
			Helper.Events.Player.InventoryChanged += Player_InventoryChanged;
		}

		internal static void AddConsoleCommands(string cmd)
		{
			Helper.ConsoleCommands.Add(cmd + "spawnpan", "Add a broken frying pan object to inventory. Not very useful.", (s, args) =>
			{
				int level = args.Length > 0 ? int.Parse(args[0]) : 0;
				level = Math.Max(0, Math.Min(3, level));
				Tool tool = GenerateCookingTool(level);
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
					bool canUpgrade = CanFarmerUpgradeCookingEquipment();
					int level = ModEntry.Instance.States.Value.CookingToolLevel;
					if (canUpgrade)
					{
						if (e.NewMenu is ShopMenu upgradeMenu && upgradeMenu.itemPriceAndStock.Keys.All(key => key.Name != "Coal"))
						{
							StardewValley.Tools.GenericTool cookingTool = GenerateCookingTool(level);
							int price = Helper.Reflection.GetMethod(
								typeof(Utility), "priceForToolUpgradeLevel").Invoke<int>(level + 1);
							int index = Helper.Reflection.GetMethod(
								typeof(Utility), "indexOfExtraMaterialForToolUpgrade").Invoke<int>(level + 1);
							upgradeMenu.itemPriceAndStock.Add(cookingTool, new int[3] { price / 2, 1, index });
							upgradeMenu.forSale.Add(cookingTool);
						}
					}
				}

				return;
			}
		}

		private static void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			// Checks keyboard+mouse menu interactions for purchasing cooking tool upgrades from Clint's upgrade tools menu
			if (BlacksmithCheck())
			{
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu menu
					&& menu.heldItem != null && menu.heldItem is StardewValley.Tools.GenericTool tool)
				{
					Log.D($"Checking tool upgrade (ButtonPressed) for {tool?.Name}",
						Config.DebugMode);
					if (IsThisCookingTool(tool))
					{
						// Set up the frying pan upgrade
						BlacksmithPrepareFryingPanUpgrade(tool);
					}
				}
			}
		}

		private static void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			// Checks gamepad (snappy) menu interactions for purchasing cooking tool upgrades from Clint's upgrade tools menu
			// This causes a 'pop' sound to play, but it's otherwise all we need.
			if (BlacksmithCheck())
			{
				foreach (Item item in e.Added)
				{
					if (item is StardewValley.Tools.GenericTool tool)
					{
						Log.D($"Checking tool upgrade (InventoryChanged) for {item?.Name}",
							Config.DebugMode);
						if (IsThisCookingTool(tool))
						{
							// Set up the frying pan upgrade
							BlacksmithPrepareFryingPanUpgrade(tool);
							// Remove item added to inventory
							Game1.player.removeItemFromInventory(tool);
							// Remove new item received popup
							var message = Game1.hudMessages.FirstOrDefault(message => message?.type == tool.DisplayName);
							Game1.hudMessages.Remove(message);
						}
					}
				}
			}
		}

		private static bool BlacksmithCheck()
		{
			return Game1.currentLocation?.Name == "Blacksmith";
		}

		private static void BlacksmithPrepareFryingPanUpgrade(Tool tool)
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

		public static StardewValley.Tools.GenericTool GenerateCookingTool(int level)
		{
			string name = GetCookingToolDisplayName(level);
			string description = i18n.Get("menu.cooking_equipment.description", new { level = level + 2 }).ToString();
			var tool = new StardewValley.Tools.GenericTool(name, description, level + 1, CookingToolSheetIndex + level, CookingToolSheetIndex + level);
			return tool;
		}

		public static string GetCookingToolDisplayName(int level)
		{
			string localisedName = i18n.Get("menu.cooking_equipment.name").ToString();
			string displayName = string.Format(Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs." + (14299 + level)), localisedName);
			return displayName;
		}

		public static bool IsThisCookingTool(Tool tool)
		{
			int index = tool.IndexOfMenuItemView - CookingToolSheetIndex;
			return index >= 0 && index < 4;
		}

		public static bool CanFarmerUpgradeCookingEquipment()
		{
			bool hasMail = Game1.player.mailReceived.Contains(ModEntry.MailCookbookUnlocked);
			bool level = ModEntry.Instance.States.Value.CookingToolLevel < 4;
			return hasMail && level;
		}

		public static void PurgeBrokenFryingPans(bool sendMail)
		{
			Log.D("Checking for broken cooking tools.",
				Config.DebugMode);

			Translation name = i18n.Get("menu.cooking_equipment.name");
			int found = 0;

			for (int i = Game1.player.Items.Count - 1; i >= 0; --i)
			{
				if (Game1.player.Items[i] == null
					|| (!Game1.player.Items[i].Name.EndsWith(name) && !Game1.player.Items[i].Name.EndsWith(ModEntry.AssetPrefix + "tool")))
					continue;

				Log.D($"Removing a broken Cooking tool in {Game1.player.Name}'s inventory slot {i}.",
					Config.DebugMode);

				++found;
				Game1.player.removeItemFromInventory(i);
			}

			foreach (GameLocation location in Game1.locations)
			{
				foreach (var chest in location.Objects.Values.Where(o => o != null && o is Chest chest && chest.items.Count > 0))
				{
					for (int i = ((Chest)chest).items.Count - 1; i >= 0; --i)
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
