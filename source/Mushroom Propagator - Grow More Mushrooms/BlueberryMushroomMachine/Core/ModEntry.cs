using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Harmony; // el diavolo

namespace BlueberryMushroomMachine
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance;

		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;

		public static Texture2D OverlayTexture;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			// Debug events.
			if (Config.DebugMode)
				helper.Events.Input.ButtonPressed += OnButtonPressed;

			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.GameLoop.DayStarted += OnDayStarted;

			// Load mushroom overlay texture for all filled machines.
			OverlayTexture = Helper.Content.Load<Texture2D>(Const.OverlayPath);

			// Harmony setup.
			var harmony = HarmonyInstance.Create($"{Const.AuthorName}.{Const.PackageName}");

			Log.D("Harmony patching methods:"
			      + "\nCraftingPage.performHoverAction:Postfix:void"
			      + "\nCraftingPage.clickCraftingRecipe:Prefix:bool"
				  + "\nCraftingRecipe.createItem:Postfix:void",
				Config.DebugMode);

			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "performHoverAction"),
				postfix: new HarmonyMethod(typeof(RecipeHoverActionPatch), nameof(RecipeHoverActionPatch.Postfix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
				prefix: new HarmonyMethod(typeof(ClickCraftingRecipePatch), nameof(ClickCraftingRecipePatch.Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CraftingRecipe), "createItem"),
				postfix: new HarmonyMethod(typeof(CreateRecipeItemPatch), nameof(CreateRecipeItemPatch.Postfix)));
		}

		#region Game Events

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			Log.I("See log file TRACE for Mushroom Machine state info." 
			      + " Post a complete log via HTTPS://LOG.SMAPI.IO/ on the Nexus page with any issues."
			      + " Thanks!");

			Log.D("Works in locations:"
			      + $" {Config.WorksInCellar} {Config.WorksInFarmCave} {Config.WorksInBuildings}"
				  + $" {Config.WorksInFarmHouse} {Config.WorksInGreenhouse} {Config.WorksOutdoors}"
				  + $"\nMushroom Cave: {Config.RecipeAlwaysAvailable}"
				  + $"\nRecipe Cheat:  {Config.RecipeAlwaysAvailable}"
				  + $"\nDebug Mode:    {Config.DebugMode}",
				Config.DebugMode);

			// Identify the tilesheet index for the machine, and then continue
			// to inject relevant data into multiple other assets if successful.
			AddObjectData();
		}
		
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// Add Robin's pre-Demetrius-event dialogue.
			if (Game1.player.daysUntilHouseUpgrade.Value == 2 && Game1.player.HouseUpgradeLevel == 2)
				Game1.player.activeDialogueEvents.Add("event.4637.0000.0000", 7);

			// Add the Propagator crafting recipe if the cheat is enabled.
			if (Config.RecipeAlwaysAvailable)
				if (!Game1.player.craftingRecipes.ContainsKey(Const.PropagatorInternalName))
					Game1.player.craftingRecipes.Add(Const.PropagatorInternalName, 0);

			// TEMPORARY FIX: Manually rebuild each Propagator in the user's inventory.
			// PyTK ~1.12.13.unofficial seemingly rebuilds inventory objects at ReturnedToTitle,
			// so inventory objects are only rebuilt after the save is reloaded for every session.
			var items = Game1.player.Items;
			for (var i = items.Count - 1; i > 0; --i)
			{
				if (items[i] == null
				    || !items[i].Name.StartsWith($"PyTK|Item|{Const.PackageName}") 
				    || !items[i].Name.Contains($"{Const.PropagatorInternalName}"))
					continue;
				
				Log.D($"Found a broken {items[i].Name} in {Game1.player.Name}'s inventory slot {i}"
				      + ", rebuilding manually.",
					Config.DebugMode);
						
				var stack = items[i].Stack;
				Game1.player.removeItemFromInventory(items[i]);
				Game1.player.addItemToInventory(new Propagator { Stack = stack }, i);
			}

			// TEMPORARY FIX: Manually DayUpdate each Propagator.
			// PyTK 1.9.11+ rebuilds objects at DayEnding, so Cask.DayUpdate is never called.
			// Also fixes 0-index objects from PyTK rebuilding before the new index is generated.
			foreach (var location in Game1.locations)
			{
				if (!location.Objects.Values.Any())
					continue;
				var objects = location.Objects.Values.Where(o => o.Name.Equals(Const.PropagatorInternalName));
				foreach (var obj in objects)
					((Propagator)obj).TemporaryDayUpdate();
			}
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			e.Button.TryGetKeyboard(out var keyPressed);

			if (Game1.activeClickableMenu != null)
				return;

			// Debug functionalities.
			if (keyPressed.ToSButton().Equals(Config.GivePropagatorKey))
			{
				var prop = new Propagator(Game1.player.getTileLocation());
				Game1.player.addItemByMenuIfNecessary(prop);
				Log.D($"{Game1.player.Name} spawned in a {Const.PropagatorInternalName} ({prop.DisplayName}).",
					Config.DebugMode);
			}
		}

		private void AddObjectData()
		{
			Log.D("Injecting object data.",
				Config.DebugMode);

			// Identify the index in bigCraftables for the machine.
			Helper.Content.AssetEditors.Add(new Editors.BigCraftablesInfoEditor());

			// Edit all assets that rely on the generated object index:

			// These can potentially input a bad index first, though BigCraftablesInfoEditor.Edit()
			// invalidates the cache once it finishes operations to reassign data with an appropriate index.

			// Inject recipe into the Craftables data sheet.
			Helper.Content.AssetEditors.Add(new Editors.CraftingRecipesEditor());
			// Inject sprite into the Craftables tilesheet.
			Helper.Content.AssetEditors.Add(new Editors.BigCraftablesTilesheetEditor());
			// Inject Demetrius' event.
			Helper.Content.AssetEditors.Add(new Editors.EventsEditor());
		}

		#endregion
	}

	#region Harmony Patches

	public class RecipeHoverActionPatch
	{
		internal static void Postfix(CraftingRecipe ___hoverRecipe)
		{
			if (___hoverRecipe == null)
				return;
			if (___hoverRecipe.name.Equals(Const.PropagatorInternalName))
				___hoverRecipe.DisplayName = new Propagator().DisplayName;
		}
	}

	public class ClickCraftingRecipePatch
	{
		internal static bool Prefix(
			List<Dictionary<ClickableTextureComponent, CraftingRecipe>> ___pagesOfCraftingRecipes,
			int ___currentCraftingPage, Item ___heldItem,
			ClickableTextureComponent c, bool playSound = true)
		{
			try
			{
				// Fetch an instance of any clicked-on craftable in the crafting menu.
				var tempItem = ___pagesOfCraftingRecipes[___currentCraftingPage][c]
					.createItem();

				// Fall through the prefix for any craftables other than the Propagator.
				if (!tempItem.Name.Equals(Const.PropagatorInternalName))
					return true;

				// Behaviours as from base method.
				if (___heldItem == null)
				{
					___pagesOfCraftingRecipes[___currentCraftingPage][c]
						.consumeIngredients(null);
					___heldItem = tempItem;
					if (playSound)
						Game1.playSound("coin");
				}
				if (Game1.player.craftingRecipes.ContainsKey(___pagesOfCraftingRecipes[___currentCraftingPage][c].name))
					Game1.player.craftingRecipes[___pagesOfCraftingRecipes[___currentCraftingPage][c].name]
						+= ___pagesOfCraftingRecipes[___currentCraftingPage][c].numberProducedPerCraft;
				if (___heldItem == null || !Game1.player.couldInventoryAcceptThisItem(___heldItem))
					return false;

				// Add the machine to the user's inventory.
				var prop = new Propagator(Game1.player.getTileLocation());
				Game1.player.addItemToInventoryBool(prop);
				___heldItem = null;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"{Const.AuthorName}.{Const.PackageName} failed in"
				      + $" {nameof(ClickCraftingRecipePatch)}.{nameof(Prefix)}"
				      + $"\n{e}");
				return true;
			}
		}
	}

	public class CreateRecipeItemPatch
	{
		internal static void Postfix(CraftingRecipe __instance, Item __result)
		{
			// Intercept machine crafts with a Propagator subclass,
			// rather than a generic nonfunctional craftable.
			if (__instance.name == Const.PropagatorInternalName)
				__result = new Propagator(Game1.player.getTileLocation());
		}
	}

	#endregion
}
