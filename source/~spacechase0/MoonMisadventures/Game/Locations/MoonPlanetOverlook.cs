/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Xml.Serialization;
using DynamicGameAssets.Game;
using MoonMisadventures.Game.Items;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace MoonMisadventures.Game.Locations
{
    [XmlType( "Mods_spacechase0_MoonMisadventures_MoonPlanetOverlook" )]
    public class MoonPlanetOverlook : LunarLocation
    {
        public MoonPlanetOverlook() { }
        public MoonPlanetOverlook( IModContentHelper content )
        :   base( content, "MoonPlanetOverlook", "MoonPlanetOverlook" )
        {
        }

        public override bool isActionableTile( int xTile, int yTile, Farmer who )
        {
            if ( xTile >= 23 && xTile <= 27 && yTile >= 10 && yTile <= 14 )
                return true;
            return base.isActionableTile( xTile, yTile, who );
        }

        public override bool checkAction( xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who )
        {
            int xTile = tileLocation.X, yTile = tileLocation.Y;
            if ( xTile >= 23 && xTile <= 27 && yTile >= 10 && yTile <= 14 )
            {
                createQuestionDialogue(I18n.Message_Planet_Jump(), createYesNoResponses(), "PlanetJump" );
                return true;
            }

            return base.checkAction( tileLocation, viewport, who );
        }

        public override bool performAction( string action, Farmer who, Location tileLocation )
        {
            if ( action == "LunarNecklaceExchange" )
            {
                createQuestionDialogue(I18n.Message_NecklaceExchange(), createYesNoResponses(), "LunarNecklaceExchange" );
            }
            return base.performAction( action, who, tileLocation );
        }

        public override bool answerDialogue( Response answer )
        {
            if ( lastQuestionKey != null && afterQuestion == null )
            {
                string qa = lastQuestionKey.Split( ' ' )[ 0 ] + "_" + answer.responseKey;
                switch ( qa )
                {
                    case "LunarNecklaceExchange_Yes":
                        int necklaceSlot = -1, sapphireSlot = -1;
                        for ( int i = 0; i < Game1.player.Items.Count; ++i )
                        {
                            if ( necklaceSlot == -1 && Game1.player.Items[ i ] is Necklace necklace && necklace.necklaceType.Value == Necklace.Type.Lunar )
                                necklaceSlot = i;
                            else if ( sapphireSlot == -1 && Game1.player.Items[ i ] is CustomObject cobj && cobj.FullId == ItemIds.SoulSapphire )
                                sapphireSlot = i;
                        }

                        if ( necklaceSlot == -1 || sapphireSlot == -1 )
                        {
                            Game1.drawObjectDialogue(I18n.Message_NecklaceExchange_Lacking());
                        }
                        else
                        {
                            Game1.player.Items[ necklaceSlot ] = null;
                            if ( Game1.player.Items[ sapphireSlot ].Stack-- == 1 )
                                Game1.player.Items[ sapphireSlot ] = null;
                            var necklace = new Necklace( PickCombatNecklace() );
                            //var debris = new Debris( necklace, new Vector2( Game1.player.getStandingX(), 128 ) );
                            //Game1.player.currentLocation.debris.Add( debris );
                            Game1.player.addItemByMenuIfNecessary(necklace);
                            Game1.player.holdUpItemThenMessage(necklace);
                            Game1.playSound( "stardrop" );
                        }
                        return true;
                    case "PlanetJump_Yes":
                        Game1.player.synchronizedJump( 8 );
                        Game1.player.health = 1;
                        Game1.warpFarmer( "Town", 28, 67, 0 );
                        return true;
                }
            }

            return base.answerDialogue( answer );
        }

        private static Necklace.Type PickCombatNecklace()
        {
            // Basically everything except another lunar necklace
            switch ( Game1.random.Next( 7 ) )
            {
                case 0: return Necklace.Type.Looting;
                case 1: return Necklace.Type.Shocking;
                case 2: return Necklace.Type.Speed;
                case 3: return Necklace.Type.Health;
                case 4: return Necklace.Type.Cooling;
                case 5: return Necklace.Type.Water;
                case 6: return Necklace.Type.Sea;
            }
            throw new Exception( "This should never happen" );
        }
    }
}
