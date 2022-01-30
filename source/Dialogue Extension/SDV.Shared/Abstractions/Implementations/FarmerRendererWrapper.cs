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
  public class FarmerRendererWrapper : IFarmerRendererWrapper
  {
    public FarmerRendererWrapper(FarmerRenderer item) => GetBaseType = item;

    public FarmerRenderer GetBaseType { get; }
    public NetFields NetFields { get; }

    public void unload()
    {
    }

    public void recolorEyes(Color lightestColor)
    {
    }

    public void recolorShoes(int which)
    {
    }

    public int recolorSkin(int which, bool force = false) => 0;

    public void changeShirt(int whichShirt)
    {
    }

    public void changePants(int whichPants)
    {
    }

    public void MarkSpriteDirty()
    {
    }

    public void ApplySleeveColor(string texture_name, Color[] pixels, IFarmerWrapper who)
    {
    }

    public void draw(SpriteBatch b, IFarmerWrapper who, int whichFrame, Vector2 position, float layerDepth = 1,
      bool flip = false)
    {
    }

    public void draw(SpriteBatch b, IFarmerSpriteWrapper farmerSprite, Rectangle sourceRect, Vector2 position,
      Vector2 origin,
      float layerDepth, Color overrideColor, float rotation, IFarmerWrapper who)
    {
    }

    public void draw(SpriteBatch b, IAnimationFrameWrapper animationFrame, int currentFrame, Rectangle sourceRect,
      Vector2 position, Vector2 origin, float layerDepth, Color overrideColor, float rotation, float scale,
      IFarmerWrapper who)
    {
    }

    public void draw(SpriteBatch b, IAnimationFrameWrapper animationFrame, int currentFrame, Rectangle sourceRect,
      Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation,
      float scale, IFarmerWrapper who)
    {
    }

    public int ClampShirt(int shirt_value) => 0;

    public int ClampPants(int pants_value) => 0;

    public void drawMiniPortrat(SpriteBatch b, Vector2 position, float layerDepth, float scale, int facingDirection,
      IFarmerWrapper who)
    {
    }

    public void drawHairAndAccesories(SpriteBatch b, int facingDirection, IFarmerWrapper who, Vector2 position,
      Vector2 origin,
      float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
    {
    }
  }
}