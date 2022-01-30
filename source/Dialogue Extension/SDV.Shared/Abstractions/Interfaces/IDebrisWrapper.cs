/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IDebrisWrapper : IWrappedType<Debris>
  {
    NetObjectShrinkList<IChunkWrapper> Chunks { get; }
    int itemQuality { get; set; }
    int chunkFinalYLevel { get; set; }
    int chunkFinalYTarget { get; set; }
    bool chunksMoveTowardPlayer { get; set; }
    Texture2D spriteChunkSheet { get; }
    IItemWrapper item { get; set; }
    NetFields NetFields { get; }
    bool isEssentialItem();
    bool collect(IFarmerWrapper farmer, IChunkWrapper chunk = null);
    Color getColorForDebris(int type);
    bool shouldControlThis(IGameLocationWrapper location);
    bool updateChunks(GameTime time, IGameLocationWrapper location);
  }
}