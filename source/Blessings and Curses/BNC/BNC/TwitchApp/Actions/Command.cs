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
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class Command : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "cmd")]
        public string cmd;


        public override ActionResponse Handle()
        {
            Game1.game1.parseDebugInput(cmd);

            return ActionResponse.Done;
        }
    }
}
