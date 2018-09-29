using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiggerBackpack
{
    class NewInventoryPage : InventoryPage
    {
        public NewInventoryPage(int x, int y, int width, int height) : base(x, y, width, height+ Game1.tileSize)
        {
            inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth, true, (IList<Item>)null, (InventoryMenu.highlightThisItem)null, 48, 4, 0, 0, true);
            inventory.capacity = 48;
            inventory.rows = 4;

            foreach ( var icon in equipmentIcons )
            {
                icon.bounds.Y += Game1.tileSize;
            }
        }

        public override void draw(SpriteBatch b)
        {
            var trashCanLidRotation = Mod.instance.Helper.Reflection.GetField<float>(this, "trashCanLidRotation").GetValue();
            //var heldItem = Mod.instance.Helper.Reflection.GetField<Item>(this, "heldItem").GetValue();
            var hoveredItem = Mod.instance.Helper.Reflection.GetField<Item>(this, "hoveredItem").GetValue();
            var hoverTitle = Mod.instance.Helper.Reflection.GetField<string>(this, "hoverTitle").GetValue();
            var hoverText = Mod.instance.Helper.Reflection.GetField<string>(this, "hoverText").GetValue();

            this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 * Game1.tileSize, false);
            this.inventory.draw(b);
            foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
            {
                string name = equipmentIcon.name;
                if (!(name == "Hat"))
                {
                    if (!(name == "Right Ring"))
                    {
                        if (!(name == "Left Ring"))
                        {
                            if (name == "Boots")
                            {
                                if (Game1.player.boots.Value != null)
                                {
                                    b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                                    Game1.player.boots.Value.drawInMenu(b, new Vector2((float)equipmentIcon.bounds.X, (float)equipmentIcon.bounds.Y), equipmentIcon.scale);
                                }
                                else
                                    b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 40, -1, -1)), Color.White);
                            }
                        }
                        else if (Game1.player.leftRing.Value != null)
                        {
                            b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                            Game1.player.leftRing.Value.drawInMenu(b, new Vector2((float)equipmentIcon.bounds.X, (float)equipmentIcon.bounds.Y), equipmentIcon.scale);
                        }
                        else
                            b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41, -1, -1)), Color.White);
                    }
                    else if (Game1.player.rightRing.Value != null)
                    {
                        b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                        Game1.player.rightRing.Value.drawInMenu(b, new Vector2((float)equipmentIcon.bounds.X, (float)equipmentIcon.bounds.Y), equipmentIcon.scale);
                    }
                    else
                        b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 41, -1, -1)), Color.White);
                }
                else if (Game1.player.hat.Value != null)
                {
                    b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White);
                    Game1.player.hat.Value.drawInMenu(b, new Vector2((float)equipmentIcon.bounds.X, (float)equipmentIcon.bounds.Y), equipmentIcon.scale, 1f, 0.866f, false);
                }
                else
                    b.Draw(Game1.menuTexture, equipmentIcon.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 42, -1, -1)), Color.White);
            }
            b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize - 8)), Color.White);
            Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, Game1.player.bathingClothes.Value ? 108 : 0, false, false, (AnimatedSprite.endOfAnimationBehavior)null, false), Game1.player.bathingClothes.Value ? 108 : 0, new Rectangle(0, Game1.player.bathingClothes.Value ? 576 : 0, 16, 32), new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 6 * Game1.tileSize - Game1.tileSize / 2)), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Game1.player);
            if (Game1.timeOfDay >= 1900)
                Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(0, 0, false, false, (AnimatedSprite.endOfAnimationBehavior)null, false), 0, new Rectangle(0, 0, 16, 32), new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 6 * Game1.tileSize - Game1.tileSize / 2)), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Game1.player);
            Utility.drawTextWithShadow(b, Game1.player.Name, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3) - Math.Min((float)Game1.tileSize, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 8 * Game1.tileSize + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text1 = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", (object)Game1.player.farmName.Value);
            Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text1).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 5 * Game1.tileSize + Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text2 = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds", (object)Utility.getNumberWithCommas(Game1.player.Money));
            Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text2).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 6 * Game1.tileSize)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            string text3 = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings", (object)Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned));
            Utility.drawTextWithShadow(b, text3, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 8 + Game1.tileSize / 2) - Game1.dialogueFont.MeasureString(text3).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 7 * Game1.tileSize - Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            if (Game1.MasterPlayer.hasPet())
            {
                string petDisplayName = Game1.MasterPlayer.getPetDisplayName();
                Utility.drawTextWithShadow(b, petDisplayName, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + 320) + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 + 8 + Game1.tileSize)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + 256) + Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 - 4 + Game1.tileSize)), new Rectangle(160 + (Game1.MasterPlayer.catPerson ? 0 : 16), 192, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, false, -1f, -1, -1, 0.35f);
            }
            if (Game1.player.horseName.Value != null && Game1.player.horseName.Value != "")
            {
                Utility.drawTextWithShadow(b, Game1.player.horseName.Value, Game1.dialogueFont, new Vector2((float)((double)(this.xPositionOnScreen + 384) + (double)Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + (Game1.player.getPetDisplayName() != null ? (double)Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.getPetDisplayName()).X) : 0.0)), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 + 8 + Game1.tileSize)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)((double)(this.xPositionOnScreen + 320 + 8) + (double)Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.Name).X / 2f) + (Game1.player.getPetDisplayName() != null ? (double)Math.Max(64f, Game1.dialogueFont.MeasureString(Game1.player.getPetDisplayName()).X) : 0.0)), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 448 - 4 + Game1.tileSize)), new Rectangle(193, 192, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, false, -1f, -1, -1, 0.35f);
            }
            int positionOnScreen = this.xPositionOnScreen;
            int num = this.width / 3;
            if (this.organizeButton != null)
                this.organizeButton.draw(b);
            this.trashCan.draw(b);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.trashCan.bounds.X + 60), (float)(this.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(686, 256, 18, 10)), Color.White, trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            if (this.checkHeldItem((Func<Item, bool>)null))
                Game1.player.CursorSlotItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 16), (float)(Game1.getOldMouseY() + 16)), 1f);
            if (hoverText == null || hoverText.Equals(""))
                return;
            IClickableMenu.drawToolTip(b, hoverText, hoverTitle, hoveredItem, this.checkHeldItem((Func<Item, bool>)null), -1, 0, -1, -1, (CraftingRecipe)null, -1);
        }
    }
}