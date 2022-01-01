/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/shailalias/NoRumbleHorse
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Locations;
using static StardewValley.Farmer;
using static StardewValley.Characters.Horse;
using static StardewValley.Rumble;
using static StardewValley.Options;

namespace NoRumbleHorse
{

    public class ModEntry : Mod
    {
        private Horse thisHorse;
        // private const bool rumbleOption = true; 
        private bool rumbleOption = Game1.options.rumble;

        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            PlayerEvents.Warped += this.PlayerEvents_Warped;
        }


        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.player.isRidingHorse() == true)
            {
                Game1.options.rumble = false;
            }
            else
            {
                Game1.options.rumble = rumbleOption;
                
            }
        }
        private void PlayerEvents_Warped(object sender, EventArgs e)
        {

            this.thisHorse = (Horse)null;
            foreach (NPC character in Game1.currentLocation.getCharacters())
            {
                if (character is Horse)
                {
                    this.thisHorse = (Horse)character;
                    break;
                }
            }
        }

 
    }
}