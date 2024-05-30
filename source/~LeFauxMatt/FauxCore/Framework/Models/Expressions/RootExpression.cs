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

/// <inheritdoc />
internal sealed class RootExpression : AnyExpression
{
    /// <summary>Initializes a new instance of the <see cref="RootExpression" /> class.</summary>
    /// <param name="expressions">The grouped expressions.</param>
    public RootExpression(params IExpression[] expressions)
        : base(expressions) { }

    /// <inheritdoc />
    public override string Text => $"{string.Join(' ', this.Expressions.Select(expression => expression.Text))}";
}