using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Locations;

namespace Demiacle.ImprovedQualityOfLife {
    internal class ReduceHorseBoundingBox {

        bool isSpawned = false;

        Horse originalHorse;
        BetterHorse betterHorse = new BetterHorse();

        /// <summary>
        /// This 
        /// </summary>
        public ReduceHorseBoundingBox() {
            replaceHorse( null, null );

            SaveEvents.BeforeSave += restoreHorse;
            SaveEvents.AfterSave += replaceHorse;

            //LocationEvents.CurrentLocationChanged += replaceRegularHorse;
            //LocationEvents.CurrentLocationChanged += restoreRegularHorse;
        }

        [Obsolete( "Never fires" )]
        private void replaceRegularHorse( object sender, EventArgsCurrentLocationChanged e ) {
            if( e.NewLocation is Farm ) {
                replaceHorse( null, null );

            }
        }

        [Obsolete( "Never fires" )]
        private void restoreRegularHorse( object sender, EventArgsCurrentLocationChanged e ) {
            if( e.NewLocation is FarmHouse ) {
                restoreHorse(null,null);
            }
        }

        /// <summary>
        /// Restores the original horse before serialization
        /// </summary>
        private void restoreHorse( object sender, EventArgs e ) {

            foreach( var location in Game1.locations ) {

                for( int i = 0; i < location.characters.Count; i++ ) {
                    NPC npc = location.characters[ i ];
                    if( npc is BetterHorse ) {
                        originalHorse.name = betterHorse.name;
                        location.characters[ i ] = originalHorse;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces the original horse after load/save
        /// </summary>
        private void replaceHorse( object sender, EventArgs e ) {

            foreach( var location in Game1.locations ) {

                for( int i = 0; i < location.characters.Count; i++ ) {
                    NPC npc = location.characters[ i ];
                    if( npc is Horse && npc is BetterHorse == false ) {
                        originalHorse = ( Horse ) npc;

                        betterHorse.position = originalHorse.position;
                        betterHorse.name = originalHorse.name;
                        location.characters[ i ] = betterHorse;

                        // Only change one horse
                        return;
                    }
                }
            }
        }

        [Obsolete( "Never fires" )]
        private void spawnHorse( object sender, EventArgsMouseStateChanged e ) {
            if( e.NewState.RightButton == ButtonState.Pressed && isSpawned == false) {
                Game1.currentLocation.characters.Add( new Horse( (int) Game1.player.position.X / 64, (int) Game1.player.position.Y /64));
                isSpawned = true;
            }
        }

    }
}