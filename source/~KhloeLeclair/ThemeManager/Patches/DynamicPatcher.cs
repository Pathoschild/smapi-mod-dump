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

namespace Leclair.Stardew.ThemeManager.Patches;

internal class DynamicPatcher : IDisposable {

	#region Static Fields

	internal static bool DidPatch = false;
	internal static Dictionary<MethodBase, DynamicPatcher> LivePatchers = new();
	private static Dictionary<string, Color>? Colors;

	#endregion

	#region Color Access

	public static void UpdateColors(Dictionary<string, Color> colors) {
		Colors = colors;
	}

	internal static Color GetColorPacked(string key, uint @default) {
		return Colors != null && Colors.TryGetValue(key, out Color val) ? val: new Color(@default);
	}

	internal static Color GetColor(string key, Color @default) {
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

		return dict;
	});

	#endregion

	#region Fields

	private readonly ModEntry Mod;

	public readonly MethodInfo Method;
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

	#region Lifecycle

	public DynamicPatcher(ModEntry mod, MethodInfo method, string key) {
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
				Fields = new(),
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

				if (patch.Fields is not null) {
					foreach (var entry in patch.Fields) {
						if (!applied.Fields.TryGetValue(entry.Key, out var existing)) {
							existing = new();
							applied.Fields[entry.Key] = entry.Value;
						}

						foreach (var ent in entry.Value)
							existing[ent.Key] = ent.Value;
					}
				}

				if (patch.RedToGreenLerp is not null) {
					foreach (var entry in patch.RedToGreenLerp)
						applied.RedToGreenLerp[entry.Key] = entry.Value;
				}
			}

			// Now, compare it to our existing applied changes.
			if (applied.Colors.Count == 0 && applied.RawColors.Count == 0 && applied.Fields.Count == 0 && applied.RedToGreenLerp.Count == 0)
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
				if (!IsDirty && !applied.Fields!.ShallowEquals(AppliedChanges.Fields!))
					IsDirty = true;
				else if (!IsDirty) {
					foreach(var entry in applied.Fields!) {
						if (AppliedChanges.Fields!.TryGetValue(entry.Key, out var existing))
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

		// Now, data structures for 

		Dictionary<MethodInfo, (Dictionary<RuleRange, string>, Color)> Colors = new();
		Dictionary<Color, Dictionary<RuleRange, string>> RawColors = new();
		Dictionary<FieldInfo, Dictionary<RuleRange, string>> Fields = new();

		Dictionary<MethodInfo, Dictionary<RuleRange, Color>> DirectColors = new();
		Dictionary<Color, Dictionary<RuleRange, Color>> DirectRawColors = new();
		Dictionary<FieldInfo, Dictionary<RuleRange, Color>> DirectFields = new();

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

		if (patcher.AppliedChanges.Fields is not null)
			foreach(var entry in patcher.AppliedChanges.Fields) {
				FieldInfo field;
				if (entry.Key == "textColor" || entry.Key == "textShadowColor" || entry.Key == "unselectedOptionColor")
					field = AccessTools.Field(typeof(Game1), entry.Key);
				else {
					patcher.Mod.Log($"Skipping unknown field name \"{entry.Key}\" processing {patcher.Key}", LogLevel.Warn);
					continue;
				}

				if (!Fields.TryGetValue(field, out var entries))
					entries = new();

				if (!DirectFields.TryGetValue(field, out var directs))
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
					Fields[field] = entries;
					count++;
				}

				if (directs.Count > 0) {
					DirectFields[field] = directs;
					count++;
				}
			}

		if (patcher.AppliedChanges.RedToGreenLerp is not null) {
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
				} else if (CommonHelper.TryParseColor(entry.Value[0], out var left) && CommonHelper.TryParseColor(entry.Value[1], out var middle) && CommonHelper.TryParseColor(entry.Value[2], out var right)) {
					DirectLerp ??= new();
					DirectLerp[range] = (left.Value, middle.Value, right.Value);
				} else
					patcher.Mod.Log($"Unable to parse color \"{entry.Value}\" processing {patcher.Key}", LogLevel.Warn);
			}

			if (Lerp != null)
				count++;
			if (DirectLerp != null)
				count++;
		}

		if (count == 0)
			return instructions;

		if (patcher.Mod.Config.DebugPatches)
			patcher.Mod.Log($"Patching {patcher.Key} with {count} changes.");

		bool has_raw = RawColors.Count > 0 || DirectRawColors.Count > 0;

		MethodInfo getColorPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(GetColorPacked));
		MethodInfo getColor = AccessTools.Method(typeof(DynamicPatcher), nameof(GetColor));
		MethodInfo getLerpPacked = AccessTools.Method(typeof(DynamicPatcher), nameof(GetLerpColorPacked));
		MethodInfo getLerp = AccessTools.Method(typeof(DynamicPatcher), nameof(GetLerpColor));

		MethodInfo RedGreenLerpInfo = AccessTools.Method(typeof(Utility), nameof(Utility.getRedToGreenLerpColor));

		ConstructorInfo cstruct = AccessTools.Constructor(typeof(Color), new Type[] {
			typeof(uint)
		});

		var instrs = instructions.ToArray();
		Color color;

		List<CodeInstruction> result = new();
		int replaced = 0;

		void AddAndLog(string message, CodeInstruction[] newInstructions, CodeInstruction[] oldInstructions) {
			if (patcher.Mod.Config.DebugPatches) {
				patcher!.Mod.Log(message);
				foreach (var entry in oldInstructions)
					patcher.Mod.Log($"-- {entry}");
				foreach (var entry in newInstructions)
					patcher.Mod.Log($"++ {entry}");
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

				if (in3.IsConstructor<Color>()) {
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
								$"Replacing raw color {c} with: {key}",
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
										operand: getColorPacked
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
								$"Replacing raw color {c} with static: {clr}",
								new CodeInstruction[] {
									new CodeInstruction(in0) {
										opcode = OpCodes.Ldc_I4,
										operand = unchecked((int) clr.PackedValue)
									},
									new CodeInstruction(
										opcode: OpCodes.Newobj,
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

			// Color Properties (Color.Red)
			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo meth) {
				// Increment Hits
				HitColors.TryGetValue(meth, out int hits);
				hits++;
				HitColors[meth] = hits;

				if (Colors.TryGetValue(meth, out var entries) && TryGetMatch(entries.Item1, i, hits, out string? key)) {
					AddAndLog(
						$"Replacing color property {meth.Name} with: {key}",
						new CodeInstruction[] {
							new CodeInstruction(in0) {
								opcode = OpCodes.Ldstr,
								operand = key
							},
							new CodeInstruction(
								opcode: OpCodes.Ldc_I4,
								operand: unchecked((int) entries.Item2.PackedValue)
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

				if (DirectColors.TryGetValue(meth, out var cent) && TryGetMatch(cent, i, hits, out color)) {
					AddAndLog(
						$"Replacing color property {meth.Name} with static: {color}",
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

			// Static Field Access (Game1.textColor)
			if (in0.opcode == OpCodes.Ldsfld && in0.operand is FieldInfo field) {
				HitFields.TryGetValue(field, out int hits);
				hits++;
				HitFields[field] = hits;

				if (Fields.TryGetValue(field, out var entries) && TryGetMatch(entries, i, hits, out string? key)) {
					AddAndLog(
						$"Replacing static field {field.Name} with: {key}",
						new CodeInstruction[] {
							// Yes, even though we're also emitting in0
							// basically, we need to replace it so that
							// labels and stuff don't get screwed up.
							new CodeInstruction(in0) {
								opcode = OpCodes.Ldstr,
								operand = key
							},
							new CodeInstruction(
								opcode: in0.opcode,
								operand: in0.operand
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

				if (DirectFields.TryGetValue(field, out var cent) && TryGetMatch(cent, i, hits, out color)) {
					AddAndLog(
						$"Replacing static field {field.Name} with static: {color}",
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

			// Red To Green Lerp
			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo minfo && minfo == RedGreenLerpInfo) {
				HitLerps++;

				if (Lerp is not null && TryGetMatch(Lerp, i, HitLerps, out var values)) {
					AddAndLog(
						$"Replacing {minfo.Name} call with: {values}",
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
						$"Replacing {minfo.Name} call with static: {cvalues}",
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

			// Still here? Just push that instruction then.
			result.Add(in0);
		}

		if (patcher.Mod.Config.DebugPatches)
			patcher.Mod.Log($"- Performed {replaced} replacements.");

		return result;
	}

	#endregion
}
