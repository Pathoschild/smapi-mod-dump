/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Config;
using FarmAnimalVarietyRedux.ContentPatcherTokens;
using FarmAnimalVarietyRedux.EqualityComparers;
using FarmAnimalVarietyRedux.Menus;
using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Models.BfavSaveData;
using FarmAnimalVarietyRedux.Models.Converted;
using FarmAnimalVarietyRedux.Models.Parsed;
using FarmAnimalVarietyRedux.Patches;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FarmAnimalVarietyRedux
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether the 8 heart Shane event has been seen (used to determine if blue chickens should be buyable.)</summary>
        private bool PreviousBlueChickenEventState;

        /// <summary>Contains all recipes that were parsed from the content packs, these are stored and then done all at once to ensure all animals have been loaded first.</summary>
        private List<ParsedIncubatorRecipe> ParsedRecipes = new List<ParsedIncubatorRecipe>();


        /*********
        ** Accessors
        *********/
        /// <summary>Provides basic animal apis.</summary>
        public IApi Api { get; private set; }

        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>The asset manager.</summary>
        public AssetManager AssetManager { get; private set; }

        /// <summary>Whether the content packs have been loaded.</summary>
        public bool ContentPacksLoaded { get; private set; }

        /// <summary>Contains all the animals that the current <see cref="CustomPurchaseAnimalsMenu"/> should contain.</summary>
        internal List<StardewValley.Object> AnimalsInPurchaseMenu { get; set; } = new List<StardewValley.Object>();

        /// <summary>Contains all animals that were parsed from the save.</summary>
        /// <remarks>The reason this is done is because when the animals need to be retreived to convert them to custom animals, the farm buildings aren't accessible from the <see cref="GameLocation"/> yet, so the references are stored from when they're created to be changed.</remarks>
        internal List<FarmAnimal> ParsedAnimals { get; private set; } = new List<FarmAnimal>();

        /// <summary>All the custom animals.</summary>
        internal List<CustomAnimal> CustomAnimals { get; } = new List<CustomAnimal>();

        /// <summary>All the custom incubator recipes.</summary>
        internal List<IncubatorRecipe> CustomIncubatorRecipes { get; } = new List<IncubatorRecipe>();

        /// <summary>The tools were the error messages should be hidden if used on an animal.</summary>
        internal List<string> SkipErrorMessagesForTools { get; } = new List<string>();

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();
            Config = this.Helper.ReadConfig<ModConfig>();
            AssetManager = new AssetManager();

            ApplyHarmonyPatches();

            this.Helper.Content.AssetLoaders.Add(AssetManager);

            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            this.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.Saving += OnSaving;
            this.Helper.Events.GameLoop.Saved += OnSaved;
            this.Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            this.Helper.Events.Display.MenuChanged += OnMenuChanged;

            this.Helper.ConsoleCommands.Add("favr_summary", "Logs the current state of all animals.\n\nUsage: favr_summary", (command, args) => CommandManager.LogSummary());
        }

        /// <summary>Loads all the content packs.</summary>
        public void LoadContentPacks()
        {
            if (ContentPacksLoaded)
                return;
            ContentPacksLoaded = true;

            CustomAnimals.Clear();
            ParsedRecipes.Clear();
            CustomIncubatorRecipes.Clear();

            PreviousBlueChickenEventState = Game1.player.eventsSeen.Contains(3900074);

            LoadDefaultAnimals();
            LoadDefaultRecipes();

            LoadJAEarly(); // loading Json Assets early is only required on connecting clients. Game1.IsClient can't be used as that hasn't been set yet. even though this is ran on the host, it shouldn't have any affect

            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    LoadContentPack(contentPack);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Unhandled exception occured when loading content pack: {contentPack.Manifest.Name}\n{ex}", LogLevel.Error);
                }
            }

            // invalid cursors cache, this is so any content packs that are editing the default animal shop icons get recached (as the source rectangles wouldn't've been set when CP patched the spritesheet)
            this.Helper.Content.InvalidateCache(Path.Combine("LooseSprites", "Cursors"));

            // add all incubator recipes found in content packs
            foreach (var incubatorRecipe in ParsedRecipes)
                Api.AddIncubatorRecipe(incubatorRecipe);
        }

        /// <inheritdoc/>
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals("data/bigcraftablesinformation");

        /// <inheritdoc/>
        public void Edit<T>(IAssetData asset)
        {
            // TODO: i18n
            var data = asset.AsDictionary<int, string>();
            data.Data[101] = "Incubator/0/-300/Crafting -9/Hatches eggs into baby animals./true/false/2/Incubator";
            data.Data[254] = "Ostrich Incubator/0/-300/Crafting -9/Hatches eggs into baby animals. Place in a barn./true/true/0/Ostrich Incubator";
        }

        /// <inheritdoc/>
        public override object GetApi() => Api;


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when the game has launched.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to record the initial blue chicken event state and to create Content Patcher tokens so FAVR assets can be edited from Content Patcher content packs.</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // create CP tokens
            if (!this.Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
                return;

            var cpApi = this.Helper.ModRegistry.GetApi("Pathoschild.ContentPatcher");
            var registerToken = cpApi.GetType().GetMethod("RegisterToken", new[] { typeof(IManifest), typeof(string), typeof(object) });
            registerToken.Invoke(cpApi, new object[] { this.ModManifest, "GetAssetPath", new GetAssetPathToken() });
            registerToken.Invoke(cpApi, new object[] { this.ModManifest, "GetSourceX", new GetSourceXToken() });
            registerToken.Invoke(cpApi, new object[] { this.ModManifest, "GetSourceY", new GetSourceYToken() });
        }

        /// <summary>Invoked each tick, approximately 60 times a second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to set the blue chicken to be buyable once the 8 heart Shane event has been seen.</remarks>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var currentBlueChickenEventState = Game1.player.eventsSeen.Contains(3900074);
            if (!PreviousBlueChickenEventState && currentBlueChickenEventState)
            {
                PreviousBlueChickenEventState = currentBlueChickenEventState;

                var blueChicken = Api.GetAnimalByInternalName("game.Blue Chicken");
                blueChicken.IsBuyable = true;
                Api.AddIncubatorRecipe(new ParsedIncubatorRecipe(IncubatorType.Regular, "176", .25f, 9000, "game.Blue Chicken")); // white egg
                Api.AddIncubatorRecipe(new ParsedIncubatorRecipe(IncubatorType.Regular, "174", .25f, 9000, "game.Blue Chicken")); // large white egg
                Api.AddIncubatorRecipe(new ParsedIncubatorRecipe(IncubatorType.Regular, "180", .25f, 9000, "game.Blue Chicken")); // brown egg
                Api.AddIncubatorRecipe(new ParsedIncubatorRecipe(IncubatorType.Regular, "182", .25f, 9000, "game.Blue Chicken")); // large brown egg
            }
        }

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to load the content packs and to subscribe the OnToolUse event handler to the ToolUse event.</remarks>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ParsedAnimals.Clear();

            LoadContentPacks();

            // convert any BFAV animals to FAVR animals
            if (Game1.CustomData.TryGetValue("smapi/mod-data/paritee.betterfarmanimalvariety/farm-animals", out var bfavSavedAnimalDataString))
            {
                var bfavSavedAnimalData = JsonConvert.DeserializeObject<BfavAnimals>(bfavSavedAnimalDataString);
                for (int i = 0; i < bfavSavedAnimalData.Animals.Count; i++)
                {
                    var bfavAnimalData = bfavSavedAnimalData.Animals[i];

                    var animal = Utility.getAnimal(bfavAnimalData.Id);
                    if (animal == null)
                    {
                        // animal no longer exists so clean up the BFAV data
                        bfavSavedAnimalData.Animals.RemoveAt(i--);
                        continue;
                    }

                    // make sure animal isn't already an FAVR animal
                    if (animal.modData.TryGetValue($"{this.ModManifest.UniqueID}/type", out var type))
                        if (type != "game.White Chicken")
                        {
                            // clean up BFAV data as it's already an animal that's been converted before
                            bfavSavedAnimalData.Animals.RemoveAt(i--);
                            continue;
                        }

                    // get FAVR animal data for BFAV animal type
                    var customAnimal = Api.GetAnimalSubtypeByName(bfavAnimalData.TypeLog.Current);
                    if (customAnimal == null)
                    {
                        this.Monitor.Log($"A BFAV animal with the type of {bfavAnimalData.TypeLog.Current} was found in the save file but no animal with that type was loaded. Animal will be loaded as a white chicken.", LogLevel.Warn);
                        animal.type.Value = "game.White Chicken";
                        animal.reloadData();
                        continue;
                    }

                    animal.type.Value = customAnimal.InternalName;
                    animal.reloadData();

                    bfavSavedAnimalData.Animals.RemoveAt(i--);
                }

                // reserialise BFAV data with the animals that were successfully converted to FAVR aniamls removed
                Game1.CustomData["smapi/mod-data/paritee.betterfarmanimalvariety/farm-animals"] = JsonConvert.SerializeObject(bfavSavedAnimalData);
            }

            var fireToolEvent = (NetEvent0)typeof(Farmer).GetField("fireToolEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.player);
            fireToolEvent.onEvent += OnToolUse;
        }

        /// <summary>Invoked when the player saves the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e) => ConvertCustomAnimalsToDefaultAnimals();

        /// <summary>Invoked after the game was saved.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaved(object sender, SavedEventArgs e) => ConvertDefaultAnimalsToCustomAnimals();

        /// <summary>Invoked when the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to unsubscribe the OnToolUse event to if the player loads into another save there aren't multiple of the same event handler subscribed.</remarks>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ContentPacksLoaded = false;

            var fireToolEvent = (NetEvent0)typeof(Farmer).GetField("fireToolEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.player);
            fireToolEvent.onEvent -= OnToolUse;
        }

        /// <summary>Invoked when the game menu changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to swap over to using the <see cref="CustomAnimalQueryMenu"/> when a <see cref="AnimalQueryMenu"/> is loaded.</remarks>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is AnimalQueryMenu animalQueryMenu)
                Game1.activeClickableMenu = new CustomAnimalQueryMenu((FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(animalQueryMenu));
            else if (e.NewMenu is PurchaseAnimalsMenu)
                Game1.activeClickableMenu = new CustomPurchaseAnimalsMenu(AnimalsInPurchaseMenu);
        }

        /// <summary>Invoked when the player uses a tool.</summary>
        /// <remarks>This is used to implement harvesting custom animal produce that uses tools.</remarks>
        private void OnToolUse()
        {
            // get the animal that was hit with the tool
            var toolLocation = Game1.player.GetToolLocation();
            var toolRectangle = new Rectangle((int)toolLocation.X - 32, (int)toolLocation.Y - 32, 64, 64);
            var hitAnimal = (FarmAnimal)null;
            if (Game1.player.currentLocation is Farm farm)
                hitAnimal = Utility.GetBestHarvestableFarmAnimal(farm.Animals.Values, Game1.player.CurrentTool, toolRectangle);
            else if (Game1.player.currentLocation is AnimalHouse animalHouse)
                hitAnimal = Utility.GetBestHarvestableFarmAnimal(animalHouse.Animals.Values, Game1.player.CurrentTool, toolRectangle);

            // ensure an animal was hit
            if (hitAnimal == null)
                return;

            var subtype = Api.GetAnimalSubtypeByInternalName(hitAnimal.type);
            if (subtype == null)
                return;

            var ignoreErrorMessage = SkipErrorMessagesForTools.Any(toolName => toolName.ToLower() == Game1.player.CurrentTool.BaseName.ToLower());
            
            // ensure animal is an adult
            if (hitAnimal.isBaby())
            {
                if (!ignoreErrorMessage)
                    Game1.showRedMessage($"{hitAnimal.displayName} is too young to produce.");
                return;
            }

            // ensure animal can be harvested
            var pendingToolProduces = Utilities.GetPendingProduceDrops(hitAnimal, HarvestType.Tool, product => product.ToolName.ToLower() == Game1.player.CurrentTool.BaseName.ToLower());
            if (pendingToolProduces.Count == 0)
            {
                if (!ignoreErrorMessage)
                    // TODO: having this check separate here is kinda hacky, perhaps have an out of how many got filtered?
                    if (subtype.Produce.Where(product => product.HarvestType == HarvestType.Tool && product.ToolName.ToLower() == Game1.player.CurrentTool.BaseName.ToLower()).Count() == 0) // check if there is *ever* any produce using the tool for the animal
                        Game1.showRedMessage($"Cannot use the {Game1.player.CurrentTool.BaseName} to harvest produce from {hitAnimal.displayName}.");
                    else
                        Game1.showRedMessage($"{hitAnimal.displayName} has no produce to harvest with the {Game1.player.CurrentTool.BaseName} right now.");
                return;
            }

            // set necessary animal state
            hitAnimal.doEmote(FarmAnimal.heartEmote);
            hitAnimal.friendshipTowardFarmer.Value = Math.Min(1000, (int)hitAnimal.friendshipTowardFarmer + 5);
            hitAnimal.pauseTimer = 1500;
            if (hitAnimal.showDifferentTextureWhenReadyForHarvest)
                hitAnimal.Sprite.LoadTexture(Path.Combine("Animals", "Sheared") + hitAnimal.type);

            Game1.playSound("coin");
            Game1.player.gainExperience(Farmer.farmingSkill, 5);

            // get all modData products
            var parsedProduces = new List<SavedProduceData>();
            if (hitAnimal.modData.TryGetValue($"{this.ModManifest.UniqueID}/produces", out var producesString))
                parsedProduces = JsonConvert.DeserializeObject<List<SavedProduceData>>(producesString);

            // drop products
            foreach (var pendingToolProduce in pendingToolProduces)
            {
                // make tool harvest sound
                if (pendingToolProduce.Key.ToolHarvestSound != null && pendingToolProduce.Key.ToolHarvestSound.ToLower() != "none")
                    Game1.player.currentLocation.localSound(pendingToolProduce.Key.ToolHarvestSound);

                if (!Game1.player.couldInventoryAcceptThisObject(pendingToolProduce.Value.ParentSheetIndex, pendingToolProduce.Value.Stack, pendingToolProduce.Value.Quality))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    continue;
                }

                Game1.player.addItemToInventory(pendingToolProduce.Value);
                Game1.addHUDMessage(new HUDMessage(pendingToolProduce.Value.DisplayName, pendingToolProduce.Value.Stack, true, Color.WhiteSmoke, pendingToolProduce.Value));

                // update parsed products to reset object
                var uniqueProduceName = pendingToolProduce.Value.modData[$"{this.ModManifest.UniqueID}/uniqueProduceName"];
                pendingToolProduce.Value.modData.Remove($"{this.ModManifest.UniqueID}/uniqueProduceName");
                parsedProduces.First(produce => produce.UniqueName.ToLower() == uniqueProduceName.ToLower()).DaysLeft = Utilities.DetermineDaysToProduce(pendingToolProduce.Key);
            }

            // update modData
            hitAnimal.modData[$"{this.ModManifest.UniqueID}/produces"] = JsonConvert.SerializeObject(parsedProduces);
        }

        /// <summary>Applies the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching game code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(AnimalQueryMenu), new[] { typeof(FarmAnimal) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.ConstructorPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.draw), new[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(PurchaseAnimalsMenu), new[] { typeof(List<StardewValley.Object>) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ConstructorPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: typeof(FarmAnimal).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null), // need manual reflection here as Harmony 1.2.0.1 doesn't have a way to differentiate between instance and static constuctors,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(FarmAnimal), new[] { typeof(string), typeof(long), typeof(long) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorTranspiler))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.reloadData)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ReloadDataPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.PetPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), "behaviors"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.BehaviorsPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.makeSound)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.MakeSoundPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.getSellPrice)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.GetSellPricePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.UpdateWhenNotCurrentLocationPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenCurrentLocation)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.UpdateWhenCurrentLocationTranspiler)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.DayUpdatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.Eat)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.EatPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.isMale)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.IsMalePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.CanHavePregnancy)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.CanHavePregnancyPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.CanSwim)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.CanSwimPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.UpdateRandomMovements)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.UpdateRandomMovementsPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isBuildingConstructed)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(BuildableGameLocationPatch), nameof(BuildableGameLocationPatch.IsBuildingConstructedPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isCollidingPosition), new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(BuildableGameLocationPatch), nameof(BuildableGameLocationPatch.IsCollindingPositionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimatedSprite), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimatedSpritePatch), nameof(AnimatedSpritePatch.LoadTexturePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getPurchaseAnimalStock)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UtilityPatch), nameof(UtilityPatch.GetPurchaseAnimalStockPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.beginUsing)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MilkPailPatch), nameof(MilkPailPatch.BeginUsingPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Shears), nameof(Shears.beginUsing)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ShearsPatch), nameof(ShearsPatch.BeginUsingPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isForage)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.IsForagePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isAnimalProduct)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.IsAnimalProductPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.getOne)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.GetOnePostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performObjectDropInAction)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.PerformObjectDropInActionTranspiler))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.PerformObjectDropInActionPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalHouse), "resetSharedState"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalHousePatch), nameof(AnimalHousePatch.ResetSharedStatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalHousePatch), nameof(AnimalHousePatch.AddNewHatchedAnimalPrefix)))
            );
        }

        /// <summary>Converts the default animals to custom animals.</summary>
        /// <remarks>This is so animals that have been saved can be converted back to their custom versions (if they were custom before being saved).</remarks>
        private void ConvertDefaultAnimalsToCustomAnimals()
        {
            var animals = GetFarmAnimals();
            for (int i = 0; i < animals.Count; i++)
            {
                var animal = animals[i];
                Utilities.ConvertDefaultAnimalToCustomAnimal(ref animal);
            }
        }

        /// <summary>Converts the custom animals to default animals.</summary>
        /// <remarks>This is so animals are saved as default animals which means if the player uninstalls the mod the save won't become unusable.</remarks>
        private void ConvertCustomAnimalsToDefaultAnimals()
        {
            var animals = GetFarmAnimals();
            for (int i = 0; i < animals.Count; i++)
            {
                var animal = animals[i];
                Utilities.ConvertCustomAnimalToDefaultAnimal(ref animal);
            }
        }

        /// <summary>Gets all the animals on the farm and in farm buildings.</summary>
        /// <returns>All the animals on the farm and in farm buildings.</returns>
        private List<FarmAnimal> GetFarmAnimals()
        {
            var animals = new List<FarmAnimal>(ParsedAnimals);

            // get all aniamls on the farm (locked outside)
            animals.AddRange(Game1.getFarm().Animals.Values);

            // get all animals in buildings
            foreach (var building in Game1.getFarm().buildings)
                if (building.indoors.Value is AnimalHouse animalHouse)
                    animals.AddRange(animalHouse.Animals.Values);

            return animals.Distinct(new FarmAnimalIdEqualityComparer()).ToList();
        }

        /// <summary>Runs the initialisation code for Json Assets early.</summary>
        /// <remarks>This is required for connecting multiplayer clients because farm animals are loaded before Json Assets loads the client assets.</remarks>
        private void LoadJAEarly()
        {
            // ensure JA is installed
            if (!this.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                return;

            this.Monitor.Log("Initialising JA early");

            // force JA to initialise early
            var jaModData = this.Helper.ModRegistry.Get("spacechase0.JsonAssets");
            var jaInstance = (Mod)jaModData.GetType().GetProperty("Mod", BindingFlags.Public | BindingFlags.Instance).GetValue(jaModData);
            jaInstance.GetType().GetMethod("InitStuff", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(jaInstance, new object[] { false });
        }

        /// <summary>Loads a content pack.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        private void LoadContentPack(IContentPack contentPack)
        {
            this.Monitor.Log($"Loading {contentPack.Manifest.Name}", LogLevel.Info);

            // check if incubator.json exists (and add the recipes if the file is present)
            var incubatorFile = Path.Combine(contentPack.DirectoryPath, "incubator.json");
            if (File.Exists(incubatorFile))
            {
                var incubatorRecipes = contentPack.LoadAsset<List<ParsedIncubatorRecipe>>("incubator.json");
                foreach (var incubatorRecipe in incubatorRecipes)
                    ParsedRecipes.Add(incubatorRecipe);
            }

            // loop through each animal folder in the current content pack
            var animalsByAction = new Dictionary<Action, List<(string UniqueModId, ParsedCustomAnimal AnimalData, SoundEffect CustomSound)>>();
            foreach (var animalFolder in new DirectoryInfo(contentPack.DirectoryPath).EnumerateDirectories())
            {
                var animalName = animalFolder.Name;
                var internalAnimalName = $"{contentPack.Manifest.UniqueID}.{animalName}";

                // ensure content.json file exists
                var contentFile = Path.Combine(animalFolder.FullName, "content.json");
                if (!File.Exists(contentFile))
                {
                    this.Monitor.Log($"Animal: {internalAnimalName} doesn't contain a content.json file, skipping", LogLevel.Error);
                    continue;
                }

                // seralise content.json file
                var animalData = contentPack.LoadAsset<ParsedCustomAnimal>(Path.Combine(animalName, "content.json"));
                if (animalData.Action == Action.Add)
                    if (!animalData.IsValid())
                    {
                        this.Monitor.Log($"Animal: {internalAnimalName} is invalid, skipping", LogLevel.Error);
                        continue;
                    }

                // ensure the name doesn't already exist
                if (Api.GetAnimalByInternalName(internalAnimalName) != null)
                {
                    this.Monitor.Log($"An animal with the name: {internalAnimalName} already exists, skipping", LogLevel.Error);
                    continue;
                }

                // ensure the action is valid (if an int was specified out of range)
                if (!Enum.GetValues(typeof(Action)).Cast<Action>().Contains(animalData.Action))
                {
                    this.Monitor.Log($"Action: {animalData.Action} for animal isn't valid, skipping", LogLevel.Error);
                    continue;
                }

                // load custom sound if it's present
                var soundFile = Path.Combine(animalFolder.FullName, "sound.wav");
                SoundEffect customSound = null;
                if (File.Exists(soundFile))
                    using (var fileStream = File.OpenRead(soundFile))
                        customSound = SoundEffect.FromStream(fileStream);

                // validate subtypes
                for (int i = 0; i < animalData.Subtypes.Count; i++)
                {
                    var subtype = animalData.Subtypes[i];

                    // ensure subtype is valid
                    if (subtype.Action == Action.Add && !subtype.IsValid())
                    {
                        this.Monitor.Log($"Subtype: {subtype.Name} is invalid, skipping", LogLevel.Error);
                        animalData.Subtypes.RemoveAt(i--);
                    }
                }

                // load subtype assets
                for (int i = 0; i < animalData.Subtypes.Count; i++)
                {
                    var subtype = animalData.Subtypes[i];

                    if (subtype.Action == Action.Delete)
                        continue;

                    if (subtype.Action == Action.Add) // only validate if the type is being added
                    {
                        if (!AreAssetsValid(contentPack, animalName, subtype.Name))
                        {
                            this.Monitor.Log($"Sprite sheets for subtype: {subtype.Name}, animal: {internalAnimalName} aren't valid, skipping", LogLevel.Error);
                            animalData.Subtypes.RemoveAt(i--);
                            continue;
                        }
                    }

                    // if the subtype is editing a previous one, get the name from the internal name
                    else if (subtype.Action == Action.Edit)
                        if (subtype.Name == null && subtype.InternalName != null)
                            subtype.Name = subtype.InternalName.Split('.').Last();

                    // determine unique mod id
                    var animalUniqueModId = contentPack.Manifest.UniqueID;
                    var subtypeUniqueModId = contentPack.Manifest.UniqueID;
                    if (subtype.Action == Action.Edit)
                    {
                        var splitInternalName = animalData.InternalName.Split('.');
                        animalUniqueModId = string.Join(".", splitInternalName.Take(splitInternalName.Length - 1));

                        splitInternalName = subtype.InternalName.Split('.');
                        subtypeUniqueModId = string.Join(".", splitInternalName.Take(splitInternalName.Length - 1));
                    }
                    else if (subtype.Action == Action.Add && !string.IsNullOrEmpty(animalData.InternalName))
                    {
                        var splitInternalName = animalData.InternalName.Split('.');
                        animalUniqueModId = string.Join(".", splitInternalName.Take(splitInternalName.Length - 1));
                    }

                    RegisterAssets(animalUniqueModId, subtypeUniqueModId, contentPack, animalName, subtype.Name);
                    RegisterAsset(Path.Combine(animalName, "shopdisplay.png"), $"{animalUniqueModId}.{animalName}", contentPack);
                }

                // ensure there are valid subtypes
                if (animalData.Subtypes.Count == 0)
                {
                    this.Monitor.Log($"No subtypes for the animal: {animalName} were loaded, skipping", LogLevel.Error);
                    continue;
                }

                if (!animalsByAction.ContainsKey(animalData.Action))
                    animalsByAction[animalData.Action] = new List<(string ModUniqueId, ParsedCustomAnimal animalData, SoundEffect customSound)>();
                animalsByAction[animalData.Action].Add((contentPack.Manifest.UniqueID, animalData, customSound));
            }

            // process animals
            if (animalsByAction.TryGetValue(Action.Add, out var animals))
                foreach (var animal in animals)
                    Api.AddAnimal(animal.UniqueModId, animal.AnimalData, animal.CustomSound);
            if (animalsByAction.TryGetValue(Action.Edit, out animals))
                foreach (var animal in animals)
                    Api.EditAnimal(animal.UniqueModId, animal.AnimalData, animal.CustomSound);
            if (animalsByAction.TryGetValue(Action.Delete, out animals))
                foreach (var animal in animals)
                    Api.DeleteAnimal(animal.AnimalData.InternalName);
        }

        /// <summary>Determines whether an animal subtype has valid sprite sheets.</summary>
        /// <param name="contentPack">The content pack that contains the assets to validate.</param>
        /// <param name="animalName">The animal name of the assets to validate.</param>
        /// <param name="animalSubtypeName">The animal subtype of the assets to validate.</param>
        /// <returns><see langword="true"/> if the assets are valid; otherwise, <see langword="false"/>.</returns>
        private bool AreAssetsValid(IContentPack contentPack, string animalName, string animalSubtypeName)
        {
            // non seasonal
            var hasAdultSpriteSheet = File.Exists(Path.Combine(contentPack.DirectoryPath, animalName, "assets", $"{animalSubtypeName}.png"));
            if (hasAdultSpriteSheet)
                return true;

            // seasonal
            var hasSpringAdultSpriteSheet = File.Exists(Path.Combine(contentPack.DirectoryPath, animalName, "assets", "spring", $"{animalSubtypeName}.png"));
            var hasSummerAdultSpriteSheet = File.Exists(Path.Combine(contentPack.DirectoryPath, animalName, "assets", "summer", $"{animalSubtypeName}.png"));
            var hasFallAdultSpriteSheet = File.Exists(Path.Combine(contentPack.DirectoryPath, animalName, "assets", "fall", $"{animalSubtypeName}.png"));
            var hasWinterAdultSpriteSheet = File.Exists(Path.Combine(contentPack.DirectoryPath, animalName, "assets", "winter", $"{animalSubtypeName}.png"));
            if (hasSpringAdultSpriteSheet && hasSummerAdultSpriteSheet && hasFallAdultSpriteSheet && hasWinterAdultSpriteSheet)
                return true;

            return false;
        }

        /// <summary>Registers the assets for a specified animal and subtype.</summary>
        /// <param name="uniqueModId">The unique mod id the animal whose assets are being registered belong to.</param>
        /// <param name="contentPack">The content pack that contains the assets to register.</param>
        /// <param name="animalName">The animal name of the assets to register.</param>
        /// <param name="animalSubtypeName">The animal subtype of the assets to register.</param>
        private void RegisterAssets(string animalUniqueModId, string subtypeUniqueModId, IContentPack contentPack, string animalName, string animalSubtypeName)
        {
            var internalAnimalName = $"{animalUniqueModId}.{animalName}";
            var internalSubtypeName = $"{subtypeUniqueModId}.{animalSubtypeName}";

            { // non seasonal
                var adultPath = Path.Combine(animalName, "assets", $"{animalSubtypeName}.png");
                var harvestablePath = Path.Combine(animalName, "assets", $"Harvestable {animalSubtypeName}.png");
                RegisterAsset(adultPath, internalAnimalName, internalSubtypeName, false, false, null, contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", $"Harvested {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, false, true, null, contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", $"Baby {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, true, false, null, contentPack);
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, harvestablePath)))
                {
                    AssetManager.UpdateAsset(internalAnimalName, internalSubtypeName, false, false, null, false, true, null); // update the adult sprite sheet to be harvested
                    RegisterAsset(harvestablePath, internalAnimalName, internalSubtypeName, false, false, null, contentPack); // set the harvestable to the adult sheet
                }
            }

            { // spring
                var adultPath = Path.Combine(animalName, "assets", "spring", $"{animalSubtypeName}.png");
                var harvestablePath = Path.Combine(animalName, "assets", "spring", $"Harvestable {animalSubtypeName}.png");
                RegisterAsset(adultPath, internalAnimalName, internalSubtypeName, false, false, "spring", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "spring", $"Harvested {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, false, true, "spring", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "spring", $"Baby {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, true, false, "spring", contentPack);
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, harvestablePath)))
                {
                    AssetManager.UpdateAsset(internalAnimalName, internalSubtypeName, false, false, "spring", false, true, "spring"); // update the adult sprite sheet to be harvested
                    RegisterAsset(harvestablePath, internalAnimalName, internalSubtypeName, false, false, "spring", contentPack); // set the harvestable to the adult sheet
                }
            }

            { // summer
                var adultPath = Path.Combine(animalName, "assets", "summer", $"{animalSubtypeName}.png");
                var harvestablePath = Path.Combine(animalName, "assets", "summer", $"Harvestable {animalSubtypeName}.png");
                RegisterAsset(adultPath, internalAnimalName, internalSubtypeName, false, false, "summer", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "summer", $"Harvested {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, false, true, "summer", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "summer", $"Baby {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, true, false, "summer", contentPack);
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, harvestablePath)))
                {
                    AssetManager.UpdateAsset(internalAnimalName, internalSubtypeName, false, false, "summer", false, true, "summer"); // update the adult sprite sheet to be harvested
                    RegisterAsset(harvestablePath, internalAnimalName, internalSubtypeName, false, false, "summer", contentPack); // set the harvestable to the adult sheet
                }
            }

            { // fall
                var adultPath = Path.Combine(animalName, "assets", "fall", $"{animalSubtypeName}.png");
                var harvestablePath = Path.Combine(animalName, "assets", "fall", $"Harvestable {animalSubtypeName}.png");
                RegisterAsset(adultPath, internalAnimalName, internalSubtypeName, false, false, "fall", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "fall", $"Harvested {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, false, true, "fall", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "fall", $"Baby {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, true, false, "fall", contentPack);
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, harvestablePath)))
                {
                    AssetManager.UpdateAsset(internalAnimalName, internalSubtypeName, false, false, "fall", false, true, "fall"); // update the adult sprite sheet to be harvested
                    RegisterAsset(harvestablePath, internalAnimalName, internalSubtypeName, false, false, "fall", contentPack); // set the harvestable to the adult sheet
                }
            }

            { // winter
                var adultPath = Path.Combine(animalName, "assets", "winter", $"{animalSubtypeName}.png");
                var harvestablePath = Path.Combine(animalName, "assets", "winter", $"Harvestable {animalSubtypeName}.png");
                RegisterAsset(adultPath, internalAnimalName, internalSubtypeName, false, false, "winter", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "winter", $"Harvested {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, false, true, "winter", contentPack);
                RegisterAsset(Path.Combine(animalName, "assets", "winter", $"Baby {animalSubtypeName}.png"), internalAnimalName, internalSubtypeName, true, false, "winter", contentPack);
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, harvestablePath)))
                {
                    AssetManager.UpdateAsset(internalAnimalName, internalSubtypeName, false, false, "winter", false, true, "winter"); // update the adult sprite sheet to be harvested
                    RegisterAsset(harvestablePath, internalAnimalName, internalSubtypeName, false, false, "winter", contentPack); // set the harvestable to the adult sheet
                }
            }
        }

        /// <summary>Registers an aniaml shop icon.</summary>
        /// <param name="path">The relative path of the shop icon to register.</param>
        /// <param name="internalAnimalName">The internal name of the animal whose shop icon is being registered.</param>
        /// <param name="contentPack">The content pack who owns the shop icon being registered.</param>
        private void RegisterAsset(string path, string internalAnimalName, IContentPack contentPack)
        {
            if (!File.Exists(Path.Combine(contentPack.DirectoryPath, path)))
                return;

            AssetManager.RegisterAsset(internalAnimalName, path, contentPack);
        }

        /// <summary>Registers an animal spritesheet asset.</summary>
        /// <param name="path">The relative path of the asset to register.</param>
        /// <param name="internalAnimalName">The internal name of the animal whose asset is being registered.</param>
        /// <param name="internalSubtypeName">The internal name of the animal subtype whose asset is being registered.</param>
        /// <param name="isBaby">Whether the asset being registered is for the baby version of the animal subtype being registered.</param>
        /// <param name="isHarvested">Whether the asset being registered is for the harvested version of the animal subtype being registered.</param>
        /// <param name="season">The season of the asset being registered.</param>
        /// <param name="contentPack">The content pack who owns the asset being registered.</param>
        private void RegisterAsset(string path, string internalAnimalName, string internalSubtypeName, bool isBaby, bool isHarvested, string season, IContentPack contentPack)
        {
            if (!File.Exists(Path.Combine(contentPack.DirectoryPath, path)))
                return;

            AssetManager.RegisterAsset(internalAnimalName, internalSubtypeName, isBaby, isHarvested, season, path, contentPack);
        }

        /// <summary>Loads all the default animals.</summary>
        /// <remarks>The default animals need to be loaded because the animal data file doesn't get used. This is because FAVR stores a lot more data about an animal than the default way animal data is stored (and as such they need to be converted to 'custom animals' so content packs can edit them).</remarks>
        private void LoadDefaultAnimals()
        {
            this.Monitor.Log("Loading default animals", LogLevel.Info);

            // TODO: ideally make this automatic, however, due to how hardcoded a lot of animal logic is this may not be feasible
            var animalSubtypes = new List<ParsedCustomAnimalType>
            {
                new ParsedCustomAnimalType(Action.Add, "", "White Chicken", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "176", upgradedProductId: "174") }, 3, "cluck", 16, 16, 16, 16, "641", 4, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Brown Chicken", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "180", upgradedProductId: "182") }, 3, "cluck", 16, 16, 16, 16, "641", 7, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Blue Chicken", PreviousBlueChickenEventState, false, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "176", upgradedProductId: "174") }, 3, "cluck", 16, 16, 16, 16, "641", 7, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Void Chicken", false, false, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "305") }, 3, "cluck", 16, 16, 16, 16, "641", 4, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Golden Chicken", false, false, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "928") }, 3, "cluck", 16, 16, 16, 16, "641", 7, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Duck", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "442", upgradedProductId: "444", upgradedProductIsRare: true, daysToProduce: 2) }, 5, "Duck", 16, 16, 16, 16, "642", 3, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Rabbit", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "440", upgradedProductId: "446", upgradedProductIsRare: true, daysToProduce: 4) }, 6, "rabbit", 16, 16, 16, 16, "643", 10),
                new ParsedCustomAnimalType(Action.Add, "", "Dinosaur", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "107", daysToProduce: 7) }, 0, null, 16, 16, 16, 16, "644", 1, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "White Cow", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "184", upgradedProductId: "186", harvestType: HarvestType.Tool, toolName: "milk pail", toolHarvestSound: "Milking") }, 5, "cow", 32, 32, 32, 32, "639", 15, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Brown Cow", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "184", upgradedProductId: "186", harvestType: HarvestType.Tool, toolName: "milk pail", toolHarvestSound: "Milking") }, 5, "cow", 32, 32, 32, 32, "639", 15, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Goat", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "436", upgradedProductId: "438", harvestType: HarvestType.Tool, toolName: "milk pail", toolHarvestSound: "Milking") }, 5, "goat", 32, 32, 32, 32, "644", 10, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Pig", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "430", harvestType: HarvestType.Forage) }, 10, "pig", 32, 32, 32, 32, "640", 20),
                new ParsedCustomAnimalType(Action.Add, "", "Sheep", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "440", daysToProduce: 3, harvestType: HarvestType.Tool, toolName: "shears", toolHarvestSound: "scissors", produceFasterWithShepherd: true) }, 4, "sheep", 32, 32, 32, 32, "644", 15, isMale: false),
                new ParsedCustomAnimalType(Action.Add, "", "Ostrich", true, true, new List<ParsedAnimalProduce> { new ParsedAnimalProduce(Action.Add, "0", "289", daysToProduce: 7) }, 7, "Ostrich", 32, 32, 32, 32, "644", 15, isMale: false)
            };

            var animals = new List<ParsedCustomAnimal>
            {
                new ParsedCustomAnimal(Action.Add, "", "Chicken", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334"), 400), new List<ParsedCustomAnimalType> { animalSubtypes[0], animalSubtypes[1], animalSubtypes[2], animalSubtypes[3], animalSubtypes[4] }, new List<string> { "Coop", "Big Coop", "Deluxe Coop" }),
                new ParsedCustomAnimal(Action.Add, "", "Duck", true, true, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11337"), 600), new List<ParsedCustomAnimalType> { animalSubtypes[5] }, new List<string> { "Big Coop", "Deluxe Coop" }),
                new ParsedCustomAnimal(Action.Add, "", "Rabbit", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11340"), 4000), new List<ParsedCustomAnimalType> { animalSubtypes[6] }, new List<string> { "Deluxe Coop" }),
                new ParsedCustomAnimal(Action.Add, "", "Dinosaur", false, false, new ParsedAnimalShopInfo(), new List<ParsedCustomAnimalType> { animalSubtypes[7] }, new List<string> { "Big Coop", "Deluxe Coop" }),
                new ParsedCustomAnimal(Action.Add, "", "Dairy Cow", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343"), 750), new List<ParsedCustomAnimalType> { animalSubtypes[8], animalSubtypes[9] }, new List<string> { "Barn", "Big Barn", "Deluxe Barn" }),
                new ParsedCustomAnimal(Action.Add, "", "Goat", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11349"), 2000), new List<ParsedCustomAnimalType> { animalSubtypes[10] }, new List<string> { "Big Barn", "Deluxe Barn" }),
                new ParsedCustomAnimal(Action.Add, "", "Pig", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11346"), 8000), new List<ParsedCustomAnimalType> { animalSubtypes[11] }, new List<string> { "Deluxe Barn" }),
                new ParsedCustomAnimal(Action.Add, "", "Sheep", true, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11352"), 4000), new List<ParsedCustomAnimalType> { animalSubtypes[12] }, new List<string> { "Deluxe Barn" }),
                new ParsedCustomAnimal(Action.Add, "", "Ostrich", false, false, new ParsedAnimalShopInfo(Game1.content.LoadString("Strings\\StringsFromCSFiles:Ostrich_Description"), 8000), new List<ParsedCustomAnimalType> { animalSubtypes[13] }, new List<string> { "Deluxe Barn" })
            };

            var animalShopIconRectangles = new Dictionary<string, Rectangle> {
                { "Chicken", new Rectangle(0, 448, 32, 16) },
                { "Duck", new Rectangle(0, 464, 32, 16) },
                { "Rabbit", new Rectangle(64, 464, 32, 16) },
                { "Dinosaur", Rectangle.Empty },
                { "Dairy Cow", new Rectangle(32, 448, 32, 16) },
                { "Pig", new Rectangle(0, 480, 32, 16) },
                { "Goat", new Rectangle(64, 448, 32, 16) },
                { "Sheep", new Rectangle(32, 464, 32, 16) },
                { "Ostrich", new Rectangle(32, 480, 32, 16) }
            };

            // create the animal objects and sprites
            foreach (var animal in animals)
            {
                var internalAnimalName = $"game.{animal.Name}";

                foreach (var subtype in animal.Subtypes)
                {
                    var internalSubtypeName = $"game.{subtype.Name}";

                    // adult sprite sheet
                    AssetManager.RegisterAsset(internalAnimalName, internalSubtypeName, false, false, null, Path.Combine("Animals", subtype.Name));

                    // baby sprite sheet
                    if (subtype.Name.ToLower() != "dinosaur") // dinosaur is never a child, so it doesn't have a sprite sheet
                        if (subtype.Name.ToLower() == "duck")
                            AssetManager.RegisterAsset(internalAnimalName, internalSubtypeName, true, false, null, Path.Combine("Animals", $"BabyWhite Chicken")); // baby duck uses baby white chicken texture
                        else
                            AssetManager.RegisterAsset(internalAnimalName, internalSubtypeName, true, false, null, Path.Combine("Animals", $"Baby{subtype.Name}"));

                    // harvested sprite sheet
                    if (subtype.Name.ToLower() == "sheep")
                        AssetManager.RegisterAsset(internalAnimalName, internalSubtypeName, false, true, null, Path.Combine("Animals", $"Sheared{subtype.Name}"));
                }

                // shop icon
                AssetManager.RegisterAsset(internalAnimalName, Path.Combine("LooseSprites", "Cursors"), animalShopIconRectangles[animal.Name]);
                Api.AddAnimal("game", animal, null);
            }
        }

        /// <summary>Loads all the default incubator recipes.</summary>
        private void LoadDefaultRecipes()
        {
            if (PreviousBlueChickenEventState) // blue chicken recipes
            {
                ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "176", .25f, 9000, "game.Blue Chicken")); // white egg
                ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "174", .25f, 9000, "game.Blue Chicken")); // large white egg
                ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "180", .25f, 9000, "game.Blue Chicken")); // brown egg
                ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "182", .25f, 9000, "game.Blue Chicken")); // large brown egg
            }

            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "176", 1, 9000, "game.White Chicken")); // white egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "174", 1, 9000, "game.White Chicken")); // large white egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "180", 1, 9000, "game.Brown Chicken")); // brown egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "182", 1, 9000, "game.Brown Chicken")); // large brown egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "305", 1, 9000, "game.Void Chicken")); // void egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "928", 1, 9000, "game.Golden Chicken")); // golden egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "442", 1, 9000, "game.Duck")); // duck egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Regular, "107", 1, 18000, "game.Dinosaur")); // dinosaur egg
            ParsedRecipes.Add(new ParsedIncubatorRecipe(IncubatorType.Ostrich, "289", 1, 15000, "game.Ostrich")); // ostrich egg
        }
    }
}
