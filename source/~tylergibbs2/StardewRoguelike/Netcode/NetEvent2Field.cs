/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Netcode;
using System.IO;

namespace StardewRoguelike.Netcode
{
    public class NetEvent2Field<T1, T1Field, T2, T2Field> : AbstractNetEvent2<T1, T2> where T1Field : NetField<T1, T1Field>, new() where T2Field : NetField<T2, T2Field>, new()
    {
		protected override T1 readEventArg1(BinaryReader reader, NetVersion version)
		{
			T1Field val = new();
			val.ReadFull(reader, version);
			return val.Value;
		}

		protected override T2 readEventArg2(BinaryReader reader, NetVersion version)
		{
			T2Field val = new();
			val.ReadFull(reader, version);
			return val.Value;
		}

		protected override void writeEventArg1(BinaryWriter writer, T1 eventArg)
		{
			T1Field val = new();
			val.Value = eventArg;
			val.WriteFull(writer);
		}

		protected override void writeEventArg2(BinaryWriter writer, T2 eventArg)
		{
			T2Field val = new();
			val.Value = eventArg;
			val.WriteFull(writer);
		}
	}
}
