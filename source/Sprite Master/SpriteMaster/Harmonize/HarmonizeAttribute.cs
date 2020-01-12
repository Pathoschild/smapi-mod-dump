using Harmony;
using System;
using System.Linq;
using System.Reflection;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	internal sealed class HarmonizeAttribute : Attribute {
		public enum Fixation {
			Prefix,
			Postfix,
			Transpile
		}

		public enum Generic {
			None,
			Struct,
			Class
		}

		public enum Platform {
			All = 0,
			Windows = 1,
			Linux = 2,
			Macintosh = 3,
			Unix = 4
		}

		public readonly Type Type;
		public readonly string Method;
		public readonly int PatchPriority;
		public readonly Fixation PatchFixation;
		public readonly Generic GenericType;
		public readonly bool Instance;
		public readonly Platform ForPlatform;

		internal static bool CheckPlatform(Platform platform) {
			return platform switch
			{
				Platform.All => true,
				Platform.Windows => Runtime.IsWindows,
				Platform.Linux => Runtime.IsLinux,
				Platform.Macintosh => Runtime.IsMacintosh,
				Platform.Unix => Runtime.IsUnix,
				_ => throw new ArgumentOutOfRangeException(nameof(ForPlatform)),
			};
		}

		internal bool CheckPlatform() {
			return CheckPlatform(ForPlatform);
		}

		private static Assembly GetAssembly(string name) {
			if (Runtime.IsUnix && name.StartsWith("Microsoft.Xna.Framework")) {
				name = "MonoGame.Framework";
			}

			try {
				return AppDomain.CurrentDomain.GetAssemblies().Single(assembly => assembly.GetName().Name == name);
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

		public HarmonizeAttribute(Type type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) {
			Type = type;
			Method = method;
			PatchPriority = (int)priority;
			PatchFixation = fixation;
			GenericType = generic;
			Instance = instance;
			ForPlatform = platform;
		}

		public HarmonizeAttribute (string assembly, string type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
			this(
				CheckPlatform(platform) ? GetAssembly(assembly).GetType(type, true) : null,
				method,
				fixation,
				priority,
				generic,
				instance,
				platform
			) { }

		public HarmonizeAttribute (Type parent, string type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
			this(
				CheckPlatform(platform) ? parent.Assembly.GetType(type, true) : null,
				method,
				fixation,
				priority,
				generic,
				instance,
				platform
			) { }

		public HarmonizeAttribute (Type parent, string[] type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
			this(
				CheckPlatform(platform) ? ResolveType(parent.Assembly, type) : null,
				method,
				fixation,
				priority,
				generic,
				instance,
				platform
			) { }

		public HarmonizeAttribute (string assembly, string[] type, string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
			this(
				CheckPlatform(platform) ? ResolveType(GetAssembly(assembly), type) : null,
				method,
				fixation,
				priority,
				generic,
				instance,
				platform
			) { }

		public HarmonizeAttribute (string method, Fixation fixation = Fixation.Prefix, PriorityLevel priority = PriorityLevel.Average, Generic generic = Generic.None, bool instance = true, Platform platform = Platform.All) :
			this(null, method, fixation, priority, generic, instance, platform) { }
	}
}
