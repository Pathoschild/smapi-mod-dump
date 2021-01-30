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
    class GiveStamina : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amt")]
        public int Amount;


        public override ActionResponse Handle()
        {
            BNC_Core.Logger.Log($"Giving Stamina: {Amount}", StardewModdingAPI.LogLevel.Debug);
            
            if (Game1.player.stamina + Amount > Game1.player.maxStamina)
                Game1.player.stamina = Game1.player.maxStamina;
            else
                Game1.player.stamina += Amount;

            return ActionResponse.Done;
        }
    }
}
