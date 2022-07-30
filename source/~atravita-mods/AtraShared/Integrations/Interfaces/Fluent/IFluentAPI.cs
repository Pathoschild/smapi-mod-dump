/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewValley.GameData;

namespace AtraShared.Integrations.Interfaces.Fluent;

#pragma warning disable SA1124 // Do not use regions - copied from API.
#pragma warning disable SA1314 // Type parameter names should begin with T
#pragma warning disable SA1623 // Property summary documentation should match accessors
/// <summary>
/// The API for ProjectFluent.
/// </summary>
public interface IFluentApi
{
    #region Locale

    /// <summary>Gets the default locale of the game (en-US).</summary>
    IGameLocale DefaultLocale { get; }

    /// <summary>The current locale of the game.</summary>
    IGameLocale CurrentLocale { get; }

    /// <summary>All of the currently known locales.</summary>
    IEnumerable<IGameLocale> AllKnownLocales { get; }

    /// <summary>Get a locale built into the vanilla game.</summary>
    /// <remarks>The <see cref="LocalizedContentManager.LanguageCode.mod"/> is not a valid argument for this method, as it is not a real language code.</remarks>
    /// <param name="languageCode">A built-in locale language code.</param>
    IGameLocale GetBuiltInLocale(LocalizedContentManager.LanguageCode languageCode);

    /// <summary>Get a locale provided by a mod.</summary>
    /// <param name="language">A mod-provided language.</param>
    IGameLocale GetModLocale(ModLanguage language);

    #endregion

    #region Localizations

    /// <summary>Get an <see cref="IFluent"/> instance that allows retrieving Project Fluent translations for a specific locale.</summary>
    /// <param name="locale">A locale for which to retrieve the translations.</param>
    /// <param name="mod">The mod for which to retrieve the translations.</param>
    /// <param name="file">An optional file name to retrieve the translations from.</param>
    IFluent<string> GetLocalizations(IGameLocale locale, IManifest mod, string? file = null);

    /// <summary>Get an <see cref="IFluent{}"/> instance that allows retrieving Project Fluent translations for the current locale.</summary>
    /// <remarks>The returned instance's locale will change automatically if the <see cref="CurrentLocale"/> changes.</remarks>
    /// <param name="mod">The mod for which to retrieve the translations.</param>
    /// <param name="file">An optional file name to retrieve the translations from.</param>
    IFluent<string> GetLocalizationsForCurrentLocale(IManifest mod, string? file = null);

    #endregion

    #region Specialized types

    /// <summary>Get a specialized <see cref="IEnumFluent{}"/> instance that allows retrieving Project Fluent translations for an <see cref="Enum"/> type.</summary>
    /// <typeparam name="EnumType">The <see cref="Enum"/> type to retrieve translations for.</typeparam>
    /// <param name="baseFluent">The underlying <see cref="IFluent{}"/> instance.</param>
    /// <param name="keyPrefix">The prefix all of the <see cref="Enum"/> values are prefixed with.</param>
    IEnumFluent<EnumType> GetEnumFluent<EnumType>(IFluent<string> baseFluent, string keyPrefix)
        where EnumType : struct, Enum;

    /// <summary>Get a specialized <see cref="IFluent{}"/> instance that allows retrieving Project Fluent translations for values of a given generic type Input.</summary>
    /// <typeparam name="T">The type to retrieve translations for.</typeparam>
    /// <param name="baseFluent">The underlying <see cref="IFluent{}"/> instance.</param>
    /// <param name="mapper">The (Input -> Output) mapper.</param>
    IFluent<Input> GetMappingFluent<Input, Output>(IFluent<Output> baseFluent, Func<Input, Output> mapper);

    #endregion
}
#pragma warning restore SA1314 // Type parameter names should begin with T
#pragma warning restore SA1124 // Do not use regions
#pragma warning restore SA1623 // Property summary documentation should match accessors