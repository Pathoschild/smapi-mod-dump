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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
//using StardewValley.Locations;
using GenericModConfigMenu;
using Helpers;


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
        added club ground slam spam attack.
        

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
            public int ClubSpamAttack = 0;
        }

        public ModConfig Config;

        internal IModHelper MyHelper;
        //private IReflectedMethod PerformFireTool;

        internal const int ClubSpamCountdown = 3;

        internal Logger Log;

        private readonly PerScreen<PerScreenData> ScreenData = new(createNewState: () => new PerScreenData());

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;

            Log = new Logger(Monitor);

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

            Config.CountdownStart = Math.Min(10, Math.Max(3, Config.CountdownStart));
            Config.CountdownRepeat = Math.Min(Config.CountdownStart, Math.Max(1, Config.CountdownRepeat));
            Config.CountdownFastDaggerOffset = Math.Min(Config.CountdownStart-1, Math.Max(1, Config.CountdownFastDaggerOffset));

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
                                   () => Config.MouseFix,
                                   (bool value) => Config.ClubSpecialSpamAttack = value,
                                   () => I18nGet("clubSpamAttack.Label"),
                                   () => I18nGet("clubSpamAttack.tooltip"));
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
                Log.LogOnce("Generic Mod Config Menu not available.");
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
            screen.ClubSpamAttack = 0;

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

        private static bool InOnScreenMenu(ICursorPosition cursor)
        {
            bool save = Game1.uiMode;
            Game1.uiMode = true;
            Vector2 v = cursor.GetScaledScreenPixels();
            Game1.uiMode = save;
            int x = (int)v.X;
            int y = (int)v.Y;
            for (int i = 0; i < Game1.onScreenMenus.Count; i++)
            {
                if (Game1.onScreenMenus[i].isWithinBounds(x, y))
                {
                    return true;
                }
            }
            return false;
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
                Context.IsPlayerFree &&
                (!who.swimming.Value) &&
                (!who.bathingClothes.Value) &&
                (! InOnScreenMenu(e.Cursor))
               )
            {
                PerScreenData screen = ScreenData.Value;

                // note: the scythe identifies itself as a melee weapon
                MeleeWeapon tool = who.CurrentTool as MeleeWeapon;

                bool controller = (e.Button == SButton.ControllerX) || (e.Button == SButton.ControllerA);
                bool scythe = false;
                bool dagger = false;
                bool club = false;
                bool special = false;
                bool swordSpecial = false;
                bool clubSpecial = false;
                if (tool != null)
                {
                    scythe = tool.isScythe();
                    dagger = tool.type.Value == MeleeWeapon.dagger;
                    club = tool.type.Value == MeleeWeapon.club;
                    //note: Game logic always returns true if no regular swing occurs after a special swing.
                    //      the boolean is not auto reset. only regular swing seems to reset the boolean.
                    //      is there a better way to detect an actual special?
                    special = tool.isOnSpecial;
                    swordSpecial = special && (tool.type.Value == MeleeWeapon.defenseSword);
                    clubSpecial = special && club;
                }

                if (useToolButtonPressed && (tool != null))
                {
                    screen.IsHoldingAttack = true;
                    screen.TickCountdown = Config.CountdownStart;

                    //Log.Debug($"Speed = {tool.speed.Value}");
                    if (dagger)
                    {
                        // fast daggers need a short initial delay. this is because their animation is so fast.
                        // if the user button press is a bit slow, they may get an unintended auto swing.
                        // this is not too bad since the dagger stab is so fast.
                        if (tool.speed.Value > 3) // >= +2 speed.
                        {
                            // fast daggers.
                            screen.TickCountdown -= Config.CountdownFastDaggerOffset;

                            if (tool.speed.Value > 5)
                            {
                                // really fast daggers.
                                screen.TickCountdown -= 1;
                            }
                            else if (tool.speed.Value > 7)
                            {
                                // crazy fast daggers.
                                screen.TickCountdown -= 1;
                            }

                            // speed buffs affect weapon speed.
                            if (who.addedSpeed != 0)
                            {
                                //Log.Debug($"SpeedBuff = {who.addedSpeed}");
                                screen.TickCountdown -= 1;
                            }

                            if (screen.TickCountdown <= 0)
                                screen.TickCountdown = 1;
                        }
                    }
                }

                if (
                    ((Config.MouseFix && !controller) || (controller && Config.ControllerFix)) &&
                    ((tool != null) || Config.RegularToolsFix) &&
                    ((who.CurrentTool is not FishingRod) || !(who.CurrentTool as FishingRod).isFishing)
                   )
                {

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

                        //Log.Debug($".movementDirections.Count={who.movementDirections.Count}");
                        if (who.movementDirections.Count == 1)
                        {
                            //Log.Debug($".xV={who.xVelocity}, .yV={who.yVelocity}");
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

                if (club && actionButtonPressed && !special)
                {
                    //Log.Debug("\n\n\nClub ground slam.");
                    screen.ClubSpamAttack = -1;
                }

                // i want to trigger this only when it is available.
                // i think this can only happen during the duration of the ground slam animation.
                if (clubSpecial && useToolButtonPressed && Config.ClubSpecialSpamAttack && (screen.ClubSpamAttack < 0))
                {
                    //Log.Debug("Club Spam attack setup");
                    screen.ClubSpamAttack = Config.ClubSpamCount;
                    screen.TickCountdown = ClubSpamCountdown;
                }

            }
        }

        /// <summary>Raised when the game state is about to be updated (≈60 times per second).
        /// This method implements the Auto Swing feature and ground slam spam attack.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicking(object sender, EventArgs e)
        {
            PerScreenData screen = ScreenData.Value;

            if (Context.IsPlayerFree)
            {
                Farmer who = Game1.player;

                if (who.CurrentTool is MeleeWeapon tool)
                {

                    if ((tool.type.Value == MeleeWeapon.club) && (screen.ClubSpamAttack > 0))
                    {
                        //Log.Debug($"UpdateTicking: countdown={screen.TickCountdown}, isOnSpecial={tool.isOnSpecial}, spamAttack={screen.ClubSpamAttack}");

                        if (tool.isOnSpecial)
                        {
                            if (screen.TickCountdown > 0)
                            {
                                screen.TickCountdown -= 1;
                            }
                            else
                            {
                                screen.ClubSpamAttack -= 1;
                                screen.TickCountdown = ClubSpamCountdown;

                                bool ok = Game1.pressUseToolButton();
                                //tool.leftClick(who);
                            }
                        }
                        else
                        {
                            screen.ClubSpamAttack = 0;
                        }
                    }
                    else if (screen.IsHoldingAttack)
                    {
                        // this auto swing code does not work for the Scythe. I am good with that.

                        bool dagger = (tool.type.Value == MeleeWeapon.dagger);
                        if ((Config.AutoSwing && !dagger) || (Config.AutoSwingDagger && dagger))
                        {
                            // we have an initial longer delay to avoid a slow finger press/click causing an unintended auto swing.
                            // that extra swing could be slow.
                            // repeat...
                            // spamming FireTool at every tick seems excessive (60/s). at least to me.
                            // FireTool seems to work with spams fine. i don't know the exact overhead.
                            // i'll reduce to every N ticks. N must be small.
                            // the next fire seems to need to be set during a current fire/swing animation.
                            // even a little spam reduction seems somehow "nicer". what the heck.
                            if (screen.TickCountdown > 0)
                            {
                                screen.TickCountdown -= 1;
                            }
                            else
                            {
                                // which is "better". FireTool or (private internal) PerformFireTool
                                // Looking at the Stardew code, PerformFireTool seems to be the implementation of NetEvent.Fire.
                                // Farmer.FireTool is just a call to NetEvent.Fire
                                // Fire clearly has a bit over minor checking/overhead before calling the implementation.
                                // code that we should probably have execute.
                                // i've seen some auto swing code use performFireTool. so I wonder about the diff.

                                who.FireTool();
                                //PerformFireTool.Invoke();
                                screen.TickCountdown = Config.CountdownRepeat;
                            }
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
                                        
                //Log.Debug($"FacingDirection different me={facing} game={Game1.player.FacingDirection}");
                Game1.player.FacingDirection = facing;
            }
        }
    }
}

