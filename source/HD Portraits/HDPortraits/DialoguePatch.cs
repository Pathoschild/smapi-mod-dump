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
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace HDPortraits
{
    [HarmonyPatch]
    class DialoguePatch
    {

        internal static FieldInfo islandwear = typeof(NPC).FieldNamed("isWearingIslandAttire");

        [HarmonyPatch(typeof(Dialogue),"exitCurrentDialogue")]
        [HarmonyPostfix]
        public static void Cleanup()
        {
            foreach (var item in PortraitDrawPatch.lastLoaded.Value)
                item?.Reload();
            PortraitDrawPatch.lastLoaded.Value.Clear();
        }

        public static void Init(DialogueBox __instance)
        {
            NPC npc = __instance.characterDialogue?.speaker;
            if (npc != null)
            {
                if (ModEntry.TryGetMetadata(npc.getTextureName(), PortraitDrawPatch.GetSuffix(npc), out var meta))
                {
                    PortraitDrawPatch.lastLoaded.Value.Add(meta);
                    PortraitDrawPatch.currentMeta.Value = meta;
                    meta.Reload();
                }
            }
            PortraitDrawPatch.overridden.Value = __instance.characterDialogue?.overridePortrait != null;
        }

        [HarmonyPatch(typeof(DialogueBox), "closeDialogue")]
        [HarmonyPostfix]
        public static void Finish()
        {
            PortraitDrawPatch.overridden.Value = false;
            PortraitDrawPatch.currentMeta.Value = null;
        }
    }
}
