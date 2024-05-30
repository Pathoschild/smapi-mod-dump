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
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewValley;
using System;
using System.Collections.Generic;
using StardewDruid.Monster;
using StardewDruid.Data;
using StardewModdingAPI;
using StardewDruid.Location;
using System.Drawing;
using System.Data.Common;


namespace StardewDruid.Cast.Stars
{
    public class Summon : EventHandle
    {

        public int summonStrength;

        public int summonRounds;

        public int roundCountdown;

        public int roundTimer;

        public int roundIndex;

        public int summonLimit;

        public bool chargeActivated;

        public List<TemporaryAnimatedSprite> lineAnimations = new();

        public Summon()
        {

            mainEvent = true;

            expireIn = 180;

        }

        public override bool EventActive()
        {

            if (!eventLocked)
            {

                if (Mod.instance.Config.riteButtons.GetState() != SButtonState.Held)
                {

                    if (chargeActivated)
                    {

                        eventLocked = true;

                    }
                    else
                    {
                        return false;

                    }

                }

                if (Vector2.Distance(origin, Game1.player.Position) > 32)
                {

                    if (chargeActivated)
                    {

                        eventLocked = true;

                    }
                    else
                    {
                        return false;

                    }

                }

            }

            return base.EventActive();

        }

        public override void EventDecimal()
        {

            if (eventLocked)
            {

                return;

            }

            if (!EventActive())
            {

                RemoveAnimations();

                return;

            }

            decimalCounter++;

            if (decimalCounter == 5)
            {

                TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.hell, 1f, new() { interval = 1000, });

                skyAnimation.scaleChange = 0.002f;

                skyAnimation.motion = new(-0.064f, -0.064f);

                skyAnimation.timeBasedMotion = true;

                animations.Add(skyAnimation);

            }

            if (decimalCounter < 15)
            {

                return;

            }

            if (decimalCounter == 15)
            {

                TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.hell, 3f, new() { interval = 2000, });

                animations.Add(skyAnimation);

                location.playSound("thunder");

                chargeActivated = true;

            }

            if (decimalCounter % 10 == 5)
            {

                summonStrength++;

                List<TemporaryAnimatedSprite> effects = LocationData.SummoningEffects(location, summonStrength);

                lineAnimations.AddRange(effects);

            }

            if (decimalCounter == 45)
            {

                eventLocked = true;

            }

        }


        public void SummonConfig()
        {

            Vector2 initial = ModUtility.PositionToTile(origin);

            summonRounds = 4;

            monsterHandle = new(initial, Mod.instance.rite.castLocation);

            monsterHandle.spawnCombat = (int)(Mod.instance.CombatDifficulty() * (1 + 0.2 * summonStrength));

            roundIndex = 0;

            SummonSetup();

        }

        public virtual void SummonSetup()
        {

            switch (location.Name)
            {

                case LocationData.druid_grove_name:
                case LocationData.druid_atoll_name:

                    switch (roundIndex)
                    {

                        default: // start, bats, 0


                            monsterHandle.spawnSchedule = new()
                            {

                                [1] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [3] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [5] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [7] = new() { new(MonsterHandle.bosses.blobfiend, Boss.temperment.aggressive, Boss.difficulty.hard) },
                                [9] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [11] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [13] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [15] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [17] = new() { new(MonsterHandle.bosses.blobfiend) },
                                [19] = new() { new(MonsterHandle.bosses.blobfiend) },

                            };

                            summonLimit = 3;

                            SetTrack("tribal");

                            break;

                        case 1: // shadows

                            monsterHandle.spawnSchedule = new()
                            {

                                [1] = new() { new(MonsterHandle.bosses.batwing) },
                                [3] = new() { new(MonsterHandle.bosses.batwing) },
                                [5] = new() { new(MonsterHandle.bosses.batwing) },
                                [7] = new() { new(MonsterHandle.bosses.batwing, Boss.temperment.aggressive, Boss.difficulty.hard) },
                                [9] = new() { new(MonsterHandle.bosses.batwing) },
                                [11] = new() { new(MonsterHandle.bosses.batwing) },
                                [13] = new() { new(MonsterHandle.bosses.batwing) },
                                [15] = new() { new(MonsterHandle.bosses.batwing) },
                                [17] = new() { new(MonsterHandle.bosses.batwing) },
                                [19] = new() { new(MonsterHandle.bosses.batwing) },

                            };

                            summonLimit = 3;

                            break;

                        case 2: // slimes

                            monsterHandle.spawnSchedule = new()
                            {

                                [1] = new() { new(MonsterHandle.bosses.rogue) },
                                [1] = new() { new(MonsterHandle.bosses.goblin) },
                                [7] = new() { new(MonsterHandle.bosses.rogue) },
                                [7] = new() { new(MonsterHandle.bosses.goblin) },
                                [13] = new() { new(MonsterHandle.bosses.rogue) },
                                [13] = new() { new(MonsterHandle.bosses.goblin) },

                            };

                            summonLimit = 2;

                            SetTrack("tribal");

                            break;

                        case 3: // pirates

                            monsterHandle.spawnSchedule = new()
                            {

                                [1] = new() { new(MonsterHandle.bosses.dragon) },
                                [4] = new() { new(MonsterHandle.bosses.dragon) },
                                [7] = new() { new(MonsterHandle.bosses.dragon) },
                                [10] = new() { new(MonsterHandle.bosses.dragon) },
                                [13] = new() { new(MonsterHandle.bosses.dragon) },
                                [16] = new() { new(MonsterHandle.bosses.dragon) },
                                [19] = new() { new(MonsterHandle.bosses.dragon) },

                            };

                            summonLimit = 2;

                            break;

                    }

                    break;

            }

            monsterHandle.spawnCounter = 0;

            roundCountdown = 5;

            roundTimer = 20 + summonStrength * 2;

            expireTime += 25;

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                DealRewards();

                RemoveMonsters();

                eventLinger = 2;

                return true;

            }

            return base.EventExpire();

        }

        public void DealRewards()
        {

            Vector2 tile = ModUtility.PositionToTile(LocationData.SummoningVectors(location));

            int tileX = (int)tile.X;

            int tileY = (int)tile.Y;

            switch (roundIndex + summonStrength)
            {

                default:

                    CastVoice(0, "unacceptable", 2000);

                    break;

                case 3:

                    Game1.createObjectDebris("334", tileX, tileY);

                    CastVoice(0, "sufficient", 2000);

                    break;

                case 4:

                    Game1.createObjectDebris("335", tileX, tileY);

                    CastVoice(0, "good", 2000);

                    break;

                case 5:

                    Game1.createObjectDebris("336", tileX, tileY);

                    CastVoice(0, "great", 2000);

                    break;

                case 6:

                    Game1.createObjectDebris("337", tileX, tileY);

                    CastVoice(0, "superb", 2000);

                    break;

                case 7:

                    Game1.createObjectDebris("446", tileX, tileY, itemQuality: 4);

                    CastVoice(0, "brilliant", 2000);

                    break;

                case 8:

                    Game1.createObjectDebris("74", tileX, tileY);

                    CastVoice(0, "legendary", 2000);

                    break;


            }

        }

        public override void EventAbort()
        {
            Mod.instance.CastMessage("The portal through the veil has collapsed", 3, true);

            if (!Mod.instance.CasterBusy())
            {

                DealRewards();

            }

        }

        public override void EventInterval()
        {

            if (!eventLocked)
            {

                return;

            }

            // -------------------------------------
            // start rounds

            if (activeCounter == 0)
            {

                Vector2 voiceVector = LocationData.SummoningVoices(location);

                AddActor(0, voiceVector);

                SummonConfig();

                Mod.instance.CastMessage("Summoning initiated at difficulty level " + summonStrength);

            }

            // --------------------------------------

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            int factor = Math.Abs(10 - activeCounter % 20);

            Microsoft.Xna.Framework.Color lineColour = Microsoft.Xna.Framework.Color.White;

            switch (location.Name)
            {

                case LocationData.druid_grove_name:

                    lineColour = new(135 + factor * 12, 255, 135 + factor * 12);

                    break;
                case LocationData.druid_atoll_name:

                    lineColour = new(135 + factor * 12, 135 + factor * 12, 255);

                    break;

            }

            foreach (TemporaryAnimatedSprite lineAnimation in lineAnimations)
            {

                lineAnimation.color = lineColour;

                lineAnimation.reset();

            }

            // -------------------------------------

            if (roundCountdown >= 1)
            {
                if (roundCountdown == 5)
                {

                    CastVoice(0, "round " + (roundIndex + 1).ToString(), 2000);

                }

                if (roundCountdown <= 3)
                {

                    CastVoice(0, roundCountdown.ToString(), 1000);

                }

                roundCountdown--;

                return;


            }

            // -------------------------------------

            if (roundTimer >= 0)
            {

                roundTimer--;

            }

            if (roundTimer == 10)
            {

                CastVoice(0, "halfway", 2000);

            }

            if (roundTimer == 5)
            {

                if (monsterHandle.monsterSpawns.Count > summonLimit)
                {

                    Mod.instance.CastMessage("At least " + (monsterHandle.monsterSpawns.Count - summonLimit).ToString() + " more summons must be defeated!", 0, true);

                }

            }

            if (roundTimer == 0)
            {

                monsterHandle.ShutDown();

                if (monsterHandle.monstersLeft > summonLimit)
                {
                    CastVoice(0, "enough", 2000);

                    Mod.instance.CastMessage(monsterHandle.monstersLeft.ToString() + " monsters remained, " + (roundIndex + 1).ToString() + " rounds cleared", 0, true);

                    expireEarly = true;

                    return;

                }

                roundIndex++;

                if (roundIndex == summonRounds)
                {

                    Mod.instance.CastMessage("The summoning concludes, with " + roundIndex.ToString() + " rounds cleared", 0, true);

                    expireEarly = true;

                    return;

                }

                SummonSetup();

                return;

            }

            if (roundTimer > 6)
            {

                monsterHandle.SpawnInterval();

            }
            else if (monsterHandle.monsterSpawns.Count == 0)
            {

                roundTimer = 1;

            }

        }

    }

}
