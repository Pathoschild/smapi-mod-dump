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
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeFates : EventHandle
    {

        public int activeSection;

        public Dictionary<int,StardewDruid.Monster.Boss> bosses = new();

        public List<Vector2> spawnPoints = new();

        public List<Vector2> retreatPoints = new();

        public Dictionary<int, int> previousModes = new();

        public Dictionary<int, EventDisplay> eventDisplays = new();

        public ChallengeFates()
        {

            activeLimit = 150;

            mainEvent = true;

        }

        public override void EventActivate()
        {

            base.EventActivate();

            narrators = DialogueData.DialogueNarrators(eventId);

            cues = DialogueData.DialogueScene(eventId);

            eventProximity = -1;

            location.warps.Clear();

            spawnPoints = new()
            {

                new Vector2(30, 16),
                new Vector2(29, 14),
                new Vector2(31, 14),
                new Vector2(33, 16),

            };

            retreatPoints = new()
            {

                new Vector2(33, 13),
                new Vector2(32, 12),
                new Vector2(34, 12),
                new Vector2(36, 13),

            };

        }

        public override void EventRemove()
        {

            if (location != null)
            {

                location.updateWarps();

            }

            for (int b = bosses.Count - 1; b >= 0; b--)
            {

                Boss boss = bosses[b];

                location.characters.Remove(boss);

            }

            bosses.Clear();

            for (int c = companions.Count - 1; c >= 0; c--)
            {

                companions[c].SwitchToMode(Character.Character.mode.random, Game1.player);

            }

            companions.Clear();

            base.EventRemove();

        }

        public override void EventInterval()
        {

            activeCounter++;

            SetTrack("LavaMine");

            switch (activeSection)
            {
                default:
                case 0:

                    EventPartOne();

                    break;

                case 1:

                    EventPartTwo();

                    break;

                case 2:

                    EventPartThree();

                    break;


            }

        }

        public void SpawnBosses()
        {

            // ========================== Leader

            DarkLeader darkLeader = new DarkLeader(spawnPoints[0], Mod.instance.CombatDifficulty());

            bosses[0] = darkLeader;

            darkLeader.SetMode(3);

            darkLeader.netPosturing.Set(true);

            location.characters.Add(darkLeader);

            darkLeader.update(Game1.currentGameTime, location);

            voices[3] = darkLeader;

            Mod.instance.iconData.ImpactIndicator(location, bosses[0].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

            // ========================== Shooter

            DarkShooter darkShooter = new(spawnPoints[1], Mod.instance.CombatDifficulty());

            bosses[1] = darkShooter;

            darkShooter.SetMode(3);

            darkShooter.netPosturing.Set(true);

            location.characters.Add(darkShooter);

            darkShooter.update(Game1.currentGameTime, location);

            voices[4] = darkShooter;

            Mod.instance.iconData.ImpactIndicator(location, bosses[1].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

            // ========================== Goblin

            DarkGoblin darkGoblin = new(spawnPoints[2], Mod.instance.CombatDifficulty());

            bosses[2] = darkGoblin;

            darkGoblin.SetMode(3);

            darkGoblin.netPosturing.Set(true);

            location.characters.Add(darkGoblin);

            darkGoblin.update(Game1.currentGameTime, location);

            voices[5] = darkGoblin;

            Mod.instance.iconData.ImpactIndicator(location, bosses[2].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

            // ========================== Rogue

            DarkRogue darkRogue = new(spawnPoints[3], Mod.instance.CombatDifficulty());

            bosses[3] = darkRogue;

            darkRogue.SetMode(3);

            darkRogue.netPosturing.Set(true);

            location.characters.Add(darkRogue);

            darkRogue.update(Game1.currentGameTime, location);

            voices[6] = darkRogue;

            Mod.instance.iconData.ImpactIndicator(location, bosses[3].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

            // =========================== MonsterHandle

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 12; i++)
            {

                monsterHandle.spawnSchedule.Add(
                    i,
                    new() {
                        new(MonsterHandle.bosses.darkbrute, Boss.temperment.random, Boss.difficulty.medium),
                    }
                );

            }

            monsterHandle.spawnWithin = ModUtility.PositionToTile(origin) - new Vector2(8, 4);

            monsterHandle.spawnRange = new(16, 7);

            return;

        }

        public void EngageBosses()
        {

            // ========================== Leader

            EventDisplay bossBar = Mod.instance.CastDisplay("Shadowfolk Leader", "Shadowfolk Leader");

            bossBar.boss = bosses[0];

            bossBar.type = EventDisplay.displayTypes.bar;

            bossBar.colour = Microsoft.Xna.Framework.Color.Red;

            bosses[0].netPosturing.Set(false);

            eventDisplays[0] = bossBar;

            // ========================== Shooter

            EventDisplay bossBar1 = Mod.instance.CastDisplay("Shadowfolk Bomber", "Shadowfolk Bomber");

            bossBar1.boss = bosses[1];

            bossBar1.type = EventDisplay.displayTypes.bar;

            bossBar1.colour = Microsoft.Xna.Framework.Color.Red;

            bosses[1].netPosturing.Set(false);

            eventDisplays[1] = bossBar1;

            // ========================== Goblin

            EventDisplay bossBar2 = Mod.instance.CastDisplay("Shadowfolk Goblin", "Shadowfolk Goblin");

            bossBar2.boss = bosses[2];

            bossBar2.type = EventDisplay.displayTypes.bar;

            bossBar2.colour = Microsoft.Xna.Framework.Color.Red;

            bosses[2].netPosturing.Set(false);

            eventDisplays[2] = bossBar2;

            // ========================== Rogue

            EventDisplay bossBar3 = Mod.instance.CastDisplay("Shadowfolk Rogue", "Shadowfolk Rogue");

            bossBar3.boss = bosses[3];

            bossBar3.type = EventDisplay.displayTypes.bar;

            bossBar3.colour = Microsoft.Xna.Framework.Color.Red;

            bosses[3].netPosturing.Set(false);

            eventDisplays[3] = bossBar3;

        }

        public void StayCompanions()
        {

            // The Effigy

            companions[0] = Mod.instance.characters[CharacterHandle.characters.Effigy];

            companions[0].SwitchToMode(Character.Character.mode.scene, Game1.player);

            CharacterMover.Warp(location, companions[0], Game1.player.Position - new Vector2(128,0));

            companions[0].LookAtTarget(bosses[0].Position);

            companions[0].eventName = eventId;

            voices[0] = companions[0];

            // Jester

            companions[1] = Mod.instance.characters[CharacterHandle.characters.Jester];

            companions[1].SwitchToMode(Character.Character.mode.scene, Game1.player);

            CharacterMover.Warp(location, companions[1], Game1.player.Position + new Vector2(128, 0));

            companions[1].LookAtTarget(bosses[0].Position);

            companions[1].eventName = eventId;

            voices[1] = companions[1];

            // Buffin

            companions[2] = Mod.instance.characters[CharacterHandle.characters.Buffin];

            companions[2].SwitchToMode(Character.Character.mode.scene, Game1.player);

            CharacterMover.Warp(location, companions[2], Game1.player.Position + new Vector2(64, 128));

            companions[2].LookAtTarget(bosses[0].Position);

            companions[2].eventName = eventId;

            voices[2] = companions[2];

        }


        public void FreeCompanions()
        {

            companions[0].SwitchToMode(Character.Character.mode.track, Game1.player);

            companions[1].SwitchToMode(Character.Character.mode.track, Game1.player);

            companions[2].SwitchToMode(Character.Character.mode.track, Game1.player);

        }

        public void DialogueCueWithFeeling(int cueIndex)
        {

            if (cues.ContainsKey(cueIndex))
            {

                foreach (KeyValuePair<int, string> cue in cues[cueIndex])
                {

                    if (voices.ContainsKey(cue.Key))
                    {

                        if(voices[cue.Key] is StardewDruid.Character.Character companion)
                        {

                            companion.netSpecialActive.Set(true);

                            companion.specialTimer = 2 * companion.specialInterval;

                        }
                        else if (voices[cue.Key] is StardewDruid.Monster.Boss boss)
                        {

                            boss.netSpecialActive.Set(true);

                            boss.specialFrame = 1;

                            boss.specialTimer = boss.specialInterval;
                        
                        }

                    }

                }

            }
            
            
            DialogueCue(cueIndex);

        }


        public void EventPartOne()
        {

            DialogueCueWithFeeling(activeCounter);

            if (activeCounter == 1)
            {

                SpawnBosses();

                StayCompanions();

            }

            if(activeCounter == 23)
            {

                activeSection = 1;

            }

        }

        public void EventPartTwo()
        {

            DialogueCue(activeCounter);

            if (activeCounter == 24)
            {

                EventBar("The Court of Shadows", 0);

                EngageBosses();

                FreeCompanions();

            }

            monsterHandle.SpawnCheck();

            if(activeCounter % 8 == 0)
            {

                monsterHandle.SpawnInterval();

            }

            int defeated = 0;

            for (int b = bosses.Count - 1; b >= 0; b--)
            {

                Boss boss = bosses[b];

                if (boss.netPosturing.Value)
                {

                    defeated++;

                    continue;

                }

                if (!ModUtility.MonsterVitals(boss,location) || activeCounter == 89)
                {

                    boss.ResetActives();

                    boss.Position = retreatPoints[b] * 64;

                    boss.Health = boss.MaxHealth;

                    boss.netPosturing.Set(true);

                    boss.netWoundedActive.Set(true);

                    boss.SetDirection(retreatPoints[0]*64);

                    eventDisplays[b].shutdown();

                    Mod.instance.iconData.AnimateQuickWarp(location, boss.Position);

                    defeated++;

                }

            }

            if(defeated == bosses.Count || activeCounter == 89)
            {

                monsterHandle.ShutDown();

                activeSection = 2;

                StayCompanions();

            }

        }

        public void EventPartThree()
        {

            DialogueCue(activeCounter);

            switch (activeCounter)
            {

                case 122:

                    Mod.instance.iconData.ImpactIndicator(location, bosses[2].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

                    bosses[1].currentLocation.characters.Remove(bosses[1]);

                    bosses.Remove(1);

                    voices.Remove(4);
                    break;

                case 123:

                    Mod.instance.iconData.ImpactIndicator(location, bosses[2].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void, });

                    bosses[3].currentLocation.characters.Remove(bosses[3]);

                    bosses.Remove(3);

                    voices.Remove(6);

                    break;

                case 124:

                    Mod.instance.iconData.ImpactIndicator(location, bosses[2].Position, IconData.impacts.puff, 2f, new() { scheme = IconData.schemes.Void,});

                    bosses[2].currentLocation.characters.Remove(bosses[2]);

                    bosses.Remove(2);

                    voices.Remove(5);

                    bosses[0].currentLocation.characters.Remove(bosses[0]);

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Shadowtin, Character.Character.mode.scene);

                    companions[3] = Mod.instance.characters[CharacterHandle.characters.Shadowtin];

                    CharacterMover.Warp(location, companions[3], bosses[0].Position, false);

                    bosses.Remove(0);

                    voices[3] = companions[3];

                    companions[3].LookAtTarget(Game1.player.Position);

                    break;

                case 135: DialogueSetups(companions[3], 1); companions[3].LookAtTarget(Game1.player.Position); break;

                case 145: DialogueSetups(companions[1], 2); companions[3].LookAtTarget(Game1.player.Position); break;

                case 150: eventComplete = true; break;

            }

        }

        public override void EventCompleted()
        {

            ThrowHandle throwRelic = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Shadowtin].Position, IconData.relics.shadowtin_tome);

            throwRelic.register();

            Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.shadowtin_tome.ToString());

            Mod.instance.questHandle.CompleteQuest(eventId);

        }

        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {
                default:
                case 1:

                    intro = "Shadow Leader: ^My name is Shadowtin Bear. I am foremost a scholar of antiquity, but I was responsible for my company's operations on the surfaceland, and for that, I accept the consequence of my defeat.";

                    break;

                case 2:

                    intro = "The Jester of Fate: ^He's tracking my kinsman! This is good for us, farmer, we might have finally found someone who can figure out where Thanatoshi went.";

                    break;

            }

            List<Response> responseList = new();

            switch (dialogueId)
            {
                default:
                case 1: 

                    responseList.Add(new Response("1a", "Shadowtin, why did you infiltrate the valley?").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("1a", "Antiquity indeed. Your tactics were certainly outdated."));
                    responseList.Add(new Response("1a", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "I feel inclined to deny your masters any power that might threaten peace in the valley. You will aid us instead.").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Enter));
                    responseList.Add(new Response("2a", "I fought a dragon once. How long ago was that? Seems like I'm always fighting when I'd rather make friends. Welcome new friend."));
                    responseList.Add(new Response("2a", "(Say nothing)").SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogue)
        {

            StardewValley.NPC npc = companions[0];

            string response;

            switch (dialogue)
            {

                case "1a":

                    activeCounter = Math.Max(144, activeCounter);

                    response = "If you're familiar with the old legends, this is where the stars fell in the war that claimed the Dragons. " +
                        "We're searching for remnants of that war. Specifically, the power of the ancient ones over the Ether. " +
                        "Our efforts have been fruitless, save for a cache of writings and other ornaments we found in the nearby tunnels. " +
                        "It appears to have been a repository for human followers of the Reaper of Fate. " +
                        "One of the texts mentions the tomb of Tyrannus Jin, once dragon king of this land, but there are no descriptions of its location.";

                    DialogueDraw(companions[3],response);

                    break;

                case "2a":

                    activeCounter = Math.Max(149, activeCounter);

                    response = "I'm surprised that you would consider me for a partnership, but I will accept, gratefully. " +
                        "I have my own reasons for studying the forgotten war between dragons and elderborn, and if I get the opportunity to pursue my research, then I will. " +
                        "I also expect I will have to face my former masters at some point. Until then, my allegiance is yours. " +
                        "As a token of my goodwill, I entrust to you the old text I spoke of, and the compendium of my own research. " +
                        "The text also made mention of the circle of druids, and are part of the reason I wanted to capture one of your number for questioning.";

                    DialogueDraw(companions[3], response);

                    ThrowHandle throwRelic = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Shadowtin].Position, IconData.relics.book_wyrven);

                    throwRelic.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.book_wyrven.ToString());

                    break;

            }

        }

        public override float DisplayProgress(int displayId = 0)
        {

            if (activeCounter <= 24)
            {

                return 0f;

            }

            if(activeCounter >= 90)
            {

                return -1f;

            }

            float progress = ((float)activeCounter - 24f) / (float)66;

            return progress;

        }

    }

}
