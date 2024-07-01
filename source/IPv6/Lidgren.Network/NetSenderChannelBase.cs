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
	internal abstract class NetSenderChannelBase
	{
		// access this directly to queue things in this channel
		protected readonly NetQueue<NetOutgoingMessage> m_queuedSends;

		protected NetSenderChannelBase(NetQueue<NetOutgoingMessage> queuedSends)
		{
			m_queuedSends = queuedSends;
		}

		internal abstract int WindowSize { get; }

		internal abstract int GetAllowedSends();

		internal int QueuedSendsCount { get { return m_queuedSends.Count; } }

		internal virtual bool NeedToSendMessages() { return m_queuedSends.Count > 0; }

		public int GetFreeWindowSlots()
		{
			return GetAllowedSends() - m_queuedSends.Count;
		}

		internal abstract NetSendResult Enqueue(NetOutgoingMessage message);
		internal abstract void SendQueuedMessages(double now);
		internal abstract void Reset();
		internal abstract void ReceiveAcknowledge(double now, int sequenceNumber);
	}
}
