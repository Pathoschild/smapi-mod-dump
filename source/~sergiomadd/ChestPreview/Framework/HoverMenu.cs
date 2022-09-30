/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SObject = StardewValley.Object;
using StardewValley.Tools;
using StardewValley.Objects;
using MaddUtil;
using System.Reflection;

namespace ChestPreview.Framework
{
    public class HoverMenu : InventoryMenu
    {
        public float Scale { get; set; }
        public int SourceX { get; set; }
		public int SourceY { get; set; }
        public int ConnectorOffset { get; set; }

        public HoverMenu(int xPosition, int yPosition, int connectorOffset, bool playerInventory, IList<Item> actualInventory, highlightThisItem highlightMethod = null, int capacity = -1, int rows = 3, int horizontalGap = 0, int verticalGap = 0, bool drawSlots = true)
		: base(xPosition, yPosition, playerInventory, actualInventory, null, capacity, rows)
		{
			Scale = Conversor.GetCurrentSizeValue();
			SourceX = xPosition;
			SourceY = yPosition;
			this.width = (int)(width * Scale);
			this.height = (int)(height * Scale);
			this.xPositionOnScreen = (int)(this.xPositionOnScreen - width / 2);
			this.yPositionOnScreen = this.yPositionOnScreen - this.height * 2;
			ConnectorOffset = connectorOffset;
		}
		
		public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
		{
			int bWidth = this.width 
				+ (int)(IClickableMenu.spaceToClearSideBorder
				+ IClickableMenu.spaceToClearSideBorder*Scale
				);
			int bHeight = this.height 
				+ (int)((IClickableMenu.spaceToClearSideBorder 
				+ IClickableMenu.spaceToClearSideBorder * Scale));
			int bXPos = (int)(this.xPositionOnScreen 
				- (IClickableMenu.spaceToClearSideBorder * Scale) 
				- (IClickableMenu.spaceToClearSideBorder * Scale)/2) 
				;
			int bYPos = (int)(this.yPositionOnScreen 
				- (IClickableMenu.spaceToClearSideBorder * Scale)
				- (IClickableMenu.spaceToClearSideBorder * Scale)/2 - 4);

			//Screen limits
			bool inLimit = false;
			int viewportWidth = Game1.uiViewport.Width;
			//Left wall
			if (bXPos < 0)
			{
				int offset = xPositionOnScreen - bXPos;
				this.xPositionOnScreen = offset + (int)(IClickableMenu.spaceToClearSideBorder * Scale) + (int)((IClickableMenu.spaceToClearSideBorder * Scale) / 2);
				bXPos = offset;
				inLimit = true;
			}
			//Right wall
			else if (bXPos + bWidth > viewportWidth)
			{
				bXPos = viewportWidth - bWidth 
					- (int)((IClickableMenu.spaceToClearSideBorder));
				this.xPositionOnScreen = (int)(bXPos
					+ (IClickableMenu.spaceToClearSideBorder * Scale)
					+ (IClickableMenu.spaceToClearSideBorder * Scale) / 2);
				inLimit = true;
			}
			//Up wall
			if (bYPos < 0)
			{
				int offset = yPositionOnScreen - bYPos;
				this.yPositionOnScreen = offset + (int)(IClickableMenu.spaceToClearSideBorder * Scale) + (int)((IClickableMenu.spaceToClearSideBorder * Scale) / 2);
				bYPos = offset;
			}
			Game1.DrawBox(bXPos, bYPos, bWidth, bHeight);
			//Chest preview connector
			if (ModEntry.config.Connector)
            {
				Texture2D menu_texture = Game1.menuTexture;
				Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64);
				/*Inner texture
				sourceRect.X = 64;
				sourceRect.Y = 128;
				//Top
				sourceRect.X = 128;
				sourceRect.Y = 0;
				//Left
				sourceRect.X = 0;
				sourceRect.Y = 128;
				//Right
				sourceRect.X = 192;
				sourceRect.Y = 128;
				*/
			
				Microsoft.Xna.Framework.Color inner_color = Microsoft.Xna.Framework.Color.White;
				int xLine = bXPos + bWidth/2;
				int yLine = bYPos + bHeight + IClickableMenu.borderWidth/4+6;
				int wLine = 64;
				int hLine = SourceY - yLine + ConnectorOffset;
				int xOffSetL = -6;
				int xOffSetR = 6;
				int xOffSetFill = 29;
				int wFill = 6;

				if (inLimit)
				{
					xLine = SourceX;
				}
				Vector2 adaptedLine = new Vector2(xLine - wLine / 2, yLine);
				if (ModEntry.CurrentSize == Size.Small)
				{
					xOffSetL = -8;
					xOffSetR = 6;
					xOffSetFill = 28;
					wFill = 6;
				}
				else if (ModEntry.CurrentSize == Size.Big)
				{
					xOffSetL = -1;
					xOffSetR = 8;
					xOffSetFill = 33;
					wFill = 5;
				}
				else if (ModEntry.CurrentSize == Size.Huge)
				{
					xOffSetL = -2;
					xOffSetR = 10;
					xOffSetFill = 33;
					wFill = 6;
				}
				//Left
				sourceRect.X = 0;
				sourceRect.Y = 128;
				b.Draw(menu_texture, new Microsoft.Xna.Framework.Rectangle((int)adaptedLine.X+xOffSetL, (int)adaptedLine.Y, wLine, hLine), sourceRect, inner_color);
				//Right
				sourceRect.X = 192;
				sourceRect.Y = 128;
				b.Draw(menu_texture, new Microsoft.Xna.Framework.Rectangle((int)adaptedLine.X+xOffSetR, (int)adaptedLine.Y, wLine, hLine), sourceRect, inner_color);
				//Middle (fillin)
				sourceRect.X = 64;
				sourceRect.Y = 128;
				b.Draw(menu_texture, new Microsoft.Xna.Framework.Rectangle((int)adaptedLine.X+ xOffSetFill, (int)adaptedLine.Y, wFill, hLine), sourceRect, inner_color);
			}

			Color tint = ((red == -1) ? Color.White : new Color((int)Utility.Lerp(red, Math.Min(255, red + 150), 0.65f), (int)Utility.Lerp(green, Math.Min(255, green + 150), 0.65f), (int)Utility.Lerp(blue, Math.Min(255, blue + 150), 0.65f)));
			Texture2D texture = ((red == -1) ? Game1.menuTexture : Game1.uncoloredMenuTexture);
			if (this.drawSlots)
			{
				for (int l = 0; l < this.capacity; l++)
				{
					Vector2 toDraw = new Vector2(base.xPositionOnScreen + l % (this.capacity / this.rows) * (64*Scale) + this.horizontalGap * (l % (this.capacity / this.rows)),
						base.yPositionOnScreen + l / (this.capacity / this.rows) * ((64*Scale) + this.verticalGap) + (l / (this.capacity / this.rows) - 1) * 4 - ((l < this.capacity / this.rows && this.playerInventory && this.verticalGap == 0) ? 12 : 0));
					b.Draw(texture, toDraw, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), tint, 0f, Vector2.Zero, 1f*Scale, SpriteEffects.None, float.MaxValue-1);
				}
				for (int k = 0; k < this.capacity; k++)
				{
					Vector2 toDraw2 = new Vector2(
						base.xPositionOnScreen + k % (this.capacity / this.rows) * (64*Scale) 
						+ this.horizontalGap * (k % (this.capacity / this.rows)) * Scale - 16,
						base.yPositionOnScreen + k / (this.capacity / this.rows) * ((64*Scale) 
						+ this.verticalGap) + (k / (this.capacity / this.rows) - 1) * 4
						- ((k < this.capacity / this.rows && this.playerInventory && this.verticalGap == 0) ? 12 : 0) - 16);

					if (this.actualInventory.Count > k && this.actualInventory.ElementAt(k) != null)
					{
						bool highlight2 = this.highlightMethod(this.actualInventory[k]);
						if (this._iconShakeTimer.ContainsKey(k))
						{
							toDraw2 += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
						}
						DrawItem(b, this.actualInventory[k], (int)toDraw2.X, (int)toDraw2.Y, Scale, 1f, float.MaxValue);
					}
				}
			}
			
		}

		public void DrawItem(SpriteBatch spriteBatch, Item item, int x, int y, float scaleSize,  float transparency, float layer)
		{
			Vector2 position = new Vector2(x, y);
			int xOffSet = 0;
			int yOffSet = 0;
			MethodInfo drawInPreview = item.GetType().GetMethod(
				"drawInPreview",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				CallingConventions.Any,
				new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) },
				null);
			if (drawInPreview != null)
			{
				drawInPreview.Invoke(item, new object[] { spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false });
			}
			else if (item is SObject)
            {
				if (item is Furniture)
				{
					if (ModEntry.CurrentSize == Size.Small)
					{
						xOffSet = -3;
						yOffSet = -1;
					}
					else if (ModEntry.CurrentSize == Size.Medium)
					{
						xOffSet = 0;
						yOffSet = 0;
					}
					else if (ModEntry.CurrentSize == Size.Big)
					{
						xOffSet = 4;
						yOffSet = 4;
					}
					else if (ModEntry.CurrentSize == Size.Huge)
					{
						xOffSet = 7;
						yOffSet = 6;
					}
					position.X += xOffSet;
					position.Y += yOffSet;
					item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
				}
				else if (item is Wallpaper)
				{
					if (ModEntry.CurrentSize == Size.Small)
					{
						xOffSet = -2;
						yOffSet = -2;
					}
					else if (ModEntry.CurrentSize == Size.Medium)
					{
						xOffSet = 0;
						yOffSet = 0;
					}
					else if (ModEntry.CurrentSize == Size.Big)
					{
						xOffSet = 4;
						yOffSet = 4;
					}
					else if (ModEntry.CurrentSize == Size.Huge)
					{
						xOffSet = 6;
						yOffSet = 6;
					}
					position.X += xOffSet;
					position.Y += yOffSet;
					item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
				}
				else if((item as SObject).bigCraftable.Value)
                {
					if (NonVanillaCheck(item))
					{
						if (ModEntry.CurrentSize == Size.Small)
						{
							xOffSet = -2;
							yOffSet = -2;
						}
						else if (ModEntry.CurrentSize == Size.Medium)
						{
							xOffSet = 8;
							yOffSet = 8;
						}
						else if (ModEntry.CurrentSize == Size.Big)
						{
							xOffSet = 4;
							yOffSet = 4;
						}
						else if (ModEntry.CurrentSize == Size.Huge)
						{
							xOffSet = 6;
							yOffSet = 6;
						}
						position.X += xOffSet;
						position.Y += yOffSet;
						item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
					}
					else
                    {
						DrawInMenu.drawInMenuObject((item as SObject), spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
                    }
				}
				else
                {
					if (NonVanillaCheck(item))
                    {
						if (ModEntry.CurrentSize == Size.Small)
						{
							xOffSet = -2;
							yOffSet = -2;
						}
						else if (ModEntry.CurrentSize == Size.Medium)
						{
							xOffSet = 8;
							yOffSet = 8;
						}
						else if (ModEntry.CurrentSize == Size.Big)
						{
							xOffSet = 4;
							yOffSet = 4;
						}
						else if (ModEntry.CurrentSize == Size.Huge)
						{
							xOffSet = 6;
							yOffSet = 6;
						}
						position.X += xOffSet;
						position.Y += yOffSet;
						item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
					}
					else
                    {
						DrawInMenu.drawInMenuObject((item as SObject), spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
					}
				}
			}
			else if (item is Tool)
			{
				if (item is MeleeWeapon && (item as MeleeWeapon).type.TargetValue == 1)
				{
					if (NonVanillaCheck(item))
					{
						if (ModEntry.CurrentSize == Size.Small)
						{
							xOffSet = -3;
							yOffSet = -4;
						}
						else if (ModEntry.CurrentSize == Size.Medium)
						{
							xOffSet = 0;
							yOffSet = 0;
						}
						else if (ModEntry.CurrentSize == Size.Big)
						{
							xOffSet = 4;
							yOffSet = 4;
						}
						else if (ModEntry.CurrentSize == Size.Huge)
						{
							xOffSet = 6;
							yOffSet = 6;
						}
						position.X += xOffSet;
						position.Y += yOffSet;
						item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
					}
					else
                    {
						DrawInMenu.drawInMenuDagger((item as MeleeWeapon), spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
                    }
				}
				else if (item is WateringCan)
				{
					DrawInMenu.drawInMenuWateringCan((item as WateringCan), spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
				}
				else
				{
					if (ModEntry.CurrentSize == Size.Small)
					{
						xOffSet = -3;
						yOffSet = -4;
					}
					else if (ModEntry.CurrentSize == Size.Medium)
					{
						xOffSet = 0;
						yOffSet = 0;
					}
					else if (ModEntry.CurrentSize == Size.Big)
					{
						xOffSet = 4;
						yOffSet = 4;
					}
					else if (ModEntry.CurrentSize == Size.Huge)
					{
						xOffSet = 6;
						yOffSet = 6;
					}
					position.X += xOffSet;
					position.Y += yOffSet;
					item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
				}
			}
			else if (item is Boots)
			{
				if (ModEntry.CurrentSize == Size.Small)
				{
					xOffSet = 3;
					yOffSet = 4;
				}
				else if (ModEntry.CurrentSize == Size.Medium)
				{
					xOffSet = 0;
					yOffSet = 0;
				}
				else if (ModEntry.CurrentSize == Size.Big)
				{
					xOffSet = 0;
					yOffSet = 0;
				}
				else if (ModEntry.CurrentSize == Size.Huge)
				{
					xOffSet = -2;
					yOffSet = 0;
				}
				position.X += xOffSet;
				position.Y += yOffSet;
				if (NonVanillaCheck(item))
				{
					item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
				}
				else
                {
					DrawInMenu.drawInMenuBoots((item as Boots), spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
                }
			}
			else if (item is Ring)
			{
				if (ModEntry.CurrentSize == Size.Small)
				{
					xOffSet = 8;
					yOffSet = 10;
				}
				else if (ModEntry.CurrentSize == Size.Medium)
				{
					xOffSet = 6;
					yOffSet = 8;
				}
				else if (ModEntry.CurrentSize == Size.Big)
				{
					xOffSet = 6;
					yOffSet = 10;
				}
				else if (ModEntry.CurrentSize == Size.Huge)
				{
					xOffSet = 8;
					yOffSet = 8;
				}
				position.X += xOffSet;
				position.Y += yOffSet;
				item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
			}
			else if (item is Hat)
			{
				if (ModEntry.CurrentSize == Size.Small)
				{
					xOffSet = -3;
					yOffSet = -2;
				}
				else if (ModEntry.CurrentSize == Size.Medium)
				{
					xOffSet = 0;
					yOffSet = 0;
				}
				else if (ModEntry.CurrentSize == Size.Big)
				{
					xOffSet = 4;
					yOffSet = 2;
				}
				else if (ModEntry.CurrentSize == Size.Huge)
				{
					xOffSet = 8;
					yOffSet = 8;
				}
				position.X += xOffSet;
				position.Y += yOffSet;
				item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
			}
			else if (item is Clothing)
			{
				if (ModEntry.CurrentSize == Size.Small)
				{
					xOffSet = -3;
					yOffSet = -3;
				}
				else if (ModEntry.CurrentSize == Size.Medium)
				{
					xOffSet = 0;
					yOffSet = 0;
				}
				else if (ModEntry.CurrentSize == Size.Big)
				{
					xOffSet = 4;
					yOffSet = 4;
				}
				else if (ModEntry.CurrentSize == Size.Huge)
				{
					xOffSet = 8;
					yOffSet = 8;
				}
				position.X += xOffSet;
				position.Y += yOffSet;
				item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
			}
			else
            {
				item.drawInMenu(spriteBatch, position, scaleSize, transparency, layer, StackDrawType.Draw, Color.White, false);
			}
		}

		public bool NonVanillaCheck(Item item)
        {
			bool nonVanilla = false;
			//Is item DGA
			if(ModEntry.DGAAPI != null && ModEntry.DGAAPI.GetDGAItemId(item) != null)
            {
				nonVanilla = true;
			}
			return nonVanilla;
        }
	}
}
