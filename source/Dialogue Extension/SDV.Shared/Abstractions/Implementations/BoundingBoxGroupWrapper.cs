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
using StardewValley.Util;

namespace SDV.Shared.Abstractions
{
  public class BoundingBoxGroupWrapper : IBoundingBoxGroupWrapper
  {
    public BoundingBoxGroup GetBaseType { get; }
    public bool Intersects(Rectangle rect) => false;

    public bool Contains(int x, int y) => false;

    public void Add(Rectangle rect)
    {
    }

    public void ClearNonIntersecting(Rectangle rect)
    {
    }

    public void Clear()
    {
    }

    public void Draw(SpriteBatch b)
    {
    }

    public bool IsEmpty() => false;
  }
}
