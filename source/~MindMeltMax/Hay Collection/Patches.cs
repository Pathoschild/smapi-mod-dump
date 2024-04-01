/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection.Emit;

namespace HayCollection
{
    internal static class Patches
    {
        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.TryDropItemsOnCut)),
                transpiler: new(typeof(Patches), nameof(Grass_TryDropItemsOnCut_Transpiler))
            );
        }

        internal static IEnumerable<CodeInstruction> Grass_TryDropItemsOnCut_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator); //My first time using a code matcher, please bear with me
            var methInfo = AccessTools.Method(typeof(Patches), nameof(tryAddHayToInventory));

            matcher.Advance(171);
            matcher.RemoveInstruction();
            matcher.Insert(new CodeInstruction(OpCodes.Call, methInfo));

            foreach (var i in matcher.InstructionEnumeration())
                yield return i;
        }

        internal static bool tryAddHayToInventory(int count, GameLocation currentLocation)
        {
            int remainder = GameLocation.StoreHayInAnySilo(count, currentLocation);
            return remainder > 0 && !Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)178", remainder)); //I'm not dropping stuff on the floor, have a silo, or have inventory space
        }
    }
}
