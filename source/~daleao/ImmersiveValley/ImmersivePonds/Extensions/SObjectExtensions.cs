/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using Common.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Whether a given object is algae or seaweed.</summary>
    public static bool IsAlgae(this SObject @object) =>
        @object.ParentSheetIndex is Constants.SEAWEED_INDEX_I or Constants.GREEN_ALGAE_INDEX_I
            or Constants.WHITE_ALGAE_INDEX_I;

    /// <summary>Whether a given object is a non-radioactive metallic ore.</summary>
    public static bool IsNonRadioactiveOre(this SObject @object) =>
        @object.ParentSheetIndex is 378 or 380 or 384 or 386;

    /// <summary>Whether a given object is a non-radioactive metal ingot.</summary>
    public static bool IsNonRadioactiveIngot(this SObject @object) =>
        @object.ParentSheetIndex is 334 or 335 or 336 or 337;

    /// <summary>Whether a given object is a radioactive fish.</summary>
    public static bool IsRadioactiveFish(this SObject @object) =>
        @object.Category == SObject.FishCategory && @object.Name.ContainsAnyOf("Mutant", "Radioactive");

    /// <summary>Whether a given object is a legendary fish.</summary>
    public static bool IsLegendary(this SObject @object) =>
        @object.HasContextTag("fish_legendary");
}