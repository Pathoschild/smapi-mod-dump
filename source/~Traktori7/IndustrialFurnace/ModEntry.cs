/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;


namespace IndustrialFurnace
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        private const int lightSourceIDMultiplier = 14097000;
        private const string controllerDataSaveKey = "controller-save";
        private const string furnaceBuildingType = "Industrial Furnace";
        private const string saveDataRefreshedMessage = "Save data refreshed";
        private const string requestSaveData = "Request save data";

        private readonly string assetPath = Path.Combine("Buildings", furnaceBuildingType);
        private readonly string blueprintsPath = Path.Combine("Data", "Blueprints");

        // Use a default texture, so the SMAPI won't freak out if exiting to menu
        private readonly string defaultFurnaceTexturePath = Path.Combine("assets", "IndustrialFurnaceOff.png");
        private readonly string blueprintDataPath = Path.Combine("assets", "IndustrialFurnaceBlueprint.json");
        private readonly string smeltingRulesDataPath = Path.Combine("assets", "SmeltingRules.json");
        private readonly string smokeAnimationDataPath = Path.Combine("assets", "SmokeAnimation.json");
        private readonly string fireAnimationDataPath = Path.Combine("assets", "FireAnimation.json");

        private int furnacesBuilt = 0;      // Used to identify furnaces, placed in maxOccupants field.
        private ModConfig config;
        private ModSaveData modSaveData;
        private BlueprintData blueprintData;
        private SmokeAnimationData smokeAnimationData;
        private FireAnimationData fireAnimationData;
        private SmeltingRulesContainer newSmeltingRules;
        private ITranslationHelper i18n;

        private int currentlyLookingAtFurnace = -1;

        private Texture2D furnaceOn;
        private Texture2D furnaceOff;

        private bool customSmokeSpriteExists = false;
        private bool customFireSpriteExists = false;

        private readonly string smokeAnimationSpritePath = Path.Combine("assets", "SmokeSprite.png");
        private readonly string fireAnimationSpritePath = Path.Combine("assets", "FireSprite.png");

        private readonly List<IndustrialFurnaceController> furnaces = new List<IndustrialFurnaceController>();
        

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            i18n = helper.Translation;

            // Check if there exists a custom sprite for the smoke
            if (File.Exists(Path.Combine(helper.DirectoryPath, smokeAnimationSpritePath)))
            {
                customSmokeSpriteExists = true;
            }
            else
            {
                Monitor.Log("Custom sprite for the smoke was not found. Using the default.");
            }

            // Check if there exists a custom sprite for the fire
            if (File.Exists(Path.Combine(helper.DirectoryPath, fireAnimationSpritePath)))
            {
                customFireSpriteExists = true;
            }
            else
            {
                Monitor.Log("Custom sprite for the fire was not found. Using the default.");
            }

            this.config = helper.ReadConfig<ModConfig>();

            // TODO: Use the name specified in the blueprint?
            blueprintData = helper.Data.ReadJsonFile<BlueprintData>(blueprintDataPath);
            newSmeltingRules = helper.Data.ReadJsonFile<SmeltingRulesContainer>(smeltingRulesDataPath);
            CheckSmeltingRules();
            smokeAnimationData = helper.Data.ReadJsonFile<SmokeAnimationData>(smokeAnimationDataPath);
            fireAnimationData = helper.Data.ReadJsonFile<FireAnimationData>(fireAnimationDataPath);

            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Player.Warped += OnWarped;
        }


        public override object GetApi()
        {
            return new IndustrialFurnaceAPI(this);
        }


        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(assetPath))
            {
                return true;
            }
            else if (asset.AssetNameEquals(smokeAnimationSpritePath))
            {
                return true;
            }
            else if (asset.AssetNameEquals(fireAnimationSpritePath))
            {
                return true;
            }

            return false;
        }


        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(assetPath))
            {
                return Helper.Content.Load<T>(defaultFurnaceTexturePath, ContentSource.ModFolder);
            }
            else if (asset.AssetNameEquals(smokeAnimationSpritePath))
            {
                return Helper.Content.Load<T>(smokeAnimationSpritePath, ContentSource.ModFolder);
            }
            else if (asset.AssetNameEquals(fireAnimationSpritePath))
            {
                return Helper.Content.Load<T>(fireAnimationSpritePath, ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }


        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being edit.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(blueprintsPath))
            {
                return true;
            }

            return false;
        }


        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(blueprintsPath))
            {
                var editor = asset.AsDictionary<string, string>();
                editor.Data[furnaceBuildingType] = blueprintData.ToBlueprintString(i18n);
                return;
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }


        /// <summary>Sends a message for all connected players the updated save data. TODO: Exclude the sender?</summary>
        public void SendUpdateMessage()
        {
            // Refresh the save data for the multiplayer message and send message to all players, including host (currently no harm in doing so)
            InitializeSaveData();
            Helper.Multiplayer.SendMessage<ModSaveData>(modSaveData, saveDataRefreshedMessage, new[] { this.ModManifest.UniqueID });
        }


        /// <summary>Checks if the building is an industrial furnace based on its buildingType</summary>
        public bool IsBuildingIndustrialFurnace(Building building)
        {
            return building.buildingType.Value.Equals(furnaceBuildingType);
        }


        public IndustrialFurnaceController GetController(int ID)
        {
            int index = GetIndexOfFurnaceControllerWithTag(ID);

            if (index > -1)
            {
                try
                {
                    return furnaces[index];
                }
                catch (Exception)
                {
                    Monitor.Log($"Trying to access invalid furnace controller with ID {ID}", LogLevel.Error);
                }
            }

            return null;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised before the game state is updated</summary>
        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (Game1.getFarm() is null) return;

            // Create a smoke sprite
            if (smokeAnimationData.Enabled && e.IsMultipleOf(smokeAnimationData.SpawnFrequency * 60 / 1000))
            {
                GameLocation location = Game1.player.currentLocation;

                if (location != null && location.IsFarm && location.IsOutdoors)
                {
                    for (int i = 0; i < furnaces.Count; i++)
                    {
                        if (!furnaces[i].CurrentlyOn) continue;

                        int x = furnaces[i].furnace.tileX.Value;
                        int y = furnaces[i].furnace.tileY.Value;

                        TemporaryAnimatedSprite sprite;

                        // See if we should do a custom sprite, or use the default
                        if (customSmokeSpriteExists)
                        {
                            sprite = new TemporaryAnimatedSprite(smokeAnimationSpritePath,
                                new Rectangle(0, 0, smokeAnimationData.SpriteSizeX, smokeAnimationData.SpriteSizeY),
                                new Vector2(x * 64 + smokeAnimationData.SpawnXOffset, y * 64 + smokeAnimationData.SpawnYOffset),
                                false, 1f / 500f, Color.Gray)
                            {
                                alpha = 0.75f,
                                motion = new Vector2(0.0f, -0.5f),
                                acceleration = new Vector2(1f / 500f, 0.0f),
                                interval = 99999f,
                                layerDepth = 1f,
                                scale = smokeAnimationData.SmokeScale,
                                scaleChange = smokeAnimationData.SmokeScaleChange,
                                rotationChange = (float)(Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
                            };
                        }
                        else
                        {
                            sprite = new TemporaryAnimatedSprite(Path.Combine("LooseSprites", "Cursors"),
                                new Rectangle(372, 1956, 10, 10),
                                new Vector2(x * 64 + smokeAnimationData.SpawnXOffset, y * 64 + smokeAnimationData.SpawnYOffset),
                                false, 1f / 500f, Color.Gray)
                            {
                                alpha = 0.75f,
                                motion = new Vector2(0.0f, -0.5f),
                                acceleration = new Vector2(1f / 500f, 0.0f),
                                interval = 99999f,
                                layerDepth = 1f,
                                scale = smokeAnimationData.SmokeScale,
                                scaleChange = smokeAnimationData.SmokeScaleChange,
                                rotationChange = (float)(Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0)
                            };
                        }

                        // Add smoke sprite
                        location.temporarySprites.Add(sprite);
                    }
                }
            }
            // Create a fire sprite
            if (fireAnimationData.Enabled && e.IsMultipleOf(fireAnimationData.SpawnFrequency * 60 / 1000))
            {
                GameLocation location = Game1.player.currentLocation;

                if (location != null && location.IsFarm && location.IsOutdoors)
                {
                    for (int i = 0; i < furnaces.Count; i++)
                    {
                        if (!furnaces[i].CurrentlyOn) continue;

                        int x = furnaces[i].furnace.tileX.Value;
                        int y = furnaces[i].furnace.tileY.Value;

                        TemporaryAnimatedSprite sprite;

                        if (customFireSpriteExists)
                        {
                            // Spark only randomly
                            if (Game1.random.NextDouble() >= fireAnimationData.SpawnChance) continue;

                            double randomX = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnXRandomOffset - fireAnimationData.SpawnXRandomOffset;
                            double randomY = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnYRandomOffset - fireAnimationData.SpawnYRandomOffset;

                            Vector2 pos = new Vector2(x * 64f + fireAnimationData.SpawnXOffset + (float)randomX,
                                y * 64f + fireAnimationData.SpawnYOffset + (float)randomY);

                            sprite = new TemporaryAnimatedSprite(fireAnimationSpritePath,
                                new Rectangle(0, 0, fireAnimationData.SpriteSizeX, fireAnimationData.SpriteSizeY),
                                fireAnimationData.AnimationSpeed, fireAnimationData.AnimationLength, 10, pos, false,
                                false,
                                (float)((y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05 + (x + 1.0) * 64.0 / 10000.0),
                                0.005f, Color.White, 1f, 0f, 0f, 0f)
                            {
                                light = true,
                                lightcolor = Color.Black
                            };

                            // Puff only randomlierly
                            if (Game1.random.NextDouble() < fireAnimationData.SoundEffectChance)
                                Game1.playSound("fireball");
                        }
                        else
                        {
                            // Spark only randomly
                            if (Game1.random.NextDouble() >= 0.2) continue;

                            double randomX = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnXRandomOffset - fireAnimationData.SpawnXRandomOffset;
                            double randomY = 2 * Game1.random.NextDouble() * fireAnimationData.SpawnYRandomOffset - fireAnimationData.SpawnYRandomOffset;

                            Vector2 pos = new Vector2(x * 64f + fireAnimationData.SpawnXOffset + (float)randomX,
                                y * 64f + fireAnimationData.SpawnYOffset + (float)randomY);

                            sprite = new TemporaryAnimatedSprite(30, pos, Color.White, fireAnimationData.AnimationLength,
                                false, fireAnimationData.AnimationSpeed, 10, 64,
                                (float)((y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05 + (x + 1.0) * 64.0 / 10000.0),
                                -1, 0)
                            {
                                alphaFade = 0.005f,
                                light = true,
                                lightcolor = Color.Black
                            };

                            // Puff only randomlierly
                            if (Game1.random.NextDouble() < fireAnimationData.SoundEffectChance) 
                                Game1.playSound("fireball");
                        }

                        location.temporarySprites.Add(sprite);
                    }
                }
            }
        }


        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Integration for Generic Mod Config Menu by spacechase0
            var GMCMApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (GMCMApi != null)
            {
                GMCMApi.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                GMCMApi.RegisterLabel(ModManifest, i18n.Get("gmcm.main-label"), "");
                GMCMApi.RegisterClampedOption(ModManifest, i18n.Get("gmcm.coal-amount-label"), i18n.Get("gmcm.coal-amount-description"), () => config.CoalAmount, (int val) => config.CoalAmount = val, 1, 100);
                GMCMApi.RegisterSimpleOption(ModManifest, i18n.Get("gmcm.instant-smelting-label"), i18n.Get("gmcm.instant-smelting-description"), () => config.InstantSmelting, (bool val) => config.InstantSmelting = val);
            }
        }


        /// <summary>Raised after the game returns to the title screen.</summary>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Reset stuff
            modSaveData = null;
            furnaces.Clear();
            furnaces.Clear();
        }


        /// <summary>Raised after a mod message is received over the network.</summary>
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID)
            {
                if (e.Type == saveDataRefreshedMessage)
                {
                    // Receive the save data
                    modSaveData = e.ReadAs<ModSaveData>();
                    // Refresh the furnace data
                    InitializeFurnaceControllers(false);

                    // If we have a menu open and we're looking at a furnace, the menu is most likely the output menu. Redraw it!
                    if (Game1.activeClickableMenu != null && currentlyLookingAtFurnace != -1)
                    {
                        DrawOutputMenu(furnaces[GetIndexOfFurnaceControllerWithTag(currentlyLookingAtFurnace)]);
                    }

                    UpdateTextures();
                    UpdateFurnaceLights();
                }
                else if (e.Type == requestSaveData)
                {
                    RequestSaveData request = e.ReadAs<RequestSaveData>();
                    Helper.Multiplayer.SendMessage<ModSaveData>(modSaveData, saveDataRefreshedMessage, new string[] { ModManifest.UniqueID }, new long[] { request.PlayerID });
                }
            }
        }


        /// <summary>Raised before/after the game writes data to save file.</summary>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Game1.player.IsMainPlayer)
            {
                InitializeSaveData();
                this.Helper.Data.WriteSaveData(controllerDataSaveKey, modSaveData);
            }
        }


        /// <summary>Raised before/after the game reads data from a save file and initialises the world (including when day one starts on a new save).</summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Only the person hosting the world loads the furnace controllers' state from the save
            if (Game1.player.IsMainPlayer)
            {
                InitializeFurnaceControllers(true);
            }
        }


        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Ignore if player hasn't loaded in yet, or is stuck in a menu or cutscene
            if (!Context.IsPlayerFree)
                return;

            // Don't check if there are no furnaces
            if (furnaces.Count == 0)
                return;
            
            // This might fix the android issue, also lets the player place items with both clicks
            if (e.Button.IsActionButton() || e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight)
            {
                // Assumes furnaces can be built only on the farm and checks if player is on the farm map
                if (!Game1.currentLocation.IsFarm || !Game1.currentLocation.IsOutdoors)
                    return;

                foreach (IndustrialFurnaceController furnace in furnaces)
                {
                    // The clicked tile
                    Vector2 tile = e.Cursor.GrabTile;
                    Building building = furnace.furnace;

                    // Allow only clicks that happen when the cursor is above the furnace to prevent trapping android and possibly controller users
                    Vector2 cursorPosition = e.Cursor.Tile;
                    if (!building.occupiesTile(cursorPosition))
                        continue;


                    // The mouth of the furnace
                    if (tile.X == building.tileX.Value + 1 && tile.Y == building.tileY.Value + 1)
                    {
                        if (PlaceItemsToTheFurnace(furnace))
                        {
                            Game1.playSound("coin");

                            SendUpdateMessage();
                        }

                        Helper.Input.Suppress(e.Button);
                    }
                    // The output chest of the furnace
                    else if (tile.X == building.tileX.Value + 3 && tile.Y == building.tileY.Value + 1)
                    {
                        CollectItemsFromTheFurnace(furnace);
                        Helper.Input.Suppress(e.Button);
                    }
                }
            }
        }


        /// <summary>The event called when the day starts.</summary>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Refresh the textures
            furnaceOn = Helper.Content.Load<Texture2D>(Path.Combine("assets", GetTexturePath("On")));
            furnaceOff = Helper.Content.Load<Texture2D>(Path.Combine("assets", GetTexturePath("Off")));


            if (Game1.player.IsMainPlayer)
            {
                // Finish smelting items
                foreach (IndustrialFurnaceController furnace in furnaces)
                {
                    if (furnace.CurrentlyOn)
                    {
                        FinishSmelting(furnace);
                    }
                }

                SendUpdateMessage();
            }
            else if (modSaveData is null)
            {
                Helper.Multiplayer.SendMessage<RequestSaveData>(new RequestSaveData(Game1.player.UniqueMultiplayerID), requestSaveData, new string[] { ModManifest.UniqueID });
            }
        }


        /// <summary>Raised after buildings are added/removed in any location.</summary>
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            // Add added furnaces to the controller list
            foreach (Building building in e.Added)
            {
                if (IsBuildingIndustrialFurnace(building))
                {
                    // Add the controller that takes care of the functionality of the furnace
                    IndustrialFurnaceController controller = new IndustrialFurnaceController(furnacesBuilt, false, this)
                    {
                        furnace = building
                    };

                    furnaces.Add(controller);
                    furnacesBuilt++;
                }
            }

            // Remove destroyed furnaces from the controller list
            foreach (Building building in e.Removed)
            {
                if (IsBuildingIndustrialFurnace(building))
                {
                    int index = GetIndexOfFurnaceControllerWithTag(building.maxOccupants.Value);

                    if (index > -1)
                    {
                        furnaces.RemoveAt(index);
                    }
                }
            }
        }


        /// <summary>The event called after an active menu is opened or closed.</summary>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Add the blueprint
            if (e.NewMenu is CarpenterMenu carpenterMenu)
            {
                bool isMagicalMenu = Helper.Reflection.GetField<bool>(carpenterMenu, "magicalConstruction").GetValue();

                if (isMagicalMenu) return;

                IList<BluePrint> blueprints = Helper.Reflection
                    .GetField<List<BluePrint>>(carpenterMenu, "blueprints")
                    .GetValue();

                // Add furnace blueprint, and tag it uniquely based on how many have been built
                blueprints.Add(new BluePrint(furnaceBuildingType)
                {
                    maxOccupants = furnacesBuilt,
                });
            }
            // If a menu was closed, reset the currently looked at furnace, just in case.
            else if (Game1.activeClickableMenu is null)
            {
                currentlyLookingAtFurnace = -1;
            }
        }


        /// <summary>Raised after the game world is drawn to the sprite patch, before it's rendered to the screen.
        /// Content drawn to the sprite batch at this point will be drawn over the world, but under any active menu, HUD elements, or cursor.</summary>
        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            
            foreach (IndustrialFurnaceController controller in furnaces)
            {
                // Assumes the furnace is built in the farm to avoid rendering the notification bubble anywhere else
                if (!Game1.player.currentLocation.IsFarm || !Game1.player.currentLocation.IsOutdoors) continue;

                // This gets called before building list gets changed, so check if the furnace has been added yet
                //int index = GetIndexOfFurnaceControllerWithTag(building.maxOccupants.Value);
                //if (index == -1) continue;

                // Copied from Mill.cs draw(SpriteBatch b) with slight edits

                // Check if there is items to render
                if (controller.output.items.Count <= 0 || controller.output.items[0] == null)
                    return;

                Building building = controller.furnace;

                // Get the bobbing from current time
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2));

                e.SpriteBatch.Draw(Game1.mouseCursors,
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(building.tileX.Value * 64 + 180, building.tileY.Value * 64 - 64 + num)),
                    new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f,
                    SpriteEffects.None,
                    (float)((building.tileY.Value + 1) * 64 / 10000.0 + 9.99999997475243E-07 + building.tileX.Value / 10000.0));

                e.SpriteBatch.Draw(Game1.objectSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(building.tileX.Value * 64 + 185 + 32 + 4, building.tileY.Value * 64 - 32 + 8 + num)),
                    new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, controller.output.items[0].ParentSheetIndex, 16, 16)),
                    Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None,
                    (float)((building.tileY.Value + 1) * 64 / 10000.0 + 9.99999974737875E-06 + building.tileX.Value / 10000.0));
            }
        }


        /// <summary>Raised after the current player moves to a new location.</summary>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                UpdateFurnaceLights();
        }


        /// <summary>Place items to the furnace</summary>
        /// <param name="furnace">The furnace controller</param>
        /// <returns>Whether the placement was successful or not</returns>
        private bool PlaceItemsToTheFurnace(IndustrialFurnaceController furnace)
        {
            // Items can be placed only if the furnace is NOT on
            if (furnace.CurrentlyOn)
            {
                DisplayMessage(i18n.Get("message.furnace-running"), 3, "cancel");
                return false;
            }

            // Get the current held object, null for tools etc.
            SObject heldItem = Game1.player.ActiveObject;
            if (heldItem == null) return false;

            int objectId = heldItem.ParentSheetIndex;
            SmeltingRule rule = newSmeltingRules.GetSmeltingRuleFromInputID(objectId);

            // Check if the object is on the smeltables list
            if (rule != null)
            {
                // Prevent the game from division by 0, even if the player edits the rules
                if (rule.InputItemAmount == 0)
                {
                    Monitor.Log($"The smelting rule for object {objectId} had 0 for input amount.", LogLevel.Error);
                    return false;
                }

                int amount = heldItem.Stack;

                // Check if the player has enough to smelt
                if (amount >= rule.InputItemAmount)
                {
                    // Remove multiples of the required input amount
                    int smeltAmount = amount / rule.InputItemAmount;
                    Game1.player.removeItemsFromInventory(objectId, smeltAmount * rule.InputItemAmount);
                    furnace.AddItemsToSmelt(objectId, smeltAmount * rule.InputItemAmount);

                    Monitor.Log($"{Game1.player.Name} placed {smeltAmount * rule.InputItemAmount} {heldItem.Name} to the furnace {furnace.ID}.");
                    return true;
                }
                else
                {
                    DisplayMessage(i18n.Get("message.need-more-ore", new { oreAmount = rule.InputItemAmount }), 3, "cancel");
                    return false;
                }
            }
            // Check if the player tries to put coal in the furnace and start the smelting
            else if (objectId == SObject.coal && !furnace.CurrentlyOn)
            {
                // The input has items to smelt
                if (furnace.input.items.Count > 0)
                {
                    if (heldItem.Stack >= config.CoalAmount)
                    {
                        Game1.player.removeItemsFromInventory(objectId, config.CoalAmount);

                        Monitor.Log($"{Game1.player.Name} started the furnace {furnace.ID} with {config.CoalAmount} {heldItem.Name}.");

                        if (config.InstantSmelting)
                        {
                            Monitor.Log("And it finished immediately.");
                            FinishSmelting(furnace);
                        }
                        else
                        {
                            furnace.ChangeCurrentlyOn(true);
                            UpdateTexture(furnace.furnace, true);

                            CreateLight(furnace);
                        }

                        Game1.playSound("furnace");
                        return true;
                    }
                    else
                    {
                        DisplayMessage(i18n.Get("message.more-coal", new { coalAmount = config.CoalAmount }), 3, "cancel");
                        return false;
                    }
                }
                else
                {
                    DisplayMessage(i18n.Get("message.place-something-first"), 3, "cancel");
                    return false;
                }
            }
            else
            {
                DisplayMessage(i18n.Get("message.cant-smelt-this"), 3, "cancel");
                return false;
            }
        }


        /// <summary>Called when the player tries to interact with the output chest</summary>
        /// <param name="furnace">The furnace controller that's being interacted with</param>
        private void CollectItemsFromTheFurnace(IndustrialFurnaceController furnace)
        {
            // Clear the output of removed items
            furnace.output.clearNulls();

            // Show output chest only if it contains something
            if (furnace.output.items.Count == 0) return;

            currentlyLookingAtFurnace = furnace.ID;
            DrawOutputMenu(furnace);
        }


        /// <summary>Processes the input chest's items and places the result to the output</summary>
        /// <param name="furnace"></param>
        private void FinishSmelting(IndustrialFurnaceController furnace)
        {
            // TODO: Add checks to prevent loss of items, since it is possible that 'output amount' > 'input amount'

            Monitor.Log("Processing the outputs.");

            // Collect the object data to a dictionary (ID, amount) first to fix losing items with over 999 stacks
            Dictionary<int, int> smeltablesDictionary = new Dictionary<int, int>();

            foreach (Item item in furnace.input.items)
            {
                int objectId = item.ParentSheetIndex;

                if (smeltablesDictionary.ContainsKey(objectId))
                {
                    smeltablesDictionary[objectId] += item.Stack;
                }
                else
                {
                    smeltablesDictionary.Add(objectId, item.Stack);
                }
            }

            // Now the dictionary consists of ItemID: Amount
            foreach (KeyValuePair<int, int> kvp in smeltablesDictionary)
            {
                SmeltingRule rule = newSmeltingRules.GetSmeltingRuleFromInputID(kvp.Key);

                if (rule is null)
                {
                    // This should never be hit, but let's error it just incase...
                    Monitor.Log($"Item with ID {kvp.Key} wasn't in the smelting rules despite being in the input chest!", LogLevel.Error);
                    continue;
                }

                if (rule.InputItemAmount == 0)
                {
                    Monitor.Log($"The input amount for object {kvp.Key} was 0. The result can't be processed so the item will be voided.", LogLevel.Error);
                }

                int outputAmount = (kvp.Value / rule.InputItemAmount) * rule.OutputItemAmount;

                Monitor.Log($"Found {kvp.Value} objects with ID {kvp.Key}. The smelting result is {outputAmount} objects of ID {rule.OutputItemID}.");

                // Add the result defined by the smelting rule to the output chest
                // Assumes the value is divisible with the input amount
                furnace.AddItemsToSmeltedChest(rule.OutputItemID, outputAmount);
            }

            for (int i = 0; i < furnace.input.items.Count; i++)
            {
                furnace.input.items[i] = null;
            }
            furnace.input.clearNulls();
            furnace.ChangeCurrentlyOn(false);

            // Update the texture of the furnace
            UpdateTexture(furnace.furnace, false);
        }


        /// <summary>Returns the index of the matching controller in the furnaces list</summary>
        /// <param name="tag">The tag of searched furnace controller</param>
        /// <returns>Either the index or -1 if no tag matches are found</returns>
        private int GetIndexOfFurnaceControllerWithTag(int tag)
        {
            for (int i = 0; i < furnaces.Count; i++)
            {
                // Assumes the furnace has been added to the list once
                if (furnaces[i].ID == tag)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>Switches the building's texture between ON and OFF versions</summary>
        /// <param name="building"></param>
        /// <param name="currentlyOn"></param>
        private void UpdateTexture(Building building, bool currentlyOn)
        {
            if (currentlyOn)
            {
                building.texture = new Lazy<Texture2D>(() => this.furnaceOn);
            }
            else
            {
                building.texture = new Lazy<Texture2D>(() => this.furnaceOff);
            }
        }


        /// <summary>Updates the textures of all furnaces. Used to sync with multiplayer save data changes.</summary>
        private void UpdateTextures()
        {
            for (int i = 0; i < furnaces.Count; i++)
            {
                UpdateTexture(furnaces[i].furnace, furnaces[i].CurrentlyOn);
            }
        }


        /// <summary>
        /// Update the light sources for furnaces, but only of the player is on the farm map.
        /// Assumes only farm can have built buildings!
        /// </summary>
        private void UpdateFurnaceLights()
        {
            for (int i = 0; i < furnaces.Count; i++)
            {
                if (furnaces[i].CurrentlyOn)
                {
                    // Assumes only farm can have built buildings!
                    if (Game1.player.currentLocation != Game1.getFarm())
                        continue;

                    if (furnaces[i].lightSource is null)
                    {
                        CreateLight(furnaces[i]);
                    }
                    else if (!Game1.currentLightSources.Contains(furnaces[i].lightSource))
                    {
                        Game1.currentLightSources.Add(furnaces[i].lightSource);
                    }
                }
                else if (furnaces[i].lightSource != null)
                {
                    if (Game1.currentLightSources.Contains(furnaces[i].lightSource))
                    {
                        Game1.currentLightSources.Remove(furnaces[i].lightSource);
                    }

                    furnaces[i].lightSource = null;
                }
            }
        }


        /// <summary>
        /// Create a new light source and link it to the furnace
        /// </summary>
        /// <param name="controller"></param>
        private void CreateLight(IndustrialFurnaceController controller)
        {
            Building building = controller.furnace;
            Vector2 pos = new Vector2(building.tileX.Value * 64 + fireAnimationData.LightSourceXOffset, building.tileY.Value * 64 + fireAnimationData.LightSourceYOffset);

            LightSource light = new LightSource(4, pos,
                fireAnimationData.LightSourceScaleMultiplier, Color.DarkCyan, controller.ID * lightSourceIDMultiplier);

            // Make the furnace light up the area
            Game1.currentLightSources.Add(light);
            controller.lightSource = light;
        }


        /// <summary>Displays a HUD message of defined type with a possible sound effect</summary>
        /// <param name="s">Displayed message</param>
        /// <param name="type">Message type</param>
        /// <param name="sound">Sound effect</param>
        private void DisplayMessage(string s, int type, string sound = null)
        {
            Game1.addHUDMessage(new HUDMessage(s, type));

            if (sound != null)
            {
                Game1.playSound(sound);
            }
        }


        /// <summary>Remove rules that depend on not installed mods</summary>
        private void CheckSmeltingRules()
        {
            newSmeltingRules.SmeltingRules.RemoveAll(item => item.RequiredModID != null && !Helper.ModRegistry.IsLoaded(item.RequiredModID));
        }


        /// <summary>Update the furnace data from the save data</summary>
        private void InitializeFurnaceControllers(bool readSaveData)
        {
            // Initialize the lists to prevent data leaking from previous games
            furnaces.Clear();
            furnaces.Clear();

            // Load the saved data. If not present, initialize new
            if (readSaveData)
                modSaveData = Helper.Data.ReadSaveData<ModSaveData>(controllerDataSaveKey);

            if (modSaveData is null)
            {
                modSaveData = new ModSaveData();
            }
            else
            {
                modSaveData.ParseModSaveDataToControllers(furnaces, this);
            }

            // Update furnacesBuilt counter to match the highest id of built furnaces (+1)
            int highestId = -1;
            for (int i = 0; i < furnaces.Count; i++)
            {
                if (furnaces[i].ID > highestId) highestId = furnaces[i].ID;
            }
            furnacesBuilt = highestId + 1;

            // Repopulate the list of furnaces, only checks the farm!
            foreach (Building building in ((BuildableGameLocation)Game1.getFarm()).buildings)
            {
                if (IsBuildingIndustrialFurnace(building))
                {
                    for (int i = 0; i < furnaces.Count; i++)
                    {
                        if (building.maxOccupants.Value == furnaces[i].ID)
                        {
                            furnaces[i].furnace = building;
                        }
                    }
                }
            }
        }


        private void DrawOutputMenu(IndustrialFurnaceController furnace)
        {
            // Display the menu for the output chest
            Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu(
                furnace.output.items,
                false,
                true,
                new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                null,
                (string)null,
                (item, farmer) => furnace.GrabItemFromChest(item, farmer),
                false,
                true,
                true,
                true,
                false,
                0,
                null,
                -1,
                null);
        }


        /// <summary>Update the save data to match the controllers data</summary>
        private void InitializeSaveData()
        {
            modSaveData.ClearOldData();
            modSaveData.ParseControllersToModSaveData(furnaces);
        }


        /// <summary>Get the texture name, checks for seasonal textures</summary>
        /// <param name="state">The state of the furnace, either "On" or "Off"</param>
        private string GetTexturePath(string state)
        {
            string textureName = $"{Game1.currentSeason}_IndustrialFurnace{state}.png";

            if (File.Exists(Path.Combine(Helper.DirectoryPath, "assets", textureName)))
            {
                Monitor.Log($"Using the texture for {Game1.currentSeason}.");
                return textureName;
            }

            Monitor.Log($"Seasonal texture not found for season {Game1.currentSeason}. Using the default.");
            return $"IndustrialFurnace{state}.png";
        }
    }


    /// <summary>
    /// Interface for the GenericModConfigMenu api.
    /// </summary>
    public interface IGenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
    }
}