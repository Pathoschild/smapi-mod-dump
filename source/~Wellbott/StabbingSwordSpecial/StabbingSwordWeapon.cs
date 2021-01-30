/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;

namespace StabbingSwordSpecial
{
    public class StabbingSwordWeapon : MeleeWeapon
    {
        public StabbingSwordWeapon()
        {
            base.NetFields.AddFields(type, minDamage, maxDamage, speed, addedPrecision, addedDefense, addedAreaOfEffect, knockback, critChance, critMultiplier, appearance);
            base.Category = -98;
        }

        /// <summary>
        /// Cosntructor to make it as much like the base sword as possible... but type "1"
        /// </summary>
        /// <param name="fromWeapon"></param>
        public StabbingSwordWeapon(MeleeWeapon fromWeapon, double damageMult)
            : this()
        {
            Category = fromWeapon.Category;
            BaseName = fromWeapon.BaseName;
            minDamage.Value = (int)Math.Floor((float)fromWeapon.minDamage.Value * damageMult);
            maxDamage.Value = (int)Math.Floor((float)fromWeapon.maxDamage.Value * damageMult);
            knockback.Value = fromWeapon.knockback.Value;
            speed.Value = fromWeapon.speed.Value;
            addedPrecision.Value = fromWeapon.addedPrecision.Value;
            addedDefense.Value = fromWeapon.addedDefense.Value;
            type.Set(1);
            addedAreaOfEffect.Value = fromWeapon.addedAreaOfEffect.Value;
            addedAreaOfEffect.Value = 0;
            critChance.Value = fromWeapon.critChance.Value - 0.005f;
            critMultiplier.Value = fromWeapon.critMultiplier.Value;
            Stack = 1;
            InitialParentTileIndex = fromWeapon.InitialParentTileIndex;
            CurrentParentTileIndex = fromWeapon.CurrentParentTileIndex;
            IndexOfMenuItemView = fromWeapon.IndexOfMenuItemView;
        }

        /// <summary>
        /// Stabby sword should have more reach than dagger. alos tweaked the other hitboxes a bit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="facingDirection"></param>
        /// <param name="tileLocation1"></param>
        /// <param name="tileLocation2"></param>
        /// <param name="wielderBoundingBox"></param>
        /// <param name="indexInCurrentAnimation"></param>
        /// <returns></returns>
        public override Rectangle getAreaOfEffect(int x, int y, int facingDirection, ref Vector2 tileLocation1, ref Vector2 tileLocation2, Rectangle wielderBoundingBox, int indexInCurrentAnimation)
        {
            Rectangle areaOfEffect = Rectangle.Empty;
            int horizontalYOffset = -32;
            int upHeightOffset = 42;
            int num = type;
            int width = 82; //74
            int height = 90; //48
            switch (facingDirection)
            {
                case 0:
                    areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Y - height - upHeightOffset, width / 2, height + upHeightOffset);
                    tileLocation1 = new Vector2(((Game1.random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Top / 64);
                    tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Top / 64);
                    areaOfEffect.Offset(20, -16);
                    areaOfEffect.Height += 16;
                    areaOfEffect.Width += 20;
                    break;
                case 1:
                    areaOfEffect = new Rectangle(wielderBoundingBox.Right, y - height / 2 + horizontalYOffset, height, width);
                    tileLocation1 = new Vector2(areaOfEffect.Center.X / 64, ((Game1.random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                    tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                    areaOfEffect.Offset(-4, 0);
                    areaOfEffect.Width += 16;
                    break;
                case 2:
                    areaOfEffect = new Rectangle(x - width / 2, wielderBoundingBox.Bottom, width, height);
                    tileLocation1 = new Vector2(((Game1.random.NextDouble() < 0.5) ? areaOfEffect.Left : areaOfEffect.Right) / 64, areaOfEffect.Center.Y / 64);
                    tileLocation2 = new Vector2(areaOfEffect.Center.X / 64, areaOfEffect.Center.Y / 64);
                    areaOfEffect.Offset(12, -8);
                    areaOfEffect.Width -= 12;
                    break;
                case 3:
                    areaOfEffect = new Rectangle(wielderBoundingBox.Left - height, y - height / 2 + horizontalYOffset, height, width);
                    tileLocation1 = new Vector2(areaOfEffect.Left / 64, ((Game1.random.NextDouble() < 0.5) ? areaOfEffect.Top : areaOfEffect.Bottom) / 64);
                    tileLocation2 = new Vector2(areaOfEffect.Left / 64, areaOfEffect.Center.Y / 64);
                    areaOfEffect.Offset(-12, 0);
                    areaOfEffect.Width += 16;
                    break;
            }
            areaOfEffect.Inflate(addedAreaOfEffect, addedAreaOfEffect);
            return areaOfEffect;
        }

        /// <summary>
        /// Repositioned icon in inventory while daggered
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="location"></param>
        /// <param name="scaleSize"></param>
        /// <param name="transparency"></param>
        /// <param name="layerDepth"></param>
        /// <param name="drawStackNumber"></param>
        /// <param name="color"></param>
        /// <param name="drawShadow"></param>
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            float coolDownLevel = 1f;
            float addedScale = 0f;
            spriteBatch.Draw(Tool.weaponsTexture, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, base.IndexOfMenuItemView, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * (scaleSize + addedScale), SpriteEffects.None, layerDepth);
            if (coolDownLevel > 0f && (Game1.activeClickableMenu == null || scaleSize != 1f))
            {
                spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X, (int)location.Y + (64 - (int)(coolDownLevel * 64f)), 64, (int)(coolDownLevel * 64f)), Color.Red * 0.66f);
            }
        }
    }
}
