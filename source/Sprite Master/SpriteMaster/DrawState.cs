/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster {
	internal static class DrawState {
		private static readonly SamplerState DefaultSamplerState = SamplerState.LinearClamp;
		private static bool _PushedUpdateThisFrame = false;
		internal static bool PushedUpdateThisFrame {
			get {
				return _PushedUpdateThisFrame;
			}
			set {
				if (value) {
					LastPushedUpdateFrame = CurrentFrame;
				}
				_PushedUpdateThisFrame = value;
			}
		}
		internal static ulong LastPushedUpdateFrame = 0UL;
		internal static VolatileULong CurrentFrame = 0UL;
		internal static TextureAddressMode CurrentAddressModeU = DefaultSamplerState.AddressU;
		internal static TextureAddressMode CurrentAddressModeV = DefaultSamplerState.AddressV;
		internal static Blend CurrentBlendSourceMode = BlendState.AlphaBlend.AlphaSourceBlend;

		internal static readonly Condition TriggerGC = new(false);

		internal static SpriteSortMode CurrentSortMode = SpriteSortMode.Deferred;
		internal static TimeSpan ExpectedFrameTime { get; private set; } = new(166_667); // default 60hz
		internal static bool ForceSynchronous = false;

		private static System.Diagnostics.Stopwatch FrameStopwatch = System.Diagnostics.Stopwatch.StartNew();

		private const int BaselineFrameTimeRunningCount = 20;
		private static TimeSpan BaselineFrameTime = TimeSpan.Zero;

		internal static void UpdateDeviceManager(GraphicsDeviceManager manager) {
			var rate = (TimeSpan?)manager?.GetField("game")?.GetProperty("TargetElapsedTime");
			ExpectedFrameTime = rate.GetValueOrDefault(ExpectedFrameTime);
		}

		internal static GraphicsDevice Device {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			get {
				return Game1.graphics.GraphicsDevice;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void SetCurrentAddressMode (SamplerState samplerState) {
			CurrentAddressModeU = samplerState.AddressU;
			CurrentAddressModeV = samplerState.AddressV;
		}

		internal static bool PushedUpdateWithin(int frames) {
			return (long)((ulong)CurrentFrame - LastPushedUpdateFrame) <= frames;
		}


		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static TimeSpan RemainingFrameTime(float multiplier = 1.0f, TimeSpan? offset = null) {
			var actualRemainingTime = ActualRemainingFrameTime();
			return (actualRemainingTime - (BaselineFrameTime + (offset ?? TimeSpan.Zero))).Multiply(multiplier);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private static TimeSpan ActualRemainingFrameTime () {
			return ExpectedFrameTime - FrameStopwatch.Elapsed;
		}

		//static bool testOnce = true;


		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void OnPresent () {
			/*
			if (testOnce) {
				Texture2D testTexture;
				using (var inFile = File.Open("D:\\test.png", FileMode.Open)) {
					testTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, inFile);
				}
				var testData = new byte[testTexture.Width * testTexture.Height * sizeof(int)];
				testTexture.GetData(testData);

				var format = TextureFormat.Color;
				testData = BlockCompress.Compress(testData, ref format, new Vector2I(testTexture.Width, testTexture.Height), false, true, true, true, true, true);

				var outData = BlockCompress.Decompress(testData, (uint)testTexture.Width, (uint)testTexture.Height, format);

				var outTexture = new Texture2D(Game1.graphics.GraphicsDevice, testTexture.Width, testTexture.Height, false, SurfaceFormat.Color);
				outTexture.SetData(outData);
				using (var outFile = File.Open("D:\\test_out.png", FileMode.Create)) {
					outTexture.SaveAsPng(outFile, outTexture.Width, outTexture.Height);
				}
				testOnce = false;
			}
			*/

			if (TriggerGC) {
				ScaledTexture.PurgeTextures((Config.Garbage.RequiredFreeMemory * Config.Garbage.RequiredFreeMemoryHysterisis).NearestLong() * 1024 * 1024);
				//Garbage.Collect();
				Garbage.Collect(compact: true, blocking: true, background: false);

				TriggerGC.Set(false);
			}

			if (Config.AsyncScaling.CanFetchAndLoadSameFrame || !PushedUpdateThisFrame) {
				var remaining = ActualRemainingFrameTime();
				SynchronizedTasks.ProcessPendingActions(remaining);
			}

			if (!PushedUpdateThisFrame) {
				var duration = FrameStopwatch.Elapsed;
				// Throw out garbage values.
				if (duration <= (ExpectedFrameTime + ExpectedFrameTime)) {
					var mean = BaselineFrameTime;
					mean -= mean / BaselineFrameTimeRunningCount;
					mean += duration / BaselineFrameTimeRunningCount;
					BaselineFrameTime = mean;

					// TODO : fix me, this doesn't work particularly well so I've disabled it.
					BaselineFrameTime = TimeSpan.Zero;
				}
			}
			else {
				PushedUpdateThisFrame = false;
			}
			++CurrentFrame;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void OnPresentPost() {
			FrameStopwatch.Restart();
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static void OnBegin (
			SpriteBatch @this,
			SpriteSortMode sortMode,
			BlendState blendState,
			SamplerState samplerState,
			DepthStencilState depthStencilState,
			RasterizerState rasterizerState,
			Effect effect,
			Matrix transformMatrix
		) {
			CurrentSortMode = sortMode;
			SetCurrentAddressMode(samplerState ?? SamplerState.PointClamp);
			CurrentBlendSourceMode = (blendState ?? BlendState.AlphaBlend).AlphaSourceBlend;

			var device = @this.GraphicsDevice;
			var renderTargets = device.GetRenderTargets();
			var renderTarget = renderTargets.Any() ? renderTargets[0].RenderTarget : null;

			//if (renderTarget is RenderTarget2D target && target.RenderTargetUsage != RenderTargetUsage.DiscardContents) {
			//	Debug.WarningLn("Non-Discarding RTT");
			//}

			ForceSynchronous = renderTarget switch {
				null => false,
				_ => true
			};
		}
	}
}
