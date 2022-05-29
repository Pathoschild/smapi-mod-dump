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

namespace HDPortraits.Patches
{
    [HarmonyPatch]
    class DialoguePatch
    {
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
            bool overriden = __instance.characterDialogue?.overridePortrait != null;
            NPC npc = __instance.characterDialogue?.speaker;
            if (npc != null || overriden)
            {
                if (ModEntry.TryGetMetadata(overriden ? PortraitDrawPatch.overrideName.Value ?? "NULL" : npc.getTextureName(), PortraitDrawPatch.GetSuffix(npc), out var meta))
                {
                    PortraitDrawPatch.lastLoaded.Value.Add(meta);
                    PortraitDrawPatch.currentMeta.Value = meta;
                    meta.Reload();
                } else
                {
                    PortraitDrawPatch.currentMeta.Value = null;
                }
            }
        }

        public static void Finish()
        {
            Cleanup();
            PortraitDrawPatch.overrideName.Value = null;
            PortraitDrawPatch.currentMeta.Value = null;
        }
    }
}
