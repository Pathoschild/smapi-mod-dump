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
using HappyHomeDesigner.Menus;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HappyHomeDesigner.Patches
{
	internal class HandCatalogue
	{
		private static readonly Vector2 menuOffset = new(32f, 32f);
		private static readonly Vector2 menuOrigin = new(8f, 8f);
		private const float PIXEL_DEPTH = 1f / 10_000f;
		private const float DISCRIMINATOR = PIXEL_DEPTH / 10f;
		private static readonly ConditionalWeakTable<Item, FrameData> frameData = new();

		public static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				typeof(Tool).GetMethod(nameof(Tool.DoFunction)),
				postfix: new(typeof(HandCatalogue), nameof(OpenIfCatalogue))
			);
			harmony.TryPatch(
				typeof(Tool).GetMethod(nameof(Tool.drawInMenu), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
				postfix: new(typeof(HandCatalogue), nameof(DrawInMenu))
			);
			harmony.TryPatch(
				typeof(GameLocation).GetMethod(nameof(GameLocation.checkAction)),
				transpiler: new(typeof(HandCatalogue), nameof(InjectAddCatalogue))
			);
		}

		private static void OpenIfCatalogue(Tool __instance, Farmer who)
		{
			if (__instance.QualifiedItemId != "(T)" + AssetManager.PORTABLE_ID)
				return;

			if (!who.IsLocalPlayer)
				return;

			var catalogues = 
				ModUtilities.CatalogType.Collector | 
				ModUtilities.CatalogType.Furniture | 
				ModUtilities.CatalogType.Wallpaper;

			Catalog.ShowCatalog(ModUtilities.GenerateCombined(catalogues), catalogues.ToString());
		}

		private static void DrawInMenu(Tool __instance, SpriteBatch spriteBatch, Vector2 location, 
			float scaleSize, float transparency, float layerDepth, Color color)
		{
			if (__instance.QualifiedItemId != "(T)" + AssetManager.PORTABLE_ID)
				return;

			var data = ItemRegistry.GetDataOrErrorItem("(T)" + AssetManager.PORTABLE_ID);
			var source = data.GetSourceRect();
			source.X = source.Right;

			spriteBatch.Draw(
				data.GetTexture(), location + menuOffset, source, 
				color.Mult(Utility.GetPrismaticColor(5)) * transparency, 
				0f, menuOrigin, scaleSize * 4f, SpriteEffects.None, layerDepth
			);
		}

		private static IEnumerable<CodeInstruction> InjectAddCatalogue(IEnumerable<CodeInstruction> source, ILGenerator gen)
		{
			var il = new CodeMatcher(source, gen);

			il
				.MatchStartForward(
					new(OpCodes.Ldarg_0),
					new(OpCodes.Ldfld, typeof(GameLocation).GetField(nameof(GameLocation.furniture)))
				)
				.MatchStartForward(
					new(OpCodes.Ldloc_S),
					new(OpCodes.Brfalse_S)
				)
				.MatchStartForward(
					new(OpCodes.Ldarg_3),
					new(OpCodes.Callvirt, typeof(Farmer).GetProperty(nameof(Farmer.ActiveObject)).GetMethod)
				);

			if (il.IsInvalid)
			{
				ModEntry.monitor.Log("Failed to find injection point for catalogue placement", LogLevel.Error);
				return null;
			}

			var injection = il.Pos;

			il
				.MatchStartForward(new CodeMatch(OpCodes.Brfalse_S))
				.MatchStartForward(new CodeMatch(OpCodes.Ldloc_S));

			if (il.IsInvalid)
			{
				ModEntry.monitor.Log("Failed to find furniture local for catalogue placement", LogLevel.Error);
				return null;
			}

			var furniture = il.Instruction.operand;

			il
				.MatchStartForward(
					new(OpCodes.Callvirt, typeof(StardewValley.Object).GetMethod(nameof(Furniture.performObjectDropInAction))),
					new(OpCodes.Brfalse_S)
				)
				.MatchStartForward(
					new(OpCodes.Stloc_S),
					new(OpCodes.Leave)
				);

			if (il.IsInvalid)
			{
				ModEntry.monitor.Log("Failed to find return point for catalogue placement", LogLevel.Error);
				return null;
			}

			var retVal = il.Instruction.Clone();
			retVal.labels.Clear();
			il.Advance(1);
			var retLoc = il.Instruction.Clone();
			retLoc.labels.Clear();

			il
				.Advance(injection - il.Pos)
				.CreateLabel(out var skip)
				.InsertAndAdvance(
					new(OpCodes.Ldloc, furniture),
					new(OpCodes.Ldarg_3),
					new(OpCodes.Call, typeof(HandCatalogue).GetMethod(nameof(TryInsertCatalogue))),
					new(OpCodes.Brfalse, skip),
					new(OpCodes.Ldc_I4_1),
					retVal,
					retLoc
				);

			var ret = il.InstructionEnumeration();
			return ret;
		}

		public static bool TryInsertCatalogue(Furniture what, Farmer who)
		{
			return
				who.ActiveItem is not null && 
				who.ActiveItem.QualifiedItemId is "(T)" + AssetManager.PORTABLE_ID &&
				what.performObjectDropInAction(ItemRegistry.Create("(O)" + AssetManager.PORTABLE_ID), false, who);
		}

		internal static void DrawInWorld(Item held, SpriteBatch batch, Vector2 position, float depth)
		{
			const int startLoop = 8;
			const int endLoop = 37;
			const float closeOffset = -3.5f * 4f;
			const float offset = -2.5f * 4f;

			var localPos = Game1.GlobalToLocal(Game1.viewport, position);
			var texture = AssetManager.BookSpriteSheet;

			if (!frameData.TryGetValue(held, out var fdata))
				frameData.Add(held, fdata = new(-1));
			var frame = fdata.Frame;

			if (fdata.LastTickedAt != Game1.ticks)
			{
				if (DistanceToNearestPlayer(position) < 200f)
					frame++;
				else
					frame--;

				if (frame < 0)
					frame = 0;
				else if (frame > endLoop)
					frame = ((frame - startLoop) % (endLoop - startLoop)) + startLoop;

				fdata.Frame = frame;
				fdata.LastTickedAt = Game1.ticks;
			}

			if (frame < startLoop)
				localPos.X += closeOffset * (startLoop - frame) / startLoop;
			localPos.X += offset;
			localPos.Y += offset;

			batch.Draw(texture, localPos, new Rectangle(frame * 20, 0, 20, 20), Color.White, 
				0f, Vector2.Zero, 4f, SpriteEffects.None, depth + DISCRIMINATOR);

			batch.Draw(texture, localPos, new Rectangle(frame * 20, 20, 20, 20), Utility.GetPrismaticColor(5), 
				0f, Vector2.Zero, 4f, SpriteEffects.None, depth + DISCRIMINATOR * 2f);
		}

		private static float DistanceToNearestPlayer(Vector2 where)
		{
			var loc = Game1.currentLocation;
			float minDist = float.PositiveInfinity;
			foreach (var player in Game1.getOnlineFarmers())
			{
				if (player.currentLocation == loc)
					minDist = MathF.Min((player.Position - where).Length(), minDist);
			}
			return minDist;
		}

		private class FrameData(int frame)
		{
			public int Frame = frame;
			public int LastTickedAt = Game1.ticks;
		}
	}
}
