/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Digus.ProducerFrameworkMod")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class ProducerRuleControllerProduceOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ProducerRuleControllerProduceOutputPatcher"/> class.</summary>
    internal ProducerRuleControllerProduceOutputPatcher()
    {
        this.Target = "ProducerFrameworkMod.Controllers.ProducerRuleController"
            .ToType()
            .RequireMethod("ProduceOutput");
        this.Postfix!.before = new[] { OverhaulModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Replaces large egg and milk output quality with quantity for PFM machines.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("Overhaul.Modules.Professions")]
    private static void ProducerRuleControllerProduceOutputPostfix(
        SObject producer, SObject? input, bool probe)
    {
        if (probe || input?.Category is not (SObject.EggCategory or SObject.MilkCategory) ||
            !input.Name.ContainsAnyOf("Large", "L.") || !TweexModule.Config.LargeProducsYieldQuantityOverQuality ||
            !TweexModule.Config.DairyArtisanMachines.Contains(producer.Name))
        {
            return;
        }

        var output = producer.heldObject.Value;
        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion harmony patches
}
