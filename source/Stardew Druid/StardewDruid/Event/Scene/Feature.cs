/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Map;
using StardewValley;

namespace StardewDruid.Event.Scene
{
    public class Feature : TriggerHandle
    {
        public Texture2D hatsTexture;
        public Rectangle hatSourceRect;

        public Feature(GameLocation location, Quest quest)
          : base(location, quest)
        {
            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
            hatSourceRect = Game1.getSourceRectForStandardTileSheet(hatsTexture, 345, 20, 20);
        }

        public override void EventInterval()
        {
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 1000f, 1, 1, targetVector * 64f, false, false)
            {
                sourceRect = hatSourceRect,
                sourceRectStartingPos = new Vector2(hatSourceRect.X, hatSourceRect.Y),
                texture = hatsTexture,
                scale = 4.5f
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite);
            animationList.Add(temporaryAnimatedSprite);
            base.EventInterval();
        }
    }
}