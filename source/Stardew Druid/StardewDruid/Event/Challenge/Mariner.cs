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

            cues = DialogueData.DialogueScene(questData.name);

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

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 991);

                    List<int> intList = new List<int>() { 797, 166 };

                    int num = questData.name.Contains("Two") ? 2 : 1;

                    for (int index = 0; index < num; ++index)
                    {
                        new Throw(targetPlayer, targetVector * 64f, intList[randomIndex.Next(intList.Count)], 0).ThrowObject();

                    }
                        
                }
                else if (monsterHandle.monsterSpawns.Count <= 7)
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 992);

                    List<int> intList = new List<int>() { 265, 275 };

                    int num = questData.name.Contains("Two") ? 2 : 1;

                    for (int index = 0; index < num; ++index)
                    {
                        new Throw(targetPlayer, targetVector * 64f, intList[randomIndex.Next(intList.Count)], 0).ThrowObject();
                    }

                }
                else
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 993);

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

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, activeCounter);

            if (activeCounter < 7)
            {
                switch (activeCounter)
                {

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

            switch (activeCounter)
            {

                case 18: CannonsAtTheReady(); break;
                case 20: CannonsToFire(); break;
                case 21: CannonsToImpact(); break;

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
            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 994);

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

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 995);

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
