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
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using HDPortraits.Models;

namespace HDPortraits.Patches
{
    [HarmonyPatch]
    class PortraitDrawPatch
    {
        private static ILHelper patcher = SetupPatch();
        internal static readonly PerScreen<HashSet<MetadataModel>> lastLoaded = new(() => new());
        internal static readonly PerScreen<MetadataModel> currentMeta = new();
        internal static readonly PerScreen<Dictionary<string, string>> EventOverrides = new(() => new());
        internal static readonly PerScreen<string> overrideName = new();

        [HarmonyPatch(typeof(DialogueBox), "drawPortrait")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in patcher.Run(instructions))
                yield return code;
        }
        public static ILHelper SetupPatch()
        {
            return new ILHelper("Dialogue Patch")
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(DialogueBox).FieldNamed("characterDialogue")),
                    new(OpCodes.Ldfld, typeof(Dialogue).FieldNamed("speaker")),
                    new(OpCodes.Callvirt,typeof(NPC).MethodNamed("get_Portrait"))
                })
                .Add(new CodeInstruction(OpCodes.Call, typeof(PortraitDrawPatch).MethodNamed("SwapTexture")))
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, typeof(DialogueBox).FieldNamed("characterDialogue")),
                    new(OpCodes.Callvirt, typeof(Dialogue).MethodNamed("getPortraitIndex"))
                })
                .Remove(new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4_S, 64),
                    new(OpCodes.Ldc_I4_S, 64),
                    new(OpCodes.Call, typeof(Game1).MethodNamed("getSourceRectForStandardTileSheet"))
                })
                .Add(new CodeInstruction(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("GetData")))
                .SkipTo(new CodeInstruction[]
                {
                    new(OpCodes.Call, typeof(Color).MethodNamed("get_White")),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Call, typeof(Vector2).MethodNamed("get_Zero"))
                })
                .Remove()
                .Add(new CodeInstruction[]{
                    new(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("GetScale"))
                })
                .Finish();
        }
        public static Texture2D SwapTexture(Texture2D texture)
        {
            return currentMeta.Value?.overrideTexture?.Value ?? texture;
        }
        public static Rectangle GetData(Texture2D texture, int index)
        {
            int asize = currentMeta.Value?.Size ?? 64;
            Rectangle ret = (currentMeta.Value?.Animation != null) ?
                currentMeta.Value.Animation.GetSourceRegion(texture, asize, index, Game1.currentGameTime.ElapsedGameTime.Milliseconds) :
                Game1.getSourceRectForStandardTileSheet(texture, index, asize, asize);
            if (!texture.Bounds.Contains(ret))
                ret = new(0, 0, asize, asize);
            return ret;
        }
        public static string GetSuffix(NPC npc)
        {
            return (bool)DialoguePatch.islandwear.GetValue(npc) ? "Beach" : npc.uniquePortraitActive ? npc.currentLocation.Name : null;
        }
        public static float GetScale()
        {
            return currentMeta.Value is not null ? 256f / currentMeta.Value.Size : 4f;
        }
    }
}
