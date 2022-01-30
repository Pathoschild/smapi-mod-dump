/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface ITemporaryAnimatedSpriteWrapper : IWrappedType<TemporaryAnimatedSprite>
  {
    Vector2 Position { get; set; }
    IGameLocationWrapper Parent { get; set; }
    Texture2D Texture { get; }
    ITemporaryAnimatedSpriteWrapper getClone();
    void Read(BinaryReader reader, IGameLocationWrapper location);
    void Write(BinaryWriter writer, IGameLocationWrapper location);

    void draw(
      SpriteBatch spriteBatch,
      bool localPosition = false,
      int xOffset = 0,
      int yOffset = 0,
      float extraAlpha = 1f);

    void bounce(int extraInfo);
    void unload();
    void reset();
    void resetEnd();
    bool update(GameTime time);
    bool clearOnAreaEntry();
  }
}