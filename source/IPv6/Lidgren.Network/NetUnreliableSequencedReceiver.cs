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
	internal sealed class NetUnreliableSequencedReceiver : NetReceiverChannelBase
	{
		private int m_lastReceivedSequenceNumber = -1;

		public NetUnreliableSequencedReceiver(NetConnection connection)
			: base(connection)
		{
		}

		internal override void ReceiveMessage(NetIncomingMessage msg)
		{
			int nr = msg.m_sequenceNumber;

			// ack no matter what
			m_connection.QueueAck(msg.m_receivedMessageType, nr);

			int relate = NetUtility.RelativeSequenceNumber(nr, m_lastReceivedSequenceNumber + 1);
			if (relate < 0)
			{
				m_connection.m_statistics.MessageDropped();
				m_peer.LogVerbose("Received message #" + nr + " DROPPING DUPLICATE");
				return; // drop if late
			}

			m_lastReceivedSequenceNumber = nr;
			m_peer.ReleaseMessage(msg);
		}
	}
}
