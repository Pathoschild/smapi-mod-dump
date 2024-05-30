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
using StardewDruid.Compat;
using StardewDruid.Compat.v100;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewDruid.Event.Challenge;
using StardewDruid.Event.Scene;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
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
            fates_challenge,
            jester_heart,
            shadowtin,
            ether_weapon,
            ether_lessons,
            ether_challenge,
            shadowtin_heart,

        }

        public static Dictionary<milestones, List<string>> milestoneQuests = new()
        {

            [milestones.effigy] = new() { "approachEffigy", },
            [milestones.weald_weapon] = new() { "swordWeald", },
            [milestones.weald_lessons] = new() { wealdOne, wealdTwo, wealdThree, wealdFour, wealdFive, },
            [milestones.weald_challenge] = new() { "challengeWeald", },
            [milestones.mists_weapon] = new() { "swordMists" },
            [milestones.mists_lessons] = new() { mistsOne, mistsTwo, mistsThree, mistsFour, },
            [milestones.quest_effigy] = new() { "questEffigy", },
            [milestones.mists_challenge] = new() { "challengeMists", },

        };

        public const string startJourney = "startJourney";

        public const string wealdOne = "clearance";

        public const string wealdTwo = "wildbounty";

        public const string wealdThree = "wildgrowth";

        public const string wealdFour = "cultivate";

        public const string wealdFive = "rockfall";

        public const string mistsOne = "sunder";

        public const string mistsTwo = "artifice";

        public const string mistsThree = "fishing";

        public const string mistsFour = "smite";

        public const string questEffigy = "questEffigy";

        public QuestHandle()
        {

            quests = QuestData.QuestList();

            effects = EffectsData.EffectList();

        }

        // ----------------------------------------------------------------------

        public List<List<string>> OrganiseQuests(bool active = false, bool reverse = false)
        {

            List<List<string>> source = new();

            List<string> pageList = new();

            List<string> activeList = new();

            foreach (KeyValuePair<string, QuestProgress> pair in Mod.instance.save.progress)
            {

                string id = pair.Key;

                QuestProgress progress = pair.Value;

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

        // ----------------------------------------------------------------------

        public void Ready()
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            Implement(startJourney);

            foreach (KeyValuePair<string, QuestProgress> pair in Mod.instance.save.progress)
            {

                string id = pair.Key;

                QuestProgress progress = pair.Value;

                if(progress.delay > 0)
                {

                    progress.delay -= 4;

                }

                if (!quests.ContainsKey(pair.Key))
                {

                    continue;

                }

                Quest quest = quests[pair.Key];

                if (progress.status == 0 && progress.delay <= 0)
                {

                    if(quest.give != Quest.questGivers.dialogue || Mod.instance.Config.autoProgress)
                    {

                        progress.status = 1;

                    }

                    DialogueBefore(id,progress.status);

                }

                if(progress.status == 1)
                {

                    Initialise(id);

                }

                if(progress.status == 2)
                {

                    Implement(id);

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

                DialogueBefore(questId,0);

            }
            else
            {

                Mod.instance.save.progress[questId] = new(1);

                DialogueBefore(questId,1);

                Initialise(questId);

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

                DialogueAfter(questId);

                Implement(questId);

                OnComplete(questId);

            }

            Mod.instance.CastMessage(quests[questId].title + " quest complete", 1, true);

            if (quests[questId].reward > 0)
            {

                Game1.player.Money += (int)(quests[questId].reward * Mod.instance.Config.adjustRewards / 100);

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

        public bool Initialise(string questId)
        {

            if (!Mod.instance.questHandle.quests[questId].runevent)
            {
                return false;
            }

            if (Mod.instance.eventRegister.ContainsKey(questId))
            {

                return true;

            }

            Vector2 origin = Mod.instance.questHandle.quests[questId].origin;

            bool trigger = Mod.instance.questHandle.quests[questId].trigger;

            switch (questId)
            {
                
                case "approachEffigy":

                    new Event.Scene.ApproachEffigy().EventSetup(origin, questId, trigger);

                    return true;

                case "swordWeald":

                    new Event.Sword.SwordWeald().EventSetup(origin, questId, trigger);

                    return true;

                case "challengeWeald":

                    new Event.Challenge.ChallengeWeald().EventSetup(origin, questId, trigger);

                    return true;

                case "swordMists":

                    new Event.Sword.SwordMists().EventSetup(origin, questId, trigger);

                    return true;

                case questEffigy:

                    new Event.Scene.QuestEffigy().EventSetup(origin, questId, trigger);

                    return true;

                case "challengeMists":

                    new Event.Challenge.ChallengeMists().EventSetup(origin, questId, trigger);

                    return true;

                case "relicsMists":

                    new Event.Relics.RelicsMists().EventSetup(origin, questId, trigger);

                    return true;

            }

            return false;

        }

        public void Implement(string questId)
        {

            switch (questId)
            {

                case startJourney:

                    LocationData.DruidLocations(LocationData.druid_grove_name);

                    return;

                case "approachEffigy":

                    Character.Character.mode effigyMode = Mod.instance.save.characters.ContainsKey(CharacterData.characters.Effigy) ? Mod.instance.save.characters[CharacterData.characters.Effigy] : Character.Character.mode.home;

                    CharacterData.CharacterLoad(CharacterData.characters.Effigy, effigyMode);

                    return;

                case "swordWeald":

                    (Mod.instance.locations[LocationData.druid_grove_name] as Grove).AddDialogueTiles();

                    return;

                case "challengeWeald":

                    LocationData.DruidLocations(LocationData.druid_atoll_name);

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.minister_mitre.ToString());

                    return;

                case "swordMists":

                    (Mod.instance.locations[LocationData.druid_atoll_name] as Atoll).AddDialogueTiles();

                    return;

                case questEffigy:

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.effigy_crest.ToString());

                    return;

            }

        }

        public void OnAccept(string questId)
        {

            switch (questId)
            {

                case wealdTwo:

                    Mod.instance.save.progress[wealdThree] = new(0, 1);

                    return;

                case wealdThree:

                    Mod.instance.save.progress[wealdFour] = new(0, 1);

                    return;

                case wealdFour:

                    Mod.instance.save.progress[wealdFive] = new(0, 1);

                    return;

                case wealdFive:

                    Mod.instance.save.progress["wealdChallenge"] = new(0, 1);

                    return;

                case mistsTwo:

                    Mod.instance.save.progress[mistsThree] = new(0, 1);

                    return;

                case mistsThree:

                    Mod.instance.save.progress[mistsFour] = new(0, 1);

                    return;

                case mistsFour:

                    Mod.instance.save.progress[questEffigy] = new(0, 1);

                    return;


            }

        }

        public void OnComplete(string questId)
        {

            switch (questId)
            {

                case "approachEffigy":

                    AssignQuest("swordWeald");

                    Milecrossed(milestones.effigy);

                    return;

                case "swordWeald":

                    Milecrossed(milestones.weald_weapon);

                    AssignQuest(wealdOne);

                    Mod.instance.save.progress[wealdTwo] = new(0, 1);

                    Mod.instance.save.rite = Rite.rites.weald;

                    return;

                case wealdOne:
                case wealdTwo:
                case wealdThree:
                case wealdFour:
                case wealdFive:

                    if (!IsComplete(wealdOne)){ return; }
                    if (!IsComplete(wealdTwo)) { return; }
                    if (!IsComplete(wealdThree)) { return; }
                    if (!IsComplete(wealdFour)) { return; }
                    if (!IsComplete(wealdFive)) { return; }

                    Milecrossed(milestones.weald_lessons);

                    return;

                case "challengeWeald":

                    Milecrossed(milestones.weald_challenge);

                    AssignQuest("swordMists");

                    break;

                case "swordMists":

                    Milecrossed(milestones.mists_weapon);

                    AssignQuest(mistsOne);

                    Mod.instance.save.progress[mistsTwo] = new(0, 1);

                    Mod.instance.save.rite = Rite.rites.mists;

                    break;

                case mistsOne:
                case mistsTwo:
                case mistsThree:
                case mistsFour:

                    if(questId == mistsTwo)
                    {
                        ModUtility.LearnRecipe();
                    }

                    if (!IsComplete(mistsOne)) { return; }
                    if (!IsComplete(mistsTwo)) { return; }
                    if (!IsComplete(mistsThree)) { return; }
                    if (!IsComplete(mistsFour)) { return; }

                    Milecrossed(milestones.mists_lessons);

                    return;

                case "questEffigy":

                    Milecrossed(milestones.quest_effigy);

                    Mod.instance.save.progress["challengeMists"] = new(0, 1);

                    return;

                case "challengeMists":

                    Milecrossed(milestones.mists_challenge);

                    AssignQuest("swordStars");

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

        // ----------------------------------------------------------------------
        
        public void DialogueBefore(string questId, int context)
        {

            if (quests[questId].before.Count > 0)
            {

                foreach(KeyValuePair<CharacterData.characters,DialogueSpecial> special in quests[questId].before)
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

                    special.Value.questContext = context;

                    Mod.instance.dialogue[special.Key].AddSpecialDialogue(questId, special.Value);

                }

            }

        }

        public void DialogueAfter(string questId)
        {

            if (quests[questId].before.Count > 0)
            {

                foreach (KeyValuePair<CharacterData.characters, DialogueSpecial> special in quests[questId].before)
                {

                    if (Mod.instance.dialogue.ContainsKey(special.Key))
                    {

                        Mod.instance.dialogue[special.Key].RemoveSpecialDialogue(questId);

                    }

                }

            }

            if (quests[questId].after.Count > 0)
            {

                foreach (KeyValuePair<CharacterData.characters, DialogueSpecial> special in quests[questId].after)
                {

                    if (Mod.instance.dialogue.ContainsKey(special.Key))
                    {

                        special.Value.questId = questId;

                        special.Value.questContext = 2;

                        Mod.instance.dialogue[special.Key].AddSpecialDialogue(questId, special.Value);

                    }

                }

            }

        }

        public void DialogueCheck(string questId, int context, CharacterData.characters characterType, int answer = 0)
        {

            if(context != 0) {  return; }

            if (Mod.instance.save.progress.ContainsKey(questId))
            {

                if(Mod.instance.save.progress[questId].status == 0)
                {
                    
                    Mod.instance.save.progress[questId].status = 1;

                    Mod.instance.save.progress[questId].delay = 0;

                    OnAccept(questId);

                    Initialise(questId);

                }

            }

        }

    }

}
