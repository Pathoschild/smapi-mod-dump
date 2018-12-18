using Microsoft.Xna.Framework;
using Netcode;
using StardustCore.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.NetCode.NetPairs
{
    public class NetVector2MultiTilePair<K,KField>: NetKeyValuePair<Vector2,MultiTileComponent,Netcode.NetVector2,NetCode.Objects.NetMultiTileComponent>
    {

        public NetVector2MultiTilePair()
        {

        }

        public NetVector2MultiTilePair(KeyValuePair<Vector2,MultiTileComponent> hello)
        {
            this.Set(Value);
        }

        public override void Read(BinaryReader reader, NetVersion version)
        {
            base.Read(reader, version);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
        }
    }
}
