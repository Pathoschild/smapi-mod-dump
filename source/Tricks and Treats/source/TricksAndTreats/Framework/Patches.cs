/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI.Events;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
	public static class HarmonyPatches
	{
		internal class PatchTemplate
		{
			public readonly HarmonyPatchType type;
			public readonly MethodInfo original;
			public readonly string patch;
			public readonly HarmonyMethod method;

			public PatchTemplate(HarmonyPatchType type, MethodInfo original, string patch = null, HarmonyMethod method = null)
			{
				this.type = type;
				this.original = original;
				this.patch = patch ?? method.methodName;
				this.method = method ?? new HarmonyMethod(
					methodType: typeof(HarmonyPatches),
					methodName: patch);
			}
		}

		internal static void Patch(string id)
		{
			Harmony harmony = new Harmony(id);

			List<PatchTemplate> patches = new List<PatchTemplate>
			{

				new PatchTemplate(
					type: HarmonyPatchType.Postfix,
					original: AccessTools.Method(typeof(Item), nameof(Item.canStackWith)),
					patch: nameof(HarmonyPatches.Item_CanStackWith_Postfix)),
				/*
				new PatchTemplate(
					type: HarmonyPatchType.Postfix,
					original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
					patch: nameof(HarmonyPatches.FarmerRenderer_Draw_Postfix)),
				*/
			};

			Log.Trace(patches.Aggregate("Applying Harmony patches:", (str, p) => $"{str}{Environment.NewLine}{p.patch}"));

			foreach (PatchTemplate patch in patches)
			{
				harmony.Patch(
					original: patch.original,
					prefix: patch.type == HarmonyPatchType.Prefix ? patch.method : null,
					postfix: patch.type == HarmonyPatchType.Postfix ? patch.method : null,
					transpiler: patch.type == HarmonyPatchType.Transpiler ? patch.method : null,
					finalizer: patch.type == HarmonyPatchType.Finalizer ? patch.method : null);
			}
		}

		private static void ErrorHandler(Exception e)
		{
			Log.Error($"TaT: Tricks and Treats failed in harmony patch method.{Environment.NewLine}{e}");
		}

		/// <summary>
		/// Prevent Mystery Treats with different contents from stacking
		/// </summary>
		public static void Item_CanStackWith_Postfix(ref Item __instance, ref bool __result, ISalable other)
		{
			try
			{
				if (!__result) return;
				bool this_isMystery = __instance.modData.TryGetValue(MysteryKey, out string this_treat);
				bool other_isMystery = (other as StardewValley.Object).modData.TryGetValue(MysteryKey, out string other_treat);
				if (this_isMystery && other_isMystery)
					__result = (this_treat == other_treat);
				else if (this_isMystery || other_isMystery)
					__result = false;
			}
			catch (Exception e)
			{
				ErrorHandler(e);
			}
			return;
		}

		/*
		/// <summary>
		/// Draw cobweb over farmer's body and clothes
		/// </summary>
		public static void FarmerRenderer_Draw_Postfix(ref FarmerRenderer __instance, SpriteBatch b)
		{
			if (!Game1.player.modData.ContainsKey(CobwebKey))
				return;
			try
			{
				if (sourceImage is null)
					sourceImage = Helper.ModContent.Load<Texture2D>("assets/cobweb.png");

				b.End();
				var prev_format = Game1.graphics.PreferredDepthStencilFormat;
				Game1.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
				Game1.graphics.ApplyChanges();

				Game1.graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

				var m = Matrix.CreateOrthographicOffCenter(0,
					Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
					Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
					0, 0, 1
				);

				var a = new AlphaTestEffect(Game1.graphics.GraphicsDevice)
				{
					Projection = m
				};

				var s1 = new DepthStencilState
				{
					StencilEnable = true,
					StencilFunction = CompareFunction.Always,
					StencilPass = StencilOperation.Replace,
					ReferenceStencil = 1,
					DepthBufferEnable = false,
				};

				var s2 = new DepthStencilState
				{
					StencilEnable = true,
					StencilFunction = CompareFunction.LessEqual,
					StencilPass = StencilOperation.Keep,
					ReferenceStencil = 1,
					DepthBufferEnable = false,
				};

				Texture2D baseTexture = Helper.Reflection.GetField<Texture2D>(__instance, "baseTexture").GetValue();

				b.Begin(SpriteSortMode.Immediate, null, null, s1, null, null);
				b.Draw(baseTexture, Vector2.Zero, Color.White); //The mask                                   
				b.End();

				b.Begin(SpriteSortMode.Immediate, null, null, s2, null, null);
				b.Draw(sourceImage, Vector2.Zero, Color.White); //The background
				b.End();

				Game1.graphics.PreferredDepthStencilFormat = prev_format;
				Game1.graphics.ApplyChanges();

				b.Begin();
			}
			catch (Exception e)
			{
				ErrorHandler(e);
			}
			return;
		}
		*/
	}
}