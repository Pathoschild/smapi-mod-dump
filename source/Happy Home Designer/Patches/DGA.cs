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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HappyHomeDesigner.Patches
{
	internal class DGA
	{
		internal static Assembly asm;

		internal static void Apply(Harmony harmony)
		{
			if (!ModUtilities.TryFindAssembly("DynamicGameAssets", out asm))
				return;

			var bedFurn = asm.GetType("DynamicGameAssets.Game.CustomBedFurniture");
			if (bedFurn is null)
			{
				ModEntry.monitor.Log("Could not find DGA bed class, DGA patching failed.", LogLevel.Warn);
				return;
			}

			if (!harmony.TryPatch(
				bedFurn.GetMethod("drawInMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
				transpiler: new(typeof(DGA), nameof(AddBedFront))
			))
				return;

			ModEntry.monitor.Log("Successfully patched DGA", LogLevel.Debug);

			if (!AltTex.IsApplied)
				return;

			var atFurn = AltTex.asm.GetType("AlternativeTextures.Framework.Patches.StandardObjects.FurniturePatch");
			if (atFurn is null)
			{
				ModEntry.monitor.Log("Could not find AT patcher, DGA-AT patching failed.", LogLevel.Warn);
				return;
			}

			if (harmony.TryPatch(
				bedFurn.GetMethod("drawInMenu", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public),
				new(atFurn, "DrawInMenuPrefix")
			))
				ModEntry.monitor.Log("Succesfully patched DGA-AT", LogLevel.Debug);
		}

		public static IEnumerable<CodeInstruction> AddBedFront(IEnumerable<CodeInstruction> codes, ILGenerator gen)
		{
			var il = new CodeMatcher(codes, gen);

			var batchdraw = typeof(SpriteBatch).GetMethod(nameof(SpriteBatch.Draw), new[] {
				typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color),
				typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });

			var flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

			var bedType = asm.GetType("DynamicGameAssets.Game.CustomBedFurniture");
			var dataType = asm.GetType("DynamicGameAssets.PackData.FurniturePackData");
			var packType = asm.GetType("DynamicGameAssets.PackData.ContentPack");

			var listStore = gen.DeclareLocal(dataType.GetMethod("get_Configurations").ReturnType);
			var itemStore = gen.DeclareLocal(listStore.LocalType.GetMethod("get_Item").ReturnType);
			var nameStore = gen.DeclareLocal(typeof(string));
			var texStore = gen.DeclareLocal(packType.GetMethod("GetTexture", flag).ReturnType);

			var skip = gen.DefineLabel();

			il
				.MatchStartForward(
					new CodeMatch(OpCodes.Callvirt, batchdraw)
				)
				.Advance(1)
				.AddLabels(new[] { skip })
				.InsertAndAdvance(

					// get config
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, bedType.GetMethod("get_Data")),
					new(OpCodes.Call, dataType.GetMethod("get_Configurations")),
					new(OpCodes.Stloc, listStore),
					new(OpCodes.Ldloc, listStore),
					new(OpCodes.Callvirt, listStore.LocalType.GetMethod("get_Count")),
					new(OpCodes.Brfalse, skip),
					new(OpCodes.Ldloc, listStore),
					new(OpCodes.Ldc_I4_0),
					new(OpCodes.Callvirt, listStore.LocalType.GetMethod("get_Item")),
					new(OpCodes.Stloc, itemStore),

					// get texture name
					new(OpCodes.Ldloc, itemStore),
					new(OpCodes.Call, itemStore.LocalType.GetMethod("get_FrontTexture")),
					new(OpCodes.Stloc, nameStore),
					new(OpCodes.Ldloc, nameStore),
					new(OpCodes.Brfalse, skip),

					// get texture
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, bedType.GetMethod("get_Data")),
					new(OpCodes.Ldfld, dataType.GetField("pack", flag)),
					new(OpCodes.Ldloc, nameStore),
					// -- width
					new(OpCodes.Ldloc, itemStore),
					new(OpCodes.Callvirt, itemStore.LocalType.GetMethod("get_DisplaySize")),
					new(OpCodes.Ldfld, typeof(Vector2).GetField(nameof(Vector2.X))),
					new(OpCodes.Conv_I4),
					new(OpCodes.Ldc_I4_S, 64),
					new(OpCodes.Mul),
					new(OpCodes.Ldc_I4_4),
					new(OpCodes.Div),
					// -- height
					new(OpCodes.Ldloc, itemStore),
					new(OpCodes.Callvirt, itemStore.LocalType.GetMethod("get_DisplaySize")),
					new(OpCodes.Ldfld, typeof(Vector2).GetField(nameof(Vector2.Y))),
					new(OpCodes.Conv_I4),
					new(OpCodes.Ldc_I4_S, 64),
					new(OpCodes.Mul),
					new(OpCodes.Ldc_I4_4),
					new(OpCodes.Div),
					// -- call
					new(OpCodes.Callvirt, packType.GetMethod("GetTexture", flag)),
					new(OpCodes.Stloc, texStore),

					// draw it
					new(OpCodes.Ldarg_1),
					// -- texture
					new(OpCodes.Ldloc, texStore),
					new(OpCodes.Call, texStore.LocalType.GetMethod("get_Texture")),
					// -- position
					new(OpCodes.Ldarg_2),
					new(OpCodes.Ldc_R4, 32f),
					new(OpCodes.Ldc_R4, 32f),
					new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new[] {typeof(float), typeof(float)})),
					new(OpCodes.Call, typeof(Vector2).GetMethod("op_Addition", new[] {typeof(Vector2), typeof(Vector2)})),
					// -- source
					new(OpCodes.Ldloc, texStore),
					new(OpCodes.Call, texStore.LocalType.GetMethod("get_Rect")),
					// -- color
					new(OpCodes.Ldarg_S, 7),
					new(OpCodes.Ldarg_S, 4),
					new(OpCodes.Call, typeof(Color).GetMethod("op_Multiply", new[] {typeof(Color), typeof(float)})),
					// -- rotation
					new(OpCodes.Ldc_R4, 0f),
					// -- origin
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.defaultSourceRect))),
					new(OpCodes.Callvirt, typeof(NetRectangle).GetProperty(nameof(NetRectangle.Width)).GetMethod),
					new(OpCodes.Ldc_I4_2),
					new(OpCodes.Div),
					new(OpCodes.Conv_R4),
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(Furniture).GetField(nameof(Furniture.defaultSourceRect))),
					new(OpCodes.Callvirt, typeof(NetRectangle).GetProperty(nameof(NetRectangle.Height)).GetMethod),
					new(OpCodes.Ldc_I4_2),
					new(OpCodes.Div),
					new(OpCodes.Conv_R4),
					new(OpCodes.Newobj, typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) })),
					// -- scale
					new(OpCodes.Ldarg_0),
					new(OpCodes.Call, bedType.GetMethod("getScaleSize", flag)),
					new(OpCodes.Ldarg_3),
					new(OpCodes.Mul),
					// -- fx
					new(OpCodes.Ldc_I4_0),
					// -- depth
					new(OpCodes.Ldarg_S, 5),
					// -- call
					new(OpCodes.Callvirt, batchdraw)
				);

			return il.InstructionEnumeration();
		}
	}
}
