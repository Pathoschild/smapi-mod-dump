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
using Netcode;
using StardewDruid.Event;
using StardewDruid.Map;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.Objects.BedFurniture;

namespace StardewDruid.Monster.Boss
{
    public class Firebird : Boss.Dragon
    {

        public float height;

        public int[] idleAnimation = new int[10]
        {
            1,1,1,1,1,1,1,1,1,1,
        };

        public int[] scratchAnimation = new int[19]
        {
            0, 1, 2, 3, 2, 3, 2, 3, 2, 3,
            2, 3, 2, 3, 2, 3, 2, 3, 2
        };

        public int[] flyAnimation = new int[11]
        {
            4, 5, 6, 7, 7, 6, 6, 5, 5, 4,
            4
        };

        public int[] currentAnimation;

        public float frameTimer;

        public int currentTickIndex;

        public int currentFrameIndex;

        public float alpha = 1f;

        public Color birdColor;

        public int birdItem;

        public Firebird()
        {

        }

        public Firebird(Vector2 vector, int combatModifier, string BirdName)
            : base(vector, combatModifier, BirdName, "Shadow Brute")
        {

        }

        public override void HardMode()
        {

            Health *= 3;

            Health /= 2;

            MaxHealth = Health;

        }


        public override void LoadOut()
        {
            
            BaseWalk();
           
            BaseFlight();
            
            BaseSpecial();

            Health /= 2;

            MaxHealth = Health;

            DamageToFarmer /= 2;

            ouchList = new()
            {
                "tweep tweep",
                "CAWW"

            };

            dialogueList = new()
            {
                "caw",
                "hahaha",
            };

            birdItem = 64;

            overHead = new(16, -144);

            string birdType = realName.Value.Replace("Firebird", "");

            birdColor = birdType switch
            {
                "Emerald" => new Color(67, 255, 83),
                "Aquamarine" => new Color(74, 243, 255),
                "Ruby" => new Color(255, 38, 38),
                "Amethyst" => new Color(255, 67, 251),
                "Topaz" => new Color(255, 156, 33),
                _ => Color.White,
            };

            birdItem = birdType switch
            {
                "Emerald" => 60,
                "Aquamarine" => 62,
                "Ruby" => 64,
                "Amethyst" => 66,
                "Topaz" => 68,
                _ => 0,
            };

            barrageColor = birdType switch
            {
                "Emerald" => "Green",
                "Aquamarine" => "Blue",
                "Ruby" => "Red",
                "Amethyst" => "Purple",
                "Topaz" => "Red",
                _ => "Red",
            };

            loadedOut = true;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            b.Draw(characterTexture, Game1.GlobalToLocal(Game1.viewport, position.Value + new Vector2(0f, 0f - height)), new Rectangle(32, 0, 32, 32), Color.White * alpha, 0f, new Vector2(8f, 0), 4f, SpriteEffects.None, (position.Value.Y - 1f) / 10000f);

            b.Draw(characterTexture, Game1.GlobalToLocal(Game1.viewport, position.Value + new Vector2(0f, 0f - height)), new Rectangle(32, 32, 32, 32), birdColor * alpha, 0f, new Vector2(8f, 0), 4f, SpriteEffects.None, position.Value.Y / 10000f);

            b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, position.Value), Game1.shadowTexture.Bounds, Color.White * alpha, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, (position.Y - 2f) / 10000f);

        }


        public override Rectangle GetBoundingBox()
        {
            Vector2 vector = Position;

            return new Rectangle((int)vector.X, (int)vector.Y + 16, 64, 112);

        }


        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            behaviourActive = behaviour.halt;

            behaviourTimer = 300;

            //netSpecialActive.Set(true);

            //if (netDashActive)
           // {

            //    return 0;

            //}

            if (damage >= Health)
            {

                //currentAnimation = flyAnimation;

                //currentFrameIndex = 0;

                //Health = 1;

                //int newDamage = Health - 1;

                DialogueData.DisplayText(this, 1, 0, "FireBird");

                Game1.playSound("batFlap");

                Vector2 birdVector = getTileLocation();

                Game1.createObjectDebris(birdItem, (int)birdVector.X, (int)birdVector.Y);

            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void ChooseBehaviour()
        {

            if (xVelocity != 0.0 || yVelocity != 0.0)
            {

                xVelocity = 0.0f;

                yVelocity = 0.0f;

                Halt();

            }

            if (behaviourActive != behaviour.idle || cooldownActive)
            {

                return;

            }

            Random random = new Random();

            List<Farmer> source = TargetFarmers();

            if (source.Count == 0)
            {
                return;
            }

            Farmer farmer = source.First();

            SetDirection(farmer.Position);

            PerformSpecial();

        }

        public override void PerformSpecial()
        {

            behaviourActive = behaviour.special;

            behaviourTimer = new Random().Next(5) * 60 + 120;

            //currentLocation.playSound("furnace");

            List<Vector2> zero = BlastTarget();

            BarrageHandle fireball = new(currentLocation, zero[0], zero[1] - new Vector2(0, 2), 2, 1, barrageColor, DamageToFarmer);

            fireball.type = BarrageHandle.barrageType.fireball;

            fireball.counter = 30;

            fireball.LaunchFireball(2);

            barrages.Add(fireball);

        }


    }

}
