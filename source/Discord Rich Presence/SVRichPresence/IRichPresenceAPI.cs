using Netcode;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace SVRichPresence {
	public interface IRichPresenceAPI {
		/// <summary>
		///	Sets a tag in the registry.
		/// </summary>
		/// <param name="mod">Your mod instance (for identifying your mod)</param>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <param name="value">The value of the tag</param>
		/// <returns>Returns <c>true</c> if the tag was set or <c>false</c> if the tag is owned by another mod.</returns>
		bool SetTag(Mod mod, string key, string value);
		bool SetTag(Mod mod, string key, NetString value);
		bool SetTag(Mod mod, string key, int value);
		bool SetTag(Mod mod, string key, decimal value, int roundDigits = -1);
		bool SetTag(Mod mod, string key, double value, int roundDigits = -1);

		/// <summary>
		///	Sets a dynamically generated tag in the registry.
		/// </summary>
		/// <param name="mod">Your mod instance (for identifying your mod)</param>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <param name="resolver">A function that returns the value of the tag</param>
		/// <returns>Returns <c>true</c> if the tag was set or <c>false</c> if the tag is owned by another mod.</returns>
		bool SetTag(Mod mod, string key, Func<string> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(Mod mod, string key, Func<NetString> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(Mod mod, string key, Func<int> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(Mod mod, string key, Func<decimal> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false);
		bool SetTag(Mod mod, string key, Func<double> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false);

		/// <summary>
		/// Removes a tag from the registry.
		/// </summary>
		/// <param name="mod">Your mod instance (for identifying your mod)</param>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns <c>true</c> if the value was removed (or already didn't exist). Returns <c>false</c> if the tag is owned by another mod and can't be removed.</returns>
		bool RemoveTag(Mod mod, string key);

		/// <summary>
		/// Gets the value of a tag.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns the value of the tag or null if the resolver threw an exception</returns>
		string GetTag(string key);

		/// <summary>
		/// Gets the value of a tag.
		/// Unlike <see cref="GetTag"/>, this method will throw an exception if the resolver throws one.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns the value of the tag</returns>
		/// <exception cref="Exception">Throws whatever exception the resolver throws</exception>
		string GetTagThrow(string key);

		/// <summary>
		/// Returns if a tag exists.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns <c>true</c> if the tag exists</returns>
		bool TagExists(string key);

		/// <summary>
		/// Returns the owner of a tag.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns the unique mod ID of the mod that owns the tag or <c>null</c> if the tag doesn't exist.</returns>
		string GetTagOwner(string key);

		/// <summary>
		/// Lists the available tags.
		/// </summary>
		/// <param name="replaceNull">Optional string to replace null values with</param>
		/// <param name="replaceException">Optional string to replace tags with exceptions with</param>
		/// <param name="removeNull">Set to true to remove all null values (applied after <paramref name="removeNull"/> and <paramref name="replaceException"/>)</param>
		/// <returns>A dictionary with the names as the keys and the resolved values as the values.</returns>
		IDictionary<string, string> ListTags(string replaceNull = null, string replaceException = null, bool removeNull = true);

		/// <summary>
		/// Returns a helper class that allows easier usage of methods in the API.
		/// </summary>
		/// <param name="mod">Your mod instance (for identifying your mod)</param>
		/// <returns>An ITagRegister class for your mod</returns>
		ITagRegister GetTagRegister(Mod mod);

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
		/// <returns>The formatted text</returns>
		string FormatText(string text);
	}

	public interface ITagRegister {
		/// <summary>
		///	Sets a tag in the registry.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <param name="value">The value of the tag</param>
		/// <returns>Returns <c>true</c> if the tag was set or <c>false</c> if the tag is owned by another mod.</returns>
		bool SetTag(string key, string value);
		bool SetTag(string key, NetString value);
		bool SetTag(string key, int value);
		bool SetTag(string key, decimal value, int roundDigits = -1);
		bool SetTag(string key, double value, int roundDigits = -1);

		/// <summary>
		///	Sets a dynamically generated tag in the registry.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <param name="resolver">A function that returns the value of the tag</param>
		/// <param name="onlyWhenWorldReady">Only resolve if <see cref="Context.IsWorldReady"/></param>
		/// <returns>Returns <c>true</c> if the tag was set or <c>false</c> if the tag is owned by another mod.</returns>
		bool SetTag(string key, Func<string> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(string key, Func<NetString> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(string key, Func<int> resolver, bool onlyWhenWorldReady = false);
		bool SetTag(string key, Func<decimal> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false);
		bool SetTag(string key, Func<double> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false);

		/// <summary>
		/// Removes a tag from the registry.
		/// </summary>
		/// <param name="key">The name of the tag (case-insensitive)</param>
		/// <returns>Returns <c>true</c> if the value was removed (or already didn't exist). Returns <c>false</c> if the tag is owned by another mod and can't be removed.</returns>
		bool RemoveTag(string key);
	}
}
