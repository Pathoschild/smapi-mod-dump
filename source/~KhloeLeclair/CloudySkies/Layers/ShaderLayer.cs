/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Layers;


[DiscriminatedType("Shader")]
public record ShaderData : BaseLayerData, IShaderLayerData {

	public string? Shader { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? Color { get; set; }

	[JsonExtensionData]
	public IDictionary<string, JToken> Fields { get; init; } = new FieldsEqualityDictionary();

}


public class ShaderLayer : IWeatherLayer, IDisposable {

	#region Static - Shader Loading

	internal static readonly Dictionary<string, Effect?> ShaderCache = new();
	internal static readonly Dictionary<string, DateTime?> ModifiedCache = new();

	private static string ResolveEffectPath(string path) {
		if (path.EndsWith(".fx"))
			path = $"{path[..^3]}.mgfx";

		else if (!path.EndsWith(".mgfx"))
			path = $"{path}.mgfx";

		if (!File.Exists(path)) {
			string alt = Path.Join(ModEntry.Instance.Helper.DirectoryPath, "assets", path);
			if (File.Exists(alt))
				return alt;
		}

		return path;
	}

	internal static Effect? GetEffect(string? path) {
		if (string.IsNullOrEmpty(path))
			return null;

		string source = path;
		bool resolved = false;
		bool exists = ShaderCache.ContainsKey(source);

		if (ModEntry.Instance.Config.RecompileShaders) {
			if (!resolved) {
				source = ResolveEffectPath(source);
				resolved = true;
			}

			string dirName = Path.GetDirectoryName(source)!;
			string srcFile = $"{Path.GetFileNameWithoutExtension(source)}.fx";
			string srcPath = Path.Combine(dirName, srcFile);
			if (File.Exists(srcPath)) {
				DateTime? srcModified;
				try {
					srcModified = File.GetLastWriteTime(srcPath);
				} catch (Exception) {
					srcModified = null;
				}

				if (ModifiedCache.GetValueOrDefault(srcPath) != srcModified) {
					if (exists) {
						ShaderCache[path]?.Dispose();
						ShaderCache.Remove(path);
						ModifiedCache[source] = null;
						exists = false;
					}

					try {
						// Attempt to compile the shader.
						var pinfo = new ProcessStartInfo {
							WorkingDirectory = dirName,
							FileName = "mgfxc",
							Arguments = $"\"{srcFile}\" \"{Path.GetFileName(source)}\" /Profile:OpenGL",
							RedirectStandardError = true,
							RedirectStandardOutput = true,
							CreateNoWindow = true
						};

						var proc = Process.Start(pinfo);
						proc!.WaitForExit();

						string err = proc.StandardError.ReadToEnd();
						if (!string.IsNullOrWhiteSpace(err)) {
							ModEntry.Instance.Log($"Error compiling shader '{path}' from '{srcFile}': {err}", StardewModdingAPI.LogLevel.Error);
							return null;
						}

						ModEntry.Instance.Log($"Compiled shader '{path}' from '{srcFile}' using mgfxc.", StardewModdingAPI.LogLevel.Alert);
						ModifiedCache[srcPath] = srcModified;

					} catch (Exception ex) {
						ModEntry.Instance.Log($"Error compiling shader '{path}' from '{srcFile}': {ex}", StardewModdingAPI.LogLevel.Error);
						ModifiedCache[srcPath] = null;
						return null;
					}
				}
			}
		}

		if (exists) {
			source = ResolveEffectPath(source);
			resolved = true;
			DateTime? modified;
			try {
				modified = File.GetLastWriteTime(source);
			} catch (Exception) {
				modified = null;
			}

			if (ModifiedCache.GetValueOrDefault(source) != modified) {
				ShaderCache[path]?.Dispose();
				ShaderCache.Remove(path);
				exists = false;
			}
		}

		if (exists)
			return ShaderCache[path];

		if (!resolved)
			source = ResolveEffectPath(source);

		Effect? effect;

		try {
			byte[] bytes = File.ReadAllBytes(source);
			effect = new Effect(Game1.graphics.GraphicsDevice, bytes);

			ModifiedCache[source] = File.GetLastWriteTime(source);

		} catch (Exception ex) {
			ModEntry.Instance.Log($"Error loading shader from '{path}': {ex}", StardewModdingAPI.LogLevel.Warn);
			effect = null;
			ModifiedCache[source] = null;
		}

		ShaderCache[path] = effect;
		return effect;
	}

	#endregion

	private readonly ModEntry Mod;


	public ulong Id { get; }

	public LayerDrawType DrawType => LayerDrawType.Normal;

	private readonly IShaderLayerData Data;
	private Effect? Effect;

	private EffectParameter? ParamTotalTime;
	private EffectParameter? ParamElapsedTime;
	private EffectParameter? ParamScreenSize;
	private EffectParameter? ParamViewportPosition;
	private EffectParameter? ParamTimeOfDay;

	protected RenderTarget2D? WorkingTarget;

	private SpriteBatch Batch;
	private bool isDisposed;

	public ShaderLayer(ModEntry mod, ulong id, IShaderLayerData data) {
		Mod = mod;
		Id = id;
		Data = data;

		Batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

		ReloadAssets();
	}

	protected virtual void Dispose(bool disposing) {
		if (!isDisposed) {
			Mod.RemoveLoadsAsset(Id);

			Batch?.Dispose();
			Effect?.Dispose();
			WorkingTarget?.Dispose();

			ParamTotalTime = null;
			ParamElapsedTime = null;
			ParamScreenSize = null;
			ParamViewportPosition = null;

			Batch = null!;
			Effect = null;
			WorkingTarget = null;

			isDisposed = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[MemberNotNull(nameof(WorkingTarget))]
	protected void CreateOrUpdateBuffer(RenderTarget2D source) {
		int width = source.Width;
		int height = source.Height;

		if (WorkingTarget is null || WorkingTarget.Width != width || WorkingTarget.Height != height) {
			WorkingTarget?.Dispose();
			WorkingTarget = new(Game1.graphics.GraphicsDevice, width, height);
		}
	}

	public void ReloadAssets() {
		Effect?.Dispose();
		Effect = GetEffect(Data.Shader)?.Clone();
		ConfigureEffect();
	}

	private void ConfigureEffect() {
		if (Effect is null) {
			ParamTotalTime = null;
			ParamElapsedTime = null;
			ParamViewportPosition = null;
			ParamScreenSize = null;
			return;
		}

		ParamTotalTime = Effect.Parameters["TotalTime"];
		ParamElapsedTime = Effect.Parameters["ElapsedTime"];
		ParamViewportPosition = Effect.Parameters["ViewportPosition"];
		ParamScreenSize = Effect.Parameters["ScreenSize"];
		ParamTimeOfDay = Effect.Parameters["TimeOfDay"];

		ParamScreenSize?.SetValue(new Vector2(Game1.viewport.Width, Game1.viewport.Height));

		// Remove any previously hinted at textures.
		Mod.RemoveLoadsAsset(Id);

		HashSet<string> fields = Data.Fields.Keys.ToHashSet();
		bool skipped_texture = false;

		foreach (var param in Effect.Parameters) {
			if (param == ParamTotalTime ||
				param == ParamElapsedTime ||
				param == ParamScreenSize ||
				param == ParamViewportPosition ||
				param == ParamTimeOfDay
			)
				continue;

			var cls = param.ParameterClass;
			var type = param.ParameterType;

			if (!skipped_texture && cls == EffectParameterClass.Object && type == EffectParameterType.Texture2D) {
				skipped_texture = true;
				continue;
			}

			if (!Data.Fields.TryGetValue(param.Name, out JToken? value)) {
				ModEntry.Instance.Log($"Missing parameter '{param.Name}' for shader '{Data.Shader}'", StardewModdingAPI.LogLevel.Debug);
				continue;
			}

			fields.Remove(param.Name);

			try {
				if (type == EffectParameterType.Texture1D || type == EffectParameterType.Texture3D || type == EffectParameterType.TextureCube)
					throw new ArgumentException($"unable to load asset for type '{type}'");

				if (type == EffectParameterType.Texture2D) {
					if (cls != EffectParameterClass.Object) {
						Mod.Log($"Parameter '{param.Name}' has type '{type}' but class '{cls}'. We don't know how to handle this.", StardewModdingAPI.LogLevel.Warn);
						continue;
					}

					// Possible values:
					// - string (texture name)

					if (value.Type != JTokenType.String)
						throw new ArgumentException($"unsupported value type '{value.Type}'");

					string? textureName = (string?) value;
					if (string.IsNullOrEmpty(textureName))
						throw new ArgumentNullException(param.Name);

					var tex = Mod.Helper.GameContent.Load<Texture2D>(textureName);
					Mod.MarkLoadsAsset(Id, textureName);

					param.SetValue(tex);
					continue;
				}

				if (cls == EffectParameterClass.Object)
					throw new ArgumentException($"unsupported parameter type");

				// TODO: Figure out a less stupid way to handle this.

				switch (value.Type) {
					case JTokenType.Boolean:
					case JTokenType.Null:
						bool val = value.Type == JTokenType.Boolean && (bool) value;
						switch (type) {
							case EffectParameterType.Bool:
								param.SetValue(val);
								break;
							case EffectParameterType.Int32:
								param.SetValue(val ? 1 : 0);
								break;
							case EffectParameterType.Single:
								param.SetValue(val ? 1.0f : 0f);
								break;
							default:
								throw new ArgumentException($"unsupported value type {value.Type}");
						}
						break;

					case JTokenType.Integer:
						if (type == EffectParameterType.Single)
							param.SetValue((float) value);
						else
							param.SetValue((int) value);
						break;

					case JTokenType.Float:
						param.SetValue((float) value);
						break;

					case JTokenType.String:
						// Maybe a Color?
						if (cls == EffectParameterClass.Vector && type == EffectParameterType.Single) {
							if ((param.ColumnCount == 3 || param.ColumnCount == 4) && CommonHelper.TryParseColor((string?) value, out Color? result)) {
								if (param.ColumnCount == 3)
									param.SetValue(new Vector3(result.Value.R / 255f, result.Value.G / 255f, result.Value.B / 255f));
								else
									param.SetValue(new Vector4(result.Value.R / 255f, result.Value.G / 255f, result.Value.B / 255f, result.Value.A / 255f));

							} else {
								if (param.ColumnCount == 2)
									param.SetValue(value.ToObject<Vector2>());
								else if (param.ColumnCount == 3)
									param.SetValue(value.ToObject<Vector3>());
								else if (param.ColumnCount == 4)
									param.SetValue(value.ToObject<Vector4>());
								else
									throw new ArgumentException("invalid column count for vector");
							}

						} else
							throw new ArgumentException($"unsupported value type '{value.Type}'");

						break;

					case JTokenType.Array:
						if (cls == EffectParameterClass.Scalar && type == EffectParameterType.Single) {
							var array = (JArray) value;
							if (array.Count != param.Elements.Count)
								throw new ArgumentException($"invalid row count for scalar array; expected {param.Elements.Count} but got {array.Count}");

							float[] floats = new float[param.Elements.Count];

							for (int i = 0; i < array.Count; i++)
								floats[i] = (float) array[i];


							param.SetValue(floats);

						} else if (cls == EffectParameterClass.Vector && type == EffectParameterType.Single) {
							var array = (JArray) value;
							if (array.Count != param.ColumnCount)
								throw new ArgumentException($"invalid column count for vector; expected {param.ColumnCount} but got {array.Count}");

							if (param.ColumnCount == 2)
								param.SetValue(new Vector2((float) array[0], (float) array[1]));
							else if (param.ColumnCount == 3)
								param.SetValue(new Vector3((float) array[0], (float) array[1], (float) array[2]));
							else if (param.ColumnCount == 4)
								param.SetValue(new Vector4((float) array[0], (float) array[1], (float) array[2], (float) array[3]));
							else
								throw new ArgumentException("invalid column count for vector");

						} else
							throw new ArgumentException($"unsupported value type '{value.Type}'");

						break;

					case JTokenType.Object:
						if (cls == EffectParameterClass.Vector && type == EffectParameterType.Single) {
							if (param.ColumnCount == 2)
								param.SetValue(value.ToObject<Vector2>());
							else if (param.ColumnCount == 3)
								param.SetValue(value.ToObject<Vector3>());
							else if (param.ColumnCount == 4)
								param.SetValue(value.ToObject<Vector4>());
							else
								throw new ArgumentException("invalid column count for vector");

						} else
							throw new ArgumentException($"unsupported value type '{value.Type}'");

						break;

					default:
						throw new ArgumentException($"Unsupported value type '{value.Type}'");
				}

			} catch (Exception ex) {
				ModEntry.Instance.Log($"Unable to assign '{value}' to parameter '{param.Name}' ({param.ParameterClass}: {param.ParameterType}) of shader '{Data.Shader}': {ex}", StardewModdingAPI.LogLevel.Warn);
			}
		}

		foreach (string field in fields)
			ModEntry.Instance.Log($"Unknown parameter '{field}' not used by shader '{Data.Shader}'", StardewModdingAPI.LogLevel.Warn);

	}

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {
		Draw(time, targetScreen, null, null);
	}

	public void Draw(GameTime time, RenderTarget2D targetScreen, ShaderLayer? prevLayer, ShaderLayer? nextLayer) {
		if (Effect is null || targetScreen is null)
			return;

		// We have two RenderTarget2D
		// First, targetScreen, is the main render target the game world has been drawn to.
		// Second, WorkingTarget, is a secondary target we can use for copying targetScreen.

		// We ALSO may have a previous and next ShaderLayer

		// If we have a previous ShaderLayer, it will have already rendered into our
		// WorkingTarget buffer. If not, we'll need to render targetScreen into our
		// buffer so that it's ready.

		if (prevLayer is null) {
			// Make sure WorkingTarget exists and is the right size.
			CreateOrUpdateBuffer(targetScreen);

			// Draw the render target to WorkingTarget.
			Game1.SetRenderTarget(WorkingTarget);
			Batch.GraphicsDevice.Clear(Game1.bgColor);
			Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
			Batch.Draw(targetScreen, Vector2.Zero, Color.White);
			Batch.End();
		}

		// Alright. Now, update the viewport position before
		// drawing with our effect.
		ParamViewportPosition?.SetValue(new Vector2(Game1.viewport.X, Game1.viewport.Y));

		// Finally, draw with our effect to the next frame buffer.
		// This could be either the next shader layer's buffer, or
		// it could be the main render target.

		nextLayer?.CreateOrUpdateBuffer(targetScreen);
		Game1.SetRenderTarget(nextLayer is null ? targetScreen : nextLayer.WorkingTarget);
		Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: Effect);
		Batch.Draw(WorkingTarget, Vector2.Zero, Data.Color ?? Color.White);
		Batch.End();

		// Don't worry about setting the render target back
		// if this isn't the final shader layer. That one
		// will handle it for us.
	}

	public void MoveWithViewport(int offsetX, int offsetY) {

	}

	public void Resize(Point newSize, Point oldSize) {
		ParamScreenSize?.SetValue(new Vector2(Game1.viewport.Width, Game1.viewport.Height));
	}

	public void Update(GameTime time) {

		ParamTotalTime?.SetValue((float) time.TotalGameTime.TotalSeconds);
		ParamElapsedTime?.SetValue((float) time.ElapsedGameTime.TotalMilliseconds);

		if (ParamTimeOfDay is not null) {
			// TimeOfDay is in fractional hours, so 7:30 would be 7.5
			int minutes = Game1.timeOfDay % 100;
			float adjusted = (Game1.timeOfDay - minutes) / 100
				+ (minutes / 60f)
				+ (Game1.gameTimeInterval / (float) Game1.realMilliSecondsPerGameTenMinutes * 0.166666666666666f);

			ParamTimeOfDay.SetValue(adjusted);
		}

	}
}
