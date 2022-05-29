/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Reflection;

using HarmonyLib;

using StardewModdingAPI;

namespace ChestEx.LanguageExtensions {
  public static class HarmonyExtensions {
    public static void PatchEx(this Harmony harmony, MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null,
                               HarmonyMethod transpiler = null, String reason = "") {
      GlobalVars.gSMAPIMonitor.Log($"Trying to patch '{original}'...", LogLevel.Debug);
      String details = String.IsNullOrWhiteSpace(reason) ? String.Empty : $" to {reason}";

      var prev = Harmony.GetPatchInfo(original);
      if (harmony.Patch(original, prefix, postfix, transpiler) is null) GlobalVars.gSMAPIMonitor.Log($"Could NOT patch '{original}'{details}!", LogLevel.Error);
      var after = Harmony.GetPatchInfo(original);

      GlobalVars.gSMAPIMonitor.Log($"Patched '{original}'{details}.", LogLevel.Info);
      if (prefix is not null && (after is null || prev is not null && after.Prefixes.Count < prev.Prefixes.Count))
        GlobalVars.gSMAPIMonitor.Log("But the prefix patch failed!", LogLevel.Error);
      if (postfix is not null && (after is null || prev is not null && after.Postfixes.Count < prev.Postfixes.Count))
        GlobalVars.gSMAPIMonitor.Log("But the postfix patch failed!", LogLevel.Error);
      if (transpiler is not null && (after is null || prev is not null && after.Transpilers.Count < prev.Transpilers.Count))
        GlobalVars.gSMAPIMonitor.Log("But the transpiler patch failed!", LogLevel.Error);
    }

    public static void ReportTranspilerStatus(this Boolean transpilerResult) {
      GlobalVars.gSMAPIMonitor.Log($"Inner transpiler patch {(transpilerResult ? "succeeded." : "failed!")}", LogLevel.Debug);
    }
  }
}
