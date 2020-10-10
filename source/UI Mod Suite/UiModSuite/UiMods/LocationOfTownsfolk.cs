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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Reflection;
using UiModSuite.Options;


namespace UiModSuite.UiMods {

    class LocationOfTownsfolk {

        private SocialPage socialPage;

        private bool isClicking;
        private List<string> listOfHoveredInformation = new List<string>();

        private List<ClickableTextureComponent> friendNames;
        private List<NPC> townsfolk = new List<NPC>();
        private List<OptionsCheckbox> checkboxes = new List<OptionsCheckbox>();

        public const string SHOW_NPCS_ON_MAP = "Show npcs on map";

        private int socialPanelWidth = 190;
        private int socialPanelOffsetX = 160;
        
        /// <summary>
        /// This mod displays mugshots of townsfolk on the map.
        /// </summary>
        public void toggleShowNPCLocationOnMap() {

            onMenuChange( null, null );
            GraphicsEvents.OnPostRenderGuiEvent -= drawNPCLocationsOnMap;
            GraphicsEvents.OnPostRenderGuiEvent -= drawSocialPageOptions;
            ControlEvents.MouseChanged -= handleClickForSocialPage;
            MenuEvents.MenuChanged -= onMenuChange;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_LOCATION_Of_TOWNSPEOPLE ) ) {
                GraphicsEvents.OnPostRenderGuiEvent += drawNPCLocationsOnMap;
                GraphicsEvents.OnPostRenderGuiEvent += drawSocialPageOptions;
                ControlEvents.MouseChanged += handleClickForSocialPage;
                MenuEvents.MenuChanged += onMenuChange;
            }
        }

        private void drawNPCLocationsOnMap( object sender, EventArgs e ) {
            
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            var currentMenu = (GameMenu) Game1.activeClickableMenu;

            if(  currentMenu.currentTab != GameMenu.mapTab ) {
                return;
            }

            listOfHoveredInformation.Clear();

            foreach( NPC npc in townsfolk ) {

                int key = npc.name.GetHashCode();

                // If key for some reason doesn't exist
                if( !( ModEntry.modData.locationOfTownsfolkOptions.ContainsKey( key ) ) ) {
                    continue;
                }

                // If key exists and value is false dont display anything
                if ( ModEntry.modData.locationOfTownsfolkOptions[ key ] == false ) {
                    continue;
                }

                int offsetIconX = 0;
                int offsetIconY = 0;
                
                // Set the correct position for character head mugshots
                switch( npc.currentLocation.name ) {

                    case "Town":
                    case "JoshHouse":
                    case "HarveyRoom":
                        offsetIconX = 680;
                        offsetIconY = 360;
                        break;

                    case "HaleyHouse":
                        offsetIconX = 652;
                        offsetIconY = 408;
                        break;

                    case "SamHouse":
                        offsetIconX = 612;
                        offsetIconY = 396;
                        break;

                    case "Blacksmith":
                        offsetIconX = 852;
                        offsetIconY = 388;
                        break;

                    case "ManorHouse":
                        offsetIconX = 768;
                        offsetIconY = 388;
                        break;

                    case "SeedShop":
                        offsetIconX = 696;
                        offsetIconY = 296;
                        break;

                    case "Saloon":
                        offsetIconX = 716;
                        offsetIconY = 352;
                        break;

                    case "Farm":
                    case "FarmHouse":
                        offsetIconX = 470;
                        offsetIconY = 260;
                        break;

                    case "Trailer":
                        offsetIconX = 780;
                        offsetIconY = 360;
                        break;

                    case "Hospital":
                        offsetIconX = 680;
                        offsetIconY = 304;
                        break;
                                        
                    case "Beach":
                        offsetIconX = 790;
                        offsetIconY = 550;
                        break;

                    case "ElliottHouse":
                        offsetIconX = 826;
                        offsetIconY = 550;
                        break;

                    case "ScienceHouse":
                    case "SebastianRoom":
                        offsetIconX = 732;
                        offsetIconY = 148;
                        break;

                    case "Mountain":
                        offsetIconX = 762;
                        offsetIconY = 154;
                        break;

                    case "Tent":
                        offsetIconX = 784;
                        offsetIconY = 128;
                        break;

                    case "Forest":
                        offsetIconX = 80;
                        offsetIconY = 272;
                        break;

                    case "WizardHouseBasement":
                    case "WizardHouse":
                        offsetIconX = 196;
                        offsetIconY = 352;
                        break;

                    case "AnimalShop":
                        offsetIconX = 420;
                        offsetIconY = 392;
                        break;

                    case "LeahHouse":
                        offsetIconX = 452;
                        offsetIconY = 436;
                        break;

                    case "BusStop":
                        offsetIconX = 516;
                        offsetIconY = 224;
                        break;

                    case "Mine":
                        offsetIconX = 880;
                        offsetIconY = 100;
                        break;

                    case "Sewer":
                        offsetIconX = 380;
                        offsetIconY = 596;
                        break;
                    
                    case "Club":
                    case "Desert":
                        offsetIconX = 60;
                        offsetIconY = 92;
                        break;
                    
                    case "ArchaeologyHouse":
                        offsetIconX = 892;
                        offsetIconY = 416;
                        break;                    

                    case "Woods":
                        offsetIconX = 100;
                        offsetIconY = 272;
                        break;

                    case "Railroad":
                        offsetIconX = 644;
                        offsetIconY = 64;
                        break;
                    
                    case "FishShop":
                        offsetIconX = 844;
                        offsetIconY = 608;
                        break;

                    case "BathHouse_Entry":
                    case "BathHouse_MensLocker":
                    case "BathHouse_WomensLocker":
                    case "BathHouse_Pool":
                        offsetIconX = 576;
                        offsetIconY = 60;
                        break;

                    case "CommunityCenter":
                        offsetIconX = 692;
                        offsetIconY = 204;
                        break;

                    case "JojaMart":
                        offsetIconX = 872;
                        offsetIconY = 280;
                        break;
                    
                    case "Backwoods":
                        offsetIconX = 460;
                        offsetIconY = 156;
                        break;

                    case "SandyHouse":
                        offsetIconX = 40;
                        offsetIconY = 40;
                        break;

                    // Known locations that npcs never use
                    case "BugLand":
                    case "Greenhouse":
                    case "SkullCave":
                    case "Tunnel":
                    case "Cellar":
                    case "WitchSwamp":
                    case "WitchHut":
                    case "WitchWarpCave":
                    case "Summit":
                    case "AdentureGuild":
                    default:
                        ModEntry.Log( $"The location {npc.currentLocation.name} is not set" );
                        break;

                }

                Rectangle rect = getHeadShot( npc );

                int positionX = Game1.activeClickableMenu.xPositionOnScreen - 158;
                int positionY = Game1.activeClickableMenu.yPositionOnScreen - 40;

                float scale = 2.3f;

                int iconPositionX = positionX + offsetIconX;
                int iconPositionY = positionY + offsetIconY;

                Stack<StardewValley.Dialogue> currentDialogue =  ModEntry.helper.Reflection.GetPrivateField<Stack<StardewValley.Dialogue>>( npc, "currentDialogue" ).GetValue();

                Color tint;
                if( currentDialogue.Count > 0 ) {
                    tint = Color.White;
                } else {
                    tint = Color.Gray;
                }

                //TOOD change to just a drawable texture
                var npcMugShot = new ClickableTextureComponent( npc.name, new Rectangle( iconPositionX, iconPositionY, 0, 0 ), null, npc.name, npc.sprite.Texture, rect, scale, false );
                Game1.spriteBatch.Draw( npc.sprite.Texture, new Vector2( iconPositionX, iconPositionY ), rect, tint, 0, Vector2.Zero, 2f, SpriteEffects.None, 1 );

                // Draw quest icon
                foreach( var item in Game1.player.questLog ) {
                    if( item.accepted && item.dailyQuest && !item.completed ) {
                        bool hasQuest = false;
                        if( item.questType == 3 ) {
                            var current = ( ItemDeliveryQuest ) item;
                            if( current.target == npc.name ) {
                                hasQuest = true;
                            }
                        } else if( item.questType == 4 ) {
                            var current = ( SlayMonsterQuest ) item;
                            if( current.target == npc.name ) {
                                hasQuest = true;
                            }
                        } else if( item.questType == 7 ) {
                            var current = ( FishingQuest ) item;
                            if( current.target == npc.name ) {
                                hasQuest = true;
                            }
                        } else if( item.questType == 10 ) {
                            var current = ( ResourceCollectionQuest ) item;
                            if( current.target == npc.name ) {
                                hasQuest = true;
                            }
                        }

                        if( hasQuest ) {
                            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( iconPositionX+10, iconPositionY - 12 ), new Rectangle(1578/4,1983/4,4,10), Color.White, 0, Vector2.Zero, 3, SpriteEffects.None, 1 );
                        }
                    }
                }
            }

            // ReDraw the mouse
            if( !Game1.options.hardwareCursor ) {
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            }

            // ReDraw the tooltip
            var listOfPages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( currentMenu );
            var mapPage = ( MapPage ) listOfPages[ currentMenu.currentTab ];
            var defaultHoverText = ( string ) typeof( MapPage ).GetField( "hoverText", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( mapPage );

            IClickableMenu.drawHoverText( Game1.spriteBatch, defaultHoverText, Game1.smallFont, 0, 0, -1, ( string ) null, -1, ( string[] ) null, ( Item ) null, 0, -1, -1, -1, -1, 1f, ( CraftingRecipe ) null );
        }

        /// <summary>
        /// Gets the rectangle for the correct head shot of the character
        /// </summary>
        /// <param name="npc">The townfolk to check</param>
        /// <returns>The area for the headshot</returns>
        public static Rectangle getHeadShot( NPC npc ) {
            int cropFactor = 0;

            switch( npc.name ) {
                case "Abigail":
                    cropFactor = 7;
                    break;
                case "Alex":
                    cropFactor = 8;
                    break;
                case "Caroline":
                    cropFactor = 5;
                    break;
                case "Clint":
                    cropFactor = 10;
                    break;
                case "Demetrius":
                    cropFactor = 11;
                    break;
                case "Dwarf":
                    cropFactor = 8;
                    break;
                case "Elliott":
                    cropFactor = 9;
                    break;
                case "Emily":
                    cropFactor = 8;
                    break;
                case "Evelyn":
                    cropFactor = 5;
                    break;
                case "George":
                    cropFactor = 5;
                    break;
                case "Gus":
                    cropFactor = 7;
                    break;
                case "Haley":
                    cropFactor = 6;
                    break;
                case "Harvey":
                    cropFactor = 9;
                    break;
                case "Jas":
                    cropFactor = 6;
                    break;
                case "Jodi":
                    cropFactor = 7;
                    break;
                case "Kent":
                    cropFactor = 10;
                    break;
                case "Krobus":
                    cropFactor = 7;
                    break;
                case "Leah":
                    cropFactor = 6;
                    break;
                case "Lewis":
                    cropFactor = 8;
                    break;
                case "Linus":
                    cropFactor = 4;
                    break;
                case "Marnie":
                    cropFactor = 5;
                    break;
                case "Maru":
                    cropFactor = 6;
                    break;
                case "Pam":
                    cropFactor = 5;
                    break;
                case "Penny":
                    cropFactor = 6;
                    break;
                case "Pierre":
                    cropFactor = 9;
                    break;
                case "Robin":
                    cropFactor = 7;
                    break;
                case "Sandy":
                    cropFactor = 7;
                    break;
                case "Sam":
                    cropFactor = 9;
                    break;
                case "Sebastian":
                    cropFactor = 7;
                    break;
                case "Shane":
                    cropFactor = 8;
                    break;
                case "Vincent":
                    cropFactor = 6;
                    break;
                case "Willy":
                    cropFactor = 10;
                    break;
                case "Marlon":
                    cropFactor = 2;
                    break;
                case "Wizard":
                    cropFactor = 9;
                    break;
                // Child name is the only unknown
                default:
                    cropFactor = 4;
                    break;
            }

            Rectangle rect = npc.getMugShotSourceRect();
            rect.Height -= cropFactor / 2;
            rect.Y -= cropFactor / 2;

            return rect;
        }

        /// <summary>
        /// Draws the options in the social pane
        /// </summary>
        private void drawSocialPageOptions( object sender, EventArgs e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            var currentMenu = ( GameMenu ) Game1.activeClickableMenu;

            if( currentMenu.currentTab != GameMenu.socialTab ) {
                return;
            }

            // Draw Panel
            Game1.drawDialogueBox( Game1.activeClickableMenu.xPositionOnScreen - socialPanelOffsetX, Game1.activeClickableMenu.yPositionOnScreen, socialPanelWidth, Game1.activeClickableMenu.height, false, true );

            // Draw Content
            var slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );
            int offsetY = 0;
            for( int i = slotPosition; i < slotPosition + 5; i++ ) {

                // Safety check... this exists inside the client so lets reproduce here just in case
                if( i > friendNames.Count ) {
                    return;
                }

                // Set Checkbox position - TODO this should be removed from the drawing method if possible
                checkboxes[ i ].bounds.X = Game1.activeClickableMenu.xPositionOnScreen - 60;
                checkboxes[ i ].bounds.Y = Game1.activeClickableMenu.yPositionOnScreen + 130 + offsetY;

                // Draw Checkbox
                checkboxes[ i ].draw( Game1.spriteBatch, 0, 0 );
                offsetY += 112;

                // Set color for magnifying glass
                Color magnifyingGlassColor = Color.Gray;
                if( checkboxes[ i ].isChecked ) {
                    magnifyingGlassColor = Color.White;
                }

                // Draw Magnifying glasses
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y ), new Rectangle( 80, 0, 16, 16 ), magnifyingGlassColor, 0f, Vector2.Zero, 3, SpriteEffects.None, 1f );

                // Draw line below boxes omitting the last box... Hacky but W/E
                if( offsetY != 560 ) {
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y + 72, socialPanelWidth / 2 - 6, 4 ), Color.SaddleBrown );
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y + 76, socialPanelWidth / 2 - 6, 4 ), Color.BurlyWood );
                }

                // ReDraw the mouse
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

                // Draw tooltip
                if( checkboxes[i].bounds.Contains( Game1.getMouseX(), Game1.getMouseY() ) ) {
                    IClickableMenu.drawHoverText( Game1.spriteBatch, $"Track on map", Game1.dialogueFont );
                }
            }
        }

        /// <summary>
        /// Resets and populates the list of townsfolk and checkboxes to display every time the game menu is called
        /// </summary>
        private void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            // Get pages from GameMenu            
            var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );

            // Save variables needed for mod 
            for( int k = 0; k < pages.Count; k++ ) {
                if( pages[ k ] is SocialPage ) {
                    socialPage = ( SocialPage ) pages[ k ];
                    friendNames = ( List<ClickableTextureComponent> ) typeof( SocialPage ).GetField( "friendNames", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( pages[ k ] );
                }
            }

            townsfolk.Clear();
            foreach( GameLocation location in Game1.locations ) {
                foreach( NPC npc in location.characters ) {
                    if( Game1.player.friendships.ContainsKey( npc.name ) ) {
                        townsfolk.Add( npc );
                    }
                }
            }

            checkboxes.Clear();

            // Create checkboxes in the same order as friends are
            foreach( ClickableTextureComponent friend in friendNames ) {

                int optionIndex = friend.name.GetHashCode();

                var checkbox = new OptionsCheckbox( "", optionIndex );
                checkboxes.Add( checkbox );

                // Disable checkbox if player has not talked to npc yet
                if( !( Game1.player.friendships.ContainsKey( friend.name ) ) ) {
                    checkbox.greyedOut = true;
                    checkbox.isChecked = false;
                }

                // Ensure an entry exists
                if( ModEntry.modData.locationOfTownsfolkOptions.ContainsKey( optionIndex ) == false ) {
                    ModEntry.modData.locationOfTownsfolkOptions.Add( optionIndex, false );
                }

                checkbox.isChecked = ModEntry.modData.locationOfTownsfolkOptions[ optionIndex ];
            }
        }

        /// <summary>
        /// Handle checkbox clicks on the social page
        /// </summary>
        private void handleClickForSocialPage( object sender, EventArgsMouseStateChanged e ) {
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            if( e.NewState.LeftButton == ButtonState.Pressed && !isClicking ) {
                isClicking = true;
                var slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );

                for( int i = slotPosition; i < slotPosition + 5; i++ ) {
                    if( checkboxes[ i ].bounds.Contains( Game1.getMouseX(), Game1.getMouseY() ) && checkboxes[ i ].greyedOut == false ) {
                        checkboxes[ i ].isChecked = !checkboxes[ i ].isChecked;
                        ModEntry.modData.locationOfTownsfolkOptions[ checkboxes[ i ].whichOption ] = checkboxes[ i ].isChecked;
                        Game1.playSound( "drumkit6" );
                    }
                }

            }

            if ( e.NewState.LeftButton == ButtonState.Released) {
                isClicking = false;
            }
        }

    }
}
