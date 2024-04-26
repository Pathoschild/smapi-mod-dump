/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using GenericModConfigMenu;

namespace HeartEventHelper
{
    /// <summary>The mod entry point.</summary>
    
    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            /// Initialize Mod & Config
            SMonitor = Monitor;
            Config = Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            /// Harmony Patching
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), "parseDialogueString"),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.parseDialogueString_Transpiler)));
            harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), "Question"),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Question_Transpiler)));
            harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), "QuickQuestion"),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.QuickQuestion_Transpiler)));
        }

        /// Harmony Methods ///
        public static IEnumerable<CodeInstruction> parseDialogueString_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Dialogue.parseDialogueString");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldelem_Ref && codes[i + 2].opcode == OpCodes.Newobj && (ConstructorInfo)codes[i + 2].operand == AccessTools.Constructor(typeof(NPCDialogueResponse), new System.Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(string) }))
                {
                    newCodes.Add(codes[i]); // load response
                    newCodes.Add(new CodeInstruction(OpCodes.Ldloc_S, 17)); // get responseSplit
                    newCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                    newCodes.Add(new CodeInstruction(OpCodes.Ldelem_Ref)); // load reaction (responseSplit[1])
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.AddReactionText)))); // call AddReactionText
                }
                else
                    newCodes.Add(codes[i]);
            }
            return newCodes.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> QuickQuestion_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Event.DefaultCommands.QuickQuestion");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && (string)codes[i].operand == "quickQuestion")
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetQuickQuestionAnswers)))); // call GetQuickQuestionAnswers
                    newCodes.Add(codes[i]); // load "quickQuestion"
                }
                else
                    newCodes.Add(codes[i]);
            }
            return newCodes.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> Question_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Event.DefaultCommands.Question");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_0)
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetQuestionAnswers)))); // call GetQuestionAnswers
                    newCodes.Add(codes[i]); // load dialogueKey
                }
                else
                    newCodes.Add(codes[i]);
            }
            return newCodes.AsEnumerable();
        }

        /// Other Methods ///
        public static Response[] GetQuickQuestionAnswers(Response[] answers)
        {
            try
            {
                string currentCommand = Game1.CurrentEvent.GetCurrentCommand();
                string[] scriptsSplit = currentCommand.Substring(currentCommand.IndexOf("(break)") + 7).Split("(break)");

                for (int i = 0; i < answers.Length; i++)
                {
                    int friendship = 0;

                    if (ArgUtility.TryGet(scriptsSplit, i, out var script, out var error))
                    {
                        string[] scriptCommands = script.Split('\\');

                        foreach (string command in scriptCommands)
                        {
                            string[] commandSplit = ArgUtility.SplitBySpaceQuoteAware(command);
                            string commandName = commandSplit[0];

                            switch (commandName)
                            {
                                case "friendship":
                                case "friend":
                                    string reaction = commandSplit[2];
                                    SMonitor.Log($"friendship command at answer {i + 1}, gives {reaction} pts", LogLevel.Trace);
                                    friendship += int.Parse(reaction);
                                    break;
                                case "fork":
                                    ArgUtility.TryGet(commandSplit, 1, out var req, out var error2);
                                    ArgUtility.TryGetOptional(commandSplit, 2, out var forkEventID, out error2);
                                    if (forkEventID == null)
                                    {
                                        forkEventID = req;
                                        req = null;
                                    }
                                    if ((req == null && Game1.CurrentEvent.specialEventVariable1) || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                                    {
                                        SMonitor.Log($"fork command at answer {i + 1}, attempting branch to event {forkEventID}", LogLevel.Trace);
                                        friendship += BranchHandler(forkEventID);
                                    }
                                    break;
                                case "switchEvent":
                                    string switchEventID = commandSplit[1];
                                    SMonitor.Log($"switchEvent command at answer {i + 1}, attempting branch to event {switchEventID}", LogLevel.Trace);
                                    friendship += BranchHandler(switchEventID);
                                    break;
                            }
                        }
                    }
                    answers[i].responseText = AddReactionText(answers[i].responseText, friendship.ToString());
                }
                return answers;
            }
            catch (Exception e)
            {
                SMonitor.Log($"GetQuickQuestionAnswers in HeartEventHelper failed! Here's some info, please report:", LogLevel.Error);
                SMonitor.Log($"Event ID: {Game1.CurrentEvent.id}", LogLevel.Error);
                SMonitor.Log($"Event Location: {Game1.currentLocation.Name}", LogLevel.Error);
                List<string> actors = new List<string>();
                foreach (NPC actor in Game1.CurrentEvent.actors) { actors.Add(actor.Name); }
                SMonitor.Log($"Event Actors: {string.Join(", ", actors.ToArray())}", LogLevel.Error);
                SMonitor.Log("Event Error Message: ", LogLevel.Error);
                SMonitor.Log(e.Message, LogLevel.Error);
                return answers;
            }
        }
        public static Response[] GetQuestionAnswers(Response[] answers)
        {
            int forkedFriendship = 0;
            int unforkedFriendship = 0;
            int numToFork = 0;

            try
            {
                for (int i = 0; i < Game1.CurrentEvent.eventCommands.Length; i++)
                {
                    string command1 = Game1.CurrentEvent.eventCommands[i];

                    if (command1 == Game1.CurrentEvent.GetCurrentCommand())
                    {
                        string[] command1Split = ArgUtility.SplitBySpaceQuoteAware(command1);

                        if (!int.TryParse(command1Split[1].Substring(4), out numToFork))
                        {
                            break;
                        }

                        for (int j = i + 1; j < Game1.CurrentEvent.eventCommands.Length; j++)
                        {
                            string command2 = Game1.CurrentEvent.eventCommands[j];
                            string[] command2Split = ArgUtility.SplitBySpaceQuoteAware(command2);
                            string command2Name = command2Split[0];

                            if (command2Name == "question" || command2Name == "end")
                            {
                                break;
                            }

                            switch (command2Name)
                            {
                                case "friendship":
                                case "friend":
                                    string reaction = command2Split[2];
                                    SMonitor.Log($"friendship command at index {j}, gives {reaction} pts", LogLevel.Trace);
                                    unforkedFriendship += int.Parse(reaction);
                                    break;
                                case "fork":
                                    ArgUtility.TryGet(command2Split, 1, out var req, out var error);
                                    ArgUtility.TryGetOptional(command2Split, 2, out var forkEventID, out error);
                                    if (forkEventID == null)
                                    {
                                        forkEventID = req;
                                        req = null;
                                    }
                                    if (req == null || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                                    {
                                        SMonitor.Log($"fork command at index {j}, attempting branch to event {forkEventID}", LogLevel.Trace);
                                        forkedFriendship += BranchHandler(forkEventID);
                                    }
                                    break;
                                case "switchEvent":
                                    string switchEventID = command2Split[1];
                                    SMonitor.Log($"switchEvent command at index {j}, attempting branch to event {switchEventID}", LogLevel.Trace);
                                    unforkedFriendship += BranchHandler(switchEventID);
                                    break;
                            }
                        }
                    }
                }
                foreach (Response answer in answers)
                {
                    bool numToForkIsSafe = numToFork >= 0 && numToFork < answers.Length;
                    bool forkedToggle = Game1.currentLocation.currentEvent.specialEventVariable1;
                    bool shouldFork = numToForkIsSafe ? (answer == answers[numToFork] && !forkedToggle) || (answer != answers[numToFork] && forkedToggle) : forkedToggle;

                    if (shouldFork)
                    {
                        answer.responseText = AddReactionText(answer.responseText, forkedFriendship.ToString());
                    }
                    else
                    {
                        answer.responseText = AddReactionText(answer.responseText, unforkedFriendship.ToString());
                    }
                }
                return answers;
            }
            catch (Exception e)
            {
                SMonitor.Log("GetQuestionAnswers in HeartEventHelper failed! Here's some info, please report:", LogLevel.Error);
                SMonitor.Log($"Event ID: {Game1.CurrentEvent.id}", LogLevel.Error);
                SMonitor.Log($"Event Location: {Game1.currentLocation.Name}", LogLevel.Error);
                List<string> actors = new List<string>();
                foreach (NPC actor in Game1.CurrentEvent.actors) { actors.Add(actor.Name); }
                SMonitor.Log($"Event Actors: {string.Join(", ", actors.ToArray())}", LogLevel.Error);
                SMonitor.Log("Event Error Message: ", LogLevel.Error);
                SMonitor.Log(e.Message, LogLevel.Error);
                return answers;
            }
        }
        public static int BranchHandler(string eventID)
        {
            Event branchEvent = Game1.currentLocation.findEventById(eventID);
            int friendship = 0;
            
            if (branchEvent == null)
            {
                if (Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Temp").TryGetValue(eventID, out var eventScript))
                {
                    branchEvent = new Event(eventScript);
                }
                else
                {
                    SMonitor.Log($"BranchHandler in HeartEventHelper couldn't find branching event with ID {eventID}, friendship calculation may be inaccurate. Please report!", LogLevel.Error);
                    return 0;
                }
            }

            for (int i = 0; i < branchEvent.eventCommands.Length; i++)
            {
                string command = branchEvent.eventCommands[i];
                string[] commandSplit = ArgUtility.SplitBySpaceQuoteAware(command);
                string commandName = commandSplit[0];

                if (commandName == "question" || commandName == "end")
                {
                    break;
                }

                switch (commandName)
                {
                    case "friendship":
                    case "friend":
                        string reaction = commandSplit[2];
                        SMonitor.Log($"friendship command in branch {eventID} at index {i}, gives {commandSplit[2]} pts", LogLevel.Trace);
                        friendship += int.Parse(reaction);
                        break;
                    case "fork":
                        ArgUtility.TryGet(commandSplit, 1, out var req, out var error);
                        ArgUtility.TryGetOptional(commandSplit, 2, out var forkEventID, out error);
                        if (forkEventID == null)
                        {
                            forkEventID = req;
                            req = null;
                        }
                        if ((req == null && Game1.CurrentEvent.specialEventVariable1) || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                        {
                            SMonitor.Log($"fork command at index {i}, attempting branch to event {forkEventID}", LogLevel.Trace);
                            friendship += BranchHandler(forkEventID); // recursion (spooky), may remove
                        }
                        break;
                    case "switchEvent":
                        string switchEventID = commandSplit[1];
                        SMonitor.Log($"switchEvent command at index {i}, attempting branch to event {switchEventID}", LogLevel.Trace);
                        friendship += BranchHandler(switchEventID); // recursion (spooky), may remove
                        break;
                }
            }
            return friendship;
        }
        public static string AddReactionText(string response, string reaction)
        {
            if (response == "") { return response; }
            int friendship = int.Parse(reaction);

            string beforePositive = ReplaceBrackets(Config.beforePositive, friendship);
            string beforeNeutral = ReplaceBrackets(Config.beforeNeutral, friendship);
            string beforeNegative = ReplaceBrackets(Config.beforeNegative, friendship);
            string afterPositive = ReplaceBrackets(Config.afterPositive, friendship);
            string afterNeutral = ReplaceBrackets(Config.afterNeutral, friendship);
            string afterNegative = ReplaceBrackets(Config.afterNegative, friendship);

            if (friendship > 0) { return $"{beforePositive}{response}{afterPositive}"; }
            if (friendship < 0) { return $"{beforeNegative}{response}{afterNegative}"; }
            else return $"{beforeNeutral}{response}{afterNeutral}";
        }
        public static string ReplaceBrackets(string config, int friendship)
        {
            int friendshipAbsolute = Math.Abs(friendship);
            float friendshipHearts = friendship / (float)250;

            config = config.Replace("{#}", friendship.ToString());
            config = config.Replace("{#_abs}", friendshipAbsolute.ToString());
            config = config.Replace("{#_heart}", friendshipHearts.ToString((friendship % 250 == 0) ? "0" : "0.0"));

            config = config.Replace("{1}", "¢");
            config = config.Replace("{2}", "£");
            config = config.Replace("{3}", "¤");
            config = config.Replace("{heart}", "♡");

            return config;
        }

        /// Config ///
        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod configs
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // add Configs title
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Configs"
            );

            // add config fields
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Positive",
                getValue: () => Config.beforePositive,
                setValue: value => Config.beforePositive = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Neutral",
                getValue: () => Config.beforeNeutral,
                setValue: value => Config.beforeNeutral = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Negative",
                getValue: () => Config.beforeNegative,
                setValue: value => Config.beforeNegative = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Positive",
                getValue: () => Config.afterPositive,
                setValue: value => Config.afterPositive = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Neutral",
                getValue: () => Config.afterNeutral,
                setValue: value => Config.afterNeutral = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Negative",
                getValue: () => Config.afterNegative,
                setValue: value => Config.afterNegative = value
            );
            /*configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Disable if All Neutral",
                tooltip: () =>
                "This option attempts to not change dialogue if all options are neutral, but there are certain dialogues it can't detect. " +
                "For the most consistency, it is best to leave 'false' and make the text before and after neutral blank if you want to disable changes to neutral responses. ",
                getValue: () => Config.neutralDisable,
                setValue: value => Config.neutralDisable = value
            );*/

            // add Notes title
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Notes"
            );

            // add Notes paragraph
            configMenu.AddParagraph(
                mod: ModManifest,
                text: () =>
                "Use '{#}', including the brackets, to insert the number of friendship points gained or lost. " +
                "Use '{#_abs}' to insert the absolute value of friendship points, and '{#_heart}' to insert the number of hearts gained or lost (points/250). " +
                "The special characters to use the custom icons are '¢', '£', and '¤', or alternatively '{1}', '{2}', and '{3}'. " +
                "Other common special characters include '♡' or '{heart}' (heart), '*' (star), '`' (up arrow), and '_' (down arrow if SVE or DaisyNiko's tilesheets are installed). " +
                "Spaces aren't automatically added between the default dialogue and affixes, so they must be included in the boxes above if desired. " +
                // "The 'Disable if All Neutral' config attempts to not change dialogue if all options are neutral, but there are certain dialogues it can't detect. " +
                // "For the most consistency, it is best to leave 'false' and make the text before and after neutral blank if you want to disable changes to neutral responses. " +
                "To disable changes to neutral responses, clear the 'Text Before Neutral' and 'Text After Neutral' fields. " +
                "See the 'Heart Event Helper - Icons (CP)' config menu to change the custom icons."
            );

        }
    }
}