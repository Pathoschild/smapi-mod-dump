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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Leclair.Stardew.ThemeManager.Models;
using Microsoft.Xna.Framework;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;
using StardewValley;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using StardewValley.BellsAndWhistles;
using static StardewValley.BellsAndWhistles.SpriteText;
using System.Runtime.CompilerServices;
using BmFont;
using Leclair.Stardew.ThemeManager.Managers;

namespace Leclair.Stardew.ThemeManager.Patches;

internal class DynamicPatcher : IDisposable {

	#region Static Fields

	internal static bool DidPatch = false;
	internal static Dictionary<MethodBase, DynamicPatcher> LivePatchers = new();

	private static IReadOnlyDictionary<string, Color>? Colors;
	private static IReadOnlyDictionary<string, Dictionary<long, Color?>>? SpriteTextColors;
	private static IReadOnlyDictionary<string, IManagedAsset<SpriteFont>>? Fonts;
	private static IReadOnlyDictionary<string, IManagedAsset<IBmFontData>>? BmFonts;
	private static IReadOnlyDictionary<string, IManagedAsset<Texture2D>>? Textures;

	#endregion

	#region Static Field Updates

	public static void UpdateColors(IReadOnlyDictionary<string, Color> colors) {
		Colors = colors;
	}

	public static void UpdateSpriteTextColors(IReadOnlyDictionary<string, Dictionary<long, Color?>> colors) {
		SpriteTextColors = colors;
	}

	public static void UpdateFonts(IReadOnlyDictionary<string, IManagedAsset<SpriteFont>> fonts) {
		Fonts = fonts;
	}

	public static void UpdateBmFonts(IReadOnlyDictionary<string, IManagedAsset<IBmFontData>> bmFonts) {
		BmFonts = bmFonts;
	}

	public static void UpdateTextures(IReadOnlyDictionary<string, IManagedAsset<Texture2D>> textures) {
		Textures = textures;
	}

	#endregion

	#region SpriteText Access

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ModifySpriteText(string? spriteTexture, string? coloredTexture, string? font) {
		Texture2D? spriteTex = string.IsNullOrEmpty(spriteTexture) ? null : Textures?.GetValueOrDefault(spriteTexture)?.Value;
		Texture2D? coloredTex = string.IsNullOrEmpty(coloredTexture) ? null : Textures?.GetValueOrDefault(coloredTexture)?.Value;
		IBmFontData? fontData = string.IsNullOrEmpty(font) ? null : BmFonts?.GetValueOrDefault(font)?.Value;

		if (spriteTex != null || coloredTex != null || fontData != null)
			SpriteTextManager.Instance?.ApplyFont(spriteTex, coloredTex, fontData);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ModifySpriteTextColor(string? colorMap, ref Color? color) {
		if (colorMap != null &&
			SpriteTextColors != null &&
			(SpriteTextColors.TryGetValue(colorMap, out Dictionary<long, Color?>? colors) || SpriteTextColors.TryGetValue("*", out colors)) &&
			colors.TryGetValue(color.HasValue ? color.Value.PackedValue : -1, out Color? replaced)
		)
			color = replaced;
	}

	public static void SpriteText_drawStringHorizontallyCenteredAt(SpriteBatch b, string s, int x, int y, int characterPosition, int width, int height, float alpha, float layerDepth, bool junimoText, Color? color, int maxWidth, string? texOne, string? texTwo, string? font, string? colorMap) {
		ModifySpriteText(texOne, texTwo, font);
		ModifySpriteTextColor(colorMap, ref color);
		SpriteText_Patches.UpdateColor = false;

		try {
			SpriteText.drawStringHorizontallyCenteredAt(b, s, x, y, characterPosition, width, height, alpha, layerDepth, junimoText, color, maxWidth);
		} finally {
			SpriteText_Patches.UpdateColor = true;
			SpriteTextManager.Instance?.ApplyNormalFont();
		}
	}

	public static void SpriteText_drawStringWithScrollCenteredAt_Int(SpriteBatch b, string s, int x, int y, int width, float alpha, Color? color, int scrollType, float layerDepth, bool junimoText, string? texOne, string? texTwo, string? font, string? colorMap) {
		ModifySpriteText(texOne, texTwo, font);
		ModifySpriteTextColor(colorMap, ref color);
		SpriteText_Patches.UpdateColor = false;

		try {
			SpriteText.drawStringWithScrollCenteredAt(b, s, x, y, width, alpha, color, scrollType, layerDepth, junimoText);
		} finally {
			SpriteText_Patches.UpdateColor = true;
			SpriteTextManager.Instance?.ApplyNormalFont();
		}
	}

	public static void SpriteText_drawStringWithScrollCenteredAt_String(SpriteBatch b, string s, int x, int y, string placeHolderWidthText, float alpha, Color? color, int scrollType, float layerDepth, bool junimoText, string? texOne, string? texTwo, string? font, string? colorMap) {
		ModifySpriteText(texOne, texTwo, font);
		ModifySpriteTextColor(colorMap, ref color);
		SpriteText_Patches.UpdateColor = false;

		try {
			SpriteText.drawStringWithScrollCenteredAt(b, s, x, y, placeHolderWidthText, alpha, color, scrollType, layerDepth, junimoText);
		} finally {
			SpriteText_Patches.UpdateColor = true;
			SpriteTextManager.Instance?.ApplyNormalFont();
		}
	}

	public static void SpriteText_drawStringWithScrollBackground(SpriteBatch b, string s, int x, int y, string placeHolderWidthText, float alpha, Color? color, ScrollTextAlignment scroll_text_alignment, string? texOne, string? texTwo, string? font, string? colorMap) {
		ModifySpriteText(texOne, texTwo, font);
		ModifySpriteTextColor(colorMap, ref color);
		SpriteText_Patches.UpdateColor = false;

		try {
			SpriteText.drawStringWithScrollBackground(b, s, x, y, placeHolderWidthText, alpha, color, scroll_text_alignment);
		} finally {
			SpriteText_Patches.UpdateColor = true;
			SpriteTextManager.Instance?.ApplyNormalFont();
		}
	}

	public static void SpriteText_drawString(SpriteBatch b, string s, int x, int y, int characterPosition, int width, int height, float alpha, float layerDepth, bool junimoText, int drawBGScroll, string placeHolderScrollWidthText, Color? color, ScrollTextAlignment scroll_text_alignment, string? texOne, string? texTwo, string? font, string? colorMap) {
		ModifySpriteText(texOne, texTwo, font);
		ModifySpriteTextColor(colorMap, ref color);
		SpriteText_Patches.UpdateColor = false;

		try {
			SpriteText.drawString(b, s, x, y, characterPosition, width, height, alpha, layerDepth, junimoText, drawBGScroll, placeHolderScrollWidthText, color, scroll_text_alignment);
		} finally {
			SpriteText_Patches.UpdateColor = true;
			SpriteTextManager.Instance?.ApplyNormalFont();
		}
	}

	#endregion

	#region Texture Access

	public static Texture2D GetTexture(Texture2D @default, string key) {
		return Textures != null && Textures.TryGetValue(key, out var tex) && tex.Value is Texture2D texture ? texture : @default;
	}

	#endregion

	#region Font Access

	public static SpriteFont GetFont(SpriteFont @default, string key) {
		return Fonts != null && Fonts.TryGetValue(key, out var font) && font.Value is SpriteFont sf ? sf: @default;
	}

	#endregion

	#region Color Access

	internal static void DrawTextWithShadowSB(SpriteBatch b, StringBuilder text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3, string? key = null) {
		if (Colors != null && key != null && Colors.TryGetValue(key, out Color val))
			Utility.drawTextWithColoredShadow(
				b: b,
				text: text.ToString(),
				font: font,
				position: position,
				color: color,
				shadowColor: val * shadowIntensity,
				scale: scale,
				layerDepth: layerDepth,
				horizontalShadowOffset: horizontalShadowOffset,
				verticalShadowOffset: verticalShadowOffset,
				numShadows: numShadows
			);
		else
			Utility.drawTextWithShadow(
				b: b,
				text: text,
				font: font,
				position: position,
				color: color,
				scale: scale,
				layerDepth: layerDepth,
				horizontalShadowOffset: horizontalShadowOffset,
				verticalShadowOffset: verticalShadowOffset,
				shadowIntensity: shadowIntensity,
				numShadows: numShadows
			);
	}

	internal static void DrawTextWithShadowStr(SpriteBatch b, string text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3, string? key = null) {
		if (Colors != null && key != null && Colors.TryGetValue(key, out Color val))
			Utility.drawTextWithColoredShadow(
				b: b,
				text: text,
				font: font,
				position: position,
				color: color,
				shadowColor: val * shadowIntensity,
				scale: scale,
				layerDepth: layerDepth,
				horizontalShadowOffset: horizontalShadowOffset,
				verticalShadowOffset: verticalShadowOffset,
				numShadows: numShadows
			);
		else
			Utility.drawTextWithShadow(
				b: b,
				text: text,
				font: font,
				position: position,
				color: color,
				scale: scale,
				layerDepth: layerDepth,
				horizontalShadowOffset: horizontalShadowOffset,
				verticalShadowOffset: verticalShadowOffset,
				shadowIntensity: shadowIntensity,
				numShadows: numShadows
			);
	}

	internal static void DrawTextWithShadowSBPacked(SpriteBatch b, StringBuilder text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3, uint key = 0) {
		Utility.drawTextWithColoredShadow(
			b: b,
			text: text.ToString(),
			font: font,
			position: position,
			color: color,
			shadowColor: new Color(key) * shadowIntensity,
			scale: scale,
			layerDepth: layerDepth,
			horizontalShadowOffset: horizontalShadowOffset,
			verticalShadowOffset: verticalShadowOffset,
			numShadows: numShadows
		);
	}

	internal static void DrawTextWithShadowStrPacked(SpriteBatch b, string text, SpriteFont font, Vector2 position, Color color, float scale = 1f, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 1f, int numShadows = 3, uint key = 0) {
		Utility.drawTextWithColoredShadow(
			b: b,
			text: text,
			font: font,
			position: position,
			color: color,
			shadowColor: new Color(key) * shadowIntensity,
			scale: scale,
			layerDepth: layerDepth,
			horizontalShadowOffset: horizontalShadowOffset,
			verticalShadowOffset: verticalShadowOffset,
			numShadows: numShadows
		);
	}

	internal static Color GetColorPacked(string key, uint @default) {
		return Colors != null && Colors.TryGetValue(key, out Color val) ? val: new Color(@default);
	}

	internal static void GetColorPackedRef(ref Color instance, string key, uint @default) {
		if (Colors != null && Colors.TryGetValue(key, out Color val))
			instance.PackedValue = val.PackedValue;
		else
			instance.PackedValue = @default;
	}

	internal static Color GetColor(Color @default, string key) {
		return Colors != null && Colors.TryGetValue(key, out Color val) ? val : @default;
	}

	internal static Color GetLerpColorPacked(float power, uint left, uint middle, uint right) {
		if (power <= 0.5f)
			return Color.Lerp(new Color(left), new Color(middle), power * 2f);
		else
			return Color.Lerp(new Color(middle), new Color(right), (power - 0.5f) * 2f);
	}

	internal static Color GetLerpColor(float power, string keyLeft, string keyMiddle, string keyRight) {
		if (Colors == null)
			return Utility.getRedToGreenLerpColor(power);

		if (!Colors.TryGetValue(keyLeft, out Color left))
			left = Color.Red;
		if (!Colors.TryGetValue(keyMiddle, out Color middle))
			middle = Color.Yellow;
		if (!Colors.TryGetValue(keyRight, out Color right))
			right = new Color(0, 255, 0);

		if (power <= 0.5f)
			return Color.Lerp(left, middle, power * 2f);
		else
			return Color.Lerp(middle, right, (power - 0.5f) * 2f);
	}

	internal static Lazy<CaseInsensitiveDictionary<(MethodInfo, Color)>> ColorProperties = new(() => {
		var dict = new CaseInsensitiveDictionary<(MethodInfo, Color)>();

		void AddColor(string name, MethodInfo method, Color color) {
			if (!dict.ContainsKey(name))
				dict[name] = (method, color);

			if (name.Contains("Gray")) {
				name = name.Replace("Gray", "Grey");
				if (!dict.ContainsKey(name))
					dict[name] = (method, color);
			}
		}

		foreach(PropertyInfo prop in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			string name = prop.Name;
			if (prop.PropertyType != typeof(Color))
				continue;

			if (prop.GetGetMethod() is not MethodInfo method)
				continue;

			Color? color;
			try {
				color = prop.GetValue(null) as Color?;
			} catch {
				continue;
			}

			if (color.HasValue)
				AddColor(name, method, color.Value);
		}

		foreach (PropertyInfo prop in typeof(SpriteText).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			string name = prop.Name;
			if (prop.PropertyType != typeof(Color))
				continue;

			if (prop.GetGetMethod() is not MethodInfo method)
				continue;

			Color? color;
			try {
				color = prop.GetValue(null) as Color?;
			} catch {
				continue;
			}

			if (name.StartsWith("color_"))
				name = name[6..];

			if (color.HasValue)
				AddColor($"SpriteText:{name}", method, color.Value);
		}


		return dict;
	});

	#endregion

	#region Fields

	private readonly ModEntry Mod;

	public readonly MethodBase Method;
	public readonly string Key;

	private readonly MethodInfo HMethod;

	private PatchData? AppliedChanges;

	private readonly List<PatchData> Patches = new();
	private readonly List<PatchData> AddedPatches = new();
	private readonly List<PatchData> RemovedPatches = new();

	private bool IsDisposed;
	private bool IsPatched;
	private bool IsDirty;

	#endregion

	#region Life Cycle

	public DynamicPatcher(ModEntry mod, MethodBase method, string key) {
		Mod = mod;
		Method = method;
		Key = key;

		HMethod = AccessTools.Method(typeof(DynamicPatcher), nameof(Transpiler));
	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {
			if (disposing) {
				Unpatch();

				AppliedChanges = null;
				Colors = null;
				AddedPatches.Clear();
				RemovedPatches.Clear();
				Patches.Clear();
			}

			IsDisposed = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	#region Updating

	/// <summary>
	/// Remove every current <see cref="PatchData"/> from this patcher. This
	/// will not update the target method until <see cref="Update"/>
	/// is called.
	/// </summary>
	public void ClearPatches() {
		RemovedPatches.Clear();
		RemovedPatches.AddRange(Patches);
	}

	/// <summary>
	/// Add a new <see cref="PatchData"/> instance to this patcher. This will
	/// not update the target method until <see cref="Update()"/>
	/// is called.
	/// </summary>
	/// <param name="data">The <see cref="PatchData"/> to add.</param>
	public void AddPatch(PatchData data) {
		AddedPatches.Add(data);
		RemovedPatches.Remove(data);
	}

	/// <summary>
	/// Remove an old <see cref="PatchData"/> instance from this patcher. This will
	/// not update the target method until <see cref="Update()"/>
	/// is called.
	/// </summary>
	/// <param name="data">The <see cref="PatchData"/> to remove.</param>
	public void RemovePatch(PatchData data) {
		RemovedPatches.Add(data);
		AddedPatches.Remove(data);
	}

	/// <summary>
	/// Update this patcher by processing the changes made using <see cref="AddPatch(PatchData)"/>
	/// and <see cref="RemovePatch(PatchData)"/>, and if necessary cause
	/// Harmony to reapply patches to the target method.
	///
	/// If the patch is not currently applied, this will not re-patch.
	/// </summary>
	/// <param name="colors"></param>
	/// <returns></returns>
	public bool Update() {
		// Update our patches lists as necessary.
		foreach(var entry in AddedPatches) {
			if (!Patches.Contains(entry)) {
				Patches.Add(entry);
				IsDirty = true;
			}
		}

		foreach(var entry in RemovedPatches) {
			if (Patches.Remove(entry))
				IsDirty = true;
		}

		AddedPatches.Clear();
		RemovedPatches.Clear();

		// If the patches list changed, we probably need to reapply our
		// transpiler but just to be safe, first we need to aggregate our
		// patches into a single PatchData.
		if (IsDirty) {
			// Build a new aggregate patch data.
			var applied = new PatchData() {
				Colors = new(),
				RawColors = new(),
				ColorFields = new(),
				FontFields = new(),
				TextureFields = new(),
				SpriteTextDraw = new(),
				DrawTextWithShadow = new(),
				RedToGreenLerp = new()
			};

			foreach (var patch in Patches) {
				if (patch.Colors is not null) {
					foreach (var entry in patch.Colors) {
						if (!applied.Colors.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.Colors[entry.Key] = existing;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.RawColors is not null) {
					foreach (var entry in patch.RawColors) {
						if (!applied.RawColors.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.RawColors[entry.Key] = existing;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.ColorFields is not null) {
					foreach (var entry in patch.ColorFields) {
						if (!applied.ColorFields.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.ColorFields[entry.Key] = entry.Value;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.FontFields is not null) {
					foreach (var entry in patch.FontFields) {
						if (!applied.FontFields.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.FontFields[entry.Key] = entry.Value;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.TextureFields is not null) {
					foreach (var entry in patch.TextureFields) {
						if (!applied.TextureFields.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.TextureFields[entry.Key] = entry.Value;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.SpriteTextDraw is not null) {
					foreach(var entry in patch.SpriteTextDraw)
						applied.SpriteTextDraw[entry.Key] = entry.Value;
				}

				if (patch.DrawTextWithShadow is not null) {
					foreach (var entry in patch.DrawTextWithShadow)
						applied.DrawTextWithShadow[entry.Key] = entry.Value;
				}

				if (patch.RedToGreenLerp is not null) {
					foreach (var entry in patch.RedToGreenLerp)
						applied.RedToGreenLerp[entry.Key] = entry.Value;
				}
			}

			// Now, compare it to our existing applied changes.
			if (applied.Colors.Count == 0 && applied.RawColors.Count == 0 && applied.ColorFields.Count == 0 && applied.FontFields.Count == 0 && applied.TextureFields.Count == 0 && applied.SpriteTextDraw.Count == 0 && applied.DrawTextWithShadow.Count == 0 && applied.RedToGreenLerp.Count == 0)
				applied = null;

			if (applied is null) {
				IsDirty = AppliedChanges is not null;
			} else if (AppliedChanges is not null) {
				IsDirty = false;
				if (!applied.Colors!.ShallowEquals(AppliedChanges.Colors!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.Colors!) {
						if (AppliedChanges.Colors!.TryGetValue(entry.Key, out var existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (! IsDirty && !applied.RawColors!.ShallowEquals(AppliedChanges.RawColors!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.RawColors!) {
						if (AppliedChanges.RawColors!.TryGetValue(entry.Key, out var existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.ColorFields!.ShallowEquals(AppliedChanges.ColorFields!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.ColorFields!) {
						if (AppliedChanges.ColorFields!.TryGetValue(entry.Key, out var existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.FontFields!.ShallowEquals(AppliedChanges.FontFields!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach (var entry in applied.FontFields!) {
						if (AppliedChanges.FontFields!.TryGetValue(entry.Key, out var existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.TextureFields!.ShallowEquals(AppliedChanges.TextureFields!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach (var entry in applied.TextureFields!) {
						if (AppliedChanges.TextureFields!.TryGetValue(entry.Key, out var existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.DrawTextWithShadow!.ShallowEquals(AppliedChanges.DrawTextWithShadow!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.DrawTextWithShadow!) {
						if (AppliedChanges.DrawTextWithShadow!.TryGetValue(entry.Key, out string? existing))
							IsDirty |= string.Equals(entry.Value, existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.SpriteTextDraw!.ShallowEquals(AppliedChanges.SpriteTextDraw!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach (var entry in applied.SpriteTextDraw!) {
						if (AppliedChanges.SpriteTextDraw!.TryGetValue(entry.Key, out string[]? existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
				if (!IsDirty && !applied.RedToGreenLerp!.ShallowEquals(AppliedChanges.RedToGreenLerp!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.RedToGreenLerp!) {
						if (AppliedChanges.RedToGreenLerp!.TryGetValue(entry.Key, out string[]? existing))
							IsDirty |= entry.Value.ShallowEquals(existing);
						else
							IsDirty = true;
					}
				}
			}

			if (IsDirty)
				AppliedChanges = applied;
		}

		// Re-patch if we're dirty.
		if (IsDirty) {
			if (AppliedChanges is null)
				Unpatch();
			else
				Repatch();
		} else
			IsDirty = false;

		// If we don't have any applied changes, this patcher can be
		// garbage collected.
		return AppliedChanges != null;
	}

	#endregion

	#region Patching

	public void Repatch() {
		if (IsDisposed || Mod.Harmony is null)
			return;

		if (! IsPatched || LivePatchers.TryGetValue(Method, out var patcher) && patcher != this) {
			Patch();
			return;
		}

		// We're relying on an implementation detail that will cause harmony to
		// update patches on a method when we call Patch, even if we don't supply
		// a new patch to be applied to the method. In case this ever changes,
		// use a static boolean to make sure our transpiler runs and, if it doesn't
		// then unpatch and repatch entirely to make sure the method gets updated.
		DidPatch = false;

		try {
			Mod.Harmony.Patch(Method);
		} catch(Exception ex) {
			Mod.Log($"There was an error applying harmony patches to {Key}: {ex}", LogLevel.Error);
		}

		if (!DidPatch) {
			Unpatch();
			Patch();
			return;
		}

		IsPatched = true;
		IsDirty = false;
	}

	public void Patch() {
		if (IsPatched || IsDisposed || AppliedChanges is null || Mod.Harmony is null || HMethod is null)
			return;

		if (LivePatchers.TryGetValue(Method, out var patcher) && patcher != this) {
			patcher.Unpatch();
			LivePatchers.Remove(Method);
		}

		LivePatchers.Add(Method, this);

		try {
			Mod.Harmony.Patch(Method, transpiler: new HarmonyMethod(HMethod, priority: Priority.Last));
		} catch(Exception ex) {
			Mod.Log($"There was an error applying harmony patches to {Key}: {ex}", LogLevel.Error);
		}

		IsPatched = true;
		IsDirty = false;
	}

	public void Unpatch() {
		if (!IsPatched || Mod.Harmony is null)
			return;

		Mod.Harmony.Unpatch(Method, HMethod);

		if (LivePatchers.TryGetValue(Method, out var patcher) && patcher == this) {
			LivePatchers.Remove(Method);
		}

		IsPatched = false;
	}

	#endregion

	#region The Actual Patch

	internal static bool TryGetMatch<T>(Dictionary<RuleRange, T> rules, int offset, int hit, [NotNullWhen(true)] out T match) {
		foreach(var entry in rules)
			if (entry.Key.Test(offset, hit)) {
				match = entry.Value;
				return match is not null;
			}

		match = default!;
		return false;
	}

	internal static int HydrateFieldSet(string key, Dictionary<string, Dictionary<string, string>>? source, Dictionary<FieldInfo, Dictionary<RuleRange, string>> values, Dictionary<FieldInfo, Dictionary<RuleRange, Color>>? directValues, Type? current = null) {
		if (source is null)
			return 0;

		int count = 0;

		foreach(var entry in source) {
			bool matched = false;
			foreach (var field in ModEntry.Instance.ResolveMembers<FieldInfo>(entry.Key, current)) {
				if (field.Item2 is null)
					continue;

				matched = true;

				foreach(var ent in entry.Value) {
					if (!RuleRange.TryParse(ent.Key, out var range)) {
						ModEntry.Instance.Log($"Unable to parse rule \"{ent.Key}\" for {entry.Key} while processing {key}", LogLevel.Warn);
						continue;
					}

					if (ent.Value.StartsWith('$')) {
						if (!values.TryGetValue(field.Item2, out var entries)) {
							entries = new();
							values[field.Item2] = entries;
						}
						entries[range] = ent.Value[1..];
						count++;

					} else if (directValues is not null) {
						if (CommonHelper.TryParseColor(ent.Value, out var color)) {
							if (!directValues.TryGetValue(field.Item2, out var entries)) {
								entries = new();
								directValues[field.Item2] = entries;
							}
							entries[range] = color.Value;
							count++;

						} else
							ModEntry.Instance.Log($"Unable to parse color \"{ent.Value}\" for {entry.Key} while processing {key}", LogLevel.Warn);

					} else
						ModEntry.Instance.Log($"Unable to parse value \"{ent.Value}\" for {entry.Key} while processing {key}", LogLevel.Warn);
				}
			}

			if (!matched)
				ModEntry.Instance.Log($"Unable to resolve field \"{entry.Key}\" while processing {key}", LogLevel.Warn);
		}

		return count;
	}


	internal static IEnumerable<CodeInstruction> Transpiler(MethodBase method, IEnumerable<CodeInstruction> instructions) {
		DidPatch = true;

		if (!LivePatchers.TryGetValue(method, out DynamicPatcher? patcher))
			return instructions;

		if (patcher.AppliedChanges is null)
			return instructions;

		int count = 0;

		// We need to keep track of how many times we've encountered any
		// given color, raw color, field, or lerp call.
		Dictionary<MethodInfo, int> HitColors = new();
		Dictionary<Color, int> HitRawColors = new();
		Dictionary<FieldInfo, int> HitFields = new();

		int HitLerps = 0;
		int HitSpriteTextDraw = 0;
		int HitDrawTextWithShadow = 0;

		// Now, data structures for storing what we're changing.
		Dictionary<MethodInfo, (Dictionary<RuleRange, string>, Color)> Colors = new();
		Dictionary<Color, Dictionary<RuleRange, string>> RawColors = new();
		Dictionary<FieldInfo, Dictionary<RuleRange, string>> ColorFields = new();

		Dictionary<MethodInfo, Dictionary<RuleRange, Color>> DirectColors = new();
		Dictionary<Color, Dictionary<RuleRange, Color>> DirectRawColors = new();
		Dictionary<FieldInfo, Dictionary<RuleRange, Color>> DirectColorFields = new();

		Dictionary<FieldInfo, Dictionary<RuleRange, string>> FontFields = new();
		Dictionary<FieldInfo, Dictionary<RuleRange, string>> TextureFields = new();

		Dictionary<RuleRange, (string, string, string?, string?)>? SpriteTextDraw = null;

		Dictionary<RuleRange, string>? DrawTextWithShadow = null;
		Dictionary<RuleRange, Color>? DirectDrawTextWithShadow = null;

		Dictionary<RuleRange, (string, string, string)>? Lerp = null;
		Dictionary<RuleRange, (Color, Color, Color)>? DirectLerp = null;

		if (patcher.AppliedChanges.Colors is not null)
			foreach(var entry in patcher.AppliedChanges.Colors) {
				if (!ColorProperties.Value.TryGetValue(entry.Key, out var getter)) {
					patcher.Mod.Log($"Unable to find color named \"{entry.Key}\" processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				Dictionary<RuleRange, string> entries;
				if (Colors.TryGetValue(getter.Item1, out var ents))
					entries = ents.Item1;
				else
					entries = new();

				if (!DirectColors.TryGetValue(getter.Item1, out var directs))
					directs = new();

				foreach (var ent in entry.Value) {
					if (!RuleRange.TryParse(ent.Key, out var range)) {
						patcher.Mod.Log($"Unable to parse rule \"{ent.Key}\" processing {patcher.Key}", LogLevel.Warn);
						continue;
					}

					if (ent.Value.StartsWith('$'))
						entries[range] = ent.Value[1..];
					else if (CommonHelper.TryParseColor(ent.Value, out var c))
						directs[range] = c.Value;
					else {
						patcher.Mod.Log($"Unable to parse color \"{ent.Value}\" processing {patcher.Key}", LogLevel.Warn);
						continue;
					}
				}

				if (entries.Count > 0) {
					Colors[getter.Item1] = (entries, getter.Item2);
					count++;
				}

				if (directs.Count > 0) {
					DirectColors[getter.Item1] = directs;
					count++;
				}
			}

		if (patcher.AppliedChanges.RawColors is not null)
			foreach(var entry in patcher.AppliedChanges.RawColors) {
				if (!CommonHelper.TryParseColor(entry.Key, out var keycolor)) {
					patcher.Mod.Log($"Unable to parse raw color \"{entry.Key}\" processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				if (!RawColors.TryGetValue(keycolor.Value, out var entries))
					entries = new();

				if (!DirectRawColors.TryGetValue(keycolor.Value, out var directs))
					directs = new();

				foreach (var ent in entry.Value) {
					if (!RuleRange.TryParse(ent.Key, out var range)) {
						patcher.Mod.Log($"Unable to parse rule \"{ent.Key}\" processing {patcher.Key}", LogLevel.Warn);
						continue;
					}

					if (ent.Value.StartsWith('$'))
						entries[range] = ent.Value[1..];
					else if (CommonHelper.TryParseColor(ent.Value, out var c))
						directs[range] = c.Value;
					else {
						patcher.Mod.Log($"Unable to parse color \"{ent.Value}\" processing {patcher.Key}", LogLevel.Warn);
						continue;
					}
				}

				if (entries.Count > 0) {
					RawColors[keycolor.Value] = entries;
					count++;
				}

				if (directs.Count > 0) {
					DirectRawColors[keycolor.Value] = directs;
					count++;
				}
			}

		count += HydrateFieldSet(patcher.Key, patcher.AppliedChanges.ColorFields, ColorFields, DirectColorFields, method.DeclaringType);
		count += HydrateFieldSet(patcher.Key, patcher.AppliedChanges.FontFields, FontFields, null, method.DeclaringType);
		count += HydrateFieldSet(patcher.Key, patcher.AppliedChanges.TextureFields, TextureFields, null, method.DeclaringType);

		if (patcher.AppliedChanges.SpriteTextDraw is not null) {
			foreach(var entry in patcher.AppliedChanges.SpriteTextDraw) {
				if (!RuleRange.TryParse(entry.Key, out var range)) {
					patcher.Mod.Log($"Unable to parse rule \"{entry.Key}\" for SpriteTextDraw while processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				if (entry.Value is null ||
					entry.Value.Length < 2 ||
					entry.Value.Length > 4 ||
					string.IsNullOrWhiteSpace(entry.Value[0]) ||
					string.IsNullOrWhiteSpace(entry.Value[1]) ||
					!entry.Value[0].StartsWith('$') ||
					!entry.Value[1].StartsWith('$') ||
					(entry.Value.Length >= 3 && (string.IsNullOrWhiteSpace(entry.Value[2]) || !entry.Value[2].StartsWith('$'))) ||
					(entry.Value.Length >= 4 && (string.IsNullOrWhiteSpace(entry.Value[3]) || !entry.Value[3].StartsWith('$')))
				) {
					patcher.Mod.Log($"Invalid value \"{entry.Value}\" for \"{entry.Key}\" for SpriteTextDraw while processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				SpriteTextDraw ??= new();
				SpriteTextDraw[range] = new(
					entry.Value[0][1..],
					entry.Value[1][1..],
					entry.Value.Length >= 3 ? entry.Value[2][1..] : null,
					entry.Value.Length >= 4 ? entry.Value[3][1..] : null
				);
				count++;
			}
		}

		if (patcher.AppliedChanges.DrawTextWithShadow is not null)
			foreach(var entry in patcher.AppliedChanges.DrawTextWithShadow) {
				if (!RuleRange.TryParse(entry.Key, out var range)) {
					patcher.Mod.Log($"Unable to parse rule \"{entry.Key}\" processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				if (entry.Value.StartsWith('$')) {
					DrawTextWithShadow ??= new();
					DrawTextWithShadow[range] = entry.Value[1..];
					count++;
				} else if (CommonHelper.TryParseColor(entry.Value, out var c)) {
					DirectDrawTextWithShadow ??= new();
					DirectDrawTextWithShadow[range] = c.Value;
					count++;
				} else
					patcher.Mod.Log($"Unable to parse color \"{entry.Value}\" processing {patcher.Key}", LogLevel.Warn);
			}

		if (patcher.AppliedChanges.RedToGreenLerp is not null)
			foreach (var entry in patcher.AppliedChanges.RedToGreenLerp) {
				if (!RuleRange.TryParse(entry.Key, out var range)) {
					patcher.Mod.Log($"Unable to parse rule \"{entry.Key}\" processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				if (entry.Value.Length != 3 || string.IsNullOrWhiteSpace(entry.Value[0]) || string.IsNullOrWhiteSpace(entry.Value[1]) || string.IsNullOrWhiteSpace(entry.Value[2])) {
					patcher.Mod.Log($"Invalid RedToGreenLerp value processing {patcher.Key}. Entries must have three entries and cannot have empty strings.", LogLevel.Warn);
					continue;
				}

				bool is_variable = entry.Value[0].StartsWith('$');
				if (is_variable != entry.Value[1].StartsWith('$') || is_variable != entry.Value[2].StartsWith('$')) {
					patcher.Mod.Log($"Unable to combine variable and non-variable colors for RedToGreenLerp.", LogLevel.Warn);
					continue;
				}

				if (is_variable) {
					Lerp ??= new();
					Lerp[range] = (entry.Value[0][1..], entry.Value[1][1..], entry.Value[2][1..]);
					count++;
				} else if (CommonHelper.TryParseColor(entry.Value[0], out var left) && CommonHelper.TryParseColor(entry.Value[1], out var middle) && CommonHelper.TryParseColor(entry.Value[2], out var right)) {
					DirectLerp ??= new();
					DirectLerp[range] = (left.Value, middle.Value, right.Value);
					count++;
				} else
					patcher.Mod.Log($"Unable to parse color \"{entry.Value}\" processing {patcher.Key}", LogLevel.Warn);
			}

		if (count == 0)
			return instructions;

		if (patcher.Mod.Config.DebugPatches)
			patcher.Mod.Log($"Patching {patcher.Key} with {count} changes.");

		bool has_raw = RawColors.Count > 0 || DirectRawColors.Count > 0;

		// SpriteTextDraw
		Dictionary<MethodInfo, MethodInfo> SpriteTextDraw_Methods = new() {
			{
				AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawString)),
				AccessTools.Method(typeof(DynamicPatcher), nameof(SpriteText_drawString))
			},
			{
				AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringHorizontallyCenteredAt)),
				AccessTools.Method(typeof(DynamicPatcher), nameof(SpriteText_drawStringHorizontallyCenteredAt))
			},
			{
				AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringWithScrollBackground)),
				AccessTools.Method(typeof(DynamicPatcher), nameof(SpriteText_drawStringWithScrollBackground))
			},
			{
				patcher.Mod.ResolveMethod($"SpriteText:{nameof(SpriteText.drawStringWithScrollCenteredAt)}(SpriteBatch,,,,int,*)"),
				AccessTools.Method(typeof(DynamicPatcher), nameof(SpriteText_drawStringWithScrollCenteredAt_Int))
			},
			{
				patcher.Mod.ResolveMethod($"SpriteText:{nameof(SpriteText.drawStringWithScrollCenteredAt)}(SpriteBatch,,,,string,*)"),
				AccessTools.Method(typeof(DynamicPatcher), nameof(SpriteText_drawStringWithScrollCenteredAt_String))
			}
		};

		MethodInfo getFont = AccessTools.Method(typeof(DynamicPatcher), nameof(GetFont));
		MethodInfo getTexture = AccessTools.Method(typeof(DynamicPatcher), nameof(GetTexture));

		MethodInfo getColorPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(GetColorPacked));
		MethodInfo getColorPackedRef = AccessTools.Method(typeof(DynamicPatcher), nameof(GetColorPackedRef));
		MethodInfo getColor = AccessTools.Method(typeof(DynamicPatcher), nameof(GetColor));
		MethodInfo getLerpPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(GetLerpColorPacked));
		MethodInfo getLerp = AccessTools.Method(typeof(DynamicPatcher), nameof(GetLerpColor));

		MethodInfo drawTextSBPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(DrawTextWithShadowSBPacked));
		MethodInfo drawTextSB = AccessTools.Method(typeof(DynamicPatcher), nameof(DrawTextWithShadowSB));
		MethodInfo drawTextStrPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(DrawTextWithShadowStrPacked));
		MethodInfo drawTextStr = AccessTools.Method(typeof(DynamicPatcher), nameof(DrawTextWithShadowStr));

		MethodInfo Utility_GetRedGreenLerp = AccessTools.Method(typeof(Utility), nameof(Utility.getRedToGreenLerpColor));

		MethodInfo Utility_DrawTextShadowSB = patcher.Mod.ResolveMethod($"Utility:{nameof(Utility.drawTextWithShadow)}(SpriteBatch,StringBuilder,*)");
		MethodInfo Utility_DrawTextShadowStr = patcher.Mod.ResolveMethod($"Utility:{nameof(Utility.drawTextWithShadow)}(SpriteBatch,string,*)");

		ConstructorInfo cstruct = AccessTools.Constructor(typeof(Color), new Type[] {
			typeof(uint)
		});

		var instrs = instructions.ToArray();
		Color color;

		List<CodeInstruction> result = new();
		int replaced = 0;

		void AddAndLog(string message, CodeInstruction[] newInstructions, CodeInstruction[] oldInstructions) {
			if (patcher.Mod.Config.DebugPatches) {
				patcher!.Mod.Log(message, LogLevel.Debug);
				int start = result.Count;
				int i = start;
				foreach (var entry in oldInstructions) {
					patcher.Mod.Log($"{i:D3} -- {entry}", LogLevel.Trace);
					i++;
				}
				i = start;
				foreach (var entry in newInstructions) {
					patcher.Mod.Log($"{i:D3} ++ {entry}", LogLevel.Trace);
					i++;
				}
			}

			result.AddRange(newInstructions);
			replaced++;
		}

		for (int i = 0; i < instrs.Length; i++) {
			CodeInstruction in0 = instrs[i];

			// Raw Colors (new Color(r, g, b))
			if (i + 3 < instrs.Length && has_raw) {
				CodeInstruction in1 = instrs[i + 1];
				CodeInstruction in2 = instrs[i + 2];
				CodeInstruction in3 = instrs[i + 3];

				bool is_con = in3.IsConstructor<Color>();

				if (is_con || in3.IsCallConstructor<Color>()) {
					int? val0 = in0.AsInt();
					int? val1 = in1.AsInt();
					int? val2 = in2.AsInt();

					if (val0.HasValue && val1.HasValue && val2.HasValue) {
						Color c = new(val0.Value, val1.Value, val2.Value);

						// Increment hits
						HitRawColors.TryGetValue(c, out int hits);
						hits++;
						HitRawColors[c] = hits;

						if (RawColors.TryGetValue(c, out var entries) && TryGetMatch(entries, i, hits, out string? key)) {
							AddAndLog(
								$"Replacing raw color {c} with \"{key}\" at {i}",
								new CodeInstruction[] {
									new CodeInstruction(in0) {
										opcode = OpCodes.Ldstr,
										operand = key
									},
									new CodeInstruction(
										opcode: OpCodes.Ldc_I4,
										operand: unchecked((int) c.PackedValue)
									),
									new CodeInstruction(
										opcode: OpCodes.Call,
										operand: is_con ? getColorPacked : getColorPackedRef
									)
								},

								oldInstructions: new CodeInstruction[] {
									in0, in1, in2, in3
								}
							);

							i += 3;
							continue;

						} else if (DirectRawColors.TryGetValue(c, out var cent) && TryGetMatch(cent, i, hits, out Color clr)) {
							AddAndLog(
								$"Replacing raw color {c} with static {clr} at {i}",
								new CodeInstruction[] {
									new CodeInstruction(in0) {
										opcode = OpCodes.Ldc_I4,
										operand = unchecked((int) clr.PackedValue)
									},
									new CodeInstruction(
										opcode: in3.opcode,
										operand: cstruct
									)
								},

								oldInstructions: new CodeInstruction[] {
									in0, in1, in2, in3
								}
							);

							i += 3;
							continue;
						}
					}
				}
			}

			// Methods
			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo meth) {
				// Color Properties (Color.Red, Color.White, etc.)
				if (Colors.TryGetValue(meth, out var centries)) {
					// Increment hits.
					HitColors.TryGetValue(meth, out int hits);
					hits++;
					HitColors[meth] = hits;

					if (TryGetMatch(centries.Item1, i, hits, out string? key)) {
						AddAndLog(
							$"Replacing color property {meth.Name} with \"{key}\" at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldstr,
									operand = key
								},
								new CodeInstruction(
									opcode: OpCodes.Ldc_I4,
									operand: unchecked((int) centries.Item2.PackedValue)
								),
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: getColorPacked
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}
				}

				// Color Properties (But Direct)
				if (DirectColors.TryGetValue(meth, out var dcentries)) {
					// Increment hits.
					HitColors.TryGetValue(meth, out int hits);
					hits++;
					HitColors[meth] = hits;

					if (TryGetMatch(dcentries, i, hits, out color)) {
						AddAndLog(
							$"Replacing color property {meth.Name} with static {color} at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldc_I4,
									operand = unchecked((int) color.PackedValue)
								},
								new CodeInstruction(
									opcode: OpCodes.Newobj,
									operand: cstruct
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}
				}

				// DrawTextWithShadow
				if (meth == Utility_DrawTextShadowSB || meth == Utility_DrawTextShadowStr) {
					HitDrawTextWithShadow++;
					bool is_sb = meth == Utility_DrawTextShadowSB;

					if (DrawTextWithShadow is not null && TryGetMatch(DrawTextWithShadow, i, HitDrawTextWithShadow, out string? key)) {
						AddAndLog(
							$"Replacing {meth.Name} call with \"{key}\" at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldstr,
									operand = key
								},
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: is_sb ? drawTextSB : drawTextStr
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}

					if (DirectDrawTextWithShadow is not null && TryGetMatch(DirectDrawTextWithShadow, i, HitDrawTextWithShadow, out var c)) {
						AddAndLog(
							$"Replacing {meth.Name} call with static {c} at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldc_I4,
									operand = unchecked((int) c.PackedValue)
								},
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: is_sb ? drawTextSBPacked : drawTextStrPacked
								)
							},

							oldInstructions: new CodeInstruction[] { in0 }
						);

						continue;
					}
				}

				// SpriteTextDraw
				if (SpriteTextDraw is not null && SpriteTextDraw_Methods.TryGetValue(meth, out var wrapped)) {
					HitSpriteTextDraw++;

					if (wrapped is null)
						patcher.Mod.Log($"Somehow, wrapped is null for {meth}", LogLevel.Warn);

					else if (TryGetMatch(SpriteTextDraw, i, HitSpriteTextDraw, out var values)) {
						AddAndLog(
							$"Replacing {meth.Name} call with \"{values}\" at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldstr,
									operand = values.Item1
								},
								new CodeInstruction(
									opcode: OpCodes.Ldstr,
									operand: values.Item2
								),
								new CodeInstruction(
									opcode: values.Item3 == null ? OpCodes.Ldnull : OpCodes.Ldstr,
									operand: values.Item3
								),
								new CodeInstruction(
									opcode: values.Item4 == null ? OpCodes.Ldnull : OpCodes.Ldstr,
									operand: values.Item4
								),
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: wrapped
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}
				}

				// RedToGreenLerp
				if (meth == Utility_GetRedGreenLerp) {
					HitLerps++;

					if (Lerp is not null && TryGetMatch(Lerp, i, HitLerps, out var values)) {
						AddAndLog(
							$"Replacing {meth.Name} call with \"{values}\" at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldstr,
									operand = values.Item1
								},
								new CodeInstruction(
									opcode: OpCodes.Ldstr,
									operand: values.Item2
								),
								new CodeInstruction(
									opcode: OpCodes.Ldstr,
									operand: values.Item3
								),
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: getLerp
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}

					if (DirectLerp is not null && TryGetMatch(DirectLerp, i, HitLerps, out var cvalues)) {
						AddAndLog(
							$"Replacing {meth.Name} call with static {cvalues} at {i}",
							new CodeInstruction[] {
								new CodeInstruction(in0) {
									opcode = OpCodes.Ldc_I4,
									operand = unchecked((int) cvalues.Item1.PackedValue)
								},
								new CodeInstruction(
									opcode: OpCodes.Ldc_I4,
									operand: unchecked((int) cvalues.Item2.PackedValue)
								),
								new CodeInstruction(
									opcode: OpCodes.Ldc_I4,
									operand: unchecked((int) cvalues.Item3.PackedValue)
								),
								new CodeInstruction(
									opcode: OpCodes.Call,
									operand: getLerpPacked
								)
							},

							oldInstructions: new CodeInstruction[] {
								in0
							}
						);

						continue;
					}
				}


			}

			// Static Fields
			if ((in0.opcode == OpCodes.Ldsfld || in0.opcode == OpCodes.Ldfld) && in0.operand is FieldInfo field) {
				HitFields.TryGetValue(field, out int hits);
				hits++;
				HitFields[field] = hits;

				// Texture Fields (Game1.mouseCursors, etc.)
				if (TextureFields.TryGetValue(field, out var tentries) && TryGetMatch(tentries, i, hits, out string? key)) {
					AddAndLog(
						$"Replacing static texture field {field.Name} with \"{key}\" at {i}",
						new CodeInstruction[] {
							in0,
							new CodeInstruction(
								opcode: OpCodes.Ldstr,
								operand: key
							),
							new CodeInstruction(
								opcode: OpCodes.Call,
								operand: getTexture
							)
						},

						oldInstructions: new CodeInstruction[] {
							in0
						}
					);

					continue;
				}

				// Font Fields (Game1.smallText, etc.)
				if (FontFields.TryGetValue(field, out var fentries) && TryGetMatch(fentries, i, hits, out key)) {
					AddAndLog(
						$"Replacing static font field {field.Name} with \"{key}\" at {i}",
						new CodeInstruction[] {
							in0,
							new CodeInstruction(
								opcode: OpCodes.Ldstr,
								operand: key
							),
							new CodeInstruction(
								opcode: OpCodes.Call,
								operand: getFont
							)
						},

						oldInstructions: new CodeInstruction[] {
							in0
						}
					);

					continue;
				}

				// Color Fields (Game1.textColor, etc.)
				if (ColorFields.TryGetValue(field, out var entries) && TryGetMatch(entries, i, hits, out key)) {
					AddAndLog(
						$"Replacing static color field {field.Name} with \"{key}\" at {i}",
						new CodeInstruction[] {
							in0,
							new CodeInstruction(
								opcode: OpCodes.Ldstr,
								operand: key
							),
							new CodeInstruction(
								opcode: OpCodes.Call,
								operand: getColor
							)
						},

						oldInstructions: new CodeInstruction[] {
							in0
						}
					);

					continue;
				}

				// Color Fields, but hard-coded
				// Only support static fields for this to maintain the stack.
				if (in0.opcode == OpCodes.Ldsfld && DirectColorFields.TryGetValue(field, out var cent) && TryGetMatch(cent, i, hits, out color)) {
					AddAndLog(
						$"Replacing static color field {field.Name} with static {color} at {i}",
						new CodeInstruction[] {
							new CodeInstruction(in0) {
								opcode = OpCodes.Ldc_I4,
								operand = unchecked((int) color.PackedValue)
							},
							new CodeInstruction(
								opcode: OpCodes.Newobj,
								operand: cstruct
							)
						},

						oldInstructions: new CodeInstruction[] {
							in0
						}
					);

					continue;
				}
			}

			// Still here? Just push that instruction then.
			result.Add(in0);
		}

		if (patcher.Mod.Config.DebugPatches)
			patcher.Mod.Log($"- Performed {replaced} replacements.");

		return result;
	}

	#endregion
}
