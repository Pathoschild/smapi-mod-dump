using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SVRichPresence {
	public class RichPresenceAPI : IRichPresenceAPI {
		private readonly IDictionary<string, Tag> tags = new Dictionary<string, Tag>(StringComparer.InvariantCultureIgnoreCase);
		private readonly RichPresenceMod RPMod;

		public RichPresenceAPI(RichPresenceMod mod) {
			RPMod = mod;
		}

		public bool SetTag(Mod mod, string key, string value) =>
			SetTag(mod, key, () => value);
		public bool SetTag(Mod mod, string key, NetString value) =>
			SetTag(mod, key, () => value);
		public bool SetTag(Mod mod, string key, int value) =>
			SetTag(mod, key, () => value);
		public bool SetTag(Mod mod, string key, decimal value, int roundDigits = -1) =>
			SetTag(mod, key, () => value, roundDigits);
		public bool SetTag(Mod mod, string key, double value, int roundDigits = -1) =>
			SetTag(mod, key, () => value, roundDigits);

		public bool SetTag(Mod mod, string key, Func<string> resolver, bool onlyWhenWorldReady = false) {
			string modID = mod.ModManifest.UniqueID;
			if (TagExists(key) && GetTagOwner(key) != modID)
				return false;
			tags[key] = new Tag {
				owner = modID,
				resolver = () => {
					if (onlyWhenWorldReady && !Context.IsWorldReady)
						return null;
					return resolver.Invoke();
				}
			};
			return true;
		}
		public bool SetTag(Mod mod, string key, Func<NetString> resolver, bool onlyWhenWorldReady = false) =>
			SetTag(mod, key, () => resolver.Invoke().ToString(), onlyWhenWorldReady);
		public bool SetTag(Mod mod, string key, Func<int> resolver, bool onlyWhenWorldReady = false) =>
			SetTag(mod, key, () => resolver.Invoke().ToString(), onlyWhenWorldReady);
		public bool SetTag(Mod mod, string key, Func<decimal> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false) =>
			SetTag(mod, key, () => {
				decimal val = resolver.Invoke();
				if (roundDigits >= 0)
					return Math.Round(val, roundDigits).ToString();
				else return val.ToString();
			}, onlyWhenWorldReady);
		public bool SetTag(Mod mod, string key, Func<double> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false) =>
			SetTag(mod, key, () => {
				double val = resolver.Invoke();
				if (roundDigits >= 0)
					return Math.Round(val, roundDigits).ToString();
				else return val.ToString();
			}, onlyWhenWorldReady);

		public bool RemoveTag(Mod mod, string key) {
			if (!TagExists(key))
				return true;
			if (GetTagOwner(key) != mod.ModManifest.UniqueID)
				return false;
			tags.Remove(key);
			return true;
		}

		public string GetTag(string key) => tags[key]?.Resolve() ?? null;

		public string GetTagThrow(string key) => tags[key]?.resolver.Invoke() ?? null;

		public bool TagExists(string key) => tags.ContainsKey(key);

		public string GetTagOwner(string key) => tags[key]?.owner ?? null;

		public IDictionary<string, string> ListTags(string replaceNull = null, string replaceException = null, bool removeNull = true) {
			IDictionary<string, string> list =
				new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (KeyValuePair<string, Tag> tag in tags) {
				string value;
				try {
					value = tag.Value.resolver.Invoke();
				} catch {
					value = replaceException;
				}
				value = value ?? replaceNull;
				if (value != null || !removeNull)
					list[tag.Key] = value;
			}
			return list;
		}

		public ITagRegister GetTagRegister(Mod mod) => new TagRegister(this, mod);

		public string None => Game1.content.LoadString("Strings\\UI:Character_none");

		public string GamePresence {
			get => RPMod.Helper.Reflection.GetField<string>
				(typeof(Game1), "debugPresenceString").GetValue();
			set => RPMod.Helper.Reflection.GetField<string>
				(typeof(Game1), "debugPresenceString").SetValue(value);
		}

		public string FormatText(string text) {
			if (text.Length == 0)
				return "";

			// Code is copied and modified from SMAPI.
			IDictionary<string, string> tags = ListTags();
			return Regex.Replace(text, @"{{([ \w\.\-]+)}}", match => {
				string key = match.Groups[1].Value.Trim();
				return tags.TryGetValue(key, out string value)
					? value : match.Value;
			});
		}

		private class Tag {
			public string owner;
			public Func<string> resolver;
			public string Resolve() {
				try {
					return resolver.Invoke();
				} catch {
					return null;
				}
			}
		}
	}

	public class TagRegister : ITagRegister {
		private readonly RichPresenceAPI api;
		private readonly Mod mod;

		public TagRegister(RichPresenceAPI api, Mod mod) {
			this.api = api;
			this.mod = mod;
		}

		public bool SetTag(string key, string value) =>
			api.SetTag(mod, key, value);
		public bool SetTag(string key, NetString value) =>
			api.SetTag(mod, key, value);
		public bool SetTag(string key, int value) =>
			api.SetTag(mod, key, value);
		public bool SetTag(string key, decimal value, int roundDigits = -1) =>
			api.SetTag(mod, key, value, roundDigits);
		public bool SetTag(string key, double value, int roundDigits = -1) =>
			api.SetTag(mod, key, value, roundDigits);

		public bool SetTag(string key, Func<string> resolver, bool onlyWhenWorldReady = false) =>
			api.SetTag(mod, key, resolver, onlyWhenWorldReady);
		public bool SetTag(string key, Func<NetString> resolver, bool onlyWhenWorldReady = false) =>
			api.SetTag(mod, key, resolver, onlyWhenWorldReady);
		public bool SetTag(string key, Func<int> resolver, bool onlyWhenWorldReady = false) =>
			api.SetTag(mod, key, resolver, onlyWhenWorldReady);
		public bool SetTag(string key, Func<decimal> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false) =>
			api.SetTag(mod, key, resolver, roundDigits, onlyWhenWorldReady);
		public bool SetTag(string key, Func<double> resolver, int roundDigits = -1, bool onlyWhenWorldReady = false) =>
			api.SetTag(mod, key, resolver, roundDigits, onlyWhenWorldReady);

		public bool RemoveTag(string key) =>
			api.RemoveTag(mod, key);
	}
}
