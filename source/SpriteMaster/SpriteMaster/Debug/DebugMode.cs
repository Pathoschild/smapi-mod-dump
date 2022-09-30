/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SpriteMaster;

internal static partial class Debug {
	internal static class Mode {
		[Flags]
		internal enum DebugModeFlags {
			None = 0,
			Select = 1 << 0
		}

		internal static DebugModeFlags CurrentMode = DebugModeFlags.None;

		internal static bool IsEnabled => CurrentMode != DebugModeFlags.None;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsModeEnabled(DebugModeFlags mode) => (CurrentMode & mode) == mode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void EnableMode(DebugModeFlags mode) => CurrentMode |= mode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DisableMode(DebugModeFlags mode) => CurrentMode &= ~mode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ToggleMode(DebugModeFlags mode) => CurrentMode ^= mode;

		private static Vector2I CurrentCursorPosition {
			get {
				var mouseRaw = (Vector2I)Game1.getMousePositionRaw();

				if (Game1.uiMode) {
					var screenRatio = (Vector2F)(Vector2I)Game1.uiViewport.Size / (Vector2F)(Vector2I)Game1.viewport.Size;
					return ((Vector2F)mouseRaw * screenRatio).NearestInt();
				}
				else {
					return Game1.getMousePositionRaw();
				}
			}
		}

		[Command("debug", "Debug Commands")]
		[UsedImplicitly]
		public static void OnConsoleCommand(string command, Queue<string> arguments) {
			if (arguments.Count == 0) {
				Error("No arguments passed for 'debug' command");
				return;
			}

			var subCommand = arguments.Dequeue();
			switch (subCommand.ToLowerInvariant()) {
				case "mode":
					ProcessModeCommand(arguments);
					break;
				default:
					Error($"Unknown 'debug' command: '{subCommand}'");
					break;
			}
		}

		private static void ProcessModeCommand(Queue<string> arguments) {
			if (arguments.Count == 0) {
				if (CurrentMode == DebugModeFlags.None) {
					Info("No Debug Modes are set");
					return;
				}

				var modes = new List<string>();
				foreach (var flag in Enum.GetNames<DebugModeFlags>()) {
					var value = Enum.Parse<DebugModeFlags>(flag);

					if (value != DebugModeFlags.None && CurrentMode.HasFlag(value)) {
						modes.Add(flag);
					}
				}

				Info($"Current Debug Modes: {string.Join(", ", modes)}");
				return;
			}

			var mode = arguments.Dequeue();

			bool enable = true;
			if (mode.StartsWith('~')) {
				enable = false;
				mode = mode.Substring(1);
			}

			var invariantMode = mode.ToLowerInvariant();

			switch (invariantMode) {
				case "off":
				case "0":
				case "disabled":
				case "disable":
				case "none":
					if (enable) {
						CurrentMode = DebugModeFlags.None;
						Info($"Debug Mode set to '{DebugModeFlags.None}'");
					}
					else {
						foreach (var value in Enum.GetValues<DebugModeFlags>()) {
							CurrentMode |= value;
						}
						Info("All Debug Mode flags enabled");
					}
					break;
				default:
					foreach (var flag in Enum.GetNames<DebugModeFlags>()) {
						if (invariantMode == flag.ToLowerInvariant()) {
							var value = Enum.Parse<DebugModeFlags>(flag);

							if (enable) {
								if (CurrentMode.HasFlag(value)) {
									Warning($"Debug Mode flag is already set: '{flag}'");
								}
								else {
									CurrentMode |= value;
									Info($"Debug Mode flag set: '{flag}'");
								}
							}
							else {
								if (!CurrentMode.HasFlag(value)) {
									Warning($"Debug Mode flag is not set: '{flag}'");
								}
								else {
									CurrentMode &= ~value;
									Info($"Debug Mode flag unset: '{flag}'");
								}
							}

							return;
						}
					}
					Error($"Unknown debug mode: '{mode}'");
					break;
			}
		}

		[StructLayout(LayoutKind.Auto)]
		private readonly struct DrawInfo {
			internal readonly ManagedSpriteInstance? Instance;
			internal readonly XTexture2D Texture;
			internal readonly Vector2F? OriginalPosition = null;
			internal readonly Bounds? OriginalSource = null;
			internal readonly Bounds? OriginalDestination = null;
			internal readonly Bounds Destination;
			internal readonly Bounds Source;
			internal readonly XColor Color;
			internal readonly float Rotation;
			internal readonly Vector2F? OriginalOrigin = null;
			internal readonly Vector2F Origin;
			internal readonly XGraphics.SpriteEffects Effects;
			internal readonly float LayerDepth;
			internal readonly float Scale;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal DrawInfo(
				ManagedSpriteInstance? instance,
				XTexture2D texture,
				Bounds destination,
				Bounds source,
				in XColor color,
				float rotation,
				Vector2F origin,
				XGraphics.SpriteEffects effects,
				float layerDepth,
				float scale,
				Vector2F? originalPosition = null,
				Bounds? originalSource = null,
				Bounds? originalDestination = null,
				Vector2F? originalOrigin = null
			) {
				Instance = instance;
				Texture = texture;
				Destination = destination;
				Source = source;
				Color = color;
				Rotation = rotation;
				Origin = origin;
				Effects = effects;
				LayerDepth = layerDepth;
				Scale = scale;
				OriginalPosition = originalPosition;
				OriginalSource = originalSource;
				OriginalDestination = originalDestination;
				OriginalOrigin = originalOrigin;
			}
		}

		private static readonly List<DrawInfo> SelectedDraws = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool RegisterDrawForSelect(
			ManagedSpriteInstance? instance,
			XTexture2D texture,
			Bounds destination,
			Bounds source,
			in XColor color,
			float rotation,
			Vector2F origin,
			XGraphics.SpriteEffects effects,
			float layerDepth,
			Vector2F? scale = null,
			Bounds? originalDestination = null,
			Vector2F? originalPosition = null,
			Bounds? originalSource = null,
			Vector2F? originalOrigin = null
		) {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

			return RegisterDrawForSelectImpl(instance, texture, destination, source, color, rotation, origin, effects, layerDepth, scale, originalDestination, originalPosition, originalSource, originalOrigin);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool RegisterDrawForSelectImpl(
			ManagedSpriteInstance? instance,
			XTexture2D texture,
			Bounds destination,
			Bounds source,
			in XColor color,
			float rotation,
			Vector2F origin,
			XGraphics.SpriteEffects effects,
			float layerDepth,
			Vector2F? scale = null,
			Bounds? originalDestination = null,
			Vector2F? originalPosition = null,
			Bounds? originalSource = null,
			Vector2F? originalOrigin = null
		) {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

			scale ??= Vector2F.One;

			var currentCursor = CurrentCursorPosition;
			if (currentCursor == destination.Offset) {
				if (texture == Game1.mouseCursors || texture == Game1.mouseCursors2) {
					return false;
				}
				if (texture is ManagedTexture2D managedTexture && managedTexture.Reference.TryGetTarget(out var reference) && (reference == Game1.mouseCursors || reference == Game1.mouseCursors2)) {
					return false;
				}
			}

			var realDestination = destination;
			realDestination.Offset -= origin.NearestInt();

			if (realDestination.Contains(currentCursor)) {
				SelectedDraws.Add(new(
					instance: instance,
					texture: texture,
					originalPosition: originalPosition,
					originalSource: originalSource,
					originalDestination: originalDestination,
					destination: destination,
					source: source,
					color: color,
					rotation: rotation,
					originalOrigin: originalOrigin,
					origin: origin,
					effects: effects,
					layerDepth: layerDepth,
					scale: 1.0f
				));

				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool RegisterDrawForSelect(
			ManagedSpriteInstance? instance,
			XTexture2D texture,
			Vector2F position,
			Bounds source,
			in XColor color,
			float rotation,
			Vector2F origin,
			Vector2F scale,
			XGraphics.SpriteEffects effects,
			float layerDepth,
			Vector2F? originalPosition = null,
			Bounds? originalSource = null,
			Vector2F? originalOrigin = null
		) {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

			return RegisterDrawForSelectImpl(
				instance, texture, position, source, color, rotation, origin, scale, effects, layerDepth, originalPosition,
				originalSource, originalOrigin
			);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool RegisterDrawForSelectImpl(
			ManagedSpriteInstance? instance,
			XTexture2D texture,
			Vector2F position,
			Bounds source,
			in XColor color,
			float rotation,
			Vector2F origin,
			Vector2F scale,
			XGraphics.SpriteEffects effects,
			float layerDepth,
			Vector2F? originalPosition = null,
			Bounds? originalSource = null,
			Vector2F? originalOrigin = null
		) {
			var roundedPosition = position.NearestInt();
			var roundedExtent = (source.ExtentF * scale).NearestInt();

			Bounds destination = new(
				roundedPosition,
				roundedExtent
			);
			return RegisterDrawForSelectImpl(
				instance: instance,
				texture: texture,
				originalPosition: originalPosition,
				originalSource: originalSource,
				destination: destination,
				source: source,
				color: color,
				rotation: rotation,
				originalOrigin: originalOrigin,
				origin: origin * scale,
				effects: effects,
				layerDepth: layerDepth,
				scale: scale
			);
			// new Bounds(((Vector2F)adjustedPosition).NearestInt(), (sourceRectangle.ExtentF * adjustedScale).NearestInt())
		}

		private static readonly Vector2F DimensionalScalingFactor = (1.0f, 1.0f);
		private static readonly Vector2F TextOffset = (15.0f, 15.0f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool Draw(TimeSpan? frameTimeCPU, TimeSpan? frameTimeTotal) {
			if (frameTimeCPU.HasValue) {
				DrawFrameTime("CPU", frameTimeCPU.Value, 0.0f);
			}
			if (frameTimeTotal.HasValue) {
				DrawFrameTime("Total", frameTimeTotal.Value, 20.0f);
			}

			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

			return DrawImpl();
		}

		private static void DrawFrameTime(string name, TimeSpan frameTime, float offset) {
			string frameTimeString = $"{frameTime.TotalMilliseconds:N2} ms ({name})";

			Game1.spriteBatch.Begin();
			Utility.drawTextWithShadow(
				b: Game1.spriteBatch,
				text: frameTimeString,
				font: Game1.smallFont,
				position: new(200.0f, offset),
				color: XColor.White,
				scale: 1.0f
			);
			Game1.spriteBatch.End();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool DrawImpl() {
			if (SelectedDraws.Count == 0) {
				return false;
			}

			List<StringBuilder> lines = new(SelectedDraws.Count);
			foreach (var draw in SelectedDraws) {
				var sb = new StringBuilder();
				sb.AppendLine(draw.Texture.NormalizedName());
				if (draw.Instance is not null) {
					sb.AppendLine(draw.Instance.Hash.ToString16());
				}

				List<(string Key, string Value)> properties = new();

				properties.Add(("dst", draw.Destination.ToString()));
				if (draw.OriginalDestination is not null) {
					properties.Add(("odst", draw.OriginalDestination.Value.ToString()));
				}

				properties.Add(("src", draw.Source.ToString()));


				if (draw.OriginalPosition.HasValue) {
					properties.Add(("opos", draw.OriginalPosition.Value.ToString()));
				}

				var originalSource = draw.OriginalSource ?? draw.Instance?.OriginalSourceRectangle;

				if (originalSource.HasValue) {
					properties.Add(("osrc", originalSource.Value.ToString()));
				}

				properties.Add(("org", draw.Origin.ToString()));
				if (draw.OriginalOrigin is not null) {
					properties.Add(("oorg", draw.OriginalOrigin.Value.ToString()));
				}
				if (draw.Rotation != 0.0f) {
					properties.Add(("rot", draw.Rotation.ToString("n2")));
				}
				properties.Add(("scale", draw.Scale.ToString(CultureInfo.CurrentCulture)));
				properties.Add(("depth", draw.LayerDepth.ToString(CultureInfo.CurrentCulture)));
				if (draw.Effects != XGraphics.SpriteEffects.None) {
					properties.Add(("effects", ""));
					foreach (var enumName in Enum.GetNames<XGraphics.SpriteEffects>()) {
						var enumValue = Enum.Parse<XGraphics.SpriteEffects>(enumName);
						if (enumValue != XGraphics.SpriteEffects.None && draw.Effects.HasFlag(enumValue)) {
							properties.Add(("", enumName));
						}
					}
				}

				int maxLength = int.MinValue;
				foreach (var property in properties) {
					maxLength = Math.Max(maxLength, property.Key.Length);
				}

				foreach (var property in properties) {
					if (string.IsNullOrEmpty(property.Key)) {
						sb.AppendLine($"  {property.Value}");
					}
					else {
						var padding = maxLength - property.Key.Length;
						sb.AppendLine($"{property.Key.PadLeft(padding)}: {property.Value}");
					}
				}

				lines.Add(sb);
			}

			if (lines.Count == 0) {
				return false;
			}

			Game1.spriteBatch.Begin();
			try {
				var font = Game1.smallFont;

				Vector2I minDimensions = (Vector2I)Game1.viewport.Size / 5;
				Vector2F actualDimensions = Vector2F.Zero;

				var lineOffsets = new float[lines.Count];
				int currentLineOffset = 0;

				foreach (var line in lines) {
					var lineMeasure = font.MeasureString(line);
					actualDimensions.Width = Math.Max(actualDimensions.Width, lineMeasure.X);
					lineOffsets[currentLineOffset++] = actualDimensions.Height;
					actualDimensions.Height += lineMeasure.Y;
				}

				actualDimensions = actualDimensions.Max(minDimensions);

				actualDimensions *= DimensionalScalingFactor;
				var dimensions = actualDimensions.NearestInt();

				Game1.DrawBox(
					x: 10,
					y: 10,
					width: dimensions.Width,
					height: dimensions.Height,
					color: null
				);

				/*
				Game1.drawDialogueBox(
						x: 10,
						y: 0,
						width: dimensions.Width,
						height: dimensions.Height,
						speaker: false,
						drawOnlyBox: true,
						message: null,
						objectDialogueWithPortrait: false,
						ignoreTitleSafe: true,
						r: -1,
						g: -1,
						b: -1
				);
				*/

				currentLineOffset = 0;
				foreach (var line in lines) {
					var lineOffset = lineOffsets[currentLineOffset++];

					Utility.drawTextWithShadow(
						b: Game1.spriteBatch,
						text: line,
						font: font,
						position: TextOffset + (0.0f, lineOffset),
						color: Game1.textColor,
						scale: 1.0f
					);
				}
			}
			finally {
				Game1.spriteBatch.End();
				SelectedDraws.Clear();
			}

			return true;
		}
	}
}
