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

		public static Func<string, string, bool> HasVariant;
		public static Action<Furniture, string, List<Furniture>> VariantsOf;

		internal static void Init(IModHelper helper)
		{
			Installed = false;
			if (!helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures") || !AltTex.IsApplied)
				return;

			ModEntry.monitor.Log("Alternative Textures detected! Integrating...", LogLevel.Debug);

			if (!ModUtilities.TryFindAssembly("AlternativeTextures", out var asm))
			{
				ModEntry.monitor.Log("Failed to find AT assembly, could not integrate.", LogLevel.Warn);
				return;
			}
			var entry = asm.GetType("AlternativeTextures.AlternativeTextures");
			if (entry is null)
			{
				ModEntry.monitor.Log("Failed to find entry point for Alternative Textures.", LogLevel.Warn);
				return;
			}
			var manager = entry.GetField("textureManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (manager is null)
			{
				ModEntry.monitor.Log("Failed to find texture manager.", LogLevel.Warn);
				return;
			}
			if (!BindHasVariant(manager))
			{
				ModEntry.monitor.Log("Failed to bind HasVariant", LogLevel.Warn);
				return;
			}
			if (!BindVariantsOf(manager))
			{
				ModEntry.monitor.Log("Failed to bind VariantOf", LogLevel.Warn);
				return;
			}
			ModEntry.monitor.Log("Integration successful.", LogLevel.Debug);
			Installed = true;
		}

		private static bool BindHasVariant(FieldInfo manager)
		{
			/*
			* HasVariant(name, season)
			*	=>	AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(name) || 
			*		AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(name + "_" + season)
			*/

			var getter = manager.FieldType.GetMethod("DoesObjectHaveAlternativeTexture", new[] { typeof(string) });
			var name = Expression.Parameter(typeof(string));
			var season = Expression.Parameter(typeof(string));
			var body = Expression.Or(
				Expression.Call(Expression.Field(null, manager), getter, name),
				Expression.Call(Expression.Field(null, manager), getter, 
					Expression.Call(typeof(string).GetMethod(nameof(string.Concat), new[] {typeof(string), typeof(string), typeof(string)}),
					name, Expression.Constant("_"), season)
				)
			);
			try
			{
				HasVariant = Expression.Lambda<Func<string, string, bool>>(body, name, season).Compile();
			} catch (Exception ex)
			{
				ModEntry.monitor.Log(ex.ToString(), LogLevel.Trace);
				return false;
			}
			return true;
		}

		internal static bool BindVariantsOf(FieldInfo manager)
		{
			var mg = manager.FieldType.GetMethod("GetAvailableTextureModels");
			if (!mg.ReturnType.TryGetGenericOf(0, out var modelType))
				return false;

			var seasonGetter = modelType.GetProperty("Season", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetMethod;

			VariantsOf = (source, season, list) => {
				var tm = manager.GetValue(null);
				IList models = mg.Invoke(tm, new[] { "Furniture_" + source.Name, season }) as IList;
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
							var furn = source.getOne() as Furniture;
							GetVariant(manualIndices[j], m, furn, seasonGetter);
							list.Add(furn);
						}
					} else
					{
						int count = m.Variations;
						for (int j = 0; j < count; j++)
						{
							var furn = source.getOne() as Furniture;
							GetVariant(j, m, furn, seasonGetter);
							list.Add(furn);
						}
					}
				}
			};

			return true;
		}

		private static void GetVariant(int variant, dynamic model, Furniture furn, MethodBase SeasonGetter)
		{
			furn.modData[KEY_OWNER] = model.Owner;
			furn.modData[KEY_NAME] = model.GetId();
			furn.modData[KEY_VARIATION] = variant.ToString();
			furn.modData[KEY_SEASON] = SeasonGetter.Invoke(model, null);
			furn.modData[KEY_DISPLAY_NAME] = string.Empty;
		}
	}
}
