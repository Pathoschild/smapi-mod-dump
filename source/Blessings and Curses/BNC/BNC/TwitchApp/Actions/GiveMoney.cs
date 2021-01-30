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
    class GiveMoney : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amt")]
        public int moneyAmt;


        public override ActionResponse Handle()
        {
            BNC_Core.Logger.Log($"Actiong Give Player Money: {moneyAmt}", StardewModdingAPI.LogLevel.Debug);

            if (Game1.player.Money + moneyAmt < 0)
                Game1.player.Money = 0;
            else
                Game1.player.Money += moneyAmt;

            return ActionResponse.Done;
        }
    }
}
