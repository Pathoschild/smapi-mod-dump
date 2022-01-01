/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using System;
using System.Reflection;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
sealed class HarmonizeAttribute : Attribute {
	internal enum Fixation {
		Prefix,
		Postfix,
		Transpile
	}

	internal enum Generic {
		None,
		Struct,
		Class
	}

	internal enum Platform {
		All = 0,
		Windows,
		Linux,
		Macintosh,
		Unix,
		XNA,
		MonoGame
	}

	internal readonly Type Type;
	internal readonly string Name;
	internal readonly int PatchPriority;
	internal readonly Fixation PatchFixation;
	internal readonly Generic GenericType;
	internal readonly bool Instance;
	internal readonly Platform ForPlatform;

	internal static bool CheckPlatform(Platform platform) {
		return platform switch {
			Platform.All => true,
			Platform.Windows => Runtime.IsWindows,
			Platform.Linux => Runtime.IsLinux,
			Platform.Macintosh => Runtime.IsMacintosh,
			Platform.Unix => Runtime.IsUnix,
			Platform.XNA => Runtime.IsXNA,
			Platform.MonoGame => Runtime.IsMonoGame,
			_ => throw new ArgumentOutOfRangeException(nameof(ForPlatform)),
		};
	}

	internal bool CheckPlatform() {
		return CheckPlatform(ForPlatform);
	}

	private static Assembly GetAssembly(string name) {
		if (Runtime.IsMonoGame && name.StartsWith("Microsoft.Xna.Framework")) {
			name = "MonoGame.Framework";
		}

		try {
			return AppDomain.CurrentDomain.GetAssemblies().SingleF(assembly => assembly.GetName().Name == name);
		}
		catch {
			Debug.ErrorLn($"Assembly Not Found For Harmonize: {name}");
			throw;
		}
	}

	private static Type ResolveType(Assembly assembly, Type parent, string[] type, int offset = 0) {
		var foundType = parent.GetNestedType(type[offset], BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		offset += 1;
		if (offset >= type.Length)
			return foundType;
		else
			return ResolveType(assembly, foundType, type, offset);
	}

	private static Type ResolveType(Assembly assembly, string[] type, int offset = 0) {
		return ResolveType(assembly, assembly.GetType(type[0], true), type, offset + 1);
	}

	internal HarmonizeAttribute(Type type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) {
		Type = type;
		Name = method;
		PatchPriority = (int)priority;
		PatchFixation = fixation;
		GenericType = generic;
		Instance = instance;
		ForPlatform = platform;
	}

	internal HarmonizeAttribute(string assembly, string type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
		this(
			CheckPlatform(platform) ? GetAssembly(assembly).GetType(type, true) : null,
			method,
			fixation,
			priority,
			generic,
			instance,
			platform
		) { }

	internal HarmonizeAttribute(Type parent, string type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
		this(
			CheckPlatform(platform) ? parent.Assembly.GetType(type, true) : null,
			method,
			fixation,
			priority,
			generic,
			instance,
			platform
		) { }

	internal HarmonizeAttribute(Type parent, string[] type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
		this(
			CheckPlatform(platform) ? ResolveType(parent.Assembly, type) : null,
			method,
			fixation,
			priority,
			generic,
			instance,
			platform
		) { }

	internal HarmonizeAttribute(string assembly, string[] type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
		this(
			CheckPlatform(platform) ? ResolveType(GetAssembly(assembly), type) : null,
			method,
			fixation,
			priority,
			generic,
			instance,
			platform
		) { }

	internal HarmonizeAttribute(string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
		this(null, method, fixation, priority, generic, instance, platform) { }
}
