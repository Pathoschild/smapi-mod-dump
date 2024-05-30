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
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Monster;
using StardewValley;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeMists : EventHandle
    {

        public StardewDruid.Monster.DarkShooter bossShooter;

        public StardewDruid.Monster.DarkLeader bossLeader;

        public int destructionAverted;

        public ChallengeMists()
        {

            mainEvent = true;

        }

        public override void EventActivate()
        {

            base.EventActivate();

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 10; i += 4)
            {

                monsterHandle.spawnSchedule.Add(i, new() { new(MonsterHandle.bosses.darkbrute, Boss.temperment.random, Boss.difficulty.medium) });

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(origin) - new Vector2(4, 3);

            monsterHandle.spawnRange = new(9, 9);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 75;

            eventProximity = 1280;

            cues = DialogueData.DialogueScene(eventId);

            narrators = DialogueData.DialogueNarrators(eventId);

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            if (Mod.instance.trackers.ContainsKey(CharacterData.characters.Effigy))
            {

                voices[3] = Mod.instance.characters[CharacterData.characters.Effigy];

                Mod.instance.characters[CharacterData.characters.Effigy].Halt();

                Mod.instance.characters[CharacterData.characters.Effigy].idleTimer = 300;

            }

            location.playSound("thunder_small");

        }

        public override void RemoveMonsters()
        {

            if (bossShooter != null)
            {

                bossShooter.currentLocation.characters.Remove(bossShooter);

                bossShooter.currentLocation = null;

                bossShooter = null;

            }

            if (bossLeader != null)
            {

                bossLeader.currentLocation.characters.Remove(bossLeader);

                bossLeader.currentLocation = null;

                bossLeader = null;

            }

            base.RemoveMonsters();

        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();

            switch (activeCounter)
            {

                case 2:

                    DarkShooter shooter = new(ModUtility.PositionToTile(origin) - new Vector2(2, 2), Mod.instance.CombatDifficulty());

                    shooter.SetMode(2);

                    shooter.netPosturing.Set(true);

                    location.characters.Add(shooter);

                    shooter.currentLocation = location;

                    shooter.LookAtFarmer();

                    shooter.update(Game1.currentGameTime, location);

                    bossShooter = shooter;

                    voices[0] = shooter;

                    break;

                case 4:

                    DarkBrute brute = new(ModUtility.PositionToTile(origin) + new Vector2(2, -1), Mod.instance.CombatDifficulty());

                    brute.SetMode(1);

                    brute.netPosturing.Set(true);

                    location.characters.Add(brute);

                    brute.currentLocation = location;

                    brute.LookAtFarmer();

                    brute.update(Game1.currentGameTime, location);

                    voices[1] = brute;

                    break;

                case 6:

                    (voices[1] as StardewDruid.Monster.Boss).netPosturing.Set(false);

                    voices.Remove(1);

                    Mod.instance.CastMessage("Hit the cannoneer to prevent them from firing on the town!");

                    RepositionShooter();

                    break;

                case 9:

                    PrepareShooter();

                    break;

                case 10:
                case 11:
                case 12:

                    StandbyShooter();

                    break;

                case 13:

                    EngageShooter(13);

                    break;

                case 15:

                    RepositionShooter();

                    break;

                case 18:

                    PrepareShooter();

                    break;

                case 19:
                case 20:
                case 21:

                    StandbyShooter();

                    break;

                case 22:

                    EngageShooter(22);

                    break;

                case 24:

                    RepositionShooter();

                    break;

                case 28:

                    if(monsterHandle.monsterSpawns.Count > 0)
                    {

                        voices[1] = monsterHandle.monsterSpawns.First();

                    }
                    else
                    {

                        voices.Remove(1);

                    }

                    RepositionShooter();

                    break;

                case 31:

                    PrepareShooter();

                    break;

                case 32:
                case 33:
                case 34:

                    StandbyShooter();

                    break;

                case 35:

                    EngageShooter(35);

                    break;

                case 37:

                    RepositionShooter();

                    break;

                case 38:


                    if (monsterHandle.monsterSpawns.Count > 0)
                    {

                        voices[1] = monsterHandle.monsterSpawns.First();

                    }
                    else
                    {

                        voices.Remove(1);

                    }
                    break;

                case 40:

                    RepositionShooter();

                    break;

                case 42:

                    if (monsterHandle.monsterSpawns.Count > 0)
                    {

                        voices[1] = monsterHandle.monsterSpawns.First();

                    }
                    else
                    {

                        voices.Remove(1);

                    }

                    break;

                case 44:

                    PrepareShooter();

                    break;

                case 45:
                case 46:
                case 47:

                    StandbyShooter();

                    break;

                case 48:

                    EngageShooter(48);

                    break;

                case 50:

                    DarkLeader leader = new(ModUtility.PositionToTile(origin), Mod.instance.CombatDifficulty());

                    leader.SetMode(2);

                    leader.netPosturing.Set(true);

                    location.characters.Add(leader);

                    leader.currentLocation = location;

                    leader.LookAtFarmer();

                    leader.update(Game1.currentGameTime, location);

                    bossLeader = leader;

                    voices[2] = leader;

                    bossShooter.SetDirection(origin + new Vector2(-128, -64));

                    bossShooter.PerformFlight(origin + new Vector2(-128,-64), 0);

                    break;

                case 55:

                    bossLeader.SetDirection(Game1.player.Position);

                    bossLeader.netSpecialActive.Set(true);

                    bossLeader.specialTimer = 90;

                    bossLeader.specialFrame = 1;

                    PrepareShooter();

                    break;

                case 56:
                case 57:
                case 58:

                    bossLeader.SetDirection(Game1.player.Position);

                    bossLeader.netSpecialActive.Set(true);

                    bossLeader.specialTimer = 90;

                    bossLeader.specialFrame = 1;
                    
                    StandbyShooter();

                    break;

                case 59:

                    EngageShooter(59);

                    break;

                case 61:

                    bossLeader.SetDirection(origin + new Vector2(384, -384));

                    bossLeader.PerformFlight(origin + new Vector2(384, -384));

                    break;

                case 63:

                    bossShooter.SetDirection(origin + new Vector2(320, -384));

                    bossShooter.PerformFlight(origin + new Vector2(320, -384));

                    break;

                case 65:

                    bossShooter.SetDirection(origin + new Vector2(640, -1280));

                    bossShooter.PerformFlight(origin + new Vector2(640, -1280));

                    bossLeader.LookAtFarmer();

                    bossLeader.netSpecialActive.Set(true);

                    bossLeader.specialTimer = 60;

                    bossLeader.specialFrame = 0;

                    break;

                case 67:

                    bossLeader.SetDirection(origin + new Vector2(640, -1280));

                    bossLeader.PerformFlight(origin + new Vector2(640, -1280));

                    break;

                case 68:

                    expireEarly = true;

                    int friendship = destructionAverted * 50;

                    Mod.instance.CastMessage($"Prevented {destructionAverted} acts of destruction, gained " + friendship + " friendship with town residents", 2);

                    VillagerData.CommunityFriendship("town", friendship);

                    Mod.instance.questHandle.CompleteQuest(eventId);

                    break;


            }

            if (activeCounter % 6 == 0)
            {

                monsterHandle.SpawnInterval();

            }

            DialogueCue(activeCounter);

        }

        public void RepositionShooter(int point = -1)
        {

            List<Vector2> points = new()
            {
                origin + new Vector2(-9*64,-7*64),
                origin + new Vector2(6*64,-8*64),
                origin + new Vector2(10*64,10*64),
                origin + new Vector2(-7*64,10*64),

            };

            if(point == -1)
            {

                point = Mod.instance.randomIndex.Next(points.Count);

            }

            bossShooter.SetDirection(points[point]);

            bossShooter.PerformFlight(points[point], 0);

        }

        public void PrepareShooter()
        {

            bossShooter.SetDirection(Game1.player.Position);

            bossShooter.netChannelActive.Set(true);

            bossShooter.specialTimer = 90;

            bossShooter.specialFrame = 0;

            Mod.instance.iconData.CursorIndicator(location, Game1.player.Position, IconData.cursors.scope, new() { scale = 4f, color = Mod.instance.iconData.schemeColours[IconData.schemes.fire], });

        }

        public void StandbyShooter()
        {

            if (bossShooter.netChannelActive.Value)
            {

                bossShooter.SetDirection(Game1.player.Position);

                bossShooter.specialTimer = 90;

                bossShooter.specialFrame = 0;

                Mod.instance.iconData.CursorIndicator(location, Game1.player.Position, IconData.cursors.scope, new() { scale = 4f, color = Mod.instance.iconData.schemeColours[IconData.schemes.fire], });

            }

        }

        public void EngageShooter(int dialogueIndex)
        {

            if (bossShooter.netChannelActive.Value && Vector2.Distance(bossShooter.Position,Game1.player.Position) > 128 )
            {

                bossShooter.PerformBarrage(Game1.player.Position);

                cues[dialogueIndex][0] = Mod.instance.randomIndex.Next(2) == 0 ? "BOOOM" : "FIRE!";

            }
            else
            {

                destructionAverted++;

            }

        }

    }

}
