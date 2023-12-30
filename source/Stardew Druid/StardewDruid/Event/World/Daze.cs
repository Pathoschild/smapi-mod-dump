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
using StardewDruid.Cast;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Event.World
{
    public class Daze : EventHandle
    {

        public StardewValley.Monsters.Monster victim;

        public int speed;

        public int threshold;

        public bool focus;

        public bool morph;

        public int slot;

        public bool complete;

        public int origin;

        public List<TemporaryAnimatedSprite> animation;

        //public List<TemporaryAnimatedSprite> animation;

        public Daze(Vector2 target, Rite rite, StardewValley.Monsters.Monster Monster, int Slot, int Origin)
            : base(target, rite)
        {

            victim = Monster;

            slot = Slot;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

            speed = Monster.speed;

            threshold = Monster.moveTowardPlayerThreshold.Value;

            focus = Monster.focusedOnFarmers;

            Monster.speed = speed / 2;

            Monster.moveTowardPlayerThreshold.Value = 0;

            Monster.focusedOnFarmers = false;

            if (!riteData.castTask.ContainsKey("masterDaze"))
            {

                Mod.instance.UpdateTask("lessonDaze", 1);

            }

            morph = (!Map.MonsterData.CustomMonsters().Contains(Monster.GetType()) && riteData.castTask.ContainsKey("masterDaze"));

            animation = new List<TemporaryAnimatedSprite>();

            origin = Origin;

            if (origin != 1)
            {

                return;

            }

            WarpAnimation();

        }

        public void CleanUp()
        {

            if (victim == null)
            {

                return;

            }

            if (!ModUtility.MonsterVitals(victim, targetLocation))
            {

                return;

            }

            victim.speed = speed;

            victim.moveTowardPlayerThreshold.Value = threshold;

            victim.focusedOnFarmers = focus;

        }

        public void MorphVictim()
        {

            if (morph && !complete)
            {

                Vector2 currentVector = victim.getTileLocation();

                int monsterIndex = riteData.randomIndex.Next(2) == 0 ? 51 : 52;

                StardewValley.Monsters.Monster spawnAttempt = Mod.instance.SpawnMonster(targetLocation, currentVector, new() { monsterIndex, });

                if (spawnAttempt != null)
                {

                    targetLocation.characters.Remove(victim);

                    victim = null;

                }

            }

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "daze" + slot.ToString());

        }

        public override bool EventActive()
        {

            if (!ModUtility.MonsterVitals(victim, targetLocation))
            {

                return false;

            }

            if (victim is StardewValley.Monsters.Mummy mummy)
            {

                if (mummy.reviveTimer.Value > 0)
                {

                    ModUtility.HitMonster(targetLocation, targetPlayer, mummy, 1, false);

                    victim = null;

                    return false;

                }

            }

            if (victim == null)
            {

                return false;

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime > Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {



                return true;

            }

            MorphVictim();

            return false;
        }

        public override bool EventPerformAction(SButton Button)
        {

            if (complete)
            {

                return false;

            }

            if (!EventActive())
            {

                return false;

            }

            List<Vector2> validVector = new();

            if (Vector2.Distance(targetPlayer.Position, victim.Position) < 447)
            {

                int direction = (targetPlayer.facingDirection + 2) % 4;

                List<Vector2> strikeVectors = ModUtility.GetTilesWithinRadius(targetLocation, victim.getTileLocation(), 2, true, direction);

                if (victim.Position.Y > targetPlayer.Position.Y)
                {

                    if (targetPlayer.facingDirection == 1)
                    {
                        strikeVectors.Reverse();

                    }

                }
                else
                {
                    if (targetPlayer.facingDirection == 3)
                    {
                        strikeVectors.Reverse();

                    }

                }

                if (victim.Position.X > targetPlayer.Position.X)
                {

                    if (targetPlayer.facingDirection == 0)
                    {
                        strikeVectors.Reverse();

                    }

                }
                else
                {

                    if (targetPlayer.facingDirection == 2)
                    {
                        strikeVectors.Reverse();

                    }

                }


                float closestDistance = 9999f;

                foreach (Vector2 strikeVector in strikeVectors)
                {

                    if (ModUtility.GroundCheck(targetLocation, strikeVector) != "ground")
                    {

                        continue;

                    }

                    Microsoft.Xna.Framework.Rectangle boundingBox = targetPlayer.GetBoundingBox();

                    int diffX = (int)(boundingBox.X - targetPlayer.Position.X);

                    int diffY = (int)(boundingBox.Y - targetPlayer.Position.Y);

                    boundingBox.X = (int)(strikeVector.X * 64) + diffX;

                    boundingBox.Y = (int)(strikeVector.Y * 64) + diffY;

                    if (targetLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, targetPlayer, pathfinding: false))
                    {
                        continue;

                    }

                    float checkDistance = Vector2.Distance(riteData.caster.Position, strikeVector * 64);

                    if (checkDistance < closestDistance)
                    {

                        closestDistance = checkDistance;

                        validVector.Clear();

                        validVector.Add(strikeVector);

                    }

                }

            }

            if (validVector.Count == 0)
            {

                return false;

            }

            targetPlayer.Position = validVector[0] * 64;

            targetPlayer.temporarilyInvincible = true;

            targetPlayer.temporaryInvincibilityTimer = 0;

            targetPlayer.currentTemporaryInvincibilityDuration = 1200;

            ModUtility.AnimateQuickWarp(targetLocation, targetPlayer.Position, "Void");

            if(targetPlayer.CurrentTool is MeleeWeapon meleeWeapon)
            {

                List<int> diff = new() { 0,0 };

                int damage = riteData.castDamage * 2;

                if (meleeWeapon.knockback.Value > 0)
                {

                    diff = ModUtility.CalculatePush(targetLocation, victim, targetPlayer.Position, 64);

                }

                if(meleeWeapon.critMultiplier.Value > 0)
                {

                    damage += (int)(riteData.castDamage / 100 * meleeWeapon.critMultiplier.Value);

                }

                ModUtility.HitMonster(targetLocation, targetPlayer, victim, damage, true, diffX: diff[0], diffY: diff[1]);

            }

            CleanUp();

            complete = true;

            return true;

        }

        public override void EventAbort()
        {

        }

        public override void EventRemove()
        {

            CleanUp();

        }

        public override void EventDecimal()
        {

            if (origin == 1)
            {

                return;

            }

            if (complete)
            {

                return;
            }

            if (!EventActive())
            {

                return;

            }

            if (animation.Count <= 0)
            {

                WarpAnimation();

            }

            if (!targetLocation.temporarySprites.Contains(animation.First()))
            {

                animation.Clear();

                WarpAnimation();

            }

            Microsoft.Xna.Framework.Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            animation[0].Position = center.ToVector2() + new Vector2(-32, 32);

            animation[0].reset();

        }

        public void WarpAnimation()
        {

            Microsoft.Xna.Framework.Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            TemporaryAnimatedSprite warpTarget = new(0, 100f, 1, 1, (center.ToVector2() - new Vector2(16, 0)) + new Vector2(0, 32), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png")),

                scale = 1.5f,

                layerDepth = 999f,

                alpha = 0.75f,

                color = new(0.4f, 0, 0.4f, 1f),

                rotation = (float)Math.PI,

            };

            targetLocation.temporarySprites.Add(warpTarget);

            animation.Add(warpTarget);

        }

    }

}
