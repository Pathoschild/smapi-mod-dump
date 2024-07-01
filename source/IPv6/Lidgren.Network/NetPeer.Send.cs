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
using System.Collections.Generic;
using System.Threading;
using System.Net;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
	public partial class NetPeer
	{
		/// <summary>
		/// Send a message to a specific connection
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="recipient">The recipient connection</param>
		/// <param name="method">How to deliver the message</param>
		public NetSendResult SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method)
		{
			return SendMessage(msg, recipient, method, 0);
		}

		/// <summary>
		/// Send a message to a specific connection
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="recipient">The recipient connection</param>
		/// <param name="method">How to deliver the message</param>
		/// <param name="sequenceChannel">Sequence channel within the delivery method</param>
		public NetSendResult SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method, int sequenceChannel)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipient == null)
				throw new ArgumentNullException("recipient");
			if (sequenceChannel >= NetConstants.NetChannelsPerDeliveryMethod)
				throw new ArgumentOutOfRangeException("sequenceChannel");

			NetException.Assert(
				((method != NetDeliveryMethod.Unreliable && method != NetDeliveryMethod.ReliableUnordered) ||
				((method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.ReliableUnordered) && sequenceChannel == 0)),
				"Delivery method " + method + " cannot use sequence channels other than 0!"
			);

			NetException.Assert(method != NetDeliveryMethod.Unknown, "Bad delivery method!");

			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			msg.m_isSent = true;

			bool suppressFragmentation = (method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.UnreliableSequenced) && m_configuration.UnreliableSizeBehaviour != NetUnreliableSizeBehaviour.NormalFragmentation;

			int len = NetConstants.UnfragmentedMessageHeaderSize + msg.LengthBytes; // headers + length, faster than calling msg.GetEncodedSize
			if (len <= recipient.m_currentMTU || suppressFragmentation)
			{
				Interlocked.Increment(ref msg.m_recyclingCount);
				return recipient.EnqueueMessage(msg, method, sequenceChannel);
			}
			else
			{
				// message must be fragmented!
				if (recipient.m_status != NetConnectionStatus.Connected)
					return NetSendResult.FailedNotConnected;
				return SendFragmentedMessage(msg, new NetConnection[] { recipient }, method, sequenceChannel);
			}
		}

		internal static int GetMTU(IList<NetConnection> recipients)
		{
			int count = recipients.Count;

			int mtu = int.MaxValue;
			if (count < 1)
			{
#if DEBUG
				throw new NetException("GetMTU called with no recipients");
#else
				// we don't have access to the particular peer, so just use default MTU
				return NetPeerConfiguration.kDefaultMTU;
#endif
			}

			for (int i = 0; i < count; i++)
			{
				var conn = recipients[i];
				int cmtu = conn.m_currentMTU;
				if (cmtu < mtu)
					mtu = cmtu;
			}
			return mtu;
		}

		/// <summary>
		/// Send a message to a list of connections
		/// </summary>
		/// <param name="msg">The message to send</param>
		/// <param name="recipients">The list of recipients to send to</param>
		/// <param name="method">How to deliver the message</param>
		/// <param name="sequenceChannel">Sequence channel within the delivery method</param>
		public void SendMessage(NetOutgoingMessage msg, IList<NetConnection> recipients, NetDeliveryMethod method, int sequenceChannel)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipients == null)
			{
				if (msg.m_isSent == false)
					Recycle(msg);
				throw new ArgumentNullException("recipients");
			}
			if (recipients.Count < 1)
			{
				if (msg.m_isSent == false)
					Recycle(msg);
				throw new NetException("recipients must contain at least one item");
			}
			if (method == NetDeliveryMethod.Unreliable || method == NetDeliveryMethod.ReliableUnordered)
				NetException.Assert(sequenceChannel == 0, "Delivery method " + method + " cannot use sequence channels other than 0!");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			msg.m_isSent = true;

			int mtu = GetMTU(recipients);

			int len = msg.GetEncodedSize();
			if (len <= mtu)
			{
				Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
				foreach (NetConnection conn in recipients)
				{
					if (conn == null)
					{
						Interlocked.Decrement(ref msg.m_recyclingCount);
						continue;
					}
					NetSendResult res = conn.EnqueueMessage(msg, method, sequenceChannel);
					if (res == NetSendResult.Dropped)
						Interlocked.Decrement(ref msg.m_recyclingCount);
				}
			}
			else
			{
				// message must be fragmented!
				SendFragmentedMessage(msg, recipients, method, sequenceChannel);
			}

			return;
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, string host, int port)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (host == null)
				throw new ArgumentNullException("host");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");

			var adr = NetUtility.Resolve(host);
			if (adr == null)
				throw new NetException("Failed to resolve " + host);

			var mtu = m_configuration.MTUForAddress(adr);
			if (msg.LengthBytes > mtu)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (whichever is appropriate, currently " + mtu + ")");

			msg.m_isSent = true;
			msg.m_messageType = NetMessageType.Unconnected;

			Interlocked.Increment(ref msg.m_recyclingCount);
			m_unsentUnconnectedMessages.Enqueue((new NetEndPoint(adr, port), msg));
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, NetEndPoint recipient)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipient == null)
				throw new ArgumentNullException("recipient");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");

			var mtu = m_configuration.MTUForEndPoint(recipient);
			if (msg.LengthBytes > mtu)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + mtu + ")");

			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;

			Interlocked.Increment(ref msg.m_recyclingCount);
			m_unsentUnconnectedMessages.Enqueue((recipient, msg));
		}

		/// <summary>
		/// Send a message to an unconnected host
		/// </summary>
		public void SendUnconnectedMessage(NetOutgoingMessage msg, IList<NetEndPoint> recipients)
		{
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (recipients == null)
				throw new ArgumentNullException("recipients");
			if (recipients.Count < 1)
				throw new NetException("recipients must contain at least one item");
			if (msg.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");

			// TODO: Avoid this extra allocating loop in the case that this isn't a dual-stack socket.
			var minimumMTU = int.MaxValue;
			foreach (var ep in recipients)
			{
				minimumMTU = Math.Min(minimumMTU, m_configuration.MTUForEndPoint(ep));
			}

			if (msg.LengthBytes > minimumMTU)
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + minimumMTU + ")");

			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;

			Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
			foreach (NetEndPoint ep in recipients)
				m_unsentUnconnectedMessages.Enqueue((ep, msg));
		}

		/// <summary>
		/// Send a message to this exact same netpeer (loopback)
		/// </summary>
		public void SendUnconnectedToSelf(NetOutgoingMessage om)
		{
			if (om == null)
				throw new ArgumentNullException("msg");
			if (om.m_isSent)
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");

			var selfEndPoint = m_socket?.LocalEndPoint;
			if (selfEndPoint == null)
				throw new InvalidOperationException("Local socket is not bound");

			om.m_messageType = NetMessageType.Unconnected;
			om.m_isSent = true;

			if (m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData) == false)
			{
				Interlocked.Decrement(ref om.m_recyclingCount);
				return; // dropping unconnected message since it's not enabled for receiving
			}

			// convert outgoing to incoming
			NetIncomingMessage im = CreateIncomingMessage(NetIncomingMessageType.UnconnectedData, om.LengthBytes);
			im.Write(om);
			im.m_isFragment = false;
			im.m_receiveTime = NetTime.Now;
			im.m_senderConnection = null;
			im.m_senderEndPoint = (NetEndPoint?)selfEndPoint;
			NetException.Assert(im.m_bitLength == om.LengthBits);

			// recycle outgoing message
			Recycle(om);

			ReleaseMessage(im);
		}
	}
}
