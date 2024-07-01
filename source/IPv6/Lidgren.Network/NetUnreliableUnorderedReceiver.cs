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
	internal sealed class NetUnreliableUnorderedReceiver : NetReceiverChannelBase
	{
		private readonly bool m_doFlowControl;

		public NetUnreliableUnorderedReceiver(NetConnection connection)
			: base(connection)
		{
			m_doFlowControl = connection.Peer.Configuration.SuppressUnreliableUnorderedAcks == false;
		}

		internal override void ReceiveMessage(NetIncomingMessage msg)
		{
			if (m_doFlowControl)
				m_connection.QueueAck(msg.m_receivedMessageType, msg.m_sequenceNumber);

			m_peer.ReleaseMessage(msg);
		}
	}
}
