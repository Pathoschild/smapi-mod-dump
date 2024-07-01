/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

namespace Lidgren.Network;

public partial class NetPeer
{
	private sealed class ReceivedFragmentGroup
	{
		//public float LastReceived;
		public byte[] Data { get; }
		public NetBitVector ReceivedChunks { get; }

		public ReceivedFragmentGroup(byte[] data, NetBitVector receivedChunks)
		{
			Data = data;
			ReceivedChunks = receivedChunks;
		}
	}
}