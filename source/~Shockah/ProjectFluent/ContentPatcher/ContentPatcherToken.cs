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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Shockah.ProjectFluent.ContentPatcher
{
	internal class ContentPatcherToken
	{
		private readonly IManifest Mod;
		private readonly IDictionary<string, IFluent<string>> Fluents = new Dictionary<string, IFluent<string>>();
		private IFluent<string>? DefaultFluent;

		private bool IsUpdated = false;

		public ContentPatcherToken(IManifest mod)
		{
			this.Mod = mod;
		}

		public bool IsReady() => true;

		public bool AllowsInput() => true;

		public bool RequiresInput() => true;

		public bool CanHaveMultipleValues(string? input = null) => false;

		public bool UpdateContext()
		{
			var wasUpdated = IsUpdated;
			IsUpdated = true;
			return !wasUpdated;
		}

		public IEnumerable<string> GetValues(string input)
		{
			var args = ParseArgs(input);
			if (args.Named.TryGetValue("file", out string? localizationsFile))
				args.Named.Remove("file");
			else
				localizationsFile = null;

			yield return ObtainFluent(localizationsFile).Get(args.Key, args.Named);
		}

		private Args ParseArgs(string input)
		{
			if (!input.Contains("|"))
				return new Args(input);

			var key = input[0..^input.IndexOf('|')];
			var named = new Dictionary<string, string>();
			var argSplit = input[input.IndexOf('|')..].Split('|').Select(s => s.Trim());
			foreach (var wholeArg in argSplit)
			{
				var split = wholeArg.Split('=');
				var argName = split[0].Trim();
				var argValue = split[1].Trim();
				named[argName] = argValue;
			}
			return new Args(key, named);
		}

		private bool TryGetFluent(string? file, [NotNullWhen(true)] out IFluent<string>? fluent)
		{
			if (file is null)
			{
				if (DefaultFluent is not null)
				{
					fluent = DefaultFluent;
					return true;
				}
			}
			else
			{
				return Fluents.TryGetValue(file, out fluent);
			}

			fluent = null;
			return false;
		}

		private IFluent<string> ObtainFluent(string? file)
		{
			if (TryGetFluent(file, out IFluent<string>? fluent))
				return fluent;
			fluent = ProjectFluent.Instance.Api.GetLocalizationsForCurrentLocale(Mod, file);

			if (file is null)
				DefaultFluent = fluent;
			else
				Fluents[file] = fluent;
			return fluent;
		}

		internal struct Args
		{
			internal string Key;
			internal Dictionary<string, string> Named;

			public Args(string key) : this(key, new Dictionary<string, string>()) { }

			public Args(string key, Dictionary<string, string> named)
			{
				this.Key = key;
				this.Named = named;
			}
		}
	}
}