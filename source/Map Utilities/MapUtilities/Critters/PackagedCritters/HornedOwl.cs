using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Critters.PackagedCritters
{
    public class HornedOwl : Critter
    {
        public int direction = 2;
        public Texture2D spriteSheet;

        public HornedOwl(Vector2 position, string direction = "")
        {
            switch (direction.ToLower().Substring(0,1))
            {
                case "n":
                case "u":
                case "0":
                    this.direction = 0;
                    break;
                case "e":
                case "r":
                case "1":
                    this.direction = 1;
                    break;
                case "s":
                case "d":
                case "2":
                    this.direction = 2;
                    break;
                case "w":
                case "l":
                case "3":
                    this.direction = 3;
                    break;
            }
            this.baseFrame = this.direction * 4;
            this.position = position * 64f;

            this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 32, 32);
            //Logger.log("Getting spriteTexture property...");
            StardewModdingAPI.IReflectedField<Texture2D> spriteTexture = Reflector.reflector.GetField<Texture2D>(sprite, "spriteTexture");
            spriteTexture.SetValue(Loader.loader.Load<Texture2D>("Content/Critters/HornedOwl.png", StardewModdingAPI.ContentSource.ModFolder));


            this.startingPosition = position;
            this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(this.direction * 4, 100),
                new FarmerSprite.AnimationFrame(this.direction * 4 + 1, 100),
                new FarmerSprite.AnimationFrame(this.direction * 4 + 2, 100),
                new FarmerSprite.AnimationFrame(this.direction * 4 + 3, 100)
            });
            Logger.log("Spawned horned owl");
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            Vector2 vector2 = new Vector2((float)Game1.viewport.X - Game1.previousViewportPosition.X, (float)Game1.viewport.Y - Game1.previousViewportPosition.Y) * 0.15f;
            if(direction == 0)
            {
                this.position.Y -= (float)(time.ElapsedGameTime.TotalMilliseconds * 0.200000002980232);
            }
            else if (direction == 1)
            {
                this.position.X += (float)(time.ElapsedGameTime.TotalMilliseconds * 0.200000002980232);
            }
            else if (direction == 2)
            {
                this.position.Y += (float)(time.ElapsedGameTime.TotalMilliseconds * 0.200000002980232);
            }
            else if (direction == 3)
            {
                this.position.X -= (float)(time.ElapsedGameTime.TotalMilliseconds * 0.200000002980232);
            }
            this.position = this.position - vector2;
            return base.update(time, environment);
        }

        public override void draw(SpriteBatch b)
        {
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            this.sprite.draw(
                b,
                Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)),
                (float)((double)this.position.Y / 10000.0 + (double)this.position.X / 100000.0),
                0,
                0,
                Color.White,
                this.flip,
                4f,
                0f,
                false
            );
            //(float)((direction * 90f - 180f) * (Math.PI /180f))
        }
    }
}
