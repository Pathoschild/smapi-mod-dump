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

#pragma warning disable SA1615 // Element return value should be documented - copied from API.
#pragma warning disable SA1201 // Elements should appear in the correct order

/// <summary>A type allowing access to Project Fluent translations.</summary>
/// <typeparam name="Key">The type of values this instance allows retrieving translations for.</typeparam>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1314:Type parameter names should begin with T", Justification = "Copied from Shockah.")]
public interface IFluent<Key>
{
    /// <summary>Returns whether a given key has a translation provided.</summary>
    /// <param name="key">The key to retrieve a translation for.</param>
    bool ContainsKey(Key key);

    /// <summary>Returns a translation for a given key.</summary>
    /// <param name="key">The key to retrieve a translation for.</param>
    string Get(Key key)
        => this.Get(key, null);

    /// <summary>Returns a translation for a given key.</summary>
    /// <param name="key">The key to retrieve a translation for.</param>
    /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
    string Get(Key key, object? tokens);

    /// <summary>Returns a translation for a given key.</summary>
    /// <param name="key">The key to retrieve a translation for.</param>
    string this[Key key]
        => this.Get(key, null);
}
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1615 // Element return value should be documented