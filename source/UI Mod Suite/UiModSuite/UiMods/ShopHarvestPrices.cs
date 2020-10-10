/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/UiModSuite
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UiModSuite.Options;

namespace UiModSuite.UiMods {
    class ShopHarvestPrices {

        /// <summary>
        /// Draws a box on the shop menus that display the harvest price of the highlighted seed or fruit tree
        /// </summary>
        internal void toggleOption() {

            GraphicsEvents.OnPostRenderGuiEvent -= drawShopHarvestPrices;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_HARVEST_PRICES_IN_SHOP ) ) {
                GraphicsEvents.OnPostRenderGuiEvent += drawShopHarvestPrices;
            }
        }

        private void drawShopHarvestPrices( object sender, EventArgs e ) {

            if( Game1.activeClickableMenu is ShopMenu == false ) {
                return;
            }

            var shopMenu = ( ShopMenu ) Game1.activeClickableMenu;
            var hoverItem = ( Item ) typeof( ShopMenu ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public ).GetValue( shopMenu );

            if( hoverItem == null ) {
                return;
            }

            string sellForAmount = "";
            string harvestPrice = "";

            int truePrice = ItemRolloverInformation.getTruePrice( hoverItem );

            if( truePrice > 0 && hoverItem.Name != "Scythe" ) {
                sellForAmount = "\n  " + truePrice / 2;

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1 ) {
                    sellForAmount += $" ({ truePrice / 2 * hoverItem.getStack() })";
                }
            }

            // Adds the price of the fully grown crop to the display text only if it is a seed
            if( hoverItem is StardewValley.Object && ( ( StardewValley.Object ) hoverItem ).type == "Seeds" && sellForAmount != "" ) {

                if( hoverItem.Name != "Mixed Seeds" || hoverItem.Name != "Winter Seeds" ) {
                    var crop = new Crop( hoverItem.parentSheetIndex, 0, 0 );
                    var debris = new Debris( crop.indexOfHarvest, Game1.player.position, Game1.player.position );
                    var item = new StardewValley.Object( debris.chunkType, 1 );
                    harvestPrice += $"    { item.price }";
                }
            }

            // Draws harvest info for seeds in shop
            if( Game1.activeClickableMenu is ShopMenu ) {

                // Don't draw if holding an item
                Item heldItem = ( Item ) typeof( ShopMenu ).GetField( "heldItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );
                if( heldItem != null ) {
                    return;
                }

                bool isSapling = false;

                // Handle cases for fruit trees
                switch( hoverItem.parentSheetIndex ) {
                    case 630: // orange
                        harvestPrice = "    100";
                        isSapling = true;
                        break;
                    case 628: // apricot
                        harvestPrice = "    50";
                        isSapling = true;
                        break;
                    case 629: // cherry
                        harvestPrice = "    80";
                        isSapling = true;
                        break;
                    case 633: // apple
                        harvestPrice = "    100";
                        isSapling = true;
                        break;
                    case 631: // peach
                        harvestPrice = "    140";
                        isSapling = true;
                        break;
                    case 632: // pomegranate
                        harvestPrice = "    140";
                        isSapling = true;
                        break;
                    default:
                        break;
                }

                // Only draw seeds or sapling info
                if( harvestPrice != "" && ( ( ( StardewValley.Object ) hoverItem ).type == "Seeds" || isSapling ) ) {

                    int positionX = Game1.activeClickableMenu.xPositionOnScreen - 30;
                    int positionY = Game1.activeClickableMenu.yPositionOnScreen + 580;

                    // Box
                    IClickableMenu.drawTextureBox( Game1.spriteBatch, positionX + 20, positionY - 52, 264, 108, Color.White );
                    //Game1.drawDialogueBox( positionX, positionY - 100, 220, 176, false, true );

                    // Text "HArvest price"
                    Game1.spriteBatch.DrawString( Game1.dialogueFont, "Harvest price", new Vector2( positionX + 30, positionY - 38 ), Color.Black * 0.2f );
                    Game1.spriteBatch.DrawString( Game1.dialogueFont, "Harvest price", new Vector2( positionX + 32, positionY - 40 ), Color.Black * 0.8f );

                    int shopIconPositionX = positionX + 80;

                    // Harvest icon
                    var spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( shopIconPositionX, positionY ), spriteRectangle, Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );

                    // Mini coin icon
                    Game1.spriteBatch.Draw( Game1.debrisSpriteSheet, new Vector2( shopIconPositionX + 32, positionY + 10 ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) 4f, SpriteEffects.None, 0.95f );

                    // Actual harvest price
                    Game1.spriteBatch.DrawString( Game1.dialogueFont, harvestPrice, new Vector2( shopIconPositionX - 2, positionY + 6 ), Color.Black * 0.2f );
                    Game1.spriteBatch.DrawString( Game1.dialogueFont, harvestPrice, new Vector2( shopIconPositionX, positionY + 4 ), Color.Black * 0.8f );

                    // Redraw tooltip
                    var hoverText = ModEntry.helper.Reflection.GetPrivateField<string>( shopMenu, "hoverText" ).GetValue();
                    var boldTitleText = ModEntry.helper.Reflection.GetPrivateField<string>( shopMenu, "boldTitleText" ).GetValue();
                    var hoveredItem = ModEntry.helper.Reflection.GetPrivateField<Item>( shopMenu, "hoveredItem" ).GetValue();
                    var currency = ModEntry.helper.Reflection.GetPrivateField<int>( shopMenu, "currency" ).GetValue();
                    var hoverPrice = ModEntry.helper.Reflection.GetPrivateField<int>( shopMenu, "hoverPrice" ).GetValue();
                    var getHoveredItemExtraItemIndex = ModEntry.helper.Reflection.GetPrivateMethod( shopMenu, "getHoveredItemExtraItemIndex" );
                    var getHoveredItemExtraItemAmount = ModEntry.helper.Reflection.GetPrivateMethod( shopMenu, "getHoveredItemExtraItemAmount" );
                    IClickableMenu.drawToolTip( Game1.spriteBatch, hoverText, boldTitleText, hoveredItem, heldItem != null, -1, currency, getHoveredItemExtraItemIndex.Invoke<int>(), getHoveredItemExtraItemAmount.Invoke<int>(), null, hoverPrice );
                }

                return;
            }
        }

    }
}
