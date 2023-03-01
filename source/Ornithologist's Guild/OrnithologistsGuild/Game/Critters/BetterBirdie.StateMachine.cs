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
        Relocating,
        Bathing
    }

    public enum BetterBirdieTrigger
    {
        Peck,
        Walk,
        Stop,
        Hop,
        FlyAway,
        Sleep,
        Relocate,
        Bathe
    }

    public partial class BetterBirdie : StardewValley.BellsAndWhistles.Critter
    {
        private Func<BetterBirdieTrigger> NextAction;

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
            StateMachine = Fsm<BetterBirdieState, BetterBirdieTrigger>.Builder(BetterBirdieState.Stopping)
                .State(BetterBirdieState.Stopping)
                    .TransitionTo(BetterBirdieState.Stopped).On(BetterBirdieTrigger.Stop)
                    .Update(a =>
                    {
                        // Wait for current animation to stop
                        if (sprite.CurrentAnimation == null) StateMachine.Trigger(BetterBirdieTrigger.Stop);
                    })
                .State(BetterBirdieState.Stopped)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .TransitionTo(BetterBirdieState.Sleeping).On(BetterBirdieTrigger.Sleep)
                    .TransitionTo(BetterBirdieState.Pecking).On(BetterBirdieTrigger.Peck)
                    .TransitionTo(BetterBirdieState.Walking).On(BetterBirdieTrigger.Walk)
                    .TransitionTo(BetterBirdieState.Hopping).On(BetterBirdieTrigger.Hop)
                    .TransitionTo(BetterBirdieState.Bathing).On(BetterBirdieTrigger.Bathe)
                    .OnEnter(e =>
                    {
                        // Reset animation to base frame
                        sprite.currentFrame = baseFrame;

                        var contextualBehavior = GetContextualBehavior();
                        var nextBehavior = Utilities.WeightedRandom(contextualBehavior, b => b.Weight);

                        if (nextBehavior.Immediate)
                        {
                            // Execute next action immediately
                            StateMachine.Trigger(nextBehavior.Action());
                        } else
                        {
                            // Wait a little while before executing next action (see `Update()`)
                            NextAction = nextBehavior.Action;
                        }
                    })
                    .Update(a =>
                    {
                        if (Game1.random.NextDouble() < 0.0075) StateMachine.Trigger(NextAction());
                    })
                .State(BetterBirdieState.Hopping)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        // Maybe flip
                        if (Game1.random.NextDouble() < 0.5) Flip();

                        gravityAffectedDY = -2f;
                    })
                    .Update(a =>
                    {
                        if (yJumpOffset >= 0) {
                            // Done hopping
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                            return;
                        }

                        var canHopLeft = !Environment.isCollidingPosition(getBoundingBox(-2, 0), Game1.viewport, false, 0, false, null, false, false, true);
                        var canHopRight = !Environment.isCollidingPosition(getBoundingBox(2, 0), Game1.viewport, false, 0, false, null, false, false, true);

                        // Hop left or right
                        if (!flip)
                        {
                            if (canHopLeft) position.X -= 2f;
                            else Flip();
                        }
                        else
                        {
                            if (canHopRight) position.X += 2f;
                            else Flip();
                        }
                    })
                .State(BetterBirdieState.Walking)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        // Start walk animation
                        sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame> {
                                    new FarmerSprite.AnimationFrame ((short)baseFrame, 100),
                                    new FarmerSprite.AnimationFrame ((short)(baseFrame + 1), 100)
                                });
                        sprite.loop = true;

                        if (position.X >= startingPosition.X) flip = false;
                        else flip = true;

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
                        if (WalkTimer <= 0)
                        {
                            StateMachine.Trigger(BetterBirdieTrigger.Stop);
                            return;
                        }

                        // TODO is this needed?
                        var canWalkLeft = !(
                            Environment.isCollidingPosition(getBoundingBox(-1, 0), Game1.viewport, false, 0, false, null, false, false, true) ||
                            Environment.isCollidingPosition(getBoundingBox(-2, 0), Game1.viewport, false, 0, false, null, false, false, true));
                        var canWalkRight = !(
                            Environment.isCollidingPosition(getBoundingBox(1, 0), Game1.viewport, false, 0, false, null, false, false, true) ||
                            Environment.isCollidingPosition(getBoundingBox(2, 0), Game1.viewport, false, 0, false, null, false, false, true));

                        if (!canWalkLeft && !canWalkRight)
                        {
                            StateMachine.Trigger(BetterBirdieTrigger.Relocate);
                            return;
                        }

                        // Move left and right
                        if (!flip)
                        {
                            if (canWalkLeft) position.X -= 1f;
                            else Flip();
                        }
                        else
                        {
                            if (canWalkRight) position.X += 1f;
                            else Flip();
                        }

                        // Move up and down randomly
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
                    })
                .State(BetterBirdieState.Pecking)
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
                                    if (Utility.isOnScreen(position, Game1.tileSize)) Game1.playSound("shiny4");
                                }));
                            }

                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 3), 100));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 2), 70, secondaryArm: false, flip));
                            list.Add(new FarmerSprite.AnimationFrame((short)(baseFrame + 1), 70, secondaryArm: false, flip));
                            list.Add(new FarmerSprite.AnimationFrame((short)baseFrame, 500, secondaryArm: false, flip, (Farmer who) =>
                            {
                                // 50% chance to peck again
                                if (Game1.random.NextDouble() < 0.5) StateMachine.Trigger(BetterBirdieTrigger.Stop);
                            }));

                            sprite.loop = false;
                            sprite.setCurrentAnimation(list);
                        }
                    })
                .State(BetterBirdieState.Sleeping)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        sprite.currentFrame = baseFrame + 5;
                    })
                    .Update(a =>
                    {
                        if (isEmoting) return;

                        if (IsRoosting && Game1.random.NextDouble() < 0.00025) StateMachine.Trigger(BetterBirdieTrigger.Relocate);
                        else if (!IsRoosting && Game1.random.NextDouble() < 0.001) StateMachine.Trigger(BetterBirdieTrigger.Stop);
                        else if (Game1.random.NextDouble() < 0.0025) doEmote(Character.sleepEmote);
                    })
                .State(BetterBirdieState.FlyingAway)
                    .OnEnter(e =>
                    {
                        // No longer perched
                        Perch = null;
                        stopEmote();

                        Character character = Utility.isThereAFarmerOrCharacterWithinDistance(position / Game1.tileSize, BirdieDef.GetContextualCautiousness(), Environment);

                        // Fly away from nearest character
                        if (character != null)
                        {
                            if (character.Position.X > position.X) flip = false;
                            else flip = true;
                        }

                        if (Game1.random.NextDouble() < 0.85) PlayCall();

                        sprite.setCurrentAnimation(GetFlyingAnimation());
                        sprite.loop = true;
                    })
                    .Update(a =>
                    {
                        if (!flip) position.X -= BirdieDef.FlySpeed - FlySpeedOffset; // Left
                        else position.X += BirdieDef.FlySpeed + FlySpeedOffset; // Right

                        yOffset -= 2f + FlySpeedOffset;
                    })
                .State(BetterBirdieState.Relocating)
                    .TransitionTo(BetterBirdieState.Stopping).On(BetterBirdieTrigger.Stop)
                    .OnEnter(e =>
                    {
                        Tuple<Vector3, Perch> relocateTo;
                        if (!ModEntry.debug_BirdWhisperer.HasValue)
                        {
                            relocateTo = GetRandomRelocationTileOrPerch();
                        } else
                        {
                            relocateTo = new Tuple<Vector3, Perch>(new Vector3(ModEntry.debug_BirdWhisperer.Value.X, ModEntry.debug_BirdWhisperer.Value.Y, 0), null);
                            ModEntry.debug_BirdWhisperer = null;
                        }

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

                            if (position.X > RelocateTo.Item1.X) flip = false;
                            else flip = true;

                            if (Game1.random.NextDouble() < 0.8) PlayCall();

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
   
                                if (IsPerched && Perch.Type == PerchType.Tree)
                                {
                                    // Shake tree on landing
                                    ModEntry.Instance.Helper.Reflection.GetMethod(Perch.Tree, "shake").Invoke(Perch.Tree.currentTileLocation, false, Game1.player.currentLocation);
                                }

                                StateMachine.Trigger(BetterBirdieTrigger.Stop);
                            }
                        }
                    })
                .State(BetterBirdieState.Bathing)
                    .OnEnter(e =>
                    {
                        if (!BirdieDef.CanBathe) {
                            StateMachine.Trigger(BetterBirdieTrigger.Relocate);
                            return;
                        }

                        sprite.setCurrentAnimation(GetBathingAnimation());
                        sprite.loop = true;
                    })
                    .OnExit(e =>
                    {
                        sprite.loop = false;
                        sprite.CurrentAnimation = null;
                    })
                    .Update(a =>
                    {
                        if (Game1.random.NextDouble() < 0.0025) Flip();
                        else if (Game1.random.NextDouble() < 0.005) StateMachine.Trigger(BetterBirdieTrigger.Relocate);
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
