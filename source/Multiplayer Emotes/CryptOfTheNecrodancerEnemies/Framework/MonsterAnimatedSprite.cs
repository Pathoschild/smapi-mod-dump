/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies.Framework {

  public static class MonsterExtension {
    public static void CreateCustomSprite(this Monster monster) {
      monster.Sprite = MonsterAnimatedSprite.From(monster, monster.Sprite);
    }
  }

  public class MonsterAnimatedSprite : AnimatedSprite {

    public static MonsterAnimatedSprite From(Monster monster, AnimatedSprite animatedSprite) {
      return new(monster, animatedSprite);
    }

    public MonsterAnimatedSprite(Monster monster, AnimatedSprite sprite) : base() {
      Owner = monster;
      SpriteWidth = sprite.SpriteWidth;
      SpriteHeight = sprite.SpriteHeight;
      spriteTexture = sprite.spriteTexture;
      loadedTexture = sprite.loadedTexture;
      textureName.Set(sprite.textureName.Value);
      timer = sprite.timer;
      interval = sprite.interval;
      framesPerAnimation = sprite.framesPerAnimation;
      currentFrame = sprite.currentFrame;
      tempSpriteHeight = sprite.tempSpriteHeight;
      sourceRect = new Rectangle(sprite.sourceRect.X, sprite.sourceRect.Y, sprite.sourceRect.Width, sprite.sourceRect.Height);
      loop = sprite.loop;
      ignoreStopAnimation = sprite.ignoreStopAnimation;
      textureUsesFlippedRightForLeft = sprite.textureUsesFlippedRightForLeft;
      CurrentAnimation = sprite.CurrentAnimation;
      oldFrame = sprite.oldFrame;
      currentAnimationIndex = sprite.currentAnimationIndex;
      //contentManager = sprite.contentManager;
      UpdateSourceRect();
    }

    public Monster Owner { get; set; }

    public new void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth) {
      draw(b, screenPosition, layerDepth, 0, 0, Color.White);
    }

    public new void draw(SpriteBatch b, Vector2 screenPosition, float layerDepth, int xOffset, int yOffset, Color c, bool flip = false, float scale = 1f, float rotation = 0f, bool characterSourceRectOffset = false) {
      if (Texture == null) return;
      b.Draw(Texture, screenPosition, new Rectangle(sourceRect.X + xOffset, sourceRect.Y + yOffset, sourceRect.Width, sourceRect.Height), c, rotation, characterSourceRectOffset ? new Vector2(SpriteWidth / 2, SpriteHeight * 3f / 4f) : Vector2.Zero, scale, (flip || (CurrentAnimation != null && CurrentAnimation[currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }

    public new void drawShadow(SpriteBatch b, Vector2 screenPosition, float scale = 4f, float alpha = 1f) {
      b.Draw(Game1.shadowTexture, screenPosition + new Vector2(SpriteWidth / 2 * Game1.pixelZoom - scale, SpriteHeight * Game1.pixelZoom - scale), Game1.shadowTexture.Bounds, Color.White * alpha, 0f, Utility.PointToVector2(Game1.shadowTexture.Bounds.Center), scale, SpriteEffects.None, 1E-05f);
    }

  }
}
