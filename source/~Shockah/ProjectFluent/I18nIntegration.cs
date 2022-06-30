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
using Shockah.CommonModCode.IL;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.ProjectFluent
{
	internal static class I18nIntegration
	{
		private delegate IDictionary<string, IDictionary<string, string>> ReadTranslationFilesDelegateType(string folderPath, out IList<string> errors);

		internal static IMonitor Monitor { get; set; } = null!;
		private static II18nDirectoryProvider I18nDirectoryProvider { get; set; } = null!;

		private static object SCoreInstance { get; set; } = null!;
		private static Action ReloadTranslationsDelegate { get; set; } = null!;
		private static ReadTranslationFilesDelegateType ReadTranslationFilesDelegate { get; set; } = null!;

		private static bool IsTrackingAccessedTranslationKeys { get; set; } = true;
		private static IDictionary<string, ISet<string?>> AccessedTranslationKeys { get; set; } = new Dictionary<string, ISet<string?>>();

		internal static void EarlySetup(Harmony harmony)
		{
			try
			{
				Type translationHelperType = AccessTools.TypeByName("StardewModdingAPI.Framework.ModHelpers.TranslationHelper, StardewModdingAPI");

				MethodInfo getWithKeyMethod = AccessTools.Method(translationHelperType, "Get", new Type[] { typeof(string) });
				MethodInfo getWithKeyAndTokensMethod = AccessTools.Method(translationHelperType, "Get", new Type[] { typeof(string), typeof(object) });
				MethodInfo getInAllLocalesMethod = AccessTools.Method(translationHelperType, "GetInAllLocales");
				MethodInfo getTranslationsMethod = AccessTools.Method(translationHelperType, "GetTranslations");

				harmony.Patch(
					original: getWithKeyMethod,
					postfix: new HarmonyMethod(AccessTools.Method(typeof(I18nIntegration), nameof(TranslationHelper_MethodWithKey_Postfix)))
				);
				harmony.Patch(
					original: getWithKeyAndTokensMethod,
					postfix: new HarmonyMethod(AccessTools.Method(typeof(I18nIntegration), nameof(TranslationHelper_MethodWithKey_Postfix)))
				);
				harmony.Patch(
					original: getInAllLocalesMethod,
					postfix: new HarmonyMethod(AccessTools.Method(typeof(I18nIntegration), nameof(TranslationHelper_MethodWithKey_Postfix)))
				);
				harmony.Patch(
					original: getTranslationsMethod,
					postfix: new HarmonyMethod(AccessTools.Method(typeof(I18nIntegration), nameof(TranslationHelper_MethodWithoutKey_Postfix)))
				);
			}
			catch (Exception ex)
			{
				if (ProjectFluent.Instance.Config.DeveloperMode)
					Monitor.Log($"Could not hook into SMAPI - untranslatable mod detection won't work.\nReason: {ex}", LogLevel.Warn);
				return;
			}
		}

		internal static void Setup(Harmony harmony, II18nDirectoryProvider i18nDirectoryProvider)
		{
			I18nDirectoryProvider = i18nDirectoryProvider;

			try
			{
				Type rawEnumerableType = typeof(IEnumerable<>);
				Type scoreType = AccessTools.TypeByName("StardewModdingAPI.Framework.SCore, StardewModdingAPI");
				Type modMetadataType = AccessTools.TypeByName("StardewModdingAPI.Framework.IModMetadata, StardewModdingAPI");
				Type modMetadataEnumerableType = rawEnumerableType.MakeGenericType(modMetadataType);

				MethodInfo scoreInstanceGetter = AccessTools.PropertyGetter(scoreType, "Instance");
				MethodInfo reloadTranslationsMethod = AccessTools.Method(scoreType, "ReloadTranslations", Array.Empty<Type>());
				MethodInfo reloadTranslationsEnumerableMethod = AccessTools.Method(scoreType, "ReloadTranslations", new Type[] { modMetadataEnumerableType });
				MethodInfo readTranslationFilesMethod = AccessTools.Method(scoreType, "ReadTranslationFiles");

				SCoreInstance = scoreInstanceGetter.Invoke(null, null)!;
				ReloadTranslationsDelegate = () => reloadTranslationsMethod.Invoke(SCoreInstance, null);
				ReadTranslationFilesDelegate = (string folderPath, out IList<string> errors) =>
				{
					var parameters = new object?[] { folderPath, null };
					var result = (IDictionary<string, IDictionary<string, string>>)readTranslationFilesMethod.Invoke(SCoreInstance, parameters)!;
					errors = (IList<string>)parameters[1]!;
					return result;
				};

				harmony.Patch(
					original: reloadTranslationsEnumerableMethod,
					transpiler: new HarmonyMethod(AccessTools.Method(typeof(I18nIntegration), nameof(SCore_ReloadTranslations_Transpiler)))
				);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Could not hook into SMAPI - i18n integration won't work.\nReason: {ex}", LogLevel.Error);
				return;
			}

			WarnAboutAccessedTranslations();
		}

		internal static void ReloadTranslations()
			=> ReloadTranslationsDelegate();

		private static void WarnAboutAccessedTranslations()
		{
			if (!ProjectFluent.Instance.Config.DeveloperMode)
			{
				AccessedTranslationKeys.Clear();
				IsTrackingAccessedTranslationKeys = false;
				return;
			}

			foreach (var (modID, keys) in AccessedTranslationKeys)
			{
				bool hasKnownKeys = keys.Any(k => k is not null);
				bool hasUnknownKeys = keys.Any(k => k is null);

				if (hasKnownKeys)
				{
					string knownKeys = string.Join("\n", keys.Where(k => k is not null).Select(k => $"\t* {k}"));
					if (hasUnknownKeys)
						Monitor.Log($"[Developer Mode] Mod `{modID}` accessed i18n translations before Project Fluent was ready (it may not be fully translatable):\n{knownKeys}\n\t* ...and possibly others.", LogLevel.Warn);
					else
						Monitor.Log($"[Developer Mode] Mod `{modID}` accessed i18n translations before Project Fluent was ready (it may not be fully translatable):\n{knownKeys}", LogLevel.Warn);
				}
				else if (hasUnknownKeys)
				{
					Monitor.Log($"[Developer Mode] Mod `{modID}` possibly accessed i18n translations before Project Fluent was ready (it may not be fully translatable).", LogLevel.Warn);
				}
			}
			AccessedTranslationKeys.Clear();
			IsTrackingAccessedTranslationKeys = false;
		}

		private static void TranslationHelper_MethodWithKey_Postfix(ITranslationHelper __instance, string key)
		{
			if (!IsTrackingAccessedTranslationKeys)
				return;
			if (!AccessedTranslationKeys.TryGetValue(__instance.ModID, out var keys))
			{
				keys = new HashSet<string?>();
				AccessedTranslationKeys[__instance.ModID] = keys;
			}
			keys.Add(key);
		}

		private static void TranslationHelper_MethodWithoutKey_Postfix(ITranslationHelper __instance)
		{
			if (!IsTrackingAccessedTranslationKeys)
				return;
			if (!AccessedTranslationKeys.TryGetValue(__instance.ModID, out var keys))
			{
				keys = new HashSet<string?>();
				AccessedTranslationKeys[__instance.ModID] = keys;
			}
			keys.Add(null);
		}

		private static IEnumerable<CodeInstruction> SCore_ReloadTranslations_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find (last occurence):
			// IL_0090: callvirt instance !0 class [System.Runtime]System.Collections.Generic.IEnumerator`1<class StardewModdingAPI.Framework.IModMetadata>::get_Current()
			// IL_0095: stloc.s 5
			// IL_0097: ldarg.0
			// IL_0098: ldloc.s 5
			// IL_009a: callvirt instance string StardewModdingAPI.Framework.IModMetadata::get_DirectoryPath()
			// IL_009f: ldstr "i18n"
			// IL_00a4: call string [System.Runtime]System.IO.Path::Combine(string, string)
			// IL_00a9: ldloca.s 7
			// IL_00ab: call instance class [System.Runtime]System.Collections.Generic.IDictionary`2<string, class [System.Runtime]System.Collections.Generic.IDictionary`2<string, string>> StardewModdingAPI.Framework.SCore::ReadTranslationFiles(string, class [System.Runtime]System.Collections.Generic.IList`1<string>&)
			// IL_00b0: stloc.s 6
			var worker = TranspileWorker.FindInstructionsBackwards(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.opcode == OpCodes.Callvirt && ((MethodBase)i.operand).Name == "get_Current",
				i => i.IsStloc(),
				i => i.IsLdarg(),
				i => i.IsLdloc(),
				i => i.opcode == OpCodes.Callvirt && ((MethodBase)i.operand).Name == "get_DirectoryPath",
				i => i.opcode == OpCodes.Ldstr && (string)i.operand == "i18n",
				i => i.opcode == OpCodes.Call && ((MethodBase)i.operand).Name == "Combine",
				i => i.IsLdloc(),
				i => i.opcode == OpCodes.Call && ((MethodBase)i.operand).Name == "ReadTranslationFiles",
				i => i.IsStloc()
			});
			if (worker is null)
			{
				Monitor.Log($"Could not patch SMAPI methods - Project Fluent probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Postfix(new[]
			{
				worker[1].ToLoadLocal()!, // modInfo
				worker[9].ToLoadLocal()!, // translations
				worker[7].ToLoadLocalAddress()!, // errors
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(I18nIntegration), nameof(SCore_ReloadTranslations_Transpiler_ModifyList)))
			});

			return instructions;
		}

		private static void SCore_ReloadTranslations_Transpiler_ModifyList(
			IModInfo modInfo,
			IDictionary<string, IDictionary<string, string>> translations,
			ref IList<string> errors
		)
		{
			foreach (var directory in I18nDirectoryProvider.GetI18nDirectories(modInfo.Manifest).Reverse())
			{
				var newTranslations = ReadTranslationFilesDelegate(directory, out var newErrors);
				foreach (var error in newErrors)
					errors.Add(error);
				foreach (var (key, value) in newTranslations)
					translations[key] = value;
			}
		}
	}
}