/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RetroactiveStardew
{
    public class RetroMail
    {
        private IModHelper helper;
        private IMonitor Monitor;

        private ModConfig Config { get; }

        public RetroMail(IModHelper helper, IMonitor monitor, ModConfig Config)
        {
            this.helper = helper;
            this.Monitor = monitor;
            this.Config = Config;
        }

        public void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!this.Config.RetroMail) return;

            this.CheckIfThereIsRecipesAvailableFromFriendship();
        }

        private void CheckIfThereIsRecipesAvailableFromFriendship()
        {
            Dictionary<string, string> cookingRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            foreach (string s in cookingRecipes.Keys)
            {
                string[] getConditions = cookingRecipes[s].Split('/')[3].Split(' ');
                if (getConditions[0].Equals("f") && Game1.player.friendshipData.ContainsKey(getConditions[1]) && Game1.player.friendshipData[getConditions[1]].Points >= Convert.ToInt32(getConditions[2]) * 250 && !Game1.player.cookingRecipes.ContainsKey(s) && !Game1.player.hasOrWillReceiveMail(getConditions[1] + "Cooking"))
                {
                    this.Monitor.Log($"[RetroMail] {getConditions[1]} added to mail :D .", LogLevel.Info);
                    Game1.addMailForTomorrow(getConditions[1] + "Cooking");
                }
            }
        }
    }
}