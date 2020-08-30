using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Utils
{
    internal static partial class Helper
    {
        private static Vector2 meleeWeaponCenter = new Vector2(1f, 15f);

        /// <summary>
        /// Edited copy of vanilla method MeleeWeapon.drawDuringUse()
        /// </summary>
        /// <param name="frameOfFarmerAnimation"></param>
        /// <param name="facingDirection"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="playerPosition"></param>
        /// <param name="c"></param>
        /// <param name="sourceRect"></param>
        /// <param name="type"></param>
        /// <param name="isOnSpecial"></param>
        public static void DrawDuringUse(int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition, Character c, Rectangle sourceRect, int type, bool isOnSpecial)
        {
            if (type != 1)
            {
                if (isOnSpecial)
                {
                    if (type == 3)
                    {
                        switch (c.FacingDirection)
                        {
                            case 0:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 44f), new Rectangle?(sourceRect), Color.White, -1.76714587f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                                return;
                            case 1:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -0.5890486f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 1) / 10000f));
                                return;
                            case 2:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 52f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 2) / 10000f));
                                return;
                            case 3:
                                spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 56f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, -0.981747746f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 1) / 10000f));
                                return;
                            default:
                                return;
                        }
                    }
                    else if (type == 2)
                    {
                        if (facingDirection == 1)
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f - 12f, playerPosition.Y - 80f), new Rectangle?(sourceRect), Color.White, -1.17809725f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 64f - 48f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 128f - 16f, playerPosition.Y - 64f - 12f), new Rectangle?(sourceRect), Color.White, 1.17809725f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 3:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 72f, playerPosition.Y - 64f + 16f - 32f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 4:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f, playerPosition.Y - 64f + 16f - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 5:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 12f, playerPosition.Y - 64f + 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 6:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 16f, playerPosition.Y - 64f + 40f - 8f), new Rectangle?(sourceRect), Color.White, 0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 7:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 96f - 8f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, 0.981747746f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                default:
                                    return;
                            }
                        }
                        else if (facingDirection == 3)
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f + 8f, playerPosition.Y - 56f - 64f), new Rectangle?(sourceRect), Color.White, 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -1.96349549f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.74889374f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 3:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 32f - 4f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, -2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 4:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f - 24f, playerPosition.Y + 64f + 12f - 64f), new Rectangle?(sourceRect), Color.White, 4.31969f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 5:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 20f, playerPosition.Y + 64f + 40f - 64f), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 6:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y + 64f + 56f), new Rectangle?(sourceRect), Color.White, 3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                case 7:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 8f, playerPosition.Y + 64f + 64f), new Rectangle?(sourceRect), Color.White, 3.73064137f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                                    return;
                                default:
                                    return;
                            }
                        }
                        else
                        {
                            switch (frameOfFarmerAnimation)
                            {
                                case 0:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 24f, playerPosition.Y - 21f - 8f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 1:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f - 64f + 4f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 2:
                                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 20f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    break;
                                case 3:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    else
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 32f - 64f), new Rectangle?(sourceRect), Color.White, -0.7853982f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 4:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), new Rectangle?(sourceRect), Color.White, -3.926991f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 5:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f - 20f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 6:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 54f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                                case 7:
                                    if (facingDirection == 2)
                                    {
                                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 58f), new Rectangle?(sourceRect), Color.White, 2.3561945f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                                    }
                                    break;
                            }
                            return;
                        }
                    }
                }
                else if (facingDirection == 1)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, playerPosition.Y - 64f + 8f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 64f + 28f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 4f), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 28f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 1.96349549f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y + 64f + 12f), new Rectangle?(sourceRect), Color.White, 1.96349537f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 3)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 64f - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 48f, playerPosition.Y - 64f + 20f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 64f + 32f, playerPosition.Y + 16f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() - 1) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f), new Rectangle?(sourceRect), Color.White, -1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f), new Rectangle?(sourceRect), Color.White, -1.96349549f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.FlipHorizontally, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X - 44f, playerPosition.Y + 96f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.FlipVertically, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 0)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -1.17809725f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), new Rectangle?(sourceRect), Color.White, -0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -1.96349537f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32 - 8) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
                else if (facingDirection == 2)
                {
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.3926991f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 1:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 2:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 40f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 1.57079637f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 3:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 4:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f), new Rectangle?(sourceRect), Color.White, 3.14159274f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 5:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 6:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 12f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 3.53429174f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        case 7:
                            spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 64f), new Rectangle?(sourceRect), Color.White, -5.105088f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                            return;
                        default:
                            return;
                    }
                }
            }
            else
            {
                frameOfFarmerAnimation %= 2;
                if (facingDirection == 1)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, 0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                    return;
                }
                else if (facingDirection == 3)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f), new Rectangle?(sourceRect), Color.White, -2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 64) / 10000f));
                    return;
                }
                else if (facingDirection == 0)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 40f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 48f), new Rectangle?(sourceRect), Color.White, -0.7853982f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() - 32) / 10000f));
                    return;
                }
                else if (facingDirection == 2)
                {
                    if (frameOfFarmerAnimation == 0)
                    {
                        spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 12f), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                        return;
                    }
                    if (frameOfFarmerAnimation != 1)
                    {
                        return;
                    }
                    spriteBatch.Draw(Tool.weaponsTexture, new Vector2(playerPosition.X + 21f, playerPosition.Y), new Rectangle?(sourceRect), Color.White, 2.3561945f, meleeWeaponCenter, 4f, SpriteEffects.None, Math.Max(0f, (float)(c.getStandingY() + 32) / 10000f));
                }
            }
        }
    }
}
