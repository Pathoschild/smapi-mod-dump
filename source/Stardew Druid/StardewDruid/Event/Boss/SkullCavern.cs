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
using StardewDruid.Cast;
using StardewDruid.Event.Challenge;
using StardewDruid.Map;
using StardewDruid.Monster.Boss;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Boss
{
    public class SkullCavern : BossHandle
    {
        public Reaper bossMonster;
        public StardewDruid.Monster.Boss.Dragon secondMonster;
        public bool secondFight;
        public int secondCounter;
        public Vector2 bossTile;
        public bool adjustWarp;

        public SkullCavern(Vector2 target, Rite rite, Quest quest)
          : base(target, rite, quest)
        {

            targetVector = target;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120.0;

        }

        public override void EventTrigger()
        {
            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Weald", 1f, 1f);

            ModUtility.AnimateRockfalls(targetLocation, targetPlayer.getTileLocation());

            DelayedAction.functionAfterDelay(RockfallSounds, 575);

            DelayedAction.functionAfterDelay(RockfallSounds, 675);

            DelayedAction.functionAfterDelay(RockfallSounds, 775);

            Mod.instance.RegisterEvent(this, "active");
        }

        public void RockfallSounds()
        {
            targetLocation.playSoundPitched(new Random().Next(2) == 0 ? "boulderBreak" : "boulderCrack", 800, 0);
        }

        public override void RemoveMonsters()
        {
            
            if (bossMonster != null)
            {
                targetLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            if (secondMonster != null)
            {
                targetLocation.characters.Remove(secondMonster);

                secondMonster = null;

            }

            base.RemoveMonsters();

        }

        public override void EventRemove()
        {
            
            base.EventRemove();

            if (!(targetLocation is MineShaft))
            {
                return;
            }

            Location.LocationData.SkullCavernExit();

            EventQuery("LocationExit");

        }

        public override bool EventExpire()
        {
            if (eventLinger == -1)
            {
                
                RemoveMonsters();
                
                eventLinger = 3;
                
                return true;
            
            }
            
            if (eventLinger == 2)
            {
                
                if (expireEarly)
                {
                    
                    if (!questData.name.Contains("Two"))
                    {
                        
                        new Throw().ThrowSword(Game1.player, 57, bossTile, 500);

                        if (Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                        {
                            
                            Mod.instance.dialogue["Jester"].specialDialogue.Add("quests", new List<string>()
                            {
                              "Jester of Fate:^Thank you for helping me put Thanatoshi to rest.",
                              "I'm sorry about your kinsman.",
                              "I think this cutlass is to blame"
                            });

                        }

                    }

                    EventComplete();

                }
                else
                {

                    if (!questData.name.Contains("Two"))
                    {

                        Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi... why...", -1, 2, 3000, 0);

                    }

                    Mod.instance.CastMessage("Try again tomorrow");

                }

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {
            ++activeCounter;

            RemoveLadders();

            if (eventLinger != -1 || activeCounter == 1)
            {
                return;
            }

            if (secondFight)
            {

                SecondFight();

                return;

            }
            
            if (activeCounter == 2)
            {
                
                Location.LocationData.SkullCavernAdd();
                Location.LocationData.SkullCavernEdit();
                targetLocation = Game1.getLocationFromName("UndergroundMine145");
                targetVector = new Vector2(13f, 18f);
                Game1.inMine = true;
                Game1.warpFarmer("UndergroundMine145", 13, 19, 2);
                Game1.xLocationAfterWarp = 13;
                Game1.yLocationAfterWarp = 19;

                voicePosition = new(17 * 64f, 13 * 64f);
                return;
            }
            
            if (activeCounter == 3)
            {

                EventQuery("LocationEdit");
                EventQuery("LocationPortal");

                targetPlayer.Position = new(targetVector.X * 64, targetVector.Y * 64);//Vector2.op_Multiply(targetVector, 64f);
                bossMonster = MonsterData.CreateMonster(17, new Vector2(13f, 9f)) as Reaper;
                if (questData.name.Contains("Two"))
                {
                    bossMonster.HardMode();
                }
                targetLocation.characters.Add(bossMonster);
                bossMonster.currentLocation = targetLocation;
                bossMonster.update(Game1.currentGameTime, targetLocation);
                SetTrack("LavaMine");
                bossTile = new Vector2(13f, 9f);

                braziers.Add(new(targetLocation, new(10, 9)));

                braziers.Add(new(targetLocation, new(7, 19)));

                braziers.Add(new(targetLocation, new(18, 9)));

                braziers.Add(new(targetLocation, new(21, 19)));

                return;

            }

            if (activeCounter == 5 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("What a moment...", -1, 2, 3000, 0);

            if (activeCounter == 10 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi?", -1, 3, 3000, 0);

            if (activeCounter == 15 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("Farmer, it's him, The Reaper of Fate", -1, 3, 3000, 0);

            if (activeCounter == 20 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("Thanatoshi, stop messing around!", -1, 3, 3000, 0);

            if (activeCounter == 25 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("I am a Fate too you know, the Jester?", -1, 2, 3000, 0);

            if (activeCounter == 30 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("It's no use, he's insane", -1, 2, 3000, 0);

            if (activeCounter == 35 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("That's... a cutlass... on the shaft", -1, 2, 3000, 0);

            if (activeCounter == 40 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("What has he done to himself?", -1, 2, 3000, 0);

            if (activeCounter == 40 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("I guess we have no choice...", -1, 2, 3000, 0);

            if (activeCounter == 50 && Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
                Mod.instance.characters["Jester"].showTextAboveHead("For Fate and Fortune!", -1, 3, 3000, 0);

            if (activeCounter > 5)
            {

                if(!ModUtility.MonsterVitals(bossMonster, targetLocation))
                {

                    if (questData.name.Contains("Two"))
                    {
                        secondFight = true;

                        SecondFight();

                    }
                    else
                    {
                        expireEarly = true;
                    }

                }
                else
                {

                    bossTile = new((int)(bossMonster.Position.X / 64), (int)(bossMonster.Position.Y / 64));

                }

            }

            if (activeCounter % 30 == 0)
            {

                ResetBraziers();

            }
        }

        public void SecondFight()
        {
            
            secondCounter++;

            if (secondCounter < 9)
            {

                switch (secondCounter)
                {

                    case 1:

                        CastVoice("...yesss...");

                        targetLocation.playSoundPitched("DragonRoar", 1200);

                        expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120.0;

                        break;

                    case 3:

                        CastVoice("you have done well, shaman");

                        targetLocation.playSoundPitched("DragonRoar", 800);

                        break;

                    case 5:

                        CastVoice("...I return...");

                        targetLocation.playSoundPitched("DragonRoar", 400);

                        break;

                    case 7:

                        secondMonster = MonsterData.CreateMonster(21, new Vector2(13f, 9f)) as Prime;

                        targetLocation.characters.Add(secondMonster);

                        secondMonster.currentLocation = riteData.castLocation;

                        SetTrack("cowboy_boss");

                        break;

                }

                return;
            }

            if(secondCounter == 15)
            {
                secondMonster.showTextAboveHead("For centuries I lingered in bone");
            }

            if (secondCounter == 20)
            {
                secondMonster.showTextAboveHead("As the reaper leeched my life force");
            }

            if (secondCounter == 25)
            {
                secondMonster.showTextAboveHead("But an ancient is never truly gone");
            }

            if (secondCounter == 30)
            {
                secondMonster.showTextAboveHead("As long as my ether remains");
            }

            if (secondCounter == 35)
            {
                secondMonster.showTextAboveHead("I will gather the essence of your soul");
            }

            if (secondCounter == 40)
            {
                secondMonster.showTextAboveHead("And fashion new form from your pieces");
            }

            if (secondCounter == 45)
            {
                secondMonster.showTextAboveHead("The Mistress of Fortune will face my wrath");
            }

            if (secondCounter == 50)
            {
                secondMonster.showTextAboveHead("I will make her my servant");
            }

            if (!ModUtility.MonsterVitals(secondMonster, targetLocation))
            {
                CastVoice("...rwwwghhhh...");

                targetLocation.playSoundPitched("DragonRoar", 400);

                expireEarly = true;

            }


            if (activeCounter % 30 == 0)
            {

                ResetBraziers();

            }

        }

    }

}