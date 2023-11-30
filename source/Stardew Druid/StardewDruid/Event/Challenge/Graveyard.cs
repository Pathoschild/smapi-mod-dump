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
using StardewDruid.Monster;
using StardewValley;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Challenge
{
    public class Graveyard : ChallengeHandle
    {

        public BossShooter bossMonster;

        public Graveyard(Vector2 target, Rite rite, Quest quest)
            : base(target, rite, quest)
        {



        }

        public override void EventTrigger()
        {

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

            if (questData.name.Contains("Two"))
            {
                challengeFrequency = 3;
                challengeAmplitude = 2;
            }

            SetupSpawn();

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

            if(eventLinger == -1)
            {

                List<string> NPCIndex = VillagerData.VillagerIndex("town");

                Game1.addHUDMessage(new HUDMessage($"You have gained favour with the town residents and their friends", ""));

                Mod.instance.CompleteQuest(questData.name);

                if (!questData.name.Contains("Two"))
                {

                    UpdateFriendship(NPCIndex);

                    Mod.instance.dialogue["Effigy"].specialDialogue["journey"] = new() { "I sense a change", "The graveyard has a few less shadows." };

                    Mod.instance.LevelBlessing("water");

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

            if(eventLinger != -1)
            {

                return;

            }

            monsterHandle.SpawnInterval();

            if (activeCounter == 1)
            {

                StardewValley.Monsters.Monster theMonster = MonsterData.CreateMonster(12, new(47, 82), riteData.combatModifier);

                bossMonster = theMonster as BossShooter;

                bossMonster.posturing = true;

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

                return;

            }

            if (bossMonster.Health >= 1)
            {
                switch (activeCounter)
                {
                    case 2: bossMonster.showTextAboveHead("discovery!", 3000); break;

                    case 5: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 6: bossMonster.showTextAboveHead("ENGAGE"); bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 8: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 9: bossMonster.showTextAboveHead("secure the graveyard", 3000); break;

                    case 11: bossMonster.shiftPosition = true; bossMonster.SetMovingRight(true); break;

                    case 12: bossMonster.showTextAboveHead("FIRE"); bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 14: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 15: bossMonster.showTextAboveHead("protect the tear", 3000); break;

                    case 17: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 18: bossMonster.showTextAboveHead("ENGAGE"); bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 20: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 21: bossMonster.showTextAboveHead("surround the farmer", 3000); break;

                    case 23: bossMonster.shiftPosition = true; bossMonster.SetMovingRight(true); break;

                    case 24: bossMonster.showTextAboveHead("FIRE"); bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 26: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 27: bossMonster.showTextAboveHead("pin them down!", 3000); break;

                    case 29: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 30: bossMonster.showTextAboveHead("ENGAGE"); bossMonster.shiftPosition = false; bossMonster.aimPlayer = true; break;

                    case 32: bossMonster.aimPlayer = false; bossMonster.shootPlayer = true; break;

                    case 33: bossMonster.showTextAboveHead("no retreat for us", 3000); break;

                    case 35: bossMonster.shiftPosition = true; bossMonster.SetMovingLeft(true); break;

                    case 36: bossMonster.showTextAboveHead("the Deep One sees all", 3000); bossMonster.shiftPosition = false; break;

                    case 39: bossMonster.showTextAboveHead("ADVANCE"); bossMonster.posturing = false; bossMonster.focusedOnFarmers = true; break;

                    case 56:

                        bossMonster.showTextAboveHead("such power");

                        break;

                    case 57:

                        ModUtility.AnimateBolt(riteData.castLocation, bossMonster.getTileLocation());

                        break;

                    case 58:

                        bossMonster.showTextAboveHead("Lord Deep... forgive us");

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
