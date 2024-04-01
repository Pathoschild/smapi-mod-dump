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
using StardewDruid.Map;
using StardewDruid.Monster.Template;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class Infestation : ChallengeHandle
    {

        public BigSlime bossMonster;

        public Infestation(Vector2 target,  Quest quest)
            : base(target, quest)
        {

        }

        public override void EventTrigger()
        {

            cues = DialogueData.DialogueScene(questData.name);

            challengeSpawn = new() { 0, };
            challengeFrequency = 1;
            challengeAmplitude = 1;
            challengeSeconds = 60;
            challengeWithin = new(72, 71);
            challengeRange = new(14, 13);
            challengeTorches = new()
            {
                new(75, 74),
                new(82, 74),
                new(75, 81),
                new(82, 81),
            };
            
            SetupSpawn();

            if (questData.name.Contains("Two"))
            {
                
                monsterHandle.spawnCombat *= 3;

                monsterHandle.spawnCombat /= 2;

            }

            Mod.instance.CastMessage("Defeat the slimes!", 2);

            SetTrack("tribal");

            Mod.instance.RegisterEvent(this, "active");

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                Mod.instance.rite.castLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            base.RemoveMonsters();

        }

        public override bool EventExpire()
        {

            EventComplete();

            if (!questData.name.Contains("Two"))
            {

                List<string> NPCIndex = VillagerData.VillagerIndex("forest");

                Mod.instance.CastMessage("You have gained favour with those who love the forest", 1, true);

                ModUtility.UpdateFriendship(Game1.player,NPCIndex);

                Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "Infestation");

                Throw throwHat = new(Game1.player,targetVector*64,99);

                throwHat.objectInstance = new Hat("(H)48");

                throwHat.ThrowObject();

            }
            else
            {

                List<int> eggList = new()
                {
                    413,
                    437,
                    439,
                    680,
                    857,
                };

                Throw throwEgg = new(Game1.player, targetVector * 64, eggList[randomIndex.Next(eggList.Count)]);

                throwEgg.ThrowObject();

            }

            return false;

        }

        public override void EventInterval()
        {
            
            activeCounter++;

            monsterHandle.SpawnCheck();

            if (eventLinger != -1)
            {

                return;

            }

            monsterHandle.SpawnInterval();

            if (activeCounter == 14)
            {

                Vector2 bossVector = monsterHandle.SpawnVector(12,76,72,5,4);

                if(bossVector == new Vector2(-1))
                {
                    bossVector = new(78, 74);

                }

                bossMonster = new(bossVector, Mod.instance.CombatModifier());

                bossMonster.posturing.Set(true);

                Mod.instance.rite.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

            }

            if (activeCounter <= 14)
            {
                return;
            }

            if (ModUtility.MonsterVitals(bossMonster, targetLocation))
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = bossMonster,}, activeCounter);

                switch (activeCounter)
                {

                    case 37:

                        bossMonster.posturing.Set(false);

                        bossMonster.focusedOnFarmers = true;

                        break;

                    case 55:

                        bossMonster.Halt();

                        bossMonster.stunTime.Set(Math.Max(bossMonster.stunTime.Value, 5000));

                        break;

                    case 56:

                        bossMonster.Halt();

                        break;

                    case 58:

                        Vector2 meteorVector = bossMonster.Tile;

                        List<Vector2> meteors = new()
                        {

                            (meteorVector + new Vector2(-2, 1)),
                            (meteorVector + new Vector2(1, -2)),
                            (meteorVector + new Vector2(2, 1)),
                            (meteorVector + new Vector2(1, 2)),

                        };
                        
                        foreach (Vector2 meteor in meteors)
                        {

                            Cast.Stars.Meteor meteorCast = new(meteor, Mod.instance.DamageLevel());

                            meteorCast.targetLocation = targetLocation;

                            meteorCast.CastEffect();

                        }

                        break;

                    default: break;

                }

            }

        }

        public void MeteorImpact()
        {

            bossMonster.takeDamage(bossMonster.MaxHealth + 5, 0, 0, false, 999, targetPlayer);

        }

    }
}
