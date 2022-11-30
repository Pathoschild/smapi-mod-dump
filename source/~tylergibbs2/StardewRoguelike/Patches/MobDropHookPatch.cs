/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Patches
{
    internal class MobDropHookPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "monsterDrop");

        public static bool Prefix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            if (BossFloor.IsBossFloor((MineShaft)__instance) || ChallengeFloor.IsChallengeFloor((MineShaft)__instance))
                return false;

            List<int> objects = new();
            int goldToDrop = Game1.random.Next(Roguelike.GoldDropMin, Roguelike.GoldDropMax + 1);

            for (int i = 0; i < goldToDrop; i++)
                objects.Add(384);

            if (Perks.HasPerk(Perks.PerkType.Scavenger) && Game1.random.NextDouble() < 0.05)
                objects.Add(874);

            Vector2 playerPosition = new(Game1.player.GetBoundingBox().Center.X, Game1.player.GetBoundingBox().Center.Y);

            for (int k = 0; k < objects.Count; k++)
            {
                int objectToAdd = objects[k];
                if (objectToAdd < 0)
                    __instance.debris.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(objectToAdd), Game1.random.Next(1, 4), new Vector2(x, y), playerPosition)));
                else
                    __instance.debris.Add(monster.ModifyMonsterLoot(new Debris(objectToAdd, new Vector2(x, y), playerPosition)));
            }

            return false;
        }
    }
}
