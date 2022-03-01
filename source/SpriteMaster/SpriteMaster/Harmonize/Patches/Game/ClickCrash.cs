/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Harmonize.Patches.Game;

static class ClickCrash {
	private static bool HasWindow => StardewValley.Game1.game1.Window != null;

	private static readonly Stopwatch SdlUpdate = Stopwatch.StartNew();
	private static readonly Thread SdlUpdateThread;
	private static readonly Action? SdlLoopMethod;
	//private static Texture2D? FishTexture;

	static ClickCrash() {
		try {
			var platformGetter = typeof(XNA.Game).GetFieldGetter<object, object>("Platform") ?? throw new NullReferenceException("PlatformGetter");
			var platform = platformGetter(StardewValley.GameRunner.instance) ?? throw new NullReferenceException("Platform");
			var sdlGamePlatformType =
				typeof(XNA.Color).Assembly.
				GetType("Microsoft.Xna.Framework.SdlGamePlatform");
			var sdlLoopMethodInfo =
				sdlGamePlatformType?.
				GetMethod("SdlRunLoop", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			SdlLoopMethod = sdlLoopMethodInfo?.CreateDelegate<Action>(platform) ?? throw new NullReferenceException(nameof(SdlLoopMethod));
			SdlUpdateThread = ThreadExt.Run(SdlUpdateLoop, background: true, name: "Click-Crash Thread");
		}
		catch (Exception ex) {
			Debug.Error("Failed to configure SDL ticker", ex);
		}
		SdlUpdateThread = null!;
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static void Initialize() {
		// Does nothing, just basically sets up the static constructor;
		//FishTexture = SpriteMaster.Self.Helper.Content.Load<Texture2D>("LooseSprites\\AquariumFish", StardewModdingAPI.ContentSource.GameContent);
	}

	[Harmonize(
		typeof(XNA.Color),
		"Microsoft.Xna.Framework.SdlGamePlatform",
		"SdlRunLoop",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static void SdlRunLoop(object __instance) {
		SdlUpdate.Restart();
	}

	[Harmonize(
		typeof(XNA.Color),
		"Microsoft.Xna.Framework.Threading",
		"EnsureUIThread",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool EnsureUIThread() => false;

	private static volatile bool UIThreadOverride = false;

	[Harmonize(
		typeof(XNA.Color),
		"Microsoft.Xna.Framework.Threading",
		"IsOnUIThread",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool IsOnUIThread(ref bool __result) {
		if (UIThreadOverride) {
			__result = true;
			return false;
		}
		return true;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static void SdlUpdateLoop() {
		if (SdlLoopMethod is null) {
			return;
		}

		while (true) {
			if (!Config.Enabled || !Config.Extras.PreventUnresponsive) {
				Thread.Sleep(10_000);
				continue;
			}

			if (!HasWindow || !SpriteMaster.Self.IsGameLoaded) {
				Thread.Sleep(1_000);
				continue;
			}

			var elapsed = SdlUpdate.ElapsedMilliseconds;
			if (elapsed <= 200) {
				Thread.Sleep(100);
				continue;
			}

			//Debug.Info($"Game has not triggered 'SdlUpdateLoop' for {elapsed} ms, triggering asynchronously (loading: {GameState.IsLoading})");
			
			SdlLoopMethod();

			/*
			if (GameState.IsLoading && FishTexture is not null) {
				var device = StardewValley.Game1.game1.GraphicsDevice;
				try {
					UIThreadOverride = true;
					Game1.graphics.BeginDraw();
					device.SetRenderTarget(null);
					device.Clear(ClearOptions.Target | ClearOptions.Stencil | ClearOptions.DepthBuffer, new XNA.Color(5, 3, 4), 1.0f, 0);
					var b = StardewValley.Game1.spriteBatch;
					b.Begin(XNA.Graphics.SpriteSortMode.Immediate, XNA.Graphics.BlendState.Opaque, XNA.Graphics.SamplerState.PointClamp);
					Game1.spriteBatch.Draw(
						FishTexture,
						Game1.viewport.ToXna(),
						new XNA.Rectangle(0, 0, 24, 24),
						XNA.Color.White,
						0.0f,
						XNA.Vector2.Zero,
						SpriteEffects.None,
						0.0f
					);
					b.End();
					Game1.graphics.EndDraw();
				}
				catch (Exception ex) {
					ex = ex;
				}
				finally {
					UIThreadOverride = false;
				}
			}
			*/
		}
	}
}
