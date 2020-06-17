using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace PregnancyRole
{
	internal abstract class DropdownOverlay : IDisposable
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		protected static readonly bool IsAndroid =
			Constants.TargetPlatform == GamePlatform.Android;

		protected Point offset { get; private set; }

		private readonly OptionsDropDown dropdown;
		private readonly int dropdownBaseWidth;
		private bool isRendering;

		protected DropdownOverlay ()
		{
			// Create the dropdown with the Role options.
			dropdown = new OptionsDropDown ("", -1, 0, 0);
			dropdown.dropDownOptions.Add ("become");
			dropdown.dropDownOptions.Add ("make");
			dropdown.dropDownOptions.Add ("adopt");
			dropdown.dropDownDisplayOptions.Add
				(Helper.Translation.Get ("role_become"));
			dropdown.dropDownDisplayOptions.Add
				(Helper.Translation.Get ("role_make"));
			dropdown.dropDownDisplayOptions.Add
				(Helper.Translation.Get ("role_adopt"));
			dropdownBaseWidth = dropdown.bounds.Width;

			// Listen for game events.
			Helper.Events.Display.RenderedActiveMenu += onRenderedActiveMenu;
			Helper.Events.Input.ButtonPressed += onButtonPressed;
			Helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
			Helper.Events.Input.ButtonReleased += onButtonReleased;
		}

		protected void setOffset (Point offset)
		{
			this.offset = offset;

			// Update the position of the control proper.
			dropdown.bounds.X = offset.X - 52;
			dropdown.bounds.Y = offset.Y;
			if (IsAndroid)
				dropdown.bounds.Width = dropdownBaseWidth - offset.X;

			// Update the bounds of the expanded menu through reflection, since
			// the Android port makes this field private for some reason.
			Rectangle bounds = new Rectangle (dropdown.bounds.X, dropdown.bounds.Y,
				dropdown.bounds.Width + (IsAndroid ? 0 : -48),
				dropdown.bounds.Height * dropdown.dropDownOptions.Count);
			Helper.Reflection.GetField<Rectangle>
				(dropdown, "dropDownBounds").SetValue (bounds);
		}

		public void Dispose ()
		{
			Helper.Events.Display.RenderedActiveMenu -= onRenderedActiveMenu;
			Helper.Events.Input.ButtonPressed -= onButtonPressed;
			Helper.Events.GameLoop.UpdateTicked -= onUpdateTicked;
			Helper.Events.Input.ButtonReleased -= onButtonReleased;
		}

		protected abstract int roleIndex { get; set; }

		protected abstract bool shouldRender { get; }

		protected virtual IClickableMenu trueMenu => Game1.activeClickableMenu;

		protected virtual string hoverText => Helper.Reflection.GetField<string>
			(trueMenu, "hoverText", false)?.GetValue ();

		protected virtual string hoverTitle => Helper.Reflection.GetField<string>
			(trueMenu, "hoverTitle", false)?.GetValue ();

		protected virtual Item hoveredItem => Helper.Reflection.GetField<Item>
			(trueMenu, "hoveredItem", false)?.GetValue ();

		public void onRenderedActiveMenu (object _sender,
			RenderedActiveMenuEventArgs e)
		{
			if (!shouldRender)
				return;
			var trueMenu = this.trueMenu;

			// Draw the label.
			string label = Helper.Translation.Get ("PregnancyRole");
			Vector2 position = new Vector2
				(trueMenu.xPositionOnScreen + offset.X -
					Game1.smallFont.MeasureString (label).X + 4 - 64,
				trueMenu.yPositionOnScreen + offset.Y + 8);
			e.SpriteBatch.DrawString (Game1.smallFont, label, position,
				Game1.textColor);

			// Draw the dropdown.
			dropdown.draw (e.SpriteBatch,
				trueMenu.xPositionOnScreen,
				trueMenu.yPositionOnScreen);

			// Raise any hovertext back over the label and dropdown.
			string hoverText = this.hoverText ?? "";
			string hoverTitle = this.hoverTitle ?? "";
			Item hoveredItem = this.hoveredItem;
			if (hoveredItem != null)
			{
				if (!Game1.options.snappyMenus &&
					!Game1.options.gamepadControls &&
					Game1.lastCursorMotionWasMouse)
				{
					IClickableMenu.drawToolTip (e.SpriteBatch,
						hoveredItem.getDescription (), hoveredItem.DisplayName,
						hoveredItem);
				}
			}
			else if (hoverText.Length > 0)
			{
				IClickableMenu.drawHoverText (e.SpriteBatch, hoverText,
					Game1.smallFont, 0, 0, -1,
					(hoverTitle.Length > 0) ? hoverTitle : null);
			}

			// Raise the mouse cursor back over everything.
			trueMenu.drawMouse (e.SpriteBatch);
		}

		private void onButtonPressed (object _sender,
			ButtonPressedEventArgs e)
		{
			if (!isRendering || !checkButtonConditions (e.Button, e.Cursor,
					out int x, out int y))
				return;
			dropdown.receiveLeftClick (x, y);
			roleIndex = dropdown.selectedOption;
		}

		private void onUpdateTicked (object _sender, UpdateTickedEventArgs _e)
		{
			if (!shouldRender)
			{
				if (isRendering && Config.VerboseLogging)
					Monitor.Log ($"Stopping {GetType ().Name} rendering", LogLevel.Debug);
				isRendering = false;
				return;
			}
			if (!isRendering)
			{
				if (Config.VerboseLogging)
					Monitor.Log ($"Starting {GetType ().Name} rendering", LogLevel.Debug);
				isRendering = true;
				if (OptionsDropDown.selected != dropdown)
					dropdown.selectedOption = roleIndex;
			}

			if (!checkButtonConditions (SButton.MouseLeft, null,
					out int x, out int y))
				return;
			if (Mouse.GetState ().LeftButton == ButtonState.Pressed &&
					Game1.oldMouseState.LeftButton == ButtonState.Pressed)
				dropdown.leftClickHeld (x, y);
		}

		private void onButtonReleased (object _sender,
			ButtonReleasedEventArgs e)
		{
			if (!isRendering || !checkButtonConditions (e.Button, e.Cursor,
					out int x, out int y))
				return;
			dropdown.leftClickReleased (x, y);
			roleIndex = dropdown.selectedOption;
		}

		private bool checkButtonConditions (SButton button,
			ICursorPosition cursor, out int x, out int y)
		{
			cursor ??= Helper.Input.GetCursorPosition ();
			var trueMenu = this.trueMenu;

			x = (int) cursor.ScreenPixels.X - trueMenu?.xPositionOnScreen ?? 0;
			y = (int) cursor.ScreenPixels.Y - trueMenu?.yPositionOnScreen ?? 0;

			if (button != SButton.MouseLeft &&
					button != SButton.ControllerA &&
					button != SButton.C)
				return false;

			if (OptionsDropDown.selected == dropdown)
				return true;

			return dropdown.bounds.Contains (x, y);
		}
	}
}
