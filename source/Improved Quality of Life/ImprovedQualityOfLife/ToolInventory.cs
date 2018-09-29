using System;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

namespace Demiacle.ImprovedQualityOfLife {
    internal class ToolInventory {

        private Dictionary<Tool, List<Item>> toolsAndTheirContainers = new Dictionary<Tool, List<Item>>();
        private List<ClickableComponent> clickableComponents = new List<ClickableComponent>();

        private bool isHeld;
        private bool justDidSwap = false;
        private int miniIconWidth = 32;
        private int miniIconHeight = 32;

        /// <summary>
        /// Adds a tool inventory that swaps tools with other tools that are stored away in a chest. It works by storing a reference to the tools container and swapping that with the current tool
        /// </summary>
        public ToolInventory() {

            onLoadFindAllToolsInChests();
            PlayerEvents.InventoryChanged += checkIfToolContainerChanged;
            ControlEvents.MouseChanged += handleRightClick;
            PlayerEvents.InventoryChanged += attachDrawMethodAfterLastHudDrawEvent;
        }

        /// <summary>
        /// Attaches the delegate just after player is loaded so toolbar will show up after all loaded render hud events
        /// **HACKY**
        /// </summary>
        private void attachDrawMethodAfterLastHudDrawEvent( object sender, EventArgsInventoryChanged e ) {
            GraphicsEvents.OnPreRenderHudEvent += drawToolbar;
            PlayerEvents.InventoryChanged -= attachDrawMethodAfterLastHudDrawEvent;
        }

        /// <summary>
        /// Swaps tools when icon is right clicked
        /// </summary>
        private void handleRightClick( object sender, EventArgsMouseStateChanged e ) {

            if( Game1.player.CurrentItem is Tool == false || Game1.player.canMove == false || Game1.player.usingTool || Game1.activeClickableMenu != null || Game1.eventUp || isToolPartOfCustomToolInventory( Game1.player.CurrentTool ) == false ) {
                return;
            }

            // Only fire next code on first registered click
            if( e.NewState.RightButton == ButtonState.Released ) {
                isHeld = false;
            }

            if( e.NewState.RightButton == ButtonState.Pressed && isHeld == false ) {
                isHeld = true;

                foreach( var component in clickableComponents.ToArray() ) {

                    if( component.bounds.Contains( e.NewPosition.X, e.NewPosition.Y ) ) {

                        // Name vars
                        Item itemToSwapIn = component.item;
                        int swapInItemIndex = Game1.player.CurrentToolIndex;
                        int swapOutItemIndex = toolsAndTheirContainers[ itemToSwapIn as Tool ].IndexOf( itemToSwapIn ) ;
                        var containerToPlaceSwappedOutItem = toolsAndTheirContainers[ itemToSwapIn as Tool ];

                        if( swapOutItemIndex == -1 ) {
                            ModEntry.Log( "The item failed to update! Please replace the offending item inside a chest!" );
                            return;
                        }

                        if( itemToSwapIn == Game1.player.CurrentItem ) {
                            return;
                        }

                        // Remove item
                        Item itemToSwapOut = Game1.player.removeItemFromInventory( swapInItemIndex );

                        // Swapp items
                        Game1.player.items[ swapInItemIndex ] = itemToSwapIn;
                        containerToPlaceSwappedOutItem[ swapOutItemIndex ] = itemToSwapOut;

                        // Update containers
                        toolsAndTheirContainers[ itemToSwapIn as Tool ] = Game1.player.items;
                        toolsAndTheirContainers[ itemToSwapOut as Tool ] = containerToPlaceSwappedOutItem;

                        // Only ignore next inv change if inventory is actually changed
                        if( Game1.player.items != containerToPlaceSwappedOutItem ) {
                            justDidSwap = true;
                        }

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Populates a list of all the tools not in players inventory.
        /// </summary>
        private void onLoadFindAllToolsInChests() {
            foreach( var location in Game1.locations ) {
                foreach( var objectInLocation in location.objects ) {
                    if( objectInLocation.Value is Chest ) {

                        var currentChest = ( objectInLocation.Value as Chest );
                        var allItemsInChest = currentChest.items;

                        foreach( var itemInChest in allItemsInChest.ToArray() ) {

                            if( itemInChest is Tool && isToolPartOfCustomToolInventory( itemInChest ) ) {
                                toolsAndTheirContainers.Add( itemInChest as Tool, currentChest.items );
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears and repopulates a list of tools to swap
        /// </summary>
        private void updateClickableComponents() {

            clickableComponents.Clear();

            int count = 0;
            int horizontalGap = 4;

            foreach( var item in toolsAndTheirContainers ) {
                var clickableComponent = new ClickableComponent( new Rectangle( ( count * miniIconWidth ) + horizontalGap, 0, miniIconWidth, miniIconHeight ), Convert.ToString( count ) );
                clickableComponent.item = item.Key;
                clickableComponents.Add( clickableComponent );
                count++;
            }           
        }

        /// <summary>
        /// Draws the toolbar based on the cached list
        /// </summary>
        private void drawToolbar( object sender, EventArgs e ) {

            if( Game1.player.CurrentItem is Tool == false || Game1.activeClickableMenu != null || Game1.eventUp || isToolPartOfCustomToolInventory( Game1.player.CurrentTool ) == false ) {
                return;
            }

            Toolbar toolbar = null;
            foreach( var menu in Game1.onScreenMenus ) {
                if( menu is Toolbar ) {
                    toolbar = ( Toolbar ) menu;
                    break;
                }
            }

            if( toolbar == null ) {
                return;
            }

            float alpha = ModEntry.helper.Reflection.GetPrivateField<float>( toolbar, "transparency" ).GetValue();

            int borderOffset = 12;
            int width = clickableComponents.Count * clickableComponents[0].bounds.Width + borderOffset * 2 ;
            var buttons = ( List<ClickableComponent> ) typeof( Toolbar ).GetField( "buttons", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( toolbar );
            int toolbarY = buttons[ Game1.player.CurrentToolIndex ].bounds.Y;

            int toolbarX = Game1.viewport.Width / 2 - Game1.tileSize * 12 / 2 - Game1.pixelZoom * 4 + ( Game1.tileSize * 12 + Game1.tileSize / 2 ) / 2 - ( width / 2 );

            // Shift location above or below toolbar
            if( toolbarY > Game1.viewport.Height / 2 ) {
                toolbarY -= 66;
            } else {
                toolbarY += 68; 
            }

            // Draw window
            IClickableMenu.drawTextureBox( Game1.spriteBatch, Game1.menuTexture, new Rectangle( 0, 256, 60, 60 ), toolbarX, toolbarY, width, miniIconHeight + 30, Color.White * alpha, 1f, false );

            // Draw buttons and borders
            int count = 0;
            foreach( var clickableComponent in clickableComponents ) {
                clickableComponent.bounds.X = toolbarX + count * clickableComponent.bounds.Width + borderOffset;
                clickableComponent.bounds.Y = toolbarY + borderOffset;
                clickableComponent.bounds.Width = 50;
                clickableComponent.bounds.Height = 38;

                int shiftedUpAmount = 0;

                // Highlight active tool
                if( clickableComponent.item == Game1.player.CurrentTool ) {
                    Rectangle highlightRectangle = clickableComponent.bounds;
                    highlightRectangle.X += 2;
                    highlightRectangle.Width -= 4;
                    highlightRectangle.Y += 4;
                    highlightRectangle.Height -= 4;
                    Game1.spriteBatch.Draw( Game1.staminaRect, highlightRectangle, Color.White * 0.5f );
                    if( toolbarY > Game1.viewport.Height / 2 ) {
                        shiftedUpAmount = -2;
                    } else {
                        shiftedUpAmount = 2;
                    }
                }

                if( clickableComponent.item.Name.Contains( "Watering" ) ) {
                    shiftedUpAmount += 10;
                }

                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( clickableComponent.bounds.X + clickableComponent.bounds.Width, clickableComponent.bounds.Y + 4, 2, clickableComponent.bounds.Height - 4 ), Color.LightGoldenrodYellow * 0.4f * alpha );
                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( clickableComponent.bounds.X + clickableComponent.bounds.Width - 2, clickableComponent.bounds.Y + 4, 2, clickableComponent.bounds.Height - 4 ), Color.Maroon * 0.2f * alpha);
                clickableComponent.item.drawInMenu( Game1.spriteBatch, new Vector2( clickableComponent.bounds.X - 8, clickableComponent.bounds.Y - borderOffset + shiftedUpAmount), 0.5f, alpha, 1, false );
                count++;
            }
        }

        /// <summary>
        /// Updates where the changed tool is stored. This also fires on load to populate the list
        /// </summary>
        private void checkIfToolContainerChanged( object sender, EventArgsInventoryChanged e ) {

            // Do nothing if swap was just made
            if( justDidSwap ) {
                justDidSwap = false;
                return;
            }

            if( e.Removed.Count > 0 ) {
                foreach( var itemStackChange in e.Removed ) {
                    handleItemRemovedFromInventory( itemStackChange.Item );
                }
            }

            if( e.Added.Count > 0 ) {
                foreach( var itemStackChange in e.Added ) {
                    handleItemAddedToInventory( itemStackChange.Item );
                }
            }
        }

        /// <summary>
        /// Updates or adds a tool to the toolbar
        /// </summary>
        /// <param name="item">The item added to the players inventory</param>
        private void handleItemAddedToInventory( Item item ) {

            // Add only selected items
            if( isToolPartOfCustomToolInventory( item ) ) {

                var addedTool = ( Tool ) item;

                // Handle taking item from chest
                if( Game1.activeClickableMenu is ItemGrabMenu && toolsAndTheirContainers.ContainsKey( addedTool ) ) {
                    toolsAndTheirContainers[ addedTool ] = Game1.player.items;
                    return;
                }

                // Handle new tools
                if( toolsAndTheirContainers.ContainsKey( addedTool ) == false ) {
                    toolsAndTheirContainers.Add( addedTool, Game1.player.items );
                    updateClickableComponents();
                }
            }
        }

        /// <summary>
        /// Updates or removes a tool from toolbar
        /// </summary>
        /// <param name="item">The item removed from the players inventory</param>
        private void handleItemRemovedFromInventory( Item item ) {

            if( isToolPartOfCustomToolInventory( item ) ) {

                var removedTool = ( Tool ) item;

                // Handle placing item in chest
                if( Game1.activeClickableMenu is ItemGrabMenu && toolsAndTheirContainers.ContainsKey( removedTool ) ) {
                    var itemGrabMenu = ( ItemGrabMenu ) Game1.activeClickableMenu;
                    var inventoryMenu = ( InventoryMenu ) typeof( ItemGrabMenu ).GetField( "ItemsToGrabMenu", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( itemGrabMenu );
                    toolsAndTheirContainers[ removedTool ] = inventoryMenu.actualInventory;
                    return;
                }

                // Handle removing item so it becomes unavailable
                if( toolsAndTheirContainers.ContainsKey( removedTool ) ) {
                    toolsAndTheirContainers.Remove( removedTool );
                    updateClickableComponents();
                }
            }
        }

        /// <summary>
        /// Determine whether or not an item is a tool
        /// </summary>
        /// <param name="item">The tool to check</param>
        /// <returns>True if the item is a tool</returns>
        private bool isToolPartOfCustomToolInventory( Item item ) {

            if( item is Tool == false ) {
                return false;
            }

            if( item is Axe ||
                item is Pickaxe ||
                item.Name.Contains( "Scythe" ) ||
                item is Hoe ||
                item is WateringCan ||
                item is Shears ||
                item is Pan ||
                item is MilkPail ||
                item is FishingRod
            ) {
                return true;
            } else {
                return false;
            }
        }

    }
}