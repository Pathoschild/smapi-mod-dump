/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Integrations.Interfaces.Fluent;

#pragma warning disable SA1314 // Type parameter names should begin with T. Reviewed.
/// <summary>A specialized type allowing access to Project Fluent translations for <typeparamref name="EnumType"/> values.</summary>
/// <typeparam name="EnumType">The type of values this instance allows retrieving translations for.</typeparam>
public interface IEnumFluent<EnumType> : IFluent<EnumType>
    where EnumType : struct, Enum
{
    /// <summary>Returns an <typeparamref name="EnumType"/> value for a given localized name, or <c>null</c> if none match.</summary>
    /// <param name="localizedName">The localized name.</param>
    EnumType? GetFromLocalizedName(string localizedName);

    /// <summary>Returns localized names for all possible <typeparamref name="EnumType"/> values.</summary>
    IEnumerable<string> GetAllLocalizedNames();
}
#pragma warning restore SA1314 // Type parameter names should begin with T