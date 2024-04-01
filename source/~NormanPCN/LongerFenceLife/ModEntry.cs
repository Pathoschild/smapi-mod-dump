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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.GameData.Fences;
using GenericModConfigMenu;
//using HarmonyLib;
using Helpers;
using Microsoft.Xna.Framework.Media;


namespace LongerFenceLife
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config;
        internal IModHelper MyHelper;
        internal static Logger Log;

        const float MinLife = 0.5f;
        const float MaxLife = 10.0f;

        public const int WoodFenceId = 322;//-5 placed
        public const int StoneFenceId = 323;//-6 placed
        public const int IronFenceId = 324;//-7 placed
        public const int HwFenceId = 298;//-8 placed
        public const int GateId = 325;//-9 placed

        public const string WoodFenceQId = "(O)322";
        public const string StoneFenceQId = "(O)323";
        public const string IronFenceQId = "(O)324";
        public const string HwFenceQId = "(O)298";
        public const string GateQId = "(O)325";

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
            MyHelper.Events.Content.AssetRequested += OnAssetRequested;
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            MyHelper.Events.Display.RenderedWorld -= Display_OnRenderedWorld;
            MyHelper.Events.Content.AssetRequested -= OnAssetRequested;
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
                WoodFenceId => 28 * 2,
                StoneFenceId => 60 * 2,
                IronFenceId => 125 * 2,
                GateId => 100 * 2 * 2,
                HwFenceId => 280 * 2,
                _ => 100 * 2
            };
            return baseLife;
        }

        internal static int GetFenceIdx(StardewValley.Fence fence)
        {
            int idx = fence.QualifiedItemId switch
            {
                WoodFenceQId => WoodFenceId,
                StoneFenceQId => StoneFenceId,
                IronFenceQId => IronFenceId,
                GateQId => GateId,
                HwFenceQId => HwFenceId,
                _ => WoodFenceId
            };
            return idx;
        }

        private static string FenceYears(int fenceType, float value)
        {
            float baseLife = (float) VanillaFenceLife(fenceType);
            value = value * baseLife / 112f;
            return value.ToString("F1");
        }

        private static string WoodFenceYears(float value)
        {
            return FenceYears(WoodFenceId, value);
        }

        private static string StoneFenceYears(float value)
        {
            return FenceYears(StoneFenceId, value);
        }

        private static string IronFenceYears(float value)
        {
            return FenceYears(IronFenceId, value);
        }

        private static string GateYears(float value)
        {
            return FenceYears(GateId, value);
        }

        private static string HwFenceYears(float value)
        {
            return FenceYears(HwFenceId, value);
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

            //var harmony = new Harmony(this.ModManifest.UniqueID);
            //harmony.PatchAll();
            //System.Reflection.MethodInfo mInfo;

            //try
            //{
            //    mInfo = harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Fence), nameof(StardewValley.Fence.ResetHealth)),
            //                            postfix: new HarmonyMethod(typeof(FencePatches), nameof(FencePatches.ResetHealth_Postfix))
            //                            );
            //}
            //catch (Exception ex)
            //{
            //    Log.Error($"Failed Harmony Fence Patches:\n{ex}");
            //}

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
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(WoodFenceId));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.StoneFenceLife,
                                     (float value) => Config.StoneFenceLife = value,
                                     () => I18nGet("stoneFence.Label"),
                                     () => I18nGet("stoneFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: StoneFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(StoneFenceId));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.IronFenceLife,
                                     (float value) => Config.IronFenceLife = value,
                                     () => I18nGet("ironFence.Label"),
                                     () => I18nGet("ironFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: IronFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(IronFenceId));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.HardwoodFenceLife,
                                     (float value) => Config.HardwoodFenceLife = value,
                                     () => I18nGet("hardwoodFence.Label"),
                                     () => I18nGet("hardwoodFence.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: HwFenceYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(HwFenceId));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.GateLife,
                                     (float value) => Config.GateLife = value,
                                     () => I18nGet("gate.Label"),
                                     () => I18nGet("gate.tooltip"),
                                     min: MinLife,
                                     max: MaxLife,
                                     interval: 0.1f,
                                     formatValue: GateYears);
                gmcm.AddParagraph(ModManifest, () => VanillaLifeText(GateId));

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

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fences"))
            {
                e.Edit(asset =>
                {
                    var fences = asset.AsDictionary<string, FenceData>();
                    int health;

                    health = fences.Data[StardewValley.Fence.woodFenceId].Health;
                    health = (int)((float)health * Config.WoodFenceLife);
                    fences.Data[StardewValley.Fence.woodFenceId].Health = health;

                    health = fences.Data[StardewValley.Fence.stoneFenceId].Health;
                    health = (int)((float)health * Config.StoneFenceLife);
                    fences.Data[StardewValley.Fence.stoneFenceId].Health = health;

                    health = fences.Data[StardewValley.Fence.ironFenceId].Health;
                    health = (int)((float)health * Config.IronFenceLife);
                    fences.Data[StardewValley.Fence.ironFenceId].Health = health;

                    health = fences.Data[StardewValley.Fence.gateId].Health;
                    health = (int)((float)health * Config.GateLife);
                    fences.Data[StardewValley.Fence.gateId].Health = health;

                    health = fences.Data[StardewValley.Fence.hardwoodFenceId].Health;
                    health = (int)((float)health * Config.HardwoodFenceLife);
                    fences.Data[StardewValley.Fence.hardwoodFenceId].Health = health;
                }
                );
            };
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
                                    f.minutesElapsed(1440);//one day by fence.minutesElaspsed logic.
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

    //[HarmonyPatch(typeof(StardewValley.Fence))]
    //public class FencePatches
    //{
    //    [HarmonyPostfix]
    //    [HarmonyPatch(nameof(StardewValley.Fence.ResetHealth))]
    //    [HarmonyPatch(new Type[] { typeof(float) })]
    //    public static void ResetHealth_Postfix(StardewValley.Fence __instance, float amount_adjustment)
    //    {
    //        try
    //        {
    //            int idx = ModEntry.GetFenceIdx(__instance);
    //            float mult = idx switch
    //            {
    //                ModEntry.WoodFenceId => ModEntry.Config.WoodFenceLife,
    //                ModEntry.StoneFenceId => ModEntry.Config.StoneFenceLife,
    //                ModEntry.IronFenceId => ModEntry.Config.IronFenceLife,
    //                ModEntry.GateId => ModEntry.Config.GateLife,
    //                ModEntry.HwFenceId => ModEntry.Config.HardwoodFenceLife,
    //                _ => 1.0f,
    //            };
    //            float before = __instance.health.Value;
    //            float beforeMax = __instance.maxHealth.Value;

    //            __instance.health.Value = before * mult;
    //            __instance.maxHealth.Value = beforeMax * mult;

    //            if (ModEntry.Debug)
    //                ModEntry.Log.Debug($"ResetHealth_PostFix: health after={__instance.health.Value}, health before={before}, fence_type={idx}");
    //        }
    //        catch (Exception ex)
    //        {
    //            ModEntry.Log.Error($"Failed in {nameof(ResetHealth_Postfix)}:\n{ex}");
    //        }
    //    }
    //}
}

