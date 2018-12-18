using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System.Collections.Generic;
using System.Linq;
using System;
using Revitalize.Objects;

namespace Revitalize.Menus
{
    public class LightCustomizer : IClickableMenu
	{
		public const int colorPickerTimerDelay = 100;

		private int currentShirt;

		private int currentHair;

		private int currentAccessory;

		private int colorPickerTimer;

		//private ColorPicker pantsColorPicker;

		//private ColorPicker hairColorPicker;

		private ColorPicker lightColorPicker;

		private List<ClickableComponent> labels = new List<ClickableComponent>();

		private List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();

		private List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();


        public bool colorChanged;


		

		private ClickableTextureComponent okButton;

        private ClickableTextureComponent cancelButton;

        private ClickableTextureComponent skipIntroButton;

		private ClickableTextureComponent randomButton;

		private TextBox nameBox;

		private TextBox farmnameBox;

		private TextBox favThingBox;

		private bool skipIntro;

		private bool wizardSource;

		private string hoverText;

		private string hoverTitle;

		private ClickableComponent nameLabel;

		private ClickableComponent farmLabel;

		private ClickableComponent favoriteLabel;

		private ClickableComponent shirtLabel;

		private ClickableComponent skinLabel;

		private ClickableComponent hairLabel;

		private ClickableComponent accLabel;

		private ColorPicker lastHeldColorPicker;

        public Light LightObject; 

        private int timesRandom;

        public bool once;

		public LightCustomizer( Light Obj, bool wizardSource = false) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
		{
			
			this.wizardSource = wizardSource;
			this.setUpPositions();
			Game1.player.faceDirection(2);
			Game1.player.FarmerSprite.StopAnimation();
            this.LightObject = Obj;
            colorChanged = false;
            this.height = this.height / 3;
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
			this.setUpPositions();
		}

		private void setUpPositions()
		{
            
			this.labels.Clear();

			this.leftSelectionButtons.Clear();
			this.rightSelectionButtons.Clear();
			this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4)/3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            
            this.cancelButton = new ClickableTextureComponent("Cancel", new Rectangle(this.xPositionOnScreen +this.width/4  - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4)/3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            /*
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
				Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4,
				Text = Game1.player.name
			};
			this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name", new object[0])));
			this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
				Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize,
				Text = Game1.player.farmName
			};
			this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm", new object[0])));
			this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
				Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize * 2,
				Text = Game1.player.favoriteThing
			};
			this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize * 2, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing", new object[0])));
			*/
            this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.pixelZoom * 12, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false);
			int num = Game1.tileSize * 2;
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
			if (!this.wizardSource)
			{
				//this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal", new object[0])));
			}
            //	this.petButtons.Add(new ClickableTextureComponent("Cat", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 6 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, "Cat", Game1.mouseCursors, new Rectangle(160, 192, 16, 16), (float)Game1.pixelZoom, false));
            //	this.petButtons.Add(new ClickableTextureComponent("Dog", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, "Dog", Game1.mouseCursors, new Rectangle(176, 192, 16, 16), (float)Game1.pixelZoom, false));
            //	this.genderButtons.Add(new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), (float)Game1.pixelZoom, false));
            //	this.genderButtons.Add(new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + Game1.tileSize + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), (float)Game1.pixelZoom, false));
            //	num = Game1.tileSize * 4 + 8;
            //	this.leftSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            //	this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4 + Game1.tileSize + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin", new object[0])));
            //	this.rightSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            /*
            if (!this.wizardSource)
			{
				Point point = new Point(this.xPositionOnScreen + this.width + Game1.pixelZoom + Game1.tileSize / 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
				this.farmTypeButtons.Add(new ClickableTextureComponent("Standard", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmStandard", new object[0]), Game1.mouseCursors, new Rectangle(0, 324, 22, 20), (float)Game1.pixelZoom, false));
				this.farmTypeButtons.Add(new ClickableTextureComponent("Riverland", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 2, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmFishing", new object[0]), Game1.mouseCursors, new Rectangle(22, 324, 22, 20), (float)Game1.pixelZoom, false));
				this.farmTypeButtons.Add(new ClickableTextureComponent("Forest", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 3, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmForaging", new object[0]), Game1.mouseCursors, new Rectangle(44, 324, 22, 20), (float)Game1.pixelZoom, false));
				this.farmTypeButtons.Add(new ClickableTextureComponent("Hills", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 4, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmMining", new object[0]), Game1.mouseCursors, new Rectangle(66, 324, 22, 20), (float)Game1.pixelZoom, false));
				this.farmTypeButtons.Add(new ClickableTextureComponent("Wilderness", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 5, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmCombat", new object[0]), Game1.mouseCursors, new Rectangle(88, 324, 22, 20), (float)Game1.pixelZoom, false));
			}
            */
			this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 16, 1, 1), "LightColor"));
			this.lightColorPicker = new ColorPicker(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder );
            

         

          

           // this.lightColorPicker.setColor(LightObject.lightColor);
            
			num += Game1.tileSize + 8;
            //	this.leftSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            //	this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair", new object[0])));
            //	this.rightSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            //	this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor", new object[0])));
            //	this.hairColorPicker = new ColorPicker(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num);
            //	this.hairColorPicker.setColor(Game1.player.hairstyleColor);
            //	num += Game1.tileSize + 8;
            //	this.leftSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            //	this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt", new object[0])));
            //	this.rightSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            //	this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor", new object[0])));
            //	this.pantsColorPicker = new ColorPicker(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num);
            //	this.pantsColorPicker.setColor(Game1.player.pantsColor);
            //this.skipIntroButton = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 - Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + Game1.tileSize * 5 / 4, Game1.pixelZoom * 9, Game1.pixelZoom * 9), null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro", new object[0]), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), (float)Game1.pixelZoom, false);
            //num += Game1.tileSize + 8;
            //this.leftSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            //this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory", new object[0])));
            //this.rightSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            once = false;
        }

		private void optionButtonClick(string name)
		{
           // Console.WriteLine(name);
			//uint num = Hash.ComputeStringHash(name);
            //Console.WriteLine(num);

							if (name == "Wilderness")
							{
								if (!this.wizardSource)
								{
									Game1.whichFarm = 4;
									Game1.spawnMonstersAtNight = true;
								}
							}
						
					
				if (name == "Standard")
					{
						if (!this.wizardSource)
						{
							Game1.whichFarm = 0;
							Game1.spawnMonstersAtNight = false;
						}
					}
				
							if (name == "Riverland")
							{
								if (!this.wizardSource)
								{
									Game1.whichFarm = 1;
									Game1.spawnMonstersAtNight = false;
								}
							}
						
					 if (name == "Dog")
					{
						if (!this.wizardSource)
						{
							Game1.player.catPerson = false;
						}
					}
				
				if (name == "Male")
				{
					if (!this.wizardSource)
					{
						Game1.player.changeGender(true);
						Game1.player.changeHairStyle(0);
					}
				}
            if (name == "Cancel")
            {
                Game1.exitActiveMenu();
                return;
            }

						if (name == "OK")
						{


                if (colorChanged == false)
                {
                    Game1.exitActiveMenu();
                   // StardewModdingAPI.Log.Info("HEY!");
                    return;
                }
              //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);

                this.lightColorPicker.setColor(LightObject.lightColor);

              //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);

                
                //UTIL FUNCTION TO GET CORRECT COLOR
                LightObject.lightColor = this.lightColorPicker.getSelectedColor();
                //LightObject.lightColor = Util.invertColor(LightObject.lightColor);

                this.LightObject.removeLights(this.LightObject.thisLocation);
                this.LightObject.addLights(this.LightObject.thisLocation,this.LightObject.lightColor);

                if (!this.canLeaveMenu())
							{
								return;
							}
							//Game1.player.Name = this.nameBox.Text.Trim();
						//	Game1.player.favoriteThing = this.favThingBox.Text.Trim();
							if (Game1.activeClickableMenu is TitleMenu)
							{
								(Game1.activeClickableMenu as TitleMenu).createdNewCharacter(this.skipIntro);
							}
							else
							{
								Game1.exitActiveMenu();
								if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
								{
									(Game1.currentMinigame as Intro).doneCreatingCharacter();
								}
								else if (this.wizardSource)
								{
									Game1.flashAlpha = 1f;
									Game1.playSound("yoba");
								}
							}
						}
			 if (name == "Cat")
				{
					if (!this.wizardSource)
					{
						Game1.player.catPerson = true;
					}
				}


            if (name == "Female")
            {
                if (!this.wizardSource)
                {
                    Game1.player.changeGender(false);
                    Game1.player.changeHairStyle(16);
                }
            }
            else if (name == "Hills")
            {
                if (!this.wizardSource)
                {
                    Game1.whichFarm = 3;
                    Game1.spawnMonstersAtNight = false;
                }
            }
			
			if (name == "Forest")
			{
				if (!this.wizardSource)
				{
					Game1.whichFarm = 2;
					Game1.spawnMonstersAtNight = false;
				}
			}
			Game1.playSound("coin");
		}

		private void selectionClick(string name, int change)
		{
			if (name == "Skin")
			{
				Game1.player.changeSkinColor(Game1.player.skin + change);
				Game1.playSound("skeletonStep");
				return;
			}
			if (name == "Hair")
			{
				Game1.player.changeHairStyle(Game1.player.hair + change);
				Game1.playSound("grassyStep");
				return;
			}
			if (name == "Shirt")
			{
				Game1.player.changeShirt(Game1.player.shirt + change);
				Game1.playSound("coin");
				return;
			}
			if (name == "Acc")
			{
				Game1.player.changeAccessory(Game1.player.accessory + change);
				Game1.playSound("purchase");
				return;
			}
			if (!(name == "Direction"))
			{
				return;
			}
			Game1.player.faceDirection((Game1.player.facingDirection - change + 4) % 4);
			Game1.player.FarmerSprite.StopAnimation();
			Game1.player.completelyStopAnimatingOrDoingAction();
			Game1.playSound("pickUpItem");
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
            /*
			foreach (ClickableComponent current in this.genderButtons)
			{
				if (current.containsPoint(x, y))
				{
					this.optionButtonClick(current.name);
					current.scale -= 0.5f;
					current.scale = Math.Max(3.5f, current.scale);
				}
                
			}
			foreach (ClickableComponent current2 in this.farmTypeButtons)
			{
            
				if (current2.containsPoint(x, y) && !current2.name.Contains("Gray"))
				{
					this.optionButtonClick(current2.name);
					current2.scale -= 0.5f;
					current2.scale = Math.Max(3.5f, current2.scale);
				}
                
			}
			foreach (ClickableComponent current3 in this.petButtons)
			{
            
				if (current3.containsPoint(x, y))
				{
					this.optionButtonClick(current3.name);
					current3.scale -= 0.5f;
					current3.scale = Math.Max(3.5f, current3.scale);
				}
                
			}
            */
			foreach (ClickableComponent current4 in this.leftSelectionButtons)
			{
				if (current4.containsPoint(x, y))
				{
					this.selectionClick(current4.name, -1);
					current4.scale -= 0.25f;
					current4.scale = Math.Max(0.75f, current4.scale);
				}
			}
			foreach (ClickableComponent current5 in this.rightSelectionButtons)
			{
				if (current5.containsPoint(x, y))
				{
					this.selectionClick(current5.name, 1);
					current5.scale -= 0.25f;
					current5.scale = Math.Max(0.75f, current5.scale);
				}
			}
			if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
			{
				this.optionButtonClick(this.okButton.name);
				this.okButton.scale -= 0.25f;
				this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
			}

            if (this.cancelButton.containsPoint(x, y))
            {
                this.optionButtonClick(this.cancelButton.name);
                this.cancelButton.scale -= 0.25f;
                this.cancelButton.scale = Math.Max(0.75f, this.cancelButton.scale);
            }
            //if (this.hairColorPicker.containsPoint(x, y))
            //{
            //Game1.player.changeHairColor(this.hairColorPicker.click(x, y));
            //this.lastHeldColorPicker = this.hairColorPicker;
            //}
            //	else if (this.pantsColorPicker.containsPoint(x, y))
            //	{
            //		
            //}
            else if (this.lightColorPicker.containsPoint(x, y))
			{

                LightObject.lightColor = this.lightColorPicker.click(x, y);
                LightObject.lightColor = Util.invertColor(LightObject.lightColor);
               // LightObject.lightColor = Util.invertColor(LightObject.lightColor);
                this.lastHeldColorPicker = this.lightColorPicker;
                colorChanged = true;
            }
			if (!this.wizardSource)
			{
				//this.nameBox.Update();
				//this.farmnameBox.Update();
				//this.favThingBox.Update();
				//if (this.skipIntroButton.containsPoint(x, y))
			//	{
				//	Game1.playSound("drumkit6");
					//this.skipIntroButton.sourceRect.X = ((this.skipIntroButton.sourceRect.X == 227) ? 236 : 227);
			//		this.skipIntro = !this.skipIntro;
			//	}
			}
			if (this.randomButton.containsPoint(x, y))
			{
				string cueName = "drumkit6";
				if (this.timesRandom > 0)
				{
					switch (Game1.random.Next(15))
					{
					case 0:
						cueName = "drumkit1";
						break;
					case 1:
						cueName = "dirtyHit";
						break;
					case 2:
						cueName = "axchop";
						break;
					case 3:
						cueName = "hoeHit";
						break;
					case 4:
						cueName = "fishSlap";
						break;
					case 5:
						cueName = "drumkit6";
						break;
					case 6:
						cueName = "drumkit5";
						break;
					case 7:
						cueName = "drumkit6";
						break;
					case 8:
						cueName = "junimoMeep1";
						break;
					case 9:
						cueName = "coin";
						break;
					case 10:
						cueName = "axe";
						break;
					case 11:
						cueName = "hammer";
						break;
					case 12:
						cueName = "drumkit2";
						break;
					case 13:
						cueName = "drumkit4";
						break;
					case 14:
						cueName = "drumkit3";
						break;
					}
				}
				Game1.playSound(cueName);
				this.timesRandom++;
				if (Game1.random.NextDouble() < 0.33)
				{
					if (Game1.player.isMale)
					{
					//	Game1.player.changeAccessory(Game1.random.Next(19));
					}
					else
					{
					//	Game1.player.changeAccessory(Game1.random.Next(6, 19));
					}
				}
				else
				{
				//	Game1.player.changeAccessory(-1);
				}
				if (Game1.player.isMale)
				{
					//Game1.player.changeHairStyle(Game1.random.Next(16));
				}
				else
				{
				//	Game1.player.changeHairStyle(Game1.random.Next(16, 32));
				}
				Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
				if (Game1.random.NextDouble() < 0.5)
				{
					c.R /= 2;
					c.G /= 2;
					c.B /= 2;
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					c.R = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					c.G = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					c.B = (byte)Game1.random.Next(15, 50);
				}
				//Game1.player.changeHairColor(c);
				//Game1.player.changeShirt(Game1.random.Next(112));
				//Game1.player.changeSkinColor(Game1.random.Next(6));
				if (Game1.random.NextDouble() < 0.25)
				{
					//Game1.player.changeSkinColor(Game1.random.Next(24));
				}
				Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
				if (Game1.random.NextDouble() < 0.5)
				{
					color.R /= 2;
					color.G /= 2;
					color.B /= 2;
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					color.R = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					color.G = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					color.B = (byte)Game1.random.Next(15, 50);
				}
				//Game1.player.changePants(color);
				Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
				c2.R /= 2;
				c2.G /= 2;
				c2.B /= 2;
				if (Game1.random.NextDouble() < 0.5)
				{
					c2.R = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					c2.G = (byte)Game1.random.Next(15, 50);
				}
				if (Game1.random.NextDouble() < 0.5)
				{
					c2.B = (byte)Game1.random.Next(15, 50);
				}
				//Game1.player.changeEyeColor(c2);
				this.randomButton.scale = (float)Game1.pixelZoom - 0.5f;


               // c2 = Util.invertColor(c2);
                colorChanged = true;
                this.lightColorPicker.setColor(c2);
                this.LightObject.lightColor =Util.invertColor( c2);
                this.LightObject.removeLights(this.LightObject.thisLocation);
                this.LightObject.addLights(this.LightObject.thisLocation, this.LightObject.lightColor);
                // LightObject.lightColor = Util.invertColor(LightObject.lightColor);
                //this.eyeColorPicker.setColor(Game1.player.newEyeColor);
                //this.hairColorPicker.setColor(Game1.player.hairstyleColor);
            }
		}

		public override void leftClickHeld(int x, int y)
		{
			this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
			if (this.colorPickerTimer <= 0)
			{
				if (this.lastHeldColorPicker != null)
				{
					
					if (this.lastHeldColorPicker.Equals(this.lightColorPicker))
					{
                        colorChanged = true;
                        this.LightObject.lightColor=Util.invertColor(this.lightColorPicker.clickHeld(x, y));
                        this.LightObject.removeLights(this.LightObject.thisLocation);
                        this.LightObject.addLights(this.LightObject.thisLocation, this.LightObject.lightColor);

                        //   LightObject.lightColor = Util.invertColor(LightObject.lightColor);
                    }
				}
				this.colorPickerTimer = 100;
			}
		}

		public override void releaseLeftClick(int x, int y)
		{
		
			this.lightColorPicker.releaseClick();
			this.lastHeldColorPicker = null;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		public override void receiveKeyPress(Keys key)
		{
			if (!this.wizardSource && key == Keys.Tab)
			{
				if (this.nameBox.Selected)
				{
					this.farmnameBox.SelectMe();
					this.nameBox.Selected = false;
					return;
				}
				if (this.farmnameBox.Selected)
				{
					this.farmnameBox.Selected = false;
					this.favThingBox.SelectMe();
					return;
				}
				this.favThingBox.Selected = false;
				this.nameBox.SelectMe();
			}
		}

		public override void performHoverAction(int x, int y)
		{
			this.hoverText = "";
			this.hoverTitle = "";
			using (List<ClickableComponent>.Enumerator enumerator = this.leftSelectionButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClickableTextureComponent clickableTextureComponent = (ClickableTextureComponent)enumerator.Current;
					if (clickableTextureComponent.containsPoint(x, y))
					{
						clickableTextureComponent.scale = Math.Min(clickableTextureComponent.scale + 0.02f, clickableTextureComponent.baseScale + 0.1f);
					}
					else
					{
						clickableTextureComponent.scale = Math.Max(clickableTextureComponent.scale - 0.02f, clickableTextureComponent.baseScale);
					}
				}
			}
			using (List<ClickableComponent>.Enumerator enumerator = this.rightSelectionButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClickableTextureComponent clickableTextureComponent2 = (ClickableTextureComponent)enumerator.Current;
					if (clickableTextureComponent2.containsPoint(x, y))
					{
						clickableTextureComponent2.scale = Math.Min(clickableTextureComponent2.scale + 0.02f, clickableTextureComponent2.baseScale + 0.1f);
					}
					else
					{
						clickableTextureComponent2.scale = Math.Max(clickableTextureComponent2.scale - 0.02f, clickableTextureComponent2.baseScale);
					}
				}
			}
			if (!this.wizardSource)
			{/*
				foreach (ClickableTextureComponent current in this.farmTypeButtons)
				{
                    
					if (current.containsPoint(x, y) && !current.name.Contains("Gray"))
					{
						current.scale = Math.Min(current.scale + 0.02f, current.baseScale + 0.1f);
						this.hoverTitle = current.hoverText.Split(new char[]
						{
							'_'
						})[0];
						this.hoverText = current.hoverText.Split(new char[]
						{
							'_'
						})[1];
					}
					else
					{
						current.scale = Math.Max(current.scale - 0.02f, current.baseScale);
						if (current.name.Contains("Gray") && current.containsPoint(x, y))
						{
							this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + current.name.Split(new char[]
							{
								'_'
							})[1], new object[0]) + " to unlock.";
						}
					}
				}
                */
			}
			if (!this.wizardSource)
			{/*
				using (List<ClickableComponent>.Enumerator enumerator = this.genderButtons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ClickableTextureComponent clickableTextureComponent3 = (ClickableTextureComponent)enumerator.Current;
						if (clickableTextureComponent3.containsPoint(x, y))
						{
							clickableTextureComponent3.scale = Math.Min(clickableTextureComponent3.scale + 0.02f, clickableTextureComponent3.baseScale + 0.1f);
						}
						else
						{
							clickableTextureComponent3.scale = Math.Max(clickableTextureComponent3.scale - 0.02f, clickableTextureComponent3.baseScale);
						}
					}
				}
				using (List<ClickableComponent>.Enumerator enumerator = this.petButtons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ClickableTextureComponent clickableTextureComponent4 = (ClickableTextureComponent)enumerator.Current;
						if (clickableTextureComponent4.containsPoint(x, y))
						{
							clickableTextureComponent4.scale = Math.Min(clickableTextureComponent4.scale + 0.02f, clickableTextureComponent4.baseScale + 0.1f);
						}
						else
						{
							clickableTextureComponent4.scale = Math.Max(clickableTextureComponent4.scale - 0.02f, clickableTextureComponent4.baseScale);
						}
					}
				}
                */
			}
            
			if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
			{
				this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
			}
			else
			{
				this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
			}


            if (this.cancelButton.containsPoint(x, y))
            {
                this.cancelButton.scale = Math.Min(this.cancelButton.scale + 0.02f, this.cancelButton.baseScale + 0.1f);
            }
            else
            {
                this.cancelButton.scale = Math.Max(this.cancelButton.scale - 0.02f, this.cancelButton.baseScale);
            }
            this.randomButton.tryHover(x, y, 0.25f);
			this.randomButton.tryHover(x, y, 0.25f);
		}

		public bool canLeaveMenu()
		{
			return this.wizardSource || (Game1.player.name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0);
		}

		public override void draw(SpriteBatch b)
		{
			Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
			//b.Draw(Game1.daybg, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4)), Color.White);
			//Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((float)(this.xPositionOnScreen - 2 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);
			if (!this.wizardSource)
			{/*
				using (List<ClickableComponent>.Enumerator enumerator = this.genderButtons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ClickableTextureComponent clickableTextureComponent = (ClickableTextureComponent)enumerator.Current;
						clickableTextureComponent.draw(b);
						if ((clickableTextureComponent.name.Equals("Male") && Game1.player.isMale) || (clickableTextureComponent.name.Equals("Female") && !Game1.player.isMale))
						{
							b.Draw(Game1.mouseCursors, clickableTextureComponent.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
						}
					}
				}
				using (List<ClickableComponent>.Enumerator enumerator = this.petButtons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ClickableTextureComponent clickableTextureComponent2 = (ClickableTextureComponent)enumerator.Current;
						clickableTextureComponent2.draw(b);
						if ((clickableTextureComponent2.name.Equals("Cat") && Game1.player.catPerson) || (clickableTextureComponent2.name.Equals("Dog") && !Game1.player.catPerson))
						{
							b.Draw(Game1.mouseCursors, clickableTextureComponent2.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
						}
					}
				}
                */
				//Game1.player.name = this.nameBox.Text;
			//	Game1.player.favoriteThing = this.favThingBox.Text;
				//Game1.player.farmName = this.farmnameBox.Text;
			}

			foreach (ClickableComponent current in this.labels)
			{
				string text = "";
				Color color = Game1.textColor;
				if (current == this.nameLabel)
				{
					color = ((Game1.player.name.Length < 1) ? Color.Red : Game1.textColor);
					if (this.wizardSource)
					{
						continue;
					}
				}
				else if (current == this.farmLabel)
				{
					color = ((Game1.player.farmName.Length < 1) ? Color.Red : Game1.textColor);
					if (this.wizardSource)
					{
						continue;
					}
				}
				else if (current == this.favoriteLabel)
				{
					color = ((Game1.player.favoriteThing.Length < 1) ? Color.Red : Game1.textColor);
					if (this.wizardSource)
					{
						continue;
					}
				}
				else if (current == this.shirtLabel)
				{
					text = string.Concat(Game1.player.shirt + 1);
				}
				else if (current == this.skinLabel)
				{
					text = string.Concat(Game1.player.skin + 1);
				}
				else if (current == this.hairLabel)
				{
					if (!current.name.Contains("Color"))
					{
						text = string.Concat(Game1.player.hair + 1);
					}
				}
				else if (current == this.accLabel)
				{
					text = string.Concat(Game1.player.accessory + 2);
				}
				else
				{
					color = Game1.textColor;
				}
				Utility.drawTextWithShadow(b, current.name, Game1.smallFont, new Vector2((float)current.bounds.X, (float)current.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
				if (text.Length > 0)
				{
					Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(current.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(current.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
				}
			}
			if (!this.wizardSource)
			{/*
				IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - Game1.pixelZoom * 4, this.farmTypeButtons[0].bounds.Y - Game1.pixelZoom * 5, 30 * Game1.pixelZoom, 110 * Game1.pixelZoom + Game1.pixelZoom * 9, Color.White);
				for (int i = 0; i < this.farmTypeButtons.Count; i++)
				{
					this.farmTypeButtons[i].draw(b, this.farmTypeButtons[i].name.Contains("Gray") ? (Color.Black * 0.5f) : Color.White, 0.88f);
					if (this.farmTypeButtons[i].name.Contains("Gray"))
					{
						b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[i].bounds.Center.X - Game1.pixelZoom * 3), (float)(this.farmTypeButtons[i].bounds.Center.Y - Game1.pixelZoom * 2)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
					}
					if (i == Game1.whichFarm)
					{
						IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[i].bounds.X, this.farmTypeButtons[i].bounds.Y - Game1.pixelZoom, this.farmTypeButtons[i].bounds.Width, this.farmTypeButtons[i].bounds.Height + Game1.pixelZoom * 2, Color.White, (float)Game1.pixelZoom, false);
					}
				}
                */
			}
            this.cancelButton.draw(b, Color.White, 0.75f);

			if (this.canLeaveMenu())
			{
				this.okButton.draw(b, Color.White, 0.75f);
			}
			else
			{
				this.okButton.draw(b, Color.White, 0.75f);
				this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
			}
		
			this.lightColorPicker.draw(b);
			if (!this.wizardSource)
			{
			//	this.nameBox.Draw(b);
			//	this.farmnameBox.Draw(b);
				////if (this.skipIntroButton != null)
				//{
				//	this.skipIntroButton.draw(b);
				//	Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro", new object[0]), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + Game1.pixelZoom * 2), (float)(this.skipIntroButton.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
				//}
				//Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix", new object[0]), Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + Game1.pixelZoom * 2), (float)(this.farmnameBox.Y + Game1.pixelZoom * 3)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
			//	this.favThingBox.Draw(b);
			}
			if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
			{
				IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
			}
			this.randomButton.draw(b);

            if (once == false)
            {
                Color c = Util.invertColor(LightObject.lightColor);

                this.lightColorPicker.setColor(c);
                once = true;
            }

			base.drawMouse(b);
		}
	}
}
