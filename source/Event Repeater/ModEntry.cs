/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using EventRepeater.Integrations;
using EventRepeater.Framework;

namespace EventRepeater
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The event IDs to forget.</summary>
        private HashSet<int> EventsToForget = new HashSet<int>();
        private HashSet<string> MailToForget = new HashSet<string>();
        private HashSet<int> ResponseToForget = new HashSet<int>();
        private Event? LastEvent;
        private List<int> ManualRepeaterList = new List<int>();
        private bool ShowEventIDs = false;
        private int LastPlayed;
        private ConfigModel Config = null!; //Menu Button
        int EventRemovalTimer = -1;
        int eventtoskip; //uses Game1.CurrentEvent.id to acquire the ID
        int mailIDCount; //used to monitor mail ID
        int responseCount; //used to monitor responses
        HashSet<string> OldFlags = new HashSet<string>();
        HashSet<int> responseMon = new HashSet<int>();
        public bool CheckMailBox = true;
        public string[]? MailboxContent;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            
            this.Config = helper.ReadConfig<ConfigModel>();

            AssetManager.Initialize(helper.GameContent, this.Monitor);
            helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
            helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

            helper.ConsoleCommands.Add("eventforget", "'usage: eventforget <id>", ForgetManualCommand);
            helper.ConsoleCommands.Add("showevents", "'usage: Lists all completed events", ShowEventsCommand);
            helper.ConsoleCommands.Add("showmail", "'usage: Lists all seen mail", ShowMailCommand);
            helper.ConsoleCommands.Add("mailforget", "'usage: mailforget <id>", ForgetMailCommand);
            helper.ConsoleCommands.Add("sendme", "'usage: sendme <id>", SendMailCommand);
            helper.ConsoleCommands.Add("showresponse", "'usage: Lists Response IDs.  For ADVANCED USERS!!", ShowResponseCommand);
            helper.ConsoleCommands.Add("responseforget", "'usage: responseforget <id>'", ForgetResponseCommand);
            //helper.ConsoleCommands.Add("responseadd", "'usage: responseadd <id>'  Inject a question response.", ResponseAddCommand);
            helper.ConsoleCommands.Add("repeateradd", "'usage: repeateradd <id(optional)>' Create a repeatable event.  If no id is given, the last seen will be repeated.  Works on Next Day", ManualRepeater);
            helper.ConsoleCommands.Add("repeatersave", "'usage: repeatersave <filename>' Creates a textfile with all events you set to repeat manually.", SaveManualCommand);
            helper.ConsoleCommands.Add("repeaterload", "'usage: repeaterload <filename>' Loads the file you designate.", LoadCommand);
            helper.ConsoleCommands.Add("inject", "'usage: inject <event, mail, response> <ID>' Example: 'inject event 1324329'  Inject IDs into the game.", injectCommand);
            helper.ConsoleCommands.Add("stopevent", "'usage: stops current event.", StopEventCommand);
            helper.ConsoleCommands.Add("showinfo", "Toggles in game visuals of certain alerts.", ShowInfoCommand);
            helper.ConsoleCommands.Add("emergencyskip", "Forces an event skip.. will progress the game", EmergencySkipCommand);
            helper.ConsoleCommands.Add("fastmail", "`usage: fastmail <mailID>` Send mail instantly to your Mailbox", FastMailCommand);
        }

        /// <summary>
        /// Reads in packs.
        /// </summary>
        /// <param name="sender">SMAPI</param>
        /// <param name="e">Event args.</param>
        /// <exception cref="InvalidDataException">The mod we tried to grab from is not a CP mod.</exception>
        private void OnLaunched(object? sender, GameLaunchedEventArgs e)
        {
           
            GMCMHelper gmcm = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
            if (gmcm.TryGetAPI())
            {
                gmcm.Register(
                    () => Config = new(),
                    () => this.Helper.WriteConfig(Config));

                foreach (var property in typeof(ConfigModel).GetProperties())
                {
                    gmcm.AddKeybindList(property, () => Config);
                }
            }

            foreach (IModInfo mod in this.Helper.ModRegistry.GetAll())
            {
                // make sure it's a Content Patcher pack
                if (!mod.IsContentPack || !mod.Manifest.ContentPackFor!.UniqueID.AsSpan().Trim().Equals("Pathoschild.ContentPatcher", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                // get the directory path containing the manifest.json
                // HACK: IModInfo is implemented by ModMetadata, an internal SMAPI class which
                // contains much more non-public information. Caveats:
                //   - This isn't part of the public API so it may break in future versions.
                //   - Since the class is internal, we need reflection to access the values.
                //   - SMAPI's reflection API doesn't let us reflect into SMAPI, so we need manual
                //     reflection instead.
                //   - SMAPI's data API doesn't let us access an absolute path, so we need to parse
                //     the model ourselves.

                IContentPack? modimpl = mod.GetType().GetProperty("ContentPack")!.GetValue(mod) as IContentPack;
                if (modimpl is null)
                    throw new InvalidDataException($"Couldn't grab mod from modinfo {mod.Manifest}");

                // read the JSON file
                if (modimpl.ReadJsonFile<ThingsToForget>("content.json") is not ThingsToForget model)
                    continue;
                //Check for Dependency
                //For now.. leave a warning!!
               /* if (!modimpl.Manifest.Dependencies.Any((dep) => dep.UniqueID.AsSpan().Trim().Equals("misscoriel.eventrepeater", StringComparison.OrdinalIgnoreCase)))
                    continue;*/
                bool loaded = false;
                // extract event IDs
                if (model.RepeatEvents?.Count is > 0)
                {
                    this.EventsToForget.UnionWith(model.RepeatEvents);
                    this.Monitor.Log($"Loading {model.RepeatEvents.Count} forgettable events for {mod.Manifest.UniqueID}", LogLevel.Trace);
                    loaded = true;
                }
                if (model.RepeatMail?.Count is > 0)
                {
                    this.MailToForget.UnionWith(model.RepeatMail);
                    this.Monitor.Log($"Loading {model.RepeatMail.Count} forgettable mail for {mod.Manifest.UniqueID}", LogLevel.Trace);
                    loaded = true;
                }
                if (model.RepeatResponse?.Count is > 0)
                {
                    this.ResponseToForget.UnionWith(model.RepeatResponse);
                    this.Monitor.Log($"Loading{model.RepeatResponse.Count} forgettable mail for {mod.Manifest.UniqueID}", LogLevel.Trace);
                    loaded = true;
                }
                if (loaded && !modimpl.Manifest.Dependencies.Any((dep) => dep.UniqueID.AsSpan().Trim().Equals("misscoriel.eventrepeater", StringComparison.OrdinalIgnoreCase)))
                {
                    Monitor.Log($"{modimpl.Manifest.Name} uses Event Repeater features, but doesn't list it as a dependency in its manifest.json. This will stop working in future versions.", LogLevel.Warn);
                }
            }
            this.Monitor.Log($"Loaded a grand total of\n\t{this.EventsToForget.Count} events\n\t{this.MailToForget.Count} mail\n\t{this.ResponseToForget.Count} responses.", LogLevel.Debug);
        }

        /*********
        ** A bunch of Methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.mailbox.Count > 0)
                MailboxMonitor();
            if (Game1.mailbox.Count == 0)
                CheckMailBox = true;
            if (MailboxContent != null && MailboxContent.Length != Game1.mailbox.Count)
                CheckMailBox = true;
            if(mailIDCount != Game1.player.mailReceived.Count)
            {
                if (mailIDCount == 0)
                {
                    OldFlags = new HashSet<string>(Game1.player.mailReceived);
                    mailIDCount = Game1.player.mailReceived.Count;
                    return;
                }                    
                else
                    MailIDMonitor();
            }
            if(responseCount != Game1.player.mailReceived.Count)
            {
                if (responseCount == 0)
                {
                    responseMon = new HashSet<int>(Game1.player.dialogueQuestionsAnswered);
                    responseCount = Game1.player.dialogueQuestionsAnswered.Count;
                    return;
                }
                else
                    ResponseMonitor();
            }
            if (this.LastEvent is null && Game1.CurrentEvent is not null)
                this.OnEventStarted(Game1.CurrentEvent);
            
            this.LastEvent = Game1.CurrentEvent;
            if (this.EventRemovalTimer > 0)
            {
                this.EventRemovalTimer--;
                if (this.EventRemovalTimer <= 0)
                {
                    Game1.player.eventsSeen.Remove(eventtoskip);
                    Monitor.Log("Event removed from seen list", LogLevel.Debug);

                }
            }
        }
        public void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
           /* if(e.Button == this.Config.EventWindow)
            {
                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = new EventRepeaterWindow(this.Helper.Data, this.Helper.DirectoryPath);
            }*/
        }

        public void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (this.Config.ShowInfo.JustPressed())
            {
                ShowInfoCommand(null, null);
            }

            if (Game1.CurrentEvent is null)
                return;


            if (this.Config.NormalSkip.JustPressed())
            {
                EmergencySkipCommand(null, null);
            }
            if (this.Config.EmergencySkip.JustPressed())
            {
                StopEventCommand(null, null);
            }
        }

        private void OnEventStarted(Event @event)
        {
            Monitor.Log($"Current Event: {Game1.CurrentEvent.id}", LogLevel.Debug);
            LastPlayed = Game1.CurrentEvent.id;
            
            if(ShowEventIDs == true)
            {
                Game1.addHUDMessage(new HUDMessage($"Current Event: {Game1.CurrentEvent.id}!"));
            }
            Game1.CurrentEvent.eventCommands = this.ExtractCommands(Game1.CurrentEvent.eventCommands, new[] { "forgetEvent", "forgetMail", "forgetResponse", "timeAdvance" }, out ISet<string> extractedCommands);

            foreach (string command in extractedCommands)
            {
                // extract command name + raw ID
                string commandName, rawId;
                {
                    string[] parts = command.Split(' ');
                    commandName = parts[0];
                    if (parts.Length != 2) // command name + ID
                    {
                        this.Monitor.Log($"The {commandName} command requires one argument (event command: {command}).", LogLevel.Warn);
                        continue;
                    }
                    rawId = parts[1];
                }

                // handle command
                switch (commandName)
                {
                    case "forgetEvent":
                        if (int.TryParse(rawId, out int eventID))
                            Game1.player.eventsSeen.Remove(eventID);
                        else
                            this.Monitor.Log($"Could not parse event ID '{rawId}' for {commandName} command.", LogLevel.Warn);
                        break;

                    case "forgetMail":
                        Game1.player.mailReceived.Remove(rawId);
                        break;

                    case "forgetResponse":
                        if (int.TryParse(rawId, out int responseID))
                        {
                            Game1.player.dialogueQuestionsAnswered.Remove(responseID);
                            //this.Monitor.Log($"Removed {responseID}", LogLevel.Debug);
                        }
                        else
                            this.Monitor.Log($"Could not parse response ID '{rawId}' for {commandName} command.", LogLevel.Warn);
                        break;
                    case "timeAdvance":
                        if (int.TryParse(rawId, out int hours))
                        {
                            int newTime = Utility.ModifyTime(Game1.timeOfDay, hours * 60);
                            if (newTime < 2600)
                            {
                                Game1.timeOfDay = newTime;
                            }
                            else
                                this.Monitor.Log($"Cannot advance time! {hours} would put the time to {newTime}!  Command ignored!", LogLevel.Error);
                        }
                        else
                            this.Monitor.Log($"Time advancement failed: invalid number '{rawId}'.", LogLevel.Warn);
                        break;                       
                    default:
                        this.Monitor.Log($"Unrecognized command name '{commandName}'.", LogLevel.Warn);
                        break;
                }
            }
        }
        private string[] ExtractCommands(string[] commands, string[] commandNamesToExtract, out ISet<string> extractedCommands)
        {
            var otherCommands = new List<string>(commands.Length);
            extractedCommands = new HashSet<string>();
            foreach (string command in commands)
            {
                if (commandNamesToExtract.Any(name => command.StartsWith(name)))
                    extractedCommands.Add(command);
                else
                    otherCommands.Add(command);
            }

            return otherCommands.ToArray();
        }


        [EventPriority(EventPriority.High + 1000)]
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            

            bool removed = false;
            mailIDCount = Game1.player.mailReceived.Count;
            if (this.EventsToForget.Count > 0 || this.ManualRepeaterList.Count > 0 || AssetManager.EventsToForget.Value.Count > 0)
            {
                for (int i = Game1.player.eventsSeen.Count - 1; i >= 0; i--)
                {
                    var evt = Game1.player.eventsSeen[i];
                    if (this.EventsToForget.Contains(evt) || AssetManager.EventsToForget.Value.Contains(evt))
                    {
                        this.Monitor.Log("Repeatable Event Found! Resetting for next time! Event ID: " + evt);
                        Game1.player.eventsSeen.RemoveAt(i);
                        removed = true;
                    }
                    else if (this.ManualRepeaterList.Contains(evt))
                    {
                        this.Monitor.Log("Manual Repeater Engaged! Resetting: " + evt);
                        Game1.player.eventsSeen.RemoveAt(i);
                        removed = true;
                    }
                }
            }
            if (!removed)
            {
                this.Monitor.Log("No repeatable events were removed");
            }

            var assetMail = Game1.content.Load<Dictionary<string, string>>(AssetManager.MailToRepeatName);

            removed = false;
            if (this.MailToForget.Count > 0 || assetMail.Count > 0)
            {
                for (int i = Game1.player.mailReceived.Count - 1; i >= 0; i--)
                {
                    var msg = Game1.player.mailReceived[i];
                    if (this.MailToForget.Contains(msg) || assetMail.ContainsKey(msg))
                    {
                        this.Monitor.Log("Repeatable Mail found!  Resetting: " + msg);
                        Game1.player.mailReceived.RemoveAt(i);
                        removed = true;
                    }
                }
            }
            if (!removed)
            {
                this.Monitor.Log("No repeatable mail found for removal.");
            }

            removed = false;
            if (this.ResponseToForget.Count > 0 || AssetManager.ResponseToForget.Value.Count > 0)
            {
                for (int i = Game1.player.dialogueQuestionsAnswered.Count - 1; i >= 0; i--)
                {
                    var response = Game1.player.dialogueQuestionsAnswered[i];
                    if (this.ResponseToForget.Contains(response) || AssetManager.ResponseToForget.Value.Contains(response))
                    {
                        Game1.player.dialogueQuestionsAnswered.RemoveAt(i);
                        this.Monitor.Log("Repeatable Response Found! Resetting: " + response);
                        removed = true;
                    }
                }
            }
            if (!removed)
            {
                this.Monitor.Log("No repeatable responses found.");
            }
        }

        private void ManualRepeater(string command, string[] parameters)
        {
            //This command will set a manual repeat to a list and save the event IDs to a file in the SDV folder.  
            //The first thing to do is create a list
            List<int> eventsSeenList = new List<int>();
            //Populate that list with Game1.eventseen
            foreach(int eventID in Game1.player.eventsSeen)
            {
                eventsSeenList.Add(eventID);
            }
            //Check to see if an EventID was added in the command.. If not, then add the last ID on the list
            if (parameters.Length == 0)
            {
                try
                {
                    int lastEvent = eventsSeenList[eventsSeenList.Count - 1];//Count -1 to account for 0
                    Game1.player.eventsSeen.Remove(lastEvent);//Removes ID from events seen
                    ManualRepeaterList.Add(lastEvent);//Adds to the Manual List
                    Monitor.Log($"{lastEvent} has been added to Manual Repeater", LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.Message, LogLevel.Warn);
                }
            }
            else
            {
                try
                {
                    //convert parameters to int
                    int parseParameter = Int32.Parse(parameters[0]);
                    ManualRepeaterList.Add(parseParameter);
                    Game1.player.eventsSeen.Remove(parseParameter);
                    Monitor.Log($"{parseParameter} has been added to Manual Repeater", LogLevel.Debug);
                }
                catch(Exception ex)
                {
                    Monitor.Log(ex.Message, LogLevel.Warn);
                }
            }

        }

        private void ShowInfoCommand(string? command, string[]? parameters)
        {
            if (ShowEventIDs == true)
            {
                ShowEventIDs = false;
                Game1.addHUDMessage(new HUDMessage("In Game Alerts Disabled!"));
                return;
            }
            if (ShowEventIDs == false)
            {
                ShowEventIDs = true;
                Game1.addHUDMessage(new HUDMessage("In Game Alerts Enabled!"));
                return;
            }
        }
        private void FastMailCommand(string command, string[] parameters)
        {
            if (parameters[0] != null)
                Game1.addMail(parameters[0]);
            else if (parameters[0] == null)
                Monitor.Log("No Mail ID was added!", LogLevel.Error);

        }
        private void EmergencySkipCommand(string? command, string[]? parameters)
        {
            if (Game1.CurrentEvent is null)
                return;

            eventtoskip = Game1.CurrentEvent.id;

            try
            {
                                
                Game1.CurrentEvent.skipEvent();
                Monitor.Log($"Event {eventtoskip} was successfully skipped!", LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"Event {eventtoskip} was successfully skipped!"));
                }

            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Message, LogLevel.Error);
            }
        }

        private void StopEventCommand(string? command, string[]? parameters)
        {
            if (Game1.CurrentEvent is null)
                return;

            eventtoskip = Game1.CurrentEvent.id;
            EventRemovalTimer = 120;
            try
            {
                
                string[] currentEventCommandList = Game1.CurrentEvent.eventCommands;
                int stoppedCommand = Game1.CurrentEvent.currentCommand;
                Monitor.Log($"Emergency skip was engaged! Event Broke at this command: {currentEventCommandList[stoppedCommand]}", LogLevel.Error);
                currentEventCommandList[stoppedCommand] = currentEventCommandList[stoppedCommand] + " <=== Event was stopped here!!";

                Game1.CurrentEvent.exitEvent();
                Game1.warpFarmer("FarmHouse", 0, 0, false);
                Monitor.Log($"The Event {eventtoskip} has been interrupted. A dump of the Event is in the SDV folder.");
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"The Event {eventtoskip} has been interrupted.  A dump of the Event is in the SDV folder."));
                }
                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, $"EventDump{eventtoskip}.txt"), currentEventCommandList);
                


            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Message, LogLevel.Error);
            }
        }

        private void SaveManualCommand(string command, string[] parameters)
        {
            //This will allow you to save your repeatable events from the manual repeater
            //Create Directory
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\ManualRepeaterFiles");
            string savePath = Environment.CurrentDirectory + "\\ManualRepeaterFiles\\" + parameters[0] + ".txt"; //Saves file in the name you designate
            string[] parse = new string[ManualRepeaterList.Count];
            int i = 0; //start at beginning of manual list
            foreach(var item in ManualRepeaterList)//Converts the Manual list to a string array
            {
                parse[i] = item.ToString();
                i++;
            }
            File.WriteAllLines(savePath, parse);
            Monitor.Log($"Saved file to {savePath}", LogLevel.Debug);
        }

        private void LoadCommand(string command, string[] parameters)
        {
            //This will allow you to load a saved manual repeater file
            //First Check to see if you have the Directory
            if(Directory.Exists(Environment.CurrentDirectory + "\\ManualRepeaterFiles"))
            {
                string loadPath = Environment.CurrentDirectory + "\\ManualRepeaterFiles\\" + parameters[0] + ".txt"; //loads the filename you choose
                //Save all strings to a List
                List<string> FileIDs = new List<string>();
                FileIDs.AddRange(File.ReadAllLines(loadPath));
                //Transfer all items to ManualList and convert to int
                foreach(string eventID in FileIDs)
                {
                    int parse = Int32.Parse(eventID);
                    ManualRepeaterList.Add(parse);
                    Game1.player.eventsSeen.Remove(parse);
                }
                Monitor.Log($"{parameters[0]} loaded!", LogLevel.Debug);
            }
        }

        private void ForgetManualCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                if(parameters[0] == "last")
                {
                    if (LastPlayed == 0)
                    {
                        Monitor.Log("There is no previously played event.  Did you restart your game?", LogLevel.Error);
                        return;
                    }
                    else
                    {
                        Game1.player.eventsSeen.Remove(LastPlayed);
                        Monitor.Log($"Last played event, {LastPlayed}, was removed!", LogLevel.Debug);
                        if (ShowEventIDs == true)
                        {
                            Game1.addHUDMessage(new HUDMessage($"Last played event, {LastPlayed}, was removed!"));
                        }

                    }

                }
                if (parameters[0] == "all")
                {
                    Game1.player.eventsSeen.Clear();
                    Game1.player.eventsSeen.Add(60367);
                    Monitor.Log("All events removed! (Except the initial event)", LogLevel.Debug);
                    if (ShowEventIDs == true)
                    {
                        Game1.addHUDMessage(new HUDMessage("All events removed! (except the initial event)"));
                    }
                }
                else
                {
                    int eventToForget = int.Parse(parameters[0]);
                    Game1.player.eventsSeen.Remove(eventToForget);
                    Monitor.Log("Forgetting event id: " + eventToForget, LogLevel.Debug);
                    if(ShowEventIDs == true)
                    {
                        Game1.addHUDMessage(new HUDMessage($"Forgetting event id: {eventToForget}"));
                    }
                }

            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Message, LogLevel.Error);
            }
        }
        private void ShowEventsCommand(string command, string[] parameters)
        {
            string eventsSeen = "Events seen: ";
            foreach (var e in Game1.player.eventsSeen)
            {
                eventsSeen += e + ", ";
            }
            Monitor.Log(eventsSeen, LogLevel.Debug);
        }
        private void ShowMailCommand(string command, string[] parameters)
        {
            string mailSeen = "Mail Seen: ";
            foreach (var e in Game1.player.mailReceived)
            {
                mailSeen += e + ", ";
            }
            Monitor.Log(mailSeen, LogLevel.Debug);
        }
        private void ForgetMailCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                string MailToForget = parameters[0];
                Game1.player.mailReceived.Remove(MailToForget);
                OldFlags = new HashSet<string>(Game1.player.mailReceived);
                Monitor.Log("Forgetting mail id: " + MailToForget, LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"Forgetting mail id: {MailToForget}"));
                }

            }
            catch (Exception) { }
        }
        private void SendMailCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                string MailtoSend = parameters[0];
                Game1.addMailForTomorrow(MailtoSend);
                Monitor.Log("Check Mail Tomorrow!! Sending: " + MailtoSend, LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"Check Mail Tomorrow!! Sending: {MailtoSend}"));
                }

            }
            catch (Exception) { }
        }
        private void ShowResponseCommand(string command, string[] parameters)
        {
            string dialogueQuestionsAnswered = "Response IDs: ";
            foreach (var e in Game1.player.dialogueQuestionsAnswered)
            {
                dialogueQuestionsAnswered += e + ", ";
            }
            Monitor.Log(dialogueQuestionsAnswered, LogLevel.Debug);
        }
        private void ForgetResponseCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int responseToForget = int.Parse(parameters[0]);
                Game1.player.dialogueQuestionsAnswered.Remove(responseToForget);
                responseMon = new HashSet<int>(Game1.player.dialogueQuestionsAnswered);
                Monitor.Log("Forgetting Response ID: " + responseToForget, LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"Forgetting Response ID: {responseToForget}"));
                }

            }
            catch (Exception) { }
        }

        void ResponseMonitor()
        {
            if (this.responseMon == null || this.responseMon.Count != Game1.player.dialogueQuestionsAnswered.Count)
            {
                HashSet<int> updatedResponse = new HashSet<int>(Game1.player.dialogueQuestionsAnswered);
                if (responseMon != null)
                {
                    foreach (int oldResponse in this.responseMon)
                    {
                        if (!updatedResponse.Contains(oldResponse))
                            this.Monitor.Log($"Response ID {oldResponse} has been removed from dialogueQuestionsAnswered!", LogLevel.Debug);
                    }
                    foreach (int newResponse in updatedResponse)
                    {
                        if (!this.responseMon.Contains(newResponse))
                            this.Monitor.Log($"Response ID {newResponse} was added to dialogueQuestionsAnswered!", LogLevel.Debug);
                    }
                }
                this.responseMon = updatedResponse;
            }
        }
        void MailboxMonitor()
        {
           
            if (Game1.mailbox.Count >= 0 && CheckMailBox == true)
            {
                MailboxContent = Game1.mailbox.ToArray();
                string ListofMail = "You have the following Mail waiting in your Mailbox: ";
                foreach(string Mail in MailboxContent)
                {
                    ListofMail += Mail + ", ";
                }
                this.Monitor.Log(ListofMail, LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage(ListofMail));
                }

                CheckMailBox = false;
            }
        }
        void MailIDMonitor()
        {

            if (this.OldFlags == null || this.OldFlags.Count != Game1.player.mailReceived.Count)
            {

                // get new values
                HashSet<string> newValues = new(Game1.player.mailReceived);

                // detect changes (unless this is the first iteration)
                if (this.OldFlags != null)
                {
                    foreach (string oldFlag in this.OldFlags)
                    {
                        if (!newValues.Contains(oldFlag))
                            this.Monitor.Log($"Mail ID {oldFlag} was removed from mailRecieved", LogLevel.Debug);
                    }

                    foreach (string newFlag in newValues)
                    {
                        if (!this.OldFlags.Contains(newFlag))
                            this.Monitor.Log($"Mail ID {newFlag} was added to mailRecieved", LogLevel.Debug);
                    }
                }

                // save for next check
                this.OldFlags = newValues;
            }
        }
        private void injectCommand(string command, string[] parameters)
        {
            ///This will replace ResponseAdd in order to inject a more versitile code
            ///function: inject <type> <ID> whereas type is event, response, mail
            ///this will not have an indicator of existing events, however will look for the ID in the appropriate list.
            ///
            if (parameters.Length == 0) return;
            if (parameters.Length == 1)
            {
                if (parameters[0] == "response")
                {
                    Monitor.Log("No response ID entered.  Please input a response ID", LogLevel.Error);
                }
                if (parameters[0] == "mail")
                {
                    Monitor.Log("No mail ID entered.  Please input a mail ID", LogLevel.Error);
                }
                if (parameters[0] == "event")
                {
                    Monitor.Log("No event ID entered.  Please input a event ID", LogLevel.Error);
                }
            }
            if (parameters.Length == 2)
            {
                //check for existing IDs
                if (parameters[0] == "event")
                {
                    int parameterParse = int.Parse(parameters[1]);
                    if(Game1.player.eventsSeen.Contains(parameterParse))
                    {
                        Monitor.Log($"{parameters[1]} Already exists within seen events.", LogLevel.Warn);
                        return;
                    }
                    else
                    {
                        Game1.player.eventsSeen.Add(parameterParse);
                        Monitor.Log($"{parameters[1]} has been added to the seen events list.", LogLevel.Debug);
                        return;
                    }
                }
                if (parameters[0] == "response")
                {
                    int parameterParse = int.Parse(parameters[1]);
                    if(Game1.player.dialogueQuestionsAnswered.Contains(parameterParse))
                    {
                        Monitor.Log($"{parameters[1]} Already exists within the response list.", LogLevel.Warn);
                        return;

                    }
                    else
                    {
                        Game1.player.dialogueQuestionsAnswered.Add(parameterParse);
                        Monitor.Log($"{parameters[1]} has been added to the response list.", LogLevel.Debug);
                        return;

                    }
                }
                if (parameters[0] == "mail")
                {
                    if(Game1.player.mailReceived.Contains(parameters[1]))
                    {
                        Monitor.Log($"{parameters[1]} Already exists within seen events.", LogLevel.Warn);
                        return;
                    }
                    else
                    {
                        Game1.player.mailReceived.Add(parameters[1]);
                        Monitor.Log($"{parameters[1]} has been added to the seen events list.", LogLevel.Debug);
                        return;

                    }
                }



            }
        }


    }
}