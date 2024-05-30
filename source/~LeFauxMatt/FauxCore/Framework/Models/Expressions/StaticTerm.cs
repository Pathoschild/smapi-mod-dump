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

using System.Collections.Immutable;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
using StardewValley.Inventories;

/// <summary>Represents a basic term.</summary>
internal class StaticTerm : IExpression
{
    /// <summary>Initializes a new instance of the <see cref="StaticTerm" /> class.</summary>
    /// <param name="term">The search value.</param>
    public StaticTerm(string term) => this.Term = term;

    /// <inheritdoc />
    public virtual ExpressionType ExpressionType => ExpressionType.Static;

    /// <inheritdoc />
    public bool IsValid => !string.IsNullOrWhiteSpace(this.Term);

    /// <inheritdoc />
    public virtual string Text =>
        this.Term.Contains(' ') || string.IsNullOrWhiteSpace(this.Term) ? $"\"{this.Term}\"" : this.Term;

    /// <inheritdoc />
    public IImmutableList<IExpression> Expressions
    {
        get => ImmutableList<IExpression>.Empty;
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public IExpression? Parent { get; set; }

    /// <inheritdoc />
    public string Term { get; set; }

    /// <inheritdoc />
    public int Compare(Item? x, Item? y) => (this.Equals(x) ? -1 : 1).CompareTo(this.Equals(y) ? -1 : 1);

    /// <inheritdoc />
    public bool Equals(Item? other) =>
        other is not null
        && ((other.DisplayName is not null && this.Equals(other.DisplayName))
            || other.GetContextTags().Any(this.Equals));

    /// <inheritdoc />
    public bool Equals(IInventory? other) => other is not null && other.Any(this.Equals);

    /// <inheritdoc />
    public bool Equals(string? other) =>
        !string.IsNullOrWhiteSpace(other) && other.Contains(this.Term, StringComparison.OrdinalIgnoreCase);
}