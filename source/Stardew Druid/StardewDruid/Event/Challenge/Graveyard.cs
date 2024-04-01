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

        public Graveyard(Vector2 target,  Quest quest)
            : base(target, quest)
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

            Mod.instance.CastMessage("Defeat the shadows!", 2);

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

            if (eventLinger == -1)
            {

                List<string> NPCIndex = VillagerData.VillagerIndex("town");

                EventComplete();

                if (!questData.name.Contains("Two"))
                {

                    Mod.instance.CastMessage("You have gained favour with the town residents and their friends");

                    ModUtility.UpdateFriendship(Game1.player, NPCIndex);
                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "Graveyard");

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

                bossMonster = new(new Vector2(47, 82), Mod.instance.CombatModifier());

                bossMonster.posturing.Set(true);

                Mod.instance.rite.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

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

                        ModUtility.AnimateBolt(Mod.instance.rite.castLocation, bossMonster.Position + new Vector2(32));

                        break;

                    case 59:

                        ModUtility.AnimateBolt(Mod.instance.rite.castLocation, bossMonster.Position + new Vector2(32));

                        bossMonster.takeDamage(bossMonster.MaxHealth + 5, 0, 0, false, 999, targetPlayer);

                        break;

                    default: break;

                }

            }

        }
    }
}
