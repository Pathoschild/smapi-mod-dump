/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/StardewSandbox
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using HatShopRestoration.Framework.Patches;
using HatShopRestoration.Framework.Patches.Locations;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace HatShopRestoration.Framework.Patches.Entities
{
    internal class NPCPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(NPC);

        public NPCPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "updateConstructionAnimation", null), transpiler: new HarmonyMethod(GetType(), nameof(UpdateConstructionAnimationTranspiler)));
        }

        private static IEnumerable<CodeInstruction> UpdateConstructionAnimationTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            try
            {
                int? index = null;
                Label returnLabel = il.DefineLabel();

                // Get the indices to insert at
                var list = instructions.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Call && list[i].operand is not null && list[i].operand.ToString().Contains("get_MasterPlayer", StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                    }
                    else if (list[i].opcode == OpCodes.Ret)
                    {
                        list[i].labels.Add(returnLabel);
                    }
                }

                if (index is not null)
                {
                    list.Insert(index.Value, new CodeInstruction(OpCodes.Brtrue_S, returnLabel));
                    list.Insert(index.Value, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NPCPatch), nameof(ShouldHandleCarpenterLocation))));
                    list.Insert(index.Value, new CodeInstruction(OpCodes.Ldarg_0));
                }

                return list;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.GameLocation.carpenters: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static bool ShouldHandleCarpenterLocation(NPC npc)
        {
            bool shouldHandle = string.IsNullOrEmpty(ModEntry.GetActiveSpecialProjectId()) is false;
            if (shouldHandle)
            {
                Game1.warpCharacter(npc, "Forest", new Vector2(37f, 94f));

                ModEntry.modHelper.Reflection.GetField<bool>(npc, "isPlayingRobinHammerAnimation").SetValue(false);
                npc.shouldPlayRobinHammerAnimation.Value = true;
            }

            return shouldHandle;
        }
    }
}