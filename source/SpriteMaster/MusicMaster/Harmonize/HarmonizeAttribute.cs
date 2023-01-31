/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using LinqFasterer;
using MusicMaster.Extensions;
using MusicMaster.Extensions.Reflection;
using System;
using System.Reflection;
using static MusicMaster.Harmonize.Harmonize;

namespace MusicMaster.Harmonize;

[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal class HarmonizeAttribute : Attribute {
	internal Type? Type => _lazyType.Value;
	private readonly Lazy<Type?> _lazyType;
	internal readonly string? Name;
	internal readonly int PatchPriority;
	internal readonly Fixation PatchFixation;
	internal readonly Generic GenericType;
	internal readonly bool Instance;
	internal readonly Platform ForPlatform;
	internal readonly bool Critical;
	internal readonly string? ForMod;
	internal readonly Type[]? ArgumentTypes;
	internal readonly Type[]? GenericTypes;
	internal readonly Type[]? GenericConstraints;

	private static Lazy<Type?> AsLazy(Func<Type?> factory) => new(factory);
	private static Lazy<Type?> AsLazy(Type? value) => new(value);

	internal static bool CheckPlatform(Platform platform) => platform switch {
		Platform.All => true,
		Platform.Windows => Runtime.IsWindows,
		Platform.Linux => Runtime.IsLinux,
		Platform.Macintosh => Runtime.IsMacintosh,
		Platform.Unix => Runtime.IsUnix,
		Platform.XNA => Runtime.IsXNA,
		Platform.MonoGame => Runtime.IsMonoGame,
		_ => throw new ArgumentOutOfRangeException(nameof(ForPlatform)),
	};

	internal bool CheckPlatform() => CheckPlatform(ForPlatform);

	private static Assembly? GetAssembly(string name, bool critical, string? forMod) {
		// If it's a mod patch, and the mod doesn't exist, don't bother searching for the assembly
		if (forMod is not null && MusicMaster.Self.Helper.ModRegistry.Get(forMod) is null) {
			return null;
		}
		
		if (Runtime.IsMonoGame && name.StartsWith("Microsoft.Xna.Framework")) {
			name = "MonoGame.Framework";
		}

		try {
			return AssemblyExt.GetAssembly(name);
		}
		catch {
			Debug.ConditionalError(critical, $"Assembly Not Found For Harmonize: {name}");
			if (critical) {
				throw;
			}
		}
		return null;
	}

	private static Type? ResolveType(Type? parent, string[] type, int offset = 0) {
		while (true) {
			if (parent is null) {
				return null;
			}

			var foundType = parent.GetNestedType(type[offset], BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (foundType is null) {
				return parent;
			}

			offset += 1;
			if (offset >= type.Length) {
				return foundType;
			}
			parent = foundType;
		}
	}

	private static Type? ResolveType(Assembly? assembly, string[] type, int offset = 0) =>
		assembly is null ? null : ResolveType(assembly.GetType(type[0], true), type, offset + 1);

	private static Type? ResolveType(string type) {
		return ReflectionExt.GetTypeExt(type);
	}

	private HarmonizeAttribute(
		Lazy<Type?> type,
		string? method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) {
		_lazyType = type;
		Name = method;
		PatchPriority = (int)priority;
		PatchFixation = fixation;
		GenericType = generic;
		Instance = instance;
		ForPlatform = platform;
		Critical = critical;
		ForMod = forMod;
		ArgumentTypes = argumentTypes;
		GenericTypes = genericTypes;
		GenericConstraints = genericConstraints;
	}

	internal HarmonizeAttribute(
		Type? type,
		string? method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) : this(
		type: AsLazy(type),
		method: method,
		fixation: fixation,
		priority: priority,
		generic: generic,
		instance: instance,
		critical: critical,
		platform: platform,
		forMod: forMod,
		argumentTypes: argumentTypes,
		genericTypes: genericTypes,
		genericConstraints: genericConstraints
		) {
	}

	internal HarmonizeAttribute(
		string assembly,
		string type,
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: AsLazy(() => CheckPlatform(platform) ? GetAssembly(assembly, critical: critical, forMod: forMod)?.GetType(type, true) : null),
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		Type parent,
		string type,
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: AsLazy(() => CheckPlatform(platform) ? parent.Assembly.GetType(type, true) : null),
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		string type,
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: AsLazy(() => CheckPlatform(platform) ? ResolveType(type) : null),
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		Type parent,
		string[] type,
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: AsLazy(() => CheckPlatform(platform) ? ResolveType(parent.Assembly, type) : null),
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		string assembly,
		string[] type,
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: AsLazy(() => CheckPlatform(platform) ? ResolveType(GetAssembly(assembly, critical: critical, forMod: forMod), type) : null),
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		string method,
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: (Type?)null,
			method: method,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }

	internal HarmonizeAttribute(
		Fixation fixation = Fixation.Prefix,
		PriorityLevel priority = PriorityLevel.Average,
		Generic generic = Generic.None,
		bool instance = true,
		bool critical = true,
		Platform platform = Platform.All,
		string? forMod = null,
		Type[]? argumentTypes = null,
		Type[]? genericTypes = null,
		Type[]? genericConstraints = null
	) :
		this(
			type: (Type?)null,
			method: null,
			fixation: fixation,
			priority: priority,
			generic: generic,
			instance: instance,
			critical: critical,
			platform: platform,
			forMod: forMod,
			argumentTypes: argumentTypes,
			genericTypes: genericTypes,
			genericConstraints: genericConstraints
		) { }
}
