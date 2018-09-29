using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditorGameIntegration.Packets
{
    public abstract class Packet
    {
        enum Id
        {
            Protocol = 0,
            PlayEvent = 1,
        }

        public abstract void write(BinaryWriter writer);
        public abstract void process(BinaryReader reader);

        public static void process( byte[] data )
        {
            using (Stream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte id = reader.ReadByte();
                    Log.trace("Processing packet " + id + " (data:" + data.Length+ ")");
                    switch ( ( Id ) id )
                    {
                        case Id.PlayEvent:
                            new PlayEventPacket().process(reader);
                            break;
                    }
                }
            }
        }
    }
}
