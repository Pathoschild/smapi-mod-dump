using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using WarpToFriends.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace WarpToFriends
{
	public class OptionDisplayAttribute : Attribute
	{
		public string Name { get; }
		public int MaxValue { get; } = 10;

		public OptionDisplayAttribute(string name)
		{
			Name = name;
		}

		public OptionDisplayAttribute(string name, int maxValue)
		{
			Name = name;
			MaxValue = maxValue;
		}
	}

	public class OptionsKeyListener<TOptions> : IClickableMenu 
		where TOptions : class, new()
	{

		private TOptions _config;
		private string _property;
		private readonly string _message = "Press New Key...";
		private IClickableMenu _returnMenu;

		public OptionsKeyListener(TOptions config, string property, IClickableMenu returnMenu)
		{
			_config = config;
			_property = property;
			_returnMenu = returnMenu;
		}

		public override void draw(SpriteBatch b)
		{
			_returnMenu.draw(b);

			var scale = 2;
			var xOffset = Game1.smallFont.MeasureString(_message).X / 2 * scale;
			var yOffset = Game1.smallFont.MeasureString(_message).Y / 2 * scale;

			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
			b.DrawString(Game1.dialogueFont, _message, new Vector2(Game1.viewport.Width / 2 - xOffset, Game1.viewport.Height / 2 - yOffset), Color.White);

		}

		public override void receiveKeyPress(Keys key)
		{
			var property = _config.GetType().GetProperties().FirstOrDefault(p => p.Name == _property);
			if(property != null)
				property.SetValue(_config, key.ToString());

			exitThisMenu(true);
			Game1.activeClickableMenu = _returnMenu;
		}
	}

	public class OptionsMenu<TOptions> : IClickableMenu 
		where TOptions : class, new()
	{

		private readonly IModHelper _helper;
		private readonly long _originPlayerId;
		private TOptions _config;
		private IClickableMenu _returnMenu;

		private static int OptionHeight => 75;
		private static int BorderWidth = 4 * Game1.pixelZoom;

		private List<ClickableTextureComponent> Checkboxes { get; set; }
		private List<ClickableTextureComponent> SetButtons { get; set; }
		private List<ClickableTextureComponent> Sliders { get; set; }

		private ClickableTextureComponent Scrollbar;
		private ClickableTextureComponent UpArrow;
		private ClickableTextureComponent DownArrow;
		private Rectangle ScrollbarRail;
		private int CurrentOptionIndex;
		private int OptionCount;
		private readonly int OptionsPerPage;

		private ClickableTextureComponent SliderHeld;
		private bool Sliding;

		public OptionsMenu(IModHelper helper, int w, int h, long originPlayerId, TOptions config, IClickableMenu returnMenu = null) 
			: base((Game1.viewport.Width / 2) - (w / 2), (Game1.viewport.Height / 2) - (h / 2), w, h, true)
		{
			_helper = helper;
			_originPlayerId = originPlayerId;
			_config = config;
			_returnMenu = returnMenu;

			OptionsPerPage = (height - (BorderWidth * 2)) / OptionHeight;

			UpArrow = new ClickableTextureComponent("up-arrow", new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen, 44, 48), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
			DownArrow = new ClickableTextureComponent("down-arrow", new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 48, 44, 48), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
			Scrollbar = new ClickableTextureComponent("scrollbar", new Rectangle(UpArrow.bounds.X + 12, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, 24, 40), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
			ScrollbarRail = new Rectangle(Scrollbar.bounds.X, Scrollbar.bounds.Y, Scrollbar.bounds.Width, height - UpArrow.bounds.Height - DownArrow.bounds.Height - 8); 

			this.exitFunction = new onExit(() => { _helper.WriteConfig(_config); });
		}

		public override void draw(SpriteBatch b)
		{
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, Game1.pixelZoom);

			var options = _config.GetType().GetProperties();
			OptionCount = options.Length;

			Checkboxes = new List<ClickableTextureComponent>();
			SetButtons = new List<ClickableTextureComponent>();
			Sliders = new List<ClickableTextureComponent>();

			for (int currOp = CurrentOptionIndex, idx = 0; idx < Math.Min(OptionCount, OptionsPerPage); idx++, currOp++)
			{
				var option = options[currOp];

				var value = option.GetValue(_config, null);

				var optionType = option.PropertyType;

				var displayName = option.Name;

				var maxValue = 10;

				var optionDisplayAttribute = option.GetCustomAttributes(true).FirstOrDefault(a => (a as OptionDisplayAttribute) != null);

				if (optionDisplayAttribute != null)
				{
					displayName = ((OptionDisplayAttribute)optionDisplayAttribute).Name;
					maxValue = ((OptionDisplayAttribute)optionDisplayAttribute).MaxValue;
				}

				if (optionType == typeof(bool))
				{
					DrawOption(idx, displayName, b);

					DrawBoolOption(idx, (bool)value, option.Name, b);
				}
				else if(optionType == typeof(string))
				{
					DrawOption(idx, displayName, b, value.ToString());

					DrawKeyOption(idx, (string)value, option.Name, b);
				}
				else if (optionType == typeof(int))
				{

					DrawOption(idx, displayName, b, value.ToString());

					DrawSliderOption(idx, (int)value, option.Name, maxValue, b);

				}
			}

			base.draw(b);
			drawScrollbar(b);
			drawMouse(b);
		}

		private void DrawOption(int idx, string optionName, SpriteBatch b, string value = null)
		{
			var xPos = this.xPositionOnScreen + BorderWidth;
			var yPos = this.yPositionOnScreen + BorderWidth + (idx * OptionHeight);

			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), xPos, yPos, width - (2 * BorderWidth), OptionHeight, Color.White, Game1.pixelZoom, false);

			var textHeight = Game1.smallFont.MeasureString(optionName).Y;
			var textY = yPos + (OptionHeight / 2f) - (textHeight / 2);

			Utility.drawTextWithShadow(b, optionName + ((value != null) ? $" : {value}" : ""), Game1.smallFont, new Vector2(xPos + BorderWidth, textY), Color.Black);
		}

		private void DrawSliderOption(int idx, int value, string propertyName, int maxValue, SpriteBatch b)
		{
			var setSliderRailSizeX = 25 * Game1.pixelZoom;
			var setSliderRailSizeY = 6 * Game1.pixelZoom;

			var xPos = this.xPositionOnScreen + this.width - (2 * BorderWidth) - setSliderRailSizeX;
			var yPos = this.yPositionOnScreen + BorderWidth + (idx * OptionHeight) + (OptionHeight / 2) - (setSliderRailSizeY / 2);

			var src = new Rectangle(403, 383, 6, 6);
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, src, xPos, yPos, setSliderRailSizeX, setSliderRailSizeY, Color.White, Game1.pixelZoom, false);

			var setSliderSizeX = 10 * Game1.pixelZoom;
			var setSliderSizeY = 6 * Game1.pixelZoom;

			xPos += setSliderRailSizeX / maxValue * value;

			src = new Rectangle(420, 441, 10, 6);
			var slider = new ClickableTextureComponent(propertyName, new Rectangle(xPos, yPos, setSliderSizeX, setSliderSizeY), "", "", Game1.mouseCursors, src, Game1.pixelZoom, true);

			Sliders.Add(slider);
			slider.draw(b);
		}

		private void DrawKeyOption(int idx, string value, string propertyName, SpriteBatch b)
		{
			var setButtonSizeX =  21 * Game1.pixelZoom;
			var setButtonSizeY =  11 * Game1.pixelZoom;

			var xPos = this.xPositionOnScreen + this.width - (2 * BorderWidth) - setButtonSizeX;
			var yPos = this.yPositionOnScreen + BorderWidth + (idx * OptionHeight) + (OptionHeight / 2) - (setButtonSizeY / 2);

			var src = new Rectangle(294, 428, 21, 11);
			var setButton = new ClickableTextureComponent(propertyName, new Rectangle(xPos, yPos, setButtonSizeX, setButtonSizeY), "", "", Game1.mouseCursors, src, Game1.pixelZoom, true);

			SetButtons.Add(setButton);

			setButton.draw(b);
		}

		private void DrawBoolOption(int idx, bool value, string propertyName, SpriteBatch b)
		{
			var checkboxSize = 9 * Game1.pixelZoom;

			var xPos = this.xPositionOnScreen + this.width - (3 * BorderWidth) - checkboxSize;
			var yPos = this.yPositionOnScreen + BorderWidth + (idx * OptionHeight) + (OptionHeight / 2) - (checkboxSize / 2);

			var uncheckedSrc = new Rectangle(227, 425, 9, 9);
			var checkedSrc = new Rectangle(236, 425, 9, 9);
			var src = value ? checkedSrc : uncheckedSrc;

			var checkbox = new ClickableTextureComponent(propertyName, new Rectangle(xPos, yPos, checkboxSize, checkboxSize), "", "", Game1.mouseCursors, src, Game1.pixelZoom, false);

			Checkboxes.Add(checkbox);
			checkbox.draw(b);
		}

		private void drawScrollbar(SpriteBatch b)
		{
			UpArrow.draw(b);
			DownArrow.draw(b);
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollbarRail.X, ScrollbarRail.Y, ScrollbarRail.Width, ScrollbarRail.Height, Color.White, 4f, false);
			Scrollbar.bounds.Y = ScrollbarRail.Height / Math.Max(1, OptionCount - OptionsPerPage + 1) * CurrentOptionIndex + ScrollbarRail.Y;
			if (CurrentOptionIndex >= OptionCount - OptionsPerPage) Scrollbar.bounds.Y = ScrollbarRail.Bottom - Scrollbar.bounds.Height;
			Scrollbar.draw(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			foreach (var checkbox in Checkboxes)
			{
				if (checkbox.containsPoint(x, y))
				{
					ToggleOption(checkbox.name);
				}
			}
			foreach (var setbutton in SetButtons)
			{
				if (setbutton.containsPoint(x, y))
				{
					Game1.activeClickableMenu = new OptionsKeyListener<TOptions>(_config, setbutton.name, this);
				}
			}
			foreach (var slider in Sliders)
			{
				if (slider.containsPoint(x, y))
				{
					SliderHeld = slider;
					Sliding = true;
				}
			}

			if (upperRightCloseButton.containsPoint(x, y))
			{
				_helper.WriteConfig(_config);

				base.receiveLeftClick(x, y, playSound);

				Game1.activeClickableMenu = _returnMenu;
			}
		}

		public override void receiveScrollWheelAction(int direction)
		{
			if (direction > 0 && CurrentOptionIndex > 0)
			{
				CurrentOptionIndex--;
			}
			else if(direction < 0 && CurrentOptionIndex + OptionsPerPage < OptionCount)
			{
				CurrentOptionIndex++;
			}
		}

		public override void leftClickHeld(int x, int y)
		{
			if (!Sliding || SliderHeld == null) return;

			setSliderValue(SliderHeld, x, y);

		}

		public override void releaseLeftClick(int x, int y)
		{
			SliderHeld = null;
			Sliding = false;
		}

		public override void emergencyShutDown()
		{
			exitFunction?.Invoke();
			_returnMenu?.emergencyShutDown();
		}

		private void ToggleOption(string checkboxName)
		{
			var property = _config.GetType().GetProperties().FirstOrDefault(p => p.Name == checkboxName);

			if (property == null)
			{
				// oops
			}
			else
			{
				property.SetValue(_config, !(bool)property.GetValue(_config, null));
			}

		}

		private void setSliderValue(ClickableTextureComponent slider, int x, int y)
		{
			var property = _config.GetType().GetProperties().FirstOrDefault(p => p.Name == slider.name);

			if (property == null) return;
			
			
			int value = Math.Min((int)property.GetValue(_config, null), 14);
			property.SetValue(_config, ++value);
			
		}

	}
}