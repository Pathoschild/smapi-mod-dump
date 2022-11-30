/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using BlueberryMushroomMachine.Core;
using BlueberryMushroomMachine.Editors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

using System;
using System.Linq;
using Object = StardewValley.Object;

namespace BlueberryMushroomMachine
{
	public sealed class ModEntry : Mod
	{
		public class SaveData
		{
			public bool PyTKMigration { get; set; } = true;
		}

		public enum Mushrooms
		{
			Morel = 257,
			Chantarelle = 281,
			Common = 404,
			Red = 420,
			Purple = 422
		}

		internal static ModEntry Instance;
		internal SaveData Data;
		internal Config Config;
		internal ITranslationHelper i18n => Helper.Translation;

		public static Texture2D OverlayTexture;

        private static IJsonAssetsAPI? jsonAssetsAPI;

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();

			if (Config.DebugMode)
				helper.Events.Input.ButtonPressed += OnButtonPressed;

			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.GameLoop.DayStarted += OnDayStarted;

			// Load mushroom overlay texture for all filled machines
			OverlayTexture = Helper.ModContent.Load<Texture2D>(ModValues.OverlayPath);

			// Harmony setup
			HarmonyPatches.Apply(this.ModManifest.UniqueID);

            // Load textures
            BigCraftablesTilesheetEditor.Initialize(helper.ModContent);
		}

		private void LoadApis()
		{
			// SpaceCore setup
			var spacecoreApi = Helper.ModRegistry.GetApi<Core.ISpaceCoreAPI>("spacechase0.SpaceCore");
			spacecoreApi.RegisterSerializerType(typeof(Propagator));

            jsonAssetsAPI =  Helper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");
            if (jsonAssetsAPI is null)
                this.Monitor.Log($"Json Assets not found, deshuffling will not happen");
            else
                jsonAssetsAPI.IdsFixed += this.FixIds;
		}

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                if (Config.DebugMode)
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
            }
            catch (Exception e1)
            {
                Log.E($"Error in printing mod configuration.\n{e1}");
            }

            LoadApis();

            this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void FixIds(object? sender, EventArgs e)
        {
            try
            {
                Utility.ForAllLocations((location) =>
                {
                    foreach (var obj in location.Objects.Values)
                    {
                        if (obj is Propagator propagator && propagator.Name == ModValues.PropagatorInternalName)
                        {
                            var newId = jsonAssetsAPI.GetObjectId(propagator.SourceMushroomName);
                            if (newId != -1 && newId != propagator.SourceMushroomIndex)
                            {
                                this.Monitor.Log($"Updating mushroom ID for mushroom propagator located at {location.NameOrUniqueName}::{propagator.TileLocation}: {propagator.SourceMushroomName} {propagator.SourceMushroomIndex} => {newId}");
                                propagator.SourceMushroomIndex = newId;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Log.E($"Error while deshuffling held mushrooms\n\n{ex}");
            }
        }

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (Data == null)
			{
				Log.D("Loading data.", Config.DebugMode);
				Data = Helper.Data.ReadSaveData<SaveData>("SaveData") ?? new SaveData();
			}

			// Add Robin's pre-Demetrius-event dialogue
			if (Game1.player.daysUntilHouseUpgrade.Value == 2 && Game1.player.HouseUpgradeLevel == 2)
				Game1.player.activeDialogueEvents.Add("event.4637.0000.0000", 7);

			// Add the Propagator crafting recipe if the cheat is enabled
			if (Config.RecipeAlwaysAvailable)
				if (!Game1.player.craftingRecipes.ContainsKey(ModValues.PropagatorInternalName))
					Game1.player.craftingRecipes.Add(ModValues.PropagatorInternalName, 0);

			// Correct invalid objects matching ours
			RebuildPropagtors();

            // Manually DayUpdate each Propagator
            Utility.ForAllLocations((location) =>
            {
                foreach (var obj in location.Objects.Values)
                {
                    if (obj is Propagator propagator && propagator.Name == ModValues.PropagatorInternalName)
                        propagator.DayUpdate();
                }
            });

            Log.D("Saving data.", Config.DebugMode);
            Helper.Data.WriteSaveData("SaveData", Data);
        }

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence
			    || Game1.currentBillboard != 0 || Game1.activeClickableMenu != null || Game1.menuUp
			    || Game1.nameSelectUp
			    || Game1.IsChatting || Game1.dialogueTyping || Game1.dialogueUp || Game1.fadeToBlack
			    || !Game1.player.CanMove || Game1.eventUp || Game1.isFestival())
				return;

			// Debug spawning for Propagator: Can't be spawned in with CJB Item Spawner as it subclasses Object
			if (e.Button == Config.DebugGivePropagatorKey)
			{
				var prop = new Propagator(Game1.player.getTileLocation());
				Game1.player.addItemByMenuIfNecessary(prop);
				Log.D($"{Game1.player.Name} spawned in a"
				      + $" [{ModValues.PropagatorIndex}] {ModValues.PropagatorInternalName} ({prop.DisplayName}).");
			}
		}

		/// <summary>
		/// Rebuilds any broken or missing Propagator objects in the player's inventory and throughout game location object lists.
		/// </summary>
		private void RebuildPropagtors()
		{
			if (Data != null && Data.PyTKMigration)
			{
				// Manually rebuild each Propagator in the player's inventory
				var rebuiltItemsCount = 0;
				var items = Game1.player.Items;
				for (var i = items.Count - 1; i > 0; --i)
				{
					if (items[i] == null
						|| !items[i].Name.StartsWith($"PyTK|Item|{ModValues.PackageName}")
						|| !items[i].Name.Contains($"{ModValues.PropagatorInternalName}"))
						continue;

					++rebuiltItemsCount;
					Log.D($"Found a broken Propagator in {Game1.player.Name}'s inventory slot {i}, rebuilding manually.",
						Config.DebugMode);

					var stack = items[i].Stack;
					Game1.player.removeItemFromInventory(items[i]);
					Game1.player.addItemToInventory(new Propagator { Stack = stack }, i);
				}

				// Manually rebuild each Propagator in the world
				var rebuiltObjectsCount = 0;
				foreach (var location in Game1.locations)
				{
					foreach (var key in location.Objects.Keys.ToList())
					{
						if (!location.Objects[key].Name.StartsWith($"PyTK|Item|{ModValues.PackageName}"))
							continue;

						int index = 0, quantity = 0, quality = 0;
						var days = 0f;
						var replacement = new Propagator();
						var tileLocation = Vector2.Zero;
						var isHoldingMushroom = false;
						var itemSplit = location.Objects[key].Name.Substring(location.Objects[key].Name.IndexOf(", ")).Split('|');
						foreach (var field in itemSplit)
						{
							var fieldSplit = field.Split(new[] { '=' }, 2);
							switch (fieldSplit[0])
							{
								case "tileLocationX":
									tileLocation.X = float.Parse(fieldSplit[1]);
									break;
								case "tileLocationY":
									tileLocation.Y = float.Parse(fieldSplit[1]);
									break;
								case "heldObjectIndex":
									index = int.Parse(fieldSplit[1]);
									break;
								case "heldObjectQuality":
									quality = int.Parse(fieldSplit[1]);
									break;
								case "heldObjectQuantity":
									quantity = int.Parse(fieldSplit[1]);
									break;
								case "days":
									days = float.Parse(fieldSplit[1]);
									break;
								case "produceExtra":
									isHoldingMushroom = true;
									break;
							}
						}
						replacement.TileLocation = tileLocation;
						replacement.PutSourceMushroom(new Object(index, quantity) { Quality = quality });
						if (isHoldingMushroom)
						{
							replacement.PutExtraHeldMushroom(daysToMature: days);
						}

						++rebuiltObjectsCount;
						Log.D($"Found a broken Propagator in {location.Name}'s objects at {key}, rebuilding manually.",
							Config.DebugMode);

						location.Objects[key] = replacement;
					}
				}

				Log.D($"Rebuilt {rebuiltItemsCount} inventory items and {rebuiltObjectsCount} world objects.");
				Data.PyTKMigration = false;
			}
		}


        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            _ = BigCraftablesInfoEditor.ApplyEdit(e)
                || BigCraftablesTilesheetEditor.ApplyEdit(e)
                || CraftingRecipesEditor.ApplyEdit(e)
                || EventsEditor.ApplyEdit(e); 
        }

		/// <summary>
		/// Determines the frame to be used for showing held mushroom growth.
		/// </summary>
		/// <param name="currentDays">Current days since last growth.</param>
		/// <param name="goalDays">Number of days when next growth happens.</param>
		/// <param name="quantity">Current count of mushrooms.</param>
		/// <param name="max">Maximum amount of mushrooms of this type.</param>
		/// <returns>Frame for mushroom growth progress.</returns>
		public static int GetOverlayGrowthFrame(float currentDays, int goalDays, int quantity, int max)
		{
			var maths =
				(((quantity - 1) + ((float)currentDays / goalDays)) * goalDays)
				/ ((max - 1) * goalDays)
				* ModValues.OverlayMushroomFrames;
            return Math.Clamp((int)maths, 0, ModValues.OverlayMushroomFrames);
		}

		/// <summary>
		/// Generates a clipping rectangle for the overlay appropriate
		/// to the current held mushroom, and its held quantity.
		/// Undefined mushrooms will use their default object rectangle.
		/// </summary>
		/// <returns></returns>
		public static Rectangle GetOverlaySourceRect(int index, int whichFrame)
		{
			return Enum.IsDefined(typeof(Mushrooms), index)
				? new Rectangle(whichFrame * 16,  GetMushroomSourceRectIndex(index) * 32, 16, 32)
				: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, index, 16, 16);
		}

        public static bool IsValidMushroom(Object o)
        {
            // from the vanilla Utility.IsPerfectlyNormalObjectAtParentSheetIndex or whatever that method was again.
            // Don't want to start growing wallpaper
            Type type = o.GetType();
            if (o is null || (type != typeof(Object) && type != typeof(ColoredObject)))
                return false;

            return Enum.IsDefined(typeof(Mushrooms), o.ParentSheetIndex)
                   || (o.Category == -75 || o.Category == -81)
                   && (o.Name.Contains("mushroom", StringComparison.InvariantCultureIgnoreCase) || o.Name.Contains("fungus", StringComparison.InvariantCultureIgnoreCase))
                   || Instance.Config.OtherObjectsThatCanBeGrown.Contains(o.Name);
        }

		public static int GetMushroomSourceRectIndex(int index)
		{
			return index switch
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
