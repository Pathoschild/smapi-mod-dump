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
using HarmonyLib;
using Helpers;


namespace LongerFenceLife
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config;
        internal IModHelper MyHelper;
        internal static Logger Log;

        const float MinLife = 0.5f;
        const float MaxLife = 10.0f;

        const int WoodFence = 322;//-5 placed
        const int StoneFence = 323;//-6 placed
        const int IronFence = 324;//-7 placed
        const int HwFence = 298;//-8 placed
        const int Gate = 325;//-9 placed

        Texture2D Texture;

        internal bool FencesAdded;

        internal static bool Debug;

        public String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;
            Log = new Logger(this.Monitor);

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

            FencesAdded = false;
        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased += Input_ButtonReleased;
            if (!Config.UseHarmony)
            {
                MyHelper.Events.Player.InventoryChanged += Player_InventoryChanged;
                MyHelper.Events.World.ObjectListChanged += World_ObjectListChanged;
            }
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            MyHelper.Events.Display.RenderedWorld -= Display_OnRenderedWorld;
            if (!Config.UseHarmony)
            {
                MyHelper.Events.Player.InventoryChanged -= Player_InventoryChanged;
                MyHelper.Events.World.ObjectListChanged -= World_ObjectListChanged;
            }
        }

        private static float ClampRange(float life)
        {
            if (life < MinLife)
                life = MinLife;
            else if (life > MaxLife)
                life = MaxLife;
            return life;
        }

        private static int VanillaFenceLife(int fenceType)
        {
            int baseLife = fenceType switch
            {
                WoodFence => 28 * 2,
                StoneFence => 60 * 2,
                IronFence => 125 * 2,
                Gate => 100 * 2 * 2,
                HwFence => 280 * 2,
                _ => 100 * 2
            };
            return baseLife;
        }

        private static string FenceYears(int fenceType, float value)
        {
            float baseLife = (float) VanillaFenceLife(fenceType);
            value = value * baseLife / 112f;
            return value.ToString("F1");
        }

        private static string WoodFenceYears(float value)
        {
            return FenceYears(WoodFence, value);
        }

        private static string StoneFenceYears(float value)
        {
            return FenceYears(StoneFence, value);
        }

        private static string IronFenceYears(float value)
        {
            return FenceYears(IronFence, value);
        }

        private static string GateYears(float value)
        {
            return FenceYears(Gate, value);
        }

        private static string HwFenceYears(float value)
        {
            return FenceYears(HwFence, value);
        }

        private string VanillaLifeText(int FenceType)
        {
            float years = VanillaFenceLife((int)FenceType) / 112f;
            return MyHelper.Translation.Get("VanillaLife", new { years = years.ToString("F1") });
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
#if DEBUG
            Debug = true;
#endif

            if (Config.UseHarmony)
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);
                //harmony.PatchAll();
                System.Reflection.MethodInfo mInfo;

                try
                {
                    mInfo = harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Fence), nameof(StardewValley.Fence.ResetHealth)),
                                          postfix: new HarmonyMethod(typeof(FencePatches), nameof(FencePatches.ResetHealth_Postfix))
                                         );
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed Harmony Fence Patches:\n{ex}");
                    Config.UseHarmony = false;
                }
            }

            // use GMCM in an optional manner.

            //IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var gmcm = Helper.ModRegistry.GetGenericModConfigMenuApi(this.Monitor);
            if (gmcm != null)
            {
                gmcm.Register(ModManifest,
                              reset: () => Config = new ModConfig(),
                              save: () => Helper.WriteConfig(Config));

                gmcm.AddParagraph(ModManifest, () => I18nGet("LifeYears"));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.WoodFenceLife,
                                     (float value) => Config.WoodFenceLife = value,
                                     () => I18nGet("woodFence.Label"),
                                     () => I18nGet("woodFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: WoodFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(WoodFence));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.StoneFenceLife,
                                     (float value) => Config.StoneFenceLife = value,
                                     () => I18nGet("stoneFence.Label"),
                                     () => I18nGet("stoneFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: StoneFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(StoneFence));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.IronFenceLife,
                                     (float value) => Config.IronFenceLife = value,
                                     () => I18nGet("ironFence.Label"),
                                     () => I18nGet("ironFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: IronFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(IronFence));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.HardwoodFenceLife,
                                     (float value) => Config.HardwoodFenceLife = value,
                                     () => I18nGet("hardwoodFence.Label"),
                                     () => I18nGet("hardwoodFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: HwFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(HwFence));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.GateLife,
                                     (float value) => Config.GateLife = value,
                                     () => I18nGet("gate.Label"),
                                     () => I18nGet("gate.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: GateYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(Gate));

                gmcm.AddKeybind(ModManifest,
                                () => Config.FenceLifeKeybind,
                                (SButton value) => Config.FenceLifeKeybind = value,
                                () => I18nGet("fenceLife.Label"),
                                () => I18nGet("fenceLife.tooltip"));
            }
            else
            {
                Log.LogOnce("Generic Mod Config Menu not available.");
            };
        }

        /// <summary>Called when objects are added/removed from a/the map.
        /// Fence repair does not trigger this event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (
                //Context.IsPlayerFree &&
                e.IsCurrentLocation &&
                (e.Added != null)
               )
            {
                FencesAdded = false;

                foreach (var item in e.Added)
                {
                    if (item.Value is StardewValley.Fence fence)
                    {
                        FencesAdded = true;

                        float before = fence.health.Value;
                        float beforeMax = fence.maxHealth.Value;
                        int idx = fence.GetItemParentSheetIndex();
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
                        fence.maxHealth.Value = beforeMax * mult;

                        if (Debug)
                            Log.Debug($"ObjectListChanged: health after={fence.health.Value}, health before={before}, idx={idx},{fence.ParentSheetIndex}");
                    }
                }
            }
        }

        /// <summary>Called when the player inventory has changed.
        /// This method implements our detection of when a fence has been placed, and our resulting actions.
        /// We use this for fence repair situations. We get this event after ObjectListChanged.
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
                //Context.IsPlayerFree &&
                e.IsLocalPlayer
               )
            {
                Vector2 tilePosition = Game1.currentCursorTile;
                StardewValley.Object obj;
                if (
                    (!FencesAdded) &&
                    (e.QuantityChanged.FirstOrDefault(x => x.Item == grabbed) is ItemStackSizeChange item) &&
                    (item.NewSize+1 == item.OldSize) &&
                    Game1.currentLocation.Objects.TryGetValue(tilePosition, out obj) &&
                    (obj is StardewValley.Fence fence) &&
                    //Game1.currentLocation.Objects.ContainsKey(tilePosition) &&
                    //(Game1.currentLocation.Objects[tilePosition] is StardewValley.Fence fence) &&
                    (fence.GetItemParentSheetIndex() == idx)
                   )
                {
                    // game has a bug where if a gate replaces an existing fence post the item id stays with the fence id
                    // and the gate keeps the fence life. it does not get the gate life.
                    // by fence id I mean the converted negative values of placed fence items.
                    // we could detect and correct this, but the game is the game.
                    // this mod just alters the life the game literally gives.
                    // fence.performObjectDropInAction()

                    // for some reason the game does not adjust the maxHealth of gates. so health is normally above maxHealth. probably a bug.

                    // fence repair: the repaired post with updated health is what we see at this event.

                    float before = fence.health.Value;
                    float beforeMax = fence.maxHealth.Value;
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
                    fence.maxHealth.Value = beforeMax * mult;

                    if (Debug)
                        Log.Debug($"InventoryChanged: health after={fence.health.Value}, health before={before}, idx={idx},{fence.ParentSheetIndex}, repair={fence.repairQueued.Value}");
                }
                else if (Debug)
                {
                    Log.Debug($"InventoryChanged: FencesAdded={FencesAdded}");

                    if (e.QuantityChanged.FirstOrDefault(x => x.Item == grabbed) is ItemStackSizeChange itemD)
                    {
                        Log.Debug($"item newSize={itemD.NewSize}, oldSize={itemD.OldSize}");
                        if (Game1.currentLocation.Objects.ContainsKey(tilePosition))
                        {
                            if (Game1.currentLocation.Objects[tilePosition] is not StardewValley.Fence)
                                Log.Debug("Failing is Fence");
                        }
                        else
                            Log.Debug("Failing ContainsKey");
                    }
                    else
                        Log.Debug("Failing FirstOrDefault");
                }
            }

            FencesAdded = false;
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

                    StardewValley.Object obj;

                    Rectangle visibleArea = GetVisibleAreaInTiles(1);
                    for (int x = visibleArea.X; x < visibleArea.X+visibleArea.Width; x++)
                    {
                        tile.X = x;
                        for (int y = visibleArea.Y; y < visibleArea.Y+visibleArea.Height; y++)
                        {
                            tile.Y = y;
                            if (location.Objects.TryGetValue(tile, out obj) && (obj is StardewValley.Fence fence1))
                            {
                                Color color = Color.Green;
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
                    if (location.Objects.TryGetValue(tile, out obj) && (obj is StardewValley.Fence fence2))
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
                GameLocation location = Game1.currentLocation;
                StardewValley.Object obj;

                if (e.Button == Config.FenceLifeKeybind)
                {
                    MyHelper.Events.Display.RenderedWorld += Display_OnRenderedWorld;
                }
                else if ((e.Button == SButton.MouseLeft) &&
                         MyHelper.Input.IsDown(Config.FenceLifeKeybind) &&
                         location.Objects.TryGetValue(Game1.currentCursorTile, out obj) &&
                         (obj is StardewValley.Fence fence)
                        //location.Objects.ContainsKey(Game1.currentCursorTile) &&
                        //(location.Objects[Game1.currentCursorTile] is StardewValley.Fence fence)
                        )
                {
                    fence.repair();
                    Helper.Input.Suppress(SButton.MouseLeft);
                }
                else if (Debug)
                {
                    if ((e.Button == SButton.F5) || (e.Button == SButton.F6) || (e.Button == SButton.F7))
                    {
                        GameLocation farm = Game1.getLocationFromName("Farm");
                        foreach (StardewValley.Object obj1 in farm.objects.Values)
                        {
                            if (obj1 is Fence f)
                            {
                                if (e.Button == SButton.F5)
                                    f.minutesElapsed(1440, farm);//one day by fence.minutesElaspsed logic.
                                else if (e.Button == SButton.F6)
                                    f.health.Value = Game1.random.Next(2, 10);
                                else if (e.Button == SButton.F7)
                                    f.health.Value = f.maxHealth.Value * (float)Game1.random.NextDouble();
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

    [HarmonyPatch(typeof(StardewValley.Fence))]
    public class FencePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StardewValley.Fence.ResetHealth))]
        [HarmonyPatch(new Type[] { typeof(float) })]
        public static void ResetHealth_Postfix(StardewValley.Fence __instance, float amount_adjustment)
        {
            try
            {
                float mult = __instance.whichType.Value switch
                {
                    1 => ModEntry.Config.WoodFenceLife,
                    2 => ModEntry.Config.StoneFenceLife,
                    3 => ModEntry.Config.IronFenceLife,
                    4 => ModEntry.Config.GateLife,
                    5 => ModEntry.Config.HardwoodFenceLife,
                    _ => 1.0f,
                };
                float before = __instance.health.Value;
                float beforeMax = __instance.maxHealth.Value;

                __instance.health.Value = before * mult;
                __instance.maxHealth.Value = beforeMax * mult;

                if (ModEntry.Debug)
                    ModEntry.Log.Debug($"ResetHealth_PostFix: health after={__instance.health.Value}, health before={before}, fence_type={__instance.whichType}");
            }
            catch (Exception ex)
            {
                ModEntry.Log.Error($"Failed in {nameof(ResetHealth_Postfix)}:\n{ex}");
            }
        }
    }
}

