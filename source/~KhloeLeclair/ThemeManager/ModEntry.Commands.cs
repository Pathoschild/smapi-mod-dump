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
using System.Reflection.Emit;
using System.Reflection;
using System.Text;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

using StardewModdingAPI;

using Leclair.Stardew.Common.Events;

using Leclair.Stardew.ThemeManager.Models;
using Leclair.Stardew.ThemeManager.Patches;
using System.Text.Unicode;
using Leclair.Stardew.ThemeManager.VariableSets;

namespace Leclair.Stardew.ThemeManager;

public partial class ModEntry {

	#region Console Commands

	[ConsoleCommand("tm_method_tree", "View a tree of draw methods. By default, tries to view the draw() method of the currently active menu.")]
	private void Command_MethodTree(string nm, string[] args) {
		// Use the current menu as the backup type.,
		IClickableMenu menu = Game1.activeClickableMenu;
		if (menu is not null) {
			if (menu is TitleMenu && TitleMenu.subMenu is not null)
				menu = TitleMenu.subMenu;

			if (menu is GameMenu gm && gm.currentTab < gm.pages.Count)
				menu = gm.pages[gm.currentTab];

		} else {
			int x = Game1.getMouseX();
			int y = Game1.getMouseY();
			foreach (var m in Game1.onScreenMenus) {
				if (m.xPositionOnScreen <= x && m.xPositionOnScreen + m.width >= x &&
					m.yPositionOnScreen <= y && m.yPositionOnScreen + m.height >= y
				) {
					menu = m;
					break;
				}
			}
		}

		IClickableMenu? child = menu?.GetChildMenu();
		while (child is not null) {
			menu = child;
			child = menu.GetChildMenu();
		}

		string input = string.Join(' ', args);
		var result = ResolveMember<MethodInfo>(string.Join(' ', args), current: menu?.GetType());

		Type? type = result?.Item1;
		MethodInfo? info = result?.Item2;

		if (type is null) {
			Log($"Could not find type.");
			return;
		}

		if (info is null) {
			Log($"Could not find method in {type.FullName}");
			return;
		}

		HashSet<MethodInfo> visited = new();
		SimpleNode<string>? root = WalkMethodTree(info, null, 10, visited);

		if (root is null) {
			Log($"Could not compute method tree.");
			return;
		}

		StringBuilder sb = new();
		PrintTree(sb, root, 0);
		Log($"Method Tree:\n\n{sb}\n", LogLevel.Info);
	}

	internal void PrintTree(StringBuilder sb, SimpleNode<string> node, int level) {
		string padding = new string(' ', level * 4);

		sb.AppendLine($"{padding}{node.Value}");

		foreach (var child in node.Children)
			PrintTree(sb, child, level + 1);
	}

	internal SimpleNode<string>? WalkMethodTree(MethodInfo method, SimpleNode<string>? parent, int remaining, HashSet<MethodInfo> visited) {
		if (!visited.Add(method) || remaining < 0)
			return null;

		SimpleNode<string> node = new(ToTargetString(method)!, parent);

		try {
			var Instructions = PatchProcessor.GetCurrentInstructions(method);
			if (Instructions is not null) {
				foreach (var instr in Instructions) {
					if ((instr.opcode == OpCodes.Call || instr.opcode == OpCodes.Callvirt) && instr.operand is MethodInfo other) {
						foreach (var par in other.GetParameters()) {
							if (par.ParameterType == typeof(SpriteBatch)) {
								// We did it reddit!
								var onode = WalkMethodTree(other, node, remaining - 1, visited);
								if (onode is not null)
									node.Children.Add(onode);
							}
						}
					}
				}
			}
		} catch (Exception) {
			// Do nothing ~
		}

		return node;
	}

	[ConsoleCommand("tm_method_genpatch", "Generate an empty patch for a method. By default, tries to view the draw() method of the currently active menu.")]
	[ConsoleCommand("tm_method_view", "View all of the supported values used in a method. By default, tries to view the draw() method of the currently active menu.")]
	private void Command_MethodView(string nm, string[] args) {
		bool genpatch = string.Equals(nm, "tm_method_genpatch", StringComparison.OrdinalIgnoreCase);

		// Use the current menu as the backup type.,
		IClickableMenu menu = Game1.activeClickableMenu;
		if (menu is not null) {
			if (menu is TitleMenu && TitleMenu.subMenu is not null)
				menu = TitleMenu.subMenu;

			if (menu is GameMenu gm && gm.currentTab < gm.pages.Count)
				menu = gm.pages[gm.currentTab];

		} else {
			int x = Game1.getMouseX();
			int y = Game1.getMouseY();
			foreach (var m in Game1.onScreenMenus) {
				if (m.xPositionOnScreen <= x && m.xPositionOnScreen + m.width >= x &&
					m.yPositionOnScreen <= y && m.yPositionOnScreen + m.height >= y
				) {
					menu = m;
					break;
				}
			}
		}

		IClickableMenu? child = menu?.GetChildMenu();
		while (child is not null) {
			menu = child;
			child = menu.GetChildMenu();
		}

		string input = string.Join(' ', args);
		bool want_ctor = false;
		if (input.IndexOf(":(") != -1 || input.IndexOf(":.ctor") != -1)
			want_ctor = true;

		(Type, MethodBase)? result;
		if (want_ctor)
			result = ResolveMember<ConstructorInfo>(input, current: menu?.GetType());
		else
			result = ResolveMember<MethodInfo>(input, current: menu?.GetType());

		Type? type = result?.Item1;
		MethodBase? info = result?.Item2;

		if (type is null) {
			Log($"Could not find type.");
			return;
		}

		if (info is null) {
			Log($"Could not find method in {type.FullName}");
			return;
		}

		// If we've already patched the method, un-patch it temporarily.
		if (DynamicPatchers.TryGetValue(info, out var patcher))
			patcher.Unpatch();
		else
			patcher = null;

		var Instructions = PatchProcessor.GetCurrentInstructions(info);
		if (Instructions is null) {
			Log($"Could not read method instructions.");
			return;
		}

		// Reapply our patch.
		patcher?.Patch();

		MethodInfo[] SpriteText_Drawing = new MethodInfo[] {
			AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawString)),
			AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringHorizontallyCenteredAt)),
			AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringWithScrollBackground)),
			ResolveMethod($"SpriteText:{nameof(SpriteText.drawStringWithScrollCenteredAt)}(SpriteBatch,,,,int,*)"),
			ResolveMethod($"SpriteText:{nameof(SpriteText.drawStringWithScrollCenteredAt)}(SpriteBatch,,,,string,*)")
			/*AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringWithScrollCenteredAt), new Type[] {
				typeof(SpriteBatch), typeof(string), typeof(int), typeof(int), typeof(int), typeof(float),
				typeof(Color?), typeof(int), typeof(float), typeof(bool)
			}),
			AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawStringWithScrollCenteredAt), new Type[] {
				typeof(SpriteBatch), typeof(string), typeof(int), typeof(int), typeof(string), typeof(float),
				typeof(Color?), typeof(int), typeof(float), typeof(bool)
			})*/
		};

		//MethodInfo SpriteText_getColorFromIndex = AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getColorFromIndex));

		MethodInfo[] Utility_DrawTextShadow = new MethodInfo[] {
			ResolveMethod($"Utility:{nameof(Utility.drawTextWithShadow)}(SpriteBatch,StringBuilder,*)"),
			ResolveMethod($"Utility:{nameof(Utility.drawTextWithShadow)}(SpriteBatch,string,*)"),
			/*AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] {
				typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int)
			}),
			AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] {
				typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(Vector2), typeof(Color), typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int)
			})*/
		};

		MethodInfo Utility_GetRedGreenLerp = AccessTools.Method(typeof(Utility), nameof(Utility.getRedToGreenLerpColor));

		Dictionary<string, Color> colorValues = new();
		Dictionary<MethodInfo, string> colors = new();
		foreach (var entry in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			if (entry.Name.Equals("White") || entry.Name.Equals("Black"))
				continue;

			if (entry.PropertyType != typeof(Color))
				continue;

			try {
				if (entry.GetValue(null) is Color color)
					colorValues[entry.Name] = color;
			} catch(Exception) {
				continue;
			}

			if (entry.GetGetMethod() is MethodInfo method) {
				colors.Add(method, entry.Name);
			}
		}

		foreach (var entry in typeof(SpriteText).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			if (entry.PropertyType != typeof(Color))
				continue;

			if (entry.GetGetMethod() is MethodInfo method) {
				string name = entry.Name;
				if (name.StartsWith("color_"))
					name = name[6..];

				name = $"SpriteText:{name}";

				try {
					if (entry.GetValue(null) is Color color)
						colorValues[name] = color;
				} catch (Exception) {
					continue;
				}

				colors.Add(method, name);
			}
		}

		Log($"Method: {ToTargetString(info)}", LogLevel.Info);
		Log($"Class: {type.FullName}", LogLevel.Trace);
		Log($"Method (Raw): {info.FullDescription()}", LogLevel.Trace);

		bool found = false;

		Dictionary<string, List<int>> Colors = new();
		Dictionary<string, List<int>> RawColors = new();
		//Dictionary<int, List<int>> SpriteTextColors = new();
		Dictionary<string, List<int>> ColorFields = new();
		Dictionary<string, List<int>> FontFields = new();
		Dictionary<string, List<int>> TextureFields = new();

		List<int> SpriteTextDraws = new();
		List<int> DrawTextShadow = new();
		List<int> RedToGreenLerps = new();

		for (int i = 0; i < Instructions.Count; i++) {
			CodeInstruction in0 = Instructions[i];

			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo method) {
				if (colors.TryGetValue(method, out string? name)) {
					if (!Colors.TryGetValue(name, out var list)) {
						list = new();
						Colors[name] = list;
					}
					list.Add(i);
					found = true;
				}

				/*if (method == SpriteText_getColorFromIndex && i > 0) {
					CodeInstruction inLast = Instructions[i - 1];
					Log($"Previous: {inLast}", LogLevel.Debug);

					found = true;
				}*/

				if (Utility_DrawTextShadow.Contains(method)) {
					DrawTextShadow.Add(i);
					found = true;
				}

				if (method == Utility_GetRedGreenLerp) {
					RedToGreenLerps.Add(i);
					found = true;
				}

				if (SpriteText_Drawing.Contains(method)) {
					SpriteTextDraws.Add(i);
					found = true;
				}
			}

			if (in0.opcode == OpCodes.Ldsfld && in0.operand is FieldInfo fld) {
				if (fld.FieldType == typeof(Texture2D)) {
					string? fname = ToTargetString(fld);
					if (fname != null) {
						if (!TextureFields.TryGetValue(fname, out var list)) {
							list = new();
							TextureFields[fname] = list;
						}

						list.Add(i);
						found = true;
					}
				}

				if (fld.FieldType == typeof(SpriteFont)) {
					string? fname = ToTargetString(fld);
					if (fname != null) {
						if (!FontFields.TryGetValue(fname, out var list)) {
							list = new();
							FontFields[fname] = list;
						}

						list.Add(i);
						found = true;
					}
				}

				if (fld.FieldType == typeof(Color)) {
					string? fname = ToTargetString(fld);
					if (fname != null) {
						if (!ColorFields.TryGetValue(fname, out var list)) {
							list = new();
							ColorFields[fname] = list;
						}

						list.Add(i);
						found = true;
					}
				}
			}

			if (i + 3 < Instructions.Count) {
				CodeInstruction in1 = Instructions[i + 1];
				CodeInstruction in2 = Instructions[i + 2];
				CodeInstruction in3 = Instructions[i + 3];

				if (in3.IsConstructor<Color>() || in3.IsCallConstructor<Color>()) {
					int? val0 = in0.AsInt();
					int? val1 = in1.AsInt();
					int? val2 = in2.AsInt();

					if (val0.HasValue && val1.HasValue && val2.HasValue) {
						string key = $"{val0.Value}, {val1.Value}, {val2.Value}";
						if (!RawColors.TryGetValue(key, out var list)) {
							list = new();
							RawColors[key] = list;
							colorValues[key] = new Color(val0.Value, val1.Value, val2.Value);
						}
						list.Add(i);
						found = true;
					}
				}
			}
		}

		if (genpatch) {
			var patch = new PatchData();
			string name = type.Name;
			var group = new PatchGroupData() {
				ID = $"Generated_{name}",
				Patches = new() {
					{ ToTargetString(info)!, patch }
				}
			};

			if (Colors.Count > 0) {
				patch.Colors = new();
				foreach (string key in Colors.Keys) {
					patch.Colors[key] = new() {
						{ "*", $"${name}:{key}" }
					};
				}
			}

			if (RawColors.Count > 0) {
				patch.RawColors = new();
				int i = 1;
				foreach (string key in RawColors.Keys) {
					patch.RawColors[key] = new() {
						{ "*", $"${name}:Raw:{i}" }
					};
					i++;
				}
			}

			/*if (SpriteTextColors.Count > 0) {
				patch.SpriteTextColors = new();
				foreach (int key in SpriteTextColors.Keys) {
					patch.SpriteTextColors[key] = new() {
						{ "*", $"${name}{key}" }
					};
				}
			}*/

			if (ColorFields.Count > 0) {
				patch.ColorFields = new();
				foreach (string k in ColorFields.Keys) {
					string key = k;
					if (key.EndsWith("(Color)"))
						key = key[..^7];

					string varname;

					if (string.Equals(key, "StardewValley.Game1:textColor")) {
						varname = $"${name}:Text";
						group.ColorVariables ??= new();
						group.ColorVariables[varname] = "$Text";

					} else if (string.Equals(key, "StardewValley.Game1:textShadowColor")) {
						varname = $"${name}:TextShadow";
						group.ColorVariables ??= new();
						group.ColorVariables[varname] = "$TextShadow";

					} else {
						int idx = key.IndexOf(':');
						if (idx == -1)
							varname = $"${name}:{key}";
						else
							varname = $"${name}{key[idx..]}";
					}

					if (key.StartsWith("StardewValley.Game1:"))
						key = key[14..];

					patch.ColorFields[key] = new() {
						{ "*", varname }
					};
				}
			}

			if (FontFields.Count > 0) {
				patch.FontFields = new();
				foreach (string k in FontFields.Keys) {
					string key = k;
					if (key.EndsWith("(SpriteFont)"))
						key = key[..^12];

					string varname;
					if (string.Equals(key, "StardewValley.Game1:dialogueFont")) {
						varname = $"${name}:Dialogue";
						group.FontVariables ??= new();
						group.FontVariables[varname] = "$Dialogue";

					} else if (string.Equals(key, "StardewValley.Game1:smallFont")) {
						varname = $"${name}:Small";
						group.FontVariables ??= new();
						group.FontVariables[varname] = "$Small";

					} else if (string.Equals(key, "StardewValley.Game1:tinyFont")) {
						varname = $"${name}:Tiny";
						group.FontVariables ??= new();
						group.FontVariables[varname] = "$Tiny";

					} else if (string.Equals(key, "StardewValley.Game1:tinyFontBorder")) {
						varname = $"${name}:TinyBorder";
						group.FontVariables ??= new();
						group.FontVariables[varname] = "$TinyBorder";

					} else {
						int idx = key.IndexOf(':');
						if (idx == -1)
							varname = $"${name}:{key}";
						else
							varname = $"${name}{key[idx..]}";
					}

					if (key.StartsWith("StardewValley.Game1:"))
						key = key[14..];

					patch.FontFields[key] = new() {
						{ "*", varname }
					};
				}
			}

			if (TextureFields.Count > 0) {
				patch.TextureFields = new();
				foreach (string k in TextureFields.Keys) {
					string key = k;
					if (key.EndsWith("(Texture2D)"))
						key = key[..^11];

					string varname;

					// Skip including certain textures.
					if (key.EndsWith(":staminaRect") || key.EndsWith(":fadeToBlackRect"))
						continue;

					if (key.StartsWith("StardewValley.Game1:"))
						key = key[14..];

					int idx = key.IndexOf(':');
					if (idx == -1)
						varname = $"${name}:{key}";
					else
						varname = $"${name}{key[idx..]}";

					patch.TextureFields[key] = new() {
						{"*", varname }
					};
				}

				if (patch.TextureFields.Count == 0)
					patch.TextureFields = null;
			}

			if (SpriteTextDraws.Count > 0) {
				patch.SpriteTextDraw = new() {
					{ "*", new string[] {
						$"${name}:ST:Normal",
						$"${name}:ST:Colored",
						$"${name}:ST:Font",
						$"${name}:Colors"
					} }
				};

				group.TextureVariables ??= new();
				group.TextureVariables[$"${name}:ST:Normal"] = "$ST:Normal";
				group.TextureVariables[$"${name}:ST:Colored"] = "$ST:Colored";

				group.BmFontVariables ??= new();
				group.BmFontVariables[$"${name}:ST:Font"] = "$ST:Font";
			}

			if (DrawTextShadow.Count > 0) {
				string varname = $"${name}:TextShadowAlt";
				patch.DrawTextWithShadow = new() {
					{ "*", varname }
				};

				group.ColorVariables ??= new();
				group.ColorVariables[varname] = "$TextShadowAlt";
			}

			if (RedToGreenLerps.Count > 0) {
				patch.RedToGreenLerp = new() {
					{"*", new string[] {
						$"${name}:Lerp:Red",
						$"${name}:Lerp:Yellow",
						$"${name}:Lerp:Green"
					} }
				};
			}

			GetJsonHelper();

			var oldHandling = JsonHelper!.JsonSettings.NullValueHandling;
			JsonHelper!.JsonSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
			string serialized = JsonHelper!.Serialize(group);
			JsonHelper!.JsonSettings.NullValueHandling = oldHandling;

			Log($"Generated Patch:\n{serialized}", LogLevel.Info);

		} else {
			StringBuilder bld = new();
			bld.AppendLine("Detected Supported Values:");

			if (!found)
				bld.AppendLine($"- Did not find any values to patch.");
			if (Colors.Count > 0) {
				bld.AppendLine($"- Colors:");
				foreach (var entry in Colors) {
					if (!colorValues.TryGetValue(entry.Key, out Color ec))
						ec = Color.Transparent;
					bld.AppendLine($"  - {entry.Key} {ec.ToString()} (Offsets: {string.Join(", ", entry.Value)})");
				}
			}
			if (RawColors.Count > 0) {
				bld.AppendLine($"- RawColors:");
				foreach (var entry in RawColors)
					bld.AppendLine($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})");
			}
			/*if (SpriteTextColors.Count > 0) {
				bld.AppendLine($"- SpriteTextColors:");
				foreach (var entry in SpriteTextColors)
					bld.AppendLine($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})");
			}*/
			if (ColorFields.Count > 0) {
				bld.AppendLine($"- ColorFields:");
				foreach (var entry in ColorFields)
					bld.AppendLine($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})");
			}
			if (FontFields.Count > 0) {
				bld.AppendLine($"- FontFields:");
				foreach (var entry in FontFields)
					bld.AppendLine($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})");
			}
			if (TextureFields.Count > 0) {
				bld.AppendLine($"- TextureFields:");
				foreach (var entry in TextureFields)
					bld.AppendLine($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})");
			}
			if (SpriteTextDraws.Count > 0)
				bld.AppendLine($"- SpriteTextDraw (Offsets: {string.Join(", ", SpriteTextDraws)})");
			if (DrawTextShadow.Count > 0)
				bld.AppendLine($"- DrawTextWithShadow (Offsets: {string.Join(", ", DrawTextShadow)})");
			if (RedToGreenLerps.Count > 0)
				bld.AppendLine($"- RedToGreenLerp (Offsets: {string.Join(", ", RedToGreenLerps)})");

			Log(bld.ToString(), LogLevel.Info);
		}
	}

	[ConsoleCommand("tm_adjust_font", "Adjust a font's margins for testing.")]
	private void Command_AdjustFont(string name, string[] args) {
		var font = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]) ? GameTheme?.GetFontVariable(args[0]) : null;
		if (font == null) {
			Log("Could not get font.", LogLevel.Info);
			return;
		}

		if (args.Length < 3 || !int.TryParse(args[2], out int value)) {
			Log($"Usage: tm_adjust_font [name] top/bottom [pixels]", LogLevel.Info);
			return;
		}

		bool top = string.Equals(args[1], "top", StringComparison.OrdinalIgnoreCase);

		if (value != 0) {
			for (int i = 0; i < font.Glyphs.Length; i++) {
				var glyph = font.Glyphs[i];
				if (top)
					glyph.Cropping = new(
						glyph.Cropping.X,
						glyph.Cropping.Y + value,
						glyph.Cropping.Width,
						glyph.Cropping.Height
					);
				else
					glyph.Cropping = new(
						glyph.Cropping.X,
						glyph.Cropping.Y,
						glyph.Cropping.Width,
						glyph.Cropping.Height + value
					);

				font.Glyphs[i] = glyph;
			}
		}

		Log($"Adjusted font.", LogLevel.Info);
	}


	[ConsoleCommand("tm_analyze_font", "Analyze a font to see how big the characters really are.")]
	private void Command_AnalyzeFont(string name, string[] args) {
		if (GameTheme is null) {
			Log($"The game theme has not been loaded. Unable to analyze fonts.", LogLevel.Info);
			return;
		}

		if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]) || string.Equals(args[0], "list")) {
			List<string[]> entries = new();

			foreach (string entry in new string[] {
				"default:smallFont", "default:dialogueFont", "default:tinyFont", "default:tinyFontBorder"
			})
				entries.Add(new string[] { entry, entry });

			foreach(var entry in GameTheme.FontVariables) {
				if (GameTheme.FontVariables.InheritedValues.TryGetValue(entry.Key, out string? src) && ! src.StartsWith('$')) {
					entries.Add(new string[] {
						$"${entry.Key}",
						src
					});
				}
			}

			StringBuilder sb2 = new();

			LogTable(sb2, new string[] {
				"Font Name",
				"Source"
			}, entries, LogLevel.Info);

			Log(sb2.ToString(), LogLevel.Info);

			return;
		}

		SpriteFont? font;
		bool want_glyphs = args.Length > 1 && args[args.Length - 1] is string bit && (bit.Equals("show", StringComparison.OrdinalIgnoreCase) || bit.Equals("glyphs", StringComparison.OrdinalIgnoreCase));
		string input = string.Join(' ', args, 0, want_glyphs ? args.Length - 1 : args.Length);

		if (input.StartsWith('$'))
			font = GameTheme?.GetFontVariable(input);
		else if (FontVariableSet.TryParseValue(input, GameThemeManager, GameThemeManager?.ActiveThemeId, out var managed))
			font = managed.Value;
		else {
			Log($"Unable to parse font: {input}", LogLevel.Warn);
			return;
		}

		if (font == null) {
			Log($"Could not load font: {input}", LogLevel.Warn);
			return;
		}

		string source = input;
		HashSet<string> visited = new();
		while(GameTheme is not null && source.StartsWith('$')) {
			if (!visited.Add(source))
				break;

			string inp = source[1..];

			if (GameTheme.FontVariables.InheritedValues.TryGetValue(inp, out string? value))
				source = value;
			else if (GameTheme.PatchFontVariables.TryGetValue(inp, out string? pvalue))
				source = pvalue;
			else
				break;
		}

		StringBuilder sb = new();

		string texname = font.Texture.Name ?? "Unnamed Texture";

		sb.AppendLine($"Font Analysis of: {input}\n");
		if (visited.Count > 1)
			sb.AppendLine($"Variable Chain: {string.Join(" => ", visited)}");
		sb.AppendLine($"Source: {source}");
		sb.AppendLine($"Texture: {texname} ({font.Texture.Bounds.Width}x{font.Texture.Bounds.Height})");
		sb.AppendLine($"Spacing: {font.Spacing}");
		sb.AppendLine($"Line Spacing: {font.LineSpacing}");

		List<string[]>? glyphs = want_glyphs ? new() : null;

		int minHeight = int.MaxValue;
		int maxHeight = int.MinValue;
		int minWidth = int.MaxValue;
		int maxWidth = int.MinValue;

		int minLeft = int.MaxValue;
		int minRight = int.MaxValue;
		int minTop = int.MaxValue;
		int minBottom = int.MaxValue;

		Dictionary<string, UnicodeRange> ranges = new();
		Dictionary<string, int> rangeRemaining = new();

		foreach(var prop in typeof(UnicodeRanges).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			if (prop.CanRead && prop.PropertyType == typeof(UnicodeRange) && prop.Name != "All") {
				try {
					if (prop.GetValue(null) is UnicodeRange range)
						ranges[prop.Name] = range;
				} catch { /* nothing */ }
			}
		}

		foreach (var glyph in font.Glyphs) {
			int width = glyph.BoundsInTexture.Width;
			int height = glyph.BoundsInTexture.Height;

			int paddingTop = glyph.Cropping.Y;
			int paddingLeft = glyph.Cropping.X;
			int paddingBottom = glyph.Cropping.Height - height;
			int paddingRight = glyph.Cropping.Width - width;

			width = (int) glyph.Width;

			minHeight = Math.Min(minHeight, height);
			maxHeight = Math.Max(maxHeight, height);
			minWidth = Math.Min(minWidth, width);
			maxWidth = Math.Max(width, maxWidth);

			minTop = Math.Min(paddingTop, minTop);
			minLeft = Math.Min(paddingLeft, minLeft);
			minBottom = Math.Min(paddingBottom, minBottom);
			minRight = Math.Min(paddingRight, minRight);

			int chr = (int) glyph.Character;
			foreach(var entry in ranges) {
				if (chr >= entry.Value.FirstCodePoint && chr < entry.Value.FirstCodePoint + entry.Value.Length) {
					if (!rangeRemaining.TryGetValue(entry.Key, out int remaining))
						remaining = entry.Value.Length;

					remaining--;
					rangeRemaining[entry.Key] = remaining;
				}
			}

			if (glyphs is not null)
				glyphs.Add(new string[] {
					((short)glyph.Character) < 32 || (short)glyph.Character == 8226 ? $"    {(short)glyph.Character}" : $" {glyph.Character} ({(short)glyph.Character})",
					$"Left={glyph.LeftSideBearing}, Width={glyph.Width}, Right={glyph.RightSideBearing}",
					glyph.BoundsInTexture.ToString(),
					glyph.Cropping.ToString(),
					$"Top={paddingTop}, Left={paddingLeft}, Right={paddingRight}, Bottom={paddingBottom}"
				});
		}

		sb.AppendLine($"Minimum Glyph Size: {minWidth}x{minHeight}");
		sb.AppendLine($"Maximum Glyph Size: {maxWidth}x{maxHeight}");
		sb.AppendLine($"Minimum Padding: Top={minTop}, Left={minLeft}, Right={minRight}, Bottom={minBottom}");

		if (rangeRemaining.Count > 0) {
			sb.AppendLine($"Unicode Ranges:");
			foreach(var entry in rangeRemaining) {
				if (ranges.TryGetValue(entry.Key, out var range)) {
					int percentage = (int) (100 * (range.Length - entry.Value) / (float) range.Length);
					if (percentage >= 100)
						sb.AppendLine($"  {entry.Key} (Complete) ({range.Length - entry.Value} of {range.Length})");
					else
						sb.AppendLine($"  {entry.Key} ({percentage}%) ({range.Length - entry.Value} of {range.Length})");
				}
			}
		}

		sb.AppendLine($"Glyphs: {font.Glyphs.Length}");

		if (glyphs is not null) {
			sb.AppendLine("");

			LogTable(sb, new string[] {
				"Character", "Kerning", "Bounds", "Cropping", "Padding"
			}, glyphs, LogLevel.Info);
		}

		Log(sb.ToString(), LogLevel.Info);
	}

	/*[ConsoleCommand("tm_toggle_font_fix", "Toggle Theme Manager's font alignment fix.")]
	private void Command_ToggleFontFix(string name, string[] args) {
		Config.AlignText = !Config.AlignText;
		SaveConfig();

		Log($"Font alignment has been set to {Config.AlignText}", LogLevel.Info);
	}*/

	[ConsoleCommand("theme", "View available themes, reload themes, and change the current themes.")]
	private void Command_Theme(string name, string[] args) {
		// List Mods
		if (args.Length == 0 || string.Equals("list", args[0], StringComparison.OrdinalIgnoreCase)) {
			List<string[]> ents = new() {
				new string[] {
					"stardew",
					GameThemeManager!.ActiveThemeId,
					GameThemeManager!.SelectedThemeId,
					GameThemeManager!.GetThemeChoices().Where(x => x.Key != "automatic" && x.Key != "default").Count().ToString()
				}
			};

			foreach (var entry in Managers)
				ents.Add(new string[] {
					entry.Key.UniqueID,
					entry.Value.Item2.ActiveThemeId,
					entry.Value.Item2.SelectedThemeId,
					entry.Value.Item2.GetThemeChoices().Where(x => x.Key != "automatic" && x.Key != "default").Count().ToString()
				});

			LogTable(new string[] {
				"Manager ID",
				"Active Theme",
				"Selected Theme",
				"Total Themes"
			}, ents, LogLevel.Info, " | ");
			return;
		}

		if (string.Equals(args[0], "help", StringComparison.OrdinalIgnoreCase)) {
			LogTable(null, new string[][] {
				new string[] {
					"list", "List all the mods currently using theme managers, as well as their active themes."
				},
				new string[] {
					"help", "View this information."
				},
				new string[] {
					"reload", "Reload all theme managers' themes."
				},
				new string[] {
					"[manager] list", "List all the themes available for a given theme manager."
				},
				new string[] {
					"[manager] paths", "List a manager's asset paths, for use with Content Patcher."
				},
				new string[] {
					"[manager] reload", "Reload a given theme manager's themes."
				},
				new string[] {
					"[manager] [theme]", "Select a theme for a given theme manager."
				}
			}, LogLevel.Info);
			return;
		}

		if (string.Equals(args[0], "reload", StringComparison.OrdinalIgnoreCase)) {
			Command_ReTheme(name, args);
			return;
		}

		// It's a manager command. Look one up.
		IThemeManager? manager = null;
		string? targetManager = null;

		// Strict matching
		if (string.Equals(args[0], "stardew", StringComparison.OrdinalIgnoreCase)) {
			targetManager = "stardew";
			manager = GameThemeManager;
		} else {
			foreach (var entry in Managers) {
				if (string.Equals(args[0], entry.Key.UniqueID, StringComparison.OrdinalIgnoreCase)) {
					manager = entry.Value.Item2;
					targetManager = entry.Key.UniqueID;
					break;
				}
			}
		}

		// Sloppy matching.
		if (manager is null) {
			if ("stardew".Contains(args[0], StringComparison.OrdinalIgnoreCase)) {
				manager = GameThemeManager;
				targetManager = "stardew";
			} else {
				foreach (var entry in Managers) {
					if (entry.Key.UniqueID.Contains(args[0], StringComparison.OrdinalIgnoreCase)) {
						manager = entry.Value.Item2;
						targetManager = entry.Key.UniqueID;
						break;
					}
				}
			}
		}

		// No matches?
		if (manager is null) {
			Log($"Unable to match manager: {args[0]}", LogLevel.Warn);
			return;
		}

		Log($"Manager: {targetManager}", LogLevel.Info);

		if (args.Length > 1 && string.Equals(args[1], "reload", StringComparison.OrdinalIgnoreCase)) {
			manager.Discover();
			manager.Invalidate();
			Log($"Reloaded all themes across 1 manager.", LogLevel.Info);
			return;
		}

		if (args.Length > 1 && string.Equals(args[1], "paths", StringComparison.OrdinalIgnoreCase)) {
			if (manager.UsingThemeRedirection)
				Log($"Theme Data: {manager.ThemeLoaderPath}", LogLevel.Info);
			else
				Log($"Theme Data Redirection is disabled for this manager.", LogLevel.Info);

			if (manager.UsingAssetRedirection) {
				Log($"Asset Prefix: {manager.AssetLoaderPrefix}", LogLevel.Info);

				Dictionary<string, string> cached = new();

				// Pretend like we're going to invalidate the cache so we can get
				// the names of all cached assets.
				Helper.GameContent.InvalidateCache(asset => {
					if (asset.Name.StartsWith(manager.AssetLoaderPrefix))
						cached[asset.Name.Name] = asset.DataType.Name;
					return false;
				});

				if (cached.Count > 0) {
					List<string[]> ents = new();
					foreach (var entry in cached)
						ents.Add(new string[] { entry.Key, entry.Value });

					Log($"Cached Assets:", LogLevel.Info);
					LogTable(new string[] { "Key", "Type" }, ents, LogLevel.Info);
				} else
					Log($"There are no cached assets.", LogLevel.Info);

			} else
				Log($"Asset Redirection is disabled for this manager.", LogLevel.Info);

			return;
		}

		if (args.Length > 1 && !string.Equals(args[1], "list", StringComparison.OrdinalIgnoreCase)) {
			string needle = string.Join(" ", args, 1, args.Length - 1);
			string? selected = null;
			string? targetTheme = null;
			var themes = manager.GetThemeChoices();

			// Check for unique ID matches first.
			foreach (var pair in themes) {
				if (pair.Key.Equals(needle, StringComparison.OrdinalIgnoreCase)) {
					selected = pair.Key;
					targetTheme = pair.Value;
					break;
				}
			}

			// Now check for unique ID partial matches.
			if (selected is null)
				foreach (var pair in themes) {
					if (pair.Key.Contains(needle, StringComparison.OrdinalIgnoreCase)) {
						selected = pair.Key;
						targetTheme = pair.Value;
						break;
					}
				}

			// Lastly select for partial display name matches
			if (selected is null)
				foreach (var pair in themes) {
					if (pair.Value.Contains(needle, StringComparison.OrdinalIgnoreCase)) {
						selected = pair.Key;
						targetTheme = pair.Value;
						break;
					}
				}

			if (selected != null) {
				manager.SelectTheme(selected);
				if (manager == GameThemeManager) {
					Config.StardewTheme = manager.SelectedThemeId;
					SaveConfig();
				} else if (!string.IsNullOrEmpty(targetManager)) {
					Config.SelectedThemes[targetManager] = manager.SelectedThemeId;
					SaveConfig();
				}

				Log($"Selected Theme: {selected} ({targetTheme})", LogLevel.Info);

			} else
				Log($"Unable to match theme: {needle}", LogLevel.Warn);
		}

		List<string[]> entries = new();

		foreach (var pair in manager.GetThemeChoices()) {
			bool sel = pair.Key == manager.SelectedThemeId;
			bool active = pair.Key == manager.ActiveThemeId;

			entries.Add(new string[] {
				sel ? "***" : "",
				active ? "***" : "",
				pair.Key,
				pair.Value
			});
		}

		LogTable(new string[] {
				"Selected", "Active", "ID", "Name"
			}, entries, LogLevel.Info);
	}

	[ConsoleCommand("tm_repatch", "Reload all patch data and reapply patches.")]
	private void Command_Repatch(string name, string[] args) {
		PatchGroups = null;
		LoadPatchGroups();

		GameTheme!.ResetPatchVariables();
		GameTheme!.ColorVariables.DefaultValues = GameTheme.PatchColorVariables;
		GameTheme!.FontVariables.DefaultValues = GameTheme.PatchFontVariables;
		GameTheme!.TextureVariables.DefaultValues = GameTheme.PatchTextureVariables;
		GameTheme!.BmFontVariables.DefaultValues = GameTheme.PatchBmFontVariables;

		DynamicPatcher.UpdateColors(GameTheme.ColorVariables);
		DynamicPatcher.UpdateSpriteTextColors(GameTheme.SpriteTextColorSets);
		DynamicPatcher.UpdateFonts(GameTheme.FontVariables);
		DynamicPatcher.UpdateTextures(GameTheme.TextureVariables);
		DynamicPatcher.UpdateBmFonts(GameTheme.BmFontVariables);

		SelectPatches(GameTheme);

		Log($"Reloaded {PatchGroups.Count} patch groups and applied patches to {DynamicPatchers.Count} methods.", LogLevel.Info);
	}

	[ConsoleCommand("tm_invalidate", "Invalidate an asset.")]
	private void Command_Invalidate(string name, string[] args) {
		if (args.Length == 0)
			return;

		string? input = string.Join(' ', args);
		if (string.IsNullOrWhiteSpace(input))
			return;

		bool wild = false;

		if (input.EndsWith('*')) {
			wild = true;
			input = input.Length == 1 ? null : input.Substring(0, input.Length - 1);
		}

		int count = 0;

		Helper.GameContent.InvalidateCache(info => {
			if (input == null || (wild ? info.Name.StartsWith(input) : info.Name.IsEquivalentTo(input))) {
				count++;
				return true;
			}
			return false;
		});

		Log($"Invalidated {count} cache entries.", LogLevel.Info);
	}

	[ConsoleCommand("retheme", "Reload all themes.")]
	private void Command_ReTheme(string name, string[] args) {

		GameThemeManager!.Discover();
		//BaseThemeManager!.Invalidate();

		foreach (var entry in Managers) {
			entry.Value.Item2.Discover();
			//entry.Value.Item2.Invalidate();
		}

		Log($"Reloaded all themes across {Managers.Count + 1} managers.", LogLevel.Info);
	}

	[ConsoleCommand("tm_harmony_summary", "View all of the harmony patches currently applied by Theme Manager.")]
	private void Command_HarmonySummary(string _, string[] args) {
		StringBuilder sb = new();
		bool brief = true;

		List<(string, HarmonyLib.Patches)> sorted = new();

		foreach (var method in PatchProcessor.GetAllPatchedMethods()) {
			var patches = PatchProcessor.GetPatchInfo(method);
			if (patches is null || !patches.Owners.Contains(ModManifest.UniqueID))
				continue;

			string? name = ToTargetString(method, false);
			if (string.IsNullOrWhiteSpace(name))
				continue;

			sorted.Add((name, patches));
		}

		sorted.Sort((a, b) => a.Item1.CompareTo(b.Item1));

		foreach (var pair in sorted) {
			string name = pair.Item1;
			var patches = pair.Item2;

			sb.AppendLine($"  {name}");

			Dictionary<string, List<string>> ByOwner = new();

			foreach (var entry in patches.Prefixes) {
				if (!ByOwner.TryGetValue(entry.owner, out var list)) {
					list = new();
					ByOwner[entry.owner] = list;
				}

				if (brief)
					list.Add($"prefix({entry.priority})");
				else
					list.Add($"prefix: {ToTargetString(entry.PatchMethod)}, priority: {entry.priority}");
			}

			foreach (var entry in patches.Transpilers) {
				if (!ByOwner.TryGetValue(entry.owner, out var list)) {
					list = new();
					ByOwner[entry.owner] = list;
				}

				if (brief)
					list.Add($"transpiler({entry.priority})");
				else
					list.Add($"transpiler: {ToTargetString(entry.PatchMethod)}, priority: {entry.priority}");
			}

			foreach (var entry in patches.Postfixes) {
				if (!ByOwner.TryGetValue(entry.owner, out var list)) {
					list = new();
					ByOwner[entry.owner] = list;
				}

				if (brief)
					list.Add($"postfix({entry.priority})");
				else
					list.Add($"postfix: {ToTargetString(entry.PatchMethod)}, priority: {entry.priority}");
			}

			foreach (var entry in patches.Finalizers) {
				if (!ByOwner.TryGetValue(entry.owner, out var list)) {
					list = new();
					ByOwner[entry.owner] = list;
				}

				if (brief)
					list.Add($"finalizer({entry.priority})");
				else
					list.Add($"finalizer: {ToTargetString(entry.PatchMethod)}, priority: {entry.priority}");
			}

			foreach (var entry in ByOwner) {
				if (brief)
					sb.AppendLine($"  - {entry.Key} : {string.Join(", ", entry.Value)}");
				else {
					sb.AppendLine($"  - {entry.Key}");
					foreach (string le in entry.Value)
						sb.AppendLine($"    - {le}");
				}
			}
			sb.AppendLine("");
		}

		Log($"Theme Manager Patched Methods:\n{sb}", LogLevel.Info);
	}

	#endregion

}
