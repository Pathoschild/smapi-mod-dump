/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using BNC.TwitchApp;
using Newtonsoft.Json;
using StardewValley;


namespace BNC.Actions
{
    class HealPlayer : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "heal")]
        public int HealthIn;


        public override ActionResponse Handle()
        {
            BNC_Core.Logger.Log($"Healing Player {HealthIn}", StardewModdingAPI.LogLevel.Debug);
            
            if (Game1.player.health + HealthIn > Game1.player.maxHealth)
                Game1.player.health = Game1.player.maxHealth;
            else
                Game1.player.health += HealthIn;

            return ActionResponse.Done;
        }
    }
}
