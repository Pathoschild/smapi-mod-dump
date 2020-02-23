using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

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
		internal static Volatile<ulong> CurrentFrame = 0UL;
		internal static TextureAddressMode CurrentAddressModeU = DefaultSamplerState.AddressU;
		internal static TextureAddressMode CurrentAddressModeV = DefaultSamplerState.AddressV;
		internal static Blend CurrentBlendSourceMode = BlendState.AlphaBlend.AlphaSourceBlend;
		internal static volatile bool TriggerGC = false;
		internal static SpriteSortMode CurrentSortMode = SpriteSortMode.Deferred;
		internal static TimeSpan ExpectedFrameTime { get; private set; } = new TimeSpan(166_667); // default 60hz
		internal static bool ForceSynchronous = false;

		private static DateTime FrameStartTime = DateTime.Now;
		private static TimeSpan BaselineFrameTime = TimeSpan.Zero;

		internal static void UpdateDeviceManager(GraphicsDeviceManager manager) {
			var rate = (TimeSpan?)manager?.GetField("game")?.GetProperty("TargetElapsedTime");
			ExpectedFrameTime = rate.GetValueOrDefault(ExpectedFrameTime);
		}

		internal static GraphicsDevice Device {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return Game1.graphics.GraphicsDevice;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetCurrentAddressMode (SamplerState samplerState) {
			CurrentAddressModeU = samplerState.AddressU;
			CurrentAddressModeV = samplerState.AddressV;
		}

		internal static bool PushedUpdateWithin(int frames) {
			return (long)((ulong)CurrentFrame - LastPushedUpdateFrame) <= frames;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static TimeSpan RemainingFrameTime(float multiplier = 1.0f, TimeSpan? offset = null) {
			return (ActualRemainingFrameTime() - (BaselineFrameTime + (offset ?? TimeSpan.Zero))).Multiply(multiplier);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TimeSpan ActualRemainingFrameTime () {
			return ExpectedFrameTime - (DateTime.Now - FrameStartTime);
		}

		//static bool testOnce = true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				ScaledTexture.PurgeTextures((Config.RequiredFreeMemory * Config.RequiredFreeMemoryHysterisis).NearestLong() * 1024 * 1024);
				//Garbage.Collect();
				Garbage.Collect(compact: true, blocking: true, background: false);

				TriggerGC = false;
			}

			if (Config.AsyncScaling.CanFetchAndLoadSameFrame || !PushedUpdateThisFrame) {
				var remaining = ActualRemainingFrameTime();
				if (remaining < TimeSpan.Zero) {
					Debug.TraceLn($"Over Time: {-remaining.TotalMilliseconds}");
				}
				SynchronizedTasks.ProcessPendingActions(remaining);
			}

			if (!PushedUpdateThisFrame) {
				var duration = DateTime.Now - FrameStartTime;
				// Throw out garbage values.
				if (duration <= (ExpectedFrameTime + ExpectedFrameTime)) {
					BaselineFrameTime = (BaselineFrameTime + duration).Halve();
				}
			}
			else {
				PushedUpdateThisFrame = false;
			}
			++CurrentFrame;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OnPresentPost() {
			FrameStartTime = DateTime.Now;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
