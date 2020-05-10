using System;
using System.Collections.Generic;
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
		public string HotKey { get; set; }
		public ModConfig()
		{
			this.HotKey = "F9";
		}
	}

	public class ModEntry: Mod
	{
		private Texture2D jojaLogo;
		private bool CalcOpen = false;
		private ModConfig Config;
		private JoJa84PlusMenu menu;

		public override void Entry(IModHelper helper)
		{
			// Load config.
			this.Config = this.Helper.ReadConfig<ModConfig>();

			// Add event listeners.
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

			// Load the JoJa 84+ logo
			this.jojaLogo = helper.Content.Load<Texture2D>("assets/joja84plus.png");
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.menu = new JoJa84PlusMenu(this.jojaLogo);
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// Ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// If the player presses F5, then open/close the calculator.
			if (e.Button == (SButton)Enum.Parse(typeof(SButton), this.Config.HotKey))
			{
				this.CalcOpen = !this.CalcOpen;
				if (this.CalcOpen)
				{
					Game1.activeClickableMenu = this.menu;
					Game1.playSound("bigSelect");
				}
				else 
				{
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			}
			else if (e.Button == SButton.Escape && this.CalcOpen) 
			{
				this.CalcOpen = false;
			}
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
			this.xPositionOnScreen = Game1.viewport.Width / 2 - 256 / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - 256;
			this.widthOnScreen = 256 + IClickableMenu.borderWidth * 2;
			this.heightOnScreen = 512 + IClickableMenu.borderWidth * 2;
			
			this.jojaLogo = logo;

			// Create the number-pad.
			for (int iy = 0; iy < 3; iy++)
			{
				for (int ix = 0; ix < 3; ix++)
				{
					int buttonWidth = ((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) - 2;
					this.numpad.Add
					(
						new ClickableComponent
						(
							new Rectangle
							(
								this.xPositionOnScreen
									+ 40
									+ buttonWidth * ix,
								this.yPositionOnScreen
									+ IClickableMenu.borderWidth
									+ IClickableMenu.spaceToClearTopBorder
									+ buttonWidth * iy
									+ (Game1.smallFont.LineSpacing * 4)
									+ (((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3) / 16,
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
				int xOffset = ((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3;
				this.opButtons.Add
				(
					new ClickableComponent
					(
						new Rectangle
						(
							this.xPositionOnScreen
								+ 40
								+ xOffset
								+ xOffset / 16,
							this.yPositionOnScreen
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
			this.clearButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 40,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2 - 1,
					Game1.smallFont.LineSpacing * 2
				),
				"clear",
				"Clear"
			);

			// Create the backspace button.
			this.backspaceButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 41
						+ (this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					Game1.smallFont.LineSpacing * 2
				),
				"backspace",
				"Back"
			);

			// Create the zero button.
			this.zeroButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 40,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ Game1.smallFont.LineSpacing * 2
						+ (this.widthOnScreen - IClickableMenu.borderWidth * 2)
						+ (((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3) / 16,
					((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3,
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
				this.jojaLogo,
				new Vector2
				(
					(float)this.xPositionOnScreen
						+ 40,
					(float)this.yPositionOnScreen
						+ this.heightOnScreen
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
				String.Concat(((this.currentInput) ? this.inputB : this.inputA), ((this.timer >= 16) ? "|" : "")),
				new Vector2(
					(float)this.xPositionOnScreen
						+ this.widthOnScreen
						- 40
						- Game1.smallFont.MeasureString((this.currentInput) ? this.inputB + "|" : this.inputA + "|").X,
					(float)this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
				),
				Game1.textColor
			);

			// Draw the previous input, if currently entering the second number.
			if (this.currentInput)
			{
				string prevInput = this.inputA 
					+ " "
					+ (this.op switch {
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
						(float)this.xPositionOnScreen
							+ this.widthOnScreen
							- 40
							- Game1.smallFont.MeasureString(prevInput).X,
						(float)this.yPositionOnScreen
							+ IClickableMenu.borderWidth
							+ IClickableMenu.spaceToClearSideBorder
							+ Game1.smallFont.LineSpacing * 2
					),
					Game1.textShadowColor
				);
			}

			// Draw the number pad.
			foreach(ClickableComponent button in this.numpad)
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
			foreach(ClickableComponent button in this.opButtons)
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
				this.clearButton.bounds.X,
				this.clearButton.bounds.Y,
				this.clearButton.bounds.Width,
				this.clearButton.bounds.Height,
				(this.clearButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			b.DrawString
			(
				Game1.smallFont,
				this.clearButton.label,
				new Vector2
				(
					(float)this.clearButton.bounds.X
						+ (this.clearButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.clearButton.label).X / 2),
					(float)this.clearButton.bounds.Y
						+ (this.clearButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.clearButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the backspace button.
			IClickableMenu.drawTextureBox
			(
				b,
				Game1.mouseCursors,
				new Rectangle(432, 439, 9, 9),
				this.backspaceButton.bounds.X,
				this.backspaceButton.bounds.Y,
				this.backspaceButton.bounds.Width,
				this.backspaceButton.bounds.Height,
				(this.backspaceButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			b.DrawString
			(
				Game1.smallFont,
				this.backspaceButton.label,
				new Vector2
				(
					(float)this.backspaceButton.bounds.X
						+ (this.backspaceButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.backspaceButton.label).X / 2),
					(float)this.backspaceButton.bounds.Y
						+ (this.backspaceButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.backspaceButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the zero button.
			IClickableMenu.drawTextureBox
			(
				b,
				Game1.mouseCursors,
				new Rectangle(432, 439, 9, 9),
				this.zeroButton.bounds.X,
				this.zeroButton.bounds.Y,
				this.zeroButton.bounds.Width,
				this.zeroButton.bounds.Height,
				(this.zeroButton.scale != 1.0001f) ? Color.Wheat : Color.White,
				4f,
				false
			);
			Utility.drawBoldText
			(
				b,
				this.zeroButton.label,
				Game1.smallFont,
				new Vector2
				(
					(float)this.zeroButton.bounds.X
						+ (this.zeroButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.zeroButton.label).X / 2),
					(float)this.zeroButton.bounds.Y
						+ (this.zeroButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.zeroButton.name).Y / 2)
				),
				Game1.textColor,
				1f,
				-1f,
				2
			);

			if (this.shouldDrawCloseButton()) base.draw(b);
			if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			
			this.timer++;
			if (this.timer >= 32) this.timer = 0;
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
				if (this.currentInput)
					this.inputB += keyChar;
				else
					this.inputA += keyChar;
			}
			else 
			{
				switch (key) 
				{
					case Keys.Add:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Add;
						break;
					case Keys.Subtract:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Subtract;
						break;
					case Keys.Multiply:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Multiply;
						break;
					case Keys.Divide:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Divide;
						break;
					case Keys.Enter:
						this.DoCalculation();
						break;
					case Keys.Delete:
						this.result = 0;
						this.inputA = "";
						this.inputB = "";
						this.currentInput = false;
						break;
					case Keys.Back:
						if (!this.currentInput)
						{
							if (this.inputA.Length >= 1) this.inputA = this.inputA.Substring(0, this.inputA.Length - 1);
						}
						else
						{
							if (this.inputB.Length >= 1) this.inputB = this.inputB.Substring(0, this.inputB.Length - 1);
						}
						break;
					case Keys.Escape:
						this.exitThisMenu();
						break;
				}
			}
		}

		public override void performHoverAction(int x, int y)
		{
			foreach(ClickableComponent button in this.numpad)
			{
				if (button.containsPoint(x,y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			foreach(ClickableComponent button in this.opButtons)
			{
				if (button.containsPoint(x,y))
					button.scale = 1.0001f;
				else
					button.scale = 1f;
			}
			if (this.zeroButton.containsPoint(x,y))
				this.zeroButton.scale = 1.0001f;
			else
				this.zeroButton.scale = 1f;
			if (this.clearButton.containsPoint(x,y))
				this.clearButton.scale = 1.0001f;
			else
				this.clearButton.scale = 1f;
			if (this.backspaceButton.containsPoint(x,y))
				this.backspaceButton.scale = 1.0001f;
			else
				this.backspaceButton.scale = 1f;
		}

		public override void receiveLeftClick(int x, int y, bool playSound)
		{
			foreach(ClickableComponent button in this.numpad)
			{
				if (button.containsPoint(x, y))
				{
					if (this.currentInput)
						this.inputB += button.name;
					else
						this.inputA += button.name;
					Game1.playSound("smallSelect");
				}
			}
			foreach(ClickableComponent button in this.opButtons)
			{
				if (button.containsPoint(x, y))
				{
					switch(button.name)
					{
						case "+":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Add;
							break;
						case "-":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Subtract;
							break;
						case "X":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Multiply;
							break;
						case "/":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Divide;
							break;
						case "EQ":
							this.DoCalculation();
							break;
						case ".":
							if (!this.currentInput)
								this.inputA += ".";
							else
								this.inputB += ".";
							break;
					}
					Game1.playSound("smallSelect");
				}
			}
			if (this.zeroButton.containsPoint(x, y))
			{
				if (!this.currentInput)
					this.inputA += "0";
				else
					this.inputB += "0";
				Game1.playSound("smallSelect");
			}
			if (this.clearButton.containsPoint(x, y))
			{
				this.result = 0;
				this.inputA = "";
				this.inputB = "";
				this.currentInput = false;
				Game1.playSound("backpackIN");
			}
			if (this.backspaceButton.containsPoint(x, y))
			{
				if (!this.currentInput)
				{
					if (this.inputA.Length >= 1) this.inputA = this.inputA.Substring(0, this.inputA.Length - 1);
				}
				else
				{
					if (this.inputB.Length >= 1) this.inputB = this.inputB.Substring(0, this.inputB.Length - 1);
				}
				Game1.playSound("smallSelect");
			}
		}

		private void DoCalculation()
		{
			if (!this.currentInput) 
			{
				double.TryParse(this.inputA, out this.result);
				this.currentInput = false;
			}
			else
			{
				double inputA, inputB;
				double.TryParse(this.inputA, out inputA);
				double.TryParse(this.inputB, out inputB);
				switch (this.op)
				{
					case Operation.Add:
						this.result = inputA + inputB;
						break;
					case Operation.Subtract:
						this.result = inputA - inputB;
						break;
					case Operation.Multiply:
						this.result = inputA * inputB;
						break;
					case Operation.Divide:
						this.result = inputA / inputB;
						break;
					case Operation.None:
						break;
				}
				this.inputA = this.result.ToString();
				if (this.inputA == "NaN") this.inputA = "";
				this.inputB = "";
				this.currentInput = false;
			}
		}
	}
}
