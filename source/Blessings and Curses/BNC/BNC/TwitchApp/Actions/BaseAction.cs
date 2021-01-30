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
using Newtonsoft.Json;


namespace BNC
{
    public abstract class BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "sender")]
        public String from;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "reciever")]
        public String reciever;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "channel")]
        public String channel;

        public DateTime? TryAfter = null;

        protected void TryLater(TimeSpan time)
        {
            TryAfter = DateTime.Now + time;
        }

        public abstract TwitchApp.ActionResponse Handle();
    }
}
