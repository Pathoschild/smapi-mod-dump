/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Reflection;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize;

internal abstract class HarmonizeFinalizeCatcherFixedAttribute : HarmonizeAttribute {
	internal readonly Type Exception;
	internal readonly MethodInfo? MethodInfo;

	internal HarmonizeFinalizeCatcherFixedAttribute(Type type, Type exception, Platform platform, MethodInfo? methodInfo, bool critical) : base(
		type: type,
		method: $"~{type.Name.Split('`', 2)[0]}",
		fixation: Fixation.Finalizer,
		priority: PriorityLevel.Highest,
		instance: true,
		platform: platform,
		critical: critical
	) {
		Exception = exception;
		MethodInfo = methodInfo;
	}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class HarmonizeFinalizeCatcherAttribute<T, E> : HarmonizeFinalizeCatcherFixedAttribute where E : Exception {
	internal static new readonly Type Type = typeof(T);
	internal static new readonly Type Exception = typeof(E);

	private static Exception? Implementation(T __instance, Exception __exception) => (__exception is E) ? null : __exception;

	internal HarmonizeFinalizeCatcherAttribute(bool critical = true, Platform platform = Platform.All) : base(
		type: Type,
		exception: Exception,
		platform: platform,
		methodInfo: typeof(HarmonizeFinalizeCatcherAttribute<T, E>).GetMethod(nameof(Implementation), BindingFlags.Static | BindingFlags.NonPublic),
		critical: critical
	) { }
}
