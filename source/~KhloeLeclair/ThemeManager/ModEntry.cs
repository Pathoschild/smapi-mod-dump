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
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Leclair.Stardew.ThemeManager.Patches;
using Leclair.Stardew.ThemeManager.Models;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common;

using SMAPIJsonHelper = StardewModdingAPI.Toolkit.Serialization.JsonHelper;
using System.Diagnostics;
using SkiaSharp;

namespace Leclair.Stardew.ThemeManager;

public class ModEntry : ModSubscriber {

	#nullable disable
	public static ModEntry Instance { get; private set; }

	internal ModConfig Config;
	#nullable enable

	internal Harmony? Harmony;
	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;
	internal Integrations.ContentPatcher.CPIntegration? intCP;

	internal bool ConfigStale = false;

	internal SMAPIJsonHelper? JsonHelper;

	internal readonly Dictionary<IManifest, IContentPack> ContentPacks = new();
	internal readonly Dictionary<string, Func<object>> LoadingAssets = new();
	internal readonly Dictionary<IManifest, (Type, IThemeManager)> Managers = new();
	internal readonly Dictionary<string, IThemeManager> ManagersByThemeAsset = new();
	internal readonly Dictionary<IManifest, ModAPI> APIs = new();

	internal Dictionary<string, PatchGroupData>? PatchGroups;

	internal ThemeManager<Models.BaseTheme>? BaseThemeManager;

	internal Models.BaseTheme? BaseTheme;

	internal readonly Dictionary<MethodInfo, DynamicPatcher> DynamicPatchers = new();

	#region Construction

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Instance = this;

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		// TODO: DigitEntryMenu
		// TODO: MuseumMenu
		// TODO: NumberSelectionMenu

		DayTimeMoneyBox_Patches.Patch(this);
		SpriteBatch_Patches.Patch(this);
		Patches.SpriteText_Patches.Patch(this);

		// Read Configuration
		Config = Helper.ReadConfig<ModConfig>();
		SpriteBatch_Patches.AlignText = Config.AlignText;

		// I18n
		I18n.Init(Helper.Translation);

		// Base Theme
		BaseTheme = BaseTheme.GetDefaultTheme();
		BaseThemeManager = new ThemeManager<BaseTheme>(
			mod: this,
			other: ModManifest,
			selectedThemeId: Config.StardewTheme ?? "automatic",
			manifestKey: "stardew:theme",
			defaultTheme: BaseTheme,
			themeLoaderPath: $"Mods/{ModManifest.UniqueID}/GameThemeData"
		);

		ManagersByThemeAsset[BaseThemeManager.ThemeLoaderPath] = BaseThemeManager;
		BaseThemeManager.ThemeChanged += OnStardewThemeChanged;
	}

	public override object? GetApi(IModInfo mod) {
		if (!APIs.TryGetValue(mod.Manifest, out var api)) {
			api = new ModAPI(this, mod.Manifest);
			APIs[mod.Manifest] = api;
		}

		return api;
	}

	#endregion

	#region Configuration

	internal void ResetConfig() {
		Config = new ModConfig();
		Patches.SpriteBatch_Patches.AlignText = Config.AlignText;
	}

	internal void SaveConfig() {
		Helper.WriteConfig(Config);
		Patches.SpriteBatch_Patches.AlignText = Config.AlignText;
	}

	[MemberNotNullWhen(true, nameof(intGMCM))]
	internal bool HasGMCM() {
		return intGMCM is not null && intGMCM.IsLoaded;
	}

	internal void OpenGMCM() {
		if (!HasGMCM())
			return;

		if (ConfigStale)
			RegisterSettings();

		intGMCM.OpenMenu();
	}

	internal void RegisterSettings() {
		intGMCM ??= new(this, () => Config, ResetConfig, SaveConfig);

		if (!intGMCM.IsLoaded)
			return;

		// Un-register and re-register so we can redo our settings.
		intGMCM.Unregister();
		intGMCM.Register(true);

		var choices = BaseThemeManager!.GetThemeChoiceMethods();

		intGMCM.AddChoice(
			name: I18n.Setting_GameTheme,
			tooltip: I18n.Setting_GameTheme_Tip,
			get: c => c.StardewTheme,
			set: (c,v) => {
				c.StardewTheme = v;
				BaseThemeManager!._SelectTheme(v);
			},
			choices: choices
		);

		intGMCM.AddLabel(I18n.Settings_ModThemes);

		foreach (var entry in Managers) {

			string uid = entry.Key.UniqueID;
			var mchoices = entry.Value.Item2.GetThemeChoiceMethods();

			string Getter(ModConfig cfg) {
				if (cfg.SelectedThemes.TryGetValue(uid, out string? value))
					return value;
				return "automatic";
			}

			void Setter(ModConfig cfg, string value) {
				cfg.SelectedThemes[uid] = value;
				if (Managers.TryGetValue(entry.Key, out var mdata) && mdata.Item2 is IThemeSelection tselect)
					tselect._SelectTheme(value);
			}

			intGMCM.AddChoice(
				name: () => entry.Key.Name,
				tooltip: null,
				get: Getter,
				set: Setter,
				choices: mchoices
			);
		}

		intGMCM.AddLabel(I18n.Setting_Advanced);

		intGMCM.Add(
			name: I18n.Setting_DebugPatches,
			tooltip: I18n.Setting_DebugPatches_Tip,
			get: c => c.DebugPatches,
			set: (c, v) => c.DebugPatches = v
		);

		intGMCM.Add(
			name: I18n.Setting_FixText,
			tooltip: I18n.Setting_FixText_Tip,
			get: c => c.AlignText,
			set: (c, v) => {
				c.AlignText = v;
				Patches.SpriteBatch_Patches.AlignText = v;
			}
		);

		var clock_choices = new Dictionary<string, Func<string>> {
			{ "by-theme", I18n.Setting_FromTheme },
			{ "top-left", I18n.Alignment_TopLeft },
			{ "top-center", I18n.Alignment_TopCenter },
			{ "default", I18n.Alignment_Default },
			{ "mid-left", I18n.Alignment_MidLeft },
			{ "mid-center", I18n.Alignment_MidCenter },
			{ "mid-right", I18n.Alignment_MidRight },
			{ "bottom-left", I18n.Alignment_BottomLeft },
			{ "bottom-center", I18n.Alignment_BottomCenter },
			{ "bottom-right", I18n.Alignment_BottomRight }
		};

		intGMCM.AddChoice(
			name: I18n.Setting_ClockPosition,
			tooltip: I18n.Setting_ClockPosition_Tip,
			get: c => {
				if (c.ClockMode == ClockAlignMode.Default)
					return "default";
				if (c.ClockMode == ClockAlignMode.ByTheme)
					return "by-theme";

				Alignment align = c.ClockAlignment ?? Alignment.None;

				if (align.HasFlag(Alignment.Middle)) {
					if (align.HasFlag(Alignment.Left))
						return "mid-left";
					if (align.HasFlag(Alignment.Center))
						return "mid-center";
					return "mid-right";

				} else if (align.HasFlag(Alignment.Bottom)) {
					if (align.HasFlag(Alignment.Left))
						return "bottom-left";
					if (align.HasFlag(Alignment.Center))
						return "bottom-center";
					return "bottom-right";
				}

				if (align.HasFlag(Alignment.Left))
					return "top-left";
				if (align.HasFlag(Alignment.Center))
					return "top-center";
				return "top-right";
			},
			set: (c, v) => {
				switch (v) {
					case "default":
						c.ClockMode = ClockAlignMode.Default;
						return;
					case "by-theme":
						c.ClockMode = ClockAlignMode.ByTheme;
						return;
				}

				Alignment align = Alignment.None;

				switch (v) {
					case "bottom-left":
					case "bottom-center":
					case "bottom-right":
						align |= Alignment.Bottom;
						break;
					case "mid-left":
					case "mid-center":
					case "mid-right":
						align |= Alignment.Middle;
						break;
					default:
						align |= Alignment.Top;
						break;
				}

				switch (v) {
					case "top-left":
					case "mid-left":
					case "bottom-left":
						align |= Alignment.Left;
						break;
					case "top-center":
					case "mid-center":
					case "bottom-center":
						align |= Alignment.Center;
						break;
					default:
						align |= Alignment.Right;
						break;
				}

				c.ClockMode = ClockAlignMode.Manual;
				c.ClockAlignment = align;
			},
			choices: clock_choices
		);

		ConfigStale = false;
	}

	#endregion

	#region Content Pack Access

	internal void GetJsonHelper() {
		if (JsonHelper is not null)
			return;

		if (Helper.Data.GetType().GetField("JsonHelper", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Helper.Data) is SMAPIJsonHelper helper) {
			JsonHelper = new();
			var converters = JsonHelper.JsonSettings.Converters;
			converters.Clear();
			foreach(var converter in helper.JsonSettings.Converters)
				if (converter.GetType().Name != "ColorConverter")
					converters.Add(converter);

			converters.Add(new Common.Serialization.Converters.ColorConverter());
		}
	}

	internal TModel? Clone<TModel>(TModel input) where TModel : class {
		if (JsonHelper is null)
			GetJsonHelper();

		if (JsonHelper is not null)
			return JsonHelper.Deserialize<TModel>(JsonHelper.Serialize(input));

		return null;
	}

	internal TModel? ReadJsonFile<TModel>(string path, IContentPack pack) where TModel : class {
		if (JsonHelper is null)
			GetJsonHelper();

		if (JsonHelper is not null) {
			if (JsonHelper.ReadJsonFileIfExists(Path.Join(pack.DirectoryPath, path), out TModel? result))
				return result;
			return null;
		}
		
		return pack.ReadJsonFile<TModel>(path);
	}

	internal IContentPack? GetContentPackFor(IManifest manifest) {
		if (ContentPacks.TryGetValue(manifest, out IContentPack? cp))
			return cp;

		IModInfo? info = Helper.ModRegistry.Get(manifest.UniqueID);
		if (info is null)
			return null;

		return GetContentPackFor(info);
	}

	internal IContentPack? GetContentPackFor(IModInfo mod) {
		if (ContentPacks.TryGetValue(mod.Manifest, out IContentPack? cp))
			return cp;

		if (mod.IsContentPack && mod.GetType().GetProperty("ContentPack", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is IContentPack pack)
			cp = pack;

		else if (mod.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is string str) {
			cp = Helper.ContentPacks.CreateTemporary(
				directoryPath: str,
				id: $"leclair.theme-loader.${mod.Manifest.UniqueID}",
				name: mod.Manifest.Name,
				description: mod.Manifest.Description,
				author: mod.Manifest.Author,
				version: mod.Manifest.Version
			);

		} else
			return null;

		ContentPacks[mod.Manifest] = cp;
		return cp;
	}

	#endregion

	#region Method Resolution

	internal string? GetEasyString(MethodInfo method) {
		string? type = method.DeclaringType?.FullName;
		if (type is null)
			return null;

		if (type.StartsWith("StardewValley.Menus."))
			type = $"#{type[20..]}";

		var parms = method.GetParameters();
		string[] args = new string[parms.Length];

		for (int i = 0; i < parms.Length; i++)
			args[i] = parms[i].ParameterType?.Name ?? string.Empty;

		string? assembly = method.DeclaringType?.Assembly.GetName().Name;
		if (assembly is null || assembly == "Stardew Valley")
			assembly = string.Empty;
		else
			assembly = $"{assembly}!";

		return $"{assembly}{type}:{method.Name}({string.Join(',', args)})";
	}

	internal (Type, MethodInfo)? ResolveMethod(string input, Type? current = null) {
		var result = ResolveMethods(input, current);
		return result.FirstOrDefault();
	}

	internal IEnumerable<(Type, MethodInfo)> ResolveMethods(string input, Type? current = null) {
		if (input == null)
			yield break;

		string? assemblyName;
		string typeName;
		string methodName;

		int idx = input.IndexOf(':');
		if (idx == -1)
			methodName = "draw";
		else {
			methodName = input[(idx + 1)..];
			input = input[..idx];
		}

		if (string.IsNullOrWhiteSpace(input)) {
			// If we don't have a class and we don't have a current target... boo.
			if (current is null)
				yield break;

			assemblyName = current.Assembly.GetName().Name;
			typeName = current.FullName ?? current.Name;
		} else {
			idx = input.IndexOf('!');
			if (idx == -1) {
				assemblyName = "Stardew Valley";
				typeName = input;
			} else {
				assemblyName = input[..idx];
				typeName = input[(idx + 1)..];
			}
		}

		if (typeName.StartsWith('#'))
			typeName = $"StardewValley.Menus.{typeName[1..]}";

		string[]? types = null;
		if (methodName.EndsWith(')')) {
			idx = methodName.IndexOf('(');

			if (idx != -1) {
				types = methodName[(idx + 1)..(methodName.Length - 1)].Split(',');
				methodName = methodName[..idx];
			}
		}

		foreach (Assembly assembly in AccessTools.AllAssemblies()) {
			if (!string.Equals(assemblyName, assembly.GetName().Name))
				continue;

			foreach (Type type in AccessTools.GetTypesFromAssembly(assembly)) {
				if (!string.Equals(type.FullName, typeName))
					continue;

				var methods = AccessTools.GetDeclaredMethods(type);

				foreach (var method in methods) {
					if (method is null || !method.Name.Equals(methodName))
						continue;

					if (types is not null) {
						var parms = method.GetParameters();
						if (parms.Length != types.Length)
							continue;

						bool valid = true;
						for (int i = 0; i < types.Length; i++) {
							string inp = types[i];
							if (string.IsNullOrEmpty(inp))
								continue;

							var parm = parms[i];
							if (!string.Equals(parm.ParameterType.FullName, inp, StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(parm.ParameterType.Name, inp, StringComparison.OrdinalIgnoreCase)) {
								valid = false;
								break;
							}
						}

						if (!valid)
							continue;
					}

					yield return (type, method);
				}
			}
		}
	}

	#endregion

	#region Patch Group Handling

	/// <summary>
	/// Load patch group data from disk and populate <see cref="PatchGroups"/>.
	/// If <see cref="PatchGroups"/> is already populated, this does nothing.
	/// </summary>
	[MemberNotNull(nameof(PatchGroups))]
	internal void LoadPatchGroups() {
		if (PatchGroups is not null)
			return;

		var result = new Dictionary<string, PatchGroupData>();

		// Load from our main assets.
		string patches_path = Path.Join(Helper.DirectoryPath, "assets", "patches");
		int loaded = _LoadPatchesFrom(result, Helper.DirectoryPath, Path.Join("assets", "patches"), Helper.ModContent);
		if (loaded == 0)
			Log($"Unable to load patches from {patches_path}. This indicates a broken installation and Stardew themes may not work correctly.", LogLevel.Warn);

		// Now load for each of our content packs.
		int packs = 0;
		int packloaded = 0;

		foreach(var cp in Helper.ContentPacks.GetOwned()) {
			int count = _LoadPatchesFrom(result, cp.DirectoryPath, "patches", cp.ModContent);
			if (count > 0) {
				packs++;
				packloaded += count;
			}
		}

		PatchGroups = result;
		Log($"Loaded {PatchGroups.Count} patch groups. ({loaded} base assets, {packloaded} from {packs} content packs)", LogLevel.Debug);
	}

	/// <summary>
	/// This method actually loads patch group data from files, and should not
	/// be called by anything other than <see cref="LoadPatchGroups"/>
	/// </summary>
	/// <param name="store">The dictionary to store loaded patch groups into.</param>
	/// <param name="root">The root file path for load operations from the <see cref="IModContentHelper"/></param>
	/// <param name="prefix">The prefix where we should search for patch group assets at.</param>
	/// <param name="helper">A content helper for loading assets.</param>
	/// <returns>The number of loaded patch group data files.</returns>
	private int _LoadPatchesFrom(Dictionary<string, PatchGroupData> store, string root, string prefix, IModContentHelper helper) {
		string path = Path.Join(root, prefix);
		if (!Directory.Exists(path))
			return 0;

		int count = 0;

		foreach(string file in Directory.EnumerateFiles(path, "*.json")) {
			string relative = Path.GetRelativePath(root, file);
			PatchGroupData? data;
			try {
				data = helper.Load<PatchGroupData>(relative);

			} catch(Exception ex) {
				Log($"Unable to read patch data from {file}: {ex}", LogLevel.Error);
				continue;
			}

			if (data is null)
				continue;

			// Check to see if all the required mods are present.
			data.CanUse = true;
			if (data.RequiredMods is not null)
				foreach(var entry in data.RequiredMods) {
					var info = Helper.ModRegistry.Get(entry.UniqueID);
					if (info is null) {
						data.CanUse = false;
						break;
					}

					bool between;
					try {
						between = info.Manifest.Version.IsBetween(
							string.IsNullOrWhiteSpace(entry.MinimumVersion) ?
								info.Manifest.Version : new SemanticVersion(entry.MinimumVersion),
							string.IsNullOrWhiteSpace(entry.MaximumVersion) ?
								info.Manifest.Version : new SemanticVersion(entry.MaximumVersion)
						);
					} catch(Exception ex) {
						Log($"An error occurred while checking the version of a required mod for patch data {data.ID}", LogLevel.Warn, ex);
						between = false;
					}

					if (!between) { 
						data.CanUse = false;
						break;
					}
				}

			if (string.IsNullOrWhiteSpace(data.ID))
				data.ID = Path.GetFileNameWithoutExtension(relative);

			if (store.TryAdd(data.ID, data))
				count++;
			else
				Log($"Duplicate key loading patch data {data.ID}. Ignoring from {file}");
		}

		return count;
	}


	#endregion

	#region Console Commands

	[ConsoleCommand("tm_menu_colors", "View all of the detected colors used in a class's draw() method (or another listed method)")]
	private void Command_MenuColors(string _, string[] args) {
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
			foreach(var m in Game1.onScreenMenus) {
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
		var result = ResolveMethod(string.Join(' ', args), current: menu?.GetType());

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

		Dictionary<FieldInfo, string> fields = new();

		foreach (string name in new string[] {
				"bgColor",
				"textColor",
				"textShadowColor",
				"unselectedOptionColor"
			}) {
			FieldInfo? field = typeof(Game1).GetField(name);
			if (field is not null)
				fields.Add(field, field.Name);
		}

		var r2gLerp = AccessTools.Method(typeof(Utility), nameof(Utility.getRedToGreenLerpColor));

		Dictionary<MethodInfo, string> colors = new();
		foreach (var entry in typeof(Color).GetProperties()) {
			if (entry.Name.Equals("White") || entry.Name.Equals("Black"))
				continue;

			if (entry.GetGetMethod() is MethodInfo method) {
				colors.Add(method, entry.Name);
			}
		}

		Log($"Method: {GetEasyString(info)}", LogLevel.Info);
		Log($"Class: {type.FullName}", LogLevel.Trace);
		Log($"Method (Raw): {info.FullDescription()}", LogLevel.Trace);
		Log($"Detected Colors:", LogLevel.Info);

		bool found = false;

		Dictionary<string, List<int>> Colors = new();
		Dictionary<string, List<int>> RawColors = new();
		Dictionary<string, List<int>> Fields = new();
		List<int> RedToGreenLerps = new();

		for (int i = 0; i < Instructions.Count; i++) {
			CodeInstruction in0 = Instructions[i];

			//Log($"{i}: {in0}", LogLevel.Debug);

			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo method && colors.TryGetValue(method, out string? name)) {
				if (!Colors.TryGetValue(name, out var list)) {
					list = new();
					Colors[name] = list;
				}
				list.Add(i);
				found = true;
			}

			if (in0.opcode == OpCodes.Call && in0.operand is MethodInfo meth && meth == r2gLerp) {
				RedToGreenLerps.Add(i);
				found = true;
			}

			if (in0.opcode == OpCodes.Ldsfld && in0.operand is FieldInfo fld && fields.TryGetValue(fld, out string? fname)) {
				if (!Fields.TryGetValue(fname, out var list)) {
					list = new();
					Fields[fname] = list;
				}
				list.Add(i);
				found = true;
			}

			if (i + 3 < Instructions.Count) {
				CodeInstruction in1 = Instructions[i + 1];
				CodeInstruction in2 = Instructions[i + 2];
				CodeInstruction in3 = Instructions[i + 3];

				if (in3.opcode == OpCodes.Newobj && in3.operand is ConstructorInfo ctor && ctor.DeclaringType == typeof(Color)) {
					int? val0 = in0.AsInt();
					int? val1 = in1.AsInt();
					int? val2 = in2.AsInt();

					if (val0.HasValue && val1.HasValue && val2.HasValue) {
						string key = $"{val0.Value}, {val1.Value}, {val2.Value}";
						if (!RawColors.TryGetValue(key, out var list)) {
							list = new();
							RawColors[key] = list;
						}
						list.Add(i);
						found = true;
					}
				}
			}
		}

		if (!found)
			Log($"- Did not find any colors.", LogLevel.Info);
		if (Colors.Count > 0) {
			Log($"- Colors:", LogLevel.Info);
			foreach (var entry in Colors)
				Log($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})", LogLevel.Info);
		}
		if (RawColors.Count > 0) {
			Log($"- Raw Colors:", LogLevel.Info);
			foreach (var entry in RawColors)
				Log($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})", LogLevel.Info);
		}
		if (Fields.Count > 0) {
			Log($"- Fields:", LogLevel.Info);
			foreach (var entry in Fields)
				Log($"  - {entry.Key} (Offsets: {string.Join(", ", entry.Value)})", LogLevel.Info);
		}
		if (RedToGreenLerps.Count > 0)
			Log($"- RedToGreenLerp (Offsets: {string.Join(", ", RedToGreenLerps)})", LogLevel.Info);

		/*Log($"Instructions:");
		for (int i = 0; i < Instructions.Length; i++) {
			CodeInstruction in0 = Instructions[i];
			Log($"{i,4} {in0}");
		}*/
	}

	[ConsoleCommand("tm_toggle_font_fix", "Toggle Theme Manager's font alignment fix.")]
	private void Command_ToggleFontFix(string name, string[] args) {
		Config.AlignText = !Config.AlignText;
		SaveConfig();

		Log($"Font alignment has been set to {Config.AlignText}", LogLevel.Info);
	}

	[ConsoleCommand("theme", "View available themes, reload themes, and change the current themes.")]
	private void Command_Theme(string name, string[] args) {
		// List Mods
		if (args.Length == 0 || string.Equals("list", args[0], StringComparison.OrdinalIgnoreCase)) {
			List<string[]> ents = new() {
				new string[] {
					"stardew",
					BaseThemeManager!.ActiveThemeId,
					BaseThemeManager!.SelectedThemeId,
					BaseThemeManager!.GetThemeChoices().Count.ToString()
				}
			};

			foreach (var entry in Managers)
				ents.Add(new string[] {
					entry.Key.UniqueID,
					entry.Value.Item2.ActiveThemeId,
					entry.Value.Item2.SelectedThemeId,
					entry.Value.Item2.GetThemeChoices().Count.ToString()
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
		string? target = null;

		// Strict matching
		if (string.Equals(args[0], "stardew", StringComparison.OrdinalIgnoreCase)) {
			target = "stardew";
			manager = BaseThemeManager;
		} else {
			foreach (var entry in Managers) {
				if (string.Equals(args[0], entry.Key.UniqueID, StringComparison.OrdinalIgnoreCase)) {
					manager = entry.Value.Item2;
					target = entry.Key.UniqueID;
					break;
				}
			}
		}

		// Sloppy matching.
		if (manager is null) {
			if ("stardew".Contains(args[0], StringComparison.OrdinalIgnoreCase)) {
				manager = BaseThemeManager;
				target = "stardew";
			} else {
				foreach (var entry in Managers) {
					if (entry.Key.UniqueID.Contains(args[0], StringComparison.OrdinalIgnoreCase)) {
						manager = entry.Value.Item2;
						target = entry.Key.UniqueID;
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

		Log($"Manager: {target}", LogLevel.Info);

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
			var themes = manager.GetThemeChoices();

			// Check for unique ID matches first.
			foreach (var pair in themes) {
				if (pair.Key.Equals(needle, StringComparison.OrdinalIgnoreCase)) {
					selected = pair.Key;
					target = pair.Value;
					break;
				}
			}

			// Now check for unique ID partial matches.
			if (selected is null)
				foreach (var pair in themes) {
					if (pair.Key.Contains(needle, StringComparison.OrdinalIgnoreCase)) {
						selected = pair.Key;
						target = pair.Value;
						break;
					}
				}

			// Lastly select for partial display name matches
			if (selected is null)
				foreach (var pair in themes) {
					if (pair.Value.Contains(needle, StringComparison.OrdinalIgnoreCase)) {
						selected = pair.Key;
						target = pair.Value;
						break;
					}
				}

			if (selected != null) {
				manager.SelectTheme(selected);
				Log($"Selected Theme: {selected} ({target})", LogLevel.Info);

			} else
				Log($"Unable to match theme: {needle}", LogLevel.Warn);
		}

		List<string[]> entries = new();

		foreach(var pair in manager.GetThemeChoices()) {
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

		SelectPatches(BaseTheme);

		Log($"Reloaded {PatchGroups.Count} patch groups and applied patches to {DynamicPatchers.Count} methods.", LogLevel.Info);
	}

	[ConsoleCommand("retheme", "Reload all themes.")]
	private void Command_ReTheme(string name, string[] args) {

		BaseThemeManager!.Discover();
		BaseThemeManager!.Invalidate();

		foreach (var entry in Managers) {
			entry.Value.Item2.Discover();
			entry.Value.Item2.Invalidate();
		}

		Log($"Reloaded all themes across {Managers.Count + 1} managers.", LogLevel.Info);
	}

	#endregion

	#region Events

	private Color? RasterizeColor(string input, Dictionary<string, string> values, Dictionary<string, Color> parsed) {
		CaseInsensitiveHashSet visited = new();
		while (input is not null) {
			if (!visited.Add(input)) {
				Log($"Infinite loop detected resolving color: {string.Join(" -> ", visited)}", LogLevel.Warn);
				return null;
			}

			if (input.StartsWith('$')) {
				string key = input[1..];
				if (parsed.TryGetValue(key, out var result))
					return result;

				if (!values.TryGetValue(key, out string? value))
					return null;

				input = value;

			} else if (CommonHelper.TryParseColor(input, out var res)) { 
				return res;

			} else {
				Log($"Unable to parse color: {input}", LogLevel.Warn);
				return null;
			}
		}

		return null;
	}

	private void SelectPatches(BaseTheme? theme) {
		LoadPatchGroups();

		// Reset our existing patches.
		foreach (var entry in DynamicPatchers.Values)
			entry.ClearPatches();

		// Update our patches.
		if (theme is not null)
			foreach (string key in theme.Patches) {
				if (!PatchGroups.TryGetValue(key, out var patch) || !patch.CanUse || patch.Patches is null)
					continue;

				patch.Methods ??= new();

				foreach (var entry in patch.Patches) {
					if (!patch.Methods.TryGetValue(entry.Key, out var methods)) {
						methods = ResolveMethods(entry.Key, null).Select(x => x.Item2).ToArray();
						patch.Methods[entry.Key] = methods;
					}

					foreach (var method in methods) {
						if (!DynamicPatchers.TryGetValue(method, out var patcher)) {
							patcher = new DynamicPatcher(this, method, entry.Key);
							DynamicPatchers.Add(method, patcher);
						}

						patcher.AddPatch(entry.Value);
					}

					if (methods.Length == 0)
						Log($"Unable to apply method patch for patch {key}. Cannot find matching method: {entry.Key}", LogLevel.Warn);
				}
			}

		// Update all the patches, and remove ones that are no longer active.
		var patchers = DynamicPatchers.Values.ToArray();
		foreach (var patcher in patchers) {
			if (!patcher.Update())
				DynamicPatchers.Remove(patcher.Method);
		}
	}

	private void OnStardewThemeChanged(object? sender, IThemeChangedEvent<BaseTheme> e) {
		BaseTheme = e.NewData;

		// Access SpriteTextColors to force all the theme's data to build.
		int _ = BaseTheme.SpriteTextColors.Count;

		// Apply the text color / text shadow color to the fields in Game1.
		Game1.textColor = BaseTheme.Variables.GetValueOrDefault("Text", BaseThemeManager!.DefaultTheme.Variables["Text"]);
		Game1.textShadowColor = BaseTheme.Variables.GetValueOrDefault("TextShadow", BaseThemeManager!.DefaultTheme.Variables["TextShadow"]);
		Game1.unselectedOptionColor = BaseTheme.Variables.GetValueOrDefault("UnselectedOption", BaseThemeManager!.DefaultTheme.Variables["UnselectedOption"]);

		SelectPatches(BaseTheme);
		DynamicPatcher.UpdateColors(BaseTheme.Variables);
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		// Integrations
		intCP = new(this);
		CheckRecommendedIntegrations();

		// Load Patches
		LoadPatchGroups();
		BaseThemeManager!.Discover();

		// Settings
		RegisterSettings();
		Helper.Events.Display.RenderingActiveMenu += OnDrawMenu;
	}

	private void OnDrawMenu(object? sender, RenderingActiveMenuEventArgs e) {
		// Rebuild our settings menu when first drawing the title menu, since
		// the MenuChanged event doesn't handle the TitleMenu.
		Helper.Events.Display.RenderingActiveMenu -= OnDrawMenu;

		if (ConfigStale)
			RegisterSettings();
	}

	[Subscriber]
	private void OnMenuChanged(object? sender, MenuChangedEventArgs e) {
		IClickableMenu? menu = e.NewMenu;
		if (menu is null)
			return;

		Type type = menu.GetType();
		string? name = type.FullName ?? type.Name;

		if (name is not null && name.Equals("GenericModConfigMenu.Framework.ModConfigMenu")) {
			if (ConfigStale)
				RegisterSettings();
		}
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach(var entry in e.Names) {
			if (ManagersByThemeAsset.TryGetValue(entry.Name, out var manager) && manager is IThemeSelection tselect)
				tselect.InvalidateThemeData();
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		Func<object>? loader;
		lock ((LoadingAssets as ICollection).SyncRoot) {
			if (!LoadingAssets.TryGetValue(e.Name.BaseName, out loader))
				return;
		}

		e.LoadFrom(
			loader,
			priority: AssetLoadPriority.Low
		);
	}

	#endregion

}
