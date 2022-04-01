/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpriteMaster;

static partial class Debug {
	static internal class Mode {
		[Flags]
		internal enum DebugModeFlags {
			None = 0,
			Select = 1 << 0
		}

		internal static DebugModeFlags CurrentMode = DebugModeFlags.None;

		internal static bool IsModeEnabled(DebugModeFlags mode) => (CurrentMode & mode) == mode;

		internal static void EnableMode(DebugModeFlags mode) => CurrentMode |= mode;

		internal static void DisableMode(DebugModeFlags mode) => CurrentMode &= ~mode;

		internal static void ToggleMode(DebugModeFlags mode) => CurrentMode ^= mode;

		private static Vector2I CurrentCursorPosition {
			get {
				var mouseRaw = (Vector2I)StardewValley.Game1.getMousePositionRaw();

				if (Game1.uiMode) {
					var screenRatio = (Vector2F)(Vector2I)StardewValley.Game1.uiViewport.Size / (Vector2F)(Vector2I)StardewValley.Game1.viewport.Size;
					return ((Vector2F)mouseRaw * screenRatio).NearestInt();
				}
				else {
					return StardewValley.Game1.getMousePositionRaw();
				}
			}
		}

		[CommandAttribute("debug", "Debug Commands")]
		public static void OnConsoleCommand(string command, Queue<string> arguments) {
			if (arguments.Count == 0) {
				Debug.Error("No arguments passed for 'debug' command");
				return;
			}

			var subCommand = arguments.Dequeue();
			switch (subCommand.ToLowerInvariant()) {
				case "mode":
					ProcessModeCommand(arguments);
					break;
				default:
					Debug.Error($"Unknown 'debug' command: '{subCommand}'");
					break;
			}
		}

		private static void ProcessModeCommand(Queue<string> arguments) {
			if (arguments.Count == 0) {
				if (CurrentMode == DebugModeFlags.None) {
					Debug.Info("No Debug Modes are set");
					return;
				}

				var modes = new List<string>();
				foreach (var flag in Enum.GetNames<DebugModeFlags>()) {
					var value = Enum.Parse<DebugModeFlags>(flag);

					if (value != DebugModeFlags.None && CurrentMode.HasFlag(value)) {
						modes.Add(flag);
					}
				}

				Debug.Info($"Current Debug Modes: {string.Join(", ", modes)}");
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
						Debug.Info($"Debug Mode set to '{DebugModeFlags.None}'");
					}
					else {
						foreach (var value in Enum.GetValues<DebugModeFlags>()) {
							CurrentMode |= value;
						}
						Debug.Info($"All Debug Mode flags enabled");
					}
					break;
				default:
					foreach (var flag in Enum.GetNames<DebugModeFlags>()) {
						if (invariantMode == flag.ToLowerInvariant()) {
							var value = Enum.Parse<DebugModeFlags>(flag);

							if (enable) {
								if (CurrentMode.HasFlag(value)) {
									Debug.Warning($"Debug Mode flag is already set: '{flag}'");
								}
								else {
									CurrentMode |= value;
									Debug.Info($"Debug Mode flag set: '{flag}'");
								}
							}
							else {
								if (!CurrentMode.HasFlag(value)) {
									Debug.Warning($"Debug Mode flag is not set: '{flag}'");
								}
								else {
									CurrentMode &= ~value;
									Debug.Info($"Debug Mode flag unset: '{flag}'");
								}
							}

							return;
						}
					}
					Debug.Error($"Unknown debug mode: '{mode}'");
					break;
			}
		}

		private readonly struct DrawInfo {
			internal readonly ManagedSpriteInstance? Instance;
			internal readonly XNA.Graphics.Texture2D Texture;
			internal readonly Vector2F? OriginalPosition = null;
			internal readonly Bounds? OriginalSource = null;
			internal readonly Bounds Destination;
			internal readonly Bounds Source;
			internal readonly XNA.Color Color;
			internal readonly float Rotation;
			internal readonly Vector2F Origin;
			internal readonly XNA.Graphics.SpriteEffects Effects;
			internal readonly float LayerDepth;

			internal DrawInfo(
				ManagedSpriteInstance? instance,
				XNA.Graphics.Texture2D texture,
				in Bounds destination,
				in Bounds source,
				in XNA.Color color,
				float rotation,
				Vector2F origin,
				XNA.Graphics.SpriteEffects effects,
				float layerDepth,
				in Vector2F? originalPosition = null,
				in Bounds? originalSource = null
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
				OriginalPosition = originalPosition;
				OriginalSource = originalSource;
			}
		}

		private static readonly List<DrawInfo> SelectedDraws = new();

		internal static bool RegisterDrawForSelect(
			ManagedSpriteInstance? instance,
			XNA.Graphics.Texture2D texture,
			in Bounds destination,
			in Bounds source,
			in XNA.Color color,
			float rotation,
			Vector2F origin,
			XNA.Graphics.SpriteEffects effects,
			float layerDepth,
			in Vector2F? originalPosition = null,
			in Bounds? originalSource = null
		) {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

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
					destination: destination,
					source: source,
					color: color,
					rotation: rotation,
					origin: origin,
					effects: effects,
					layerDepth: layerDepth
				));

				return true;
			}

			return false;
		}

		internal static bool RegisterDrawForSelect(
			ManagedSpriteInstance? instance,
			XNA.Graphics.Texture2D texture,
			Vector2F position,
			in Bounds source,
			in XNA.Color color,
			float rotation,
			Vector2F origin,
			Vector2F scale,
			XNA.Graphics.SpriteEffects effects,
			float layerDepth,
			in Vector2F? originalPosition = null,
			in Bounds? originalSource = null
		) {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

			var roundedPosition = position.NearestInt();
			var roundedExtent = (source.ExtentF * scale).NearestInt();

			Bounds destination = new(
				roundedPosition,
				roundedExtent
			);
			return RegisterDrawForSelect(
				instance: instance,
				texture: texture,
				originalPosition: originalPosition,
				originalSource: originalSource,
				destination: destination,
				source: source,
				color: color,
				rotation: rotation,
				origin: origin * scale,
				effects: effects,
				layerDepth: layerDepth
			);
			// new Bounds(((Vector2F)adjustedPosition).NearestInt(), (sourceRectangle.ExtentF * adjustedScale).NearestInt())
		}

		private static readonly Vector2F DimensionalScalingFactor = (1.0f, 1.0f);
		private static readonly Vector2F TextOffset = (15.0f, 15.0f);

		internal static bool Draw() {
			if (!IsModeEnabled(DebugModeFlags.Select)) {
				return false;
			}

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
				sb.AppendLine($"  dst: {draw.Destination}");
				sb.AppendLine($"  src: {draw.Source}");

				if (draw.OriginalPosition.HasValue) {
					sb.AppendLine($" opos: {draw.OriginalPosition.Value}");
				}

				var originalSource = draw.OriginalSource ?? draw.Instance?.OriginalSourceRectangle;

				if (originalSource.HasValue) {
					sb.AppendLine($" osrc: {originalSource.Value}");
				}

				sb.AppendLine($"  org: {draw.Origin}");
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
						scale: 1
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
