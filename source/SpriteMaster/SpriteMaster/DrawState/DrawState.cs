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
using SpriteMaster.Tasking;
using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster;

static partial class DrawState {
	private static readonly SamplerState DefaultSamplerState = SamplerState.LinearClamp;

	internal static readonly InterlockedULong LastPushedUpdateFrame = 0UL;

	private static volatile bool _PushedUpdateThisFrame = false;
	internal static bool PushedUpdateThisFrame {
		get {
			return _PushedUpdateThisFrame;
		}
		set {
			if (value) {
				LastPushedUpdateFrame.Set(CurrentFrame);
			}
			_PushedUpdateThisFrame = value;
		}
	}

	internal static InterlockedULong CurrentFrame = 0UL;

	private static class Defaults {
		internal static readonly SamplerState SamplerState = SamplerState.LinearClamp;
		internal static readonly BlendState BlendState = BlendState.AlphaBlend;
		internal static readonly RasterizerState RasterizerState = RasterizerState.CullCounterClockwise;
		internal const SpriteSortMode SortMode = SpriteSortMode.Deferred;
	}

	internal static readonly Lazy<SamplerState> LinearBorder = new(() => {
		var state = SamplerStateClone!(SamplerState.LinearClamp);
		state.AddressU = state.AddressV = TextureAddressMode.Border;
		return state;
	});
	internal static readonly Lazy<SamplerState> LinearMirror = new(() => {
		var state = SamplerStateClone!(SamplerState.LinearClamp);
		state.AddressU = state.AddressV = TextureAddressMode.Mirror;
		return state;
	});

	internal static SamplerState CurrentSamplerState = Defaults.SamplerState;
	internal static BlendState CurrentBlendState = Defaults.BlendState;
	internal static RasterizerState CurrentRasterizerState = Defaults.RasterizerState;
	internal static SpriteSortMode CurrentSortMode = Defaults.SortMode;

	internal static readonly Condition TriggerGC = new(false);

	private static TimeSpan ExpectedFrameTime = new(166_667); // default 60hz
	internal static bool ForceSynchronous = false;

	private static System.Diagnostics.Stopwatch FrameStopwatch = System.Diagnostics.Stopwatch.StartNew();

	private const int BaselineFrameTimeRunningCount = 20;
	private static TimeSpan BaselineFrameTime = TimeSpan.Zero;

	internal static void UpdateDeviceManager(GraphicsDeviceManager manager) {
		var rate = (TimeSpan?)manager?.GetField("game")?.GetProperty("TargetElapsedTime");
		ExpectedFrameTime = rate.GetValueOrDefault(ExpectedFrameTime);
	}

	internal static GraphicsDevice Device => Game1.graphics.GraphicsDevice;

	internal static bool PushedUpdateWithin(int frames) => (long)(CurrentFrame - LastPushedUpdateFrame) <= frames;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static TimeSpan RemainingFrameTime(float multiplier = 1.0f, in TimeSpan? offset = null) {
		var actualRemainingTime = ActualRemainingFrameTime();
		return (actualRemainingTime - (BaselineFrameTime + (offset ?? TimeSpan.Zero))).Multiply(multiplier);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static TimeSpan ActualRemainingFrameTime() => ExpectedFrameTime - FrameStopwatch.Elapsed;


	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnPresent() {
		Thread.CurrentThread.Priority = ThreadPriority.Highest;

		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		if (TriggerGC.GetAndClear()) {
			ManagedSpriteInstance.PurgeTextures((Config.Garbage.RequiredFreeMemory * Config.Garbage.RequiredFreeMemoryHysterisis).NearestLong() * 1024 * 1024);
			Garbage.Collect(compact: true, blocking: true, background: false);
		}

		if (Config.AsyncScaling.CanFetchAndLoadSameFrame || !PushedUpdateThisFrame) {
			var remaining = ActualRemainingFrameTime();
			SynchronizedTaskScheduler.Instance.Dispatch(remaining);
		}

		if (!PushedUpdateThisFrame) {
			var duration = FrameStopwatch.Elapsed;
			// Throw out garbage values.
			if (duration <= ExpectedFrameTime + ExpectedFrameTime) {
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

		TransientGC(++CurrentFrame);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void TransientGC(ulong currentFrame) {
		var tickCount = Config.Performance.TransientGCTickCount;
		if (tickCount > 0 && (currentFrame % 150) == 0) {
			// No trace message as that would be _incredibly_ annoying.
			GC.Collect(
				generation: 1,
				mode: GCCollectionMode.Forced,
				blocking: false,
				compacting: false
			);
		}
	}

	private static WeakReference<xTile.Display.IDisplayDevice> LastMitigatedDevice = new(null!);
	private static void ApplyPyTKMitigation() {
		if (!Config.Extras.ModPatches.DisablePyTKMitigation) {
			return;
		}
		if (LastMitigatedDevice.TryGetTarget(out var lastDevice) && lastDevice == Game1.mapDisplayDevice) {
			return;
		}
		if (Game1.mapDisplayDevice is not null && Game1.mapDisplayDevice.GetType().Name.Contains("PyDisplayDevice")) {
			var adjustOriginField = Game1.mapDisplayDevice.GetType().GetField("adjustOrigin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (adjustOriginField is not null) {
				adjustOriginField.SetValue(Game1.mapDisplayDevice, false);
			}

			LastMitigatedDevice.SetTarget(Game1.mapDisplayDevice);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnPresentPost() {
		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		// Apply the PyTK mediation here because we do not know when it might be set up
		ApplyPyTKMitigation();
		
		FrameStopwatch.Restart();
	}

	private static bool FirstDraw = true;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnBegin(
		SpriteBatch @this,
		SpriteSortMode sortMode,
		BlendState blendState,
		SamplerState samplerState,
		DepthStencilState depthStencilState,
		RasterizerState rasterizerState,
		Effect effect,
		in Matrix transformMatrix
	) {
		using var watchdogScoped = WatchDog.WatchDog.ScopedWorkingState;

		if (FirstDraw) {
			SpriteMaster.Self.OnFirstDraw();
			FirstDraw = false;
		}

		CurrentSortMode = sortMode;
		CurrentSamplerState = samplerState ?? Defaults.SamplerState;// ConditionallyClone(samplerState, Defaults.SamplerState);
		CurrentBlendState = blendState ?? Defaults.BlendState; // ConditionallyClone(blendState, Defaults.BlendState);
		CurrentRasterizerState = rasterizerState ?? Defaults.RasterizerState;

		CheckStates();

		var device = @this.GraphicsDevice;
		var renderTargets = device.GetRenderTargets();
		var renderTarget = renderTargets.Length != 0 ? renderTargets[0].RenderTarget : null;

		//if (renderTarget is RenderTarget2D target && target.RenderTargetUsage != RenderTargetUsage.DiscardContents) {
		//	Debug.Warning("Non-Discarding RTT");
		//}

		// If we're drawing to the system target or to the game's front buffer or ui buffer, we do not want to be in synchronized mode.
		// Otherwise, we _do_ want to be, because it might be a mod drawing to a render target, and we need to make sure that said draw
		// actually goes through, processed!
		// TODO : though it might make more sense to do the _reverse_ for that - don't resample the draws to the render target, only resample
		// draws _from_ the render target
		// We intentionally synchronize all other game targets as well, such as the lightmap, as those are not constantly updated
		if (renderTarget is null) {
			ForceSynchronous = false;
		}
		else if (renderTarget == Game1.game1.uiScreen || renderTarget == Game1.game1.screen) {
			ForceSynchronous = false;
		}
		else {
			ForceSynchronous = true;
		}
	}
}
