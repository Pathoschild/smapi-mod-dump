using System;
using Harmony;
using Netcode;
using StardewValley;
using StardewValley.Locations;

namespace MoreMineLadders
{
    public static class checkStoneForItems_Patch
    {
        static void Prefix(int x, int y)
        {
            if (!MoreMineLadders.instance.config.Enabled)
                return;

            // We'll store this tile to make sure the original method doesn't create a ladder on this tile while we're using it.
            createLadderDown_Patch.hx = x;
            createLadderDown_Patch.hy = y;
        }
        static void Postfix(MineShaft __instance, int x, int y, bool ___ladderHasSpawned, NetIntDelta ___netStonesLeftOnThisLevel)
        {
            createLadderDown_Patch.hx = -1; // We don't really need this anymore
            createLadderDown_Patch.hy = -1;
            if (__instance == null || !MoreMineLadders.instance.config.Enabled || ___ladderHasSpawned)
                return;

            var cfg = MoreMineLadders.instance.config;

            if (___netStonesLeftOnThisLevel.Value == 0)
            {
                __instance.createLadderDown(x, y);
                return;
            }

            Random random = new Random(x * 1000 + y + __instance.mineLevel + (int)Game1.uniqueIDForThisGame / 2); // Game uses this to seed the ladder generation so I guess we will too.
            random.NextDouble();

            // We'll make luck have a slightly bigger impact when enabled to compensate for the fact we're not checking for remaining stones.
            double chance = cfg.affectedByLuck ? cfg.dropLadderChance + (double)Game1.player.LuckLevel / 100.0 + Game1.dailyLuck / 3.5 : cfg.dropLadderChance;
            double num = random.NextDouble();
            if (num < chance)
            {
                __instance.createLadderDown(x, y);
            }

        }
    }

    public static class createLadderDown_Patch
    {
        public static int hx = -1;
        public static int hy = -1;

        static bool Prefix(int x, int y)
        {
            if (hx == x && hy == y) // Don't create the ladder if we're going to use this tile.
                return false;

            return true;
        }
    }
}
