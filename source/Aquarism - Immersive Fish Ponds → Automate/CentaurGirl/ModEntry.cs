/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Characters;

namespace DaLion.Stardew.Centaur;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // create and patch Harmony instance
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new Type[] {typeof(SpriteBatch)}),
            transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(HorseDrawTranspiler)))
        );
    }

    /// <summary>Patch to offset the draw horse head to accomodate the larger elf head and torso.</summary>
    private static IEnumerable<CodeInstruction> HorseDrawTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        for (int i = 0; i < l.Count; ++i)
        {
            if (l[i].opcode == OpCodes.Ldc_R4 && l[i].operand is 48f)
            {
                l[i].operand = 36f;
                l[++i].operand = -48f;
            }
            else if (l[i].opcode == OpCodes.Ldc_I4_S && l[i].operand is (sbyte) 9)
            {
                l[i].operand = (sbyte) 16;
                l[++i].operand = (sbyte) 18;
                break;
            }
        }

        return l.AsEnumerable();
    }
}