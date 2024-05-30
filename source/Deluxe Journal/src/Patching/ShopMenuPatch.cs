/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="ShopMenu"/>.</summary>
    internal class ShopMenuPatch : PatchBase<ShopMenuPatch>
    {
        private static readonly MethodInfo ChargePlayerMethod;
        private static readonly MethodInfo OnSellMethod;

        private EventManager EventManager { get; }

        static ShopMenuPatch()
        {
            ChargePlayerMethod = AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.chargePlayer));
            OnSellMethod = SymbolExtensions.GetMethodInfo((Item soldItem) => OnSell(soldItem));
        }

        public ShopMenuPatch(EventManager eventManager, IMonitor monitor) : base(monitor)
        {
            EventManager = eventManager;
            Instance = this;
        }

        private static void OnSell(Item soldItem)
        {
            try
            {
                Farmer player = Game1.player;
                Instance.EventManager.SalableSold.Raise(player, new SalableEventArgs(player, soldItem, soldItem.Stack));
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(OnSell));
            }
        }

        /// <summary>
        /// Inject the <see cref="OnSell(ISalable)"/> method after the player is paid via a mouse click in
        /// a <see cref="ShopMenu"/>, indicating that an item was sold.
        /// </summary>
        /// 
        /// <remarks>
        /// This patch is required since the <see cref="ShopMenu.onSell"/> callback replaces the default sell
        /// behaviour. Patching is safer and improves compatibility with other mods and/or future updates.
        /// </remarks>
        private static IEnumerable<CodeInstruction> Transpiler_receiveLeftClick(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(ChargePlayerMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, OnSellMethod);
                }
            }
        }

        /// <inheritdoc cref="Transpiler_receiveLeftClick(IEnumerable{CodeInstruction})"/>
        private static IEnumerable<CodeInstruction> Transpiler_receiveRightClick(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(ChargePlayerMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc, 4);
                    yield return new CodeInstruction(OpCodes.Call, OnSellMethod);
                }
            }
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveLeftClick)),
                transpiler: new HarmonyMethod(typeof(ShopMenuPatch), nameof(Transpiler_receiveLeftClick))
            );

            Patch(harmony,
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveRightClick)),
                transpiler: new HarmonyMethod(typeof(ShopMenuPatch), nameof(Transpiler_receiveRightClick))
            );
        }
    }
}
