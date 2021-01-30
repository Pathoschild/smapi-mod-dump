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
using System.Collections.Generic;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class Gift : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "item_id")]
        public int item;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "cnt")]
        public int cnt;


        public override ActionResponse Handle()
        {
           int tries = 0;
           BNC_Core.Logger.Log("Try Gifting", StardewModdingAPI.LogLevel.Debug);

            if (Game1.player.currentLocation is FarmHouse)
                return ActionResponse.Retry;

            Vector2 pos = Utility.getRandomAdjacentOpenTile(Game1.player.getTileLocation(), Game1.player.currentLocation);
            while (pos == Vector2.Zero && tries++ < 6){
                BNC_Core.Logger.Log($"Trying... {tries} {pos.X}, {pos.Y}", StardewModdingAPI.LogLevel.Debug);
                pos = Utility.getRandomAdjacentOpenTile(Game1.player.getTileLocation(), Game1.player.currentLocation);
            }

            if (tries >= 6)
            {
                BNC_Core.Logger.Log($"Failed!", StardewModdingAPI.LogLevel.Debug);
                return ActionResponse.Retry;
            }

            BNC_Core.Logger.Log($"Spawning at {pos.X}, {pos.Y}!", StardewModdingAPI.LogLevel.Debug);
            Chest giftbox = new Chest(0, new List<Item>
            {
                new StardewValley.Object(item, cnt)
            }, pos, giftbox: true, giftboxIndex: 1);

            Game1.player.currentLocation.objects.Add(pos, giftbox);

            return ActionResponse.Done;
        }
    }
}
