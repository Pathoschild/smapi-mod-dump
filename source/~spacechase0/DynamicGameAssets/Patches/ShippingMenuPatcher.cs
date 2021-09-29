/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DynamicGameAssets.Framework;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Menus;

namespace DynamicGameAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="ShippingMenu"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class ShippingMenuPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<ShippingMenu>(nameof(ShippingMenu.parseItems)),
                transpiler: this.GetHarmonyMethod(nameof(Transpile_ParseItems))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method which transpiles <see cref="ShippingMenu.parseItems"/>.</summary>
        public static IEnumerable<CodeInstruction> Transpile_ParseItems(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            return PatchCommon.RedirectForFakeObjectIdTranspiler(gen, original, insns);
        }
    }
}
