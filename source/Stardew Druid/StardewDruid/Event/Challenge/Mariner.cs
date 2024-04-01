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

        public Mariner(Vector2 target,  Quest quest)
            : base(target, quest)
        {

            cannons = new();

        }

        public override void EventTrigger()
        {

            cues = DialogueData.DialogueScene(questData.name);

            AddActor(targetVector * 64 + new Vector2(-10, -56));

            monsterHandle = new(targetVector, Mod.instance.rite.castLocation);

            monsterHandle.spawnIndex = new() { 2, 3 };

            monsterHandle.spawnFrequency = 2;

            if (questData.name.Contains("Two"))
            {

                monsterHandle.spawnFrequency = 1;

            }

            monsterHandle.spawnWithin = targetVector + new Vector2(-5, 1);

            monsterHandle.spawnRange = new Vector2(11, 11);

            ModUtility.AnimateBolt(targetLocation, targetVector*64 + new Vector2(32));

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

                        break;

                }

                return;
            }

            if (activeCounter <= 56) { monsterHandle.SpawnInterval(); } else { monsterHandle.SpawnCheck(); }

            switch (activeCounter)
            {

                case 17: CannonsAtTheReady(); break;
                case 19: CannonsToFire(); break;
                case 21: CannonsToImpact(); break;

                case 41: CannonsAtTheReady(); break;
                case 43: CannonsToFire(); break;
                case 45: CannonsToImpact(); break;

                case 51: CannonsAtTheReady(); break;
                case 53: CannonsToFire(); break;
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

                Vector2 impact = cannons[k] * 64;

                SpellHandle missile = new(targetLocation, impact, impact - new Vector2(0, 640), 2, 1, Mod.instance.CombatModifier() * 2, Mod.instance.DamageLevel());

                missile.type = SpellHandle.barrages.ballistic;

                missile.scheme = SpellHandle.schemes.death;

                missile.indicator = SpellHandle.indicators.death;

                Mod.instance.spellRegister.Add(missile);

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

        }

        public void CannonsToFire()
        {

            CannonsToTheFore();

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[0], }, 995);

        }

        public void CannonsToImpact()
        {

            targetLocation.localSound("explosion");

        }

    }

}
