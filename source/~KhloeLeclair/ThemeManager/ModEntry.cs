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
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using HarmonyLib;

using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Leclair.Stardew.Common;

using Leclair.Stardew.ThemeManager.Models;
using Leclair.Stardew.ThemeManager.Patches;
using Leclair.Stardew.ThemeManager.Serialization;

using SMAPIJsonHelper = StardewModdingAPI.Toolkit.Serialization.JsonHelper;
using Leclair.Stardew.ThemeManager.Managers;
using Leclair.Stardew.ThemeManager.VariableSets;
using Nanoray.Pintail;
using System.Security.AccessControl;
using Leclair.Stardew.Common.Extensions;

namespace Leclair.Stardew.ThemeManager;

public partial class ModEntry : ModSubscriber {

	#nullable disable
	public static ModEntry Instance { get; private set; }
	internal ModConfig Config;
	#nullable enable

	#region Fields - Integrations

	internal Integrations.ContentPatcher.CPIntegration? intCP;

	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;
	internal bool ConfigStale = false;

	#endregion

	#region Fields - Content Packs + JsonHelper

	internal SMAPIJsonHelper? JsonHelper;

	internal readonly Dictionary<IManifest, IContentPack> ContentPacks = new();

	#endregion

	#region Fields - Theme Manager Storage & APIs

	internal ThemeManager<GameTheme>? GameThemeManager;
	internal GameTheme? GameTheme;

	internal readonly Dictionary<IManifest, (Type, IThemeManager)> Managers = new();

	internal readonly Dictionary<string, IThemeManagerInternal> ManagersByThemeAsset = new();
	internal readonly Dictionary<string, IThemeManagerInternal> ManagersByAssetPrefix = new();

	internal readonly Dictionary<IManifest, ModAPI> APIs = new();

	#endregion

	#region Fields - Patches

	internal Harmony? Harmony;

	internal Dictionary<string, PatchGroupData>? PatchGroups;

	internal readonly Dictionary<MethodInfo, DynamicPatcher> DynamicPatchers = new();

	#endregion

	#region Fields - Managed Assets

	internal readonly Dictionary<IAssetName, WeakReference<IManagedAsset>> ManagedAssets = new();
	internal readonly Dictionary<IAssetName, Type> AssetTypes = new();
	internal readonly Dictionary<IAssetName, string?> AssetExtensions = new();

	#endregion

	#region Fields - Non-Theme Managers

	// Temporarily disable nullable because these should always be set before
	// any other code runs.
	#nullable disable

	internal SpriteFontManager SpriteFontManager;
	internal SpriteTextManager SpriteTextManager;

	#nullable enable

	#endregion

	#region Fields - GameContentManager.DoesAssetExist<T>()

	internal delegate bool GCMDoesAssetExist<T>(IAssetName assetName);

	internal bool GameContentManager_Loaded;
	internal object? GameContentManager_Instance;
	internal MethodInfo? GameContentManager_DoesAssetExist;
	internal Hashtable? GameContentManager_Delegates;

	#endregion

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
		Game1_Patches.Patch(this);
		SpriteText_Patches.Patch(this);
		OptionsDropDown_Patches.Patch(this);

		// Read Configuration
		Config = Helper.ReadConfig<ModConfig>();
		SpriteBatch_Patches.AlignText = Config.AlignText;

		// I18n
		I18n.Init(Helper.Translation);

		// Managers
		SpriteFontManager = new(this);
		SpriteTextManager = new(this);

		// Base Theme Manager
		GameTheme = GameTheme.GetDefaultTheme();
		GameThemeManager = new ThemeManager<GameTheme>(
			mod: this,
			other: ModManifest,
			selectedThemeId: Config.StardewTheme ?? "automatic",
			manifestKey: "stardew:theme",
			defaultTheme: GameTheme,
			assetLoaderPrefix: $"Mods/{ModManifest.UniqueID}/GameThemes"
		);

		ManagersByAssetPrefix[GameThemeManager.AssetLoaderPrefix] = GameThemeManager;
		ManagersByThemeAsset[GameThemeManager.ThemeLoaderPath] = GameThemeManager;

		GameThemeManager.ThemeChanged += OnStardewThemeChanged;
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

		var choices = GameThemeManager!.GetThemeChoiceMethods();

		intGMCM.AddChoice(
			name: I18n.Setting_GameTheme,
			tooltip: I18n.Setting_GameTheme_Tip,
			get: c => c.StardewTheme,
			set: (c,v) => {
				c.StardewTheme = v;
				GameThemeManager!._SelectTheme(v);
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
				if (Managers.TryGetValue(entry.Key, out var mdata) && mdata.Item2 is IThemeManagerInternal tselect)
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

			//converters.Add(new VariableSetConverter());
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

	/// <summary>
	/// Generate a string for targeting a member of a type.
	/// </summary>
	/// <param name="member">The member we want a string for.</param>
	/// <param name="replaceMenuWithHash">When true, if a type name starts with
	/// <c>StardewValley.Menus.</c> it will be replaced with a <c>#</c> for
	/// the sake of brevity.</param>
	/// <param name="includeTypes">Whether or not a type list should be
	/// included. By default, this is null and a list will be included only
	/// if the member is a constructor or method AND there is potential
	/// ambiguity in selecting the correct method.</param>
	internal string? ToTargetString(MemberInfo member, bool replaceMenuWithHash = true, bool? includeTypes = null) {
		string? type = member.DeclaringType?.FullName;
		if (type is null)
			return null;

		if (type.StartsWith("StardewValley.Menus.") && replaceMenuWithHash)
			type = $"#{type[20..]}";

		if (!includeTypes.HasValue) {
			if (member is ConstructorInfo) {
				var ctors = member.DeclaringType is not null ?
					AccessTools.GetDeclaredConstructors(member.DeclaringType)
					: null;

				includeTypes = ctors is null || ctors.Count > 1 || ctors[0] != member;

			} else if (member is MethodBase) {
				var meths = member.DeclaringType is not null ?
					AccessTools.GetDeclaredMethods(member.DeclaringType).Where(x => x.Name.Equals(member.Name)).ToArray()
					: null;

				includeTypes = meths is null || meths.Length > 1 || meths[0] != member;

			} else
				includeTypes = false;
		}

		string? argumentList;

		if (includeTypes.Value) {
			string[] args;

			if (member is MethodBase method) {
				var parms = method.GetParameters();
				args = new string[parms.Length];

				for (int i = 0; i < parms.Length; i++)
					args[i] = parms[i].ParameterType?.Name ?? string.Empty;
			} else if (member is PropertyInfo prop)
				args = new string[] {
				prop.PropertyType?.Name ?? string.Empty
			};
			else if (member is FieldInfo field)
				args = new string[] {
				field.FieldType?.Name ?? string.Empty
			};
			else
				args = Array.Empty<string>();

			argumentList = $"({string.Join(',', args)})";
		} else
			argumentList = string.Empty;

		string? assembly = member.DeclaringType?.Assembly.GetName().Name;
		if (assembly is null || assembly == "Stardew Valley")
			assembly = string.Empty;
		else
			assembly = $"{assembly}!";

		return $"{assembly}{type}:{member.Name}{argumentList}";
	}

	internal (Type, TValue)? ResolveMember<TValue>(string input, Type? current = null) where TValue : MemberInfo {
		var result = ResolveMembers<TValue>(input, current);
		return result.FirstOrDefault();
	}

	internal IEnumerable<(Type, TValue)> ResolveMembers<TValue>(string input, Type? current = null) where TValue : MemberInfo {
		if (input == null)
			yield break;

		string? assemblyName;
		string typeName;
		string entryName;

		int idx = input.IndexOf(':');
		if (idx == -1) {
			if (typeof(TValue) == typeof(ConstructorInfo)) {
				// Do nothing?
				entryName = string.Empty;
			} else if (typeof(TValue) == typeof(MethodInfo)) {
				// For methods, default to "draw"
				entryName = "draw";
			} else {
				// For everything else, default to using that name.
				entryName = input;
				input = string.Empty;
			}
		} else {
			entryName = input[(idx + 1)..];
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
		else if (typeName.Equals("Game1"))
			typeName = "StardewValley.Game1";

		string[]? types = null;
		if (entryName.EndsWith(')')) {
			idx = entryName.IndexOf('(');

			if (idx != -1) {
				types = entryName[(idx + 1)..(entryName.Length - 1)].Split(',');
				entryName = entryName[..idx];

				if (types.Length > 1 && typeof(TValue) != typeof(MethodInfo))
					yield break;
			}
		}

		foreach (Assembly assembly in AccessTools.AllAssemblies()) {
			if (!string.Equals(assemblyName, assembly.GetName().Name))
				continue;

			foreach (Type type in AccessTools.GetTypesFromAssembly(assembly)) {
				if (!string.Equals(type.FullName, typeName))
					continue;

				List<TValue>? members;

				if (typeof(TValue) == typeof(MethodInfo))
					members = AccessTools.GetDeclaredMethods(type) as List<TValue>;
				else if (typeof(TValue) == typeof(FieldInfo))
					members = AccessTools.GetDeclaredFields(type) as List<TValue>;
				else if (typeof(TValue) == typeof(PropertyInfo))
					members = AccessTools.GetDeclaredProperties(type) as List<TValue>;
				else if (typeof(TValue) == typeof(ConstructorInfo))
					members = AccessTools.GetDeclaredConstructors(type) as List<TValue>;
				else
					yield break;

				if (members is null)
					continue;

				foreach(var member in members) {
					if (member is null || !member.Name.Equals(entryName))
						continue;

					if (types is not null) {
						if (member is MethodBase method) {
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

						} else if (member is PropertyInfo || member is FieldInfo) {
							var mtype = member is PropertyInfo prop ? prop.PropertyType : member is FieldInfo field ? field.FieldType : null;
							if (mtype is null)
								continue;

							bool valid = false;
							for (int i = 0; i < types.Length; i++) {
								string inp = types[i];
								if (string.IsNullOrEmpty(inp))
									continue;
								if (string.Equals(mtype.FullName, inp, StringComparison.OrdinalIgnoreCase) ||
									string.Equals(mtype.Name, inp, StringComparison.OrdinalIgnoreCase)) {
									valid = true;
									break;
								}
							}

							if (!valid)
								continue;
						} else
							continue;
					}

					yield return (type, member);
				}
			}
		}
	}

	#endregion

	#region Pin the Tail on Pintail

	private IProxyManager<string>? SMAPI_ProxyManager;

	/// <summary>
	/// Grab SMAPI's ProxyManager so we use the same Pintail instances as
	/// SMAPI's API proxying.
	/// </summary>
	public IProxyManager<string>? GetProxyManager() {
		if (SMAPI_ProxyManager is not null)
			return SMAPI_ProxyManager;

		try {
			var field = AccessTools.Field(Helper.ModRegistry.GetType(), "ProxyFactory");
			object? InterfaceProxyFactory = field.GetValue(Helper.ModRegistry);
			if (InterfaceProxyFactory is null)
				throw new ArgumentNullException(nameof(InterfaceProxyFactory));

			field = AccessTools.Field(InterfaceProxyFactory.GetType(), "ProxyManager");
			object? ProxyManager = field.GetValue(InterfaceProxyFactory);

			if (ProxyManager is IProxyManager<string> pms)
				SMAPI_ProxyManager = pms;
			else
				throw new ArgumentException(nameof(ProxyManager));

		} catch(Exception ex) {
			Log($"Unable to grab ProxyManager from SMAPI: {ex}", LogLevel.Error);
		}

		return SMAPI_ProxyManager;
	}

	public bool CanProxy<T>(Type destinationType, string destinationModId) {
		return CanProxy(typeof(T), destinationType, destinationModId);
	}

	public bool CanProxy(Type sourceType, Type destinationType, string destinationModId) {
		return CanProxy(sourceType, ModManifest.UniqueID, destinationType, destinationModId);
	}

	public bool CanProxy(Type sourceType, string sourceModId, Type destinationType, string destinationModId) {
		if (sourceModId == destinationModId)
			return false;

		var proxy = GetProxyManager();
		if (proxy is null)
			return false;

		try {
			// Try to un-proxy
			foreach (Type itype in sourceType.GetInterfacesRecursively(includingSelf: true)) {
				var unfactory = proxy.GetProxyFactory(new ProxyInfo<string>(
					target: new TypeInfo<string>(sourceModId, destinationType),
					proxy: new TypeInfo<string>(destinationModId, itype)
				));

				if (unfactory is null)
					continue;

				return true;
			}

			proxy.ObtainProxyFactory(new ProxyInfo<string>(
				target: new TypeInfo<string>(sourceModId, sourceType),
				proxy: new TypeInfo<string>(destinationModId, destinationType)
			));

			return true;
		} catch {
			return false;
		}
	}

	public bool TryProxyRemote<T>(object? sourceInstance, string sourceModId, [NotNullWhen(true)] out T? destinationInstance, bool silent = false, Type? sourceType = null) {
		if (TryProxy(sourceInstance, sourceModId, typeof(T), ModManifest.UniqueID, out object? obj, silent: silent, sourceType: sourceType) && obj is T tobj) {
			destinationInstance = tobj;
			return true;
		}

		destinationInstance = default;
		return false;
	}

	public bool TryProxy(object? sourceInstance, string sourceModId, Type destinationType, string destinationModId, [NotNullWhen(true)] out object? destinationInstance, bool silent = false, Type? sourceType = null) {
		var proxy = GetProxyManager();
		if (sourceModId == destinationModId || proxy is null || sourceInstance is null) {
			destinationInstance = null;
			return false;
		}

		try {
			sourceType ??= sourceInstance.GetType();

			// Short circuit Pintail proxies if we can.
			if (sourceType.GetField("__Target", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo field && destinationType.IsAssignableFrom(field.FieldType)) {
				destinationInstance = field.GetValue(sourceInstance);
				if (destinationInstance is not null)
					return true;
			}

			// Try to un-proxy
			foreach (Type itype in sourceType.GetInterfacesRecursively(includingSelf: true)) {
				var unfactory = proxy.GetProxyFactory(new ProxyInfo<string>(
					target: new TypeInfo<string>(sourceModId, destinationType),
					proxy: new TypeInfo<string>(destinationModId, itype)
				));

				if (unfactory is null)
					continue;

				if (unfactory.TryUnproxy(proxy, sourceInstance, out object? unproxied)) {
					destinationInstance = unproxied;
					return true;
				}
			}

			var factory = proxy.ObtainProxyFactory(new ProxyInfo<string>(
				target: new TypeInfo<string>(sourceModId, sourceType),
				proxy: new TypeInfo<string>(destinationModId, destinationType)
			));

			destinationInstance = factory.ObtainProxy(proxy, sourceInstance);
			return true;

		} catch(Exception ex) {
			if (!silent)
				Log($"Unable to proxy type {sourceInstance.GetType()} to {destinationType}: {ex}", LogLevel.Debug);
			destinationInstance = null;
			return false;
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
					if (!entry.Matches(info)) {
						data.CanUse = false;
						break;
					}
				}

			if (data.ForbiddenMods is not null)
				foreach(var entry in data.ForbiddenMods) {
					var info = Helper.ModRegistry.Get(entry.UniqueID);
					if (entry.Matches(info)) {
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

	#region Events

	private void SelectPatches(GameTheme? theme) {
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
						methods = ResolveMembers<MethodInfo>(entry.Key, null).Select(x => x.Item2).ToArray();
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

	private void OnStardewThemeChanged(object? sender, IThemeChangedEvent<GameTheme> e) {
		GameTheme = e.NewData;

		GameTheme.ColorVariables ??= new ColorVariableSet();
		GameTheme.FontVariables ??= new FontVariableSet();
		GameTheme.TextureVariables ??= new TextureVariableSet();
		GameTheme.BmFontVariables ??= new BmFontVariableSet();

		// Assign the patch variables.
		GameTheme.ColorVariables.DefaultValues = GameTheme.PatchColorVariables;
		GameTheme.FontVariables.DefaultValues = GameTheme.PatchFontVariables;
		GameTheme.TextureVariables.DefaultValues = GameTheme.PatchTextureVariables;
		GameTheme.BmFontVariables.DefaultValues = GameTheme.PatchBmFontVariables;

		// Access SpriteTextColors to force all the theme's data to build.
		int _ = GameTheme.SpriteTextColors.Count;

		// Apply the text color / text shadow color to the fields in Game1.
		Game1.textColor = GameTheme.ColorVariables.GetValueOrDefault("Text", GameThemeManager!.DefaultTheme.ColorVariables["Text"]);
		Game1.textShadowColor = GameTheme.ColorVariables.GetValueOrDefault("TextShadow", GameThemeManager!.DefaultTheme.ColorVariables["TextShadow"]);
		Game1.unselectedOptionColor = GameTheme.ColorVariables.GetValueOrDefault("UnselectedOption", GameThemeManager!.DefaultTheme.ColorVariables["UnselectedOption"]);

		// Apply the font fields in Game1.
		SpriteFontManager!.AssignFonts(GameTheme);
		SpriteTextManager!.AssignFonts(GameTheme);

		// Apply the patches.
		SelectPatches(GameTheme);

		// Update the values used by the patches.
		DynamicPatcher.UpdateColors(GameTheme.ColorVariables);
		DynamicPatcher.UpdateFonts(GameTheme.FontVariables);
		DynamicPatcher.UpdateTextures(GameTheme.TextureVariables);
		DynamicPatcher.UpdateBmFonts(GameTheme.BmFontVariables);
	}


	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		// Integrations
		intCP = new(this);
		CheckRecommendedIntegrations();

		// Load Patches
		LoadPatchGroups();
		GameThemeManager!.Discover();

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

	#endregion

	#region Managed Assets

	public bool TryGetManagedAsset<T>(string assetName, [NotNullWhen(true)] out IManagedAsset<T>? managedAsset) where T : notnull {
		if (string.IsNullOrWhiteSpace(assetName)) {
			managedAsset = null;
			return false;
		}

		return TryGetManagedAsset<T>(Helper.GameContent.ParseAssetName(assetName), out managedAsset);
	}

	public bool TryGetManagedAsset<T>(IAssetName assetName, [NotNullWhen(true)] out IManagedAsset<T>? managedAsset) where T : notnull {
		if (assetName is null) {
			managedAsset = null;
			return false;
		}

		if (ManagedAssets.TryGetValue(assetName, out var reference) && reference.TryGetTarget(out var target)) {
			if (target is IManagedAsset<T> mat) {
				managedAsset = mat;
				return true;

			} else {
				managedAsset = null;
				return false;
			}
		}

		DeclareAssetType<T>(assetName);

		lock ((ManagedAssets as ICollection).SyncRoot) {
			managedAsset = new ManagedAsset<T>(this, assetName);
			ManagedAssets[assetName] = new WeakReference<IManagedAsset>(managedAsset);
		}

		return true;
	}

	#endregion

	#region Asset Handling

	#region DoesAssetExist Delegates

	internal void LoadGameContentManager() {
		GameContentManager_Loaded = false;
		if (!GameContentManager_Loaded) {
			GameContentManager_Loaded = true;
			FieldInfo? field = AccessTools.Field(Helper.GameContent.GetType(), "GameContentManager");
			if (field is not null) {
				try {
					GameContentManager_Instance = field.GetValue(Helper.GameContent);
				} catch(Exception ex) {
					Log($"Unable to read GameContentManager from GameContent helper. Asset loading will break.", LogLevel.Error, ex);
					return;
				}

				if (GameContentManager_Instance is not null)
					GameContentManager_DoesAssetExist = GameContentManager_Instance.GetType().GetMethod("DoesAssetExist", BindingFlags.Instance | BindingFlags.Public);
			}
		}
	}

	internal GCMDoesAssetExist<T>? GetDoesAssetExistDelegate<T>() {
		LoadGameContentManager();
		if (GameContentManager_DoesAssetExist is null)
			return null;

		Type tType = typeof(T);
		GameContentManager_Delegates ??= new();

		lock (GameContentManager_Delegates.SyncRoot) {
			if (!GameContentManager_Delegates.ContainsKey(tType)) {
				GCMDoesAssetExist<T>? @delegate;
				try {
					var generic = GameContentManager_DoesAssetExist.MakeGenericMethod(typeof(T));
					@delegate = generic.CreateDelegate<GCMDoesAssetExist<T>>(GameContentManager_Instance);
				} catch(Exception ex) {
					Log($"Unable to create DoesAssetExist delegate: {ex}", LogLevel.Error);
					@delegate = null;
				}

				GameContentManager_Delegates[tType] = @delegate;
				return @delegate;
			}

			return GameContentManager_Delegates[tType] as GCMDoesAssetExist<T>;
		}
	}

	#endregion

	public bool DoesAssetExist<T>([NotNullWhen(true)] IAssetName? name) {
		if (name is null)
			return false;

		LoadGameContentManager();
		if (GameContentManager_DoesAssetExist is null)
			return false;

		var @delegate = GetDoesAssetExistDelegate<T>();
		if (@delegate is null)
			return false;

		DeclareAssetType<T>(name);
		return @delegate(name);
	}

	public void DeclareAssetType<T>(IAssetName? assetName) {
		if (assetName is null)
			return;

		Type tType = typeof(T);

		// If we're requesting a managed asset, get the generic type from it.
		if (tType.IsConstructedGenericType && tType.GetGenericTypeDefinition() == typeof(IManagedAsset<>))
			tType = tType.GetGenericArguments()[0];

		lock ((AssetTypes as ICollection).SyncRoot) {
			AssetTypes[assetName] = tType;
		}
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		bool reload_spritefonts = false;
		bool reload_spritetext = false;

		foreach(var entry in e.Names) {
			if (ManagersByThemeAsset.TryGetValue(entry.Name, out var manager))
				manager.InvalidateThemeData();

			if (ManagedAssets.TryGetValue(entry, out var reference) && reference.TryGetTarget(out var managed))
				managed.MarkStale();

			switch(entry.Name.ToLower()) {
				case "fonts/spritefont1":
				case "fonts/smallfont":
				case "fonts/tinyfont":
				case "fonts/tinyfontborder":
					reload_spritefonts = true;
					break;
				case "loosesprites/font_bold":
				case "loosesprites/font_colored":
					reload_spritetext = true;
					break;
			}
		}

		if (reload_spritefonts) {
			SpriteFontManager.UpdateDefaultFonts();
			SpriteFontManager.AssignFonts(GameTheme);
		}

		if (reload_spritetext) {
			SpriteTextManager.UpdateDefaultTextures();
			SpriteTextManager.AssignFonts(GameTheme);
		}
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (ManagersByThemeAsset.TryGetValue(e.Name.Name, out var manager)) {
			manager.HandleAssetRequested(e);
			return;
		}

		foreach(var entry in ManagersByAssetPrefix) {
			if (e.Name.StartsWith(entry.Key)) {
				entry.Value.HandleAssetRequested(e);
				return;
			}
		}
	}

	#endregion

}
