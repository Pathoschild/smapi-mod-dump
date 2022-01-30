/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class AnimatedSpriteWrapper : IAnimatedSpriteWrapper
  {
    public delegate void endOfAnimationBehavior(IFarmerWrapper farmer);

    public AnimatedSpriteWrapper(AnimatedSprite animatedSprite) => GetBaseType = animatedSprite;
    public int CurrentFrame { get; set; }
    public int SpriteWidth { get; set; }
    public int SpriteHeight { get; set; }
    public Rectangle SourceRect { get; set; }
    public IEnumerable<IAnimationFrameWrapper> CurrentAnimation { get; set; }
    public NetFields NetFields { get; }
    public Texture2D Texture { get; }

    public void LoadTexture(string textureName)
    {
    }

    public int getHeight() => 0;

    public int getWidth() => 0;

    public void StopAnimation()
    {
    }

    public void standAndFaceDirection(int direction)
    {
    }

    public void faceDirectionStandard(int direction)
    {
    }

    public void faceDirection(int direction)
    {
    }

    public void AnimateRight(GameTime gameTime, int intervalOffset = 0, string soundForFootstep = "")
    {
    }

    public void AnimateUp(GameTime gameTime, int intervalOffset = 0, string soundForFootstep = "")
    {
    }

    public void AnimateDown(GameTime gameTime, int intervalOffset = 0, string soundForFootstep = "")
    {
    }

    public void AnimateLeft(GameTime gameTime, int intervalOffset = 0, string soundForFootstep = "")
    {
    }

    public bool Animate(GameTime gameTime, int startFrame, int numberOfFrames, float interval) => false;

    public bool AnimateBackwards(GameTime gameTime, int startFrame, int numberOfFrames, float interval) => false;

    public void setCurrentAnimation(IEnumerable<IAnimationFrameWrapper> animation)
    {
    }

    public bool animateOnce(GameTime time) => false;

    public void UpdateSourceRect()
    {
    }

    public void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth)
    {
    }

    public void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth, int xOffset, int yOffset, Color c,
      bool flip = false,
      float scale = 1, float rotation = 0, bool characterSourceRectOffset = false)
    {
    }

    public void drawShadow(SpriteBatch b, Vector2 screenPosition, float scale = 4, float alpha = 1)
    {
    }

    public void drawShadow(SpriteBatch b, Vector2 screenPosition, float scale = 4)
    {
    }

    public IAnimatedSpriteWrapper Clone() => null;

    public AnimatedSprite GetBaseType { get; }
  }
}