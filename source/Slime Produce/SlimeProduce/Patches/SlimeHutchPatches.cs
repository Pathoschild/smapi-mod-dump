/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace SlimeProduce
{
    public static class SlimeHutchPatches
    {
        public static void DayUpdatePostfix(SlimeHutch __instance)
        {
            Building building = __instance.getBuilding();
            Random r = new((int)(Game1.stats.DaysPlayed + (uint)((int)Game1.uniqueIDForThisGame / 10) + (uint)(building.tileX.Value * 77) + (uint)(building.tileY.Value * 777)));
            List<GreenSlime> slimes = new();

            foreach (NPC npc in __instance.characters)
                if (npc is GreenSlime slime)
                    slimes.Add(slime);

            foreach (KeyValuePair<Vector2, SObject> o in __instance.Objects.Pairs)
                if (o.Value.Name == "Slime Ball")
                    o.Value.orderData.Set(StringyStuff.ToSlimeString(slimes[r.Next(slimes.Count)]));
        }
    }
}
