/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace BetterBeehouses.integration
{
	internal class CJBPatch
	{
		private static bool isPatched = false;
		private static HashSet<int> knownFruits;
		private static MethodInfo TryCreate;
		private static MethodInfo Postfix;
		private static FieldInfo IDOffset;
		private static MethodInfo GetVariants;
		private static readonly ISemanticVersion Version = new SemanticVersion("2.1.7");

		internal static void ReloadFruits()
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("CJBok.ItemSpawner") ||
				ModEntry.helper.ModRegistry.Get("CJBok.ItemSpawner").Manifest.Version.IsOlderThan(Version))
				return;

			knownFruits = GetKnownFruits();
		}

		internal static void Setup()
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("CJBok.ItemSpawner") || 
				ModEntry.helper.ModRegistry.Get("CJBok.ItemSpawner").Manifest.Version.IsOlderThan(Version))
				return;

			knownFruits ??= GetKnownFruits();

			var type = AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.ItemRepository");
			TryCreate ??= AccessTools.Method(type, "TryCreate");
			IDOffset ??= AccessTools.Field(type, "CustomIDOffset");
			Postfix ??= typeof(CJBPatch).MethodNamed(nameof(GetFlavorWrapper)).MakeGenericMethod(
				AccessTools.TypeByName("CJBItemSpawner.Framework.ItemData.SearchableItem")
			);
			GetVariants ??= AccessTools.Method(type, "GetFlavoredObjectVariants");

			if (ModEntry.config.PatchCJB && !isPatched)
			{
				ModEntry.harmony.Patch(GetVariants, postfix: new(Postfix));
				isPatched = true;
			} else if (!ModEntry.config.PatchCJB && isPatched)
			{
				ModEntry.harmony.Unpatch(GetVariants, HarmonyPatchType.Postfix);
				isPatched = false;
			}
		}
		public static IEnumerable<T> GetFlavorWrapper<T>(IEnumerable<T> __result, object __instance, SObject item) where T : class
		{
			foreach (var n in __result)
				yield return n;

			if (item.HasContextTag("honey_source") || 
				(ModEntry.config.UseAnyFruitTrees && item.Category != -80 && knownFruits.Contains(item.ParentSheetIndex)))
			{
				// this is godawful
				Func<object, Item> Generate = (_) =>
				{
					SObject honey = new(Vector2.Zero, 340, $"{item.Name} Honey", false, true, false, false)
					{
						Name = $"{item.Name} Honey",
						preservedParentSheetIndex = { item.ParentSheetIndex }
					};
					honey.Price += item.Price * 2;
					return honey;
				};
				yield return TryCreate?.Invoke(__instance, new object[] { 6, IDOffset.GetValue(__instance), Generate }) as T;
			}
		}
		private static HashSet<int> GetKnownFruits()
		{
			var treedata = ModEntry.helper.GameContent.Load<Dictionary<int, string>>("Data/fruitTrees");
			if (treedata is null)
				return new();
			HashSet<int> ret = new();
			foreach (var tree in treedata.Values)
				if (int.TryParse(tree.GetChunk('/', 2), out int fruit))
					ret.Add(fruit);
			return ret;
		}
	}
}
