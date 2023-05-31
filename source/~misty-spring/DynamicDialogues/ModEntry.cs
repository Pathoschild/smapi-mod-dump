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
using System.Linq;
using static DynamicDialogues.Framework.Parser;
using static DynamicDialogues.Framework.Getter;
using DynamicDialogues.Framework;
using System.Reflection;
using DynamicDialogues.Patches;

// ReSharper disable All

namespace DynamicDialogues
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //get status and information
            //helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.Content.LocaleChanged += GetYesNo;

            helper.Events.GameLoop.ReturnedToTitle += OnTitleReturn;

            //set file type, npc dialogues, etc
            helper.Events.GameLoop.TimeChanged += Setter.OnTimeChange;
            helper.Events.Content.AssetRequested += Setter.OnAssetRequest;
            helper.Events.Content.AssetsInvalidated += Setter.ReloadAssets;

            ModEntry.Config = this.Helper.ReadConfig<ModConfig>();
            Mon = this.Monitor;
            Help = this.Helper;

            helper.ConsoleCommands.Add("ddprint", "Prints dialogue type", Debug.Print);
            helper.ConsoleCommands.Add("sayHiTo", "Test sayHiTo command", Debug.SayHiTo);
            helper.ConsoleCommands.Add("getQs", "Get NPC questions", Debug.GetQuestionsFor);
            
            var harmony = new Harmony(this.ModManifest.UniqueID);
            DialoguePatches.Apply(harmony);
            NPCPatches.Apply(harmony);
            EventPatches.Apply(harmony);
        }
/*
        internal void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            spaceCoreAPI = this.Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");

            if (spaceCoreAPI != null)
            {
                MethodInfo adder = typeof(EventScene).GetMethod(nameof(EventScene.Add)); //old: StaticMethodNamed
                MethodInfo remover = typeof(EventScene).GetMethod(nameof(EventScene.Remove));
                MethodInfo hunt = typeof(EventScene).GetMethod(nameof(Finder.ObjectHunt));

                spaceCoreAPI.AddEventCommand(AddScene, adder);
                spaceCoreAPI.AddEventCommand(RemoveScene, remover);
                spaceCoreAPI.AddEventCommand(PlayerFind, hunt);
            }
            else
            {
                this.Monitor.Log("SpaceCore not detected, adding event command manually.", LogLevel.Info);

                var harmony = new Harmony(this.ModManifest.UniqueID);
                this.Monitor.Log($"Applying Harmony patch \"{nameof(Patches.EventPatches)}\": prefixing SDV method \"Event.tryEventCommand(GameLocation location, GameTime time, string[] args)\".");

                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.tryEventCommand)),
                    prefix: new HarmonyMethod(typeof(Patches.EventPatches), nameof(Patches.EventPatches.PrefixTryGetCommandH))
                    );
            }

        }
*/
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            GetInteractableNPCs();

            this.Monitor.Log($"Found {PatchableNPCs?.Count ?? 0} patchable characters.");
            this.Monitor.Log($"Found {Game1.player.friendshipData?.Length ?? 0} characters in friendship data.");

            GetFilesFirstTime();
        }

        private void GetFilesFirstTime()
        {
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

                HasCustomDialogue(name);
            }

            RemoveAnyEmpty();
            
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

            this.Monitor.Log($"{dc + gc + nc + qc + rr} total user patches loaded.",LogLevel.Debug);
        }

        private void OnTitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearTemp();
            PatchableNPCs?.Clear();
        }

        #region methods to get data
        // do NOT change unless fixes are required
        private static void GetYesNo(object sender, LocaleChangedEventArgs e)
        {
            Yes = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes");
            No = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
        }

        internal static void GetNPCDialogues(Dictionary<string, RawDialogues> raw, string nameof)
        {
            foreach (var singular in raw)
            {
                var dialogueInfo = singular.Value;
                if (dialogueInfo is null)
                {
                    ModEntry.Mon.Log($"The dialogue data for {nameof} is empty!", LogLevel.Warn);
                }
                else if (IsValid(dialogueInfo, nameof))
                {
                    ModEntry.Mon.Log($"Dialogue key \"{singular.Key}\" ({nameof}) parsed successfully. Adding to dictionary");
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
                    ModEntry.Mon.Log($"Patch '{singular.Key}' won't be added.", LogLevel.Warn);
                }
            }
        }

        internal static void GetGreetings(Dictionary<string, Dictionary<string, string>> greetRaw)
        {
            foreach (var edit in greetRaw)
            {
                NPC mainCh = Game1.getCharacterFromName(edit.Key);
                if (!Exists(mainCh))
                {
                    continue;
                }

                ModEntry.Mon.Log($"Loading greetings for {edit.Key}...");
                Dictionary<NPC, string> ValueOf = new();

                foreach (var npcgreet in edit.Value)
                {
                    ModEntry.Mon.Log($"Checking greet data for {npcgreet.Key}...");
                    var chara = Game1.getCharacterFromName(npcgreet.Key);

                    if (IsValidGreeting(chara, npcgreet.Value))
                    {
                        Greetings.Add((edit.Key, npcgreet.Key), npcgreet.Value);
                        ModEntry.Mon.Log("Greeting added.");
                    }
                }
            }
        }

        internal static void GetNotifs(Dictionary<string, RawNotifs> notifRaw)
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

        internal static void GetQuestions(Dictionary<string, RawQuestions> QRaw, string nameof)
        {
            Questions.Remove(nameof);
            
            var dict = new List<RawQuestions>();
            Questions.Add(nameof, dict);
            
            foreach (var extra in QRaw)
            {
                if (ModEntry.Config.Debug)
                    ModEntry.Mon.Log($"checking question data: {nameof}, answer: {extra.Value.Answer}",LogLevel.Debug);

                var title = extra.Key;
                var QnA = extra.Value;
                if(IsValidQuestion(QnA) && !String.IsNullOrWhiteSpace(title))
                {
                    Questions[nameof].Add(QnA);
                }
                else
                {
                    var pos = GetIndex(QRaw, extra.Key);
                    ModEntry.Mon.Log($"Entry {pos} for {extra.Key} is faulty! It won't be added.", LogLevel.Warn);
                }
            }
        }
        private void GetDialoguePool()
        {
            foreach(var name in PatchableNPCs)
            {
                if(Game1.player.friendshipData.ContainsKey(name))
                {
                    if(Config.Verbose)
                    {
                        this.Monitor.Log($"Character {name} found.");
                    }

                    var dialogues = ModEntry.Help.GameContent.Load<Dictionary<string, string>>($"Characters/Dialogue/{name}");

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

                    this.Monitor.Log($"Character {name} hasn't been met yet. Any of their custom dialogue won't be added.", LogLevel.Debug);
                }
            }
        }

        internal static void GetInteractableNPCs()
        {
            if(ModEntry.Config.Debug)
                ModEntry.Mon.Log("GOT CALLED (interactableNPCs)",LogLevel.Warn);
            
            PatchableNPCs = new();
            
            foreach (var charData in Game1.characterData)
            {
                //var data = charData.Value;
                var name = charData.Key;
                
                //if they don't have a dialogues file, don't include them
                try
                {
                    Game1.content.Load<Dictionary<string, string>>("Characters/Dialogue/" + name);
                }
                catch (Exception)
                {
                    continue;
                }

                PatchableNPCs.Add(name);
            }
        }
        private static void HasCustomDialogue(string name)
        {
            var temp = new List<string>();
            var lines = Game1.content.Load<Dictionary<string, string>>($"Characters/Dialogue/{name}");
            
            foreach (var pair in lines)
            {
                if (pair.Value.StartsWith("Gift."))
                {
                    temp.Add(pair.Value);
                }
            }
            
            HasCustomGifting.Add(name,temp.ToArray());
        }

        internal static void ClearTemp()
        {
            if(ModEntry.Config.Debug)
                ModEntry.Mon.Log("GOT CALLED (ClearTemp)",LogLevel.Alert);
            
            AlreadyPatched?.Clear();
            Dialogues?.Clear();
            Greetings?.Clear();
            Notifs?.Clear();
            Questions?.Clear();
            QuestionCounter?.Clear();
            RandomPool?.Clear();
            HasCustomGifting?.Clear();
        }
        internal static void RemoveAnyEmpty()
        {
            List<string> toremove = new();
            
            foreach (var pair in Questions)
            {
                if (!pair.Value.Any())
                    toremove.Add(pair.Key);
            }
            
            foreach (var name in toremove)
            {
                Questions.Remove(name);
            }
        }
        #endregion
        /* Required by mod to work */
        #region own data
        internal static Dictionary<string, List<RawQuestions>> Questions { get; private set; } = new();
        internal static Dictionary<string, List<RawDialogues>> Dialogues { get; private set; } = new();
        internal static Dictionary<(string, string), string> Greetings { get; private set; } = new();
        internal static List<RawNotifs> Notifs { get; private set; } = new();
        internal static Dictionary<string, List<string>> RandomPool { get; private set; } = new();
        internal static Dictionary<string, int> QuestionCounter { get; set; } = new();
        #endregion
        
        #region variable data
        internal static List<string> PatchableNPCs { get; private set; } = new();
        internal static List<(string, string, string)> AlreadyPatched { get; set; } = new();
        internal static Dictionary<string,string[]> HasCustomGifting { get; set; } = new();
        #endregion

        #region constants
        internal const string PlayerFind = "playerFind";
        internal const string AddScene = "AddScene";
        internal const string RemoveScene = "RemoveScene";
        internal static string Yes { get; set; } = "Yes";
        internal static string No { get; set; } = "No";
        #endregion

        internal static IMonitor Mon { get; private set; }
        internal static IModHelper Help { get; private set; }
        internal static ModConfig Config;

/*#pragma warning disable CS8632
        private ISpaceCoreAPI? spaceCoreAPI;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
*/
    }
}