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
using StardewModdingAPI.Events;
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

        public SkullCavern(Vector2 target,  Quest quest)
          : base(target, quest)
        {

            targetVector = target;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120.0;

        }

        public override void EventTrigger()
        {

            cues = DialogueData.DialogueScene(questData.name);

            ModUtility.AnimateDecoration(targetLocation, targetVector*64, "weald");

            ModUtility.AnimateRockfalls(targetLocation, targetPlayer.Tile);

            DelayedAction.functionAfterDelay(RockfallSounds, 575);

            DelayedAction.functionAfterDelay(RockfallSounds, 675);

            DelayedAction.functionAfterDelay(RockfallSounds, 775);

            Mod.instance.RegisterEvent(this, "active");
        }

        public void RockfallSounds()
        {
            targetLocation.playSound(new Random().Next(2) == 0 ? "boulderBreak" : "boulderCrack", targetVector*64, 800);
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
                            
                            Mod.instance.dialogue["Jester"].AddSpecial("Jester", "ThanatoshiThree");

                        }

                    }

                    EventComplete();

                }
                else
                {

                    if (!questData.name.Contains("Two"))
                    {
                        
                        DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = Mod.instance.characters["Jester"], }, 991);

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
                
                Location.LocationData.SkullCavernEdit();
                targetLocation = Game1.getLocationFromName("UndergroundMine145");
                targetVector = new Vector2(13f, 18f);
                Game1.warpFarmer("UndergroundMine145", 13, 19, 2);
                Game1.xLocationAfterWarp = 13;
                Game1.yLocationAfterWarp = 19;

                EventQuery("LocationEdit");
                //EventQuery("LocationPortal");

                AddActor(new(17 * 64f, 13 * 64f));
                return;
            }
            
            if (activeCounter == 3)
            {

                targetPlayer.Position = new(targetVector.X * 64, targetVector.Y * 64);//Vector2.op_Multiply(targetVector, 64f);
                EventQuery("LocationPortal");

                bossMonster = new(new Vector2(13f, 9f),Mod.instance.CombatModifier());
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

            if(Mod.instance.characters["Jester"].currentLocation.Name == targetLocation.Name)
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = Mod.instance.characters["Jester"], }, activeCounter);

            }

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

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [1] = actors[0], }, secondCounter + 200);

                switch (secondCounter)
                {

                    case 1:

                        targetLocation.playSound("DragonRoar",targetVector*64, 1200);

                        expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 120.0;

                        break;

                    case 3:

                        targetLocation.playSound("DragonRoar", targetVector * 64, 800);

                        break;

                    case 5:

                        targetLocation.playSound("DragonRoar", targetVector * 64, 400);

                        break;

                    case 7:

                        secondMonster = new(new Vector2(13f, 9f),Mod.instance.CombatModifier());

                        targetLocation.characters.Add(secondMonster);

                        secondMonster.currentLocation = Mod.instance.rite.castLocation;

                        SetTrack("cowboy_boss");

                        break;

                }

                return;
            }

            if (!ModUtility.MonsterVitals(secondMonster, targetLocation))
            {
                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [1] = actors[0], }, 992);

                targetLocation.playSound("DragonRoar", targetVector*64,400);

                expireEarly = true;

                return;

            }

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [1] = secondMonster, }, secondCounter + 200);

            if (activeCounter % 30 == 0)
            {

                ResetBraziers();

            }

        }

    }

}