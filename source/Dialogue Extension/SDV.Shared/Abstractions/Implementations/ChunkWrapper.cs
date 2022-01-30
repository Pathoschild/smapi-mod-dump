/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ChunkWrapper : IChunkWrapper
  {
    public ChunkWrapper(Chunk item) => GetBaseType = item;
    public Chunk GetBaseType { get; }
    public int debrisType { get; set; }
    public float scale { get; set; }
    public float alpha { get; set; }
    public NetFields NetFields { get; }
    public float getSpeed() => 0;
  }
}
