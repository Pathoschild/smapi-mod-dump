/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using static DynamicDialogues.Parser;
using static DynamicDialogues.Getter;
using System.Linq;
using System.Reflection;

namespace DynamicDialogues
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //get status and information
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStart;

            helper.Events.GameLoop.ReturnedToTitle += OnTitleReturn;

            //set file type, npc dialogues, etc
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.TimeChanged += Setter.OnTimeChange;
            helper.Events.Content.AssetRequested += Setter.OnAssetRequest;
            helper.Events.GameLoop.DayEnding += Setter.OnDayEnded;

            ModEntry.Config = this.Helper.ReadConfig<ModConfig>();
            Mon = this.Monitor;

            helper.ConsoleCommands.Add("ddprint", "Prints dialogue type", Debug.Print);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            this.Monitor.Log($"Applying Harmony patch \"{nameof(ModPatches)}\": prefixing SDV method \"NPC.sayHiTo(Character)\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), nameof(StardewValley.NPC.sayHiTo)),
                prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.SayHiTo_Prefix))
                );

            this.Monitor.Log($"Applying Harmony patch \"{nameof(ModPatches)}\": postfixing SDV method \"Dialogue.exitCurrentDialogue\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Dialogue), nameof(StardewValley.Dialogue.exitCurrentDialogue)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.PostCurrentDialogue))
                );

        }

        internal void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GetYesNo();

            //based off atravita's SpecialOrdersExtended ↓

            spaceCoreAPI = this.Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");

            if (spaceCoreAPI != null)
            {
                MethodInfo adder = typeof(EventScene).GetMethod(nameof(EventScene.Add)); //old: StaticMethodNamed
                MethodInfo remover = typeof(EventScene).GetMethod(nameof(EventScene.Remove));

                spaceCoreAPI.AddEventCommand(AddScene, adder);
                spaceCoreAPI.AddEventCommand(RemoveScene, remover);
            }
            else
            {
                this.Monitor.Log("SpaceCore not detected, adding event command manually.", LogLevel.Info);

                var harmony = new Harmony(this.ModManifest.UniqueID);
                this.Monitor.Log($"Applying Harmony patch \"{nameof(ModPatches)}\": prefixing SDV method \"Event.tryEventCommand(GameLocation location, GameTime time, string[] split)\".");

                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.tryEventCommand)),
                    prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.PrefixTryGetCommand))
                    );
            }

        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var allNPCs = this.Helper.GameContent.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            NPCDispositions = allNPCs.Keys.ToList();

            // For each string: Check if npc has been met, to not cause errors with locked/unmet NPCs.
            GetFriendedNPCs();
            this.Monitor.Log($"Found {NPCDispositions?.Count ?? 0} characters in NPC dispositions.");
            this.Monitor.Log($"Found {PatchableNPCs?.Count ?? 0} characters in friendship data.");
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            //clear temp data
            ClearTemp();
            GetFriendedNPCs();

            //get dialogue for NPCs
            foreach (var name in PatchableNPCs)
            {
                if (!Exists(name)) //if NPC doesnt exist in savedata
                {
                    this.Monitor.Log($"{name} data won't be added. Check log for more details.", LogLevel.Warn);
                    continue;
                }
                if(Config.Verbose)
                {
                    this.Monitor.Log($"Checking patch data for NPC {name}...");
                }
                var CompatRaw = Game1.content.Load<Dictionary<string, RawDialogues>>($"mistyspring.dynamicdialogues/Dialogues/{name}");
                GetNPCDialogues(CompatRaw, name);

                //get questions
                var QRaw = Game1.content.Load<Dictionary<string, RawQuestions>>($"mistyspring.dynamicdialogues/Questions/{name}");
                GetQuestions(QRaw, name);


                //get missions
                var missionRaw = Game1.content.Load<Dictionary<string, RawMission>>($"mistyspring.dynamicdialogues/Quests/{name}");
                GetMissions(missionRaw, name);
            }
            var dc = Dialogues?.Count ?? 0;
            this.Monitor.Log($"Loaded {dc} user patches. (Dialogues)", LogLevel.Debug);

            var qc = Questions?.Count ?? 0;
            this.Monitor.Log($"Loaded {qc} user patches. (Questions)", LogLevel.Debug);

            //get greetings
            var greetRaw = Game1.content.Load<Dictionary<string, Dictionary<string, string>>>("mistyspring.dynamicdialogues/Greetings");
            GetGreetings(greetRaw);
            var gc = Greetings?.Count ?? 0;
            this.Monitor.Log($"Loaded {gc} user patches. (Greetings)", LogLevel.Debug);
            
            //get notifs
            var notifRaw = Game1.content.Load<Dictionary<string, RawNotifs>>("mistyspring.dynamicdialogues/Notifs");
            GetNotifs(notifRaw);
            var nc = Notifs?.Count ?? 0;
            this.Monitor.Log($"Loaded {nc} user patches. (Notifs)", LogLevel.Debug);

            //get random dialogue
            GetDialoguePool();
            var rr = RandomPool?.Count ?? 0;
            this.Monitor.Log($"Loaded {rr} user patches. (Dialogue pool)", LogLevel.Debug);

            //missions
            var m = MissionData?.Count ?? 0;
            this.Monitor.Log($"Loaded {m} user patches. (Mission/Quests)", LogLevel.Debug);

            this.Monitor.Log($"{dc + gc + nc + qc + rr + m} total user patches loaded.",LogLevel.Debug);
        }

        private void OnTitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearTemp();
            NPCDispositions?.Clear();
        }


        /* Methods used to get dialogues 
         * do NOT change unless bug-fixing is required
         */
        private static void GetYesNo()
        {
            Yes = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes");
            No = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
        }
        private void GetNPCDialogues(Dictionary<string, RawDialogues> raw, string nameof)
        {
            foreach (var singular in raw)
            {
                var dialogueInfo = singular.Value;
                if (dialogueInfo is null)
                {
                    this.Monitor.Log($"The dialogue data for {nameof} is empty!", LogLevel.Warn);
                }
                else if (IsValid(dialogueInfo, nameof))
                {
                    this.Monitor.Log($"Dialogue key \"{singular.Key}\" ({nameof}) parsed successfully. Adding to dictionary");
                    var data = dialogueInfo;

                    if ((bool)(Dialogues?.ContainsKey(nameof)))
                    {
                        Dialogues[nameof].Add(data);
                    }
                    else
                    {
                        var list = new List<RawDialogues>();
                        list.Add(data);
                        Dialogues.Add(nameof, list);
                    }
                }
                else
                {
                    this.Monitor.Log($"Patch '{singular.Key}' won't be added.", LogLevel.Warn);
                }
            }
        }
        private void GetGreetings(Dictionary<string, Dictionary<string, string>> greetRaw)
        {
            foreach (var edit in greetRaw)
            {
                NPC mainCh = Game1.getCharacterFromName(edit.Key);
                if (!Exists(mainCh))
                {
                    continue;
                }

                this.Monitor.Log($"Loading greetings for {edit.Key}...");
                Dictionary<NPC, string> ValueOf = new();

                foreach (var npcgreet in edit.Value)
                {
                    this.Monitor.Log($"Checking greet data for {npcgreet.Key}...");
                    var chara = Game1.getCharacterFromName(npcgreet.Key);

                    if (IsValidGreeting(chara, npcgreet.Value))
                    {
                        Greetings.Add((edit.Key, npcgreet.Key), npcgreet.Value);
                        this.Monitor.Log("Greeting added.");
                    }
                }
            }
        }
        private void GetNotifs(Dictionary<string, RawNotifs> notifRaw)
        {
            foreach (var pair in notifRaw)
            {
                var notif = pair.Value;
                if (IsValidNotif(notif))
                {
                    ModEntry.Mon.Log($"Notification \"{pair.Key}\" parsed successfully.");
                    Notifs.Add(notif);
                }
                else
                {
                    ModEntry.Mon.Log($"Found error in \"{pair.Key}\" while parsing, check Log for details.", LogLevel.Error);
                }
            }
        }
        private void GetQuestions(Dictionary<string, RawQuestions> QRaw, string nameof)
        {
            foreach (var extra in QRaw)
            {
                var title = extra.Key;
                var QnA = extra.Value;
                if(IsValidQuestion(QnA) && !String.IsNullOrWhiteSpace(title))
                {
                    if((bool)(Questions?.ContainsKey(nameof)))
                    {
                        Questions[nameof].Add(QnA);
                    }
                    else
                    {
                        var dict = new List<RawQuestions>();
                        dict.Add(QnA);
                        Questions.Add(nameof, dict);
                    }
                }
                else
                {
                    var pos = GetIndex(QRaw, extra.Key);
                    this.Monitor.Log($"Entry {pos} for {extra.Key} is faulty! It won't be added.", LogLevel.Warn);
                }
            }
        }
        private void GetDialoguePool()
        {
            foreach(var name in NPCDispositions)
            {
                if(Game1.player.friendshipData.ContainsKey(name))
                {
                    if(Config.Verbose)
                    {
                        this.Monitor.Log($"Character {name} found.");
                    }

                    var dialogues = Helper.GameContent.Load<Dictionary<string, string>>($"Characters/Dialogue/{name}");

                    if (dialogues == null || dialogues.Count == 0)
                        continue;

                    List<string> texts = new();

                    foreach (var pair in dialogues)
                    {
                        if(pair.Key.StartsWith("Random"))
                        {
                            texts.Add(pair.Value);
                        }
                    }

                    //dont add npcs with no dialogue
                    if (texts == null || texts.Count == 0)
                    {
                        continue;
                    }

                    RandomPool.Add(name, texts);
                }
                else
                {
                    if (name == "Marlon")
                        continue; //we dont warn bc hes not interactable

                    this.Monitor.Log($"Character {name} hasn't been met yet. Their random dialogue will not be added.", LogLevel.Info);
                }
            }
        }
        private void GetMissions(Dictionary<string, RawMission> data, string who)
        {
            this.Monitor.Log($"Checking {who} quests...");
            foreach(var group in data)
            {
                var mission = group.Value;

                //if any of the values aren't valid, give error and continue
                if (string.IsNullOrWhiteSpace(mission.Dialogue))
                {
                    this.Monitor.Log($"Quest dialogue for {group.Key} is empty! It won't be added.",LogLevel.Error);
                    continue;
                }
                if (mission.From < 600 || mission.To > 2600 || mission.From > mission.To)
                {
                    this.Monitor.Log($"Time in quest '{group.Key}' has a faulty hour! Make sure it's between 600 and 2600", LogLevel.Error);
                    continue;
                }
                if (mission.Location is not "any")
                {
                    if (Game1.getLocationFromName(mission.Location) == null)
                    {
                        this.Monitor.Log($"Location for quest '{group.Key}' could not be found. Mission won't be added.", LogLevel.Error);
                        continue;
                    }
                }
                if (StardewValley.Quests.Quest.getQuestFromId(mission.ID) == null)
                {
                    this.Monitor.Log($"ID for '{group.Key}' doesn't exist!", LogLevel.Error);
                    continue;
                }

                var ParsedMission = mission;

                //if using the default answers and not playing in english
                if(mission.AcceptQuest.Equals("Yes") && mission.RejectQuest.Equals("No") && LocalizedContentManager.CurrentLanguageCode is not LocalizedContentManager.LanguageCode.en)
                {
                    try
                    {
                        //get the yes/no for native language and use them here
                        var yes = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes");
                        var no = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");

                        ParsedMission.AcceptQuest = yes;
                        ParsedMission.RejectQuest = no;

                    }
                    catch(Exception ex)
                    {
                        this.Monitor.Log($"Error: {ex}",LogLevel.Error);
                    }
                }    
                
                //if a list for npc already exists, just add value
                if(MissionData.ContainsKey(who))
                {
                    MissionData[who].Add(ParsedMission);
                }
                //else, create it
                else
                {
                    var list = new List<RawMission>();
                    list.Add(ParsedMission);
                    MissionData.Add(who, list);
                }
            }
        }

        private void GetFriendedNPCs()
        {
            /* we could use: 
             * PatchableNPCs = Game1.player.friendshipData.Keys;
             * However using netfields might cause more bugs. so we get it manually.
             */

            foreach (var name in NPCDispositions)
            {
                if(Config.Debug)
                {
                    this.Monitor.Log($"Checking {name}...");
                }

                NPC npc = Game1.getCharacterFromName(name);
                if (npc is not null)
                {
                    PatchableNPCs.Add(name);
                }
                else if (Config.Verbose)
                {
                    this.Monitor.Log($"NPC {name} doesn't exist in save yet.");
                }
            }
        }
        private static void ClearTemp()
        {
            AlreadyPatched?.Clear();
            Dialogues?.Clear();
            Greetings?.Clear();
            Notifs?.Clear();
            Questions?.Clear();
            PatchableNPCs?.Clear();
            QuestionCounter?.Clear();
            RandomPool?.Clear();
            CurrentQuests?.Clear();
        }

        /* Required by mod to work */
        internal static Dictionary<string, List<RawQuestions>> Questions { get; private set; } = new();
        internal static Dictionary<string, List<RawDialogues>> Dialogues { get; private set; } = new();
        internal static Dictionary<(string, string), string> Greetings { get; private set; } = new();
        internal static List<RawNotifs> Notifs { get; private set; } = new();
        internal static Dictionary<string, List<string>> RandomPool { get; private set; } = new();

        internal static Dictionary<string, int> QuestionCounter { get; set; } = new();
        internal static Dictionary<string, List<RawMission>> MissionData { get; set; } = new();

        internal static List<string> CurrentQuests { get; set; } = new();

        internal static List<string> PatchableNPCs { get; private set; } = new();
        internal static List<string> NPCDispositions { get; private set; } = new();

        //changes int to string (due to adding dialogues' from-to)
        internal static List<(string, string, string)> AlreadyPatched { get; set; } = new();
        internal static IMonitor Mon { get; private set; }

        internal static ModConfig Config;
        private ISpaceCoreAPI? spaceCoreAPI;
        internal const string AddScene = "AddScene";
        internal const string RemoveScene = "RemoveScene";

        //their default value is in eng, we get localized on SaveLoaded
        internal static string Yes { get; set; } = "Yes";
        internal static string No { get; set; } = "No";
    }
}