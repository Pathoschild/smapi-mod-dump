/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

using StardewValley;
using StardewValley.BellsAndWhistles;

using StardewModdingAPI;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.Types;
using Microsoft.Xna.Framework.Graphics;
using Leclair.Stardew.ThemeManager.Serialization;
using Leclair.Stardew.ThemeManager.VariableSets;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Leclair.Stardew.ThemeManager.Models;

public class GameTheme : IGameTheme {

	#region Processing Related Fields

	[JsonIgnore]
	private IThemeManifest? _Manifest;

	[JsonIgnore]
	internal bool Processing = false;

	[JsonIgnore]
	private Dictionary<int, Color>? _IndexedSpriteTextColors;
	[JsonIgnore]
	private Dictionary<int, string>? _InheritedIndexedSpriteTextColors;

	[JsonIgnore]
	private Dictionary<string, Dictionary<long, Color?>>? _SpriteTextColorSets;
	[JsonIgnore]
	private Dictionary<string, Dictionary<string, string?>>? _InheritedSpriteTextColorSets;

	[JsonIgnore]
	private List<string>? _Patches;
	[JsonIgnore]
	private List<string>? _InheritedPatches;
	[JsonIgnore]
	private Dictionary<string, string>? _PatchColorVariables;
	private Dictionary<string, string>? _PatchFontVariables;
	private Dictionary<string, string>? _PatchBmFontVariables;
	private Dictionary<string, string>? _PatchTextureVariables;

	#endregion

	#region Construction

	internal static string ColorToString(Color color) {
		return $"{color.R},{color.G},{color.B},{color.A}";
	}

	/// <summary>
	/// Construct a new <see cref="GameTheme"/> instance using the game's
	/// default values. This should be done when the game is first started,
	/// before any mods (or we ourselves) would have changed these values.
	/// </summary>
	internal static GameTheme GetDefaultTheme() {
		var theme = new GameTheme();

		Dictionary<string, string> rawVariables = new();

		rawVariables["Text"] = ColorToString(Game1.textColor);
		rawVariables["TextShadow"] = ColorToString(Game1.textShadowColor);
		rawVariables["UnselectedOption"] = ColorToString(Game1.unselectedOptionColor);
		rawVariables["TextShadowAlt"] = ColorToString(Game1.textShadowDarkerColor);

		rawVariables["ErrorText"] = "Red";
		rawVariables["Hover"] = "Wheat";
		rawVariables["ButtonHover"] = "LightPink";

		theme.ColorVariables.RawValues = rawVariables;

		theme.RawIndexedSpriteTextColors ??= new();
		for (int i = 0; i <= 8; i++) {
			var color = SpriteText.getColorFromIndex(i);
			theme.IndexedSpriteTextColors[i] = color;
			theme.RawIndexedSpriteTextColors[i] = ColorToString(color);
		}

		return theme;
	}

	#endregion

	/// <inheritdoc />
	public Color? GetColorVariable(string key) {
		if (! string.IsNullOrEmpty(key) && ColorVariables.TryGetValue(key.StartsWith('$') ? key[1..] : key, out var color))
			return color;
		return null;
	}

	/// <inheritdoc />
	public IManagedAsset<SpriteFont>? GetManagedFontVariable(string key) {
		if (!string.IsNullOrEmpty(key) && FontVariables.TryGetValue(key.StartsWith('$') ? key[1..] : key, out var font))
			return font;
		return null;
	}

	/// <inheritdoc />
	public SpriteFont? GetFontVariable(string key) {
		return GetManagedFontVariable(key)?.Value;
	}

	/// <inheritdoc />
	public IManagedAsset<Texture2D>? GetManagedTextureVariable(string key) {
		if (!string.IsNullOrEmpty(key) && TextureVariables.TryGetValue(key.StartsWith('$') ? key[1..] : key, out var texture))
			return texture;
		return null;
	}

	/// <inheritdoc />
	public Texture2D? GetTextureVariable(string key) {
		return GetManagedTextureVariable(key)?.Value;
	}

	/// <inheritdoc />
	public IManagedAsset<IBmFontData>? GetManagedBmFontVariable(string key) {
		if (!string.IsNullOrEmpty(key) && BmFontVariables.TryGetValue(key.StartsWith('$') ? key[1..] : key, out var font))
			return font;
		return null;
	}

	/// <inheritdoc />
	public IBmFontData? GetBmFontVariable(string key) {
		return GetManagedBmFontVariable(key)?.Value;
	}

	#region Base Data

	[JsonIgnore]
	public IThemeManifest? Manifest {
		get {
			return _Manifest;
		}
		set {
			if (_Manifest != value) {
				string? old_fallback = _Manifest?.FallbackTheme;
				_Manifest = value;

				if (_Manifest?.FallbackTheme != old_fallback) {
					_InheritedIndexedSpriteTextColors = null;
					_InheritedSpriteTextColorSets = null;
					_InheritedPatches = null;
					_Patches = null;
					ResetPatchVariables();
				}
			}
		}
	}

	/// <inheritdoc />
	[JsonConverter(typeof(RealVariableSetConverter))]
	public IVariableSet<Color> ColorVariables { get; set; } = new ColorVariableSet();

	/// <inheritdoc />
	[JsonConverter(typeof(RealVariableSetConverter))]
	public IVariableSet<IManagedAsset<SpriteFont>> FontVariables { get; set; } = new FontVariableSet();

	/// <inheritdoc />
	[JsonConverter(typeof(RealVariableSetConverter))]
	public IVariableSet<IManagedAsset<Texture2D>> TextureVariables { get; set; } = new TextureVariableSet();

	/// <inheritdoc />
	[JsonConverter(typeof(RealVariableSetConverter))]
	public IVariableSet<IManagedAsset<IBmFontData>> BmFontVariables { get; set; } = new BmFontVariableSet();

	/// <summary>
	/// The raw sprite text color sets, read from the underlying JSON object.
	/// You should use <see cref="SpriteTextColorSets"/> rather than this.
	/// </summary>
	[JsonProperty("SpriteTextColorSets")]
	public Dictionary<string, Dictionary<string, string?>>? RawSpriteTextColorSets { get; set; }

	/// <summary>
	/// The raw sprite text colors read from the underlying JSON object.
	/// You should use <see cref="IndexedSpriteTextColors"/> rather than this.
	/// </summary>
	[JsonProperty(nameof(IndexedSpriteTextColors))]
	public Dictionary<int, string>? RawIndexedSpriteTextColors { get; set; }

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
	internal Dictionary<string, Dictionary<string, string?>> InheritedSpriteTextColorSets {
		get {
			if (_InheritedSpriteTextColorSets is not null)
				return _InheritedSpriteTextColorSets;

			Dictionary<string, Dictionary<string, string?>> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.GameThemeManager!.GetTheme(Manifest.FallbackTheme) is GameTheme other && !other.Processing) {
				result = RawSpriteTextColorSets is not null ? new(RawSpriteTextColorSets) : new();
				foreach (var entry in other.InheritedSpriteTextColorSets) {
					if (result.TryGetValue(entry.Key, out var existing)) { 
						foreach(var pair in entry.Value)
							existing.TryAdd(pair.Key, pair.Value);
					} else
						result.TryAdd(entry.Key, entry.Value);
				}
			} else
				result = RawSpriteTextColorSets ?? new();

			_InheritedSpriteTextColorSets = result;
			Processing = false;
			return _InheritedSpriteTextColorSets;
		}
	}

	[JsonIgnore]
	internal Dictionary<int, string> InheritedSpriteTextColors {
		get {
			if (_InheritedIndexedSpriteTextColors is not null)
				return _InheritedIndexedSpriteTextColors;

			Dictionary<int, string> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.GameThemeManager!.GetTheme(Manifest.FallbackTheme) is GameTheme other && !other.Processing) {
				result = RawIndexedSpriteTextColors is not null ? new(RawIndexedSpriteTextColors) : new();
				foreach (var entry in other.InheritedSpriteTextColors)
					result.TryAdd(entry.Key, entry.Value);
			} else
				result = RawIndexedSpriteTextColors ?? new();

			_InheritedIndexedSpriteTextColors = result;
			Processing = false;
			return _InheritedIndexedSpriteTextColors;
		}
	}

	[JsonIgnore]
	internal List<string> InheritedPatches {
		get {
			if (_InheritedPatches is not null)
				return _InheritedPatches;

			List<string> result;
			Processing = true;

			if (!string.IsNullOrEmpty(Manifest?.FallbackTheme) && ModEntry.Instance.GameThemeManager!.GetTheme(Manifest.FallbackTheme) is GameTheme other && !other.Processing) {
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
	public Dictionary<string, Dictionary<long, Color?>> SpriteTextColorSets {
		get {
			if (_SpriteTextColorSets is not null)
				return _SpriteTextColorSets;

			var result = new Dictionary<string, Dictionary<long, Color?>>();

			foreach(var entry in InheritedSpriteTextColorSets) {
				var subresult = new Dictionary<long, Color?>();
				foreach(var pair in entry.Value) {
					long key;
					if (pair.Key is null || pair.Key.Equals("null", System.StringComparison.OrdinalIgnoreCase) || pair.Key.Equals("default", System.StringComparison.OrdinalIgnoreCase))
						key = -1;
					else if (CommonHelper.TryParseColor(pair.Key, out Color? ckey))
						key = ckey.Value.PackedValue;
					else {
						ModEntry.Instance.Log($"Unable to parse color for theme {Manifest?.UniqueID}: {pair.Key}", LogLevel.Warn);
						continue;
					}

					if (pair.Value is null || pair.Value.Equals("null", System.StringComparison.OrdinalIgnoreCase))
						subresult[key] = null;
					else if (pair.Value.StartsWith('$')) {
						if (ColorVariables.TryGetValue(pair.Value[1..], out var color))
							subresult[key] = color;
					} else if (CommonHelper.TryParseColor(pair.Value, out var color))
						subresult[key] = color.Value;
					else
						ModEntry.Instance.Log($"Unable to parse color for theme {Manifest?.UniqueID}: {pair.Value}", LogLevel.Warn);
				}

				if (subresult.Count > 0)
					result.Add(entry.Key, subresult);
			}

			_SpriteTextColorSets = result;
			return result;
		}
	}

	/// <inheritdoc />
	[JsonIgnore]
	public Dictionary<int, Color> IndexedSpriteTextColors {
		get {
			if (_IndexedSpriteTextColors is not null)
				return _IndexedSpriteTextColors;

			var result = new Dictionary<int, Color>();

			foreach(var entry in InheritedSpriteTextColors) {
				if (entry.Value.StartsWith('$')) {
					if (ColorVariables.TryGetValue(entry.Value[1..], out var color))
						result[entry.Key] = color;
				} else if (CommonHelper.TryParseColor(entry.Value, out var color))
					result[entry.Key] = color.Value;
				else
					ModEntry.Instance.Log($"Unable to parse color for theme {Manifest?.UniqueID}: {entry.Value}", LogLevel.Warn);
			}

			_IndexedSpriteTextColors = result;
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

	internal void ResetPatchVariables() {
		_PatchColorVariables = null;
		_PatchFontVariables = null;
		_PatchBmFontVariables = null;
		_PatchTextureVariables = null;
	}

	internal Dictionary<string, string> PatchColorVariables {
		get {
			if (_PatchColorVariables is not null)
				return _PatchColorVariables;

			var result = new CaseInsensitiveDictionary<string>();

			if (Patches.Count > 0) {
				ModEntry.Instance.LoadPatchGroups();

				foreach (string patch in Patches) {
					if (ModEntry.Instance.PatchGroups.TryGetValue(patch, out var group)) {
						if (group.CanUse && group.ColorVariables is not null) {
							foreach (var entry in group.ColorVariables) {
								// Make sure to strip the "$" from the start of
								// variable keys.
								result.TryAdd(
									entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
									entry.Value
								);
							}
						}
					}
				}
			}

			_PatchColorVariables = result;
			return result;
		}
	}

	internal Dictionary<string, string> PatchFontVariables {
		get {
			if (_PatchFontVariables is not null)
				return _PatchFontVariables;

			var result = new CaseInsensitiveDictionary<string>();

			if (Patches.Count > 0) {
				ModEntry.Instance.LoadPatchGroups();

				foreach (string patch in Patches) {
					if (ModEntry.Instance.PatchGroups.TryGetValue(patch, out var group)) {
						if (group.CanUse && group.FontVariables is not null) {
							foreach (var entry in group.FontVariables) {
								// Make sure to strip the "$" from the start of
								// variable keys.
								result.TryAdd(
									entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
									entry.Value
								);
							}
						}
					}
				}
			}

			_PatchFontVariables = result;
			return result;
		}
	}

	internal Dictionary<string, string> PatchBmFontVariables {
		get {
			if (_PatchBmFontVariables is not null)
				return _PatchBmFontVariables;

			var result = new CaseInsensitiveDictionary<string>();

			if (Patches.Count > 0) {
				ModEntry.Instance.LoadPatchGroups();

				foreach (string patch in Patches) {
					if (ModEntry.Instance.PatchGroups.TryGetValue(patch, out var group)) {
						if (group.CanUse && group.BmFontVariables is not null) {
							foreach (var entry in group.BmFontVariables) {
								// Make sure to strip the "$" from the start of
								// variable keys.
								result.TryAdd(
									entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
									entry.Value
								);
							}
						}
					}
				}
			}

			_PatchBmFontVariables = result;
			return result;
		}
	}

	internal Dictionary<string, string> PatchTextureVariables {
		get {
			if (_PatchTextureVariables is not null)
				return _PatchTextureVariables;

			var result = new CaseInsensitiveDictionary<string>();

			if (Patches.Count > 0) {
				ModEntry.Instance.LoadPatchGroups();

				foreach (string patch in Patches) {
					if (ModEntry.Instance.PatchGroups.TryGetValue(patch, out var group)) {
						if (group.CanUse && group.TextureVariables is not null) {
							foreach (var entry in group.TextureVariables) {
								// Make sure to strip the "$" from the start of
								// variable keys.
								result.TryAdd(
									entry.Key.StartsWith('$') ? entry.Key[1..] : entry.Key,
									entry.Value
								);
							}
						}
					}
				}
			}

			_PatchTextureVariables = result;
			return result;
		}
	}

	#endregion

	#endregion
}
