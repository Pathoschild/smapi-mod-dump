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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster;

static class DrawState {
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

	private static readonly Func<SamplerState, SamplerState> SamplerStateClone =
		typeof(SamplerState).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic)?.CreateDelegate<Func<SamplerState, SamplerState>>() ?? throw new NullReferenceException(nameof(SamplerStateClone));

	private static readonly Func<BlendState, BlendState> BlendStateClone =
		typeof(BlendState).GetMethod("Clone", BindingFlags.Instance | BindingFlags.NonPublic)?.CreateDelegate<Func<BlendState, BlendState>>() ?? throw new NullReferenceException(nameof(SamplerStateClone));

	private static class Defaults {
		internal static readonly SamplerState SamplerState = SamplerStateClone(SamplerState.LinearClamp);
		internal static readonly BlendState BlendState = BlendStateClone(BlendState.AlphaBlend);
		internal const SpriteSortMode SortMode = SpriteSortMode.Deferred;
	}

	internal static SamplerState CurrentSamplerState = Defaults.SamplerState;
	internal static BlendState CurrentBlendState = Defaults.BlendState;
	internal static SpriteSortMode CurrentSortMode = Defaults.SortMode;

	internal static readonly Condition TriggerGC = new(false);

	internal static TimeSpan ExpectedFrameTime { get; private set; } = new(166_667); // default 60hz
	internal static bool ForceSynchronous = false;

	private static System.Diagnostics.Stopwatch FrameStopwatch = System.Diagnostics.Stopwatch.StartNew();

	private const int BaselineFrameTimeRunningCount = 20;
	private static TimeSpan BaselineFrameTime = TimeSpan.Zero;

	internal static void UpdateDeviceManager(GraphicsDeviceManager manager) {
		var rate = (TimeSpan?)manager?.GetField("game")?.GetProperty("TargetElapsedTime");
		ExpectedFrameTime = rate.GetValueOrDefault(ExpectedFrameTime);
	}

	internal static GraphicsDevice Device => Game1.graphics.GraphicsDevice;

	internal static bool PushedUpdateWithin(int frames) => (long)((ulong)CurrentFrame - LastPushedUpdateFrame) <= frames;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static TimeSpan RemainingFrameTime(float multiplier = 1.0f, in TimeSpan? offset = null) {
		var actualRemainingTime = ActualRemainingFrameTime();
		return (actualRemainingTime - (BaselineFrameTime + (offset ?? TimeSpan.Zero))).Multiply(multiplier);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static TimeSpan ActualRemainingFrameTime() => ExpectedFrameTime - FrameStopwatch.Elapsed;

	//static bool testOnce = true;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnPresent() {
		Thread.CurrentThread.Priority = ThreadPriority.Highest;
		if (TriggerGC) {
			ManagedSpriteInstance.PurgeTextures((Config.Garbage.RequiredFreeMemory * Config.Garbage.RequiredFreeMemoryHysterisis).NearestLong() * 1024 * 1024);
			//Garbage.Collect();
			Garbage.Collect(compact: true, blocking: true, background: false);

			TriggerGC.Set(false);
		}

		if (Config.AsyncScaling.CanFetchAndLoadSameFrame || !PushedUpdateThisFrame) {
			var remaining = ActualRemainingFrameTime();
			SynchronizedTaskScheduler.Instance.Dispatch(remaining);
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnPresentPost() {
		FrameStopwatch.Restart();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static SamplerState ConditionallyClone(SamplerState value, SamplerState defaultValue) => (value is null) ? defaultValue : SamplerStateClone(value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static BlendState ConditionallyClone(BlendState value, BlendState defaultValue) => (value is null) ? defaultValue : BlendStateClone(value);

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
		CurrentSortMode = sortMode;
		CurrentSamplerState = ConditionallyClone(samplerState, Defaults.SamplerState);
		CurrentBlendState = ConditionallyClone(blendState, Defaults.BlendState);

		var device = @this.GraphicsDevice;
		var renderTargets = device.GetRenderTargets();
		var renderTarget = (renderTargets.Length != 0) ? renderTargets[0].RenderTarget : null;

		//if (renderTarget is RenderTarget2D target && target.RenderTargetUsage != RenderTargetUsage.DiscardContents) {
		//	Debug.WarningLn("Non-Discarding RTT");
		//}

		ForceSynchronous = renderTarget switch {
			null => false,
			_ => true
		};
	}
}
