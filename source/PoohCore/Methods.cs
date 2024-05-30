/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/poohnhi/PoohCore
**
*************************************************/

using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using StardewValley.Extensions;
using static System.Collections.Specialized.BitVector32;
using StardewValley.Delegates;
using StardewValley.TokenizableStrings;
using StardewValley.GameData.Characters;
using StardewValley.ItemTypeDefinitions;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using static StardewValley.GameStateQuery;
using StardewValley.Internal;
using StardewValley.Objects;

namespace PoohCore
{
    public partial class ModEntry
    {
        public static string GetMailFlagProgressNumberFromMailFlag(string mailFlag)
        {
            string currentNum = "0";
            foreach (string s in Game1.player.mailReceived)
            {
                if (s.Contains(mailFlag) && s.Length != mailFlag.Length)
                {
                    string getNumMailFlag = s.Replace(mailFlag, "");
                    int num = Int32.Parse(getNumMailFlag);
                    currentNum = num.ToString();
                    break;
                }
            }
            return currentNum;
        }
        public static string GetNextNumberFromString(string number)
        {
            int num = Int32.Parse(number);
            num += 1;
            return num.ToString();
        }
        internal class GetGiftTasteCPTokenFromSomeOneToken
        {
            private string NPCName = "";
            private string relativecheck = "false";
            public bool AllowsInput()
            {
                return true;
            }
            public bool RequiresInput()
            {
                return true;
            }
            public bool CanHaveMultipleValues(string input = null)
            {
                return false;
            }
            public bool UpdateContext()
            {
                string check = "";
                string oldCheck = this.NPCName;
                string relativecheck = "";
                string oldrelativeCheck = this.relativecheck;
                return (check != oldCheck || relativecheck != oldrelativeCheck);
            }
            public IEnumerable<string> GetValues(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    yield break;
                string[] tempInput = input.Split(' ');
                string NPCInternalName = tempInput[0];
                string giftTaste = tempInput[1];
                string relativeCheck = tempInput[2];
                string excludeId = tempInput[3];
                string[] excludeIdList;
                if (excludeId != "none")
                    excludeIdList = excludeId.Split('-');
                else
                    excludeIdList = "".Split(' ');
                this.relativecheck = relativeCheck;
                yield return GetGiftTasteCPTokenFromSomeOne(NPCInternalName, giftTaste, relativeCheck, excludeIdList);
            }
        }
        internal class GetMailFlagProgressNumberToken
        {
            private string MailFlag = "";
            public bool AllowsInput()
            {
                return true;
            }
            public bool RequiresInput()
            {
                return true;
            }
            public bool CanHaveMultipleValues(string input = null)
            {
                return false;
            }
            public bool UpdateContext()
            {
                string check = "";
                string oldCheck = this.MailFlag;
                return check != oldCheck;
            }
            public IEnumerable<string> GetValues(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    yield break;
                string[] tempInput = input.Split(' ');
                string thisMailFlag = tempInput[0];
                this.MailFlag = thisMailFlag;
                if (!ModEntry.ListOfMailFlag.Contains(thisMailFlag))
                {
                    ModEntry.ListOfMailFlag.Add(thisMailFlag);
                }
                if (GetNextNumberFromString(GetMailFlagProgressNumberFromMailFlag(thisMailFlag)) != null && GetNextNumberFromString(GetMailFlagProgressNumberFromMailFlag(thisMailFlag)) != "")
                    yield return GetNextNumberFromString(GetMailFlagProgressNumberFromMailFlag(thisMailFlag));
                else yield return "1";
            }
        }
        
        public static int ParseGiftTaste(string giftTasteString)
        {
            int tempNum = 9;
            switch (giftTasteString)
            {
                case "loved":
                case "love":
                    tempNum = 1;
                    break;
                case "liked":
                case "like":
                    tempNum = 3;
                    break;
                case "disliked":
                case "dislike":
                    tempNum = 5;
                    break;
                case "hated":
                case "hate":
                    tempNum = 7;
                    break;
            }
            return tempNum;
        }

        public static string GetIDGenerator(string internalNPC, string giftTaste, string[] excludeIdList, int randomSeed)
        {
            int randomCounter = 1;
            Random r = Utility.CreateDaySaveRandom(Game1.stats.DaysPlayed * 10 * randomCounter * randomSeed);

            // giftTaste is loved, liked, ... hated
            // internalNPC is the internal NPC name that it try to get gift taste
            NPC npc = Game1.getCharacterFromName(internalNPC);
            if (npc != null)
            {
                CharacterData npcData = npc.GetData();
                Dictionary<string, string> npcGiftTastes = DataLoader.NpcGiftTastes(Game1.content);
                if (npcGiftTastes.TryGetValue(internalNPC, out var rawGiftTasteData))
                {
                    string[] rawGiftTasteFields = rawGiftTasteData.Split('/');
                    string[] Items = ArgUtility.SplitBySpace(ArgUtility.Get(rawGiftTasteFields, ParseGiftTaste(giftTaste)));
                    string[] FinalItems = Items;
                    switch (ParseGiftTaste(giftTaste))
                    {
                        case 1:
                            FinalItems = Items.Union(ModEntry.universalLoves).ToArray();
                            break;
                        case 3:
                            FinalItems = Items.Union(ModEntry.universalLikes).ToArray();
                            break;
                        case 5:
                            FinalItems = Items.Union(ModEntry.universalDislikes).ToArray();
                            break;
                        case 7:
                            FinalItems = Items.Union(ModEntry.universalHates).ToArray();
                            break;
                        case 9:
                            FinalItems = Items.Union(ModEntry.universalNeutrals).ToArray();
                            break;
                    }
                    string item;
                    r = Utility.CreateDaySaveRandom(Game1.stats.DaysPlayed * 10 * randomCounter * randomSeed * FinalItems.Length);
                    item = r.Choose(FinalItems);
                    // if the item chosen is in excludeIdList, choose again
                    while (Array.IndexOf(excludeIdList, item) > -1)
                    {
                        randomCounter += 1;
                        r = Utility.CreateRandom(Game1.stats.DaysPlayed * 75 * randomCounter * randomSeed * FinalItems.Length);
                        item = r.Choose(Items);
                    }
                    return item;
                }
            }
            return "74";
        }
        public static string GetOneRandomGiftTasteItem(string internalNPC, string giftTaste, string[] excludeIdList, int initRandomSeed)
        {
            var ExcludeIDs = new HashSet<string>(excludeIdList);
            int randomSeed = initRandomSeed;
            int tryGetFiveRandom = 0;
            // giftTaste is loved, liked, ... hated
            // internalNPC is the internal NPC name that it try to get gift taste
            NPC npc = Game1.getCharacterFromName(internalNPC);
            if (npc != null)
            {
                string getIDStraightFromGiftTaste = GetIDGenerator(internalNPC, giftTaste, excludeIdList, randomSeed);
                //string query = "RANDOM_ITEMS (O)" + " " + GetIDGenerator + " " + GetIDGenerator;
                string query = "RANDOM_ITEMS (O)";
                Item getItemFromId = ItemRegistry.Create(getIDStraightFromGiftTaste);
                while (tryGetFiveRandom < 5)
                {
                    Item getOneRandomItem = ItemQueryResolver.TryResolveRandomItem(query, null, true, ExcludeIDs);
                    tryGetFiveRandom++;
                    randomSeed++;
                    if(npc.getGiftTasteForThisItem(getOneRandomItem) == ParseGiftTaste(giftTaste)-1)
                        return getOneRandomItem.QualifiedItemId;
                }
                while (true)
                {
                    if (npc.getGiftTasteForThisItem(getItemFromId) == ParseGiftTaste(giftTaste) - 1)
                        return getItemFromId.QualifiedItemId;
                    getIDStraightFromGiftTaste = GetIDGenerator(internalNPC, giftTaste, excludeIdList, randomSeed);
                    getItemFromId = ItemRegistry.Create(getIDStraightFromGiftTaste);
                    randomSeed++;
                }
            }
            return "74";
        }

        public static string GetGiftTasteCPTokenFromSomeOne(string NPCInternalName, string giftTaste, string relativeCheck, string[] excludeIdList)
        {
            string replacement = "";
            replacement += " |originalCharacterName=" + NPCInternalName;
            try
            {
                NPC npc = Game1.getCharacterFromName(NPCInternalName);
                if (relativeCheck == "true" || relativeCheck == "True")
                {
                    string relativeName = "";
                    string relativeTitle = "";
                    string relativeDisplayName = "";
                    if (npc != null)
                    {
                        CharacterData npcData = npc.GetData();
                        while (npcData?.FriendsAndFamily != null && Utility.TryGetRandom(npcData.FriendsAndFamily, out relativeName, out relativeTitle))
                        {
                            NPC relativenpc = Game1.getCharacterFromName(relativeName);
                            if (relativenpc != null)
                            {
                                relativeDisplayName = relativenpc.displayName;
                                break;
                            }
                        }
                        
                        replacement += " |relativeCharacterName=" + relativeName;
                        try
                        {
                            NPC relativeNpc = Game1.getCharacterFromName(relativeName);
                            switch (relativeNpc.Gender)
                            {
                                case Gender.Female:
                                    if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName))
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName + "]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + relativeName + "]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + relativeName + "]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + relativeName + "]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + relativeName + "]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + relativeName + "]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + relativeName + "]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + relativeName + "]";
                                    }
                                    else
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineSubject]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineObject]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FemininePossessive]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineReflexive]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineSubjectLowercase]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineObjectLowercase]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FemininePossessiveLowercase]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineReflexiveLowercase]";
                                    }
                                    break;
                                case Gender.Male:
                                    if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName))
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName + "]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + relativeName + "]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + relativeName + "]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + relativeName + "]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + relativeName + "]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + relativeName + "]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + relativeName + "]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + relativeName + "]";
                                    }
                                    else
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineSubject]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineObject]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculinePossessive]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineReflexive]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineSubjectLowercase]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineObjectLowercase]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculinePossessiveLowercase]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineReflexiveLowercase]";
                                    }
                                    break;
                                default:
                                    if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName))
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + relativeName + "]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + relativeName + "]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + relativeName + "]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + relativeName + "]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + relativeName + "]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + relativeName + "]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + relativeName + "]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + relativeName + "]";
                                    }
                                    else
                                    {
                                        replacement += " |relativeSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralSubject]";
                                        replacement += " |relativeObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralObject]";
                                        replacement += " |relativePossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralPossessive]";
                                        replacement += " |relativeReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralReflexive]";
                                        replacement += " |relativeSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralSubjectLowercase]";
                                        replacement += " |relativeObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralObjectLowercase]";
                                        replacement += " |relativePossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralPossessiveLowercase]";
                                        replacement += " |relativeReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralReflexiveLowercase]";
                                    }
                                    break;
                            }
                        }
                        catch { }
                        if (relativeTitle != "" && relativeTitle != relativeDisplayName && relativeTitle != relativeName)
                        {
                            replacement += " |relativeHasSpecialTitle=" + "true";
                            replacement += " |relativeCharacterTitle=" + TokenParser.ParseText(relativeTitle);
                        }
                        else
                        {
                            replacement += " |relativeHasSpecialTitle=" + "false";
                            replacement += " |relativeCharacterTitle=" + relativeDisplayName;
                        }
                    }
                    string itemId = GetOneRandomGiftTasteItem(relativeName, giftTaste, excludeIdList, (88 + relativeName.Length + NPCInternalName.Length));
                    replacement += " |GiftTasteItemId=" + itemId;
                }
            }
            catch { }
            if (relativeCheck != "true" && relativeCheck != "True")
            {
                string itemId = GetOneRandomGiftTasteItem(NPCInternalName, giftTaste, excludeIdList, (11 + NPCInternalName.Length + relativeCheck.Length));
                try
                {
                    NPC npc = Game1.getCharacterFromName(NPCInternalName);
                    switch (npc.Gender)
                    {
                        case Gender.Female:
                            if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName))
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName + "]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + NPCInternalName + "]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + NPCInternalName + "]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + NPCInternalName + "]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + NPCInternalName + "]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + NPCInternalName + "]";
                            }
                            else
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineSubject]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineObject]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FemininePossessive]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineReflexive]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineSubjectLowercase]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineObjectLowercase]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FemininePossessiveLowercase]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.FeminineReflexiveLowercase]";
                            }
                            break;
                        case Gender.Male:
                            if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName))
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName + "]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + NPCInternalName + "]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + NPCInternalName + "]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + NPCInternalName + "]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + NPCInternalName + "]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + NPCInternalName + "]";
                            }
                            else
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineSubject]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineObject]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculinePossessive]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineReflexive]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineSubjectLowercase]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineObjectLowercase]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculinePossessiveLowercase]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.MasculineReflexiveLowercase]";
                            }
                            break;
                        default:
                            if (Game1.content.LoadString("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName) != ("Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName))
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Subject_" + NPCInternalName + "]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Object_" + NPCInternalName + "]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Possessive_" + NPCInternalName + "]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.Reflexive_" + NPCInternalName + "]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.SubjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ObjectLowercase_" + NPCInternalName + "]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.PossessiveLowercase_" + NPCInternalName + "]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.ReflexiveLowercase_" + NPCInternalName + "]";
                            }
                            else
                            {
                                replacement += " |originalSubject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralSubject]";
                                replacement += " |originalObject=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralObject]";
                                replacement += " |originalPossessive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralPossessive]";
                                replacement += " |originalReflexive=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralReflexive]";
                                replacement += " |originalSubjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralSubjectLowercase]";
                                replacement += " |originalObjectLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralObjectLowercase]";
                                replacement += " |originalPossessiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralPossessiveLowercase]";
                                replacement += " |originalReflexiveLowercase=" + "[LocalizedText Strings\\StringsFromCSFiles:poohnhi.MoreGiftTasteRevealDialogue.NeutralReflexiveLowercase]";
                            }
                            break;
                    }
                }
                catch { }
                replacement += " |GiftTasteItemId=" + itemId;
            }
            return replacement;
        }
    }
}
    