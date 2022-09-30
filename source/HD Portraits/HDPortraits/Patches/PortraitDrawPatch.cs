/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using HDPortraits.Models;
using AeroCore.Utils;
using AeroCore;

namespace HDPortraits.Patches
{
    [HarmonyPatch]
    class PortraitDrawPatch
    {
        internal static readonly PerScreen<HashSet<MetadataModel>> lastLoaded = new(() => new());
        internal static readonly PerScreen<MetadataModel> currentMeta = new();
        internal static readonly PerScreen<string> overrideName = new();
        internal static readonly PerScreen<Dictionary<NPC, string>> NpcEventSuffixes = new(() => new());
        internal static FieldInfo islandwear = typeof(NPC).FieldNamed("isWearingIslandAttire");

        [HarmonyPatch(typeof(Event), "command_changePortrait")]
        [HarmonyPostfix]
        public static void changeActivePortraitOf(string[] split, Event __instance)
        {
            NPC n = __instance.getActorByName(split[1]) ?? Game1.getCharacterFromName(split[1]);
            NpcEventSuffixes.Value[n] = split[2];
            if (Game1.activeClickableMenu is DialogueBox db && db.characterDialogue?.speaker == n)
            {
                DialoguePatch.Finish();
                DialoguePatch.Init(db);
            }
        }

        [HarmonyPatch(typeof(DialogueBox), "drawPortrait")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => patcher.Run(instructions);
        private static ILHelper patcher = new ILHelper(ModEntry.monitor, "Dialogue Patch")
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(DialogueBox).FieldNamed("characterDialogue")),
                    new(OpCodes.Ldfld, typeof(Dialogue).FieldNamed("speaker")),
                    new(OpCodes.Callvirt,typeof(NPC).MethodNamed("get_Portrait"))
                })
                .Skip(4)
                .Add(new CodeInstruction(OpCodes.Call, typeof(PortraitDrawPatch).MethodNamed("SwapTexture")))
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(DialogueBox).FieldNamed("characterDialogue")),
                    new(OpCodes.Callvirt, typeof(Dialogue).MethodNamed("getPortraitIndex"))
                })
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4_S, 64),
                    new(OpCodes.Ldc_I4_S, 64),
                    new(OpCodes.Call, typeof(Game1).MethodNamed("getSourceRectForStandardTileSheet"))
                })
                .Remove(3)
                .Add(new CodeInstruction(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("GetData")))
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Call, typeof(Color).MethodNamed("get_White")),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Call, typeof(Vector2).MethodNamed("get_Zero"))
                })
                .Skip(3)
                .Remove(1)
                .Add(new CodeInstruction[]{
                    new(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("GetScale"))
                })
                .Finish();
        public static Texture2D SwapTexture(Texture2D texture) 
            => currentMeta.Value is not null && currentMeta.Value.TryGetTexture(out var tex) ? tex : texture;
        public static Rectangle GetData(Texture2D texture, int index)
            => currentMeta.Value?.GetRegion(index, Game1.currentGameTime.ElapsedGameTime.Milliseconds) ??
            Game1.getSourceRectForStandardTileSheet(texture, index, 64, 64);
        public static string GetSuffix(NPC npc)
        {
            return NpcEventSuffixes.Value.TryGetValue(npc, out string s) ? s : 
                (bool)islandwear.GetValue(npc) ? "Beach" : 
                npc.uniquePortraitActive ? npc.currentLocation.Name : null;
        }
    }
}
