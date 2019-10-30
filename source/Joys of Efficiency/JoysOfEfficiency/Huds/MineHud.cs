using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.Core;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace JoysOfEfficiency.Huds
{
    internal class MineHud
    {
        private static ITranslationHelper Translation => InstanceHolder.Translation;

        private static readonly MineIcons Icons = new MineIcons();
        private static List<Monster> _lastMonsters = new List<Monster>();

        public static string LastKilledMonster { get; private set; }

        public static void DrawMineGui(MineShaft shaft)
        {
            int stonesLeft = CountActualStones(shaft);
            Vector2 ladderPos = FindLadder(shaft);
            bool ladder = ladderPos != Vector2.Zero;

            List<Monster> currentMonsters = shaft.characters.OfType<Monster>().ToList();
            foreach (Monster mon in _lastMonsters)
            {
                if (!currentMonsters.Contains(mon) && mon.Name != "ignoreMe")
                {
                    LastKilledMonster = mon.Name;
                }
            }
            _lastMonsters = currentMonsters.ToList();
            string tallyStr = null;
            string ladderStr = null;
            if (LastKilledMonster != null)
            {
                int kills = Game1.stats.getMonstersKilled(LastKilledMonster);
                tallyStr = string.Format(Translation.Get("monsters.tally"), LastKilledMonster, kills);
            }

            string stonesStr;
            if (stonesLeft == 0)
            {
                stonesStr = Translation.Get("stones.none");
            }
            else
            {
                bool single = stonesLeft == 1;
                stonesStr = single ? Translation.Get("stones.one") : string.Format(Translation.Get("stones.many"), stonesLeft);
            }
            if (ladder)
            {
                ladderStr = Translation.Get("ladder");
            }
            Icons.Draw(stonesStr, tallyStr, ladderStr);

        }
        private static int CountActualStones(GameLocation shaft)
        {
            return shaft.Objects.Pairs.Count(kv => kv.Value.Name.Contains("Stone"));
        }
        private static Vector2 FindLadder(GameLocation shaft)
        {
            for (int i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; i++)
            {
                for (int j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; j++)
                {
                    int index = shaft.getTileIndexAt(new Point(i, j), "Buildings");
                    Vector2 loc = new Vector2(i, j);
                    if (shaft.Objects.ContainsKey(loc) || shaft.terrainFeatures.ContainsKey(loc))
                    {
                        continue;
                    }

                    if (index == 171 || index == 173 || index == 174)
                    {
                        return loc;
                    }
                }
            }
            return Vector2.Zero;
        }
    }
}
