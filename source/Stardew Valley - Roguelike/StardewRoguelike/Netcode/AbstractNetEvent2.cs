/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Netcode;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewRoguelike.Netcode
{
    public abstract class AbstractNetEvent2<T1, T2> : AbstractNetSerializable
    {
		private class EventRecording
		{
			public T1 arg1;

			public T2 arg2;

			public uint timestamp;

			public EventRecording(T1 arg1, T2 arg2, uint timestamp)
			{
				this.arg1 = arg1;
				this.arg2 = arg2;
				this.timestamp = timestamp;
			}
		}

		public delegate void Event(T1 arg1, T2 arg2);

		public bool InterpolationWait = true;

		private List<EventRecording> outgoingEvents = new();

		private List<EventRecording> incomingEvents = new();

		public event Event onEvent;

		public AbstractNetEvent2()
		{
		}

		public bool HasPendingEvent(Predicate<T1> match)
		{
			return incomingEvents.Exists((EventRecording e) => match(e.arg1));
		}

		public void Clear()
		{
			outgoingEvents.Clear();
			incomingEvents.Clear();
		}

		public void Fire(T1 arg1, T2 arg2)
		{
			EventRecording recording = new(arg1, arg2, GetLocalTick());
			outgoingEvents.Add(recording);
			incomingEvents.Add(recording);
			MarkDirty();
			Poll();
		}

		public void Poll()
		{
			List<EventRecording> triggeredEvents = null;
			foreach (EventRecording e2 in this.incomingEvents)
			{
				if (Root is null || GetLocalTick() >= e2.timestamp)
				{
					if (triggeredEvents is null)
						triggeredEvents = new List<EventRecording>();

					triggeredEvents.Add(e2);
					continue;
				}
				break;
			}
			if (triggeredEvents is null || triggeredEvents.Count <= 0)
			{
				return;
			}
			incomingEvents.RemoveAll(new Predicate<EventRecording>(triggeredEvents.Contains));
			if (onEvent is null)
				return;

			foreach (EventRecording e in triggeredEvents)
				onEvent(e.arg1, e.arg2);
		}

		protected abstract T1 readEventArg1(BinaryReader reader, NetVersion version);

		protected abstract void writeEventArg1(BinaryWriter writer, T1 arg1);

		protected abstract T2 readEventArg2(BinaryReader reader, NetVersion version);

		protected abstract void writeEventArg2(BinaryWriter writer, T2 arg2);

		public override void Read(BinaryReader reader, NetVersion version)
		{
			uint count = reader.Read7BitEncoded();
			uint timestamp = GetLocalTick();
			if (InterpolationWait)
				timestamp += (uint)Root.Clock.InterpolationTicks;
			for (uint i = 0u; i < count; i++)
			{
				uint delay = reader.ReadUInt32();
				incomingEvents.Add(new(readEventArg1(reader, version), readEventArg2(reader, version), timestamp + delay));
			}
			ChangeVersion.Merge(version);
		}

		public override void ReadFull(BinaryReader reader, NetVersion version)
		{
			ChangeVersion.Merge(version);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write7BitEncoded((uint)outgoingEvents.Count);
			if (outgoingEvents.Count > 0)
			{
				uint baseTime = outgoingEvents[0].timestamp;
				foreach (EventRecording e in this.outgoingEvents)
				{
					writer.Write(e.timestamp - baseTime);
					writeEventArg1(writer, e.arg1);
					writeEventArg2(writer, e.arg2);
				}
			}
			outgoingEvents.Clear();
		}

		protected override void CleanImpl()
		{
			base.CleanImpl();
			outgoingEvents.Clear();
		}

		public override void WriteFull(BinaryWriter writer)
		{
		}
	}
}
