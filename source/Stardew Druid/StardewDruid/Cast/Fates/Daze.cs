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
using StardewDruid.Cast.Mists;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace StardewDruid.Cast.Fates
{
    public class Daze : EventHandle
    {

        public StardewValley.Monsters.Monster victim;

        public int speed;

        public int threshold;

        public bool focus;

        public int slot;

        public bool complete;

        public float damage;

        public Daze(Vector2 target,  StardewValley.Monsters.Monster Monster, int Slot, float Damage)
            : base(target)
        {

            victim = Monster;

            slot = Slot;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

            damage = Damage * 3 / 2;

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

            victim.moveTowardPlayerThreshold.Set(threshold);

            victim.focusedOnFarmers = focus;

        }

        public override void EventTrigger()
        {

            string register = "daze" + slot.ToString();

            Mod.instance.RegisterEvent(this, register);

            int click = 5 + slot;

            Mod.instance.clickRegister[click] = register;

            speed = victim.speed;

            threshold = victim.moveTowardPlayerThreshold.Value;

            focus = victim.focusedOnFarmers;

            victim.speed = speed / 2;

            victim.moveTowardPlayerThreshold.Set(0);

            victim.focusedOnFarmers = false;

            if (!Mod.instance.rite.castTask.ContainsKey("masterDaze"))
            {

                Mod.instance.UpdateTask("lessonDaze", 1);

            }

            TargetIcon();

        }

        public override bool EventActive()
        {

            if (expireEarly)
            {

                return false;

            }

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

            return false;

        }

        public override bool EventPerformAction(SButton Button, string Type)
        {

            if (Type != "Action")
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

                int direction = (targetPlayer.facingDirection.Value + 2) % 4;

                List<Vector2> strikeVectors = ModUtility.GetTilesWithinRadius(targetLocation, victim.Tile, 2, true, direction);

                if (victim.Position.Y > targetPlayer.Position.Y)
                {

                    if (targetPlayer.facingDirection.Value == 1)
                    {
                        strikeVectors.Reverse();

                    }

                }
                else
                {
                    if (targetPlayer.facingDirection.Value == 3)
                    {
                        strikeVectors.Reverse();

                    }

                }

                if (victim.Position.X > targetPlayer.Position.X)
                {

                    if (targetPlayer.facingDirection.Value == 0)
                    {
                        strikeVectors.Reverse();

                    }

                }
                else
                {

                    if (targetPlayer.facingDirection.Value == 2)
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

                    Rectangle boundingBox = targetPlayer.GetBoundingBox();

                    int diffX = (int)(boundingBox.X - targetPlayer.Position.X);

                    int diffY = (int)(boundingBox.Y - targetPlayer.Position.Y);

                    boundingBox.X = (int)(strikeVector.X * 64) + diffX;

                    boundingBox.Y = (int)(strikeVector.Y * 64) + diffY;

                    if (targetLocation.isCollidingPosition(boundingBox, Game1.viewport, isFarmer: false, 0, glider: false, targetPlayer, pathfinding: false))
                    {

                        continue;

                    }

                    float checkDistance = Vector2.Distance(Mod.instance.rite.caster.Position, strikeVector * 64);

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

            if (!Mod.instance.eventRegister.ContainsKey("shield"))
            {

                ShieldEvent shieldEvent = new(Game1.player.Position);

                shieldEvent.EventTrigger();

            }

            ModUtility.AnimateQuickWarp(targetLocation, targetPlayer.Position);

            if (targetPlayer.CurrentTool is MeleeWeapon meleeWeapon)
            {

                List<int> diff = new() { 0, 0 };

                if (meleeWeapon.knockback.Value > 0)
                {

                    diff = ModUtility.CalculatePush(targetLocation, victim, targetPlayer.Position, 64);

                }

                if (meleeWeapon.critMultiplier.Value > 0)
                {

                    damage += damage / 100 * meleeWeapon.critMultiplier.Value;

                }

                ModUtility.HitMonster(targetLocation, targetPlayer, victim, (int)damage, true, diffX: diff[0], diffY: diff[1]);

            }

            expireEarly = true;

            return true;

        }

        public override void EventRemove()
        {

            CleanUp();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                return;

            }

            if (animations.Count <= 0)
            {

                TargetIcon();

            }

            if (!targetLocation.temporarySprites.Contains(animations.First()))
            {

                animations.Clear();

                TargetIcon();

            }

            Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            animations[0].Position = center.ToVector2() + new Vector2(-32, 32);

            animations[0].reset();

        }

        public void TargetIcon()
        {

            Rectangle box = victim.GetBoundingBox();

            Point center = box.Center;

            TemporaryAnimatedSprite warpTarget = new(0, 100f, 1, 1, center.ToVector2() - new Vector2(16, 0) + new Vector2(0, 32), false, false)
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

            animations.Add(warpTarget);

        }

    }

}
