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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;

namespace MoonMisadventures.Game.Locations
{
    [XmlType( "Mods_spacechase0_MoonMisadventures_LunarFarmHouse" )]
    public class LunarFarmHouse : LunarLocation
    {
        public readonly NetBool visited = new();

        public LunarFarmHouse()
        {
        }

        public LunarFarmHouse( IContentHelper content )
        : base( content, "MoonFarmHouse", "MoonFarmHouse" )
        {
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields( visited );

            visited.InterpolationEnabled = false;
            visited.fieldChangeVisibleEvent += delegate { SpawnBeds(); };
        }

        public override void TransferDataFromSavedLocation( GameLocation l )
        {
            visited.Value = ( l as LunarFarmHouse ).visited.Value;
            base.TransferDataFromSavedLocation( l );
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            visited.Value = true;
            Game1.background = null;
        }

        private void SpawnBeds()
        {
            if ( !visited.Value )
                return;

            int players = 0;
            foreach ( var farmer in Game1.getAllFarmers() )
                ++players;

            furniture.Add( new BedFurniture( 2048, new Vector2( 4, 4 ) ) );
            if ( --players > 0 )
            {
                furniture.Add( new BedFurniture( 2048, new Vector2( 4, 8 ) ) );
                if ( --players > 0 )
                {
                    furniture.Add( new BedFurniture( 2048, new Vector2( 12, 4 ) ) );
                    if ( --players > 0 )
                        furniture.Add( new BedFurniture( 2048, new Vector2( 12, 8 ) ) );
                }
            }
        }
    }
}
