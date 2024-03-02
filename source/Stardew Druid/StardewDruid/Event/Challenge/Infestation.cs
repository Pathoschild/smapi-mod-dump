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
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class Infestation : ChallengeHandle
    {

        public BigSlime bossMonster;

        public Infestation(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
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

            Game1.addHUDMessage(new HUDMessage($"Defeat the slimes!", "2"));

            SetTrack("tribal");

            Mod.instance.RegisterEvent(this, "active");

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                riteData.castLocation.characters.Remove(bossMonster);

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

                Game1.addHUDMessage(new HUDMessage($"You have gained favour with those who love the forest", ""));

                ModUtility.UpdateFriendship(Game1.player,NPCIndex);

                Mod.instance.dialogue["Effigy"].specialDialogue["journey"] = new() { "I sense a change", "I defeated the Pumpkin Slime. Now I'm covered in his gunk." };

                Throw throwHat = new(Game1.player,targetVector*64,99);

                throwHat.objectInstance = new Hat(48);

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

                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(13, bossVector);

                bossMonster = theMonster as BigSlime;

                bossMonster.posturing.Set(true);

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

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

                        bossMonster.stunTime = 5000;

                        break;

                    case 56:

                        bossMonster.Halt();

                        break;

                    case 58:

                        Vector2 meteorVector = bossMonster.getTileLocation();

                        ModUtility.AnimateRadiusDecoration(targetLocation, meteorVector + new Vector2(-2, 1), "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(riteData.castLocation, meteorVector + new Vector2(-2, 1), true);

                        ModUtility.AnimateRadiusDecoration(targetLocation, meteorVector + new Vector2(1, -2), "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(riteData.castLocation, meteorVector + new Vector2(1, -2), true);

                        ModUtility.AnimateRadiusDecoration(targetLocation, meteorVector + new Vector2(2, 1), "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(riteData.castLocation, meteorVector + new Vector2(2, 1), false);

                        ModUtility.AnimateRadiusDecoration(targetLocation, meteorVector + new Vector2(1, 2), "Stars", 1f, 1f);

                        ModUtility.AnimateMeteor(riteData.castLocation, meteorVector + new Vector2(1, 2), false);

                        DelayedAction.functionAfterDelay(MeteorImpact, 600);

                        break;

                    default: break;

                }

            }

        }

        public void MeteorImpact()
        {

            List<Vector2> impactVectors;

            Vector2 impactCenter = bossMonster.getTileLocation();

            for (int i = 0; i < 5; i++)
            {

                impactVectors = ModUtility.GetTilesWithinRadius(riteData.castLocation, impactCenter, i);

                foreach (Vector2 impactVector in impactVectors)
                {

                    ModUtility.AnimateDestruction(targetLocation, impactVector);

                }

            }

            bossMonster.takeDamage(bossMonster.MaxHealth + 5, 0, 0, false, 999, targetPlayer);

        }

    }
}
