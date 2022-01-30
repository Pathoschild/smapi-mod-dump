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
  public class WarpWrapper : IWarpWrapper
  {
    public WarpWrapper(Warp item) => GetBaseType = item;
    public Warp GetBaseType { get; }
    public int X { get; }
    public int Y { get; }
    public int TargetX { get; set; }
    public int TargetY { get; set; }
    public string TargetName { get; set; }
    public NetFields NetFields { get; }
  }
}
