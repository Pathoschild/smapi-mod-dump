/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/K4rakara/Stardew-Mods
**
*************************************************/

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
	public class ModConfig
	{
		public SButton HotKey { get; set; }
		public bool EnableHotKey { get; set; }
		public bool EnableMobileApp { get; set; }
		public Color AppBackgroundColor { get; set; }
		public Color InputColor { get; set; }
		public Color PrevInputColor { get; set; }
		public int AppMarginX { get; set; }
		public int AppMarginY { get; set; }

		public ModConfig()
		{
			HotKey = SButton.F9;
			EnableHotKey = true;
			EnableMobileApp = true;
			AppBackgroundColor = new Color(255, 200, 120);
			InputColor = Color.Black;
			PrevInputColor = new Color(100, 100, 100);
			AppMarginX = 5;
			AppMarginY = 5;
		}
	}

	public class ModEntry: Mod
	{
		public static Texture2D jojaLogo;
		public static Texture2D jojaAppLogo;
		private bool CalcOpen = false;
		private ModConfig Config;
		private JoJa84PlusMenu menu;
		public static IMobilePhoneApi api;

		public override void Entry(IModHelper helper)
		{
			// Load config.
			Config = Helper.ReadConfig<ModConfig>();

			// Add event listeners.
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

			// Load the JoJa 84+ logo
			jojaLogo = helper.Content.Load<Texture2D>("assets/joja84plus.png");
			jojaAppLogo = helper.Content.Load<Texture2D>("assets/app_icon.png");
		}
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			if (Config.EnableMobileApp)
			{
				api = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
				if (api != null)
				{
					Texture2D appIcon;
					bool success;
					appIcon = Helper.Content.Load<Texture2D>(Path.Combine("assets", "app_icon.png"));
					success = api.AddApp(Helper.ModRegistry.ModID, Helper.Translation.Get("app-name"), OpenApp, appIcon);
					Monitor.Log($"loaded app successfully: {success}", LogLevel.Debug);
					JoJa84PlusApp.Initialize(Helper, Monitor, Config, api);
				}
			}
		}
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			menu = new JoJa84PlusMenu(jojaLogo);
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// Ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// If the player presses F9, then open/close the calculator.
			if (e.Button == Config.HotKey && Config.EnableHotKey)
			{
				CalcOpen = !CalcOpen;
				if (CalcOpen)
				{
					Game1.activeClickableMenu = menu;
					Game1.playSound("bigSelect");
				}
				else 
				{
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			}
			else if (e.Button == SButton.Escape && CalcOpen) 
			{
				CalcOpen = false;
			}
		}
		void OpenApp()
		{
			Monitor.Log("Opening App");
			JoJa84PlusApp.Start();
		}
	}

	public class JoJa84PlusMenu: IClickableMenu
	{
		private enum Operation
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			None
		}
		private Operation op = Operation.None;
		private double result = 0;
		private string inputA = "";
		private string inputB = "";
		private bool currentInput = false;
		private int widthOnScreen = 0;
		private int heightOnScreen = 0;
		private int timer = 0;
		private List<ClickableComponent> numpad = new List<ClickableComponent>();
		private List<ClickableComponent> opButtons = new List<ClickableComponent>();
		private ClickableComponent clearButton;
		private ClickableComponent backspaceButton;
		private ClickableComponent zeroButton;
		private Texture2D jojaLogo;
		public JoJa84PlusMenu(Texture2D logo): 
			base(
				Game1.viewport.Width / 2 - 256 / 2,
				Game1.viewport.Height / 2 - 256,
				256 + IClickableMenu.borderWidth * 2,
				512 + IClickableMenu.borderWidth * 2,
				showUpperRightCloseButton: false
			)
		{
			xPositionOnScreen = Game1.viewport.Width / 2 - 256 / 2;
			yPositionOnScreen = Game1.viewport.Height / 2 - 256;
			widthOnScreen = 256 + IClickableMenu.borderWidth * 2;
			heightOnScreen = 512 + IClickableMenu.borderWidth * 2;
			
			jojaLogo = logo;

			// Create the number-pad.
			for (int iy = 0; iy < 3; iy++)
			{
				for (int ix = 0; ix < 3; ix++)
				{
					int buttonWidth = ((widthOnScreen - IClickableMenu.borderWidth * 2) / 4) - 2;
					numpad.Add
					(
						new ClickableComponent
						(
							new Rectangle
							(
								xPositionOnScreen
									+ 40
									+ buttonWidth * ix,
								yPositionOnScreen
									+ IClickableMenu.borderWidth
									+ IClickableMenu.spaceToClearTopBorder
									+ buttonWidth * iy
									+ (Game1.smallFont.LineSpacing * 4)
									+ (((widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3) / 16,
								buttonWidth - 2,
								buttonWidth - 2
							),
							(Math.Abs(((3-iy)*3)+ix-2)).ToString()
						)
					);
				}
			}

			// Create the op buttons.
			for (int i = 0; i < 6; i++)
			{
				int xOffset = ((widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3;
				opButtons.Add
				(
					new ClickableComponent
					(
						new Rectangle
						(
							xPositionOnScreen
								+ 40
								+ xOffset
								+ xOffset / 16,
							yPositionOnScreen
								+ IClickableMenu.borderWidth
								+ IClickableMenu.spaceToClearTopBorder
								+ ((xOffset) / 4) * i
								+ (Game1.smallFont.LineSpacing * 4),
							xOffset / 4 - 2,
							xOffset / 4 - 2
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
					xPositionOnScreen
						+ 40,
					yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(widthOnScreen - IClickableMenu.borderWidth * 2) / 2 - 1,
					Game1.smallFont.LineSpacing * 2
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
						+ 41
						+ (widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					Game1.smallFont.LineSpacing * 2
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
						+ 40,
					yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ Game1.smallFont.LineSpacing * 2
						+ (widthOnScreen - IClickableMenu.borderWidth * 2)
						+ (((widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3) / 16,
					((widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3,
					Game1.smallFont.LineSpacing * 2
				),
				"0",
				"0"
			);
		}
		public override void draw(SpriteBatch b)
		{
			if (!Game1.options.showMenuBackground) b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
			Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);
			// Draw JoJa watermark thing.
			b.Draw(
				jojaLogo,
				new Vector2
				(
					(float)xPositionOnScreen
						+ 40,
					(float)yPositionOnScreen
						+ heightOnScreen
						- IClickableMenu.borderWidth 
						- 40
				),
				new Rectangle(0, 0, 42, 16),
				Color.White,
				0f,
				new Vector2(0f, 0f),
				2f,
				SpriteEffects.None,
				0f
			);

			// Draw the current input.
			b.DrawString
			(
				Game1.smallFont,
				String.Concat(((currentInput) ? inputB : inputA), ((timer >= 16) ? "|" : "")),
				new Vector2(
					(float)xPositionOnScreen
						+ widthOnScreen
						- 40
						- Game1.smallFont.MeasureString((currentInput) ? inputB + "|" : inputA + "|").X,
					(float)yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
				),
				Game1.textColor
			);

			// Draw the previous input, if currently entering the second number.
			if (currentInput)
			{
				string prevInput = inputA 
					+ " "
					+ (op switch {
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
							- 40
							- Game1.smallFont.MeasureString(prevInput).X,
						(float)yPositionOnScreen
							+ IClickableMenu.borderWidth
							+ IClickableMenu.spaceToClearSideBorder
							+ Game1.smallFont.LineSpacing * 2
					),
					Game1.textShadowColor
				);
			}

			// Draw the number pad.
			foreach(ClickableComponent button in numpad)
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
			foreach(ClickableComponent button in opButtons)
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

			if (shouldDrawCloseButton()) base.draw(b);
			if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			
			timer++;
			if (timer >= 32) timer = 0;
		}
		public override void receiveKeyPress(Keys key)
		{
			char keyChar = key switch
			{
				Keys.NumPad0 => '0',
				Keys.NumPad1 => '1',
				Keys.NumPad2 => '2',
				Keys.NumPad3 => '3',
				Keys.NumPad4 => '4',
				Keys.NumPad5 => '5',
				Keys.NumPad6 => '6',
				Keys.NumPad7 => '7',
				Keys.NumPad8 => '8',
				Keys.NumPad9 => '9',
				Keys.D0 => '0',
				Keys.D1 => '1',
				Keys.D2 => '2',
				Keys.D3 => '3',
				Keys.D4 => '4',
				Keys.D5 => '5',
				Keys.D6 => '6',
				Keys.D7 => '7',
				Keys.D8 => '8',
				Keys.D9 => '9',
				Keys.OemPeriod => '.',
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
				switch (key) 
				{
					case Keys.Add:
						if (!currentInput)
							currentInput = true;
						op = Operation.Add;
						break;
					case Keys.Subtract:
						if (!currentInput)
							currentInput = true;
						op = Operation.Subtract;
						break;
					case Keys.Multiply:
						if (!currentInput)
							currentInput = true;
						op = Operation.Multiply;
						break;
					case Keys.Divide:
						if (!currentInput)
							currentInput = true;
						op = Operation.Divide;
						break;
					case Keys.Enter:
						DoCalculation();
						break;
					case Keys.Delete:
						result = 0;
						inputA = "";
						inputB = "";
						currentInput = false;
						break;
					case Keys.Back:
						if (!currentInput)
						{
							if (inputA.Length >= 1) inputA = inputA.Substring(0, inputA.Length - 1);
						}
						else
						{
							if (inputB.Length >= 1) inputB = inputB.Substring(0, inputB.Length - 1);
						}
						break;
					case Keys.Escape:
						exitThisMenu();
						break;
				}
			}
		}

		public override void performHoverAction(int x, int y)
		{
			foreach(ClickableComponent button in numpad)
			{
				if (button.containsPoint(x,y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			foreach(ClickableComponent button in opButtons)
			{
				if (button.containsPoint(x,y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			if (zeroButton.containsPoint(x,y))
				zeroButton.scale = 1.0001f;
			else
				zeroButton.scale = 1f;
			if (clearButton.containsPoint(x,y))
				clearButton.scale = 1.0001f;
			else
				clearButton.scale = 1f;
			if (backspaceButton.containsPoint(x,y))
				backspaceButton.scale = 1.0001f;
			else
				backspaceButton.scale = 1f;
		}

		public override void receiveLeftClick(int x, int y, bool playSound)
		{
			foreach(ClickableComponent button in numpad)
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
			foreach(ClickableComponent button in opButtons)
			{
				if (button.containsPoint(x, y))
				{
					switch(button.name)
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
		}

		private void DoCalculation()
		{
			if (!currentInput) 
			{
				double.TryParse(inputA, out result);
				currentInput = false;
			}
			else
			{
				double inputA, inputB;
				double.TryParse(this.inputA, out inputA);
				double.TryParse(this.inputB, out inputB);
				switch (op)
				{
					case Operation.Add:
						result = inputA + inputB;
						break;
					case Operation.Subtract:
						result = inputA - inputB;
						break;
					case Operation.Multiply:
						result = inputA * inputB;
						break;
					case Operation.Divide:
						result = inputA / inputB;
						break;
					case Operation.None:
						break;
				}
				this.inputA = result.ToString();
				if (this.inputA == "NaN") this.inputA = "";
				this.inputB = "";
				currentInput = false;
			}
		}
	}
}
