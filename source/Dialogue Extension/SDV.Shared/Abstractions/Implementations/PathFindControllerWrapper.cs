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
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class PathFindControllerWrapper : IPathFindControllerWrapper
  {
    public delegate void endBehavior(ICharacterWrapper c, IGameLocationWrapper location);

    public delegate bool isAtEnd(
      PathNode currentNode,
      Point endPoint,
      IGameLocationWrapper location,
      ICharacterWrapper c);

    public PathFindControllerWrapper(PathFindController item) => GetBaseType = item;

    public PathFindController GetBaseType { get; }
    public bool isPlayerPresent() => false;

    public bool update(GameTime time) => false;

    public void handleWarps(Rectangle position)
    {
    }
  }
}