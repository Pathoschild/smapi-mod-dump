/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/arannya/BoomerangMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;

namespace Boomerang
{ 
    internal sealed class Patches
    {
        internal static bool getCategoryName_Prefix(MeleeWeapon __instance, ref string __result)
        {
            if (__instance.itemId.Value == Boomerang.ModEntry.itemID_c)
            {
                __result = "Boomerang";
                return false;
            }
            return true;
        }
        
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            bool foundSourceRect = false;
            var locItemID = ItemRegistry.type_weapon + Boomerang.ModEntry.itemID_c;
            var funcGetSourceRect = AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetSourceRect));;
            var codes = new List<CodeInstruction>(instructions);
            var exitPatchLabel = gen.DefineLabel();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (i > 0 && codes[i-1].opcode == OpCodes.Callvirt &&
                    codes[i-1].operand == (object) funcGetSourceRect &&
                    codes[i].opcode == OpCodes.Stloc_1)
                {
                    if (!foundSourceRect)
                    {
                        codes[i + 1].labels.Add(exitPatchLabel);
                        yield return new CodeInstruction(OpCodes.Ldarg, 5);
                        yield return new CodeInstruction(OpCodes.Ldstr, locItemID);
                        yield return new CodeInstruction(OpCodes.Ldarg, 7);
                        yield return new CodeInstruction(OpCodes.Call, 
                            AccessTools.Method(typeof(Boomerang.Patches), nameof(Boomerang.Patches.cmpWeaponSpecial)));
                        yield return new CodeInstruction(OpCodes.Brfalse, exitPatchLabel);
                        //yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Ldsfld, 
                            AccessTools.Field(typeof(Rectangle), "emptyRectangle"));
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        foundSourceRect = true;
                    }
                }
            }
        }

        internal static bool cmpWeaponSpecial(string s1, string s2, bool isOnSpecial)
        {
            return s1.Equals(s2) && isOnSpecial;
        }
    }
}