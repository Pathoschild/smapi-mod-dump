/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using System.IO;

namespace StardewValleyMP.Packets
{
    // Server -> Client
    // Send the client their ID.
    // Also signals them to send their farmer info.
    public class YourIDPacket : Packet
    {
        public byte clientId;

        public YourIDPacket() : base( ID.YourID )
        {
        }

        public YourIDPacket(byte theId)
            : this()
        {
            clientId = theId;
        }

        protected override void read(BinaryReader reader)
        {
            clientId = reader.ReadByte();
        }

        protected override void write(BinaryWriter writer)
        {
            writer.Write(clientId);
        }

        public override void process(Client client)
        {
            client.id = clientId;
        }

        public override string ToString()
        {
            return base.ToString() + " " + (int)clientId;
        }
    }
}
