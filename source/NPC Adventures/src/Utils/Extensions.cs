using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Utils
{
    public static class Extensions
    {
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {

            first = list.Count > 0 ? list[0] : default(T); // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            rest = list.Skip(2).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            third = list.Count > 1 ? list[2] : default(T); // or throw
            rest = list.Skip(3).ToList();
        }

        public static bool EqualsMajorMinor(this ISemanticVersion baseVersion, ISemanticVersion version)
        {
            return baseVersion.MajorVersion == version.MajorVersion 
                && baseVersion.MinorVersion == version.MinorVersion;
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static bool DamageMonsterByCompanion(
            this GameLocation location,
            Rectangle areaOfEffect,
            int minDamage,
            int maxDamage,
            float knockBackModifier,
            int addedPrecision,
            float critChance,
            float critMultiplier,
            bool triggerMonsterInvincibleTimer,
            Character who,
            Farmer leader)
        {
            bool flag1 = false;

            for (int j = location.characters.Count - 1; j >= 0; j--)
            {
                if (
                    j < location.characters.Count && location.characters[j].GetBoundingBox().Intersects(areaOfEffect)
                    && location.characters[j].IsMonster 
                    && ((Monster)location.characters[j]).Health > 0 
                    && !location.characters[j].IsInvisible 
                    && !(location.characters[j] as Monster).isInvincible())
                {
                    flag1 = true;
                    Microsoft.Xna.Framework.Rectangle boundingBox = location.characters[j].GetBoundingBox();
                    Vector2 trajectory = Helper.GetAwayFromCharacterTrajectory(boundingBox, who);
                    if (knockBackModifier > 0.0)
                        trajectory *= knockBackModifier;
                    else
                        trajectory = new Vector2(location.characters[j].xVelocity, location.characters[j].yVelocity);
                    if ((location.characters[j] as Monster).Slipperiness == -1)
                        trajectory = Vector2.Zero;
                    bool flag3 = false;
                    int number;
                    if (maxDamage >= 0)
                    {
                        int num = Game1.random.Next(minDamage, maxDamage + 1);
                        if (who != null && Game1.random.NextDouble() < (double)critChance + (double)leader.LuckLevel * ((double)critChance / 40.0))
                        {
                            flag3 = true;
                            location.playSound("crit");
                        }
                        int damage = Math.Max(1, (flag3 ? (int)((double)num * (double)critMultiplier) : num) + (who != null ? leader.attack * 3 : 0));
                        number = ((Monster)location.characters[j]).takeDamage(damage, (int)trajectory.X, (int)trajectory.Y, false, (double)addedPrecision / 10.0, leader);

                        if (number == -1)
                        {
                            location.debris.Add(new Debris("Miss", 1, new Vector2(boundingBox.Center.X, (float)boundingBox.Center.Y), Color.LightGray, 1f, 0.0f));
                        }
                        else
                        {
                            location.debris.Filter(d =>
                            {
                                if (d.toHover != null && d.toHover.Equals((object)location.characters[j]) && !d.nonSpriteChunkColor.Equals(Color.Yellow))
                                    return (double)d.timeSinceDoneBouncing <= 900.0;
                                return true;
                            });
                            location.debris.Add(new Debris(number, new Vector2((float)(boundingBox.Center.X + 16), (float)boundingBox.Center.Y), flag3 ? Color.Yellow : new Color((int)byte.MaxValue, 130, 0), flag3 ? (float)(1.0 + (double)number / 300.0) : 1f, location.characters[j]));
                        }
                        if (triggerMonsterInvincibleTimer)
                            (location.characters[j] as Monster).setInvincibleCountdown(450 / 2);
                    }
                    else
                    {
                        number = -2;
                        location.characters[j].setTrajectory(trajectory);
                        if (((Monster)location.characters[j]).Slipperiness > 10)
                        {
                            location.characters[j].xVelocity /= 2f;
                            location.characters[j].yVelocity /= 2f;
                        }
                    }
                    if (((Monster)location.characters[j]).Health <= 0)
                    {
                        if (!location.IsFarm)
                            leader.checkForQuestComplete((NPC)null, 1, 1, (Item)null, location.characters[j].Name, 4, -1);
                        Monster character = location.characters[j] as Monster;
                        location.monsterDrop(character, boundingBox.Center.X, boundingBox.Center.Y, leader);
                        location.characters.Remove(character);
                        ++Game1.stats.MonstersKilled;
                    }
                    else if (number > 0)
                    {
                        ((Monster)location.characters[j]).shedChunks(Game1.random.Next(1, 3));
                        if (flag3)
                        {
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, location.characters[j].getStandingPosition() - new Vector2(32f, 32f), false, Game1.random.NextDouble() < 0.5)
                            {
                                scale = 0.75f,
                                alpha = flag3 ? 0.75f : 0.5f
                            });
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, location.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) + 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                            {
                                scale = 0.5f,
                                delayBeforeAnimationStart = 50,
                                alpha = flag3 ? 0.75f : 0.5f
                            });
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, location.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) - 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                            {
                                scale = 0.5f,
                                delayBeforeAnimationStart = 100,
                                alpha = flag3 ? 0.75f : 0.5f
                            });
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, location.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) + 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                            {
                                scale = 0.5f,
                                delayBeforeAnimationStart = 150,
                                alpha = flag3 ? 0.75f : 0.5f
                            });
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(362, (float)Game1.random.Next(15, 50), 6, 1, location.characters[j].getStandingPosition() - new Vector2((float)(32 + Game1.random.Next(-21, 21) - 32), (float)(32 + Game1.random.Next(-21, 21))), false, Game1.random.NextDouble() < 0.5)
                            {
                                scale = 0.5f,
                                delayBeforeAnimationStart = 200,
                                alpha = flag3 ? 0.75f : 0.5f
                            });
                        }
                    }
                }
            }

            return flag1;
        }
    }
}
