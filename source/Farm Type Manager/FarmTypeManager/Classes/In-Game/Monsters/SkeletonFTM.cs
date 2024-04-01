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
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using System;
using System.Xml.Serialization;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's Skeleton class, adjusted for use by this mod.</summary>
        public class SkeletonFTM : Skeleton
        {
            /*** Net fields ***/

            /// <summary>True if this monster's normal ranged attack behavior should be enabled.</summary>
            [XmlElement("FTM_rangedAttacks")]
            public readonly NetBool rangedAttacks = new NetBool(value: true);

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
                this.rangedAttacks.Value = rangedAttacks;
            }

            /// <summary>Initialize any net fields added by this subclass.</summary>
            protected override void initNetFields()
            {
                base.initNetFields();
                NetFields.AddField(rangedAttacks, "rangedAttacks");
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
                if (Player == null) { return; } //if this was somehow called while no farmers exist, do nothing

                if (!throwing.Value)
                {
                    Monster_behaviorAtGameTick(time); //replace inaccessible "base" call with a local copy
                }
                if (!spottedPlayer && !base.wildernessFarmMonster && StardewValley.Utility.doesPointHaveLineOfSightInMine(base.currentLocation, base.Tile, base.Player.Tile, moveTowardPlayerThreshold.Value)) //replace 8 with the threshold value (sight range)
                {
                    controller = new PathFindController(this, base.currentLocation, base.Player.TilePoint, -1, null, 200);
                    spottedPlayer = true;
                    if (controller == null || controller.pathToEndPoint == null || controller.pathToEndPoint.Count == 0)
                    {
                        Halt();
                        facePlayer(base.Player);
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
                    if (Sprite.Animate(time, 20, 4, 150f))
                    {
                        this.throwing.Value = false;
                        this.Sprite.currentFrame = 0;
                        this.faceDirection(2);
                        Vector2 v = StardewValley.Utility.getVelocityTowardPlayer(new Point((int)base.Position.X, (int)base.Position.Y), 8f, base.Player);
                        if (isMage.Value)
                        {
                            if (Game1.random.NextBool())
                            {
                                base.currentLocation.projectiles.Add(new DebuffingProjectile("19", 14, 4, 4, (float)Math.PI / 16f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), base.currentLocation, this));
                            }
                            else
                            {
                                base.currentLocation.projectiles.Add(new BasicProjectile(base.DamageToFarmer * 2, 9, 0, 4, 0f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), "flameSpellHit", "flameSpell", null, explode: false, damagesMonsters: false, base.currentLocation, this));
                            }
                        }
                        else
                        {
                            base.currentLocation.projectiles.Add(new BasicProjectile(base.DamageToFarmer, 4, 0, 0, (float)Math.PI / 16f, v.X, v.Y, new Vector2(base.Position.X, base.Position.Y), "skeletonHit", "skeletonStep", null, explode: false, damagesMonsters: false, base.currentLocation, this));
                        }
                    }
                }
                //check the ranged attacks setting before attempting to start throwing, and replace 8 with the threshold value (sight range)
                else if (rangedAttacks.Value && spottedPlayer && controller == null && Game1.random.NextDouble() < (isMage.Value ? 0.008 : 0.002) && !base.wildernessFarmMonster && StardewValley.Utility.doesPointHaveLineOfSightInMine(base.currentLocation, base.Tile, base.Player.Tile, moveTowardPlayerThreshold.Value))
                {
                    throwing.Value = true;
                    Halt();
                    Sprite.currentFrame = 20;
                    shake(750);
                }
                else if (this.withinPlayerThreshold(2))
                {
                    controller = null;
                }
                else if (spottedPlayer && controller == null && controllerAttemptTimer <= 0)
                {
                    controller = new PathFindController(this, base.currentLocation, base.Player.TilePoint, -1, null, 200);
                    controllerAttemptTimer = (base.wildernessFarmMonster ? 2000 : 1000);
                    if (controller == null || controller.pathToEndPoint == null || controller.pathToEndPoint.Count == 0)
                    {
                        Halt();
                    }
                }
                else if (base.wildernessFarmMonster)
                {
                    spottedPlayer = true;
                    base.IsWalkingTowardPlayer = true;
                }
                controllerAttemptTimer -= time.ElapsedGameTime.Milliseconds;
            }

            /// <summary>Except where commented, this is a copy of "Monster.behaviorAtGameTick", used to implement this monster's "base.behaviorAtGameTick" call.</summary>
            private void Monster_behaviorAtGameTick(GameTime time)
            {
                if (timeBeforeAIMovementAgain > 0f)
                {
                    timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;
                }
                if (Player?.isRafting != true || !withinPlayerThreshold(4)) //check for null on Player due to reported errors (not necessarily FTM-specific)
                {
                    return;
                }
                IsWalkingTowardPlayer = false;
                Point monsterPixel = StandingPixel;
                Point playerPixel = Player.StandingPixel;
                if (Math.Abs(playerPixel.Y - monsterPixel.Y) > 192)
                {
                    if (playerPixel.X - monsterPixel.X > 0)
                    {
                        SetMovingLeft(b: true);
                    }
                    else
                    {
                        SetMovingRight(b: true);
                    }
                }
                else if (playerPixel.Y - monsterPixel.Y > 0)
                {
                    SetMovingUp(b: true);
                }
                else
                {
                    SetMovingDown(b: true);
                }
                MovePosition(time, Game1.viewport, currentLocation);
            }
        }
    }
}
