/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Binarynova/MyStardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley.Inventories;
using StardewValley.Menus;
using Microsoft.Xna.Framework;

namespace LevelUpNotifications
{
    public class ModEntry : Mod
    {
        // Using a custom counter to avoid duplicating the level up message on the first level up of every day.
        private int numOfLevelUpsToday = 0;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.LevelChanged += Player_LevelChanged;
            helper.Events.GameLoop.DayStarted += ResetLevelUpCounter;
        }
        
        private void Player_LevelChanged(object? sender, LevelChangedEventArgs e)
        {
            if(e.NewLevel > e.OldLevel)
            {
                if(numOfLevelUpsToday > 0)
                {
                    IDictionary<string, string> data = Helper.GameContent.Load<Dictionary<string, string>>("Strings/1_6_Strings");
                    string levelUpMessage = data["NewIdeas"];
                    Game1.addHUDMessage(HUDMessage.ForCornerTextbox(levelUpMessage));
                }

                numOfLevelUpsToday++;
            }
        }

        private void ResetLevelUpCounter(object? sender, EventArgs e)
        {
            // Reset counter every morning.
            numOfLevelUpsToday = 0;
        }
    }
}