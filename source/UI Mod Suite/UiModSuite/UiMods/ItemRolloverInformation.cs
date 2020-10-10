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
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UiModSuite.Options;

namespace UiModSuite.UiMods {
    class ItemRolloverInformation {

        Item hoverItem;
        CommunityCenter communityCenter;
        Dictionary<string, List<int>> prunedRequiredBundles = new Dictionary<string, List<int>>();
        Dictionary<string, string> bundleData;
        ClickableTextureComponent bundleIcon =  new ClickableTextureComponent("", new Rectangle( 0, 0, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), (float) Game1.pixelZoom, false);

        /// <summary>
        /// This mod displays an improved tooltip
        /// </summary>
        public void toggleOption() {
            PlayerEvents.InventoryChanged -= populateRequiredBundled;
            GraphicsEvents.OnPostRenderEvent -= drawAdvancedToolipForMenu;
            GraphicsEvents.OnPostRenderHudEvent -= drawAdvancedToolipForToolbar;
            GraphicsEvents.OnPreRenderEvent -= getHoverItem;

            if ( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_EXTRA_ITEM_INFORMATION ) ) {

                // Load bundle data
                communityCenter = ( CommunityCenter ) Game1.getLocationFromName( "CommunityCenter" );
                bundleData = Game1.content.Load<Dictionary<string, string>>( "Data\\Bundles" );

                // Parse data to easily work with bundle data
                populateRequiredBundled( null, null );

                PlayerEvents.InventoryChanged += populateRequiredBundled;
                GraphicsEvents.OnPostRenderEvent += drawAdvancedToolipForMenu;
                GraphicsEvents.OnPostRenderHudEvent += drawAdvancedToolipForToolbar;
                GraphicsEvents.OnPreRenderEvent += getHoverItem;
            }
        }

        private void drawAdvancedToolipForMenu( object sender, EventArgs e ) {
            if( Game1.activeClickableMenu != null ) {
                drawAdvancedToolip( sender, e );
            }
        }

        private void drawAdvancedToolipForToolbar( object sender, EventArgs e ) {
            if( Game1.activeClickableMenu == null ) {
                drawAdvancedToolip( sender, e );
            }
        }

        /// <summary>
        /// Finds all the bundles still needing resources
        /// </summary>
        private void populateRequiredBundled( object sender, EventArgs e ) {
            prunedRequiredBundles.Clear();

            foreach( var item in bundleData ) {

                // This code sucks....
                string bundleArea = item.Key.Split( '/' )[ 0 ];
                int noteInt = 0;

                switch( bundleArea ) {
                    case "Pantry":
                        noteInt = 0;
                        break;
                    case "Crafts Room":
                        noteInt = 1;
                        break;
                    case "Fish Tank":
                        noteInt = 2;
                        break;
                    case "Boiler Room":
                        noteInt = 3;
                        break;
                    case "Vault":
                        noteInt = 4;
                        break;
                    case "Bulletin Board":
                        noteInt = 5;
                        break;
                    default:
                        continue;
                }

                // Ignore items if bundles cannot be turned in
                if( communityCenter.shouldNoteAppearInArea( noteInt ) == false ) {
                    continue;
                }

                // Required since the index in the bundles are all wonky
                int indexInSavedBundleData = Convert.ToInt32( item.Key.Split( '/' )[ 1 ] );

                string[] data = item.Value.Split( '/' );
                string bundleType = data[ 0 ];
                //string reward = data[ 1 ];
                string[] requiredItems = data[ 2 ].Split( ' ' );

                List<int> prunedRequiredItems = new List<int>();

                // Add only every 3rd entry ( required item tile index )
                for( int i = 0; i < requiredItems.Count(); i++ ) {
                    if( i % 3 == 0 ) {
                        if( Convert.ToInt32( requiredItems[ i ] ) == -1 ) {
                            continue;
                        }

                        //ModEntry.Log( $"count is {indexInSavedBundleData} and prunedItems is {prunedRequiredItems.Count()}" );
                        if( communityCenter.bundles[ indexInSavedBundleData ][ prunedRequiredItems.Count() ] == false ) {
                            prunedRequiredItems.Add( Convert.ToInt32( requiredItems[ i ] ) );
                        }
                    }
                }

                prunedRequiredBundles.Add( bundleType, prunedRequiredItems );
            }
        }

        /// <summary>
        /// Draw it!
        /// </summary>
        private void drawAdvancedToolip( object sender, EventArgs e ) {
            if( hoverItem == null ) {
                return;
            }

            string sellForAmount = "";
            string harvestPrice = "";

            int truePrice = getTruePrice( hoverItem );

            if( truePrice > 0 && hoverItem.Name != "Scythe" ) {
                sellForAmount = "\n  " + truePrice / 2;

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1 ) {
                    sellForAmount += $" ({ truePrice / 2 * hoverItem.getStack() })";
                }
            } 

            bool isDrawingHarvestPrice = false;

            // Adds the price of the fully grown crop to the display text only if it is a seed
            if( hoverItem is StardewValley.Object && ( ( StardewValley.Object ) hoverItem ).type == "Seeds" && sellForAmount != "" ) {

                if( hoverItem.Name != "Mixed Seeds" || hoverItem.Name != "Winter Seeds" ) {
                    var crop = new Crop( hoverItem.parentSheetIndex, 0, 0 );
                    var debris = new Debris( crop.indexOfHarvest, Game1.player.position, Game1.player.position );
                    var item = new StardewValley.Object( debris.chunkType, 1 );
                    harvestPrice += $"    { item.price }";
                    isDrawingHarvestPrice = true;
                }
            }

            string advancedTitle = hoverItem.Name + sellForAmount + harvestPrice;

            // Draw tooltip
            Vector2 iconPosition = drawToolTip( Game1.spriteBatch, hoverItem.getDescription(), advancedTitle, hoverItem, false, -1, 0, -1, -1, null, -1 );
            float iconPositionX = iconPosition.X;
            float iconPositionY = iconPosition.Y;

            // Reposition coin and harvest icon
            iconPositionX += 30;
            iconPositionY -= 10;

            // Draw icons inside description text
            if( sellForAmount != "" ) {

                // Draw coin icon
                Game1.spriteBatch.Draw( Game1.debrisSpriteSheet, new Vector2( iconPositionX, iconPositionY ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );
               
                // Draw harvest icon
                if( isDrawingHarvestPrice ) {
                    var spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( iconPositionX + Game1.dialogueFont.MeasureString( sellForAmount ).X - 10, iconPositionY - 20 ), spriteRectangle, Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );
                }

            }

            // Draw bundle info
            foreach( var bundleType in prunedRequiredBundles ) {
                if( bundleType.Value.Contains( hoverItem.parentSheetIndex ) ) {
                    int xPos = ( int ) iconPositionX - 64;
                    int yPos = ( int ) iconPositionY - 110;

                    int bgPositionX = xPos + 52;
                    int bgPositionY = yPos - 2;
                    int totalWidth = 288;
                    int height = 36;
                    int amountOfSections = 36;
                    int sectionWidth = totalWidth / amountOfSections;
                    int amountOfSectionsWithoutAlpha = 6;

                    for( int i = 0; i < amountOfSections; i++ ) {
                        float alpha;
                        if( i < amountOfSectionsWithoutAlpha ) {
                            alpha = 0.92f;
                        } else {
                            alpha = 0.92f - ( i - amountOfSectionsWithoutAlpha ) * ( 1f / ( amountOfSections - amountOfSectionsWithoutAlpha ) ) ;
                        }
                        Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( bgPositionX + (sectionWidth * i), bgPositionY, sectionWidth, height ), Color.Crimson * alpha );
                    }

                    Game1.spriteBatch.DrawString( Game1.dialogueFont, bundleType.Key, new Vector2( xPos + 72, yPos ), Color.White );

                    bundleIcon.bounds.X = xPos + 16;
                    bundleIcon.bounds.Y = yPos;
                    bundleIcon.scale = 3f;
                    bundleIcon.draw( Game1.spriteBatch );

                    break;
                }
            }

            restoreMenuState();
        }

        /// <summary>
        /// This restores the hover item so it will be usable for lookup anything or any other mod
        /// </summary>
        private void restoreMenuState() {
            if( Game1.activeClickableMenu is ItemGrabMenu ) {
                var itemGrabMenu = ( ItemGrabMenu ) Game1.activeClickableMenu;
                itemGrabMenu.hoveredItem = hoverItem;
            }
        }

        /// <summary>
        /// Gets the correct item price as ore prices use the price property
        /// </summary>
        /// <param name="hoverItem">The item</param>
        /// <returns>The correct sell price</returns>
        public static int getTruePrice( Item hoverItem ) {
            if( hoverItem is StardewValley.Object ) {

                // No clue why selToStorePrice needs to be multiplied for the correct value...???
                return ( hoverItem as StardewValley.Object ).sellToStorePrice() * 2; 
            }

            // Overwrite ores cause salePrice() is not accurate for some reason...???
            switch( hoverItem.parentSheetIndex ) {
                case 378:
                case 380:
                case 382:
                case 384:
                case 388:
                case 390:
                    return ( int ) ( ( double ) ( (hoverItem as StardewValley.Object).price * 2 ) * ( 1.0 + ( double ) ( hoverItem as StardewValley.Object ).quality * 0.25 ) );
                default:
                    return hoverItem.salePrice();
            }
        }

        /// <summary>
        /// Gets the hover item and removes the vanilla tooltip displays **HACKY
        /// </summary>
        private void getHoverItem( object sender, EventArgs e ) {
            var test = Game1.player.items;

            // Remove hovers from toolbar
            for( int j = 0; j < Game1.onScreenMenus.Count; j++ ) {
                if( Game1.onScreenMenus[ j ] is Toolbar ) {
                    var menu = Game1.onScreenMenus[ j ] as Toolbar;

                    hoverItem = ( Item ) typeof( Toolbar ).GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( menu );
                    typeof( Toolbar ).GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( menu, null );
                }
            }

            // Remove hovers from inventory
            if( Game1.activeClickableMenu is GameMenu ) {

                // Get pages from GameMenu            
                var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );

                // Overwrite Inventory Menu
                for( int i = 0; i < pages.Count; i++ ) {
                    if( pages[ i ] is InventoryPage ) {
                        var inventoryPage = ( InventoryPage ) pages[ i ];
                        hoverItem = ( Item ) typeof( InventoryPage ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( inventoryPage );
                        typeof( InventoryPage ).GetField( "hoverText", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( inventoryPage, "" );
                    }
                }
            }

            // Remove hovers from chests and shipping bin
            if( Game1.activeClickableMenu is ItemGrabMenu ) {
                var itemGrabMenu  = ( ItemGrabMenu ) Game1.activeClickableMenu;
                hoverItem = itemGrabMenu.hoveredItem;
                itemGrabMenu.hoveredItem = null;
            }
        }

        // --------------------Below is the copied tooltip modified to my own specifications--------------------

        public static Vector2 drawToolTip( SpriteBatch b, string hoverText, string hoverTitle, Item hoveredItem, bool heldItem = false, int healAmountToDisplay = -1, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, CraftingRecipe craftingIngredients = null, int moneyAmountToShowAtBottom = -1 ) {
            bool flag = hoveredItem != null && hoveredItem is StardewValley.Object && ( hoveredItem as StardewValley.Object ).edibility != -300;
            SpriteBatch b1 = b;
            string text = hoverText;
            SpriteFont smallFont = Game1.smallFont;
            int xOffset = heldItem ? Game1.tileSize / 2 + 8 : 0;
            int yOffset = heldItem ? Game1.tileSize / 2 + 8 : 0;
            int moneyAmountToDisplayAtBottom = moneyAmountToShowAtBottom;
            string boldTitleText = hoverTitle;
            int healAmountToDisplay1 = flag ? ( hoveredItem as StardewValley.Object ).edibility : -1;
            string[] buffIconsToDisplay;
            if( flag ) {
                if( Game1.objectInformation[ ( hoveredItem as StardewValley.Object ).parentSheetIndex ].Split( '/' ).Length >= 7 ) {
                    buffIconsToDisplay = Game1.objectInformation[ ( hoveredItem as StardewValley.Object ).parentSheetIndex ].Split( '/' )[ 6 ].Split( ' ' );
                    goto label_4;
                }
            }
            buffIconsToDisplay = ( string[] ) null;
            label_4:
            Item hoveredItem1 = hoveredItem;
            int currencySymbol1 = currencySymbol;
            int extraItemToShowIndex1 = extraItemToShowIndex;
            int extraItemToShowAmount1 = extraItemToShowAmount;
            int overrideX = -1;
            int overrideY = -1;
            double num = 1.0;
            CraftingRecipe craftingIngredients1 = craftingIngredients;
            return drawHoverText( b1, text, smallFont, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay1, buffIconsToDisplay, hoveredItem1, currencySymbol1, extraItemToShowIndex1, extraItemToShowAmount1, overrideX, overrideY, ( float ) num, craftingIngredients1 );
        }

        public static Vector2 drawHoverText( SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, CraftingRecipe craftingIngredients = null ) {
            if( text == null || text.Length == 0 )
                return Vector2.Zero;
            if( boldTitleText != null && boldTitleText.Length == 0 )
                boldTitleText = ( string ) null;
            int num1 = 20;
            int width = Math.Max( healAmountToDisplay != -1 ? ( int ) font.MeasureString( healAmountToDisplay.ToString() + "+ Energy" + ( object ) ( Game1.tileSize / 2 ) ).X : 0, Math.Max( ( int ) font.MeasureString( text ).X, boldTitleText != null ? ( int ) Game1.dialogueFont.MeasureString( boldTitleText ).X : 0 ) ) + Game1.tileSize / 2;
            int height = Math.Max( num1 * 3, ( int ) font.MeasureString( text ).Y + Game1.tileSize / 2 + ( moneyAmountToDisplayAtBottom > -1 ? ( int ) ( ( double ) font.MeasureString( string.Concat( ( object ) moneyAmountToDisplayAtBottom ) ).Y + 4.0 ) : 0 ) + ( boldTitleText != null ? ( int ) ( ( double ) Game1.dialogueFont.MeasureString( boldTitleText ).Y + ( double ) ( Game1.tileSize / 4 ) ) : 0 ) + ( healAmountToDisplay != -1 ? 38 : 0 ) );
            if( buffIconsToDisplay != null ) {
                foreach( string str in buffIconsToDisplay ) {
                    if( !str.Equals( "0" ) )
                        height += 34;
                }
                height += 4;
            }
            string text1 = ( string ) null;
            if( hoveredItem != null ) {
                height += ( Game1.tileSize + 4 ) * hoveredItem.attachmentSlots();
                text1 = hoveredItem.getCategoryName();
                if( text1.Length > 0 )
                    height += ( int ) font.MeasureString( "T" ).Y;
                if( hoveredItem is MeleeWeapon ) {
                    height = Math.Max( num1 * 3, ( boldTitleText != null ? ( int ) ( ( double ) Game1.dialogueFont.MeasureString( boldTitleText ).Y + ( double ) ( Game1.tileSize / 4 ) ) : 0 ) + Game1.tileSize / 2 ) + ( int ) font.MeasureString( "T" ).Y + ( moneyAmountToDisplayAtBottom > -1 ? ( int ) ( ( double ) font.MeasureString( string.Concat( ( object ) moneyAmountToDisplayAtBottom ) ).Y + 4.0 ) : 0 ) + ( int ) ( ( double ) ( ( hoveredItem as MeleeWeapon ).getNumberOfDescriptionCategories() * Game1.pixelZoom * 12 ) + ( double ) font.MeasureString( Game1.parseText( ( hoveredItem as MeleeWeapon ).description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ) ).Y );
                    width = ( int ) Math.Max( ( float ) width, font.MeasureString( "99-99 Damage" ).X + ( float ) ( 15 * Game1.pixelZoom ) + ( float ) ( Game1.tileSize / 2 ) );
                } else if( hoveredItem is Boots ) {
                    height = height - ( int ) font.MeasureString( text ).Y + ( int ) ( ( double ) ( ( hoveredItem as Boots ).getNumberOfDescriptionCategories() * Game1.pixelZoom * 12 ) + ( double ) font.MeasureString( Game1.parseText( ( hoveredItem as Boots ).description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ) ).Y );
                    width = ( int ) Math.Max( ( float ) width, font.MeasureString( "99-99 Damage" ).X + ( float ) ( 15 * Game1.pixelZoom ) + ( float ) ( Game1.tileSize / 2 ) );
                } else if( hoveredItem is StardewValley.Object && ( hoveredItem as StardewValley.Object ).edibility != -300 ) {
                    if( healAmountToDisplay == -1 )
                        height += ( Game1.tileSize / 2 + Game1.pixelZoom * 2 ) * ( healAmountToDisplay > 0 ? 2 : 1 );
                    else
                        height += Game1.tileSize / 2 + Game1.pixelZoom * 2;
                    healAmountToDisplay = ( int ) Math.Ceiling( ( double ) ( hoveredItem as StardewValley.Object ).Edibility * 2.5 ) + ( hoveredItem as StardewValley.Object ).quality * ( hoveredItem as StardewValley.Object ).Edibility;
                }
            }
            if( craftingIngredients != null ) {
                width = Math.Max( ( int ) Game1.dialogueFont.MeasureString( boldTitleText ).X + Game1.pixelZoom * 3, Game1.tileSize * 6 );
                height += craftingIngredients.getDescriptionHeight( width - Game1.pixelZoom * 2 ) + ( healAmountToDisplay == -1 ? -Game1.tileSize / 2 : 0 );
            }
            int x = Game1.getOldMouseX() + Game1.tileSize / 2 + xOffset;
            int y1 = Game1.getOldMouseY() + Game1.tileSize / 2 + yOffset;
            if( overrideX != -1 )
                x = overrideX;
            if( overrideY != -1 )
                y1 = overrideY;
            if( x + width > Game1.viewport.Width ) {
                x = Game1.viewport.Width - width;
                y1 += Game1.tileSize / 4;
            }
            if( y1 + height > Game1.viewport.Height ) {
                x += Game1.tileSize / 4;
                y1 = Game1.viewport.Height - height;
            }
            
            IClickableMenu.drawTextureBox( b, Game1.menuTexture, new Rectangle( 0, 256, 60, 60 ), x, y1, width + ( craftingIngredients != null ? Game1.tileSize / 3 : 0 ), height, Color.White * alpha, 1f, true );

            if( boldTitleText != null ) {
                IClickableMenu.drawTextureBox( b, Game1.menuTexture, new Rectangle( 0, 256, 60, 60 ), x, y1, width + ( craftingIngredients != null ? Game1.tileSize / 3 : 0 ), ( int ) Game1.dialogueFont.MeasureString( boldTitleText ).Y + Game1.tileSize / 2 + ( hoveredItem == null || text1.Length <= 0 ? 0 : ( int ) font.MeasureString( "asd" ).Y ) - Game1.pixelZoom, Color.White * alpha, 1f, false );
                b.Draw( Game1.menuTexture, new Rectangle( x + Game1.pixelZoom * 3, y1 + ( int ) Game1.dialogueFont.MeasureString( boldTitleText ).Y + Game1.tileSize / 2 + ( hoveredItem == null || text1.Length <= 0 ? 0 : ( int ) font.MeasureString( "asd" ).Y ) - Game1.pixelZoom, width - Game1.pixelZoom * ( craftingIngredients == null ? 6 : 1 ), Game1.pixelZoom ), new Rectangle?( new Rectangle( 44, 300, 4, 4 ) ), Color.White );
                b.DrawString( Game1.dialogueFont, boldTitleText, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y1 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 2f, 2f ), Game1.textShadowColor );
                b.DrawString( Game1.dialogueFont, boldTitleText, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y1 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 0.0f, 2f ), Game1.textShadowColor );
                b.DrawString( Game1.dialogueFont, boldTitleText, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y1 + Game1.tileSize / 4 + 4 ) ), Game1.textColor );
                y1 += ( int ) Game1.dialogueFont.MeasureString( boldTitleText ).Y;
            }
            int y2;
            if( hoveredItem != null && text1.Length > 0 ) {
                int num2 = y1 - 4;
                Utility.drawTextWithShadow( b, text1, font, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( num2 + Game1.tileSize / 4 + 4 ) ), hoveredItem.getCategoryColor(), 1f, -1f, 2, 2, 1f, 3 );
                y2 = num2 + ( ( int ) font.MeasureString( "T" ).Y + ( boldTitleText != null ? Game1.tileSize / 4 : 0 ) + Game1.pixelZoom );
            } else
                y2 = y1 + ( boldTitleText != null ? Game1.tileSize / 4 : 0 );
            if( hoveredItem != null && hoveredItem is Boots ) {
                Boots boots = hoveredItem as Boots;
                Utility.drawTextWithShadow( b, Game1.parseText( boots.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), Game1.textColor, 1f, -1f, -1, -1, 1f, 3 );
                y2 += ( int ) font.MeasureString( Game1.parseText( boots.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ) ).Y;
                if( boots.defenseBonus > 0 ) {
                    Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 110, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                    Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_DefenseBonus", ( object ) boots.defenseBonus ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                    y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                }
                if( boots.immunityBonus > 0 ) {
                    Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 150, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                    Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_ImmunityBonus", ( object ) boots.immunityBonus ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                    y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                }
            } else if( hoveredItem != null && hoveredItem is MeleeWeapon ) {
                MeleeWeapon meleeWeapon = hoveredItem as MeleeWeapon;
                Utility.drawTextWithShadow( b, Game1.parseText( meleeWeapon.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), Game1.textColor, 1f, -1f, -1, -1, 1f, 3 );
                y2 += ( int ) font.MeasureString( Game1.parseText( meleeWeapon.description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4 ) ).Y;
                if( meleeWeapon.indexOfMenuItemView != 47 ) {
                    Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 120, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                    Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_Damage", ( object ) meleeWeapon.minDamage, ( object ) meleeWeapon.maxDamage ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                    y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    if( meleeWeapon.speed != ( meleeWeapon.type == 2 ? -8 : 0 ) ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 130, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                        bool flag = meleeWeapon.type == 2 && meleeWeapon.speed < -8 || meleeWeapon.type != 2 && meleeWeapon.speed < 0;
                        Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_Speed", ( object ) ( ( meleeWeapon.speed > 0 ? ( object ) "+" : ( object ) "" ).ToString() + ( object ) ( ( meleeWeapon.type == 2 ? meleeWeapon.speed - -8 : meleeWeapon.speed ) / 2 ) ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), flag ? Color.DarkRed : Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    }
                    if( meleeWeapon.addedDefense > 0 ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 110, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                        Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_DefenseBonus", ( object ) meleeWeapon.addedDefense ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    }
                    if( ( double ) meleeWeapon.critChance / 0.02 >= 2.0 ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 40, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                        Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_CritChanceBonus", ( object ) ( ( double ) ( int ) meleeWeapon.critChance / 0.02 ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    }
                    if( ( ( double ) meleeWeapon.critMultiplier - 3.0 ) / 0.02 >= 1.0 ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 160, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                        Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_CritChanceBonus", ( object ) ( int ) ( ( ( double ) meleeWeapon.critMultiplier - 3.0 ) / 0.02 ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 11 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    }
                    if( ( double ) meleeWeapon.knockback != ( double ) meleeWeapon.defaultKnockBackForThisType( meleeWeapon.type ) ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), new Rectangle( 70, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, false, 1f, -1, -1, 0.35f );
                        Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_Weight", ( object ) ( ( ( double ) meleeWeapon.knockback > ( double ) meleeWeapon.defaultKnockBackForThisType( meleeWeapon.type ) ? ( object ) "+" : ( object ) "" ).ToString() + ( object ) ( int ) Math.Ceiling( ( double ) Math.Abs( meleeWeapon.knockback - meleeWeapon.defaultKnockBackForThisType( meleeWeapon.type ) ) * 10.0 ) ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom * 13 ), ( float ) ( y2 + Game1.tileSize / 4 + Game1.pixelZoom * 3 ) ), Game1.textColor * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += ( int ) Math.Max( font.MeasureString( "TT" ).Y, ( float ) ( 12 * Game1.pixelZoom ) );
                    }
                }
            } else if( text.Length > 1 ) {
                b.DrawString( font, text, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 2f, 2f ), Game1.textShadowColor * alpha );
                b.DrawString( font, text, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 0.0f, 2f ), Game1.textShadowColor * alpha );
                b.DrawString( font, text, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 2f, 0.0f ), Game1.textShadowColor * alpha );
                b.DrawString( font, text, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), Game1.textColor * 0.9f * alpha );
                y2 += ( int ) font.MeasureString( text ).Y + 4;
            }
            if( craftingIngredients != null ) {
                craftingIngredients.drawRecipeDescription( b, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 - Game1.pixelZoom * 2 ) ), width );
                y2 += craftingIngredients.getDescriptionHeight( width );
            }
            if( healAmountToDisplay != -1 ) {
                Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 ) ), new Rectangle( healAmountToDisplay < 0 ? 140 : 0, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f );
                Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_Energy", ( object ) ( ( healAmountToDisplay > 0 ? ( object ) "+" : ( object ) "" ).ToString() + ( object ) healAmountToDisplay ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + 34 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 8 ) ), Game1.textColor, 1f, -1f, -1, -1, 1f, 3 );
                y2 += 34;
                if( healAmountToDisplay > 0 ) {
                    Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 ) ), new Rectangle( 0, 438, 10, 10 ), Color.White, 0.0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f );
                    Utility.drawTextWithShadow( b, Game1.content.LoadString( "Strings\\UI:ItemHover_Health", ( object ) ( ( healAmountToDisplay > 0 ? ( object ) "+" : ( object ) "" ).ToString() + ( object ) ( int ) ( ( double ) healAmountToDisplay * 0.400000005960464 ) ) ), font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + 34 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 8 ) ), Game1.textColor, 1f, -1f, -1, -1, 1f, 3 );
                    y2 += 34;
                }
            }
            if( buffIconsToDisplay != null ) {
                for( int index = 0; index < buffIconsToDisplay.Length; ++index ) {
                    if( !buffIconsToDisplay[ index ].Equals( "0" ) ) {
                        Utility.drawWithShadow( b, Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 4 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 ) ), new Rectangle( 10 + index * 10, 428, 10, 10 ), Color.White, 0.0f, Vector2.Zero, 3f, false, 0.95f, -1, -1, 0.35f );
                        string text2 = ( Convert.ToInt32( buffIconsToDisplay[ index ] ) > 0 ? "+" : "" ) + buffIconsToDisplay[ index ] + " ";
                        if( index <= 10 )
                            text2 = Game1.content.LoadString( "Strings\\UI:ItemHover_Buff" + ( object ) index, ( object ) text2 );
                        Utility.drawTextWithShadow( b, text2, font, new Vector2( ( float ) ( x + Game1.tileSize / 4 + 34 + Game1.pixelZoom ), ( float ) ( y2 + Game1.tileSize / 4 + 8 ) ), Game1.textColor, 1f, -1f, -1, -1, 1f, 3 );
                        y2 += 34;
                    }
                }
            }
            if( hoveredItem != null && hoveredItem.attachmentSlots() > 0 ) {
                y2 += 16;
                hoveredItem.drawAttachments( b, x + Game1.tileSize / 4, y2 );
                if( moneyAmountToDisplayAtBottom > -1 )
                    y2 += Game1.tileSize * hoveredItem.attachmentSlots();
            }
            if( moneyAmountToDisplayAtBottom > -1 ) {
                b.DrawString( font, string.Concat( ( object ) moneyAmountToDisplayAtBottom ), new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 2f, 2f ), Game1.textShadowColor );
                b.DrawString( font, string.Concat( ( object ) moneyAmountToDisplayAtBottom ), new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 0.0f, 2f ), Game1.textShadowColor );
                b.DrawString( font, string.Concat( ( object ) moneyAmountToDisplayAtBottom ), new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ) + new Vector2( 2f, 0.0f ), Game1.textShadowColor );
                b.DrawString( font, string.Concat( ( object ) moneyAmountToDisplayAtBottom ), new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( y2 + Game1.tileSize / 4 + 4 ) ), Game1.textColor );
                if( currencySymbol == 0 )
                    b.Draw( Game1.debrisSpriteSheet, new Vector2( ( float ) ( x + Game1.tileSize / 4 ) + font.MeasureString( moneyAmountToDisplayAtBottom.ToString() + "  " ).X, ( float ) ( y2 + Game1.tileSize / 4 + 16 ) ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0.0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );
                else if( currencySymbol == 1 )
                    b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 8 ) + font.MeasureString( moneyAmountToDisplayAtBottom.ToString() + "  " ).X, ( float ) ( y2 + Game1.tileSize / 4 ) ), new Rectangle?( new Rectangle( 338, 400, 8, 8 ) ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 1f );
                else if( currencySymbol == 2 )
                    b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( x + Game1.tileSize / 8 ) + font.MeasureString( moneyAmountToDisplayAtBottom.ToString() + "  " ).X, ( float ) ( y2 + Game1.tileSize / 4 ) ), new Rectangle?( new Rectangle( 211, 373, 9, 10 ) ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 1f );
                y2 += Game1.tileSize * 3 / 4;
            }
            if( extraItemToShowIndex == -1 )
                return new Vector2( x, y1 );
            IClickableMenu.drawTextureBox( b, Game1.menuTexture, new Rectangle( 0, 256, 60, 60 ), x, y2 + Game1.pixelZoom, width, Game1.tileSize * 3 / 2, Color.White, 1f, true );
            int num3 = y2 + Game1.pixelZoom * 5;
            string text3 = Game1.content.LoadString( "Strings\\UI:ItemHover_Requirements", ( object ) extraItemToShowAmount, ( object ) Game1.objectInformation[ extraItemToShowIndex ].Split( '/' )[ 0 ] );
            b.DrawString( font, text3, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( num3 + Game1.pixelZoom ) ) + new Vector2( 2f, 2f ), Game1.textShadowColor );
            b.DrawString( font, text3, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( num3 + Game1.pixelZoom ) ) + new Vector2( 0.0f, 2f ), Game1.textShadowColor );
            b.DrawString( font, text3, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( num3 + Game1.pixelZoom ) ) + new Vector2( 2f, 0.0f ), Game1.textShadowColor );
            b.DrawString( Game1.smallFont, text3, new Vector2( ( float ) ( x + Game1.tileSize / 4 ), ( float ) ( num3 + Game1.pixelZoom ) ), Game1.textColor );
            b.Draw( Game1.objectSpriteSheet, new Vector2( ( float ) ( x + Game1.tileSize * 2 + Game1.tileSize * 2 / 3 ), ( float ) num3 ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16 ) ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 1f );
            return new Vector2( x, y1 );
        }

    }
}
