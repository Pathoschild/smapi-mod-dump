/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

// Stardew Valley, Version=1.6.8.24119, Culture=neutral, PublicKeyToken=null
// StardewValley.Network.LidgrenMessageUtils
extern alias slnet;
using slnet::Lidgren.Network;
using StardewValley.Network;

namespace IPv6.Patch.Classes;

/// <summary>A set of utilities for packing/unpacking data within Lidgren messages.</summary>
public static class LidgrenMessageUtils
{
    /// <summary>Write (and potentially compress) the message from <paramref name="srcMsg" /> into <paramref name="destMsg" />.</summary>
    /// <param name="srcMsg">The outgoing message to read data from.</param>
    /// <param name="destMsg">The net outgoing message to write (and potentially compress) data into.</param>
    internal static void WriteMessage(OutgoingMessage srcMsg, NetOutgoingMessage destMsg)
    {
        byte[] dataRaw;
        using (MemoryStream stream = new MemoryStream())
        {
            using BinaryWriter writer = new BinaryWriter(stream);
            srcMsg.Write(writer);
            dataRaw = stream.ToArray();
        }
        using MemoryStream srcStream = new MemoryStream(MyPatch.netCompression.CompressAbove(dataRaw, 1024));
        using NetBufferWriteStream destStream = new NetBufferWriteStream(destMsg);
        srcStream.CopyTo(destStream);
    }

    /// <summary>Reads a message from <paramref name="stream" /> into <paramref name="msg" />, and decompresses it if necessary.</summary>
    /// <param name="stream">The stream to read message data from.</param>
    /// <param name="msg">The message to write (and potentially decompress) data into.</param>
    internal static void ReadStreamToMessage(NetBufferReadStream stream, IncomingMessage msg)
    {
        Stream messageStream = stream;
        if (MyPatch.netCompression.TryDecompressStream(stream, out var decompressed))
        {
            messageStream = new MemoryStream(decompressed);
        }
        using BinaryReader reader = new BinaryReader(messageStream);
        msg.Read(reader);
    }
}
