/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Monsters;
using System;
using xTile.Dimensions;

namespace StardewRoguelike.Extensions
{
    internal static class MonsterExtensions
    {
        public static float AdjustRangeForHealth(this Monster monster, float min, float max)
        {
            float range = max - min;
            float hpPercent = (float)monster.Health / monster.MaxHealth;
            float toSubtract = hpPercent * range;
            return Math.Min(Math.Max(min, max - toSubtract), max);
        }

        public static int AdjustRangeForHealth(this Monster monster, int min, int max)
        {
            int range = max - min;
            float hpPercent = (float)monster.Health / monster.MaxHealth;
            int toSubtract = (int)Math.Round(hpPercent * range, 0);
            return Math.Min(Math.Max(min, max - toSubtract), max);
        }

        public static void KeepInMap(this Monster monster)
        {
            if (monster.Position.X < 0f)
                monster.Position = new(0, monster.Position.Y);
            if (monster.Position.Y < 0f)
                monster.Position = new(monster.Position.X, 0);
            if (monster.Position.X > monster.currentLocation.map.GetLayer("Back").LayerWidth * 64)
                monster.Position = new(monster.currentLocation.map.GetLayer("Back").LayerWidth * 64, monster.Position.Y);
            if (monster.Position.Y > monster.currentLocation.map.GetLayer("Back").LayerHeight * 64)
                monster.Position = new(monster.Position.X, monster.currentLocation.map.GetLayer("Back").LayerHeight * 64);
        }
    }
}
