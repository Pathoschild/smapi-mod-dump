/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shockah.CommonModCode
{
	public static class HarmonyPatchExtensions
	{
		public static void Patch(
			this Harmony self,
			MethodBase? original,
			IMonitor monitor,
			LogLevel problemLogLevel = LogLevel.Error,
			LogLevel successLogLevel = LogLevel.Trace,
			HarmonyMethod? prefix = null,
			HarmonyMethod? postfix = null,
			HarmonyMethod? transpiler = null,
			HarmonyMethod? finalizer = null
		)
		{
			if (original is null)
			{
				monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: Unknown method to patch.", problemLogLevel);
				return;
			}
			self.Patch(original, prefix, postfix, transpiler, finalizer);
			monitor.Log($"Patched method {original.FullDescription()}.", successLogLevel);
		}

		public static bool TryPatch(
			this Harmony self,
			Func<MethodBase?> original,
			IMonitor monitor,
			LogLevel problemLogLevel = LogLevel.Error,
			LogLevel successLogLevel = LogLevel.Trace,
			HarmonyMethod? prefix = null,
			HarmonyMethod? postfix = null,
			HarmonyMethod? transpiler = null,
			HarmonyMethod? finalizer = null
		)
		{
			try
			{
				var originalMethod = original();
				if (originalMethod is null)
				{
					monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: Unknown method to patch.", problemLogLevel);
					return false;
				}
				self.Patch(originalMethod, prefix, postfix, transpiler, finalizer);
				monitor.Log($"Patched method {originalMethod.FullDescription()}.", successLogLevel);
				return true;
			}
			catch (Exception ex)
			{
				monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: {ex}", problemLogLevel);
				return false;
			}
		}

		public static void PatchVirtual(
			this Harmony self,
			MethodBase? original,
			IMonitor monitor,
			LogLevel problemLogLevel = LogLevel.Error,
			LogLevel successLogLevel = LogLevel.Trace,
			HarmonyMethod? prefix = null,
			HarmonyMethod? postfix = null,
			HarmonyMethod? finalizer = null
		)
		{
			if (original is null)
			{
				monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: Unknown method to patch.", problemLogLevel);
				return;
			}

			Type? declaringType = original.DeclaringType;
			if (declaringType == null)
				throw new ArgumentException($"{nameof(original)}.{nameof(original.DeclaringType)} is null.");
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				IEnumerable<Type> subtypes = Enumerable.Empty<Type>();
				try
				{
					subtypes = assembly.GetTypes().Where(t => t.IsAssignableTo(declaringType));
				}
				catch (Exception ex)
				{
					monitor.Log($"There was a problem while getting types defined in assembly {assembly.GetName().Name}, ignoring it. Reason:\n{ex}", LogLevel.Trace);
				}

				foreach (Type subtype in subtypes)
				{
					var originalParameters = original.GetParameters();
					var subtypeOriginal = AccessTools.Method(
						subtype,
						original.Name,
						originalParameters.Select(p => p.ParameterType).ToArray()
					);
					if (subtypeOriginal is null)
						continue;
					if (!subtypeOriginal.IsDeclaredMember())
						continue;
					if (!subtypeOriginal.HasMethodBody())
						continue;

					static bool ContainsNonSpecialArguments(HarmonyMethod patch)
						=> patch.method.GetParameters().Any(p => !(p.Name ?? "").StartsWith("__"));

					if (
						(prefix is not null && ContainsNonSpecialArguments(prefix)) ||
						(postfix is not null && ContainsNonSpecialArguments(postfix)) ||
						(finalizer is not null && ContainsNonSpecialArguments(finalizer))
					)
					{
						var subtypeOriginalParameters = subtypeOriginal.GetParameters();
						for (int i = 0; i < original.GetParameters().Length; i++)
							if (originalParameters[i].Name != subtypeOriginalParameters[i].Name)
								throw new InvalidOperationException($"Method {declaringType.Name}.{original.Name} cannot be automatically patched for subtype {subtype.Name}, because argument #{i} has a mismatched name: `{originalParameters[i].Name}` vs `{subtypeOriginalParameters[i].Name}`.");
					}

					self.Patch(subtypeOriginal, prefix, subtypeOriginal.HasMethodBody() ? postfix : null, null, finalizer);
					monitor.Log($"Patched method {subtypeOriginal.FullDescription()}.", successLogLevel);
				}
			}
		}

		public static (int patched, int total) TryPatchVirtual(
			this Harmony self,
			Func<MethodBase?> original,
			IMonitor monitor,
			LogLevel problemLogLevel = LogLevel.Error,
			LogLevel successLogLevel = LogLevel.Trace,
			HarmonyMethod? prefix = null,
			HarmonyMethod? postfix = null,
			HarmonyMethod? finalizer = null
		)
		{
			try
			{
				var originalMethod = original();
				if (originalMethod is null)
				{
					monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: Unknown method to patch.", problemLogLevel);
					return (0, 1);
				}

				int patched = 0;
				int total = 0;
				Type? declaringType = originalMethod.DeclaringType;
				if (declaringType == null)
					throw new ArgumentException($"{nameof(original)}.{nameof(originalMethod.DeclaringType)} is null.");
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					total++;
					IEnumerable<Type> subtypes = Enumerable.Empty<Type>();
					try
					{
						subtypes = assembly.GetTypes().Where(t => t.IsAssignableTo(declaringType));
					}
					catch (Exception ex)
					{
						monitor.Log($"There was a problem while getting types defined in assembly {assembly.GetName().Name}, ignoring it. Reason:\n{ex}", LogLevel.Trace);
					}

					foreach (Type subtype in subtypes)
					{
						try
						{
							var originalParameters = originalMethod.GetParameters();
							var subtypeOriginal = AccessTools.Method(
								subtype,
								originalMethod.Name,
								originalParameters.Select(p => p.ParameterType).ToArray()
							);
							if (subtypeOriginal is null)
								continue;
							if (!subtypeOriginal.IsDeclaredMember())
								continue;
							if (!subtypeOriginal.HasMethodBody())
								continue;

							static bool ContainsNonSpecialArguments(HarmonyMethod patch)
								=> patch.method.GetParameters().Any(p => !(p.Name ?? "").StartsWith("__"));

							if (
								(prefix is not null && ContainsNonSpecialArguments(prefix)) ||
								(postfix is not null && ContainsNonSpecialArguments(postfix)) ||
								(finalizer is not null && ContainsNonSpecialArguments(finalizer))
							)
							{
								var subtypeOriginalParameters = subtypeOriginal.GetParameters();
								for (int i = 0; i < originalMethod.GetParameters().Length; i++)
									if (originalParameters[i].Name != subtypeOriginalParameters[i].Name)
										throw new InvalidOperationException($"Method {declaringType.Name}.{originalMethod.Name} cannot be automatically patched for subtype {subtype.Name}, because argument #{i} has a mismatched name: `{originalParameters[i].Name}` vs `{subtypeOriginalParameters[i].Name}`.");
							}

							self.Patch(subtypeOriginal, prefix, postfix, null, finalizer);
							monitor.Log($"Patched method {subtypeOriginal.FullDescription()}.", successLogLevel);
							patched++;
						}
						catch (Exception ex)
						{
							monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: {ex}", problemLogLevel);
						}
					}
				}
				return (patched, total);
			}
			catch (Exception ex)
			{
				monitor.Log($"Could not patch method - the mod may not work correctly.\nReason: {ex}", problemLogLevel);
				return (0, 1);
			}
		}
	}
}
