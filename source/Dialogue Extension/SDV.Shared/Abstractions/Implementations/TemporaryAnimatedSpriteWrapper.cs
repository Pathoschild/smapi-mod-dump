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
  public class TemporaryAnimatedSpriteWrapper : ITemporaryAnimatedSpriteWrapper
  {
    public TemporaryAnimatedSpriteWrapper(TemporaryAnimatedSprite item) => GetBaseType = item;
    public TemporaryAnimatedSprite GetBaseType { get; }
    public Vector2 Position { get; set; }
    public IGameLocationWrapper Parent { get; set; }
    public Texture2D Texture { get; }
    public ITemporaryAnimatedSpriteWrapper getClone() => null;

    public void Read(BinaryReader reader, IGameLocationWrapper location)
    {
    }

    public void Write(BinaryWriter writer, IGameLocationWrapper location)
    {
    }

    public void draw(SpriteBatch spriteBatch, bool localPosition = false, int xOffset = 0, int yOffset = 0, float extraAlpha = 1)
    {
    }

    public void bounce(int extraInfo)
    {
    }

    public void unload()
    {
    }

    public void reset()
    {
    }

    public void resetEnd()
    {
    }

    public bool update(GameTime time) => false;

    public bool clearOnAreaEntry() => false;
  }
}
