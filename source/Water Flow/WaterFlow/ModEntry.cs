/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/WaterFlow
**
*************************************************/

using HarmonyLib; // el diavolo nuevo
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using Colour = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WaterFlow
{
	public enum WaterFlow
	{
		Hide = -2,
		None = -1,
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3
	}

	public class Config
	{
		public bool VerboseLogging { get; set; } = false;
	}

	public class ModEntry : Mod
	{
		public const WaterFlow DefaultWaterFlow = WaterFlow.Down;
		public const string MapPropertyGlobal = "blueberry.water.flow.global";
		public const string MapPropertyLocal = "blueberry.water.flow.local";

		public class ModState
		{
			public WaterFlow WaterFlow;
			public List<(WaterFlow flow, Rectangle area)> Areas;

			public ModState()
			{
				this.Reset();
			}

			public void Reset()
			{
				this.WaterFlow = default;
				this.Areas = new();
			}
		}

		public static readonly PerScreen<ModState> State = new(() => new ModState());

		public static IEnumerable<CodeInstruction> GameLocation_DrawWater_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo m1 = AccessTools.PropertyGetter(type: typeof(xTile.Dimensions.Rectangle), name: nameof(xTile.Dimensions.Rectangle.Width));
			List<CodeInstruction> il = instructions.ToList();
			if (il.FindIndex(match: (CodeInstruction i) => i.opcode == OpCodes.Call && (MethodInfo)i.operand == m1) is int i1
				&& il.FindIndex(startIndex: i1, match: (CodeInstruction i) => i.opcode == OpCodes.Ldc_I4_1) is int i2)
			{
				il[i2].opcode = OpCodes.Ldc_I4_2;
			}
			return il;
		}

		public static bool GameLocation_DrawWaterTile_Prefix(GameLocation __instance,
			SpriteBatch b, int x, int y, Colour color)
		{
			void draw(Vector2 position, Rectangle sourceRectangle)
			{
				const float scale = 1;
				const float rotation = 0;
				const float origin = 0;
				const SpriteEffects effects = SpriteEffects.None;
				const float layerDepth = 0.56f;

				b.Draw(
					texture: Game1.mouseCursors,
					position: Game1.GlobalToLocal(
						viewport: Game1.viewport,
						globalPosition: position),
					sourceRectangle: sourceRectangle,
					color: color,
					rotation: rotation,
					origin: new Vector2(origin),
					scale: scale,
					effects: effects,
					layerDepth: layerDepth);
			}

			bool isLocal = ModEntry.State.Value.Areas.FindIndex(pair => pair.area.Contains(x, y)) is int i && i >= 0;
			WaterFlow waterFlow = isLocal
				? ModEntry.State.Value.Areas[i].flow
				: ModEntry.State.Value.WaterFlow;

			if (waterFlow is WaterFlow.Hide)
				return false;

			const int sourceX = 0;
			const int sourceY = 2064;

			bool isLeftOrRight = waterFlow is WaterFlow.Left or WaterFlow.Right;
			bool isUpOrDown = waterFlow is WaterFlow.Up or WaterFlow.Down;
			bool isUpOrLeft = waterFlow is WaterFlow.Up or WaterFlow.Left;
			bool isDownOrRight = waterFlow is WaterFlow.Down or WaterFlow.Right;

			int forLR = isLeftOrRight ? 1 : 0;
			int forUD = isUpOrDown ? 1 : 0;
			int forUL = isUpOrLeft ? 1 : 0;
			int forDR = isUpOrLeft ? 0 : 1;

			int flipUL = isUpOrLeft ? -1 : 1;

			const int n = 1;
			int start = isLeftOrRight ? x : y;
			int span = isLeftOrRight ? __instance.Map.Layers[0].LayerWidth : __instance.Map.Layers[0].LayerHeight;

			bool isTopTile = start == 0 || !__instance.waterTiles[x - (n * forLR), y - (n * forUD)];
			bool isBottomTile = start == span - n || !__instance.waterTiles[x + (n * forLR), y + (n * forUD)];
			if (isDownOrRight)
			{
				bool temp = isTopTile;
				isTopTile = isBottomTile;
				isBottomTile = temp;
			}

			float waterPosition = __instance.waterPosition;
			bool waterTileFlip = __instance.waterTileFlip;
			if (waterFlow is WaterFlow.None)
			{
				waterPosition = 1;
				waterTileFlip = false;
			}

			int tileCrop = (int)(isDownOrRight || !isTopTile
				? waterPosition
				: 0);
			int tileSize = (int)(isTopTile
				? waterPosition
				: 0);
			int sourceOffset = ((x + y) % 2 != 0)
				? ((!waterTileFlip) ? Game1.tileSize * 2 : 0)
				: (waterTileFlip ? Game1.tileSize * 2 : 0);

			Vector2 position = new Vector2(
				x: (x * Game1.tileSize) + (tileCrop * forLR * flipUL),
				y: (y * Game1.tileSize) + (tileCrop * forUD * flipUL));
			Rectangle sourceRectangle = new Rectangle(
				x: sourceX + (tileSize * forLR * forUL) + (__instance.waterAnimationIndex * Game1.tileSize),
				y: sourceY + (tileSize * forUD * forUL) + (sourceOffset * forUD),
				width: Game1.tileSize + (-tileSize * forLR),
				height: Game1.tileSize + (-tileSize * forUD));

			draw(position: position, sourceRectangle: sourceRectangle);

			if (isBottomTile)
			{
				sourceOffset = (((x + (1 * forLR)) + (y + (1 * forUD))) % 2 != 0)
						? ((!waterTileFlip) ? Game1.tileSize * 2 : 0)
						: (waterTileFlip ? Game1.tileSize * 2 : 0);
				position = new Vector2(
					x: ((x + (1 * forLR * forUL)) * Game1.tileSize) - (int)(waterPosition * forLR * forUL),
					y: ((y + (1 * forUD * forUL)) * Game1.tileSize) - (int)(waterPosition * forUD * forUL));
				sourceRectangle = new Rectangle(
					x: (int)(sourceX + ((Game1.tileSize - waterPosition) * forLR * forDR) + (__instance.waterAnimationIndex * Game1.tileSize)),
					y: (int)(sourceY + ((sourceOffset - (waterPosition * forDR)) * forUD)),
					width: (int)(Game1.tileSize - ((Game1.tileSize - waterPosition - 1) * forLR)) - (1 * forLR),
					height: (int)(Game1.tileSize - ((Game1.tileSize - waterPosition - 1) * forUD)) - (1 * forUD));
				draw(position: position, sourceRectangle: sourceRectangle);
			}

			return false;
		}

		public override void Entry(IModHelper helper)
		{
			Config config = helper.ReadConfig<Config>();

			bool parseLocalAreaValues(PropertyValue localValue, Size mapSize)
			{
				int i = 0;
				string[] fields = null;
				string error = null;
				List<(WaterFlow flow, Rectangle area)> areas = new();
				try
				{
					ModEntry.State.Value.Areas.Clear();
					fields = localValue.ToString().Trim().Split();
					for (; i < fields.Length && string.IsNullOrEmpty(error); i += 5)
					{
						if (Enum.TryParse(enumType: typeof(WaterFlow), value: fields[i], ignoreCase: true, out object result)
							&& result is WaterFlow flow)
						{
							int[] values = fields.Skip(i + 1).Take(4).ToList().ConvertAll(int.Parse).ToArray();
							if (values[2] < 1)
								values[2] = mapSize.Width;
							if (values[3] < 1)
								values[3] = mapSize.Height;
							values[0] = Math.Min(mapSize.Width, Math.Max(0, values[0]));
							values[1] = Math.Min(mapSize.Height, Math.Max(0, values[1]));
							values[2] = Math.Min(mapSize.Width - values[0], values[2]);
							values[3] = Math.Min(mapSize.Height - values[1], values[3]);
							if (values[2] > 0 && values[3] > 0)
							{
								Rectangle area = new Rectangle(values[0], values[1], values[2], values[3]);
								areas.Add((flow: flow, area: area));
								if (config.VerboseLogging)
								{
									this.Monitor.Log(
										message: $"Parsed '{localValue}' to {nameof(WaterFlow)} {flow}, {nameof(Rectangle)} {area}.",
										level: LogLevel.Debug);
								}
							}
							else
							{
								error = $"Invalid parsed area ({nameof(Rectangle)}): {string.Join(' ', values)}";
							}
						}
						else
						{
							error = $"Invalid parsed flow ({nameof(WaterFlow)}): {fields[i + 0]}";
						}
					}
				}
				catch (Exception e)
				{
					error = $"Error while reading {nameof(WaterFlow)} entry {i} of {fields?.Length / 5 ?? 0}:{Environment.NewLine}{e}";
				}
				if (!string.IsNullOrEmpty(error))
				{
					error += $"{Environment.NewLine}Failed to read local {nameof(WaterFlow)} entry:{Environment.NewLine}'{localValue}'";
					this.Monitor.Log(message: error, level: LogLevel.Error);
					return false;
				}
				ModEntry.State.Value.Areas = areas;
				return true;
			}

			Harmony harmony = new Harmony(id: this.ModManifest.UniqueID);
			harmony.Patch(
				original: AccessTools.Method(type: typeof(GameLocation), name: nameof(GameLocation.drawWaterTile),
				parameters: new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(Colour) }),
				prefix: new HarmonyMethod(methodType: typeof(ModEntry), methodName: nameof(ModEntry.GameLocation_DrawWaterTile_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(type: typeof(GameLocation), name: nameof(GameLocation.drawWater)),
				transpiler: new HarmonyMethod(methodType: typeof(ModEntry), methodName: nameof(ModEntry.GameLocation_DrawWater_Transpiler)));

			helper.Events.Player.Warped += (object sender, WarpedEventArgs e) =>
			{
				ModEntry.State.Value.Reset();

				if (e.NewLocation is null)
					return;
				
				object result = ModEntry.DefaultWaterFlow;
				bool isCustomLocation = e.NewLocation.Name.StartsWith("Custom_", StringComparison.OrdinalIgnoreCase);
				bool isEnabledLocalInMap = e.NewLocation.Map.Properties.TryGetValue(key: ModEntry.MapPropertyLocal, out PropertyValue localValue)
					&& parseLocalAreaValues(localValue: localValue, mapSize: e.NewLocation.Map.Layers[0].LayerSize);
				bool isEnabledGlobalInMap = e.NewLocation.Map.Properties.TryGetValue(key: ModEntry.MapPropertyGlobal, out PropertyValue value)
					&& Enum.TryParse(enumType: typeof(WaterFlow), value: value, ignoreCase: true, out result) && result is WaterFlow;
				if (!isCustomLocation || isEnabledLocalInMap || isEnabledGlobalInMap)
				{
					ModEntry.State.Value.WaterFlow = isEnabledGlobalInMap ? (WaterFlow)result : ModEntry.DefaultWaterFlow;
					if (config.VerboseLogging)
					{
						this.Monitor.Log(
							message: $"{e.NewLocation.Name}: {ModEntry.State.Value.WaterFlow} ({(int)ModEntry.State.Value.WaterFlow})",
							level: LogLevel.Debug);
						foreach ((WaterFlow flow, Rectangle area) in ModEntry.State.Value.Areas)
						{
							this.Monitor.Log(
								message: $"({flow}: {area})",
								level: LogLevel.Debug);
						}
					}
				}
			};
		}
	}
}
