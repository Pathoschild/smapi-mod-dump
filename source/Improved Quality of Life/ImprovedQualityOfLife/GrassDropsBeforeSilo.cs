using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Demiacle.ImprovedQualityOfLife {
    internal class GrassDropsBeforeSilo {

        /// <summary>
        /// This mod allows hay to be collected from grass without a silo. It works by replacing a method via pointer replacement
        /// </summary>
        public GrassDropsBeforeSilo() {

            MethodInfo methodToReplace = typeof( Farm ).GetMethod( "tryToAddHay", BindingFlags.Instance | BindingFlags.Public );
            MethodInfo methodToInject = typeof( GrassDropsBeforeSilo ).GetMethod( "tryToAddHayMod", BindingFlags.Instance | BindingFlags.Public );
            RuntimeHelpers.PrepareMethod( methodToReplace.MethodHandle );
            RuntimeHelpers.PrepareMethod( methodToInject.MethodHandle );

            unsafe {
                // 32 bit
                if( IntPtr.Size == 4 ) {

                    int* inj = ( int* ) methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = ( int* ) methodToReplace.MethodHandle.Value.ToPointer() + 2;
                    *tar = *inj;

                // 64 bit just in case
                } else {

                    long* inj = ( long* ) methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = ( long* ) methodToReplace.MethodHandle.Value.ToPointer() + 1;
                    *tar = *inj;

                }

            }

        }

        /// <summary>
        /// This method replaces the one in Farm class and drops hay if a silo is either full or if the player doesn't have one
        /// </summary>
        /// <param name="num">The amount of hay gained from action</param>
        /// <returns>The amount of hay to be stored in the silo</returns>
        public int tryToAddHayMod( int num ) {

            // Method only exists in farm so will always be farm
            Farm farm = (Farm) Game1.getLocationFromName( "Farm" );

            float random = Game1.random.Next( 0, 2 );

            // Only drop hay at a 50/50 chance when grass is cut and silos are maxed
            if( farm.piecesOfHay >= Utility.numSilos() * 240 && random > 0.5f && num > 0) {
                farm.debris.Add( new Debris( 178, Game1.player.GetToolLocation(), Game1.player.position ) );
                return -1;
            }

            // Default behavior
            int num1 = Math.Min( Utility.numSilos() * 240 - farm.piecesOfHay, num );
            farm.piecesOfHay = farm.piecesOfHay + num1;
            return num - num1;
        }

    }
}