/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.BellsAndWhistles;
using Newtonsoft.Json;
using Leclair.Stardew.Common.UI;
using System.Text;

using StardewModdingAPI;
using System.Windows.Markup;
using Leclair.Stardew.Common;

namespace Leclair.Stardew.ThemeManager.Models;

public class BaseTheme : IBaseTheme {

	#region Processing Related Fields

	[JsonIgnore]
	internal bool Processing = false;

	[JsonIgnore]
	private Dictionary<string, Color>? _Variables;
	[JsonIgnore]
	private Dictionary<int, Color>? _SpriteTextColors;
	[JsonIgnore]
	private List<string>? _Patches;
	[JsonIgnore]
	private CaseInsensitiveDictionary<string>? _InheritedVariables;
	[JsonIgnore]
	private Dictionary<int, string>? _InheritedSpriteTextColors;
	[JsonIgnore]
	private List<string>? _InheritedPatches;

	#endregion

	#region Construction

	/// <summary>
	/// Construct a new <see cref="BaseTheme"/> instance using the game's
	/// default values. This should be done when the game is first started,
	/// before any mods (or we ourselves) would have changed these values.
	/// </summary>
	internal static BaseTheme GetDefaultTheme() {
		var theme = new BaseTheme();

		theme.RawVariables ??= new();
		theme.RawSpriteTextColors ??= new();

		theme.Variables["Text"] = Game1.textColor;
		theme.Variables["TextShadow"] = Game1.textShadowColor;
		theme.Variables["UnselectedOption"] = Game1.unselectedOptionColor;
		theme.Variables["TextShadowAlt"] = new Color(221, 148, 84);

		theme.Variables["ErrorText"] = Color.Red;
		theme.Variables["Hover"] = Color.Wheat;
		theme.Variables["ButtonHover"] = Color.LightPink;

		foreach (var entry in theme.Variables)
			theme.RawVariables[entry.Key] = entry.Value.ToString();

		for (int i = -1; i <= 8; i++) {
			var color = SpriteText.getColorFromIndex(i);
			theme.SpriteTextColors[i] = color;
			theme.RawSpriteTextColors[i] = color.ToString();
		}

		return theme;
	}

	#endregion

	public Color? GetVariable(string key) {
		if (Variables.TryGetValue(key, out var color))
			return color;
		return null;
	}

	#region Base Data

	[JsonIgnore]
	public IThemeManifest Manifest { get; set; } = null!;

	/// <summary>
	/// The raw variables read from the underlying JSON object. You should
	/// use <see cref="Variables"/> rather than this.
	/// </summary>
	[JsonProperty("Variables")]
	public CaseInsensitiveDictionary<string>? RawVariables { get; set; }

	/// <summary>
	/// The raw sprite text colors read from the underlying JSON object.
	/// You should use <see cref="SpriteTextColors"/> rather than this.
	/// </summary>
	[JsonProperty("SpriteTextColors")]
	public Dictionary<int, string>? RawSpriteTextColors { get; set; }

	/// <summary>
	/// The raw list of patches read from the underlying JSON object.
	/// You should use <see cref="Patches"/> rather than this.
	/// </summary>
	[JsonProperty("Patches")]
	public List<string>? RawPatches { get; set; }

	#endregion

	#region DayTimeMoneyBox Clock Positioning

	public Alignment? DayTimeAlignment { get; set; }
	public int? DayTimeOffsetX { get; set; }
	public int? DayTimeOffsetY { get; set; }

	#endregion

	#region Processing

	#region Inherited Values

	[JsonIgnore]
	internal CaseInsensitiveDictionary<string> InheritedVariables {
		get {
			if (_InheritedVariables is not null)
				return _InheritedVariables;

			CaseInsensitiveDictionary<string> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.BaseThemeManager!.TryGetTheme(Manifest.FallbackTheme, out var other) && !other.Processing)
				result = new(other.InheritedVariables);
			else
				result = new();

			if (RawVariables is not null)
				foreach (var entry in RawVariables)
					result[entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key] = entry.Value;

			_InheritedVariables = result;
			Processing = false;
			return _InheritedVariables;
		}
	}

	[JsonIgnore]
	internal Dictionary<int, string> InheritedSpriteTextColors {
		get {
			if (_InheritedSpriteTextColors is not null)
				return _InheritedSpriteTextColors;

			Dictionary<int, string> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.BaseThemeManager!.TryGetTheme(Manifest.FallbackTheme, out var other) && !other.Processing) {
				result = RawSpriteTextColors is not null ? new(RawSpriteTextColors) : new();
				foreach (var entry in other.InheritedSpriteTextColors)
					result.TryAdd(entry.Key, entry.Value);
			} else
				result = RawSpriteTextColors ?? new();

			_InheritedSpriteTextColors = result;
			Processing = false;
			return _InheritedSpriteTextColors;
		}
	}

	[JsonIgnore]
	internal List<string> InheritedPatches {
		get {
			if (_InheritedPatches is not null)
				return _InheritedPatches;

			List<string> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.BaseThemeManager!.TryGetTheme(Manifest.FallbackTheme, out var other) && !other.Processing) {
				result = new(other.InheritedPatches);
				if (RawPatches is not null)
					foreach (string entry in RawPatches)
						result.Add(entry);
			} else
				result = RawPatches ?? new();

			_InheritedPatches = result;
			Processing = false;
			return _InheritedPatches;
		}
	}

	#endregion

	#region Value Hydration

	/// <inheritdoc />
	[JsonIgnore]
	public Dictionary<string, Color> Variables {
		get {
			if (_Variables is not null)
				return _Variables;

			var result = new CaseInsensitiveDictionary<Color>();

			CaseInsensitiveDictionary<string> raw;

			// Apply patches if we have any.
			if (Patches.Count > 0) {
				raw = new(InheritedVariables);
				ModEntry.Instance.LoadPatchGroups();

				foreach(string patch in Patches) {
					if (ModEntry.Instance.PatchGroups.TryGetValue(patch, out var group)) {
						if (group.CanUse && group.Variables is not null) {
							foreach(var entry in group.Variables) {
								// Make sure to strip the "$" from the start of
								// variable keys.
								raw.TryAdd(
									entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
									entry.Value
								);
							}
						}
					}
				}

			} else
				raw = InheritedVariables;
			
			// Now, resolve every color.
			foreach(var entry in raw) {
				Color? color = ResolveColor(entry.Value, raw, result, Manifest);
				if (color.HasValue)
					result[entry.Key] = color.Value;
			}

			_Variables = result;
			return result;
		}
	}

	private static Color? ResolveColor(string input, Dictionary<string, string> sources, Dictionary<string, Color> processed, IThemeManifest? manifest) {
		CaseInsensitiveHashSet visited = new();
		while(input is not null) {
			if (!visited.Add(input)) {
				ModEntry.Instance.Log($"Infinite loop detected resolving color for theme {manifest?.UniqueID}: {string.Join(" -> ", visited)}", LogLevel.Warn);
				return null;
			}

			if (input.StartsWith('$')) {
				string key = input[1..];
				if (processed.TryGetValue(key, out var result))
					return result;

				if (!sources.TryGetValue(key, out string? value))
					return null;

				input = value;

			} else if (CommonHelper.TryParseColor(input, out var res))
				return res;

			else {
				ModEntry.Instance.Log($"Unable to parse color for theme {manifest?.UniqueID}: {input}", LogLevel.Warn);
				return null;
			}
		}

		return null;
	}

	/// <inheritdoc />
	[JsonIgnore]
	public Dictionary<int, Color> SpriteTextColors {
		get {
			if (_SpriteTextColors is not null)
				return _SpriteTextColors;

			var result = new Dictionary<int, Color>();

			foreach(var entry in InheritedSpriteTextColors) {
				if (entry.Value.StartsWith('$')) {
					if (Variables.TryGetValue(entry.Value[1..], out var color))
						result[entry.Key] = color;
				} else if (CommonHelper.TryParseColor(entry.Value, out var color))
					result[entry.Key] = color.Value;
				else
					ModEntry.Instance.Log($"Unable to parse color for theme {Manifest?.UniqueID}: {entry.Value}", LogLevel.Warn);
			}

			_SpriteTextColors = result;
			return result;
		}
	}

	/// <inheritdoc />
	[JsonIgnore]
	public List<string> Patches {
		get {
			if (_Patches is not null)
				return _Patches;

			var result = new List<string>();

			foreach(string entry in InheritedPatches) {
				if (entry.StartsWith('-'))
					result.Remove(entry[1..]);
				else
					result.Add(entry);
			}

			_Patches = result;
			return result;
		}
	}

	#endregion

	#endregion
}
