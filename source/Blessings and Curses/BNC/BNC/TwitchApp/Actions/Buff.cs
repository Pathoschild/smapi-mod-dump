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
using static BNC.BuffManager;

namespace BNC.Actions
{
    class Buff : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "buffType")]
        public string buffType;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "duration")]
        public int Duration;

        public override ActionResponse Handle()
        {
            BuffOption buff; 
      
            if(buffType == "random") {
                buff = BuffManager.getRandomBuff(1)[0];
            }
            else {
                buff = BuffManager.getIDtoBuff(buffType);
            }

            if(buff == null) {
                BNC_Core.Logger.Log($"Could not find buff type of: {buffType}", StardewModdingAPI.LogLevel.Error);
                return ActionResponse.Done;
            }
            else  {

                //If not Default
                if (Duration != 1200 && Duration > 0 && Duration < 5000)
                    buff.setDuration(Duration);

                BuffManager.AddBuffToQueue(buff);
                return ActionResponse.Done;
            }
        }
    }
}
