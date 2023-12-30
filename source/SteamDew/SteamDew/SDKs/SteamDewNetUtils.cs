/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/myuusubi/SteamDew
**
*************************************************/

using StardewValley.Network;
using Steamworks;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SteamDew.SDKs {

public class SteamDewNetUtils {

internal enum SDMsgType : byte {
	Compressed = 127
}

private const bool IncreaseBufferSize = true;
private const int CompressThreshold = 1024;

public static int GetNetworkingOptions(out SteamNetworkingConfigValue_t[] pOptions) {
	pOptions = null;

	if (SteamDewNetUtils.IncreaseBufferSize) {
		pOptions = new SteamNetworkingConfigValue_t[1];

		pOptions[0].m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize;
		pOptions[0].m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32;
		pOptions[0].m_val.m_int32 = 1024 * 1024;
	}

	if (pOptions == null) {
		return 0;
	}

	return pOptions.Length;
}

private static byte[] DecompressMessage(SteamNetworkingMessage_t msgSteam)
{
	int headerSize = sizeof(byte) + sizeof(int);
	int srcSize = msgSteam.m_cbSize - headerSize;

	IntPtr src = msgSteam.m_pData;
	src = IntPtr.Add(src, sizeof(byte));

	byte[] sizeBytes = new byte[sizeof(int)];
	Marshal.Copy(src, sizeBytes, 0, sizeof(int));
	int dstSize = BitConverter.ToInt32(sizeBytes);
	src = IntPtr.Add(src, sizeof(int));

	IntPtr dest = Marshal.AllocHGlobal(dstSize);

	LZ4.decompress_safe(src, dest, srcSize, dstSize);

	byte[] msgBytes = new byte[dstSize];
	Marshal.Copy(dest, msgBytes, 0, dstSize);

	Marshal.FreeHGlobal(dest);

	return msgBytes;
}

public static void HandleSteamMessage(IntPtr msgPtr, IncomingMessage msg, out HSteamNetConnection msgConn, BandwidthLogger bandwidthLogger)
{
	SteamNetworkingMessage_t msgSteam = Marshal.PtrToStructure<SteamNetworkingMessage_t>(msgPtr);

	msgConn = msgSteam.m_conn;

	byte[] msgType = new byte[1];
	Marshal.Copy(msgSteam.m_pData, msgType, 0, 1);

	byte[] msgBytes = null;
	
	switch (msgType[0]) {
	case ((byte) SDMsgType.Compressed):
		msgBytes = SteamDewNetUtils.DecompressMessage(msgSteam);
		break;
	default:
		msgBytes = new byte[msgSteam.m_cbSize];
		Marshal.Copy(msgSteam.m_pData, msgBytes, 0, msgBytes.Length);
		break;
	}

	MemoryStream msgStream = new MemoryStream(msgBytes);
	msgStream.Position = 0L;

	BinaryReader msgReader = new BinaryReader(msgStream);

	msg.Read(msgReader);

	SteamNetworkingMessage_t.Release(msgPtr);

	if (bandwidthLogger == null) {
		return;
	}

	bandwidthLogger.RecordBytesDown(msgStream.Length);
}

public static void SendMessage(HSteamNetConnection msgConn, OutgoingMessage msg, BandwidthLogger bandwidthLogger)
{
	MemoryStream msgStream = new MemoryStream();
	BinaryWriter msgWriter = new BinaryWriter(msgStream);
	msg.Write(msgWriter);

	msgStream.Seek(0L, SeekOrigin.Begin);
	byte[] msgBytes = msgStream.ToArray();

	int srcSize = Convert.ToInt32(msgBytes.Length);

	long pOutMessageNumber;

	IntPtr src = Marshal.AllocHGlobal(srcSize);
	Marshal.Copy(msgBytes, 0, src, srcSize);

	IntPtr msgPtr = src;
	int msgSize = srcSize;

	if (srcSize >= SteamDewNetUtils.CompressThreshold) {
		const int headerSize = sizeof(byte) + sizeof(int);
		int destSize = LZ4.compressBound(srcSize);
		IntPtr buffer = Marshal.AllocHGlobal(headerSize + destSize);
		IntPtr dest = IntPtr.Add(buffer, headerSize);

		int compressedSize = LZ4.compress_default(src, dest, srcSize, destSize);

		Marshal.FreeHGlobal(src);

		IntPtr header = buffer;

		byte[] typeBytes = new byte[1] { (byte) SDMsgType.Compressed };
		Marshal.Copy(typeBytes, 0, header, sizeof(byte));
		header = IntPtr.Add(header, sizeof(byte));

		byte[] sizeBytes = BitConverter.GetBytes(srcSize);
		Marshal.Copy(sizeBytes, 0, header, sizeof(int));

		msgPtr = buffer;
		msgSize = headerSize + compressedSize;
	}

	EResult result = SteamNetworkingSockets.SendMessageToConnection(
		msgConn, 
		msgPtr, 
		Convert.ToUInt32(msgSize), 
		Constants.k_nSteamNetworkingSend_Reliable, 
		out pOutMessageNumber
	);
	
	Marshal.FreeHGlobal(msgPtr);

	if (result != EResult.k_EResultOK) {
		SteamDew.Log("Failed to send message. Closing connection.");
		SteamDewNetUtils.CloseConnection(msgConn);
		return;
	}

	if (bandwidthLogger == null) {
		return;
	}

	bandwidthLogger.RecordBytesUp(msgSize);
}

public static void CloseConnection(HSteamNetConnection conn)
{
	if (conn == HSteamNetConnection.Invalid) {
		return;
	}
	SteamNetworkingSockets.SetConnectionPollGroup(conn, HSteamNetPollGroup.Invalid);
	SteamNetworkingSockets.CloseConnection(conn, (int) ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_App_Generic, null, true);
}

} /* class SteamDewNetUtils */

} /* namespace SteamDew.SDKs */