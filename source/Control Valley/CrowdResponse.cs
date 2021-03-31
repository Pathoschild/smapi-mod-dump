/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tesla1889tv/ControlValleyMod
**
*************************************************/

using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ControlValley
{
    public class CrowdResponse
    {
        public enum Status
        {
            STATUS_SUCCESS,
            STATUS_FAILURE,
            STATUS_UNAVAIL,
            STATUS_RETRY,
            STATUS_KEEPALIVE=255
        }

        public int id;
        public string message;
        public int status;

        public CrowdResponse(int id, Status status = Status.STATUS_SUCCESS, string message = "")
        {
            this.id = id;
            this.message = message;
            this.status = (int)status;
        }

        public static void KeepAlive(Socket socket)
        {
            new CrowdResponse(0, Status.STATUS_KEEPALIVE).Send(socket);
        }

        public void Send(Socket socket)
        {
            byte[] tmpData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
            byte[] outData = new byte[tmpData.Length + 1];
            Buffer.BlockCopy(tmpData, 0, outData, 0, tmpData.Length);
            outData[tmpData.Length] = 0;
            socket.Send(outData);
        }
    }
}
