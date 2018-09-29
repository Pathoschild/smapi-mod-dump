using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using xTile.Tiles;
using xTile.Layers;
using xTile.ObjectModel;
using StardewValley.TerrainFeatures;
using SFarmer = StardewValley.Farmer;

namespace Demiacle.ImprovedQualityOfLife {
    internal class RestoreStaminaOnToolFail {

        private float restoreAmount = 0;

        private int toolPower;
        private bool hasCaluclatedRestore = false;

        /// <summary>
        /// This mod is used to restore stamina on failed tool uses - Currently on hold
        /// </summary>
        public RestoreStaminaOnToolFail() {
            GameEvents.UpdateTick += checkForToolAction;
        }

        private void checkForToolAction( object sender, EventArgs e ) {
            SFarmer player = Game1.player;

            if( player.usingTool ) {
                toolPower = player.toolPower;
                if( hasCaluclatedRestore == true ) {
                    return;
                }

                hasCaluclatedRestore = true;
                Tool currentTool = player.CurrentTool;
                Vector2 toolLocation = player.GetToolLocation() / Game1.tileSize;
                toolLocation.X = (int) Math.Floor( toolLocation.X );
                toolLocation.Y = (int) Math.Floor( toolLocation.Y );
                var toolLocationX = (int) toolLocation.X;
                var toolLocationY = (int) toolLocation.Y;

                if( currentTool is Hoe ) {

                    // Check if you can even dig
                    if( Game1.currentLocation.doesTileHaveProperty( toolLocationX, toolLocationY, "Diggable", "Back" ) == null ) {
                        return;
                    }

                    // Check if already dug
                    if( Game1.currentLocation.isTileHoeDirt( toolLocation ) == false ) {
                        return; 
                    }

                    restoreAmount = 2 + ( float ) player.FarmingLevel * 0.1f;

                }

                if( currentTool is Axe ) {

                    // Check for trees
                    if( Game1.currentLocation.terrainFeatures.ContainsKey( toolLocation ) && 
                        Game1.currentLocation.terrainFeatures[ toolLocation ] is Tree ) {
                        return;
                    }

                    // Check for twigs or weeds
                    if( Game1.currentLocation.objects.ContainsKey( toolLocation ) && ( Game1.currentLocation.objects[ toolLocation ].name == "Twig" || Game1.currentLocation.objects[ toolLocation ].name == "Weeds" ) ) {
                        return;
                    }

                    // Check for advanced tree stumps
                    if( Game1.currentLocation is Farm ) {
                        var toolHitBounds = new Rectangle( toolLocationX * Game1.tileSize, toolLocationY * Game1.tileSize, Game1.tileSize, Game1.tileSize );
                        foreach( var item in (Game1.currentLocation as Farm).resourceClumps ) {
                            if( item.getBoundingBox( item.tile ).Contains( toolHitBounds  ) ) {
                                return;
                            }
                        }
                    } 

                    restoreAmount = 2 + ( float ) player.ForagingLevel * 0.1f;

                }
                
                if( currentTool is Pickaxe ) {


                    if( Game1.currentLocation.objects.ContainsKey( toolLocation ) ) {

                        StardewValley.Object groundObject = Game1.currentLocation.objects[ toolLocation ];

                        // Check if stone or boulder
                        if( groundObject.name.Contains( "Stone" ) || groundObject.name.Contains( "Boulder" ) ) {
                            return;
                        }

                        // Check if crafting object
                        if( groundObject.type.Equals( "Crafting" ) && groundObject.fragility != 2 ) {
                            return;
                        }

                    }

                    // Check if removing hoedirt
                    if( ( Game1.currentLocation.doesTileHaveProperty( toolLocationX, toolLocationY, "Diggable", "Back" ) == null ) ) {
                        return;
                    }

                    restoreAmount = 2 + ( float ) player.miningLevel * 0.1f;

                }

                if( currentTool is WateringCan ) {

                    // Check if watering hoedirt
                    if( ( Game1.currentLocation.isTileHoeDirt( toolLocation ) == true ) ) {
                        return;
                    }

                    restoreAmount = 2 + ( float ) player.farmingLevel * 0.1f;

                }

                if( currentTool is FishingRod ) {

                    // Check for water
                    if( Game1.currentLocation.doesTileHaveProperty( toolLocationX, toolLocationY, "Water", "Back" ) != null && Game1.currentLocation.doesTileHaveProperty( toolLocationX, toolLocationY, "NoFishing", "Back" ) == null && Game1.currentLocation.getTileIndexAt( toolLocationX, toolLocationY, "Buildings" ) == -1 || Game1.currentLocation.doesTileHaveProperty( toolLocationX, toolLocationY, "Water", "Buildings" ) != null) {
                        return;
                    }

                    restoreAmount = 2 + ( float ) ( 8.0 - ( double ) player.FishingLevel * 0.100000001490116 );

                }

            } else if( toolPower == 0 && restoreAmount > 0 ) {

                Game1.player.stamina += restoreAmount;

                toolPower = -1;
                restoreAmount = -1;
                hasCaluclatedRestore = false;
            } else if( hasCaluclatedRestore ) {
                hasCaluclatedRestore = false;
            }



        }
    }
}