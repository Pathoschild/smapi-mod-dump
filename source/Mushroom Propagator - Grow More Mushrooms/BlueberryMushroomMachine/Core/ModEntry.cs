using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Object = StardewValley.Object;

namespace BlueberryMushroomMachine
{
	public class ModEntry : Mod
	{
		public enum Mushrooms
		{
			Morel = 257,
			Chantarelle = 281,
			Common = 404,
			Red = 420,
			Purple = 422
		}

		internal static ModEntry Instance;

		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;

		public static Texture2D OverlayTexture;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			if (Config.DebugMode)
				helper.Events.Input.ButtonPressed += OnButtonPressed;

			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.GameLoop.DayStarted += OnDayStarted;

			// Load mushroom overlay texture for all filled machines
			OverlayTexture = Helper.Content.Load<Texture2D>(ModValues.OverlayPath);

			// Harmony setup
			HarmonyPatches.Apply();
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			try
			{
				Log.D("== CONFIG SUMMARY ==\n"
				      + "\nWorks in locations:"
				      + $"\n    {Config.WorksInCellar} {Config.WorksInFarmCave} {Config.WorksInBuildings}" 
				      + $"\n    {Config.WorksInFarmHouse} {Config.WorksInGreenhouse} {Config.WorksOutdoors}\n" 
				      + $"\nMushroom Cave:  {Config.DisabledForFruitCave}" 
				      + $"\nRecipe Cheat:   {Config.RecipeAlwaysAvailable}" 
				      + $"\nQuantity Cheat: {Config.MaximumQuantityLimitsDoubled}" 
				      + $"\nDays To Mature: {Config.MaximumDaysToMature}" 
				      + $"\nGrowth Pulse:   {Config.PulseWhenGrowing}" 
				      + $"\nOnly Tools Pop: {Config.OnlyToolsCanRemoveRootMushrooms}" 
				      + $"\nCustom Objects: {Config.OtherObjectsThatCanBeGrown.Aggregate("", (s, s1) => $"{s}\n    {s1}")}\n" 
				      + $"\nLanguage:       {LocalizedContentManager.CurrentLanguageCode.ToString().ToUpper()}" 
				      + $"\nDebugging:      {Config.DebugMode}",
					Config.DebugMode);
			}
			catch (Exception e1)
			{
				Log.E($"Error in printing mod configuration.\n{e1}");
			}
			finally
			{
				// Identify the tilesheet index for the machine, and then continue
				// to inject relevant data into multiple other assets if successful
				AddObjectData();
			}
		}
		
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// Add Robin's pre-Demetrius-event dialogue
			if (Game1.player.daysUntilHouseUpgrade.Value == 2 && Game1.player.HouseUpgradeLevel == 2)
				Game1.player.activeDialogueEvents.Add("event.4637.0000.0000", 7);

			// Add the Propagator crafting recipe if the cheat is enabled
			if (Config.RecipeAlwaysAvailable)
				if (!Game1.player.craftingRecipes.ContainsKey(ModValues.PropagatorInternalName))
					Game1.player.craftingRecipes.Add(ModValues.PropagatorInternalName, 0);

			// TEMPORARY FIX: Manually rebuild each Propagator in the user's inventory.
			// PyTK ~1.12.13.unofficial seemingly rebuilds inventory objects at ReturnedToTitle,
			// so inventory objects are only rebuilt after the save is reloaded for every session
			if (Config.CheckForPyTKMigration)
			{
				var items = Game1.player.Items;
				for (var i = items.Count - 1; i > 0; --i)
				{
					if (items[i] == null
					    || !items[i].Name.StartsWith($"PyTK|Item|{ModValues.PackageName}") 
					    || !items[i].Name.Contains($"{ModValues.PropagatorInternalName}"))
						continue;
				
					Log.T($"Found a broken {items[i].Name} in {Game1.player.Name}'s inventory slot {i}"
					      + ", rebuilding manually.",
						Config.DebugMode);
						
					var stack = items[i].Stack;
					Game1.player.removeItemFromInventory(items[i]);
					Game1.player.addItemToInventory(new Propagator { Stack = stack }, i);
				}
			}

			// TEMPORARY FIX: Manually DayUpdate each Propagator.
			// PyTK 1.9.11+ rebuilds objects at DayEnding, so Cask.DayUpdate is never called.
			// Also fixes 0-index objects from PyTK rebuilding before the new index is generated.
			foreach (var location in Game1.locations)
			{
				if (!location.Objects.Values.Any())
					continue;
				var objects = location.Objects.Values.Where(o => o.Name.Equals(ModValues.PropagatorInternalName));
				foreach (var obj in objects)
					((Propagator)obj).TemporaryDayUpdate();
			}
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			e.Button.TryGetKeyboard(out var keyPressed);
			if (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence
			    || Game1.currentBillboard != 0 || Game1.activeClickableMenu != null || Game1.menuUp
			    || Game1.nameSelectUp
			    || Game1.IsChatting || Game1.dialogueTyping || Game1.dialogueUp || Game1.fadeToBlack
			    || !Game1.player.CanMove || Game1.eventUp || Game1.isFestival())
				return;

			// Debug spawning for Propagator: Can't be spawned in with CJB Item Spawner as it subclasses Object
			if (keyPressed.ToSButton().Equals(Config.GivePropagatorKey))
			{
				var prop = new Propagator(Game1.player.getTileLocation());
				Game1.player.addItemByMenuIfNecessary(prop);
				Log.D($"{Game1.player.Name} spawned in a"
				      + $" [{ModValues.PropagatorIndex}] {ModValues.PropagatorInternalName} ({prop.DisplayName}).");
			}
		}

		private void AddObjectData()
		{
			Log.D("Injecting object data.",
				Config.DebugMode);

			// Identify the index in bigCraftables for the machine
			Helper.Content.AssetEditors.Add(new Editors.BigCraftablesInfoEditor());

			// Edit all assets that rely on the new bigCraftables index:

			// These can potentially input a bad index first, though BigCraftablesInfoEditor.Edit() will
			// invalidate the cache once it finishes reassigning data with an appropriate index

			// Inject recipe into the Craftables data sheet
			Helper.Content.AssetEditors.Add(new Editors.CraftingRecipesEditor());
			// Inject sprite into the Craftables tilesheet
			Helper.Content.AssetEditors.Add(new Editors.BigCraftablesTilesheetEditor());
			// Inject Demetrius' event
			Helper.Content.AssetEditors.Add(new Editors.EventsEditor());
		}
		
		/// <summary>
		/// Determines the frame to be used for showing held mushroom growth.
		/// </summary>
		/// <param name="days">Current days since last growth.</param>
		/// <param name="quantity">Current count of mushrooms.</param>
		/// <param name="max">Maximum amount of mushrooms of this type.</param>
		/// <returns>Frame for mushroom growth progress.</returns>
		public static int GetOverlayGrowthFrame(int days, int daysToMature, int quantity, int max)
		{
			var maths =
				(((quantity - 1) + ((float)days / daysToMature)) * daysToMature)
				/ ((max - 1) * daysToMature)
				* ModValues.OverlayMushroomFrames;
			maths = Math.Max(0, Math.Min(ModValues.OverlayMushroomFrames, maths));
			return (int)Math.Floor(maths);
		}

		/// <summary>
		/// Generates a clipping rectangle for the overlay appropriate
		/// to the current held mushroom, and its held quantity.
		/// Undefined mushrooms will use their default object rectangle.
		/// </summary>
		/// <returns></returns>
		public static Rectangle GetOverlaySourceRect(Object o, int whichFrame)
		{
			return Enum.IsDefined(typeof(Mushrooms), o.ParentSheetIndex)
				? new Rectangle(whichFrame * 16,  GetMushroomSourceRectIndex(o) * 32, 16, 32)
				: Game1.getSourceRectForStandardTileSheet(
					Game1.objectSpriteSheet, o.ParentSheetIndex, 16, 16);
		}
		
		public static bool IsValidMushroom(Object o)
		{
			return Enum.IsDefined(typeof(Mushrooms), o.ParentSheetIndex)
			       || (o.Category == -75 || o.Category == -81)
			       && (o.Name.ToLower().Contains("mushroom") || o.Name.ToLower().Contains("fungus"))
			       || Instance.Config.OtherObjectsThatCanBeGrown.Contains(o.Name);
		}

		public static int GetMushroomSourceRectIndex(Object o)
		{
			return o.ParentSheetIndex switch
			{
				(int) Mushrooms.Morel => 0,
				(int) Mushrooms.Chantarelle => 1,
				(int) Mushrooms.Common => 2,
				(int) Mushrooms.Red => 3,
				(int) Mushrooms.Purple => 4,
				_ => -1
			};
		}

		public static void GetMushroomGrowthRate(Object o, out float rate)
		{
			rate = o.ParentSheetIndex switch
			{
				(int) Mushrooms.Morel => 0.5f,
				(int) Mushrooms.Chantarelle => 0.5f,
				(int) Mushrooms.Common => 1.0f,
				(int) Mushrooms.Red => 0.5f,
				(int) Mushrooms.Purple => 0.25f,
				_ => o.Price < 50 ? 1.0f : o.Price < 100 ? 0.75f : o.Price < 200 ? 0.5f : 0.25f
			};
		}

		public static void GetMushroomMaximumQuantity(Object o, out int quantity)
		{
			quantity = o.ParentSheetIndex switch
			{
				(int) Mushrooms.Morel => 4,
				(int) Mushrooms.Chantarelle => 4,
				(int) Mushrooms.Common => 6,
				(int) Mushrooms.Red => 3,
				(int) Mushrooms.Purple => 2,
				_ => o.Price < 50 ? 5 : o.Price < 100 ? 4 : o.Price < 200 ? 3 : 2
			};
			quantity *= Instance.Config.MaximumQuantityLimitsDoubled ? 2 : 1;
		}
	}
}
