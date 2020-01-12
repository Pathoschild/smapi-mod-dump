using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using StardewValley;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SpriteMaster {
	internal static class DrawState {
		private static readonly SamplerState DefaultSamplerState = SamplerState.LinearClamp;
		private static bool FetchedThisFrame = false;
		private static long RemainingTexelFetchBudget = Config.AsyncScaling.ScalingBudgetPerFrameTexels;
		private static bool PushedUpdateThisFrame = false;
		public static long CurrentFrame = 0;
		public static TextureAddressMode CurrentAddressModeU = DefaultSamplerState.AddressU;
		public static TextureAddressMode CurrentAddressModeV = DefaultSamplerState.AddressV;
		public static Blend CurrentBlendSourceMode = BlendState.AlphaBlend.AlphaSourceBlend;
		public static volatile bool TriggerGC = false;
		public static SpriteSortMode CurrentSortMode = SpriteSortMode.Deferred;

		internal static GraphicsDevice Device {
			get {
				return Game1.graphics.GraphicsDevice;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetCurrentAddressMode (SamplerState samplerState) {
			CurrentAddressModeU = samplerState.AddressU;
			CurrentAddressModeV = samplerState.AddressV;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetUpdateToken (int texels) {
			if (FetchedThisFrame && texels > RemainingTexelFetchBudget) {
				return false;
			}

			FetchedThisFrame = true;
			RemainingTexelFetchBudget -= texels;
			return true;
		}

		private static bool IsMemoryPressured() {
			try {
				var RequiredMemory = (Config.RequiredFreeMemory * 1.5).NearestInt();
				using var _ = new MemoryFailPoint(RequiredMemory);
				return false;
			}
			catch (Exception ex) when (ex is InsufficientMemoryException) {
				return true;
			}
			catch (Exception ex) {
				// I'm not sure how we'd get here.
				ex.PrintWarning();
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OnPresent () {
			if (TriggerGC) {
				ScaledTexture.PurgeTextures((Config.RequiredFreeMemory * Config.RequiredFreeMemoryHysterisis).NearestLong() * 1024 * 1024);
				//Garbage.Collect();
				Garbage.Collect(compact: true, blocking: true, background: false);

				TriggerGC = false;
			}
			
			if (Config.AsyncScaling.CanFetchAndLoadSameFrame || !PushedUpdateThisFrame) {
				ScaledTexture.ProcessPendingActions();
			}
			RemainingTexelFetchBudget = Config.AsyncScaling.ScalingBudgetPerFrameTexels;
			FetchedThisFrame = false;
			PushedUpdateThisFrame = false;
			++CurrentFrame;
		}

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
		}
	}
}
