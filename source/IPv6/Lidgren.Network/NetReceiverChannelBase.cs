/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

using System;

namespace Lidgren.Network
{
	internal abstract class NetReceiverChannelBase
	{
		internal NetPeer m_peer;
		internal NetConnection m_connection;

		public NetReceiverChannelBase(NetConnection connection)
		{
			m_connection = connection;
			m_peer = connection.m_peer;
		}

		internal abstract void ReceiveMessage(NetIncomingMessage msg);
	}
}
