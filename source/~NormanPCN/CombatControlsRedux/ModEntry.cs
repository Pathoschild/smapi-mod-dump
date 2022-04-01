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
using System.Collections.Generic;
//using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using GenericModConfigMenu;

/*
    This is an adaption of the work of Dj-Stalin (DJ-STLN), who created the original Combat Controls Mod.
    That mod is available at https://www.nexusmods.com/stardewvalley/mods/2590
    As source was not available, the original Mod was decompiled.
    The NexusMods page lists permission as
    "You are allowed to modify my files and release bug fixes or improve on the features so long as you credit me as the original creator"

    This source changes most things from the original source but the core functions of the original Mod are unchanged. "how it works".
        the facing direction change logic.
        the slick moves velocity tweaks logic.

    Additions/changes to the original Mod
        implemented config file support.
          added Generic Mod Config menu support with i18n internationalization support.
        facing direction change (MouseFix) on melee weapons/tools only. (left-click, use tool button, controllers).
           config to disable the direction fix for the controllers.
        Split screen support 
        facing direction change for dagger special attack. (right-click, action button, controllers).
        separate control for slick moves on sword and club.
        auto swing with separate control for sword/club and dagger.
        slick moves disabled for daggers and scythe. there were issues here anyway.
        added slick move velocity config settings. separate velocity for special and not special slides.
        

    Combat Controls Redux is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License  
    as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

    The Combat Controls Redux mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  
    without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
    See the GNU General Public License for more details. <http://www.gnu.org/licenses/>

    I don't really care if you redistribute it or alter it or use it in compilations.  
    I'd ask that you give credit to myself (Norman Black, NormanPCN) and (Dj-Stalin, DJ-STLN), that's all.  
 */

namespace CombatControlsRedux
{
    public class ModEntry : Mod
    {
        private class PerScreenData
        {
            public int TickCountdown = 0;
            public bool IsHoldingAttack = false;
            public int MyFacingDirection = -1;
        }

        public ModConfig Config;

        internal IModHelper MyHelper;
        //private IReflectedMethod PerformFireTool;


        private const int CountdownStart = 6;
        private const int CountdownRepeat = 1;
        private readonly PerScreen<PerScreenData> ScreenData = new(createNewState: () => new PerScreenData());

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;

            // original Mod: all config load and event hooking was done on entry. no removal.
            // entry config loads don't work when using GMCM optional. GMCM may not have been loaded yet.
            // added OnGameLaunched for config setup/load.
            // added OnSaveLoaded to insert input/update event hooks
            // added OnReturnedToTitle to remove the input/update event hooks.
            //     probably not necessary. i.e. WorldIsReady/PlayerIsFree. i just don't want events if a game is not running.
            // added OnUpdateTicked for facing direction change correction.
            

            MyHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            MyHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            MyHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        internal String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>Raised after the game has loaded and all Mods are loaded. Here we load the config.json file and setup GMCM </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            const float minSlideVelocity = 2.0f;
            const float maxSlideVelocity = 10.0f;

            Config = MyHelper.ReadConfig<ModConfig>();

            // lets clamp the range of these values in case someone editing the config gives a value that may cause problems.

            Config.SlideVelocity = Math.Min(maxSlideVelocity, Math.Max(minSlideVelocity, Config.SlideVelocity));
            Config.SpecialSlideVelocity = Math.Min(maxSlideVelocity, Math.Max(minSlideVelocity, Config.SpecialSlideVelocity));

            // use GMCM in an optional manner.

            //IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var gmcm = Helper.ModRegistry.GetGenericModConfigMenuApi(this.Monitor);
            if (gmcm != null)
            {
                gmcm.Register(ModManifest,
                              reset: () => Config = new ModConfig(),
                              save: () => Helper.WriteConfig(Config));

                gmcm.AddBoolOption(ModManifest,
                                   () => Config.MouseFix,
                                   (bool value) => Config.MouseFix = value,
                                   () => I18nGet("mouseFix.Label"),
                                   () => I18nGet("mouseFix.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.ControllerFix,
                                   (bool value) => Config.ControllerFix = value,
                                   () => I18nGet("controllerFix.Label"),
                                   () => I18nGet("controllerFix.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.RegularToolsFix,
                                   (bool value) => Config.RegularToolsFix = value,
                                   () => I18nGet("regularToolsFix.Label"),
                                   () => I18nGet("regularToolsFix.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.AutoSwing,
                                   (bool value) => Config.AutoSwing = value,
                                   () => I18nGet("autoSwing.Label"),
                                   () => I18nGet("autoSwing.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.AutoSwingDagger,
                                   (bool value) => Config.AutoSwingDagger = value,
                                   () => I18nGet("autoSwingDagger.Label"),
                                   () => I18nGet("autoSwingDagger.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.SlickMoves,
                                   (bool value) => Config.SlickMoves = value,
                                   () => I18nGet("slickMoves.Label"),
                                   () => I18nGet("slickMoves.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.SwordSpecialSlickMove,
                                   (bool value) => Config.SwordSpecialSlickMove = value,
                                   () => I18nGet("swordSpecial.Label"),
                                   () => I18nGet("swordSpecial.tooltip"));
                gmcm.AddBoolOption(ModManifest,
                                   () => Config.ClubSpecialSlickMove,
                                   (bool value) => Config.ClubSpecialSlickMove = value,
                                   () => I18nGet("clubSpecial.Label"),
                                   () => I18nGet("clubSpecial.tooltip"));

                gmcm.AddNumberOption(ModManifest,
                                     () => Config.SlideVelocity,
                                     (float value) => Config.SlideVelocity = value,
                                     () => I18nGet("slideVelocity.Label"),
                                     () => I18nGet("slideVelocity.tooltip"),
                                     min: minSlideVelocity,
                                     max: maxSlideVelocity,
                                     interval: 0.1f);
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.SpecialSlideVelocity,
                                     (float value) => Config.SpecialSlideVelocity = value,
                                     () => I18nGet("specSlideVelocity.Label"),
                                     () => I18nGet("specSlideVelocity.tooltip"),
                                     min: minSlideVelocity,
                                     max: maxSlideVelocity,
                                     interval: 0.1f);

                //gmcm.SetTitleScreenOnlyForNextOptions(ModManifest, true);
                //gmcm.AddBoolOption(ModManifest,
                //                   () => Config.NearTileFacingFix,
                //                   (bool value) => Config.NearTileFacingFix = value,
                //                   () => "Near tile facing fix",
                //                   () => "Near tile facing fix");
            }
            else
            {
                Monitor.LogOnce("Generic Mod Config Menu not available.", LogLevel.Info);
            };
        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            PerScreenData screen = ScreenData.Value;
            screen.TickCountdown = 0;
            screen.IsHoldingAttack = false;
            screen.MyFacingDirection = -1;

            //PerformFireTool = MyHelper.Reflection.GetMethod(Game1.player, "performFireTool");

            MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased += Input_ButtonReleased;
            MyHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            if (Config.NearTileFacingFix)
                MyHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            //PerformFireTool = null;

            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            MyHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            if (Config.NearTileFacingFix)
                MyHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (SButtonExtensions.IsUseToolButton(e.Button))
            {
                PerScreenData screen = ScreenData.Value;
                screen.IsHoldingAttack = false;
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.
        /// This method implements the facing direction change and Slick moves of the Mod.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Farmer who = Game1.player;
            bool useToolButtonPressed = SButtonExtensions.IsUseToolButton(e.Button);
            bool actionButtonPressed = SButtonExtensions.IsActionButton(e.Button);

            if (
                (who.CurrentTool != null) &&
                (useToolButtonPressed || actionButtonPressed) &&
                Context.IsPlayerFree
               )
            {
                PerScreenData screen = ScreenData.Value;

                // note: the scythe identifies itself as a melee weapon
                MeleeWeapon tool = who.CurrentTool as MeleeWeapon;

                if (useToolButtonPressed && (tool != null))
                {
                    screen.IsHoldingAttack = true;
                    screen.TickCountdown = CountdownStart;
                }

                bool controller = (e.Button == SButton.ControllerX) || (e.Button == SButton.ControllerA);
                if (
                    ((Config.MouseFix && !controller) || (controller && Config.ControllerFix)) &&
                    ((tool != null) || Config.RegularToolsFix)
                   )
                {
                    bool scythe = tool?.isScythe() == true;
                    bool dagger = (tool?.type.Value == MeleeWeapon.dagger);
                    bool special = tool?.isOnSpecial == true;
                    bool swordSpecial = special && (tool?.type.Value == MeleeWeapon.defenseSword);
                    bool clubSpecial = special && (tool?.type.Value == MeleeWeapon.club);

                    if (
                        (tool != null) &&
                        useToolButtonPressed &&
                        (!dagger) &&
                        (!scythe) &&
                        (
                          ((!special) && Config.SlickMoves) ||
                          (swordSpecial && Config.SwordSpecialSlickMove) ||
                          (clubSpecial && Config.ClubSpecialSlickMove)
                        )
                       )
                    {
                        float newVelocity = (special ? Config.SpecialSlideVelocity : Config.SlideVelocity);

                        // diagonal movement returns an up/down/left/right.
                        // for now limit the velocity change to the cardinal directions. Count = 1
                        // it still seems to work okay on the diagonal, with single velocity adjustment.
                        // so maybe not limit

                        //Monitor.Log($".movementDirections.Count={who.movementDirections.Count}", LogLevel.Debug);
                        if (who.movementDirections.Count == 1)
                        {
                            //Monitor.Log($".xV={who.xVelocity}, .yV={who.yVelocity}", LogLevel.Debug);
                            switch (who.getDirection())
                            {
                            case Game1.left:
                                who.canMove = true;
                                who.xVelocity = 0f - newVelocity;
                                break;
                            case Game1.right:
                                who.canMove = true;
                                who.xVelocity = newVelocity;
                                break;
                            case Game1.up:
                                who.canMove = true;
                                who.yVelocity = newVelocity;
                                break;
                            case Game1.down:
                                who.canMove = true;
                                who.yVelocity = 0f - newVelocity;
                                break;
                            default:
                                break;
                            }
                        }
                    }

                    // change the player facing direction.

                    if (useToolButtonPressed || (dagger && actionButtonPressed))
                    {

                        // .Cursor.AbsolutePixels are map relative coords
                        //  who.GetBoundingBox().Center.X/Y, who.Position.X/Y
                        //  should I use BoundingBox center?
                        //Log.Debug($"Pos.X,Y {(int) who.Position.X},{(int) who.Position.Y} " +
                        //                $"Center.X,Y {(int) who.GetBoundingBox().Center.X},{(int) who.GetBoundingBox().Center.Y} " +
                        //                $"Cursor.X,Y {(int) e.Cursor.AbsolutePixels.X},{(int) e.Cursor.AbsolutePixels.Y}");
                        //float mouseDirectionX = e.Cursor.AbsolutePixels.X - who.Position.X;
                        //float mouseDirectionY = e.Cursor.AbsolutePixels.Y - who.Position.Y;
                        Microsoft.Xna.Framework.Point pos = who.GetBoundingBox().Center;
                        float mouseDirectionX = e.Cursor.AbsolutePixels.X - pos.X;
                        float mouseDirectionY = e.Cursor.AbsolutePixels.Y - pos.Y;
                        float mouseDirectionXpower = mouseDirectionX * mouseDirectionX;
                        float mouseDirectionYpower = mouseDirectionY * mouseDirectionY;

                        if (mouseDirectionXpower > mouseDirectionYpower)
                        {
                            if (mouseDirectionX < 0f)
                            {
                                who.FacingDirection = Game1.left;
                                screen.MyFacingDirection = Game1.left;
                            }
                            else
                            {
                                who.FacingDirection = Game1.right;
                                screen.MyFacingDirection = Game1.right;
                            }
                        }
                        else if (mouseDirectionY < 0f)
                        {
                            who.FacingDirection = Game1.up;
                            screen.MyFacingDirection = Game1.up;
                        }
                        else
                        {
                            who.FacingDirection = Game1.down;
                            screen.MyFacingDirection = Game1.down;
                        }
                    }
                }
            }
        }

        /// <summary>Raised when the game state is about to be updated (≈60 times per second).
        /// This method implements the Auto Swing feature of the mod.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicking(object sender, EventArgs e)
        {
            // Test IsHoldingAttack first. It is the main logic restricter here.

            PerScreenData screen = ScreenData.Value;

            if (screen.IsHoldingAttack && Context.IsPlayerFree)
            {
                Farmer who = Game1.player;

                // this auto swing code does not work for the Scythe.
                // I am good with that. I explicitly disable the scythe anyway.

                if (who.CurrentTool is MeleeWeapon tool)
                {
                    bool dagger = (tool.type.Value == MeleeWeapon.dagger);
                    if ((!tool.isScythe()) && ((Config.AutoSwing && !dagger) || (Config.AutoSwingDagger && dagger)))
                    {
                        // spamming FireTool at every tick (60/s) seems excessive. at least to me.
                        // it seems to work with spams. i don't know the exact overhead.
                        // i'll reduce to every N ticks. N must be small.
                        // too big a number and auto swing just does not work at all.
                        // the next fire may need to be set during a current fire/swing/something.
                        // even a little reduction seems somehow "nicer". what the heck.
                        // update: fast weapons + lots of speed buffs need a real repeat short delay. maybe dump the repeat timing.
                        if (screen.TickCountdown > 0)
                        {
                            screen.TickCountdown -= 1;
                        }
                        else
                        {
                            // which is "better" FireTool or (private internal) PerformFireTool
                            // Looking at the Stardew code, PerformFireTool seems to be the implementation of NetEvent.Fire.
                            // Farmer.FireTool is just a call to NetEvent.Fire
                            // Fire clearly has a bit over minor checking/overhead before calling the implementation.
                            // code that we should probably have execute.
                            // i've seen some auto swing code use performFireTool. so I wondered the diff.

                            who.FireTool();
                            //PerformFireTool.Invoke();
                            screen.TickCountdown = CountdownRepeat;
                        }
                    }
                }
            }
        }

        /// <summary>Raised just after the game state is updated (≈60 times per second).
        /// This method implements facing direction change correction.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicked(object sender, EventArgs e)
        {
            PerScreenData screen = ScreenData.Value;
            int facing = screen.MyFacingDirection;
            if ((facing >= 0) && (facing != Game1.player.FacingDirection))
            {
                screen.MyFacingDirection = -1;

                //the game changed the facing direction we selected. set it back.
                //this disagreement only happens in the 8 tiles surrounding the player where the game code may set the facing direction.
                //re-setting the direction here seems to work well enough. I've seen some animation quirks at times.
                //this re-change may be happening on the next game tick.
                                        
                //Monitor.Log($"FacingDirection different me={facing} game={Game1.player.FacingDirection}", LogLevel.Debug);
                Game1.player.FacingDirection = facing;
            }
        }
    }
}

