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
using System.Reflection;
using System.Runtime.CompilerServices;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewValley.Menus.CharacterCustomization;


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
            mists_challenge,
            stars_weapon,
            stars_lessons,
            stars_challenge,
            stars_threats,
            effigy_heart,
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

        public Dictionary<milestones, List<string>> milestoneQuests = new()
        {

            [milestones.effigy] = new() { "approachEffigy", },
            [milestones.weald_weapon] = new() { "swordWeald", },
            [milestones.weald_lessons] = new() { "wealdOne", "wealdTwo", "wealdThree", "wealdFour", "wealdFive", },
            [milestones.weald_challenge] = new() { "challengeWeald", },

        };

        public static string clearLesson = "wealdOne";

        public static string bushLesson = "wealdTwo";

        public static string spawnLesson = "wealdThree";

        public static string cropLesson = "wealdFour";

        public static string rockLesson = "wealdFive";

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

                if (progress.status >= 1)
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
            
            foreach (KeyValuePair<string, QuestProgress> pair in Mod.instance.save.progress)
            {

                Quest quest = quests[pair.Key];

                string id = pair.Key;

                QuestProgress progress = pair.Value;

                if(progress.delay > 0)
                {

                    Mod.instance.save.progress[id].delay -= 1;

                    progress.delay--;

                }

                if (progress.status == 0 && progress.delay <= 0)
                {

                    if (quest.give == Quest.questGivers.dialogue && !Mod.instance.Config.autoProgress)
                    {

                        DialogueBefore(id);

                    }
                    else
                    {

                        Mod.instance.save.progress[id].status = 1;

                        progress.status = 1;

                    }

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

            foreach (string questId in milestoneQuests[milestone])
            {

                AssignQuest(questId,false);

            }

            Mod.instance.SyncMultiplayer();

        }

        public void AssignQuest(string questId, bool sync = true)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (quests[questId].delay > 0)
            {

                Mod.instance.save.progress[questId] = new(0, quests[questId].delay);

            }
            else if (quests[questId].give == Quest.questGivers.dialogue)
            {

                Mod.instance.save.progress[questId] = new();

                DialogueBefore(questId);

            }
            else
            {

                Mod.instance.save.progress[questId] = new(1);

                Initialise(questId);

                Mod.instance.CastMessage("Druid journal (" + Mod.instance.Config.journalButtons.ToString() + ") has been updated");

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

                Implement(questId);

                Graduate(questId);

                DialogueAfter(questId);

            }

            Mod.instance.CastMessage(quests[questId].title + " quest complete", 1);

            if (quests[questId].reward > 0)
            {

                Game1.player.Money += quests[questId].reward;

            }

        }

        public int UpdateTask(string quest, int update)
        {

            if (!Mod.instance.save.progress.ContainsKey(quest))
            {
                return -1;

            }

            if (Mod.instance.save.progress[quest].status > 1)
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

            int portion = (limit / 5);

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

                return (Mod.instance.save.progress[quest].status > 1);

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

            return Mod.instance.save.progress.ContainsKey(quest);

        }

        public void NewStart()
        {

            Promote((milestones)1);

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

                    new ApproachEffigy().EventSetup(origin, questId, trigger);

                    return true;

                case "swordWeald":

                    new Event.Sword.SwordWeald().EventSetup(origin, questId, trigger);

                    return true;

            }

            return false;

        }

        public void Implement(string questId)
        {

            switch (questId)
            {

                case "approachEffigy":

                    LocationData.DruidEdit();

                    CharacterData.CharacterLoad(CharacterData.characters.effigy, Character.Character.mode.home);

                    return;

                case "swordWeald":

                    Mod.instance.save.rite = Rite.rites.weald;

                    return;
            }

        }

        public bool Graduate(string questId)
        {

            switch (questId)
            {

                case "approachEffigy":

                    AssignQuest("swordWeald");

                    Mod.instance.save.milestone = milestones.effigy;

                    return true;

                case "swordWeald":

                    AssignQuest(clearLesson);

                    AssignQuest(bushLesson);

                    AssignQuest(spawnLesson);

                    AssignQuest(cropLesson);

                    AssignQuest(rockLesson);

                    return true;

            }

            return true;

        }

        // ----------------------------------------------------------------------
        
        public void DialogueBefore(string questId)
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

                        Mod.instance.dialogue[special.Key] = new(Mod.instance.characters[special.Key]);

                    }

                    special.Value.questId = questId;

                    special.Value.questContext = 0;

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

                foreach (KeyValuePair<CharacterData.characters, DialogueSpecial> special in quests[questId].before)
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

        public void DialogueCheck(string questId, int context, StardewDruid.Character.Character character, int answer = 0)
        {

            if(context != 0) {  return; }

            if (Mod.instance.save.progress.ContainsKey(questId))
            {

                if(Mod.instance.save.progress[questId].status == 0)
                {
                    
                    Mod.instance.save.progress[questId].status = 1;

                    Mod.instance.save.progress[questId].delay = 0;

                    Initialise(questId);

                    Mod.instance.CastMessage("Driud journal (" + Mod.instance.Config.journalButtons.ToString() + ") has been updated");

                }

            }

        }

    }

}
