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

namespace Shockah.ProjectFluent.ContentPatcher
{
	internal static class ContentPatcherIntegration
	{
		private static readonly string ContentPatcherModID = "Pathoschild.ContentPatcher";
		private static readonly string ContentPatcherModTokenContextQualifiedName = "ContentPatcher.Framework.ModTokenContext, ContentPatcher";
		private static readonly string RegisterFluentTokenManifestKey = "UsesFluentContentPatcherTokens";
		private static readonly string TokenName = "Fluent";

		private static bool DidTryGetTokenWithNamespace = false;
		private static Func<object, string> GetScopeDelegate = null!;
		private static Func<object, string, bool, object?> GetTokenDelegate = null!;

		internal static void Setup(Harmony harmony)
		{
			var api = ProjectFluent.Instance.Helper.ModRegistry.GetApi<IContentPatcherApi>(ContentPatcherModID);
			if (api is null)
				return;

			var version = ProjectFluent.Instance.Helper.ModRegistry.Get(ContentPatcherModID)!.Manifest.Version;
			if (version.MajorVersion > 1)
				ProjectFluent.Instance.Monitor.Log("Detected newer Content Patcher than 1.x, integration might not behave correctly.", LogLevel.Warn);

			Patch(harmony);
			RegisterTokenInContentPacks(api);
		}

		private static void Patch(Harmony harmony)
		{
			try
			{
				var modTokenContextType = Type.GetType(ContentPatcherModTokenContextQualifiedName);

				var getTokenMethod = AccessTools.Method(modTokenContextType, "GetToken");
				GetTokenDelegate = (context, name, enforceContext) => getTokenMethod.Invoke(context, new object[] { name, enforceContext });

				var getScopeField = AccessTools.Field(modTokenContextType, "Scope");
				GetScopeDelegate = (context) => (string)getScopeField.GetValue(context)!;

				harmony.Patch(
					original: getTokenMethod,
					postfix: new HarmonyMethod(typeof(ContentPatcherIntegration), nameof(ModTokenContext_GetToken_Postfix))
				);
			}
			catch (Exception e)
			{
				ProjectFluent.Instance.Monitor.Log($"Could not patch Content Patcher methods - integration with it probably won't work.\nReason: {e}", LogLevel.Error);
			}
		}

		private static void RegisterTokenInContentPacks(IContentPatcherApi api)
		{
			foreach (var modInfo in ProjectFluent.Instance.Helper.ModRegistry.GetAll())
			{
				if (modInfo.Manifest.ContentPackFor?.UniqueID != ContentPatcherModID)
					continue;
				if (modInfo.Manifest.ExtraFields.TryGetValue(RegisterFluentTokenManifestKey, out object? value) && value is true)
					api.RegisterToken(modInfo.Manifest, TokenName, new ContentPatcherToken(modInfo.Manifest));
			}
		}

		private static void ModTokenContext_GetToken_Postfix(ref object __instance, ref object? __result, string name, bool enforceContext)
		{
			switch (ProjectFluent.Instance.Config.ContentPatcherPatchingMode)
			{
				case ContentPatcherPatchingMode.Disabled:
					return;
				case ContentPatcherPatchingMode.PatchFluentToken:
					if (name != TokenName)
						return;
					break;
				case ContentPatcherPatchingMode.PatchAllTokens:
					break;
			}

			if (__result != null)
				return;
			if (DidTryGetTokenWithNamespace)
				return;
			if (name.Contains('/'))
				return;

			DidTryGetTokenWithNamespace = true;
			__result = GetTokenDelegate(__instance, $"{GetScopeDelegate(__instance)}/{name}", enforceContext);
			DidTryGetTokenWithNamespace = false;
		}
	}
}