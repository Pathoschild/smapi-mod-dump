/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;

namespace StabbingSwordSpecial
{
    /// <summary>The mod entry point.</summary>
    public class StabbingSwordEntry : Mod
    {
        public MeleeWeapon cachedWeapon;
        public StabbingSwordWeapon stabWeapon;
        private StabbingSwordConfig Config;
        private int replacementIndex = 0;
        public double cachedCooldown = 0;
        public double defenseCooldown = 0;
        public double cooldownRatio = 1;
        public bool slowCooldown = false;
        private bool firstTickMeansSpecialFailed = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            Config = helper.ReadConfig<StabbingSwordConfig>();
            cooldownRatio = MeleeWeapon.defenseCooldownTime / (Config.BaseCooldownOfSpecial * 1000);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                e.Button.IsActionButton() &&
                Game1.activeClickableMenu == null &&
                Game1.player.CurrentTool != null &&
                Game1.player.CurrentTool.GetType().Equals(typeof(MeleeWeapon)) &&
                Game1.player.CanMove
                )
            {
                MeleeWeapon activeWeapon = (MeleeWeapon)Game1.player.CurrentTool;
                if (Helper.Reflection.GetMethod(activeWeapon, "specialCooldown").Invoke<int>() <= 0 && !activeWeapon.isScythe() && activeWeapon.type == 3 || activeWeapon.type == 0)
                {
                    Monitor.Log($"Intercepting special move!", LogLevel.Trace);
                    if (Config.FaceCursorForSpecial)
                        Game1.player.faceGeneralDirection(e.Cursor.AbsolutePixels);
                    cachedWeapon = activeWeapon;
                    if (MeleeWeapon.daggerCooldown > 0)
                        cachedCooldown = MeleeWeapon.daggerCooldown;
                    MeleeWeapon.daggerCooldown = 0;
                    defenseCooldown = Config.BaseCooldownOfSpecial * 1000;
                    if (Game1.player.professions.Contains(28))
                    {
                        defenseCooldown /= 2;
                    }
                    if (cachedWeapon.hasEnchantmentOfType<ArtfulEnchantment>())
                    {
                        defenseCooldown /= 2;
                    }
                    stabWeapon = new StabbingSwordWeapon(activeWeapon, Config.FlurryDamageMult);
                    replacementIndex = Game1.player.CurrentToolIndex;
                    Game1.player.removeItemFromInventory(replacementIndex);
                    Game1.player.addItemToInventory(stabWeapon, replacementIndex);
                    firstTickMeansSpecialFailed = true;
                    Monitor.Log($"Replaced MeleeWeapon with StabbingSwordWeapon", LogLevel.Trace);
                }
            }
        }

        /// <summary>
        /// Tracks internal special coodowns and handles switching back of weapons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.shouldTimePass())
            {
                if (cachedCooldown > 0)
                    cachedCooldown -= (1000 / 60);
                if (defenseCooldown > 0)
                    defenseCooldown -= (1000 / 60);
                if (slowCooldown)
                {
                    if (MeleeWeapon.defenseCooldown > 0)
                    {
                        MeleeWeapon.defenseCooldown = (int)(defenseCooldown * cooldownRatio);
                    }
                    else
                        slowCooldown = false;
                }
                //if (e.IsMultipleOf(10))
                //    Monitor.Log($"dagCD: {MeleeWeapon.daggerCooldown} ({cachedCooldown}): defCD: {MeleeWeapon.defenseCooldown} ({defenseCooldown})", LogLevel.Debug);
            }
            if (cachedWeapon != null && Game1.player.CurrentTool == stabWeapon)
            {
                if (!stabWeapon.isOnSpecial)
                {
                    MeleeWeapon.daggerCooldown = (int)cachedCooldown;
                    if (!firstTickMeansSpecialFailed)
                    {
                        MeleeWeapon.defenseCooldown = (int)(defenseCooldown * cooldownRatio);
                        slowCooldown = true;
                    }
                    Monitor.Log($"Weapon swapped back: special execution = {slowCooldown}", LogLevel.Trace);
                    Game1.player.removeItemFromInventory(replacementIndex);
                    Game1.player.addItemToInventory(cachedWeapon, replacementIndex);
                    cachedWeapon = null;
                    stabWeapon = null;
                }
                firstTickMeansSpecialFailed = false;
            }
        }
    }
}