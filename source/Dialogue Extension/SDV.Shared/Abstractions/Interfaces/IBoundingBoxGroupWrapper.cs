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
  public interface IBoundingBoxGroupWrapper : IWrappedType<BoundingBoxGroup>
  {
    bool Intersects(Rectangle rect);
    bool Contains(int x, int y);
    void Add(Rectangle rect);
    void ClearNonIntersecting(Rectangle rect);
    void Clear();
    void Draw(SpriteBatch b);
    bool IsEmpty();
  }
}