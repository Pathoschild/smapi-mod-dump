/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StardewValley;

namespace StackEverythingRedux.Patches
{
    internal class AttachPatches
    {
        private static StardewValley.Object tackleToAttach;
        private static int tackleToAttachStack;

        public static void Prefix(Tool __instance, StardewValley.Object o)
        {
            if (__instance.QualifiedItemId == "(T)AdvancedIridiumRod" && o != null && o.Category == -22)
            {
                tackleToAttach = o;
                tackleToAttachStack = o.Stack;
            }
        }

        public static void Postfix(Tool __instance)
        {
            if (tackleToAttach != null && tackleToAttachStack > 0 && tackleToAttach.Stack == 0)
            {
                bool hasEmptySlot = false;
                int emptySlotIndex = 0;

                for (int i = 1; i < __instance.attachments.Count; i++)
                {
                    if (__instance.attachments[i] == null)
                    {
                        hasEmptySlot = true;
                        emptySlotIndex = i;
                        break;
                    }
                }

                if (hasEmptySlot)
                {
                    tackleToAttach.Stack = tackleToAttachStack;
                    __instance.attachments[emptySlotIndex] = tackleToAttach;
                    __instance.attachments[emptySlotIndex * 2 / 3].Stack -= tackleToAttachStack;
                }
            }

            tackleToAttach = null;
            tackleToAttachStack = 0;
        }
    }
}
