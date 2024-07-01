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
using Microsoft.Xna.Framework.Input;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Compat;
using StardewDruid.Compat.v100;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewDruid.Event.Challenge;
using StardewDruid.Event.Scene;
using StardewDruid.Event.Sword;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Constants;
using StardewValley.GameData.Pets;
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace StardewDruid.Journal
{
    public class QuestHandle
    {

        public Dictionary<string, Quest> quests = new();

        public Dictionary<string, List<Effect>> effects = new();

        public Dictionary<LoreData.stories, Lorestory> lores = new();

        public List<LoreData.stories> stories = new();

        public enum milestones
        {
            none,
            effigy,
            weald_weapon,
            weald_lessons,
            weald_challenge,
            mists_weapon,
            mists_lessons,
            quest_effigy,
            mists_challenge,
            stars_weapon,
            stars_lessons,
            stars_challenge,
            stars_threats,
            jester,
            fates_weapon,
            fates_lessons,
            quest_jester,
            fates_enchant,
            fates_challenge,
            ether_weapon,
            ether_lessons,
            quest_shadowtin,
            ether_treasure,
            ether_challenge,

        }

        public static Dictionary<milestones, List<string>> milestoneQuests = new()
        {

            [milestones.effigy] = new() { approachEffigy, },
            [milestones.weald_weapon] = new() { swordWeald, },
            [milestones.weald_lessons] = new() { herbalism, wealdOne, wealdTwo, wealdThree, wealdFour, wealdFive, },
            [milestones.weald_challenge] = new() { challengeWeald, },
            [milestones.mists_weapon] = new() { swordMists },
            [milestones.mists_lessons] = new() { mistsOne, mistsTwo, mistsThree, mistsFour, },
            [milestones.quest_effigy] = new() { questEffigy, },
            [milestones.mists_challenge] = new() { challengeMists, },
            [milestones.stars_weapon] = new() { swordStars, },
            [milestones.stars_lessons] = new() { starsOne, starsTwo, },
            [milestones.stars_challenge] = new() { challengeStars, },
            [milestones.stars_threats] = new() { challengeAtoll, challengeDragon, },
            [milestones.jester] = new() { approachJester, },
            [milestones.fates_weapon] = new() { swordFates, },
            [milestones.fates_lessons] = new() { fatesOne, fatesTwo, fatesThree, },
            [milestones.quest_jester] = new() { questJester, },
            [milestones.fates_enchant] = new() { fatesFour, },
            [milestones.fates_challenge] = new() { challengeFates, },
            [milestones.ether_weapon] = new() { swordEther, },
            [milestones.ether_lessons] = new() { etherOne, etherTwo, etherThree,  },
            [milestones.quest_shadowtin] = new() { questJester, },
            [milestones.ether_treasure] = new() { etherFour, },
            [milestones.ether_challenge] = new() { challengeEther, },

        };

        public const string approachEffigy = "approachEffigy";

        public const string swordWeald = "swordWeald";

        public const string herbalism = "herbalism";

        public const string wealdOne = "clearance";

        public const string wealdTwo = "wildbounty";

        public const string wealdThree = "wildgrowth";

        public const string wealdFour = "cultivate";

        public const string wealdFive = "rockfall";

        public const string challengeWeald = "aquifer";

        public const string swordMists = "swordMists";

        public const string mistsOne = "sunder";

        public const string mistsTwo = "artifice";

        public const string mistsThree = "fishing";

        public const string mistsFour = "smite";

        public const string questEffigy = "questEffigy"; // wisps

        public const string challengeMists = "graveyard";

        public const string relicWeald = "relicWeald";

        public const string relicMists = "relicMists";

        public const string swordStars = "swordStars";

        public const string starsOne = "meteor";

        public const string starsTwo = "gravity";

        public const string challengeStars = "infestation";

        public const string challengeAtoll = "seafarers";

        public const string challengeDragon = "dragon";

        public const string approachJester = "approachJester";

        public const string swordFates = "swordFates";

        public const string fatesOne = "whisk";

        public const string fatesTwo = "warpstrike";

        public const string fatesThree = "tricks";

        public const string questJester = "questJester"; // tricks

        public const string fatesFour = "enchant";

        public const string challengeFates = "rogues";

        public const string swordEther = "swordEther";

        public const string etherOne = "transform";

        public const string etherTwo = "breath";

        public const string etherThree = "dive";

        public const string questShadowtin = "questShadowtin"; // tricks

        public const string etherFour = "treasure";

        public const string challengeEther = "dustfiends";

        public QuestHandle()
        {

            quests = QuestData.QuestList();

            effects = EffectsData.EffectList();

            lores = LoreData.LoreList();

        }

        // ----------------------------------------------------------------------

        public void FixQuests(string questId)
        {

            QuestProgress progress = Mod.instance.save.progress[questId];

            switch (questId)
            {
                case "wealdChallenge":
                case "challengeWeald":

                    Mod.instance.save.progress[challengeWeald] = new(progress.status,progress.delay,progress.progress,progress.replay);

                    break;

                case "challengeMists":

                    Mod.instance.save.progress[challengeMists] = new(progress.status, progress.delay, progress.progress, progress.replay);

                    break;

            }

            Mod.instance.save.progress.Remove(questId);

        }

        public List<List<string>> OrganiseQuests(bool active = false, bool reverse = false)
        {

            List<List<string>> source = new();

            List<string> pageList = new();

            List<string> activeList = new();

            foreach(string id in quests.Keys)
            {

                if (!Mod.instance.save.progress.ContainsKey(id))
                {
                    continue;
                }

                QuestProgress progress = Mod.instance.save.progress[id];

                if (progress.status == 1)
                {
                    if (active)
                    {

                        activeList.Add(id);

                    }
                    else
                    {

                        pageList.Add(id);

                    }

                }

                if (progress.status == 2)
                {

                    pageList.Add(id);

                }
            
            }

            if (reverse)
            {

                pageList.Reverse();

                activeList.Reverse();
            }
            
            if (active)
            {

                activeList.AddRange(pageList);

                pageList = activeList;
            }

            foreach (string page in pageList)
            {

                if (source.Count == 0 || source.Last().Count() == 6)
                {
                    source.Add(new List<string>());
                }

                source.Last().Add(page);

            }

            return source;

        }

        public List<List<string>> OrganiseEffects(bool reverse = false)
        {

            List<List<string>> source = new();

            List<string> pageList = new();

            foreach (KeyValuePair<string, QuestProgress> pair in Mod.instance.save.progress)
            {

                string id = pair.Key;

                QuestProgress progress = pair.Value;

                if (!quests.ContainsKey(id))
                {

                    continue;

                }

                int requirement = quests[id].type == Quest.questTypes.lesson ? 1 : 2;

                if (progress.status >= requirement)
                {

                    if (effects.ContainsKey(id))
                    {

                        for(int i = 0; i < effects[id].Count; i++)
                        {

                            pageList.Add(id+"|"+i.ToString());

                        }

                    }

                }

            }

            if (reverse)
            {

                pageList.Reverse();

            }

            foreach (string page in pageList)
            {

                if (source.Count == 0 || source.Last().Count() == 6)
                {
                    source.Add(new List<string>());
                }

                source.Last().Add(page);

            }

            return source;

        }


        public List<List<string>> OrganiseProgress(bool reverse = false)
        {

            List<List<string>> source = new();

            List<string> pageList = new();

            foreach (KeyValuePair<string, QuestProgress> pair in Mod.instance.save.progress)
            {

                string id = pair.Key;

                QuestProgress progress = pair.Value;

                if (!quests.ContainsKey(id))
                {

                    continue;

                }

                int requirement = quests[id].type == Quest.questTypes.lesson ? 1 : 2;

                if (progress.status >= requirement)
                {

                    if (effects.ContainsKey(id))
                    {

                        for (int i = 0; i < effects[id].Count; i++)
                        {

                            pageList.Add(id + "|" + i.ToString());

                        }

                    }

                }

            }

            if (reverse)
            {

                pageList.Reverse();

            }

            foreach (string page in pageList)
            {

                if (source.Count == 0 || source.Last().Count() == 6)
                {
                    source.Add(new List<string>());
                }

                source.Last().Add(page);

            }

            return source;

        }



        // ----------------------------------------------------------------------

        public void Ready()
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            List<string> keys = new(Mod.instance.save.progress.Keys);

            foreach (string key in keys)
            {
                
                if (!quests.ContainsKey(key))
                {

                    FixQuests(key);

                }

            }

            keys = new(Mod.instance.save.progress.Keys);

            foreach(string key in keys)
            {

                if (Mod.instance.save.progress[key].delay > 0)
                {

                    Mod.instance.save.progress[key].delay -= 1;

                }

                if (!quests.ContainsKey(key))
                {

                    continue;

                }

                Quest quest = quests[key];

                if (Mod.instance.save.progress[key].status == 0 && Mod.instance.save.progress[key].delay <= 0)
                {

                    if (quest.give != Quest.questGivers.dialogue || Mod.instance.Config.autoProgress)
                    {

                        Mod.instance.save.progress[key].status = 1;

                        Initialise(key);

                    }

                    DialogueBefore(key);

                }
                else
                if (Mod.instance.save.progress[key].status == 1)
                {

                    Initialise(key);

                }
                else
                if (Mod.instance.save.progress[key].status == 2)
                {

                    if(quest.lore.Count > 0)
                    {

                        stories = quest.lore;

                    }

                    Implement(key);

                }

            }

        }

        public void Promote(milestones milestone)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            foreach(KeyValuePair<milestones,List<string>> mile in milestoneQuests)
            {
                
                if (mile.Key == milestone)
                {
                    foreach (string questId in mile.Value)
                    {

                        if (quests[questId].give == Quest.questGivers.dialogue)
                        {

                            Mod.instance.save.progress[questId] = new();
                        }
                        else
                        {

                            Mod.instance.save.progress[questId] = new(1);

                        }

                    }

                    break;

                }

                foreach (string questId in mile.Value)
                {

                    Mod.instance.save.progress[questId] = new(2);

                }

                Mod.instance.save.milestone = mile.Key;

            }

            Mod.instance.SyncMultiplayer();

        }

        public void AssignQuest(string questId, bool sync = true)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (!quests.ContainsKey(questId))
            {

                return;

            }

            if (quests[questId].give == Quest.questGivers.dialogue)
            {

                Mod.instance.save.progress[questId] = new();

                DialogueBefore(questId);

            }
            else
            {

                Mod.instance.save.progress[questId] = new(1);

                Initialise(questId);

                DialogueBefore(questId);

            }

            if (sync)
            {

                Mod.instance.SyncMultiplayer();

            }

        }

        public void CompleteQuest(string questId)
        {
            
            if(Context.IsMainPlayer)
            {

                Mod.instance.save.progress[questId].status = 2;

                OnComplete(questId);


                if (quests[questId].lore.Count > 0)
                {

                    stories = quests[questId].lore;

                }

                Implement(questId);

                DialogueAfter(questId);

            }

            Mod.instance.CastMessage(quests[questId].title + " quest complete", 1, true);

            if (quests[questId].reward > 0)
            {
                float adjustReward = 1.2f - ((float)Mod.instance.ModDifficulty() * 0.1f);

                Game1.player.Money += (int)((float)quests[questId].reward * adjustReward);

            }

        }

        public void DelayQuest(string questId)
        {

            if (Context.IsMainPlayer)
            {

                Mod.instance.save.progress[questId].status = 0;

                Mod.instance.save.progress[questId].delay = 1;

            }

        }

        public int UpdateTask(string quest, int update)
        {

            if (!Mod.instance.save.progress.ContainsKey(quest))
            {
                return -1;

            }

            if (Mod.instance.save.progress[quest].status != 1)
            {

                return -1;

            }

            int progress = Mod.instance.save.progress[quest].progress + update;

            int limit = quests[quest].requirement;

            Mod.instance.save.progress[quest].progress = Math.Min(progress,limit);

            if(progress >= limit)
            {

                CompleteQuest(quest);

                return progress;

            }

            int portion = (limit / 2);

            if (portion != 0)
            {

                if (progress % portion == 0)
                {

                    Mod.instance.CastMessage(quests[quest].title + " " + ((progress * 100) / limit).ToString() + " percent complete", 2, true);
                }

            }

            return progress;

        }

        public void TaskSet(string quest, int set)
        {
            
            if (Mod.instance.save.progress.ContainsKey(quest))
            {

                Mod.instance.save.progress[quest].progress = set;

            }

        }

        public bool IsComplete(string quest)
        {

            if (Mod.instance.save.progress.ContainsKey(quest))
            {

                return (Mod.instance.save.progress[quest].status >= 2);

            }

            return false;

        }

        public bool IsOpen(string quest)
        {

            if (Mod.instance.save.progress.ContainsKey(quest))
            {

                return (Mod.instance.save.progress[quest].status <= 1);

            }

            return false;

        }

        public bool IsGiven(string quest)
        {

            if (Mod.instance.save.progress.ContainsKey(quest))
            {

                return (Mod.instance.save.progress[quest].status >= 1);

            }

            return false;

        }

        // ----------------------------------------------------------------------

        public void Initialise(string questId)
        {

            Vector2 origin = Mod.instance.questHandle.quests[questId].origin;

            bool trigger = Mod.instance.questHandle.quests[questId].trigger;

            switch (questId)
            {
                
                case approachEffigy:

                    LocationData.DruidLocations(LocationData.druid_grove_name);

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Scene.ApproachEffigy().EventSetup(origin, questId, trigger);

                    }

                    return;

                case swordWeald:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Sword.SwordWeald().EventSetup(origin, questId, trigger);

                    }

                    return;

                case herbalism:

                    (Mod.instance.locations[LocationData.druid_grove_name] as Grove).HerbalTiles(true);

                    return;

                case wealdOne:

                    CheckAssignment(wealdTwo,1);

                    return;

                case wealdTwo:

                    CheckAssignment(wealdThree,1);

                    return;

                case wealdThree:

                    CheckAssignment(wealdFour,1);

                    return;

                case wealdFour:

                    CheckAssignment(wealdFive,1);

                    return;

                case wealdFive:

                    CheckAssignment(challengeWeald,1);

                    return;

                case challengeWeald:

                    new Event.Challenge.ChallengeWeald().EventSetup(origin, questId, trigger);

                    return;

                case swordMists:

                    LocationData.DruidLocations(LocationData.druid_atoll_name);

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Sword.SwordMists().EventSetup(origin, questId, trigger);
                    
                    }

                    return;

                case mistsOne:

                    CheckAssignment(mistsTwo,1);

                    return;

                case mistsTwo:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_pan.ToString());

                    CheckAssignment(mistsThree,1);

                    return;

                case mistsThree:

                    CheckAssignment(mistsFour,1);

                    return;

                case mistsFour:

                    CheckAssignment(questEffigy,1);

                    return;

                case questEffigy:

                    
                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Scene.QuestEffigy().EventSetup(origin, questId, trigger);

                    }

                    return;

                case challengeMists:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Challenge.ChallengeMists().EventSetup(origin, questId, trigger);

                    }

                    return;

                case relicWeald:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Relics.RelicWeald().EventSetup(origin, questId, trigger);

                    }

                    return;

                case relicMists:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Relics.RelicMists().EventSetup(origin, questId, trigger);

                    }

                    return;

                case swordStars:

                    LocationData.DruidLocations(LocationData.druid_chapel_name);

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Revenant, Character.Character.mode.home);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_lantern.ToString());

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Sword.SwordStars().EventSetup(origin, questId, trigger);

                    }

                    return;

                case starsOne:

                    CheckAssignment(starsTwo, 1);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_still.ToString());

                    return;

                case starsTwo:

                    CheckAssignment(challengeStars, 1);

                    CheckAssignment(challengeAtoll, 1);

                    CheckAssignment(challengeDragon, 1);

                    return;

                case challengeStars:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Challenge.ChallengeStars().EventSetup(origin, questId, trigger);

                    }

                    return;

                case challengeAtoll:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Challenge.ChallengeAtoll().EventSetup(origin, questId, trigger);

                    }

                    return;

                case challengeDragon:

                    LocationData.DruidLocations(LocationData.druid_vault_name);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_water.ToString());

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Challenge.ChallengeDragon().EventSetup(origin, questId, trigger);

                    }
                    return;

                case approachJester:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Scene.ApproachJester().EventSetup(origin, questId, trigger);

                    }

                    return;

                case swordFates:

                    LocationData.DruidLocations(LocationData.druid_court_name);

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Sword.SwordFates().EventSetup(origin, questId, trigger);

                    }

                    return;

                case fatesOne:

                    CheckAssignment(fatesTwo, 1);

                    return;

                case fatesTwo:

                    CheckAssignment(fatesThree, 1);

                    return;

                case fatesThree:

                    CheckAssignment(questJester, 1);

                    return;

                case questJester:

                    LocationData.DruidLocations(LocationData.druid_archaeum_name);

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Scene.QuestJester().EventSetup(origin, questId, trigger);

                    }

                    return;

                case fatesFour:

                    CheckAssignment(challengeFates, 1);

                    return;

                case challengeFates:

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Challenge.ChallengeFates().EventSetup(origin, questId, trigger);

                    }

                    return;

                case swordEther:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_ceremonial.ToString());

                    if (!Mod.instance.eventRegister.ContainsKey(questId))
                    {

                        new Event.Sword.SwordEther().EventSetup(origin, questId, trigger);

                    }

                    return;

                case etherOne:

                    CheckAssignment(etherTwo, 0);

                    return;

                case etherTwo:

                    CheckAssignment(etherThree, 0);

                    return;

                case etherThree:

                    CheckAssignment(etherFour, 0);

                    return;

            }

        }

        public void Implement(string questId)
        {

            switch (questId)
            {

                case approachEffigy:

                    LocationData.DruidLocations(LocationData.druid_grove_name);

                    Character.Character.mode effigyMode = 
                        Mod.instance.save.characters.ContainsKey(CharacterHandle.characters.Effigy) ?
                        Mod.instance.save.characters[CharacterHandle.characters.Effigy] : 
                        Character.Character.mode.home;

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Effigy, effigyMode);

                    CheckAssignment(swordWeald, 0);

                    Milecrossed(milestones.effigy);

                    return;

                case swordWeald:

                    (Mod.instance.locations[LocationData.druid_grove_name] as Grove).AddDialogueTiles();

                    Milecrossed(milestones.weald_weapon);

                    CheckAssignment(wealdOne, 0);

                    CheckAssignment(herbalism, 0);

                    return;

                case herbalism:

                    (Mod.instance.locations[LocationData.druid_grove_name] as Grove).HerbalTiles();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_mortar.ToString());

                    return;

                case wealdOne:

                    CheckAssignment(wealdTwo, 0);

                    return;

                case wealdTwo:

                    CheckAssignment(wealdThree, 0);

                    return;

                case wealdThree:

                    CheckAssignment(wealdFour, 0);

                    return;

                case wealdFour:

                    CheckAssignment(wealdFive, 0);

                    return;

                case wealdFive:

                    CheckAssignment(challengeWeald, 0);

                    if (!IsComplete(wealdOne)) { return; }
                    if (!IsComplete(wealdTwo)) { return; }
                    if (!IsComplete(wealdThree)) { return; }
                    if (!IsComplete(wealdFour)) { return; }

                    Milecrossed(milestones.weald_lessons);

                    return;

                case challengeWeald:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.runestones_spring.ToString());

                    Milecrossed(milestones.weald_challenge);

                    CheckAssignment(swordMists, 1);

                    return;

                case swordMists:

                    LocationData.DruidLocations(LocationData.druid_atoll_name);

                    (Mod.instance.locations[LocationData.druid_atoll_name] as Atoll).AddDialogueTiles();

                    Milecrossed(milestones.mists_weapon);

                    CheckAssignment(mistsOne, 0);

                    return;

                case mistsOne:

                    CheckAssignment(mistsTwo, 0);

                    return;

                case mistsTwo:

                    CheckAssignment(mistsThree, 0);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_pan.ToString());

                    return;

                case mistsThree:

                    CheckAssignment(mistsFour, 0);

                    return;

                case mistsFour:

                    CheckAssignment(questEffigy, 0);

                    if (!IsComplete(mistsOne)) { return; }
                    if (!IsComplete(mistsTwo)) { return; }
                    if (!IsComplete(mistsThree)) { return; }

                    Milecrossed(milestones.mists_lessons);

                    return;

                case questEffigy:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.effigy_crest.ToString());

                    Milecrossed(milestones.quest_effigy);

                    CheckAssignment(challengeMists, 1);

                    return;

                case challengeMists:

                    Milecrossed(milestones.mists_challenge);

                    CheckAssignment(swordStars, 1);

                    return;

                case swordStars:

                    Milecrossed(milestones.stars_weapon);

                    CheckAssignment(starsOne, 0);

                    CheckAssignment(starsTwo, 1);

                    LocationData.DruidLocations(LocationData.druid_chapel_name);

                    if (!Mod.instance.characters.ContainsKey(CharacterHandle.characters.Revenant))
                    {

                        CharacterHandle.CharacterLoad(CharacterHandle.characters.Revenant, Character.Character.mode.home);

                    }

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_lantern.ToString());

                    break;

                case starsOne:

                    CheckAssignment(starsTwo, 0);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_still.ToString());

                    return;

                case starsTwo:

                    CheckAssignment(challengeStars, 0);

                    if (!IsComplete(starsOne)) { return; }

                    Milecrossed(milestones.stars_lessons);

                    return;

                case challengeStars:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.runestones_moon.ToString());

                    CheckAssignment(challengeAtoll, 0);

                    CheckAssignment(challengeDragon, 0);

                    Milecrossed(milestones.stars_challenge);

                    return;

                case challengeAtoll:
                case challengeDragon:

                    if (questId == challengeAtoll)
                    {
                        Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.runestones_farm.ToString());
                    }

                    if (questId == challengeDragon)
                    {

                        Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_water.ToString());

                        Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.runestones_cat.ToString());
                    }

                    if (!IsComplete(challengeAtoll)) { return; }

                    if (!IsComplete(challengeDragon)) { return; }

                    Milecrossed(milestones.stars_threats);

                    CheckAssignment(approachJester, 1);

                    return;

                case approachJester:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.jester_dice.ToString());

                    Character.Character.mode jesterMode = 
                        Mod.instance.save.characters.ContainsKey(CharacterHandle.characters.Jester) ? 
                        Mod.instance.save.characters[CharacterHandle.characters.Jester] : 
                        Character.Character.mode.home;

                    CharacterHandle.CharacterLoad(CharacterHandle.characters.Jester, jesterMode);

                    CheckAssignment(swordFates, 0);

                    Milecrossed(milestones.jester);

                    break;

                case swordFates:

                    Milecrossed(milestones.fates_weapon);

                    CheckAssignment(fatesOne, 0);

                    CheckAssignment(fatesTwo, 1);

                    LocationData.DruidLocations(LocationData.druid_court_name);

                    break;

                case fatesOne:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_eye.ToString());

                    CheckAssignment(fatesTwo, 0);

                    return;

                case fatesTwo:

                    CheckAssignment(fatesThree, 0);

                    return;

                case fatesThree:

                    CheckAssignment(questJester, 0);

                    if (!IsComplete(fatesOne)) { return; }
                    if (!IsComplete(fatesTwo)) { return; }

                    Milecrossed(milestones.fates_lessons);

                    return;

                case questJester:

                    CheckAssignment(fatesFour, 1);

                    Milecrossed(milestones.quest_jester);

                    if (!Mod.instance.characters.ContainsKey(CharacterHandle.characters.Buffin))
                    {

                        CharacterHandle.CharacterLoad(CharacterHandle.characters.Buffin, Character.Character.mode.home);

                    }

                    return;

                case fatesFour:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_crucible.ToString());

                    CheckAssignment(challengeFates, 1);

                    Milecrossed(milestones.fates_enchant);

                    LocationData.DruidLocations(LocationData.druid_tomb_name);

                    return;

                case challengeFates:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.book_wyrven.ToString());

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.shadowtin_tome.ToString());

                    if (!Mod.instance.characters.ContainsKey(CharacterHandle.characters.Shadowtin))
                    {

                        Character.Character.mode shadowtinMode =
                            Mod.instance.save.characters.ContainsKey(CharacterHandle.characters.Shadowtin) ?
                            Mod.instance.save.characters[CharacterHandle.characters.Shadowtin] :
                            Character.Character.mode.home;

                        CharacterHandle.CharacterLoad(CharacterHandle.characters.Shadowtin, shadowtinMode);

                    }

                    CheckAssignment(swordEther,0);

                    Milecrossed(milestones.fates_challenge);

                    return;

                case swordEther:

                    CheckAssignment(etherOne, 0);

                    CheckAssignment(etherTwo, 0);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_ceremonial.ToString());

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.courtesan_pin.ToString());

                    Milecrossed(milestones.ether_weapon);

                    return;

                case etherOne:

                    CheckAssignment(etherTwo, 0);

                    return;

                case etherTwo:

                    CheckAssignment(etherThree, 0);

                    return;

                case etherThree:

                    CheckAssignment(etherFour, 0);

                    if (!IsComplete(etherOne)) { return; }
                    if (!IsComplete(etherTwo)) { return; }

                    Milecrossed(milestones.ether_lessons);

                    return;

            }

        }


        public void OnAccept(string questId)
        {

            switch (questId)
            {

                case mistsTwo:

                    ThrowHandle throwPan = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Effigy].Position, IconData.relics.herbalism_pan);

                    throwPan.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_pan.ToString());

                    Mod.instance.save.herbalism[HerbalData.herbals.melius_ligna] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.melius_impes] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.melius_celeri] = 3;

                    break;

                case swordStars:

                    ThrowHandle throwLantern = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Effigy].Position, IconData.relics.wayfinder_lantern);

                    throwLantern.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_lantern.ToString());

                    break;

                case starsOne:

                    ThrowHandle throwStill = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Revenant].Position, IconData.relics.herbalism_still);

                    throwStill.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_still.ToString());

                    Mod.instance.save.herbalism[HerbalData.herbals.satius_ligna] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.satius_impes] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.satius_celeri] = 3;

                    break;

                case fatesOne:

                    ThrowHandle throwEye = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Jester].Position, IconData.relics.wayfinder_eye);

                    throwEye.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_eye.ToString());

                    break;

                case fatesFour:

                    ThrowHandle throwCup = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Buffin].Position, IconData.relics.herbalism_crucible);

                    throwCup.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_crucible.ToString());

                    Mod.instance.save.herbalism[HerbalData.herbals.magnus_ligna] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.magnus_impes] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.magnus_celeri] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.faeth] = 5;

                    break;

                case swordEther:

                    ThrowHandle throwCeremonial = new(Game1.player, Mod.instance.characters[CharacterHandle.characters.Revenant].Position, IconData.relics.wayfinder_ceremonial);

                    throwCeremonial.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.wayfinder_ceremonial.ToString());

                    break;
            }

            return;

        }

        public void OnComplete(string questId)
        {

            switch (questId)
            {

                case swordWeald:

                    Mod.instance.save.rite = Rite.rites.weald;

                    break;

                case herbalism:

                    ThrowHandle throwRelic = new(Game1.player, Game1.player.Position + new Vector2(128, 64), IconData.relics.herbalism_mortar);

                    throwRelic.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.herbalism_mortar.ToString());

                    Mod.instance.save.herbalism[HerbalData.herbals.ligna] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.impes] = 3;

                    Mod.instance.save.herbalism[HerbalData.herbals.celeri] = 3;

                    break;

                case swordMists:

                    Mod.instance.save.rite = Rite.rites.mists;

                    break;

                case mistsTwo:

                    ModUtility.LearnRecipe();

                    break;

                case swordStars:

                    Mod.instance.save.rite = Rite.rites.stars;

                    break;

                case swordFates:

                    Mod.instance.save.rite = Rite.rites.fates;

                    break;

                case swordEther:

                    Mod.instance.save.rite = Rite.rites.ether;

                    break;

            }

            return;

        }

        public void Milecrossed(milestones milestone)
        {

            if(milestone > Mod.instance.save.milestone)
            {

                Mod.instance.save.milestone = milestone;

            }

        }

        public void CheckAssignment(string id, int delay)
        {

            if (!Mod.instance.save.progress.ContainsKey(id))
            {

                switch (delay)
                {
                    case 0:

                        AssignQuest(id);

                        break;

                    default:

                        Mod.instance.save.progress[id] = new(0, delay);

                        break;


                }

            }

        }


        // ----------------------------------------------------------------------
        
        public void DialogueBefore(string questId)
        {

            if (quests[questId].before.Count > 0)
            {

                foreach(KeyValuePair<CharacterHandle.characters,DialogueSpecial> special in quests[questId].before)
                {

                    if (!Mod.instance.characters.ContainsKey(special.Key))
                    {

                        return;

                    }

                    if (!Mod.instance.dialogue.ContainsKey(special.Key))
                    {

                        Mod.instance.dialogue[special.Key] = new(special.Key);

                    }

                    special.Value.questId = questId;

                    Mod.instance.dialogue[special.Key].AddSpecialDialogue(questId, special.Value);

                }

            }

        }

        public void DialogueAfter(string questId)
        {

            if (quests[questId].before.Count > 0)
            {

                foreach (KeyValuePair<CharacterHandle.characters, DialogueSpecial> special in quests[questId].before)
                {

                    if (Mod.instance.dialogue.ContainsKey(special.Key))
                    {

                        Mod.instance.dialogue[special.Key].RemoveSpecialDialogue(questId);

                    }

                }

            }

            if (quests[questId].after.Count > 0)
            {

                foreach (KeyValuePair<CharacterHandle.characters, DialogueSpecial> special in quests[questId].after)
                {

                    if (Mod.instance.dialogue.ContainsKey(special.Key))
                    {

                        special.Value.questId = questId;

                        Mod.instance.dialogue[special.Key].AddSpecialDialogue(questId, special.Value);

                    }

                }

            }

        }

        public void DialogueCheck(string questId, int context, CharacterHandle.characters characterType, int answer = 0)
        {

            if (Mod.instance.save.progress.ContainsKey(questId))
            {

                if(Mod.instance.save.progress[questId].status == 0 && context == 0)
                {

                    Mod.instance.save.progress[questId].status = 1;

                    Mod.instance.save.progress[questId].delay = 0;

                    OnAccept(questId);

                    Initialise(questId);

                    foreach (KeyValuePair<CharacterHandle.characters,Dialogue.Dialogue> dialogues in Mod.instance.dialogue)
                    {

                        dialogues.Value.RemoveSpecialDialogue(questId);

                    }

                }

            }

        }

    }

}
