/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace DynamicBodies.UI
{
    internal class TabbedTailoringMenu : TailoringMenu
    {

        public Texture2D moreTailoringTextures;
        public const int region_tabmake = 50, region_tabdye = 52, region_tabedit = 51;
        public Color bgColor;
        public ClickableComponent makeTab;
		public ClickableComponent dyeTab;
		public ClickableComponent editTab;
        public TabbedTailoringMenu() : base()
        {
            moreTailoringTextures = Game1.temporaryContent.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/more_tailoring.png");

            Color[] data = new Color[moreTailoringTextures.Width * moreTailoringTextures.Height];
            moreTailoringTextures.GetData(data, 0, data.Length);
            //get the colour from the tab image rather than hardcode
            bgColor = data[moreTailoringTextures.Width * 2 + -6];

            makeTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 64, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Make", "Sewing Machine")
            {
                myID = region_tabmake,
                downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            };
			dyeTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 128, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Edit", "Adjusting Tools")
			{
				myID = region_tabedit,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			};
			editTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 192, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Edit", "Adjusting Tools")
            {
                myID = region_tabedit,
                downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            };

			//snap to tabs
			rightIngredientSpot.upNeighborID = region_tabedit;
			blankRightIngredientSpot.upNeighborID = region_tabdye;
			equipmentIcons.First().upNeighborID = region_tabmake;

			if (Game1.options.SnappyMenus)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //Add tab action
            if(editTab.containsPoint(x, y))
            {
				//return any items etc
				cleanupBeforeExit();

				Game1.playSound("smallSelect");
                Game1.activeClickableMenu = new ClothingModifier();
                return;
            }

			//Add tab action
			if (dyeTab.containsPoint(x, y))
			{
				//return items etc
				cleanupBeforeExit();

				Game1.playSound("smallSelect");
				Game1.activeClickableMenu = new TabbedDyeMenu();
				return;
			}

			base.receiveLeftClick(x, y, playSound);
        }

        //Rewrite of the rendering, mostly the same but uses dynamic color now and includes tabs
        public override void draw(SpriteBatch b)
        {
            
			//Tint screen
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
			//Draw trees in Haley/Emily's place
			if (Game1.currentLocation.Name == "HaleyHouse")
			{
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 96f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 352f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 608f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 256f, base.yPositionOnScreen), new Rectangle(79, 97, 22, 20), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 512f, base.yPositionOnScreen), new Rectangle(79, 97, 22, 20), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 32f, base.yPositionOnScreen + 44), new Rectangle(81, 81, 16, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 768f, base.yPositionOnScreen + 44), new Rectangle(81, 81, 16, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			}

			
			Game1.DrawBox(base.xPositionOnScreen - 64, base.yPositionOnScreen + 128, 128, 265, bgColor);
			Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float)(base.xPositionOnScreen - 64) + 9.6f, base.yPositionOnScreen + 128), 0.87f, 4f, 2, Game1.player);
			base.draw(b, drawUpperPortion: true, drawDescriptionArea: true, bgColor.R, bgColor.G, bgColor.B);
			b.Draw(this.tailoringTextures, new Vector2(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder), new Rectangle(0, 0, 142, 80), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);

			//tab for this
			b.Draw(moreTailoringTextures, new Vector2(makeTab.bounds.X, makeTab.bounds.Y + 8), new Rectangle(64, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for dyeing
			b.Draw(moreTailoringTextures, new Vector2(dyeTab.bounds.X, dyeTab.bounds.Y), new Rectangle(64, 32, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for editing
			b.Draw(moreTailoringTextures, new Vector2(editTab.bounds.X, editTab.bounds.Y), new Rectangle(64, 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);


			this.startTailoringButton.draw(b, Color.White, 0.96f);
			this.startTailoringButton.drawItem(b, 16, 16);
			this.presserSprite.draw(b, Color.White, 0.99f);
			this.needleSprite.draw(b, Color.White, 0.97f);
			Point random_shaking = new Point(0, 0);
			if (!this.IsBusy())
			{
				if (this.leftIngredientSpot.item != null)
				{
					this.blankLeftIngredientSpot.draw(b);
				}
				else
				{
					this.leftIngredientSpot.draw(b, Color.White, 0.87f, (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
				}
			}
			else
			{
				random_shaking.X = Game1.random.Next(-1, 2);
				random_shaking.Y = Game1.random.Next(-1, 2);
			}
			this.leftIngredientSpot.drawItem(b, (4 + random_shaking.X) * 4, (4 + random_shaking.Y) * 4);
			if (this.craftResultDisplay.visible)
			{
				string make_result_text = Game1.content.LoadString("Strings\\UI:Tailor_MakeResult");
				Utility.drawTextWithColoredShadow(position: new Vector2((float)this.craftResultDisplay.bounds.Center.X - Game1.smallFont.MeasureString(make_result_text).X / 2f, (float)this.craftResultDisplay.bounds.Top - Game1.smallFont.MeasureString(make_result_text).Y), b: b, text: make_result_text, font: Game1.smallFont, color: Game1.textColor * 0.75f, shadowColor: Color.Black * 0.2f);
				this.craftResultDisplay.draw(b);
				if (this.craftResultDisplay.item != null)
				{
					if (this._isMultipleResultCraft)
					{
						Rectangle question_mark_bounds2 = this.craftResultDisplay.bounds;
						question_mark_bounds2.X += 6;
						question_mark_bounds2.Y -= 8 + (int)this.questionMarkOffset.Y;
						b.Draw(this.tailoringTextures, question_mark_bounds2, new Rectangle(112, 208, 16, 16), Color.White);
					}
					else if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
					{
						this.craftResultDisplay.drawItem(b);
					}
					else
					{
						if (this.craftResultDisplay.item is Hat)
						{
							b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle(96, 208, 16, 16), Color.White);
						}
						else if (this.craftResultDisplay.item is Clothing)
						{
							if ((this.craftResultDisplay.item as Clothing).clothesType.Value == 1)
							{
								b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle(64, 208, 16, 16), Color.White);
							}
							else if ((this.craftResultDisplay.item as Clothing).clothesType.Value == 0)
							{
								b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle(80, 208, 16, 16), Color.White);
							}
						}
						else if (this.craftResultDisplay.item is Object crafted_object && Utility.IsNormalObjectAtParentSheetIndex(crafted_object, 71))
						{
							b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle(64, 208, 16, 16), Color.White);
						}
						Rectangle question_mark_bounds = this.craftResultDisplay.bounds;
						question_mark_bounds.X += 24;
						question_mark_bounds.Y += 12 + (int)this.questionMarkOffset.Y;
						b.Draw(this.tailoringTextures, question_mark_bounds, new Rectangle(112, 208, 16, 16), Color.White);
					}
				}
			}
			foreach (ClickableComponent c in this.equipmentIcons)
			{
				switch (c.name)
				{
					case "Hat":
						if (Game1.player.hat.Value != null)
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(0, 208, 16, 16), Color.White);
							float transparency = 1f;
							if (!this.HighlightItems((Hat)Game1.player.hat))
							{
								transparency = 0.5f;
							}
							if (Game1.player.hat.Value == base.heldItem)
							{
								transparency = 0.5f;
							}
							Game1.player.hat.Value.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale, transparency, 0.866f, StackDrawType.Hide);
						}
						else
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(48, 208, 16, 16), Color.White);
						}
						break;
					case "Shirt":
						if (Game1.player.shirtItem.Value != null)
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(0, 208, 16, 16), Color.White);
							float transparency2 = 1f;
							if (!this.HighlightItems((Clothing)Game1.player.shirtItem))
							{
								transparency2 = 0.5f;
							}
							if (Game1.player.shirtItem.Value == base.heldItem)
							{
								transparency2 = 0.5f;
							}
							Game1.player.shirtItem.Value.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale, transparency2, 0.866f);
						}
						else
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(32, 208, 16, 16), Color.White);
						}
						break;
					case "Pants":
						if (Game1.player.pantsItem.Value != null)
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(0, 208, 16, 16), Color.White);
							float transparency3 = 1f;
							if (!this.HighlightItems((Clothing)Game1.player.pantsItem))
							{
								transparency3 = 0.5f;
							}
							if (Game1.player.pantsItem.Value == base.heldItem)
							{
								transparency3 = 0.5f;
							}
							Game1.player.pantsItem.Value.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale, transparency3, 0.866f);
						}
						else
						{
							b.Draw(this.tailoringTextures, c.bounds, new Rectangle(16, 208, 16, 16), Color.White);
						}
						break;
				}
			}
			if (!this.IsBusy())
			{
				if (this.rightIngredientSpot.item != null)
				{
					this.blankRightIngredientSpot.draw(b);
				}
				else
				{
					this.rightIngredientSpot.draw(b, Color.White, 0.87f, (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
				}
			}
			this.rightIngredientSpot.drawItem(b, 16, (4 + (int)this._rightItemOffset) * 4);
			if (!base.hoverText.Equals(""))
			{
				IClickableMenu.drawHoverText(b, base.hoverText, Game1.smallFont, (base.heldItem != null) ? 32 : 0, (base.heldItem != null) ? 32 : 0);
			}
			else if (base.hoveredItem != null)
			{
				IClickableMenu.drawToolTip(b, base.hoveredItem.getDescription(), base.hoveredItem.DisplayName, base.hoveredItem, base.heldItem != null);
			}
			if (base.heldItem != null)
			{
				base.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
			}
			if (!Game1.options.hardwareCursor)
			{
				base.drawMouse(b);
			}
		}
    }
}
