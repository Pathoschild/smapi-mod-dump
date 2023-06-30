/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace Shockah.Kokoro.SMAPI;

public interface ITranslationSet<Key>
{
	bool ContainsKey(Key key);
	string Get(Key key);
	string Get(Key key, object? tokens);
}

public sealed class SMAPITranslationSetWrapper : ITranslationSet<string>
{
	private ITranslationHelper Helper { get; set; }

	public SMAPITranslationSetWrapper(ITranslationHelper helper)
	{
		this.Helper = helper;
	}

	public bool ContainsKey(string key)
		=> Helper.Get(key).HasValue();

	public string Get(string key)
		=> Helper.Get(key);

	public string Get(string key, object? tokens)
		=> Helper.Get(key, tokens);
}