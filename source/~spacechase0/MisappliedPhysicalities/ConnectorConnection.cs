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
using Microsoft.Xna.Framework;
using Netcode;

namespace MisappliedPhysicalities.Game.Network
{
    public class ConnectorConnection : INetObject<NetFields>
    {
        private readonly NetEnum<Layer> otherLayer = new();
        private readonly NetPoint otherPoint = new( new Point( -1, -1 ) );

        public Layer OtherLayer { get { return otherLayer.Value; } set { otherLayer.Value = value; } }
        public Point OtherPoint { get { return otherPoint.Value; } set { otherPoint.Value = value; } }

        public NetFields NetFields { get; } = new NetFields();

        public ConnectorConnection()
        {
            NetFields.AddFields( otherLayer, otherPoint );
        }

        public override bool Equals( object obj )
        {
            if ( obj is not ConnectorConnection conn )
                return false;

            return OtherLayer == conn.OtherLayer && OtherPoint == conn.OtherPoint;
        }
    }
}
