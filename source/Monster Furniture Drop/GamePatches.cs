/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MangusuPixel/MonsterFurnitureDrop
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace MonsterFurnitureDrop
{
    internal static class GamePatches
    {
        internal static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
                    prefix: new HarmonyMethod(typeof(GamePatches), nameof(MonsterDrop_Prefix))
                );
            }
            catch (Exception e)
            {
                ModEntry.SMonitor.Log($"Failed patching game:\n{e}", LogLevel.Error);
            }
        }

        internal static bool MonsterDrop_Prefix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            var monsterDropRate = ModEntry.Config.MonsterDropRates.ContainsKey(monster.Name) ? ModEntry.Config.MonsterDropRates[monster.Name] : 0;
            var dropChance = ModEntry.Config.GlobalDropChance * monsterDropRate;

            if (ModEntry.ItemDropPool.Count > 0 && dropChance > Game1.random.NextDouble())
            {
                var randKey = "(F)" + ModEntry.ItemDropPool.Next();
                var itemInfo = ItemRegistry.GetDataOrErrorItem(randKey);

                monster.objectsToDrop.Add(randKey);

                if (ModEntry.Config.LogDropMessages)
                    ModEntry.SMonitor.Log($"{monster.displayName} dropped '{itemInfo.DisplayName}'.", LogLevel.Info);
            }

            return true;
        }
    }
}
