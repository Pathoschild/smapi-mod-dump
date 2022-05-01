/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Linq;
using BetterReturnScepter.HarmonyPatches;
using BetterReturnScepter.Helpers;
using BetterReturnScepter.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace BetterReturnScepter
{
    public class ModEntry : Mod
    {
        private Logger logger = null!;
        private WandPatches patches = null!;
        private ModConfig config = null!;

        private PreviousPoint previousPoint = new PreviousPoint();
        private RodCooldown rodCooldown = null!;
        private bool multiObeliskModInstalled;

        public override void Entry(IModHelper helper)
        {
            logger = new Logger(Monitor);
            rodCooldown = new RodCooldown();
            patches = new WandPatches(logger, previousPoint, rodCooldown);
            config = helper.ReadConfig<ModConfig>();

            Harmony harmony = new Harmony(ModManifest.UniqueID);

            // This is where we check for the return sceptre being used.
            harmony.Patch(
                original: AccessTools.Method(typeof(Wand), nameof(Wand.DoFunction)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches.WandPatches), nameof(HarmonyPatches.WandPatches.Wand_DoFunction_Prefix)));

            logger.Log($"Wand.DoFunction patched with prefix {nameof(WandPatches.Wand_DoFunction_Prefix)}.");

            // This is where we'll register with GMCM.
            helper.Events.GameLoop.GameLaunched += (sender, args) =>
            {
                multiObeliskModInstalled = this.Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks");
                RegisterWithGmcm();
            };

            // This is where we'll handle all of our input.
            helper.Events.Input.ButtonsChanged += ButtonsChanged;

            // On day start, we'll reset the previous point to wherever the player spawns, in their bed.
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            // On every tick, we'll increment the wand usage countdown timer.
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            // Every tick, we increment our timer.
            rodCooldown.IncrementTimer();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            // On the start of the day, we set our previous locations to the player's spawn point.
            previousPoint.Location = Game1.player.currentLocation;
            previousPoint.Tile = Game1.player.getTileLocation();
        }

        private void ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // No world, we return.
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu != null)
                return;

            // If an event is up, return, because we don't want to accidentally warp out of an event.
            // This shouldn't be possible anyway, but I'm keeping this here for safety.
            if (Game1.eventUp)
                return;

            Farmer player = Game1.player;

            // If the return sceptre cooldown is over...
            if (rodCooldown.CanWarp)
            {
                // And if the player is holding a return scepter...
                if (player.CurrentItem is Wand && player.CurrentItem.Name.Equals("Return Scepter"))
                {
                    // The cooldown is over, and the player is holding a return sceptre. Now we check to see 
                    // whether they press the return to previous point bind, or the Multiple Mini-Obelisk menu bind.
                    if (e.Pressed.Contains(SButton.MouseRight) || config.ReturnToLastPoint.JustPressed())
                    {
                        // The player pressed the return to previous point bind.
                        if (!player.bathingClothes.Value && player.IsLocalPlayer && !player.onBridge.Value)
                        {
                            logger.Log($"Warping farmer {player.Name} to {previousPoint.Location.Name} on tile {previousPoint.Tile}", LogLevel.Trace);

                            // We reset our countdown timer and status to zero, because we want it to start from when the player last did a normal warp.
                            rodCooldown.ResetCountdown();

                            #region Modified decompile section

                            // Shamelessly taken from the decompile and modified slightly. Sorry, CA!

                            for (int i = 0; i < 12; i++)
                            {
                                TemporaryAnimatedSprite zoomyDust = new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)player.position.X - 256, (int)player.position.X + 192), Game1.random.Next((int)player.position.Y - 256, (int)player.position.Y + 192)), flicker: false, (Game1.random.NextDouble() < 0.5));
                                Game1.currentLocation.TemporarySprites.Add(zoomyDust);
                            }

                            Game1.currentLocation.playSound("wand");

                            Game1.displayFarmer = false;
                            player.temporarilyInvincible = true;
                            player.temporaryInvincibilityTimer = -2000;
                            player.Halt();
                            player.faceDirection(2);
                            player.CanMove = false;
                            player.freezePause = 2000;
                            Game1.flashAlpha = 1f;
                            DelayedAction.fadeAfterDelay(DoWarp, 1000);

                            int nextTileDelay = 0;

                            for (int xTile = player.getTileX() + 8; xTile >= player.getTileX() - 8; xTile--)
                            {
                                TemporaryAnimatedSprite flash = new TemporaryAnimatedSprite(6, new Vector2(xTile, player.getTileY()) * 64f, Color.White, 8, flipped: false, 50f)
                                {
                                    layerDepth = 1f,
                                    delayBeforeAnimationStart = nextTileDelay * 25,
                                    motion = new Vector2(-0.25f, 0f)
                                };

                                Game1.currentLocation.TemporarySprites.Add(flash);
                                nextTileDelay++;
                            }

                            #endregion

                        }
                    }
                    else if (config.EnableMultiObeliskSupport)
                    {
                        // Support is enabled...
                        if (config.OpenObeliskWarpMenuController.JustPressed() || config.OpenObeliskWarpMenuKbm.JustPressed())
                        {
                            // The button was pressed, so we want to check to see if the mod is installed.
                            if (multiObeliskModInstalled)
                            {
                                // The mod is installed, so we can continue.
                                if (config.CountWarpMenuAsScepterUsage)
                                {
                                    // The setting to count warp menu usage as scepter usage is enabled, so we set our previous point.
                                    previousPoint.Location = player.currentLocation;
                                    previousPoint.Tile = player.getTileLocation();
                                }

                                // Reset our cooldown.
                                rodCooldown.ResetCountdown();

                                // And create a temporary, dummy mini-obelisk, and "interact" with it to spawn the warp menu.
                                SObject dummyObelisk = new SObject();
                                dummyObelisk.ParentSheetIndex = 238;
                                dummyObelisk.checkForAction(Game1.player);
                            }
                            else
                            {
                                Game1.showRedMessage("Multiple Mini-Obelisks support is enabled, but the mod itself isn't installed.");
                            }
                        }
                    }
                }
            }
        }

        private void DoWarp()
        {
            // Cache our player for tidiness.
            Farmer player = Game1.player;

            // Start the warp to the previous location.
            Game1.warpFarmer(previousPoint.Location.Name, (int)previousPoint.Tile.X, (int)previousPoint.Tile.Y, 0, false);

            // This is all taken from the game's code in order to replicate the way the regular warp works.
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            player.temporarilyInvincible = false;
            player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
            player.CanMove = true;
        }

        private void RegisterWithGmcm()
        {
            // Get our API reference.
            IGenericModConfigMenuApi configMenuApi =
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            // If this is null, GMCM wasn't installed.
            if (configMenuApi == null)
            {
                logger.Log("The user doesn't have GMCM installed. This is not an error.");

                return;
            }

            // Register with GMCM
            configMenuApi.Register(ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config));

            // The toggle for whether or not we want Multiple Mini-Obelisks support.
            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => "Multiple Mini-Obelisks mod support",
                tooltip: () => "If the Multiple Mini-Obelisks mod is installed, the keybind will work when the scepter is selected to open the warp menu.",
                getValue: () => config.EnableMultiObeliskSupport,
                setValue: value => config.EnableMultiObeliskSupport = value);

            // Whether or not to count using MMO's warp menu as using the return scepter.
            configMenuApi.AddBoolOption(
                mod: ModManifest,
                name: () => "Count warp menu as using scepter",
                tooltip: () => "Whether or not you want to be able to warp back to where you last used the scepter's integration with Multiple Mini-Obelisks's warp menu.",
                getValue: () => config.CountWarpMenuAsScepterUsage,
                setValue: value => config.CountWarpMenuAsScepterUsage = value);

            // Add a nice title for prettiness.
            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => "Keybinds");

            // The bind for opening MMO's warp menu.
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => "Open warp menu",
                getValue: () => config.OpenObeliskWarpMenuKbm,
                setValue: value => config.OpenObeliskWarpMenuKbm = value);

            // Add a nice title for prettiness.
            configMenuApi.AddSectionTitle(
                mod: ModManifest,
                text: () => "Controller Keybinds");

            // The controller bind for warping to our last point.
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => "Warp to last point",
                getValue: () => config.ReturnToLastPoint,
                setValue: value => config.ReturnToLastPoint = value);

            // The controller bind for opening MMO's warp menu.
            configMenuApi.AddKeybindList(
                mod: ModManifest,
                name: () => "Open warp menu",
                getValue: () => config.OpenObeliskWarpMenuController,
                setValue: value => config.OpenObeliskWarpMenuController = value);
        }
    }
}