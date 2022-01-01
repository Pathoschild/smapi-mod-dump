/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ConvenientInventory.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace ConvenientInventory
{
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	public class ModEntry : Mod
	{
		public static ModEntry Context { get; private set; }

		public static ModConfig Config { get; private set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Context = this;
			Config = helper.ReadConfig<ModConfig>();

			ConvenientInventory.QuickStackButtonIcon = helper.Content.Load<Texture2D>(@"Assets\icon.png");
			ConvenientInventory.FavoriteItemsCursorTexture = helper.Content.Load<Texture2D>(@"Assets\favoriteCursor.png");
			ConvenientInventory.FavoriteItemsHighlightTexture = helper.Content.Load<Texture2D>($@"Assets\favoriteHighlight_{Config.FavoriteItemsHighlightTextureChoice}.png");
			ConvenientInventory.FavoriteItemsBorderTexture = helper.Content.Load<Texture2D>(@"Assets\favoriteBorder.png");

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saving += OnSaving;

			// TODO: Controller button should have different logic. Should toggle favorite on button_press, not button_hold+select.
			// TODO: Suppress inputs. That way, item is not picked up when toggling favorite.
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.Input.ButtonReleased += OnButtonReleased;
		}

		/// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			var harmony = new Harmony(this.ModManifest.UniqueID);

			// Manually patch InventoryPage constructor, otherwise Harmony cannot find method.
			harmony.Patch(
				original: AccessTools.Constructor(typeof(StardewValley.Menus.InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
				postfix: new HarmonyMethod(typeof(InventoryPageConstructorPatch), nameof(InventoryPageConstructorPatch.Postfix))
			);

			harmony.PatchAll();

			// Get Generic Mod Config Menu API (if it's installed)
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api != null)
			{
				api.RegisterModConfig(
					mod: ModManifest,
					revertToDefault: () => Config = new ModConfig(),
					saveToFile: () => Helper.WriteConfig(Config)
				);

				api.SetDefaultIngameOptinValue(ModManifest, true);

				api.RegisterLabel(
					mod: ModManifest,
					labelName: "Quick Stack To Nearby Chests",
					labelDesc: null
				);

				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Enable Quick stack?",
					optionDesc: "If enabled, adds a \"Quick Stack To Nearby Chests\" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.",
					optionGet: () => Config.IsEnableQuickStack,
					optionSet: value => Config.IsEnableQuickStack = value
				);
				api.RegisterClampedOption(
					mod: ModManifest,
					optionName: "Range",
					optionDesc: "How many tiles away from the player to search for nearby chests.",
					optionGet: () => Config.QuickStackRange,
					optionSet: value => Config.QuickStackRange = value,
					min: 0,
					max: 10,
					interval: 1
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Quick stack into buildings?",
					optionDesc: "If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.",
					optionGet: () => Config.IsQuickStackIntoBuildingsWithInventories,
					optionSet: value => Config.IsQuickStackIntoBuildingsWithInventories = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Quick stack overflow items?",
					optionDesc: "If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.",
					optionGet: () => Config.IsQuickStackOverflowItems,
					optionSet: value => Config.IsQuickStackOverflowItems = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Show nearby chests in tooltip?",
					optionDesc: "If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.",
					optionGet: () => Config.IsQuickStackTooltipDrawNearbyChests,
					optionSet: value => Config.IsQuickStackTooltipDrawNearbyChests = value
				);

				api.RegisterLabel(
					mod: ModManifest,
					labelName: "Favorite Items",
					labelDesc: null
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Enable favorite items?",   // TODO: Will favorited items ignore Add To Existing Stacks? Or just quick stack?
					optionDesc: "If enabled, items in your inventory can be favorited. Favorited items will be ignored when stacking into chests.",
					optionGet: () => Config.IsEnableFavoriteItems,
					optionSet: value => Config.IsEnableFavoriteItems = value
				);
				string[] highlightStyleDescriptions =
				{
					": Gold dashed",
					": Clean gold dashed",
					": Thick gold border",
					": Textured gold inset border",
					": Gold inset border",
					": Dark dashed"
				};
				api.RegisterChoiceOption(
					mod: ModManifest,
					optionName: "Highlight style",
					optionDesc: "Choose your preferred texture style for highlighting favorited items in your inventory.",
					optionGet: () => Config.FavoriteItemsHighlightTextureChoice.ToString() + highlightStyleDescriptions[Config.FavoriteItemsHighlightTextureChoice],
					optionSet: value =>
					{
						Config.FavoriteItemsHighlightTextureChoice = int.Parse(value.Substring(0, 1));
						ConvenientInventory.FavoriteItemsHighlightTexture = Helper.Content.Load<Texture2D>($@"Assets\favoriteHighlight_{value[0]}.png");
					},
					choices: new string[]
					{
						"0" + highlightStyleDescriptions[0],
						"1" + highlightStyleDescriptions[1],
						"2" + highlightStyleDescriptions[2],
						"3" + highlightStyleDescriptions[3],
						"4" + highlightStyleDescriptions[4],
						"5" + highlightStyleDescriptions[5]
					}
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Keybind (keyboard)",
					optionDesc: "Hold this key when selecting an item to favorite it.",
					optionGet: () => Config.FavoriteItemsKeyboardHotkey,
					optionSet: value => Config.FavoriteItemsKeyboardHotkey = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Keybind (controller)",
					optionDesc: "Hold this button when selecting an item to favorite it.",
					optionGet: () => Config.FavoriteItemsControllerHotkey,
					optionSet: value => Config.FavoriteItemsControllerHotkey = value
				);
			}
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			if (Config.IsEnableFavoriteItems)
			{
				ConvenientInventory.LoadFavoriteItemSlots();
			}
		}

		private void OnSaving(object sender, SavingEventArgs e)
        {
			if (Config.IsEnableFavoriteItems)
			{
				ConvenientInventory.SaveFavoriteItemSlots();
			}
        }

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// Handle favorite items hotkey being pressed
			if (Config.IsEnableFavoriteItems && e.Button == Config.FavoriteItemsKeyboardHotkey || e.Button == Config.FavoriteItemsControllerHotkey)
            {
				ConvenientInventory.IsFavoriteItemsHotkeyDown = true;
            }
		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			// Handle favorite items hotkey being released
			if (Config.IsEnableFavoriteItems && e.Button == Config.FavoriteItemsKeyboardHotkey || e.Button == Config.FavoriteItemsControllerHotkey)
			{
				ConvenientInventory.IsFavoriteItemsHotkeyDown = false;
			}
		}
	}
}
