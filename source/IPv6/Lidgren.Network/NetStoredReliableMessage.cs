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
	internal struct NetStoredReliableMessage
	{
		public int NumSent;
		public double LastSent;
		public NetOutgoingMessage? Message;
		public int SequenceNumber;

		public void Reset()
		{
			NumSent = 0;
			LastSent = 0.0;
			Message = null;
		}
	}
}