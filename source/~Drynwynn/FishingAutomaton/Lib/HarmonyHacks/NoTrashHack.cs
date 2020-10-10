/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

using DsStardewLib.Config;
using DsStardewLib.Utils;
using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FishingAutomaton.Lib.HarmonyHacks
{
  /// <summary>
  /// Removes the chance that trash can be returned when fishing.
  /// </summary>
  [HarmonyPatch(typeof(GameLocation), "getFish", new Type[] { typeof(float), typeof(int), typeof(int), typeof(Farmer), typeof(double), typeof(string) })]
  class NoTrashHack : HarmonyHack
  {
    // Variables to do the business, and also to meet the Hack interface.
    private static ModConfig config = null;
    private static Logger log = null;
    public Logger Log { get => NoTrashHack.log; set => NoTrashHack.log = value; }
    public HarmonyConfig Config { get => NoTrashHack.config; set => NoTrashHack.config = value as ModConfig; }

    /// <summary>
    /// The random check for trash is burned into the getFish function (without calling out to a separate function would could PostFix patch,
    /// so remove the BR opcode after the check so it never branches to the trash option.
    /// </summary>
    /// <param name="instructions">Harmony var - the list of original instructions</param>
    /// <returns>An enumerable of code instructions Harmony will inject.</returns>
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SkipTheTrash(IEnumerable<CodeInstruction> instructions)
    {
      log.Debug("Starting IL Harmony injection for skipping trash");
      List<CodeInstruction> allInst = instructions.ToList<CodeInstruction>();

      bool done = false;
      bool foundChanceVar = false;

      if (!config.noTrash) {
        log.Debug("Config option is false, skipping Harmony injection");
        foreach (var ci in instructions) { yield return ci; }
      }
      else {
        log.Debug("Harmony injecting IL into getFish(float, int, int, Farmer, double, string) in GameLocation.cs");
        for (int i = 0; i < allInst.Count; ++i) {
          CodeInstruction ci = allInst[i];

          log.Silly($"CI is |{ci}| with operand type |{ci?.operand?.GetType()}|");
          if (!done) {
            if (!foundChanceVar) {
              if (ci.opcode == OpCodes.Ldloc_S && ci.operand is LocalBuilder && (ci.operand as LocalBuilder).LocalIndex == 17) {
                log.Silly("Found local variable 'chance' load");
                foundChanceVar = true;
              }
            }
            else {
              // We've found chance, check for branch if greater than.
              if (ci.opcode == OpCodes.Bgt_Un) {
                // We're done!
                log.Debug("Found the branch after loading chance.  Removing branch");
                // We could do something here like adding a label jumping to self, or nop'ing the last four ops, but let's just
                // pop off the values as the BGT would have and move on.
                yield return new CodeInstruction(OpCodes.Pop);
                ci.opcode = OpCodes.Pop;
                done = true;
              }
              else {
                // we did not find the right location
                foundChanceVar = false;
              }
            }
          }
          log.Silly($"Sending on the code {ci.opcode}");
          yield return ci;
        }
      }
    }
  }
}
