/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using StardewValley.Network;

namespace SDV.Shared.Abstractions
{
  public class OutgoingMessageWrapper : IOutgoingMessageWrapper
  {
    public OutgoingMessageWrapper(OutgoingMessage item) => GetBaseItem = item;
    public OutgoingMessage GetBaseItem { get; }
    public byte MessageType { get; }
    public long FarmerID { get; }
    public IReadOnlyCollection<object> Data { get; }
    public IFarmerWrapper SourceFarmer { get; }
    public void Write(BinaryWriter writer)
    {
    }
  }
}
