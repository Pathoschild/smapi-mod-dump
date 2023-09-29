/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's Skeleton class, adjusted for use by this mod.</summary>
        public class SkeletonFTM : Skeleton
        {
            /*** New fields ***/

            /// <summary>True if this monster's normal ranged attack behavior should be enabled.</summary>
            public bool RangedAttacks { get; set; } = true;

            /*** Reflected fields ***/

            private IReflectedField<bool> _spottedPlayer = null;
            /// <summary>A reflection wrapper for a non-public field in this monster's base class.</summary>
            public bool spottedPlayer
            {
                get
                {
                    if (_spottedPlayer == null)
                        _spottedPlayer = Utility.Helper.Reflection.GetField<bool>(this, "spottedPlayer", true);
                    return _spottedPlayer.GetValue();
                }
                set
                {
                    if (_spottedPlayer == null)
                        _spottedPlayer = Utility.Helper.Reflection.GetField<bool>(this, "spottedPlayer", true);
                    _spottedPlayer.SetValue(value);
                }
            }

            private IReflectedField<NetBool> _throwing = null;
            /// <summary>A reflection wrapper for a non-public field in this monster's base class.</summary>
            public NetBool throwing
            {
                get
                {
                    if (_throwing == null)
                        _throwing = Utility.Helper.Reflection.GetField<NetBool>(this, "throwing", true);
                    return _throwing.GetValue();
                }
            }

            private IReflectedField<int> _controllerAttemptTimer = null;
            /// <summary>A reflection wrapper for a non-public field in this monster's base class.</summary>
            public int controllerAttemptTimer
            {
                get
                {
                    if (_controllerAttemptTimer == null)
                        _controllerAttemptTimer = Utility.Helper.Reflection.GetField<int>(this, "controllerAttemptTimer", true);
                    return _controllerAttemptTimer.GetValue();
                }
                set
                {
                    if (_controllerAttemptTimer == null)
                        _controllerAttemptTimer = Utility.Helper.Reflection.GetField<int>(this, "controllerAttemptTimer", true);
                    _controllerAttemptTimer.SetValue(value);
                }
            }

            /*** Methods ***/

            /// <summary>Creates an instance of Stardew's Skeleton class, but with adjustments made for this mod.</summary>
            public SkeletonFTM()
                : base()
            {

            }

            /// <summary>Creates an instance of Stardew's Skeleton class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            /// <param name="">True if this is a skeleton mage.</param>
            /// <param name="rangedAttacks">True if this monster's normal ranged attack behavior should be enabled.</param>
            public SkeletonFTM(Vector2 position, bool isMage = false, bool rangedAttacks = true)
                : base(position, isMage)
            {
                RangedAttacks = rangedAttacks;
            }

            /// <summary>A modified version of the base monster class's method.</summary>
            /// <remarks>
            /// Based on the original code of SDV v1.5.6. Modifed code sections are commented.
            /// Intended changes:
            /// * Fix a base game issue where Skeletons always use a sight range of 8
            /// * Implement a custom monster setting to disable ranged attacks (bone throwing)
            /// * Minimize a reported issue where behaviorAtGameTick is called while no Farmer instances exist (i.e. Monster.Player returns null)
            /// </remarks>
            public override void behaviorAtGameTick(GameTime time)
            {
                if (Player == null) //if this was somehow called while no farmers exist
                {
                    return; //do nothing
                }
                if (!throwing.Value)
                {
                    Monster_behaviorAtGameTick(time); //replace inaccessible "base" call with a local copy
                }
                if (!this.spottedPlayer && !base.wildernessFarmMonster && StardewValley.Utility.doesPointHaveLineOfSightInMine(base.currentLocation, base.getTileLocation(), base.Player.getTileLocation(), moveTowardPlayerThreshold.Value)) //replace 8 with the threshold value (a.k.a. sight range)
                {
                    base.controller = new PathFindController(this, base.currentLocation, new Point(base.Player.getStandingX() / 64, base.Player.getStandingY() / 64), -1, null, 200);
                    this.spottedPlayer = true;
                    if (base.controller == null || base.controller.pathToEndPoint == null || base.controller.pathToEndPoint.Count == 0)
                    {
                        this.Halt();
                        base.facePlayer(base.Player);
                    }
                    base.currentLocation.playSound("skeletonStep");
                    base.IsWalkingTowardPlayer = true;
                }
                else if (throwing.Value)
                {
                    if (base.invincibleCountdown > 0)
                    {
                        base.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                        if (base.invincibleCountdown <= 0)
                        {
                            base.stopGlowing();
                        }
                    }
                    this.Sprite.Animate(time, 20, 5, 150f);
                    if (this.Sprite.currentFrame == 24)
                    {
                        this.throwing.Value = false;
                        this.Sprite.currentFrame = 0;
                        this.faceDirection(2);
                        Vector2 v = StardewValley.Utility.getVelocityTowardPlayer(new Point((int)base.Position.X, (int)base.Position.Y), 8f, base.Player);
                        if (this.isMage.Value)
                        {
                            if (Game1.random.NextDouble() < 0.5)
                            {
                                base.currentLocation.projectiles.Add(new DebuffingProjectile(19, 14, 4, 4, (float)Math.PI / 16f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), base.currentLocation, this));
                            }
                            else
                            {
                                base.currentLocation.projectiles.Add(new BasicProjectile(base.DamageToFarmer * 2, 9, 0, 4, 0f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), "flameSpellHit", "flameSpell", explode: false, damagesMonsters: false, base.currentLocation, this));
                            }
                        }
                        else
                        {
                            base.currentLocation.projectiles.Add(new BasicProjectile(base.DamageToFarmer, 4, 0, 0, (float)Math.PI / 16f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), "skeletonHit", "skeletonStep", explode: false, damagesMonsters: false, base.currentLocation, this));
                        }
                    }
                } //check the ranged attacks setting before attempting to start throwing
                else if (RangedAttacks && this.spottedPlayer && base.controller == null && Game1.random.NextDouble() < (isMage.Value ? 0.008 : 0.002) && !base.wildernessFarmMonster && StardewValley.Utility.doesPointHaveLineOfSightInMine(base.currentLocation, base.getTileLocation(), base.Player.getTileLocation(), 8))
                {
                    this.throwing.Value = true;
                    this.Halt();
                    this.Sprite.currentFrame = 20;
                    base.shake(750);
                }
                else if (this.withinPlayerThreshold(2))
                {
                    base.controller = null;
                }
                else if (this.spottedPlayer && base.controller == null && this.controllerAttemptTimer <= 0)
                {
                    base.controller = new PathFindController(this, base.currentLocation, new Point(base.Player.getStandingX() / 64, base.Player.getStandingY() / 64), -1, null, 200);
                    this.controllerAttemptTimer = (base.wildernessFarmMonster ? 2000 : 1000);
                    if (base.controller == null || base.controller.pathToEndPoint == null || base.controller.pathToEndPoint.Count == 0)
                    {
                        this.Halt();
                    }
                }
                else if (base.wildernessFarmMonster)
                {
                    this.spottedPlayer = true;
                    base.IsWalkingTowardPlayer = true;
                }
                this.controllerAttemptTimer -= time.ElapsedGameTime.Milliseconds;
            }

            /// <summary>Except where commented, this is a copy of "Monster.behaviorAtGameTick", used to implement this monster's "base.behaviorAtGameTick" call.</summary>
            private void Monster_behaviorAtGameTick(GameTime time)
            {
                if (base.timeBeforeAIMovementAgain > 0f)
                {
                    base.timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;
                }
                if (this.Player?.isRafting != true || !this.withinPlayerThreshold(4)) //check for null on Player due to reported errors (not necessarily FTM-specific)
                {
                    return;
                }
                if (Math.Abs(this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y) > 192)
                {
                    if (this.Player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X > 0)
                    {
                        this.SetMovingLeft(b: true);
                    }
                    else
                    {
                        this.SetMovingRight(b: true);
                    }
                }
                else if (this.Player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y > 0)
                {
                    this.SetMovingUp(b: true);
                }
                else
                {
                    this.SetMovingDown(b: true);
                }
                this.MovePosition(time, Game1.viewport, base.currentLocation);
            }
        }
    }
}
