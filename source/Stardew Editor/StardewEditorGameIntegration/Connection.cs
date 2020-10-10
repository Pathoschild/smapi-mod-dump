/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewEditor
**
*************************************************/

using StardewEditorGameIntegration.Packets;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditorGameIntegration
{
    internal class Connection
    {
        public const int PORT = 24700;
        private TcpClient socket;

        private int pendingLen = -1;
        private int readLen = 0;
        private byte[] data;

        public Connection()
        {
            socket = new TcpClient();
            new Task(() =>
            {
                try
                {
                    Log.info("Connecting to editor...");
                    socket.Connect("127.0.0.1", PORT);
                    Log.info("Success!");
                    GameEvents.UpdateTick += update;
                }
                catch (Exception e)
                {
                    Log.warn("Failed to connet to editor! " + e);
                    Mod.instance.conn = null;
                }
            }).Start();
        }

        public void update( object sender, EventArgs args )
        {
            if (!socket.Connected)
            {
                Log.warn("Lost connection to editor!");
                GameEvents.UpdateTick -= update;
                Mod.instance.conn = null;
                return;
            }
        
            Stream stream = socket.GetStream();
            if ( pendingLen == -1 )
            {
                if ( data == null || data.Length != 4 )
                {
                    data = new byte[4];
                    readLen = 0;
                }

                if (socket.Available <= 0)
                    return;
                readLen += stream.Read(data, readLen, 4 - readLen);
                if ( readLen == 4 )
                {
                    pendingLen = (int)Util.swapIfLittleEndian(BitConverter.ToUInt32(data, 0));
                    readLen = 0;
                    data = new byte[pendingLen];
                }
            }
            if ( pendingLen != -1 )
            {
                if (socket.Available <= 0)
                    return;
                readLen += stream.Read(data, readLen, pendingLen - readLen);

                if ( readLen == pendingLen )
                {
                    Packet.process(data);
                    readLen = 0;
                    pendingLen = -1;
                }
            }
        }
    }
}
