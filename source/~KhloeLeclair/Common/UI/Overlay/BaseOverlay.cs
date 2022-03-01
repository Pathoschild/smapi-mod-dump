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
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI.Overlay
{
    public class BaseOverlay<T> : IOverlay, IDisposable where T : IClickableMenu {

		protected readonly Mod Mod;
		protected IModEvents Events => Mod.Helper.Events;
		protected IInputHelper InputHelper => Mod.Helper.Input;

		protected readonly T Menu;
		protected bool? AssumeUIMode;

		private bool hasOtherKeys;
		private readonly int ScreenID;
		private Rectangle LastViewport;

		#region Lifecycle

		protected BaseOverlay(T menu, Mod mod, bool? assumeUI = null) {
			Menu = menu;
			Mod = mod;
			AssumeUIMode = assumeUI;

			ScreenID = Context.ScreenId;
			LastViewport = Game1.uiViewport.ToXna();

			RegisterEvents();
		}

		public virtual void Dispose() {
			UnregisterEvents();
		}

		#endregion

		#region Events

		// Drawing

		protected virtual void PreDrawUI(SpriteBatch batch) { }

		protected virtual void DrawUI(SpriteBatch batch) { }

		protected virtual void DrawWorld(SpriteBatch batch) { }

		// Mouse Events

		protected virtual bool ReceiveLeftClick(int x, int y) {
			return false;
		}

		protected virtual bool ReceiveScrollWheelAction(int amount) {
			return false;
		}

		protected virtual bool ReceiveRightClick(int x, int y) {
			return false;
		}

		protected virtual bool ReceiveCursorHover(int x, int y) {
			return false;
		}

		// Other Events

		protected virtual void HandleUpdateTicked() { }

		protected virtual void ReceiveButtonsChanged(object sender, ButtonsChangedEventArgs e) { }

		protected virtual bool ReceiveButtonPressed(object sender, ButtonPressedEventArgs e) { return false; }

		protected virtual void ReceiveGameWindowResized(Rectangle NewViewport) { }

		#endregion

		#region Event Registration

		protected virtual void RegisterEvents() {
			Events.GameLoop.UpdateTicked += OnUpdateTicked;

			hasOtherKeys = IsMethodOverridden(nameof(ReceiveButtonPressed));

			if (IsMethodOverridden(nameof(PreDrawUI)))
				Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;

			if (IsMethodOverridden(nameof(DrawUI)))
				Events.Display.Rendered += OnRendered;

			if (IsMethodOverridden(nameof(DrawWorld)))
				Events.Display.RenderedWorld += OnRenderedWorld;

			if (hasOtherKeys || IsMethodOverridden(nameof(ReceiveLeftClick)) || IsMethodOverridden(nameof(ReceiveRightClick)))
				Events.Input.ButtonPressed += OnButtonPressed;

			if (IsMethodOverridden(nameof(ReceiveButtonsChanged)))
				Events.Input.ButtonsChanged += OnButtonsChanged;

			if (IsMethodOverridden(nameof(ReceiveCursorHover)))
				Events.Input.CursorMoved += OnCursorMoved;

			if (IsMethodOverridden(nameof(ReceiveScrollWheelAction)))
				Events.Input.MouseWheelScrolled += OnMouseWheelScrolled;
		}

		protected virtual void UnregisterEvents() {
			Events.GameLoop.UpdateTicked -= OnUpdateTicked;

			Events.Display.RenderingActiveMenu -= OnRenderingActiveMenu;
			Events.Display.Rendered -= OnRendered;
			Events.Display.RenderedWorld -= OnRenderedWorld;

			Events.Input.ButtonPressed -= OnButtonPressed;
			Events.Input.ButtonsChanged -= OnButtonsChanged;
			Events.Input.CursorMoved -= OnCursorMoved;
			Events.Input.MouseWheelScrolled -= OnMouseWheelScrolled;
		}

		#endregion

		#region Event Wrapping

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
			if (Context.ScreenId == ScreenID) {

				var view = Game1.uiViewport;
				if (view.Width != LastViewport.Width || view.Height != LastViewport.Height) {
					Rectangle rect = view.ToXna();
					ReceiveGameWindowResized(rect);
					LastViewport = rect;
				}

				HandleUpdateTicked();

			} else if (!Context.HasScreenId(ScreenID))
				Dispose();
		}

		private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			PreDrawUI(e.SpriteBatch);
		}

		private void OnRendered(object sender, RenderedEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			DrawUI(e.SpriteBatch);
		}

		private void OnRenderedWorld(object sender, RenderedWorldEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			DrawWorld(e.SpriteBatch);
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			// Left Mouse / Use Tool
			bool useTool = e.Button.IsUseToolButton();
			bool action = e.Button.IsActionButton();

			if (!useTool && !action && !hasOtherKeys)
				return;

			bool uiMode = AssumeUIMode ?? Game1.uiMode;
			bool handled;

			int x = Game1.getMouseX(uiMode);
			int y = Game1.getMouseY(uiMode);

			if (useTool)
				handled = ReceiveLeftClick(x, y);
			else if (action)
				handled = ReceiveRightClick(x, y);
			else
				handled = ReceiveButtonPressed(sender, e);

			if (handled)
				InputHelper.Suppress(e.Button);
		}

		private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			ReceiveButtonsChanged(sender, e);
		}

		private void OnCursorMoved(object sender, CursorMovedEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			bool uiMode = AssumeUIMode ?? Game1.uiMode;
			bool handled = ReceiveCursorHover(Game1.getMouseX(uiMode), Game1.getMouseY(uiMode));
			if (handled)
				Game1.InvalidateOldMouseMovement();
		}

		private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e) {
			if (Context.ScreenId != ScreenID)
				return;

			bool handled = ReceiveScrollWheelAction(e.Delta);
			if (handled) {
				MouseState old = Game1.oldMouseState;
				Game1.oldMouseState = new MouseState(
					x: old.X,
					y: old.Y,
					scrollWheel: e.NewValue,
					leftButton: old.LeftButton,
					middleButton: old.MiddleButton,
					rightButton: old.RightButton,
					xButton1: old.XButton1,
					xButton2: old.XButton2
				);
			}
		}

		#endregion

		#region Utilities

		protected void DrawCursor(SpriteBatch batch) {
			if (Game1.options.hardwareCursor)
				return;

			Vector2 pos = new(Game1.getMouseX(), Game1.getMouseY());
			batch.Draw(Game1.mouseCursors, pos, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0f, Vector2.Zero, Game1.pixelZoom * Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
		}

		private bool IsMethodOverridden(string name) {
			MethodInfo method = GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (method == null)
				throw new InvalidOperationException($"Cannot find method {GetType().FullName}.{name}");

			return method.DeclaringType != typeof(BaseOverlay<T>);
		}

		#endregion

	}
}
