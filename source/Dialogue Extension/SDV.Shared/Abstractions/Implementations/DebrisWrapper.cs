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
  public class DebrisWrapper : IDebrisWrapper
  {
    public DebrisWrapper(Debris item) => GetBaseType = item;
    public Debris GetBaseType { get; }
    public NetObjectShrinkList<IChunkWrapper> Chunks { get; }
    public int itemQuality { get; set; }
    public int chunkFinalYLevel { get; set; }
    public int chunkFinalYTarget { get; set; }
    public bool chunksMoveTowardPlayer { get; set; }
    public Texture2D spriteChunkSheet { get; }
    public IItemWrapper item { get; set; }
    public NetFields NetFields { get; }
    public bool isEssentialItem() => false;

    public bool collect(IFarmerWrapper farmer, IChunkWrapper chunk = null) => false;

    public Color getColorForDebris(int type) => default;

    public bool shouldControlThis(IGameLocationWrapper location) => false;

    public bool updateChunks(GameTime time, IGameLocationWrapper location) => false;
  }
}
