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
  public interface IOutgoingMessageWrapper
  {
    OutgoingMessage GetBaseItem { get; }
    byte MessageType { get; }
    long FarmerID { get; }
    IReadOnlyCollection<object> Data { get; }
    IFarmerWrapper SourceFarmer { get; }
    void Write(BinaryWriter writer);
  }
}