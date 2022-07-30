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
    internal class TabbedDyeMenu : DyeMenu
    {

        public Texture2D moreTailoringTextures;
        public const int region_tabmake = 50, region_tabdye = 52, region_tabedit = 51;
        public Color bgColor;
        public ClickableComponent makeTab;
		public ClickableComponent dyeTab;
		public ClickableComponent editTab;
        public TabbedDyeMenu() : base()
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
			if(this.dyePots.Count > 3)
            {
				this.dyePots[0].upNeighborID = region_tabmake;
				this.dyePots[1].upNeighborID = region_tabdye;
				this.dyePots[2].upNeighborID = region_tabedit;
			}

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
			if (makeTab.containsPoint(x, y))
			{
				//return items etc
				cleanupBeforeExit();

				Game1.playSound("smallSelect");
				Game1.activeClickableMenu = new TabbedTailoringMenu();
				return;
			}

			base.receiveLeftClick(x, y, playSound);
        }

        //Rewrite of the rendering, mostly the same but uses dynamic color now and includes tabs
        public override void draw(SpriteBatch b)
        {
            
			//Tint screen
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);

			
			//Draw menu
			base.draw(b, drawUpperPortion: true, drawDescriptionArea: true, bgColor.R, bgColor.G, bgColor.B);

			//tab for making
			b.Draw(moreTailoringTextures, new Vector2(makeTab.bounds.X, makeTab.bounds.Y), new Rectangle(64, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for this
			b.Draw(moreTailoringTextures, new Vector2(dyeTab.bounds.X, dyeTab.bounds.Y + 8), new Rectangle(64, 32, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for editing
			b.Draw(moreTailoringTextures, new Vector2(editTab.bounds.X, editTab.bounds.Y), new Rectangle(64, 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);


			b.Draw(this.dyeTexture, new Vector2(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder), new Rectangle(0, 0, 142, 80), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			for (int j = 0; j < this._slotDrawPositions.Count; j++)
			{
				if (j >= base.inventory.actualInventory.Count || base.inventory.actualInventory[j] == null || !this._highlightDictionary.ContainsKey(base.inventory.actualInventory[j]))
				{
					continue;
				}
				int index = this._highlightDictionary[base.inventory.actualInventory[j]];
				if (index >= 0)
				{
					Color color = this.GetColorForPot(index);
					if (this._hoveredPotIndex == -1 && this.HighlightItems(base.inventory.actualInventory[j]))
					{
						b.Draw(this.dyeTexture, this._slotDrawPositions[j], new Rectangle(32, 96, 32, 32), color, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
					}
				}
			}
			this.dyeButton.draw(b, Color.White * (this.CanDye() ? 1f : 0.55f), 0.96f);
			this.dyeButton.drawItem(b, 16, 16);
			string make_result_text = Game1.content.LoadString("Strings\\UI:DyePot_WillDye");
			Vector2 dyed_items_position = this._dyedClothesDisplayPosition;
			Utility.drawTextWithColoredShadow(position: new Vector2(dyed_items_position.X - Game1.smallFont.MeasureString(make_result_text).X / 2f, (float)(int)dyed_items_position.Y - Game1.smallFont.MeasureString(make_result_text).Y), b: b, text: make_result_text, font: Game1.smallFont, color: Game1.textColor * 0.75f, shadowColor: Color.Black * 0.2f);
			foreach (ClickableTextureComponent dyedClothesDisplay in this.dyedClothesDisplays)
			{
				dyedClothesDisplay.drawItem(b);
			}
			for (int i = 0; i < this.dyePots.Count; i++)
			{
				this.dyePots[i].drawItem(b, 0, -16);
				if (this._dyeDropAnimationFrames[i] >= 0)
				{
					Color color2 = this.GetColorForPot(i);
					b.Draw(this.dyeTexture, new Vector2(this.dyePots[i].bounds.X, this.dyePots[i].bounds.Y - 12), new Rectangle(this._dyeDropAnimationFrames[i] / 50 * 16, 128, 16, 16), color2, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				}
				this.dyePots[i].draw(b);
			}
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
