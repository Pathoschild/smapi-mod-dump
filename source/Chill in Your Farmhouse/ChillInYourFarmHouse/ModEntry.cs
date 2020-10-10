/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Husky110/ChillInYourFarmHouse
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ChillInYourFarmHouse
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private string _location = "";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            
        }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Called when a player changes maps
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event args that contain information about the previous and new maps.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                _location = e.NewLocation.Name;
            }
        }

        /// <summary>
        /// Called when the time changes
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event args that contain information related to the time change.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {

            Farmer player = Game1.player;

            bool allowAccess = _location == "FarmHouse" || _location == "Cabin";

            if (Context.IsWorldReady == false || allowAccess == false || player.Stamina >= player.MaxStamina)
                return;
       
            
            float currentPercentage = player.Stamina / player.MaxStamina * 100;
            float multiplicator = currentPercentage < 20 || currentPercentage >= 80 ? 0.05f : 0.1f;

            int staminaToGive = (int)Math.Round(player.MaxStamina * multiplicator, MidpointRounding.AwayFromZero);

            if((player.Stamina + staminaToGive) > player.MaxStamina)
                staminaToGive = (int)Math.Floor(player.MaxStamina - player.stamina);

            player.Stamina += staminaToGive;

        }


    }
}