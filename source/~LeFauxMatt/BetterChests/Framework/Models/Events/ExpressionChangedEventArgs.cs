/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.Events;

using StardewMods.BetterChests.Framework.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;

/// <summary>The event arguments when a change is made to an expression.</summary>
internal sealed class ExpressionChangedEventArgs : EventArgs
{
    /// <summary>Represents an event argument containing information about the change made to an expression.</summary>
    /// <param name="change">The expression change type.</param>
    /// <param name="expression">The expression.</param>
    public ExpressionChangedEventArgs(ExpressionChange change, IExpression expression)
    {
        this.Change = change;
        this.Expression = expression;
    }

    /// <summary>Gets the expression change.</summary>
    public ExpressionChange Change { get; }

    /// <summary>Gets the expression the action pertains to.</summary>
    public IExpression Expression { get; }
}