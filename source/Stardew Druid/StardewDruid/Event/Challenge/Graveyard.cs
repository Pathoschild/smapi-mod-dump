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
using System.Collections.Generic;

namespace StardewDruid.Event.Challenge
{
    public class Graveyard : ChallengeHandle
    {

        public Shooter bossMonster;

        public Graveyard(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {



        }

        public override void EventTrigger()
        {

            cues = DialogueData.DialogueScene(questData.name);

            challengeSpawn = new() { 1, };
            challengeFrequency = 3;
            challengeAmplitude = 1;
            challengeSeconds = 60;
            challengeWithin = new(42, 85);
            challengeRange = new(10, 7);
            challengeTorches = new()
            {
                new(44, 89),
                new(50, 89),
            };

            SetupSpawn();

            if (questData.name.Contains("Two"))
            {

                monsterHandle.spawnCombat *= 3;

                monsterHandle.spawnCombat /= 2;

            }

            Game1.addHUDMessage(new HUDMessage($"Defeat the shadows!", "2"));

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

            if (eventLinger == -1)
            {

                List<string> NPCIndex = VillagerData.VillagerIndex("town");

                Game1.addHUDMessage(new HUDMessage($"You have gained favour with the town residents and their friends", ""));

                EventComplete();

                if (!questData.name.Contains("Two"))
                {

                    ModUtility.UpdateFriendship(Game1.player, NPCIndex);
                    Mod.instance.dialogue["Effigy"].specialDialogue["journey"] = new() { "I sense a change", "The graveyard has a few less shadows." };

                }

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

            monsterHandle.SpawnInterval();

            if (activeCounter == 1)
            {

                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(12, new(47, 82));

                bossMonster = theMonster as Shooter;

                bossMonster.posturing.Set(true);

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

                return;

            }

            if (ModUtility.MonsterVitals(bossMonster, targetLocation))
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = bossMonster, },activeCounter);

                switch (activeCounter)
                {

                    case 5: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 6: bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 8: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 11: bossMonster.shiftPosition = true; bossMonster.SetMovingRight(true); break;

                    case 12: bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 14: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 17: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 18: bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 20: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 23: bossMonster.shiftPosition = true; bossMonster.SetMovingRight(true); break;

                    case 24: bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 26: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 29: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 30:  bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 32: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 35: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 36: bossMonster.shiftPosition = false; break;

                    case 39: 
                        bossMonster.posturing.Set(false);
                        bossMonster.focusedOnFarmers = true; break;

                    case 57:

                        ModUtility.AnimateBolt(riteData.castLocation, bossMonster.getTileLocation());

                        break;

                    case 59:

                        ModUtility.AnimateBolt(riteData.castLocation, bossMonster.getTileLocation());

                        bossMonster.takeDamage(bossMonster.MaxHealth + 5, 0, 0, false, 999, targetPlayer);

                        break;

                    default: break;

                }

            }

        }
    }
}
