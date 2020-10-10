/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewEditor
**
*************************************************/

using StardewValley;
using System;
using System.IO;
using System.Text;

namespace StardewEditorGameIntegration.Packets
{
    public class PlayEventPacket : Packet
    {
        public string Location { get; set; }
        public string Data { get; set; }
        
        public override void write(BinaryWriter writer)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Location);
            writer.Write(Util.swapIfLittleEndian((uint)bytes.Length));
            writer.Write(bytes);

            bytes = Encoding.ASCII.GetBytes(Data);
            writer.Write(Util.swapIfLittleEndian((uint)bytes.Length));
            writer.Write(bytes);
        }

        public override void process(BinaryReader reader)
        {
            uint len = Util.swapIfLittleEndian(reader.ReadUInt32());
            byte[] bytes = reader.ReadBytes((int)len);
            Location = Encoding.ASCII.GetString(bytes);

            len = Util.swapIfLittleEndian(reader.ReadUInt32());
            bytes = reader.ReadBytes((int)len);
            Data = Encoding.ASCII.GetString(bytes);

            Log.debug("Triggering event @ " + Location + ": " + Data);
            Game1.getLocationFromName(Location).currentEvent = new Event(Data);
            Game1.warpFarmer(Location, 8, 8, false);
        }
    }
}