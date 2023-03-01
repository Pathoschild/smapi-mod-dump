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
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
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

		private static IEnumerable<CodeInstruction> SCore_ReloadTranslations_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsAnchorable<CodeInstruction, Guid, Guid, SequencePointerMatcher<CodeInstruction>, SequenceBlockMatcher<CodeInstruction>>()
					.Find(
						ILMatches.Call("get_Current"),
						ILMatches.AnyStloc.WithAutoAnchor(out Guid modInfoLocalInstruction),
						ILMatches.AnyLdarg,
						ILMatches.AnyLdloc,
						ILMatches.Call("get_DirectoryPath"),
						ILMatches.Ldstr("i18n"),
						ILMatches.Call("Combine"),
						ILMatches.AnyLdloca.WithAutoAnchor(out Guid errorsLocalInstruction),
						ILMatches.Call("ReadTranslationFiles"),
						ILMatches.AnyStloc.WithAutoAnchor(out Guid translationsLocalInstruction)
					)
					.AnchorBlock(out Guid findBlock)

					.MoveToPointerAnchor(modInfoLocalInstruction)
					.CreateLdlocInstruction(out var modInfoLoadInstruction)

					.MoveToPointerAnchor(errorsLocalInstruction)
					.CreateLdlocaInstruction(out var errorsLoadAddressInstruction)

					.MoveToPointerAnchor(translationsLocalInstruction)
					.CreateLdlocInstruction(out var translationsLoadInstruction)

					.MoveToBlockAnchor(findBlock)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, true,

						modInfoLoadInstruction,
						translationsLoadInstruction,
						errorsLoadAddressInstruction,
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(I18nIntegration), nameof(SCore_ReloadTranslations_Transpiler_ModifyList)))
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Monitor.Log($"Could not patch methods - {ProjectFluent.Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
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