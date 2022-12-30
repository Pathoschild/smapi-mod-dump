/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Models;
using AeroCore.Utils;
using HarmonyLib;
using StardewValley;
using System;
using SObject = StardewValley.Object;
using System.Collections.Generic;
using System.Reflection.Emit;
using AeroCore.API;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;

namespace AeroCore.Patches
{
    [ModInit]
    [HarmonyPatch(typeof(Game1))]
    internal class UseItem
    {
        internal static event Action<IUseItemEventArgs> OnUseItem;
        internal static event Action<IHeldItemEventArgs> OnItemHeld;
        internal static event Action<IHeldItemEventArgs> OnStopItemHeld;
        private static readonly PerScreen<bool> ConsumeItem = new();

        internal static void Init()
        {
            ModEntry.monitor.Log("Prefixing DoFunction on all Tools...");
            var ToolTypes = Reflection.GetAllKnownTypes().Where(b => b.IsAssignableTo(typeof(Tool))).ToArray();
            for(int i = 0; i < ToolTypes.Length; i++)
				ModEntry.harmony.TryPatch(
                    AccessTools.DeclaredMethod(ToolTypes[i], "DoFunction"),
					prefix: new(typeof(UseItem).MethodNamed(nameof(UseTool)))
				);
            ModEntry.monitor.Log("Tool Prefixing complete!");
        }

        private static bool UseTool(Tool __instance, Farmer who, int x, int y, int power, GameLocation location)
        {
            UseItemEventArgs ev = new(__instance, new(x, y), who, location, power);
            OnUseItem?.Invoke(ev);
            ConsumeItem.Value = false;
            return !ev.IsHandled;
        }

        [HarmonyPatch(typeof(Item),nameof(Item.actionWhenBeingHeld))]
        [HarmonyPostfix]
        internal static void ItemHeld(Farmer who, Item __instance)
            => OnItemHeld?.Invoke(new HeldItemEventArgs(who, __instance));

        [HarmonyPatch(typeof(Item), nameof(Item.actionWhenStopBeingHeld))]
        [HarmonyPostfix]
        internal static void ItemStopHeld(Farmer who, Item __instance)
            => OnStopItemHeld?.Invoke(new HeldItemEventArgs(who, __instance));

        [HarmonyPatch(nameof(Game1.pressActionButton))]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> UseObject(IEnumerable<CodeInstruction> codes, ILGenerator gen) => objectUsePatch.Run(codes, gen);

        private static readonly ILHelper objectUsePatch = new ILHelper(ModEntry.monitor, "Use Item: Object")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                new(OpCodes.Callvirt, typeof(Farmer).PropertyGetter(nameof(Farmer.ActiveObject))),
                new(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.currentLocation))),
                new(OpCodes.Callvirt, typeof(SObject).MethodNamed(nameof(SObject.performUseAction)))
            })
            .Add(
                new CodeInstruction(OpCodes.Call,typeof(UseItem).MethodNamed(nameof(ActivateObject)))
            )
            .AddJump(OpCodes.Brtrue, "activated")
            .Skip(5)
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                new(OpCodes.Callvirt, typeof(Farmer).MethodNamed(nameof(Farmer.reduceActiveItemByOne)))
            })
            .AddWithLabels(new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, typeof(UseItem).FieldNamed(nameof(ConsumeItem))),
                new(OpCodes.Callvirt, typeof(PerScreen<bool>).PropertyGetter(nameof(PerScreen<bool>.Value)))
            }, new[] {"activated"})
            .AddJump(OpCodes.Brfalse, "no_consume")
            .Skip(2)
            .AddLabel("no_consume")
            .Finish();

        private static bool ActivateObject()
        {
            var who = Game1.player;
            if(!who.canMove || who.ActiveObject.isTemporarilyInvisible)
                return false;
            var ev = new UseItemEventArgs(who.ActiveObject);
            OnUseItem?.Invoke(ev);
            ConsumeItem.Value = ev.ConsumeItem;
            return ev.IsHandled;
        }
        private static bool ShouldConsume()
            => ConsumeItem.Value;
    }
}
