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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

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
        private Event LastEvent;
        private List<int> ManualRepeaterList = new List<int>();
        private bool ShowEventIDs = true;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;

            // collect data models
            IList<ThingsToForget> models = new List<ThingsToForget>();
            foreach (IModInfo mod in this.Helper.ModRegistry.GetAll())
            {
                // make sure it's a Content Patcher pack
                if (!mod.IsContentPack || mod.Manifest.ContentPackFor?.UniqueID.Trim().Equals("Pathoschild.ContentPatcher", StringComparison.InvariantCultureIgnoreCase) != true)
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
                string directoryPath = (string)mod.GetType().GetProperty("DirectoryPath")?.GetValue(mod);
                if (directoryPath == null)
                    throw new InvalidOperationException($"Couldn't fetch the DirectoryPath property from the mod info for {mod.Manifest.Name}.");

                // read the JSON file
                IContentPack contentPack = this.Helper.ContentPacks.CreateFake(directoryPath);
                models.Add(contentPack.ReadJsonFile<ThingsToForget>("content.json"));
                // extract event IDs
                foreach (ThingsToForget model in models)
                {
                    if (model?.RepeatEvents == null)
                        continue;

                    foreach (int eventID in model.RepeatEvents)
                        this.EventsToForget.Add(eventID);

                }
                foreach (ThingsToForget model in models)
                {
                    if (model?.RepeatMail == null)
                        continue;

                    foreach (string mailID in model.RepeatMail)
                        this.MailToForget.Add(mailID);

                }
                foreach (ThingsToForget model in models)
                {
                    if (model?.RepeatResponse == null)
                        continue;

                    foreach (int ResponseID in model.RepeatResponse)
                        this.ResponseToForget.Add(ResponseID);

                } 

            }
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
        }

        /*********
        ** A bunch of Methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.LastEvent == null && Game1.CurrentEvent != null)
                this.OnEventStarted(Game1.CurrentEvent);

            this.LastEvent = Game1.CurrentEvent;
        }

        private void OnEventStarted(Event @event)
        {
            Monitor.Log($"Current Event: {Game1.CurrentEvent.id}", LogLevel.Debug);
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
            var otherCommands = new List<string>();
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
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (var seenEvent in this.EventsToForget)
            {
                bool anyEventRemoved = false;

                
                    if (Game1.player.eventsSeen.Remove(seenEvent))
                    {
                        anyEventRemoved = true;
                        this.Monitor.Log("Repeatable Event Found! Resetting for next time! Event ID: " + seenEvent, LogLevel.Debug);
                    }
                

                if (!anyEventRemoved)
                    this.Monitor.Log("You have not seen any Repeatable Events.");
            }
            
            foreach (string seenMail in this.MailToForget)
            {
                bool anyMailRemoved = false;
                if (Game1.player.mailReceived.Remove(seenMail))
                {
                    anyMailRemoved = true;
                    Monitor.Log("Repeatable Mail found!  Resetting: " + seenMail, LogLevel.Debug);
                }
                if (!anyMailRemoved)
                {
                    this.Monitor.Log("You have not opened any Repeatable mail.");
                }
                    
            }
            foreach (var seenResponse in this.ResponseToForget)
            {
                bool anyResponseRemoved = false;
                if (Game1.player.dialogueQuestionsAnswered.Remove(seenResponse))
                {
                    anyResponseRemoved = true;
                    Monitor.Log("Repeatable Response Found! Resetting: " + seenResponse, LogLevel.Debug);
                }
                if (!anyResponseRemoved)
                {
                    this.Monitor.Log("No repeatable responses found.");
                }
            }
            if (ManualRepeaterList.Count != 0)
            {
                foreach(int manualEvent in ManualRepeaterList)
                {
                    bool manualEventsRemoved = false;
                    if(Game1.player.eventsSeen.Remove(manualEvent))
                    {
                        manualEventsRemoved = true;
                        Monitor.Log("Manual Repeater Engaged! Resetting: " + manualEvent, LogLevel.Debug);
                    }
                    if(!manualEventsRemoved)
                    {
                        Monitor.Log("Manual Repeater is not being used.", LogLevel.Debug);
                    }
                }
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
        private void ShowInfoCommand(string command, string[] parameters)
        {
            if (ShowEventIDs == true)
            {
                ShowEventIDs = false;
                Game1.addHUDMessage(new HUDMessage("In Game Alerts Disabled!"));
            }
            if (ShowEventIDs == false)
            {
                ShowEventIDs = true;
                Game1.addHUDMessage(new HUDMessage("In Game Alerts Enabled!"));
            }
        }
        private void EmergencySkipCommand(string command, string[] parameters)
        {
            int eventtoskip = Game1.CurrentEvent.id;
            try
            {
                Game1.CurrentEvent.skipEvent();
                Monitor.Log($"Event {eventtoskip} was successfully skipped!");
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
        private void StopEventCommand(string command, string[] parameters)
        {
            int suddenstop = Game1.CurrentEvent.id;
            try
            {
                Game1.CurrentEvent.exitEvent();
                Monitor.Log($"The Event {suddenstop} has been interrupted. You can remove the event manually if needed.");
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"The Event {suddenstop} has been interrupted.  You can remove the event manually if needed."));
                }

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
                Monitor.Log("Forgetting Response ID: " + responseToForget, LogLevel.Debug);
                if (ShowEventIDs == true)
                {
                    Game1.addHUDMessage(new HUDMessage($"Forgetting Response ID: {responseToForget}"));
                }

            }
            catch (Exception) { }
        }
        private void ResponseAddCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int responseAdd = int.Parse(parameters[0]);
                Game1.player.dialogueQuestionsAnswered.Add(responseAdd);
                Monitor.Log("Injecting Response ID: " + responseAdd, LogLevel.Debug);
            }
            catch (Exception) { }
                        
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