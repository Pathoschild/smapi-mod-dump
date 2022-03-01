/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
//using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using GenericModConfigMenu;
//using xTile.Layers;


namespace LongerFenceLife
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        internal IModHelper MyHelper;

        const float MinLife = 0.5f;
        const float MaxLife = 10.0f;

        const int WoodFence = 322;//-5 placed
        const int StoneFence = 323;//-6 placed
        const int IronFence = 324;//-7 placed
        const int HwFence = 298;//-8 placed
        const int Gate = 325;//-9 placed

        Texture2D Texture;

        internal bool Debug;

#if Debug
        const LogLevel LogType = LogLevel.Debug;
#else
        const LogLevel LogType = LogLevel.Trace;
#endif

        public String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;

            MyHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            MyHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            MyHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            //Texture = new Texture2D(Game1.graphics.GraphicsDevice, Game1.tileSize, Game1.tileSize);
            //Color[] arr = new Color[Game1.tileSize*Game1.tileSize];
            //for (int i = 0; i < arr.Length; i++)
            //    arr[i] = Color.White;
            //Texture.SetData(arr);
            Texture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Texture.SetData(new[] { Color.White });
        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased += Input_ButtonReleased;
            MyHelper.Events.Player.InventoryChanged += Player_InventoryChanged;
            //MyHelper.Events.World.ObjectListChanged += World_ObjectListChanged;
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            MyHelper.Events.Player.InventoryChanged -= Player_InventoryChanged;
            //MyHelper.Events.World.ObjectListChanged -= World_ObjectListChanged;
            MyHelper.Events.Display.RenderedWorld -= Display_OnRenderedWorld;
        }

        private static float ClampRange(float life)
        {
            if (life < MinLife)
                life = MinLife;
            else if (life > MaxLife)
                life = MaxLife;
            return life;
        }

        /// <summary>Raised after the game has loaded and all Mods are loaded. Here we load the config.json file and setup GMCM </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = MyHelper.ReadConfig<ModConfig>();

            // clamp these values in case someone edits the config manually
            Config.WoodFenceLife = ClampRange(Config.WoodFenceLife);
            Config.StoneFenceLife = ClampRange(Config.StoneFenceLife);
            Config.IronFenceLife = ClampRange(Config.IronFenceLife);
            Config.HardwoodFenceLife = ClampRange(Config.HardwoodFenceLife);
            Config.GateLife = ClampRange(Config.GateLife);
            Debug = Config.Debug;

#if Debug
            Debug = true;
#endif

            // use GMCM in an optional manner.

            //IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var gmcm = Helper.ModRegistry.GetGenericModConfigMenuApi(this.Monitor);
            if (gmcm != null)
            {
                gmcm.Register(ModManifest,
                              reset: () => Config = new ModConfig(),
                              save: () => Helper.WriteConfig(Config));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.WoodFenceLife,
                                     (float value) => Config.WoodFenceLife = value,
                                     () => I18nGet("woodFence.Label"),
                                     () => I18nGet("woodFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f);
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.StoneFenceLife,
                                     (float value) => Config.StoneFenceLife = value,
                                     () => I18nGet("stoneFence.Label"),
                                     () => I18nGet("stoneFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f);
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.IronFenceLife,
                                     (float value) => Config.IronFenceLife = value,
                                     () => I18nGet("ironFence.Label"),
                                     () => I18nGet("ironFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f);
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.HardwoodFenceLife,
                                     (float value) => Config.HardwoodFenceLife = value,
                                     () => I18nGet("hardwoodFence.Label"),
                                     () => I18nGet("hardwoodFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f);
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.GateLife,
                                     (float value) => Config.GateLife = value,
                                     () => I18nGet("gate.Label"),
                                     () => I18nGet("gate.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f);
                gmcm.AddKeybind(ModManifest,
                                () => Config.FenceLifeKeybind,
                                (SButton value) => Config.FenceLifeKeybind = value,
                                () => I18nGet("fenceLife.Label"),
                                () => I18nGet("fenceLife.tooltip"));
            }
            else
            {
                Monitor.LogOnce("Generic Mod Config Menu not available.", LogLevel.Info);
            };
        }

        //private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        //{
        //    if (
        //        Context.IsPlayerFree &&
        //        e.IsCurrentLocation &&
        //        (e.Added != null)
        //       )
        //    {
        //        foreach (var item in e.Added)
        //        {
        //            if (item.Value is StardewValley.Fence fence)
        //            {
        //                float before = fence.health.Value;
        //                float baseHealth = before / 2.0f;
        //                int idx = fence.GetItemParentSheetIndex();
        //                float mult = idx switch
        //                {
        //                    WoodFence => Config.WoodFenceLife,
        //                    StoneFence => Config.StoneFenceLife,
        //                    IronFence => Config.IronFenceLife,
        //                    Gate => Config.GateLife,
        //                    HwFence => Config.HardwoodFenceLife,
        //                    _ => 1.0f,
        //                };

        //                fence.health.Value = before * mult;
        //                fence.maxHealth.Value = fence.health.Value;
        //                //fence.ResetHealth((baseHealth * mult) - baseHealth);

        //                if (Debug)
        //                    Monitor.Log($"health after={fence.health.Value}, health before={before}, idx={idx}", LogType);
        //            }
        //        }
        //    }
        //}

        /// <summary>Called when the player inventory has changed.
        /// This method implements our detection of when a fence has been placed, and our resulting actions.
        /// We cannot use the ObjectListChanged event because in a fence repair situation we do not receive any event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            Item grabbed = e.Player.mostRecentlyGrabbedItem;
            if (grabbed == null)
                return;

            int idx = grabbed.ParentSheetIndex;
            if (
                (((idx >= WoodFence) && (idx <= Gate)) || (idx == HwFence)) &&
                Context.IsPlayerFree &&
                e.IsLocalPlayer
               )
            {
                Vector2 tilePosition = Game1.currentCursorTile;
                if (
                    (e.QuantityChanged.FirstOrDefault(x => x.Item == grabbed) is ItemStackSizeChange item) &&
                    (item.NewSize+1 == item.OldSize) &&
                    Game1.currentLocation.Objects.ContainsKey(tilePosition) &&
                    (Game1.currentLocation.Objects[tilePosition] is StardewValley.Fence fence) &&
                    (fence.GetItemParentSheetIndex() == idx)
                   )
                {
                    // game has a bug where if a gate replaces an existing fence post the item id stays with the fence id
                    // and the gate keeps the fence life. it does not get the gate life.
                    // by fence id I mean the converted negative values of placed fence items.
                    // we could detect and correct this, but the game is the game.
                    // this mod just alters the life the game literally gives.
                    // fence.performObjectDropInAction()

                    // the game fence ResetHealth code takes a base health (which we cannot change) and multiplies it by 2.
                    // the param for ResetHealth is an additive adjustment value to the base health before the *2.
                    // the additive value is basically used to provide health variation.
                    // we use the adjust value to make our adjustments to the overall health (and maxHealth).
                    // gates: the adjust value is ignored for gates.

                    // fence repair: the repaired post with updated health is dropped and that is what we see

                    // the health and maxHealth fields are readonly, but the Value property can be accessed.
                    // using the API the game uses to set health (ResetHealth) seems best but we are taking knowledge of what the game
                    // does inside that method to properly use the available parameter.
                    // we would be using that param outside of its intended use. health variation.

                    // we just alter the health values directly. this also allows us to adjust gate life.

                    float before = fence.health.Value;
                    float baseHealth = before / 2.0f;
                    float mult = idx switch
                    {
                        WoodFence => Config.WoodFenceLife,
                        StoneFence => Config.StoneFenceLife,
                        IronFence => Config.IronFenceLife,
                        Gate => Config.GateLife,
                        HwFence => Config.HardwoodFenceLife,
                        _ => 1.0f,
                    };

                    fence.health.Value = before * mult;
                    fence.maxHealth.Value = fence.health.Value;
                    //fence.ResetHealth((baseHealth * mult) - baseHealth);

                    if (Debug)
                        Monitor.Log($"health after={fence.health.Value}, health before={before}, idx={idx},{fence.ParentSheetIndex}", LogType);
                }
                else if (Debug)
                {
                    if (e.QuantityChanged.FirstOrDefault(x => x.Item == grabbed) is ItemStackSizeChange itemD)
                    {
                        Monitor.Log($"item newSize={itemD.NewSize}, oldSize={itemD.OldSize}", LogType);
                        if (Game1.currentLocation.Objects.ContainsKey(tilePosition))
                        {
                            if (Game1.currentLocation.Objects[tilePosition] is not StardewValley.Fence)
                                Monitor.Log("Failing is Fence", LogType);
                        }
                        else
                            Monitor.Log("Failing ContainsKey", LogType);
                    }
                    else
                        Monitor.Log("Failing FirstOrDefault", LogType);
                }
            }
        }

        public static Rectangle GetVisibleAreaInTiles(int expand = 0)
        {
            return new Rectangle(
                x: (Game1.viewport.X / Game1.tileSize) - expand,
                y: (Game1.viewport.Y / Game1.tileSize) - expand,
                width: (int)Math.Ceiling(Game1.viewport.Width / (decimal)Game1.tileSize) + (expand * 2),
                height: (int)Math.Ceiling(Game1.viewport.Height / (decimal)Game1.tileSize) + (expand * 2)
            );
        }

        /// <summary>Raised after drawing the world to the sprite batch, but before it's rendered to the screen.
        /// This method draws the fence display life overlay and tooltip.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Display_OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (MyHelper.Input.IsDown(Config.FenceLifeKeybind))
            {
                GameLocation location = Game1.currentLocation;
                if (location != null)
                {
                    Vector2 view = new Vector2(Game1.viewport.X, Game1.viewport.Y);
                    Vector2 tile = new Vector2(0, 0);

                    Rectangle visibleArea = GetVisibleAreaInTiles(1);
                    for (int x = visibleArea.X; x < visibleArea.X+visibleArea.Width; x++)
                    {
                        tile.X = x;
                        for (int y = visibleArea.Y; y < visibleArea.Y+visibleArea.Height; y++)
                        {
                            tile.Y = y;
                            if (location.Objects.ContainsKey(tile) && (location.Objects[tile] is StardewValley.Fence fence1))
                            {
                                Color color = Color.Green;
                                //float health = fence1.health.Value / fence1.maxHealth.Value;
                                //if (health <= 0.33)
                                //    color = Color.Red;
                                //else if (health <= 0.66)
                                //    color = Color.Yellow;
                                int daysLeft = (int)(fence1.health.Value * 1440f / 60 / 24);
                                if (daysLeft <= 28)
                                    color = Color.Red;
                                else if (daysLeft <= 56)
                                    color = Color.Yellow;

                                Vector2 pixelPosition = tile * Game1.tileSize - view;
                                e.SpriteBatch.Draw(Texture,
                                                   new Rectangle((int)pixelPosition.X, (int)pixelPosition.Y, Game1.tileSize, Game1.tileSize),
                                                   color * 0.3f);
                            }
                        }
                    }

                    tile = Game1.currentCursorTile;
                    if (location.Objects.ContainsKey(tile) && (location.Objects[tile] is StardewValley.Fence fence2))
                    {
                        int daysLeft = (int)(fence2.health.Value * 1440f / 60 / 24);

                        IClickableMenu.drawHoverText(e.SpriteBatch,
                                                     MyHelper.Translation.Get("hover.tooltip", new { daysLeft = daysLeft.ToString() }),
                                                     Game1.smallFont,
                                                     0, 0);
                    }
                }
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.
        /// This method implements triggers the fence display life mechanism.///
        /// It also implements an instant fence decay for testing purposes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree)
            {
                if (e.Button == Config.FenceLifeKeybind)
                {
                    MyHelper.Events.Display.RenderedWorld += Display_OnRenderedWorld;
                }
                else if (Debug)
                {
                    if ((e.Button == SButton.F5) || (e.Button == SButton.F6) || (e.Button == SButton.F7))
                    {
                        GameLocation farm = Game1.getLocationFromName("Farm");
                        foreach (StardewValley.Object obj in farm.objects.Values)
                        {
                            if (obj is Fence f)
                            {
                                if (e.Button == SButton.F6)
                                    f.health.Value = Game1.random.Next(1, 10);
                                else if (e.Button == SButton.F7)
                                    f.health.Value = f.maxHealth.Value * (float)Game1.random.NextDouble();
                                else
                                    f.minutesElapsed(1440, farm);//one day by fence.minutesElaspsed logic.
                            }
                        }
                    }
                }
            }
        }
        
        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == Config.FenceLifeKeybind)
            {
                MyHelper.Events.Display.RenderedWorld -= Display_OnRenderedWorld;
            }
        }
    }
}

