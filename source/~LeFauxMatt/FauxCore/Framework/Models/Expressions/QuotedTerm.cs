/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Models.Expressions;

using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

/// <summary>Represents an item attribute term.</summary>
internal sealed class QuotedTerm : StaticTerm
{
    /// <summary>The begin attribute character.</summary>
    public const char BeginChar = '"';

    /// <summary>The end attribute character.</summary>
    public const char EndChar = '"';

    /// <summary>Initializes a new instance of the <see cref="QuotedTerm" /> class.</summary>
    /// <param name="term">The expression.</param>
    public QuotedTerm(string term)
        : base(term) { }

    /// <inheritdoc />
    public override ExpressionType ExpressionType => ExpressionType.Quoted;

    /// <inheritdoc />
    public override string Text => $"{QuotedTerm.BeginChar}{this.Term}{QuotedTerm.EndChar}";
}