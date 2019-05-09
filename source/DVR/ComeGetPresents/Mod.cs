using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastPlace
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += TimeChanged;

        }

        private IDictionary<string, Vector2> npcs = new Dictionary<string, Vector2>
        {
            {"Abigail", new Vector2(1,22) },
            {"Emily", new Vector2(2,22) },
            {"Haley", new Vector2(3,22) },
            {"Leah", new Vector2(4,22) },
            {"Maru", new Vector2(5,22) },
            {"Penny", new Vector2(6,22) },

            {"Alex", new Vector2(7,22) },
            {"Elliott", new Vector2(8,22) },
            {"Harvey", new Vector2(9,22) },
            {"Sam", new Vector2(10,22) },
            {"Sebastian", new Vector2(12,22) },
            {"Shane", new Vector2(16,22) },

            {"Caroline", new Vector2(17,22) },
            {"Clint", new Vector2(18,22) },
            {"Demetrius", new Vector2(19,22) },
            {"Dwarf", new Vector2(20,22) },
            {"Evelyn", new Vector2(21,22) },
            {"George", new Vector2(22,22) },
            {"Gus", new Vector2(23,22) },

        };

        private IDictionary<NPC, int> returnTimes;

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            returnTimes = new Dictionary<NPC, int>();
            foreach (var kvp in npcs)
            {
                var npc = Game1.getCharacterFromName(kvp.Key);
                if (npc == null)
                {
                    Monitor.Log($"Character {kvp.Key} not found.", LogLevel.Error);
                    continue;
                }
                Game1.warpCharacter(npc, "BusStop", kvp.Value);
                npc.faceDirection(2);

                var sched = npc.Schedule;
                var ttl = 800;
                if (sched != null)
                {
                    ttl = sched.Min(x => x.Key);
                }
                if (ttl > 800)
                {
                    ttl = 800;
                }
                if (ttl % 100 == 0)
                {
                    ttl -= 50;
                }
                else
                {
                    ttl -= 10;
                }
                returnTimes[npc] = ttl;
            }
        }

        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            var returns = returnTimes.Where(x => x.Value == e.NewTime).Select(x => x.Key);
            foreach (var npc in returns)
            {
                Game1.warpCharacter(npc, npc.DefaultMap, npc.DefaultPosition / 64f);
            }
        }
    }
}
