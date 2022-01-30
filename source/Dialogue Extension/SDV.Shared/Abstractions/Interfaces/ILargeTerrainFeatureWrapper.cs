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

namespace SDV.Shared.Abstractions
{
  public interface ILargeTerrainFeatureWrapper : ITerrainFeaturesWrapper
  {
    Rectangle getBoundingBox();
    void dayUpdate(IGameLocationWrapper l);
    bool tickUpdate(GameTime time, IGameLocationWrapper location);
    void draw(SpriteBatch b);
  }
}