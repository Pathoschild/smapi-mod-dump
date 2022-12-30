/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StateMachine;

namespace OrnithologistsGuild.Game.Critters
{
    public enum BetterBirdieState
    {
        Stopping,
        Stopped,
        Hopping,
        Walking,
        Pecking,
        Sleeping,
        FlyingAway,
        Relocating
    }

    public enum BetterBirdieTrigger
    {
        Peck,
        Walk,
        Stop,
        Hop,
        FlyAway,
        Sleep,
        Relocate
    }

    public partial class BetterBirdie : StardewValley.BellsAndWhistles.Critter
    {
        // Timers
        private int WalkTimer;

        // Relocate
        private Vector3? RelocateFrom;
        private Tuple<Vector3, Perch> RelocateTo;
        private float? RelocateDistance;
        private int? RelocateDuration;
        private int? RelocateElapsed;

        private void InitializeStateMachine()
        {
            StateMachine = Fsm<BetterBirdieState, BetterBirdieTrigger>.Builder(BetterBirdieState.Stopped)
                .State(BetterBirdieState.Stopping) // Done!
                    .TransitionTo(BetterBirdieState.Stopped).On(BetterBirdieTrigger.Stop)
                    .Update(a =>
                    {
                        // Wait for current animation to stop
                        if (sprite.CurrentAnimation == null)
                        {
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                        }
                    })
                .State(BetterBirdieState.Stopped) // Done!
                    .TransitionTo(BetterBirdieState.Sleeping).On(BetterBirdieTrigger.Sleep)
                    .TransitionTo(BetterBirdieState.Pecking).On(BetterBirdieTrigger.Peck)
                    .TransitionTo(BetterBirdieState.Walking).On(BetterBirdieTrigger.Walk)
                    .TransitionTo(BetterBirdieState.Hopping).On(BetterBirdieTrigger.Hop)
                    .OnEnter(e =>
                    {
                        // Reset animation to base frame
                        sprite.currentFrame = baseFrame;
                    })
                    .Update(a =>
                    {
                        if (IsRoosting) {
                            StateMachine.Trigger(BetterBirdieTrigger.Sleep);
                            return;
                        }

                        if (Game1.random.NextDouble() < 0.008)
                        {
                            switch (Game1.random.Next(7))
                            {
                                case 0:
                                    StateMachine.Trigger(IsPerched ? BetterBirdieTrigger.Peck : BetterBirdieTrigger.Sleep);
                                    break;
                                case 1:
                                    StateMachine.Trigger(BetterBirdieTrigger.Peck);
                                    break;
                                case 2:
                                    StateMachine.Trigger(IsPerched ? BetterBirdieTrigger.Peck : BetterBirdieTrigger.Hop);
                                    break;
                                case 3:
                                    flip = !flip;
                                    StateMachine.Trigger(IsPerched ? BetterBirdieTrigger.Peck : BetterBirdieTrigger.Hop);
                                    break;
                                case 4:
                                case 5:
                                    StateMachine.Trigger(BetterBirdieTrigger.Walk);
                                    break;
                                case 6:
                                    var random = Game1.random.NextDouble();
                                    if (random < 0.025)
                                    {
                                        StateMachine.Trigger(BetterBirdieTrigger.FlyAway);
                                    }
                                    else if (random < 0.1)
                                    {
                                        StateMachine.Trigger(BetterBirdieTrigger.Relocate);

                                    }
                                    break;
                            }
                        }
                    })
                .State(BetterBirdieState.Hopping) // Done!
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        gravityAffectedDY = -2f;
                    })
                    .Update(a =>
                    {
                        if (!IsPerched && yJumpOffset < 0f)
                        {
                            // Hop left or right
                            if (!flip)
                            {
                                if (!Environment.isCollidingPosition(getBoundingBox(-2, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true))
                                {
                                    position.X -= 2f;
                                }
                                else
                                {
                                    // Can't hop left -- flip instead
                                    flip = !flip;
                                }
                            }
                            else
                            {
                                if (!Environment.isCollidingPosition(getBoundingBox(2, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true))
                                {
                                    position.X += 2f;
                                }
                                else
                                {
                                    flip = !flip;
                                }
                            }
                        }
                        else if (yJumpOffset >= 0)
                        {
                            // Done hopping
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                        }
                    })
                .State(BetterBirdieState.Walking) // Done!
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        if (!IsPerched)
                        {
                            // Ensure there's ample room to walk
                            var roomToWalkLeft = !Environment.isCollidingPosition(getBoundingBox(-4, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true);
                            var roomToWalkRight = !Environment.isCollidingPosition(getBoundingBox(4, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true);

                            if (!roomToWalkLeft && !roomToWalkRight)
                            {
                                StateMachine.Trigger(BetterBirdieTrigger.Relocate);
                                return;
                            }
                        }

                        // Start walk animation
                        sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame> {
                                    new FarmerSprite.AnimationFrame ((short)baseFrame, 100),
                                    new FarmerSprite.AnimationFrame ((short)(baseFrame + 1), 100)
                                });
                        sprite.loop = true;

                        if (position.X >= startingPosition.X)
                        {
                            flip = false;
                        }
                        else
                        {
                            flip = true;
                        }

                        WalkTimer = Game1.random.Next(5, 50) * 100;
                    })
                    .OnExit(e =>
                    {
                        // Stop walk animation
                        sprite.loop = false;
                        sprite.CurrentAnimation = null;
                    })
                    .Update(a =>
                    {
                        WalkTimer -= a.ElapsedTimeSpan.Milliseconds;

                        if (!IsPerched)
                        {
                            var canWalkLeft = !Environment.isCollidingPosition(getBoundingBox(-1, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true);
                            var canWalkRight = !Environment.isCollidingPosition(getBoundingBox(1, 0), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: false, projectile: false, ignoreCharacterRequirement: true);

                            if (!flip)
                            {
                                if (canWalkLeft) position.X -= 1f;
                                else flip = !flip;
                            }
                            else
                            {
                                if (canWalkRight) position.X += 1f;
                                else flip = !flip;
                            }

                            switch(Game1.random.Next(3))
                            {
                                case 0:
                                    break;
                                case 1:
                                    position.Y += 0.5f;
                                    break;
                                case 2:
                                    position.Y -= 0.5f;
                                    break;
                            }
                        }
                        else
                        {
                            // TODO custom feeder bounds
                            var canWalkLeft = position.X >= startingPosition.X - 1f;
                            var canWalkRight = position.X <= startingPosition.X + 3f;

                            if (!flip)
                            {
                                if (canWalkLeft) position.X -= 1f;
                                else
                                {
                                    flip = !flip;
                                    StateMachine.Trigger(BetterBirdieTrigger.Stop);
                                }
                            }
                            else
                            {
                                if (canWalkRight) position.X += 1f;
                                else
                                {
                                    flip = !flip;
                                    StateMachine.Trigger(BetterBirdieTrigger.Stop);
                                }
                            }
                        }

                        if (WalkTimer <= 0)
                        {
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                        }
                    })
                .State(BetterBirdieState.Pecking) // Done!
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .Update(a =>
                    {
                        if (sprite.CurrentAnimation == null)
                        {
                            List<FarmerSprite.AnimationFrame> list = new List<FarmerSprite.AnimationFrame>();
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 2), 480));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 3), 170, secondaryArm: false, flip));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 4), 170, secondaryArm: false, flip));

                            int num = Game1.random.Next(1, 5);
                            for (int i = 0; i < num; i++)
                            {
                                list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 3), 70));
                                list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 4), 100, secondaryArm: false, flip, (Farmer who) =>
                                {
                                    // Play pecking noise
                                    if (Utility.isOnScreen(position, Game1.tileSize))
                                    {
                                        Game1.playSound("shiny4");
                                    }
                                }));
                            }

                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 3), 100));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 2), 70, secondaryArm: false, flip));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 1), 70, secondaryArm: false, flip));
                            list.Add(new FarmerSprite.AnimationFrame((short)baseFrame, 500, secondaryArm: false, flip, (Farmer who) =>
                            {
                                // 50% chance to peck again
                                if (Game1.random.NextDouble() < 0.5)
                                {
                                    StateMachine.Trigger(BetterBirdieTrigger.Stop);
                                }
                            }));

                            sprite.loop = false;
                            sprite.setCurrentAnimation(list);
                        }
                    })
                .State(BetterBirdieState.Sleeping) // Done!
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        sprite.currentFrame = baseFrame + 5;
                    })
                    .Update(a =>
                    {
                        if (isEmoting) return;

                        if (Game1.random.NextDouble() < 0.002)
                        {
                            doEmote(Character.sleepEmote);
                        }
                        else if (!IsRoosting && Game1.random.NextDouble() < 0.003)
                        {
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                        }
                    })
                .State(BetterBirdieState.FlyingAway) // Done!
                    .OnEnter(e =>
                    {
                        // No longer perched
                        Perch = null;
                        stopEmote();

                        Character character = Utility.isThereAFarmerOrCharacterWithinDistance(position / Game1.tileSize, BirdieDef.GetContextualCautiousness(), Environment);

                        // Fly away from nearest character
                        if (character != null)
                        {
                            if (character.Position.X > position.X)
                            {
                                flip = false;
                            }
                            else
                            {
                                flip = true;
                            }
                        }

                        if (Game1.random.NextDouble() < 0.85)
                        {
                            Game1.playSound(BirdieDef.SoundID == null ? "SpringBirds" : BirdieDef.SoundID);
                        }

                        sprite.setCurrentAnimation(GetFlyingAnimation());
                        sprite.loop = true;
                    })
                    .Update(a =>
                    {
                        if (!flip)
                        {
                            position.X -= BirdieDef.FlySpeed - FlySpeedOffset;
                        }
                        else
                        {
                            position.X += BirdieDef.FlySpeed + FlySpeedOffset;
                        }
                        yOffset -= 2f + FlySpeedOffset;
                    })
                .State(BetterBirdieState.Relocating)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        var relocateTo = GetRandomRelocationTileOrPerch();

                        if (relocateTo != null)
                        {
                            stopEmote();

                            // Immediately update perch to prevent collisions
                            Perch = relocateTo.Item2;

                            RelocateFrom = Position3;
                            RelocateTo = relocateTo;

                            RelocateDistance = Vector2.Distance(position, Utilities.XY(relocateTo.Item1));

                            RelocateDuration = (int)(RelocateDistance.Value / ((BirdieDef.FlySpeed + FlySpeedOffset) / 15f));
                            RelocateElapsed = 0;

                            if (position.X > RelocateTo.Item1.X)
                            {
                                flip = false;
                            }
                            else
                            {
                                flip = true;
                            }

                            if (Game1.random.NextDouble() < 0.8)
                            {
                                Game1.playSound(BirdieDef.SoundID == null ? "SpringBirds" : BirdieDef.SoundID);
                            }

                            sprite.setCurrentAnimation(GetFlyingAnimation());
                            sprite.loop = true;
                        }
                        else
                        {
                            // No clear location -- fly away instead
                            StateMachine.Trigger(BetterBirdieTrigger.FlyAway);
                        }
                    })
                    .OnExit(e =>
                    {
                        if (RelocateTo != null) {
                            // Stop fly animation
                            sprite.loop = false;
                            sprite.CurrentAnimation = null;

                            // Clean up
                            RelocateFrom = null;
                            RelocateTo = null;

                            RelocateDistance = null;

                            RelocateDuration = null;
                            RelocateElapsed = null;
                        }
                    })
                    .Update(a =>
                    {
                        if (RelocateTo != null)
                        {
                            // Fly to tile
                            RelocateElapsed += a.ElapsedTimeSpan.Milliseconds;
                            if (RelocateElapsed <= RelocateDuration)
                            {
                                var factor = ((float)RelocateElapsed.Value / (float)RelocateDuration.Value);

                                var midPointZ = ((RelocateFrom.Value.Z + RelocateTo.Item1.Z) / 2) - (RelocateDistance.Value / 6f); // Midpoint of Z values + (distance / 6)

                                // Fly in an arc
                                // Note: yOffset is Z
                                if (factor < 0.5f)
                                {
                                    // Fly up to mid point
                                    var arcFactor = factor * 2f;
                                    yOffset = Utility.Lerp(RelocateFrom.Value.Z, midPointZ, Utilities.EaseOutSine(arcFactor));
                                }
                                else
                                {
                                    // Fly down from mid point
                                    var arcFactor = (factor - 0.5f) * 2f;
                                    yOffset = Utility.Lerp(midPointZ, RelocateTo.Item1.Z, Utilities.EaseOutSine(arcFactor));
                                }

                                position = Vector2.Lerp(Utilities.XY(RelocateFrom.Value), Utilities.XY(RelocateTo.Item1), Utilities.EaseOutSine(factor));
                            }
                            else
                            {
                                // Relocation complete
                                Position3 = RelocateTo.Item1;
                                startingPosition = position;
   
                                if (IsRoosting && Perch.Tree != null)
                                {
                                    // Shake tree on landing
                                    ModEntry.Instance.Helper.Reflection.GetMethod(Perch.Tree, "shake").Invoke(Perch.Tree.currentTileLocation, false, Game1.player.currentLocation);
                                }

                                StateMachine.Trigger(BetterBirdieTrigger.Stop);
                            }
                        }
                    })
                .GlobalTransitionTo(BetterBirdieState.FlyingAway).OnGlobal(BetterBirdieTrigger.FlyAway)
                .GlobalTransitionTo(BetterBirdieState.Relocating).OnGlobal(BetterBirdieTrigger.Relocate)
                .Build();

            StateMachine.AddStateChangeHandler((state, e) =>
            {
                ModEntry.Instance.Monitor.Log($"{BirdieDef.ID}: {e.From.ToString()} -> {e.To.ToString()}");
            });
        }
    }
}
