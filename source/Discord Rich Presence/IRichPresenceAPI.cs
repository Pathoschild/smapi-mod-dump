/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RuiNtD/SVRichPresence
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace SVRichPresence
{
  public interface IRichPresenceAPI
  {
    /// <summary>
    ///	Defines a tag with a resolver function.
    /// </summary>
    /// <param name="mod">Your mod manifest</param>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <param name="func">A function that returns the value of the tag</param>
    /// <returns><c>true</c> if the tag was set or <c>false</c> if the tag is owned by another mod.</returns>
    bool SetTag(IManifest mod, string key, Func<string> func);

    /// <summary>
    /// Removes a tag.
    /// </summary>
    /// <param name="mod">Your mod manifest</param>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <returns>Returns <c>true</c> if the tag was removed or doesn't exist. Returns <c>false</c> if the tag is owned by another mod.</returns>
    bool RemoveTag(IManifest mod, string key);

    /// <summary>
    /// Attempts to resolve a tag.
    /// </summary>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <returns>An instance of <see cref="IResolvedTag"/> or <c>null</c> if the tag doesn't exist.</returns>
    IResolvedTag ResolveTag(string key);

    /// <summary>
    /// Gets the resolved value of a tag.
    /// </summary>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <returns>
    ///   The value of the tag.
    ///   If the value is null, returns <paramref name="replaceNull"/>.
    ///   If the tag throws an exception, returns <paramref name="replaceError"/>.
    /// </returns>
    string FormatTag(string key, string replaceError = null, string replaceNull = null);

    /// <summary>
    /// Returns if a tag exists.
    /// </summary>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <returns><c>true</c> if the tag exists</returns>
    bool TagExists(string key);

    /// <summary>
    /// Returns the <see cref="IManifest.UniqueID"/> of the mod that set a tag.
    /// </summary>
    /// <param name="key">The name of the tag (case-insensitive)</param>
    /// <returns>The <see cref="IManifest.UniqueID"/> of the mod that owns the tag or <c>null</c> if the tag doesn't exist.</returns>
    string GetTagOwner(string key);

    /// <summary>
    /// Lists all tags.
    /// </summary>
    /// <returns>A dictionary with <see cref="IResolvedTag"/>s as the values.</returns>
    IDictionary<string, IResolvedTag> ResolveAllTags();

    /// <summary>
    /// Lists all tags.
    /// </summary>
    /// <param name="replaceErrors">Text to replace tags that throw an exception. If <c>null</c>, the tag is omitted.</param>
    /// <param name="replaceNulls">Text to replace tags that return null. If <c>null</c>, the tag is omitted.</param>
    /// <returns>A dictionary using <see cref="FormatTag"/> for each value.</returns>
    IDictionary<string, string> FormatAllTags(
      string replaceErrors = null,
      string replaceNulls = null
    );

    public interface IResolvedTag
    {
      bool Success { get; }
      string Value { get; }
      Exception Exception { get; }
    }

    /// <summary>
    /// A string saying "None" that will be translated based on the user's language.
    /// </summary>
    string None { get; }

    /// <summary>
    /// A reference to Stardew Valley's internal presence string. This value is used for <c>{{ Activity }}</c>.
    /// </summary>
    string GamePresence { get; set; }

    /// <summary>
    /// Formats text using the tags in the registry.
    /// </summary>
    /// <param name="text">The text to register</param>
    /// <param name="replaceError">Text to replace tags that throw an exception. If <c>null</c>, the tag is untouched.</param>
    /// <param name="replaceNull">Text to replace tags that return null. If <c>null</c>, the tag is untouched.</param>
    /// <returns>The formatted text</returns>
    string FormatText(string text, string replaceError = null, string replaceNull = null);
  }
}
