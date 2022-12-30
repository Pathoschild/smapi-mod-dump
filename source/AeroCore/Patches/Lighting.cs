/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using AeroCore.Utils;
using AeroCore.Models;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;
using Microsoft.Xna.Framework.Graphics;

namespace AeroCore.Patches
{
    [HarmonyPatch]
    internal class Lighting
    {
        internal static event Action<LightingEventArgs> LightingEvent;
        private static Vector2 offset;
        private static Vector2 v_offset;

        internal static MethodBase TargetMethod() => AccessTools.TypeByName("StardewModdingAPI.Framework.SGame").MethodNamed("DrawImpl");
        internal static void Prefix()
        {
            Game1.drawLighting = Game1.hasLoadedGame;
            if (Game1.outdoorLight == Color.White)
                Game1.outdoorLight = Color.Black;
        }
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => patcher.Run(instructions);

        private static ILHelper patcher = new ILHelper(ModEntry.monitor, "Lighting")
            .SkipTo(new CodeInstruction[] {
                new(OpCodes.Ldsfld, typeof(Game1).FieldNamed(nameof(Game1.drawLighting))),
                new(OpCodes.Brfalse)
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Game1).MethodNamed("get_lightmap")),
                new(OpCodes.Callvirt, typeof(Texture2D).MethodNamed("get_Bounds"))
            })
            .Skip(2)
            .Transform(InjectEvent)
            .Remove(1)
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldfld, typeof(Options).FieldNamed("lightingQuality")),
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Div),
                new(OpCodes.Conv_R4)
            })
            .Skip(5)
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, typeof(Lighting).FieldNamed(nameof(v_offset))),
                new(OpCodes.Call, typeof(Vector2).MethodNamed("op_Addition"))
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Call,typeof(Game1).MethodNamed("get_lightmap")),
                new(OpCodes.Call,typeof(Vector2).MethodNamed("get_Zero"))
            })
            .Skip(1)
            .Remove(1)
            .Add(new CodeInstruction(OpCodes.Ldsfld, typeof(Lighting).FieldNamed(nameof(offset))))
            .Finish();

        internal static IList<CodeInstruction> InjectEvent(ILHelper.ILEnumerator cursor)
        {
            int box = ((LocalBuilder)cursor.Current.operand).LocalIndex;
            return new CodeInstruction[] {
                cursor.Current,
                cursor.source.GetNext(),
                new(OpCodes.Ldloc_S, box),
                new(OpCodes.Ldloc_S, box + 1),
                new(OpCodes.Call, typeof(Lighting).MethodNamed(nameof(EmitEvent)))
            };
        }
        internal static void EmitEvent(Color ambient, float intensity)
        {
            GetOffset();
            LightingEvent?.Invoke(new(intensity, ambient, v_offset, offset));
        }
        internal static void GetOffset()
        {
            int pixsize = Game1.options.lightingQuality / 2;
            var pos = Game1.viewport.Location;
            // lightmap draw offset
            offset = new(-(pos.X % pixsize), -(pos.Y % pixsize));
            // lighting subpixel offset
            v_offset = new(-offset.X / (float)pixsize, -offset.Y / (float)pixsize);
        }
    }
}
