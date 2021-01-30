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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PGraphicsDeviceManager {

		// D3DCREATE_MULTITHREADED
		/*
		[HarmonyPatch(typeof(GraphicsDevice), "CreateDevice", HarmonyPatch.Fixation.Transpile, PriorityLevel.First)]
		internal static IEnumerable<Harmony.CodeInstruction> CreateDeviceTranspiler (IEnumerable<Harmony.CodeInstruction> instructions) {
			return new List<Harmony.CodeInstruction>(instructions);
		}
		*/

		private struct State {
			bool Initialized;
			bool IsFullscreen;
			int Width;
			int Height;

			internal State(bool initialized) {
				Initialized = initialized;
				IsFullscreen = false;
				Width = int.MinValue;
				Height = int.MinValue;
			}

			internal bool Dirty(GraphicsDeviceManager instance) {
				bool isFullscreen = instance.IsFullScreen;
				int width = instance.PreferredBackBufferWidth;
				int height = instance.PreferredBackBufferHeight;

				if (!Initialized || IsFullscreen != isFullscreen || Width != width || Height != height) {
					Initialized = true;
					IsFullscreen = isFullscreen;
					Width = width;
					Height = height;
					return true;
				}
				return false;
			}
		}
		private static State LastState = new State(false);

		[Harmonize("ApplyChanges", HarmonizeAttribute.Fixation.Prefix, PriorityLevel.First)]
		internal static bool OnApplyChanges (GraphicsDeviceManager __instance) {
			var @this = __instance;

			if (!LastState.Dirty(@this)) {
				return false;
			}

			DrawState.UpdateDeviceManager(@this);

			@this.PreferMultiSampling = Config.DrawState.EnableMSAA;
			@this.SynchronizeWithVerticalRetrace = true;
			@this.PreferredBackBufferFormat = Config.DrawState.BackbufferFormat;
			if (Config.DrawState.DisableDepthBuffer)
				@this.PreferredDepthStencilFormat = DepthFormat.None;

			return true;
		}

		private static bool DumpedSystemInfo = false;
		private static WeakReference<GraphicsDevice> LastGraphicsDevice = null;

		[Harmonize("ApplyChanges", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last)]
		internal static void OnApplyChangesPost (GraphicsDeviceManager __instance) {
			var @this = __instance;

			var device = @this.GraphicsDevice;

			if (LastGraphicsDevice == null) {
				LastGraphicsDevice = device.MakeWeak();
			}
			else if (!LastGraphicsDevice.TryGetTarget(out var lastDevice) || lastDevice != device) {
				LastGraphicsDevice.SetTarget(device);
			}
			else {
				return;
			}

			if (!DumpedSystemInfo) {
				try {
					SystemInfo.Dump(__instance, device);
				}
				catch { }
				DumpedSystemInfo = true;
			}

			if (!Runtime.IsWindows) {
				return;
			}

			try {
				static FieldInfo getPrivateField (object obj, string name, bool instance = true) {
					return obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static));
				}

				var capabilitiesProperty = getPrivateField(device, "_profileCapabilities");

				var capabilitiesMember = capabilitiesProperty.GetValue(device);

				var capabilitiesList = new [] {
					getPrivateField(capabilitiesMember, "HiDef", instance: false).GetValue(capabilitiesMember),
					capabilitiesMember
				};

				foreach (var capabilities in capabilitiesList) {
					if (capabilities == null) {
						continue;
					}
					var maxTextureSizeProperty = getPrivateField(capabilities, "MaxTextureSize");
					for (var currentDimension = Config.AbsoluteMaxTextureDimension; currentDimension >= Config.BaseMaxTextureDimension; currentDimension >>= 1) {
						maxTextureSizeProperty.SetValue(capabilities, currentDimension);
						getPrivateField(capabilities, "MaxTextureAspectRatio").SetValue(capabilities, currentDimension / 2);
						try {
							Config.ClampDimension = currentDimension;
							//Math.Min(i, Config.PreferredMaxTextureDimension);
							using (var testTexture = new Texture2D(@this.GraphicsDevice, currentDimension, currentDimension)) {
								/* do nothing. We want to dispose of it immediately. */
							}
							Garbage.Collect(compact: true, blocking: true, background: false);
							break;
						}
						catch {
							Config.ClampDimension = Config.BaseMaxTextureDimension;
							maxTextureSizeProperty.SetValue(capabilities, Config.BaseMaxTextureDimension);
							getPrivateField(capabilities, "MaxTextureAspectRatio").SetValue(capabilities, Config.BaseMaxTextureDimension / 2);
						}
						Garbage.Collect(compact: true, blocking: true, background: false);
					}
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
			}
		}
	}
}
