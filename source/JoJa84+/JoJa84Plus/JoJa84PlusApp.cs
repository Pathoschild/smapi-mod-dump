using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;


namespace JoJa84Plus
{
    internal class JoJa84PlusApp
	{
		private static IModHelper helper;
		private static IMonitor monitor;
		private static ModConfig config;
        private static IMobilePhoneApi api;
		private enum Operation
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			None
		}
		private static Operation op = Operation.None;
		private static double result = 0;
		private static string inputA = "";
		private static string inputB = "";
		private static bool currentInput = false;
		private static int xPositionOnScreen;
		private static int yPositionOnScreen;
		private static int widthOnScreen = 0;
		private static int heightOnScreen = 0;
		private static int timer = 0;
		private static List<ClickableComponent> numpad = new List<ClickableComponent>();
		private static List<ClickableComponent> opButtons = new List<ClickableComponent>();
		private static ClickableComponent clearButton;
		private static ClickableComponent backspaceButton;
		private static ClickableComponent zeroButton;
        private static Texture2D backgroundTexture;
        private static Texture2D backgroundLandscapeTexture;

        internal static void Initialize(IModHelper _helper, IMonitor _monitor, ModConfig _config, IMobilePhoneApi _api)
        {
			helper = _helper;
			monitor = _monitor;
			config = _config;
			api = _api;
			Vector2 ps = api.GetScreenSize(false);
			Vector2 ls = api.GetScreenSize(true);
			backgroundTexture = new Texture2D(Game1.graphics.GraphicsDevice, (int)ps.X, (int)ps.Y);
			backgroundLandscapeTexture = new Texture2D(Game1.graphics.GraphicsDevice, (int)ls.X, (int)ls.Y);
			Color[] data = new Color[backgroundTexture.Width * backgroundTexture.Height];
			Color[] data2 = new Color[backgroundLandscapeTexture.Width * backgroundLandscapeTexture.Height];
			int i = 0;
			while (i < data.Length || i < data2.Length)
			{
				if (i < data.Length)
					data[i] = config.AppBackgroundColor;
				if (i < data2.Length)
					data2[i] = config.AppBackgroundColor;
				i++;
			}
			backgroundTexture.SetData(data);
			backgroundLandscapeTexture.SetData(data2);
		}

		internal static void Start()
		{
			api.SetRunningApp(helper.ModRegistry.ModID);
			api.SetAppRunning(true);
			helper.Events.Display.Rendered += Display_Rendered;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
		}

        private static void Display_Rendered(object sender, RenderedEventArgs e)
        {
			if (api.IsCallingNPC())
				return;

			if(!api.GetPhoneOpened() || !api.GetAppRunning() || api.GetRunningApp() != helper.ModRegistry.ModID)
            {
				monitor.Log($"Closing app");
				helper.Events.Display.Rendered -= Display_Rendered;
				helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
				return;
            }

			Rectangle screenRect = api.GetScreenRectangle();
			xPositionOnScreen = screenRect.X + config.AppMarginX;
			yPositionOnScreen = screenRect.Y + config.AppMarginY;
			widthOnScreen = screenRect.Width - config.AppMarginX * 2;
			heightOnScreen = screenRect.Height - config.AppMarginY * 2;

			bool rotated = api.GetPhoneRotated();
			int textHeight = (Game1.smallFont.LineSpacing + config.AppMarginY) * (rotated ? 1 : 2);
			int spaceBelowText = heightOnScreen - textHeight;
			int buttonHeight1 = spaceBelowText / 5; 
			int buttonHeight2 = (spaceBelowText - buttonHeight1) / 6;

			numpad.Clear();
			opButtons.Clear();

			// Create the number-pad.
			for (int iy = 0; iy < 3; iy++)
			{
				for (int ix = 0; ix < 3; ix++)
				{
					int buttonWidth = (widthOnScreen / 4) - 2;
					numpad.Add
					(
						new ClickableComponent
						(
							new Rectangle
							(
								xPositionOnScreen
									+ buttonWidth * ix,
								yPositionOnScreen
									+ buttonHeight1 * (iy + 1)
									+ textHeight,
								buttonWidth - 2,
								buttonHeight1 - 2
							),
							(Math.Abs(((3 - iy) * 3) + ix - 2)).ToString()
						)
					);
				}
			}

			// Create the op buttons.
			for (int i = 0; i < 6; i++)
			{
				int xOffset = ((widthOnScreen) / 4) * 3;
				int yOffset = ((heightOnScreen) * 9 / 16 / 4) * 3;
				opButtons.Add
				(
					new ClickableComponent
					(
						new Rectangle
						(
							xPositionOnScreen
								+ xOffset
								+ xOffset / 16,
							yPositionOnScreen
								+ (buttonHeight2) * i
								+ textHeight
								+ buttonHeight1,
							xOffset / 4 - 2,
							buttonHeight2 - 2
						),
						i switch
						{
							0 => "+",
							1 => "-",
							2 => "X",
							3 => "/",
							4 => "EQ",
							5 => ".",
							_ => ""
						}
					)
				);
			}

			// Create the clear button.
			clearButton = new ClickableComponent
			(
				new Rectangle
				(
					xPositionOnScreen,
					yPositionOnScreen
						+ textHeight,
					(widthOnScreen) / 2 - 1,
					buttonHeight1 - 2
				),
				"clear",
				"Clear"
			);

			// Create the backspace button.
			backspaceButton = new ClickableComponent
			(
				new Rectangle
				(
					xPositionOnScreen
						+ 1
						+ (widthOnScreen) / 2,
					yPositionOnScreen
						+ textHeight,
					(widthOnScreen) / 2,
					buttonHeight1 -2
				),
				"backspace",
				"Back"
			);

			// Create the zero button.
			zeroButton = new ClickableComponent
			(
				new Rectangle
				(
					xPositionOnScreen 
						+ ModEntry.jojaAppLogo.Width + 4,
					yPositionOnScreen
						+ textHeight + buttonHeight1 * 4,
					((widthOnScreen) / 4) * 3 - ModEntry.jojaAppLogo.Width - 12,
					buttonHeight1 - 2
				),
				"0",
				"0"
			);

			SpriteBatch b = e.SpriteBatch;

			b.Draw(rotated ? backgroundLandscapeTexture : backgroundTexture, screenRect, Color.White);

			// Draw JoJa watermark thing.
			b.Draw(
				ModEntry.jojaAppLogo,
				new Vector2
				(
					xPositionOnScreen,
					yPositionOnScreen
						+ heightOnScreen
						- buttonHeight1 / 2
						- ModEntry.jojaAppLogo.Height / 2
				),
				Color.White
			);

			string inputOut = currentInput ? inputB : inputA;

			// Draw the current input.
			b.DrawString
			(
				Game1.smallFont,
				inputOut + ((timer >= 16) ? "|" : ""),
				new Vector2(
					(float)xPositionOnScreen
						+ widthOnScreen
						- Game1.smallFont.MeasureString(inputOut).X
						- config.AppMarginX,
					(float)yPositionOnScreen
				),
				config.InputColor
			);

			// Draw the previous input, if currently entering the second number.
			if (currentInput)
			{
				string prevInput = inputA
					+ " "
					+ (op switch
					{
						Operation.Add => "+",
						Operation.Subtract => "-",
						Operation.Multiply => "X",
						Operation.Divide => "/",
						Operation.None => "ERR",
						_ => "ERR",
					});
				b.DrawString
				(
					Game1.smallFont,
					prevInput,
					new Vector2
					(
						(float)xPositionOnScreen
							+ widthOnScreen
							- Game1.smallFont.MeasureString(prevInput).X 
							- (rotated ? (Game1.smallFont.MeasureString(inputOut).X + config.AppMarginX) : 0)
							- config.AppMarginX,
						(float)yPositionOnScreen
							+ (rotated ? 0 : (Game1.smallFont.LineSpacing + config.AppMarginY))
					),
					config.PrevInputColor
				);
			}

			// Draw the number pad.
			foreach (ClickableComponent button in numpad)
			{
				IClickableMenu.drawTextureBox
				(
					b,
					Game1.mouseCursors,
					new Rectangle(432, 439, 9, 9),
					button.bounds.X,
					button.bounds.Y,
					button.bounds.Width,
					button.bounds.Height,
					(button.scale != 1.0001f) ? Color.Wheat : Color.White,
					4f,
					false
				);
				Utility.drawBoldText
				(
					b,
					button.name,
					Game1.smallFont,
					new Vector2
					(
						(float)button.bounds.X
							+ (button.bounds.Width / 2)
							- (Game1.smallFont.MeasureString(button.name).X / 2),
						(float)button.bounds.Y
							+ (button.bounds.Height / 2)
							- (Game1.smallFont.MeasureString(button.name).Y / 2)
					),
					Game1.textColor,
					1f,
					-1f,
					2
				);
			}

			// Draw the operator buttons.
			foreach (ClickableComponent button in opButtons)
			{
				IClickableMenu.drawTextureBox
				(
					b,
					Game1.mouseCursors,
					new Rectangle(432, 439, 9, 9),
					button.bounds.X,
					button.bounds.Y,
					button.bounds.Width,
					button.bounds.Height,
					(button.scale != 1.0001f) ? Color.Wheat : Color.White,
					4f,
					false
				);
				Utility.drawBoldText
				(
					b,
					button.name,
					Game1.smallFont,
					new Vector2(
						(float)button.bounds.X
							+ (button.bounds.Width / 2)
							- (Game1.smallFont.MeasureString(button.name).X / 2),
						(float)button.bounds.Y
							+ (button.bounds.Height / 2)
							- (Game1.smallFont.MeasureString(button.name).Y / 2)
							+ 2
					),
					Game1.textColor,
					1f,
					-1f,
					2
				);
			}

			// Draw the clear button.
			IClickableMenu.drawTextureBox
			(
				b,
				Game1.mouseCursors,
				new Rectangle(432, 439, 9, 9),
				clearButton.bounds.X,
				clearButton.bounds.Y,
				clearButton.bounds.Width,
				clearButton.bounds.Height,
				(clearButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			b.DrawString
			(
				Game1.smallFont,
				clearButton.label,
				new Vector2
				(
					(float)clearButton.bounds.X
						+ (clearButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(clearButton.label).X / 2),
					(float)clearButton.bounds.Y
						+ (clearButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(clearButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the backspace button.
			IClickableMenu.drawTextureBox
			(
				b,
				Game1.mouseCursors,
				new Rectangle(432, 439, 9, 9),
				backspaceButton.bounds.X,
				backspaceButton.bounds.Y,
				backspaceButton.bounds.Width,
				backspaceButton.bounds.Height,
				(backspaceButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			b.DrawString
			(
				Game1.smallFont,
				backspaceButton.label,
				new Vector2
				(
					(float)backspaceButton.bounds.X
						+ (backspaceButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(backspaceButton.label).X / 2),
					(float)backspaceButton.bounds.Y
						+ (backspaceButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(backspaceButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the zero button.
			IClickableMenu.drawTextureBox
			(
				b,
				Game1.mouseCursors,
				new Rectangle(432, 439, 9, 9),
				zeroButton.bounds.X,
				zeroButton.bounds.Y,
				zeroButton.bounds.Width,
				zeroButton.bounds.Height,
				(zeroButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			Utility.drawBoldText
			(
				b,
				zeroButton.label,
				Game1.smallFont,
				new Vector2
				(
					(float)zeroButton.bounds.X
						+ (zeroButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(zeroButton.label).X / 2),
					(float)zeroButton.bounds.Y
						+ (zeroButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(zeroButton.name).Y / 2)
				),
				Game1.textColor,
				1f,
				-1f,
				2
			);

			//if (shouldDrawCloseButton()) base.draw(b);
			if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);

			timer++;
			if (timer >= 32) timer = 0;
		}


		private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!api.GetPhoneOpened() || !api.GetAppRunning() || api.GetRunningApp() != helper.ModRegistry.ModID || !api.GetScreenRectangle().Contains(Game1.getMousePosition()))
				return;

			helper.Input.Suppress(e.Button);

			if (e.Button == SButton.MouseLeft)
            {
				int x = Game1.getMouseX();
				int y = Game1.getMouseY();

				performHoverAction(x, y);

				foreach (ClickableComponent button in numpad)
				{
					if (button.containsPoint(x, y))
					{
						if (currentInput)
							inputB += button.name;
						else
							inputA += button.name;
						Game1.playSound("smallSelect");
					}
				}
				foreach (ClickableComponent button in opButtons)
				{
					if (button.containsPoint(x, y))
					{
						switch (button.name)
						{
							case "+":
								if (!currentInput)
									currentInput = true;
								op = Operation.Add;
								break;
							case "-":
								if (!currentInput)
									currentInput = true;
								op = Operation.Subtract;
								break;
							case "X":
								if (!currentInput)
									currentInput = true;
								op = Operation.Multiply;
								break;
							case "/":
								if (!currentInput)
									currentInput = true;
								op = Operation.Divide;
								break;
							case "EQ":
								DoCalculation();
								break;
							case ".":
								if (!currentInput)
									inputA += ".";
								else
									inputB += ".";
								break;
						}
						Game1.playSound("smallSelect");
					}
				}
				if (zeroButton.containsPoint(x, y))
				{
					if (!currentInput)
						inputA += "0";
					else
						inputB += "0";
					Game1.playSound("smallSelect");
				}
				if (clearButton.containsPoint(x, y))
				{
					result = 0;
					inputA = "";
					inputB = "";
					currentInput = false;
					Game1.playSound("backpackIN");
				}
				if (backspaceButton.containsPoint(x, y))
				{
					if (!currentInput)
					{
						if (inputA.Length >= 1) inputA = inputA.Substring(0, inputA.Length - 1);
					}
					else
					{
						if (inputB.Length >= 1) inputB = inputB.Substring(0, inputB.Length - 1);
					}
					Game1.playSound("smallSelect");
				}
				return;
			}

			char keyChar = e.Button switch
			{
				SButton.NumPad0 => '0',
				SButton.NumPad1 => '1',
				SButton.NumPad2 => '2',
				SButton.NumPad3 => '3',
				SButton.NumPad4 => '4',
				SButton.NumPad5 => '5',
				SButton.NumPad6 => '6',
				SButton.NumPad7 => '7',
				SButton.NumPad8 => '8',
				SButton.NumPad9 => '9',
				SButton.D0 => '0',
				SButton.D1 => '1',
				SButton.D2 => '2',
				SButton.D3 => '3',
				SButton.D4 => '4',
				SButton.D5 => '5',
				SButton.D6 => '6',
				SButton.D7 => '7',
				SButton.D8 => '8',
				SButton.D9 => '9',
				SButton.OemPeriod => '.',
				_ => '_',
			};
			if (keyChar != '_')
			{
				if (currentInput)
					inputB += keyChar;
				else
					inputA += keyChar;
			}
			else
			{
				switch (e.Button)
				{
					case SButton.Add:
						if (!currentInput)
							currentInput = true;
						op = Operation.Add;
						break;
					case SButton.Subtract:
						if (!currentInput)
							currentInput = true;
						op = Operation.Subtract;
						break;
					case SButton.Multiply:
						if (!currentInput)
							currentInput = true;
						op = Operation.Multiply;
						break;
					case SButton.Divide:
						if (!currentInput)
							currentInput = true;
						op = Operation.Divide;
						break;
					case SButton.Enter:
						DoCalculation();
						break;
					case SButton.Delete:
						result = 0;
						inputA = "";
						inputB = "";
						currentInput = false;
						break;
					case SButton.Back:
						if (!currentInput)
						{
							if (inputA.Length >= 1) inputA = inputA.Substring(0, inputA.Length - 1);
						}
						else
						{
							if (inputB.Length >= 1) inputB = inputB.Substring(0, inputB.Length - 1);
						}
						break;
					case SButton.Escape:
						exitApp();
						break;
				}
			}
		}

        private static void exitApp()
        {
			if(api.GetRunningApp() == helper.ModRegistry.ModID)
            {
				api.SetAppRunning(false);
				api.SetRunningApp(null);
            }
        }

        public static void performHoverAction(int x, int y)
		{
			foreach (ClickableComponent button in numpad)
			{
				if (button.containsPoint(x, y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			foreach (ClickableComponent button in opButtons)
			{
				if (button.containsPoint(x, y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			if (zeroButton.containsPoint(x, y))
				zeroButton.scale = 1.0001f;
			else
				zeroButton.scale = 1f;
			if (clearButton.containsPoint(x, y))
				clearButton.scale = 1.0001f;
			else
				clearButton.scale = 1f;
			if (backspaceButton.containsPoint(x, y))
				backspaceButton.scale = 1.0001f;
			else
				backspaceButton.scale = 1f;
		}

		public void receiveLeftClick(int x, int y, bool playSound)
		{

		}

		private static void DoCalculation()
		{
			if (!currentInput)
			{
				double.TryParse(inputA, out result);
				currentInput = false;
			}
			else
			{
				double iA, iB;
				double.TryParse(inputA, out iA);
				double.TryParse(inputB, out iB);
				switch (op)
				{
					case Operation.Add:
						result = iA + iB;
						break;
					case Operation.Subtract:
						result = iA - iB;
						break;
					case Operation.Multiply:
						result = iA * iB;
						break;
					case Operation.Divide:
						result = iA / iB;
						break;
					case Operation.None:
						break;
				}
				inputA = result.ToString();
				if (inputA == "NaN") inputA = "";
				inputB = "";
				currentInput = false;
			}
		}
	}
}