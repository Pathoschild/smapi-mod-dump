/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HappyHomeDesigner.Patches;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HappyHomeDesigner.Integration
{
	internal class AlternativeTextures
	{
		const string KEY_OWNER = "AlternativeTextureOwner";
		const string KEY_NAME = "AlternativeTextureName";
		const string KEY_VARIATION = "AlternativeTextureVariation";
		const string KEY_SEASON = "AlternativeTextureSeason";
		const string KEY_DISPLAY_NAME = "AlternativeTextureDisplayName";

		public static bool Installed;

		public static Func<string, string, string, bool> HasVariant;
		public static Action<Furniture, Season, List<Furniture>> VariantsOfFurniture;
		public static Action<StardewValley.Object, Season, List<StardewValley.Object>> VariantsOfCraftable;

		internal static void Init(IModHelper helper)
		{
			Installed = false;
			if (!helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures") || !AltTex.IsApplied)
				return;

			ModEntry.monitor.Log("Alternative Textures detected! Integrating...", LogLevel.Debug);

			const BindingFlags STATIC = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			string error = null;
			Type entry;
			FieldInfo manager;


			// Find assembly
			if (!ModUtilities.TryFindAssembly("AlternativeTextures", out var asm))
				error = "Failed to find AT assembly, could not integrate.";

			// Find mod entry
			else if ((entry = asm.GetType("AlternativeTextures.AlternativeTextures")) is null)
				error = "Failed to find entry point for Alternative Textures.";

			// Get handle for texture manager
			else if ((manager = entry.GetField("textureManager", STATIC)) is null)
				error = "Failed to find texture manager.";

			// bind variant checker
			else if (!BindHasVariant(manager))
				error = "Failed to bind HasVariant.";

			// bind furniture variant factory
			else if (!TryBindVariantsOf(manager, "Furniture_", out VariantsOfFurniture))
				error = "Failed to bind Furniture variants.";

			// bind craftable variant factory
			else if (!TryBindVariantsOf(manager, "Craftable_", out VariantsOfCraftable))
				error = "Failed to bind Craftable variants.";


			if (error is null)
			{
				ModEntry.monitor.Log("Integration successful.", LogLevel.Debug);
				Installed = true;
			} 
			else
			{
				ModEntry.monitor.Log("Error integrating Alternative Textures: " + error, LogLevel.Error);
				Installed = false;
			}
		}

		private static bool BindHasVariant(FieldInfo manager)
		{
			/*
			* HasVariant(name, season)
			*	=>	AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(name) || 
			*		AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(name + "_" + season)
			*/

			var getter = manager.FieldType.GetMethod("DoesObjectHaveAlternativeTexture", new[] { typeof(string), typeof(bool) });
			var name = Expression.Parameter(typeof(string));
			var season = Expression.Parameter(typeof(string));
			var id = Expression.Parameter(typeof(string));

			var body = Expression.Or(
				HasVariantCheck(manager, getter, name, season, Expression.Constant(false)),
				HasVariantCheck(manager, getter, id, season, Expression.Constant(true))
			);
			try
			{
				HasVariant = Expression.Lambda<Func<string, string, string, bool>>(body, id, name, season).Compile();
			} catch (Exception ex)
			{
				ModEntry.monitor.Log(ex.ToString(), LogLevel.Trace);
				return false;
			}
			return true;
		}

		private static Expression HasVariantCheck(FieldInfo manager, MethodInfo getter, Expression name, Expression season, Expression isId)
		{
			return Expression.Or(
				Expression.Call(Expression.Field(null, manager), getter, name, isId),
				Expression.Call(Expression.Field(null, manager), getter,
					Expression.Call(typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string) }),
					name, Expression.Constant("_"), season),
					isId
				)
			);
		}

		internal static bool TryBindVariantsOf<T>(FieldInfo manager, string prefix, out Action<T, Season, List<T>> variantsOf)
			where T : Item
		{
			variantsOf = null;

			var mg = manager.FieldType.GetMethod("GetAvailableTextureModels", new[] {typeof(string), typeof(string), typeof(Season)});
			if (!mg.ReturnType.TryGetGenericOf(0, out var modelType))
				return false;

			var seasonGetter = modelType.GetProperty("Season", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetMethod;

			variantsOf = (source, season, list) => {
				var tm = manager.GetValue(null);
				IList models = mg.Invoke(tm, new object[] { prefix + source.ItemId, prefix + source.Name, season }) as IList;
				for (int i = 0; i < models.Count; i++)
				{
					var m = models[i] as dynamic;
					IList manualVariants = m.ManualVariations;
					List<int> manualIndices = new(manualVariants.Count);

					for (int j = 0; j < manualVariants.Count; j++)
					{
						int index = (manualVariants[j] as dynamic).Id;
						if (index is not -1)
							manualIndices.Add(index);
					}

					if (manualIndices.Count is not 0)
					{
						for (int j = 0; j < manualIndices.Count; j++)
						{
							var furn = source.getOne() as T;
							GetVariant(manualIndices[j], m, furn, seasonGetter);
							list.Add(furn);
						}
					} else
					{
						int count = m.Variations;
						for (int j = 0; j < count; j++)
						{
							var furn = source.getOne() as T;
							GetVariant(j, m, furn, seasonGetter);
							list.Add(furn);
						}
					}
				}
			};

			return true;
		}

		private static void GetVariant(int variant, dynamic model, Item furn, MethodBase SeasonGetter)
		{
			furn.modData[KEY_OWNER] = model.Owner;
			furn.modData[KEY_NAME] = model.GetId();
			furn.modData[KEY_VARIATION] = variant.ToString();
			furn.modData[KEY_SEASON] = SeasonGetter.Invoke(model, null);
			furn.modData[KEY_DISPLAY_NAME] = string.Empty;
		}
	}
}
