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
  public interface IFarmerRendererWrapper : IWrappedType<FarmerRenderer>
  {
    NetFields NetFields { get; }
    void unload();
    void recolorEyes(Color lightestColor);
    void recolorShoes(int which);
    int recolorSkin(int which, bool force = false);
    void changeShirt(int whichShirt);
    void changePants(int whichPants);
    void MarkSpriteDirty();
    void ApplySleeveColor(string texture_name, Color[] pixels, IFarmerWrapper who);

    void draw(
      SpriteBatch b,
      IFarmerWrapper who,
      int whichFrame,
      Vector2 position,
      float layerDepth = 1f,
      bool flip = false);

    void draw(
      SpriteBatch b,
      IFarmerSpriteWrapper farmerSprite,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      Color overrideColor,
      float rotation,
      IFarmerWrapper who);

    void draw(
      SpriteBatch b,
      IAnimationFrameWrapper animationFrame,
      int currentFrame,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      Color overrideColor,
      float rotation,
      float scale,
      IFarmerWrapper who);

    void draw(
      SpriteBatch b,
      IAnimationFrameWrapper animationFrame,
      int currentFrame,
      Rectangle sourceRect,
      Vector2 position,
      Vector2 origin,
      float layerDepth,
      int facingDirection,
      Color overrideColor,
      float rotation,
      float scale,
      IFarmerWrapper who);

    int ClampShirt(int shirt_value);
    int ClampPants(int pants_value);

    void drawMiniPortrat(
      SpriteBatch b,
      Vector2 position,
      float layerDepth,
      float scale,
      int facingDirection,
      IFarmerWrapper who);

    void drawHairAndAccesories(
      SpriteBatch b,
      int facingDirection,
      IFarmerWrapper who,
      Vector2 position,
      Vector2 origin,
      float scale,
      int currentFrame,
      float rotation,
      Color overrideColor,
      float layerDepth);
  }
}