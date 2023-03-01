/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LookToTheSky
{
    class Firework : SkyObject
    {
        public Firework(int xPos, int yPos, Color color) : base(new TemporaryAnimatedSprite(), 0, false)
        {
            this.Sprite.texture = ModEntry.Assets.Firework;
            
            this.Sprite.position.X = xPos + 64;
            this.Sprite.position.Y = yPos + 64;
            this.Sprite.sourceRect = new Rectangle(0, 0, 64, 64);
            this.Sprite.sourceRectStartingPos = Vector2.Zero;
            this.Sprite.animationLength = 1;
            this.Sprite.acceleration = new Vector2(0.032f, 0.032f);
            this.Sprite.motion = new Vector2(-3.2f, -3.2f);
            this.Sprite.alphaFade = 0.00005f;
            this.Sprite.alphaFadeFade = -0.00016f;
            this.Sprite.rotation = (float)Game1.random.Next(360);
            this.Sprite.scale = 0.01f;
            this.Sprite.scaleChange = 0.1f;
            this.Sprite.scaleChangeChange = -0.001f;
            this.Sprite.color = color;
        }

        public override StardewValley.Object GetDropItem(int type = 0)
        {
            return null;
        }

        public override bool OnHit(StardewValley.Object ammo)
        {
            return false;
        }
    }
}
