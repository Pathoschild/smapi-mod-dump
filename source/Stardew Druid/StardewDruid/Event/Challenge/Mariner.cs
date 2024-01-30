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
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewDruid.Monster.Template;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class Mariner : ChallengeHandle
    {
        public List<Vector2> cannons;

        public List<BarrageHandle> barrages;

        public Mariner(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {

            voicePosition = targetVector * 64 + new Vector2(-10, -56);

            cannons = new();

            barrages = new();

        }

        public override void EventTrigger()
        {

            monsterHandle = new(targetVector, riteData.castLocation);

            monsterHandle.spawnIndex = new() { 2, 3 };

            monsterHandle.spawnFrequency = 2;

            if (questData.name.Contains("Two"))
            {

                monsterHandle.spawnFrequency = 1;

            }

            monsterHandle.spawnWithin = targetVector + new Vector2(-5, 1);

            monsterHandle.spawnRange = new Vector2(11, 11);

            ModUtility.AnimateBolt(targetLocation, targetVector);

            Mod.instance.RegisterEvent(this, "active");

        }

        public override bool EventExpire()
        {

            if (eventLinger == -1)
            {

                if (monsterHandle.monsterSpawns.Count <= 3)
                {
                    CastVoice("aye... maybe as strong as Deep himself...");

                    List<int> intList = new List<int>() { 797, 166 };

                    int num = questData.name.Contains("Two") ? 2 : 1;

                    for (int index = 0; index < num; ++index)
                    {
                        new Throw(targetPlayer, targetVector * 64f, intList[randomIndex.Next(intList.Count)], 0).ThrowObject();

                    }
                        
                }
                else if (monsterHandle.monsterSpawns.Count <= 7)
                {
                    CastVoice("eh. take this an sod off.");

                    List<int> intList = new List<int>() { 265, 275 };

                    int num = questData.name.Contains("Two") ? 2 : 1;

                    for (int index = 0; index < num; ++index)
                    {
                        new Throw(targetPlayer, targetVector * 64f, intList[randomIndex.Next(intList.Count)], 0).ThrowObject();
                    }

                }
                else
                {

                    CastVoice("haha! not good enough for the Lady");

                }

                int monsterDefeated = monsterHandle.spawnTotal - monsterHandle.monsterSpawns.Count;

                Mod.instance.CastMessage("Defeated " + monsterDefeated + " out of " + monsterHandle.spawnTotal + " opponents");

                EventComplete();

                eventLinger = 3;

                RemoveMonsters();

                return true;

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            if (activeCounter < 7)
            {
                switch (activeCounter)
                {
                    case 1:

                        CastVoice("oi matey!");

                        break;

                    case 3:

                        CastVoice("ya dare wield the Lady's power here?");

                        break;

                    case 5:

                        CastVoice("the deep one take you!");

                        break;

                    case 6:

                        SetTrack("PIRATE_THEME");

                        break;

                    default:

                        targetLocation.playSound("thunder_small");

                        soundTrack = true;

                        Mod.instance.CastMessage("Defeat more foes for better rewards", -1);

                        CannonsToTheFore();

                        break;

                }

                return;
            }

            if (activeCounter <= 56) { monsterHandle.SpawnInterval(); } else { monsterHandle.SpawnCheck(); }

            /*"There's nay a way to modify me greatness",
            "I've been here chatting every minute,",
            "Of every day for months!",
            "My friend gave me the old Mariner role!",
            "Now I can perpetuate the same dumb interjections,",
            "to every newb that walks over here!",
            "!WherePendant",
            "You're not ready for it",
            "!GetLost",
            "What do you mean my feet aren't natural?",
            "No one asked for your critique, feet hater!",
            "Making a fuss for attention is expected here",
            "It's the only thing I know I'm good at",
            "I don't care if I asked you to comment! Bigot!",
            "I'm not much of a fighter myself.",
            "Pr'fer to just ban folks I don't agree wid.",*/

            switch (activeCounter)
            {

                case 12: CastVoice("ya can't touch me! I be a reflection!", 3000); break;

                case 15: CastVoice("this ere beach is for private members!", 3000); break;

                case 18: CannonsAtTheReady(); break;
                case 20: CannonsToFire(); break;
                case 21: CannonsToImpact(); break;

                case 24: CastVoice("the Lady is not a friend to the drowned", 3000); break;

                case 27: CastVoice("she buried us with our boats on this shore", 3000); break;

                case 30: CastVoice("but soon Lord Deep will avenge us!", 3000); break;

                case 33: CastVoice("he'll swallow the ol' sea witch whole", 3000); break;

                case 36: CastVoice("then the waves will no longer wash our tattered bones", 3000); break;

                case 39: CastVoice("an we'll sink into the warm embrace of the earth", 3000); break;

                case 42: CannonsAtTheReady(); break;
                case 44: CannonsToFire(); break;
                case 45: CannonsToImpact(); break;

                case 52: CannonsAtTheReady(); break;
                case 54: CannonsToFire(); break;
                case 55: CannonsToImpact(); break;

                default: break;

            }

        }

        public void CannonsToTheFore()
        {

            cannons = new()
            {
                monsterHandle.spawnWithin + new Vector2(1,4),

                monsterHandle.spawnWithin + new Vector2(7,4),

                monsterHandle.spawnWithin + new Vector2(13,4),

                monsterHandle.spawnWithin + new Vector2(4,9),

                monsterHandle.spawnWithin + new Vector2(10,9),

                monsterHandle.spawnWithin + new Vector2(1,14),

                monsterHandle.spawnWithin + new Vector2(7,14),

                monsterHandle.spawnWithin + new Vector2(13,14),

            };

            for (int k = 0; k < cannons.Count; k++)
            {

                BarrageHandle missile = new(targetLocation, cannons[k], monsterHandle.spawnWithin, 2, 1, "Black", Mod.instance.CombatModifier() * 2, Mod.instance.DamageLevel());

                barrages.Add(missile);

            }

        }

        public void CannonsAtTheReady()
        {

            CastVoice("CANNONS AT THE READY!", 3000);

            foreach (StardewValley.Monsters.Monster monsterSpawn in monsterHandle.monsterSpawns)
            {
                
                if (monsterSpawn is Skeleton pirateSkeleton)
                {
                    pirateSkeleton.triggerPanic();

                }
                else if (monsterSpawn is Golem pirateGolem)
                {

                    pirateGolem.triggerPanic();

                }

            }
            
            foreach(BarrageHandle barrage in barrages)
            {

                barrage.TargetCircle(3);

                barrage.OuterCircle(3);

            }


        }

        public void CannonsToFire()
        {

            CastVoice("FIRE!");

            foreach (BarrageHandle barrage in barrages)
            {

                barrage.MissileBarrage();

            }

        }

        public void CannonsToImpact()
        {

            targetLocation.localSound("explosion");

            foreach (BarrageHandle barrage in barrages)
            {

                barrage.RadialDamage();

            }

        }

    }

}
