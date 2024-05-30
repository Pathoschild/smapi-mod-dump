/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Limits;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for LimitBreak abilities.</summary>
public interface ILimitBreak : IEquatable<ILimitBreak>
{
    /// <summary>Gets the corresponding combat profession which offers this <see cref="ILimitBreak"/>.</summary>
    Profession ParentProfession { get; }

    /// <summary>Gets the ID of the <see cref="ILimitBreak"/>, which equals the corresponding combat profession index.</summary>
    int Id { get; }

    /// <summary>Gets the technical name of the <see cref="ILimitBreak"/>.</summary>
    string Name { get; }

    /// <summary>Gets the localized and gendered name for the <see cref="ILimitBreak"/>.</summary>
    string DisplayName { get; }

    /// <summary>Gets the localized description text for the <see cref="ILimitBreak"/>.</summary>
    string Description { get; }

    /// <summary>Gets the ID of the corresponding <see cref="Buff"/>.</summary>
    string BuffId { get; }

    /// <summary>Gets <see cref="ILimitBreak"/>'s principal <see cref="Color"/>.</summary>
    Color Color { get; }

    /// <summary>Gets or sets the current charge value.</summary>
    double ChargeValue { get; set; }

    /// <summary>Gets a value indicating whether the <see cref="ILimitBreak"/> is currently active.</summary>
    bool IsActive { get; }

    /// <summary>Gets a value indicating whether all activation conditions for the <see cref="ILimitBreak"/> are currently met.</summary>
    bool CanActivate { get; }

    /// <summary>Gets a value indicating whether the <see cref="LimitGauge"/> is currently rendering.</summary>
    bool IsGaugeVisible { get; }
}
