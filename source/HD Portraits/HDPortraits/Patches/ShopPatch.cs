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
using StardewValley;
using StardewValley.Menus;
using HDPortraits.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HDPortraits.Patches
{
    [HarmonyPatch(typeof(ShopMenu))]
    internal class ShopPatch
    {
        [HarmonyPatch("setUpShopOwner")]
        [HarmonyPostfix]
        internal static void Init(ShopMenu __instance, string who)
        {
            ModEntry.monitor.Log(who);

            if (who is null && __instance.portraitPerson?.Name is null)
                return;

            string name = __instance.portraitPerson?.Name is not null ?
                __instance.portraitPerson.getTextureName() : NPC.getTextureNameForCharacter(who);

            string suffix = null;
            if(__instance.portraitPerson is not null)
                suffix = PortraitDrawPatch.GetSuffix(__instance.portraitPerson);

            if (ModEntry.TryGetMetadata(name, suffix, out var meta))
            {
                PortraitDrawPatch.lastLoaded.Value.Add(meta);
                PortraitDrawPatch.currentMeta.Value = meta;
                meta.Reload();
            }
        }
        [HarmonyPatch("draw")]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> drawPatch(IEnumerable<CodeInstruction> instructions)
        {
            return drawPatcher.Run(instructions);
        }

        internal static Rectangle GetData()
        {
            var current = PortraitDrawPatch.currentMeta.Value;
            if (current is null)
                return new(0, 0, 64, 64);

            Rectangle ret = (current.Animation is not null) ?
                current.Animation.GetSourceRegion(current.overrideTexture.Value ?? current.GetDefault(), 
                current.Size, 0, Game1.currentGameTime.ElapsedGameTime.Milliseconds) :
                new(0, 0, current.Size, current.Size);
            return ret;
        }

        internal static ILHelper drawPatcher = new ILHelper("Shop draw")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(ShopMenu).FieldNamed("portraitPerson")),
                new(OpCodes.Callvirt, typeof(NPC).MethodNamed("get_Portrait"))
            })
            .Add(new CodeInstruction(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("SwapTexture")))
            .Remove(new CodeInstruction[] { 
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_S, 64),
                new(OpCodes.Ldc_I4_S, 64),
            })
            .Remove()
            .Add(new CodeInstruction(OpCodes.Call, typeof(ShopPatch).MethodNamed("GetData")))
            .Remove(new CodeInstruction[] {
                new(OpCodes.Ldc_R4, 4f)
            })
            .Add(new CodeInstruction(OpCodes.Call, typeof(PortraitDrawPatch).MethodNamed("GetScale")))
            .Finish();
    }
}
