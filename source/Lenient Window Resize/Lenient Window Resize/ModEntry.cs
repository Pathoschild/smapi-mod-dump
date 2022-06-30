/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-lenient-window-resize
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using StardewValley.Menus;

namespace Lenient_Window_Resize
{
    /// <summary>The mod entry point.</summary>

    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            ObjectPatches.Initialize(this.Monitor);

            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.SetWindowSize)),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.SetWindowSize_Prefix))
            );

        }

    }

    public class ObjectPatches
    {
        private static IMonitor Monitor;
		private static int minW = 640;
		private static int minH = 360;
		public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool SetWindowSize_Prefix(Game1 __instance, int w, int h)
        {
            try
            {
				SetWindowSize(w, h, __instance, __instance);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(SetWindowSize_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    
	//Sadly I can't access the minimum W and H from the outside, as they're hard coded, so I copied the entire functionality and replaced the minW and minH
	//Not a clean code solution, but it works, so I won't complain
	public static void SetWindowSize(int w, int h, InstanceGame _base, Game1 _instance)
	{
		Rectangle oldWindow = new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
		_ = _base.Window.ClientBounds;
		bool recalculateClientBounds = false;
		try
		{
			if (w < minW && !Game1.graphics.IsFullScreen)
			{
				Game1.graphics.PreferredBackBufferWidth = minW;
				w = minW;
				recalculateClientBounds = true;
			}
			if (h < minH && !Game1.graphics.IsFullScreen)
			{
				Game1.graphics.PreferredBackBufferHeight = minH;
				h = minH;
				recalculateClientBounds = true;
			}
		}
		catch (Exception)
		{
			Game1.graphics.PreferredBackBufferWidth = minW;
			Game1.graphics.PreferredBackBufferHeight = minH;
			w = Game1.graphics.PreferredBackBufferWidth;
			h = Game1.graphics.PreferredBackBufferHeight;
		}
		if (recalculateClientBounds)
		{
			_ = _base.Window.ClientBounds;
		}
		if (_base.IsMainInstance && Game1.graphics.SynchronizeWithVerticalRetrace != Game1.options.vsyncEnabled)
		{
			Game1.graphics.SynchronizeWithVerticalRetrace = Game1.options.vsyncEnabled;
			Console.WriteLine("Vsync toggled: " + Game1.graphics.SynchronizeWithVerticalRetrace.ToString());
		}
		Game1.graphics.ApplyChanges();
		try
		{
			if (Game1.graphics.IsFullScreen)
			{
				_instance.localMultiplayerWindow = new Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			}
			else
			{
				_instance.localMultiplayerWindow = new Rectangle(0, 0, w, h);
			}
		}
		catch (Exception)
		{
		}
		Game1.defaultDeviceViewport = new Viewport(_instance.localMultiplayerWindow);
		List<Vector4> screen_splits = new List<Vector4>();
		if (GameRunner.instance.gameInstances.Count <= 1)
		{
			screen_splits.Add(new Vector4(0f, 0f, 1f, 1f));
		}
		else if (GameRunner.instance.gameInstances.Count == 2)
		{
			screen_splits.Add(new Vector4(0f, 0f, 0.5f, 1f));
			screen_splits.Add(new Vector4(0.5f, 0f, 0.5f, 1f));
		}
		else if (GameRunner.instance.gameInstances.Count == 3)
		{
			screen_splits.Add(new Vector4(0f, 0f, 1f, 0.5f));
			screen_splits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
			screen_splits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
		}
		else if (GameRunner.instance.gameInstances.Count == 4)
		{
			screen_splits.Add(new Vector4(0f, 0f, 0.5f, 0.5f));
			screen_splits.Add(new Vector4(0.5f, 0f, 0.5f, 0.5f));
			screen_splits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
			screen_splits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
		}
		if (GameRunner.instance.gameInstances.Count <= 1)
		{
			_instance.zoomModifier = 1f;
		}
		else
		{
			_instance.zoomModifier = 0.5f;
		}
		Vector4 current_screen_split = screen_splits[Game1.game1.instanceIndex];
		Vector2? old_ui_dimensions = null;
		if (_instance.uiScreen != null)
		{
			old_ui_dimensions = new Vector2(_instance.uiScreen.Width, _instance.uiScreen.Height);
		}
		_instance.localMultiplayerWindow.X = (int)((float)w * current_screen_split.X);
		_instance.localMultiplayerWindow.Y = (int)((float)h * current_screen_split.Y);
		_instance.localMultiplayerWindow.Width = (int)Math.Ceiling((float)w * current_screen_split.Z);
		_instance.localMultiplayerWindow.Height = (int)Math.Ceiling((float)h * current_screen_split.W);
		try
		{
			int sw = Math.Min(GameRunner.MaxTextureSize, (int)Math.Ceiling((float)_instance.localMultiplayerWindow.Width * (1f / Game1.options.zoomLevel)));
			int sh = Math.Min(GameRunner.MaxTextureSize, (int)Math.Ceiling((float)_instance.localMultiplayerWindow.Height * (1f / Game1.options.zoomLevel)));
			_instance.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, sw, sh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			_instance.screen.Name = "Screen";
			int uw = Math.Min(GameRunner.MaxTextureSize, (int)Math.Ceiling((float)_instance.localMultiplayerWindow.Width / Game1.options.uiScale));
			int uh = Math.Min(GameRunner.MaxTextureSize, (int)Math.Ceiling((float)_instance.localMultiplayerWindow.Height / Game1.options.uiScale));
			_instance.uiScreen = new RenderTarget2D(Game1.graphics.GraphicsDevice, uw, uh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			_instance.uiScreen.Name = "UI Screen";
		}
		catch (Exception)
		{
		}
		Game1.updateViewportForScreenSizeChange(fullscreenChange: false, _instance.localMultiplayerWindow.Width, _instance.localMultiplayerWindow.Height);
		if (old_ui_dimensions.HasValue && old_ui_dimensions.Value.X == (float)_instance.uiScreen.Width && old_ui_dimensions.Value.Y == (float)_instance.uiScreen.Height)
		{
			return;
		}
		Game1.PushUIMode();
		if (Game1.textEntry != null)
		{
			Game1.textEntry.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
		}
		foreach (IClickableMenu onScreenMenu in Game1.onScreenMenus)
		{
			onScreenMenu.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
		}
		if (Game1.currentMinigame != null)
		{
			Game1.currentMinigame.changeScreenSize();
		}
		if (Game1.activeClickableMenu != null)
		{
			Game1.activeClickableMenu.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
		}
		if (Game1.activeClickableMenu is GameMenu && !Game1.overrideGameMenuReset)
		{
			if ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() is OptionsPage)
			{
				((Game1.activeClickableMenu as GameMenu).GetCurrentPage() as OptionsPage).preWindowSizeChange();
			}
			Game1.activeClickableMenu = new GameMenu((Game1.activeClickableMenu as GameMenu).currentTab);
			if ((Game1.activeClickableMenu as GameMenu).GetCurrentPage() is OptionsPage)
			{
				((Game1.activeClickableMenu as GameMenu).GetCurrentPage() as OptionsPage).postWindowSizeChange();
			}
		}
		Game1.PopUIMode();
	}
	}
}
