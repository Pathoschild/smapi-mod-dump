/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using AeroCore.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AeroCore.Patches
{
    [HarmonyPatch]
    internal class Action
    {
        internal static readonly Dictionary<string, int> ActionCursors = new(StringComparer.OrdinalIgnoreCase);
        internal static readonly Dictionary<string, IAeroCoreAPI.ActionHandler> Actions = new(StringComparer.OrdinalIgnoreCase);
        private static readonly PerScreen<int> CurrentActionCursor = new();

        [HarmonyPatch(typeof(GameLocation),"performAction")]
        [HarmonyPrefix]
        internal static bool performAction(string action, Farmer who, ref bool __result, GameLocation __instance, xTile.Dimensions.Location tileLocation)
        {
            if (action == null || !who.IsLocalPlayer)
                return true;

            string name = action.GetChunk(' ', 0);
            if (!Actions.TryGetValue(name, out var exec))
                return true;

            int index = action.IndexOf(' ') + 1;
            __result = true;
            exec(who, index > 0 && index < action.Length ? action[index..] : "", new(tileLocation.X, tileLocation.Y), __instance);
            return false;
        }

        [HarmonyPatch(typeof(Game1),"updateCursorTileHint")]
        [HarmonyPostfix]
        internal static void changeActionCursor()
        {
            CurrentActionCursor.Value = 0;
            string action = Game1.currentLocation.doesTileHaveProperty((int)Game1.lastCursorTile.X, (int)Game1.lastCursorTile.Y, "Action", "Buildings");
            if (action is not null && ActionCursors.TryGetValue(action.GetChunk(' ', 0), out var cursor))
                CurrentActionCursor.Value = cursor;
        }

        [HarmonyPatch(typeof(Game1), "drawMouseCursor")]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> PatchCursor(IEnumerable<CodeInstruction> instructions, ILGenerator gen) => cursorPatch.Run(instructions, gen);
        private static ILHelper cursorPatch = new ILHelper(ModEntry.monitor, "Mouse Cursor")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldsfld,typeof(Game1).FieldNamed("isActionAtCurrentCursorTile")),
                new(OpCodes.Brfalse_S),
                new(OpCodes.Call,typeof(Game1).MethodNamed("get_currentMinigame")),
                new(OpCodes.Brtrue_S)
            })
            .Skip(4)
            .Add(new CodeInstruction[]
            {
				new(OpCodes.Ldsfld, typeof(Action).FieldNamed(nameof(CurrentActionCursor))),
				new(OpCodes.Call, typeof(PerScreen<int>).MethodNamed("get_Value")),
				new(OpCodes.Dup)
			})
            .AddJump(OpCodes.Brtrue, "override")
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Pop)
            })
            .SkipTo(new CodeInstruction(OpCodes.Stsfld,typeof(Game1).FieldNamed("mouseCursor")))
            .AddLabel("override")
            .Finish();
    }
}
