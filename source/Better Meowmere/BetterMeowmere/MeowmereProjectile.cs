/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/BetterMeowmere
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BetterMeowmere
{
    public class MeowmereProjectile 
        : Projectile
    {
        private static IModHelper helper;
        public static Texture2D projectile;
        public static Texture2D tail;
        private static ModConfig config;

        public delegate void onCollisionBehavior(GameLocation location, int xPosition, int yPosition, Character who);
        public readonly NetInt damage = new NetInt();
        public NetInt debuff = new NetInt(-1);
        public static void Initialise(IModHelper modhelper, ModConfig config)
        {
            MeowmereProjectile.helper = modhelper;
            projectile = modhelper.ModContent.Load<Texture2D>(PathUtilities.NormalizePath("assets/meowmereprojectile.png"));
            tail = modhelper.ModContent.Load<Texture2D>(PathUtilities.NormalizePath("assets/meowmeretail.png"));
            MeowmereProjectile.config = config;
        }
        public MeowmereProjectile()
        { }

        public MeowmereProjectile(int damage, float xVelocity, float yVelocity, Vector2 startingPosition, int bouncesTillDestruct, int tailLength, string soundonbounce, GameLocation location = null, Character owner = null)
            : this()
        {
            this.damage.Value = damage;
            base.damagesMonsters.Value = true;
            this.damagesMonsters.Value = true;
            base.theOneWhoFiredMe.Set(location, owner);
            base.currentTileSheetIndex.Value = 0;
            base.tailLength.Value = tailLength;
            base.xVelocity.Value = xVelocity;
            base.yVelocity.Value = yVelocity;
            base.position.Value = startingPosition;
            base.bouncesLeft.Value = bouncesTillDestruct;
            base.bounceSound.Value = soundonbounce == "" ? null : soundonbounce;
        }
        public override void updatePosition(GameTime time)
        {
            base.position.X += xVelocity.Value;
            base.position.Y += yVelocity.Value;
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
            this.explosionAnimation(location);
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
            if(config.ProjectileSound != "None")
            {
                location.playSound("terraria_meowmere");
            }           
            this.explosionAnimation(location);
            base.piercesLeft.Value--;
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
            if (config.ProjectileSound != "None")
            {
                location.playSound("terraria_meowmere");
            }
            this.explosionAnimation(location);
            base.piercesLeft.Value--;
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (config.ProjectileSound != "None")
            {
                location.playSound("terraria_meowmere");
            }
            this.explosionAnimation(location);
            if (n is Monster)
            {
                location.damageMonster(n.GetBoundingBox(), this.damage.Value, this.damage.Value, isBomb: false, (base.theOneWhoFiredMe.Get(location) is Farmer) ? (base.theOneWhoFiredMe.Get(location) as Farmer) : Game1.player);
            }
            base.piercesLeft.Value--;
        }

        private void explosionAnimation(GameLocation location)
        {
            Multiplayer multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\Animations", new Rectangle(192, 320, 64, 64), 60, 5, 1, base.position.Value, flicker: false, flipped: false));      
            base.destroyMe = true;
            this.destroyMe = true;
        }

        public static void explodeOnImpact(GameLocation location, int x, int y, Character who)
        {
        }

        public static Vector2 TailOffset(Vector2 basevector, float xVelocity, float yVelocity)
        {
            if (yVelocity < 0f)
            {
                basevector.Y += 19f;
            }
            else if (yVelocity > 0f)
            {
                basevector.Y -= 19f;
            }

            if (xVelocity < 0f)
            {
                basevector.X += 19f;
                basevector.Y += 4f;
            }
            else if (xVelocity > 0f)
            {
                basevector.X -= 19f;
                basevector.Y += 4f;
            }
            return basevector;
        }

        public static float GetAngleForTexture(float xVelocity, float yVelocity)
        {
            var angle = 0f;           

            if (yVelocity > 0f)
            {
                angle = (float)Math.PI / 2.0f; // 90 degrees
            }
            else if (yVelocity < 0f)
            {
                angle = (float)Math.PI / -2.0f; // 270 degrees
            }
            return angle;
        }

        public override void draw(SpriteBatch b)
        {
            float projectile_scale = 2f * this.localScale;

            float tailAlpha = this.alpha.Value;

            var tailoffset = TailOffset(new Vector2(32f, 32f), base.xVelocity.Value, base.yVelocity.Value);

            var angle = GetAngleForTexture(base.xVelocity.Value, base.yVelocity.Value);

            float tail_scale = 1.67f * this.localScale;

            for (int i = base.tail.Count - 1; i >= 0; i--)
            {
                b.Draw(tail, Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp((i == base.tail.Count - 1) ? this.position.Value : base.tail.ElementAt(i + 1), base.tail.ElementAt(i), (float)base.tailCounter / 60f) + new Vector2(0f, 0f - this.height.Value) + tailoffset), new Rectangle(0, 0, 24, 24), this.color.Value * tailAlpha, angle, new Vector2(8f, 8f), tail_scale, GetSpriteDirection(base.xVelocity.Value, base.yVelocity.Value), (this.position.Y - (float)(base.tail.Count - i) + 96f) / 10000f);
                tailAlpha -= 1f / (float)base.tail.Count;
            }

            b.Draw(projectile, Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(0f, 0f - this.height.Value) + new Vector2(32f, 32f)), new Rectangle(0, 0, 24, 24), this.color.Value, angle, new Vector2(8f, 8f), projectile_scale, GetSpriteDirection(base.xVelocity.Value, base.yVelocity.Value), (this.position.Y + 96f) / 10000f);
            if (this.height.Value > 0f)
            {
                b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position.Value + new Vector2(32f, 32f)), Game1.shadowTexture.Bounds, Color.White * 0.75f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 2f, SpriteEffects.None, (this.position.Y - 1f) / 10000f);
            }
        }
        public static SpriteEffects GetSpriteDirection(float xVelocity, float yVelocity)
        {
            var spriteffect = SpriteEffects.None;
            if (yVelocity > 0f)
            {
                spriteffect = SpriteEffects.FlipVertically;
            }
            else if (xVelocity < 0f)
            {
                spriteffect = SpriteEffects.FlipHorizontally;
            }

            return spriteffect;
        }

        //public override Rectangle getBoundingBox()
        //{
        //    Vector2 pos = this.position.Value;
        //    int damageSize = (int)this.boundingBoxWidth.Value + (this.damagesMonsters.Value ? 8 : 0);
        //    float current_scale = this.localScale * 1.5f;
        //    damageSize = (int)((float)damageSize * current_scale);
        //    return new Rectangle((int)pos.X + 32 - damageSize / 2, (int)pos.Y + 32 - damageSize / 2, damageSize, damageSize);
        //}
    }
}

