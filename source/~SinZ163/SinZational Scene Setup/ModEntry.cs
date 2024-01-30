/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace SinZational_Scene_Setup
{
    public class ModEntry : Mod
    {
        private Queue<string> EventQueue = new();
        private int IterationstoSkip = 0;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            helper.ConsoleCommands.Add("sinz.playevents", "Auto plays events in the current location. If arguments are given is treated as a specific, or all if location is 'ALL'", (command, args) =>
            {
                EventQueue = new();
                ArgUtility.TryGetOptionalRemainder(args, 0, out string arg);
                if (arg?.ToUpper() == "ALL")
                {
                    foreach (var location in Game1.locations)
                    {
                        AddEvents(location);
                    }
                }
                else
                {
                    var location = Game1.currentLocation;
                    if (arg != null)
                    {
                        location = Utility.fuzzyLocationSearch(arg);
                        if (location == null)
                        {
                            this.Monitor.Log($"Unknown location {arg}", LogLevel.Warn);
                            return;
                        }
                    }
                    if (location == null)
                    {
                        this.Monitor.Log("Current location is null, rip", LogLevel.Warn);
                        return;
                    }
                    AddEvents(location);
                }
            });
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            // Run 3 times a second for speeed
            if (!e.IsMultipleOf(20)) return;

            if (IterationstoSkip-- > 0) return;

            if (Game1.eventUp)
            {
                if (Game1.activeClickableMenu is DialogueBox db)
                {
                    if (db.isQuestion)
                    {
                        // TODO: Iterate all question variants?
                        Monitor.Log($"Asked a question with {db.responses.Length} options, selecting option 0", LogLevel.Info);
                        Monitor.Log(JsonConvert.SerializeObject(db.responses), LogLevel.Trace);
                        db.selectedResponse = 0;
                        IterationstoSkip = 1;
                    }
                    Monitor.Log("Clicking on the dialogue box");
                    db.receiveLeftClick(0, 0);
                }
                else if (Game1.activeClickableMenu is NamingMenu nm)
                {
                    // Hope doing this at 3tps isn't a problem
                    nm.receiveLeftClick(nm.doneNamingButton.bounds.Center.X, nm.doneNamingButton.bounds.Center.Y);
                }
                return;
            }
            if (!Game1.game1.IsActive) return;
            if (EventQueue.TryDequeue(out var eventId))
            {
                // Burn the players inventory every event to make sure space exists
                for (int i = Game1.player.Items.Count - 1; i >= 0; i--)
                {
                    Game1.player.Items[i] = null;
                }
                eventId = eventId.Split('/')[0];
                Monitor.Log($"Playing {eventId}, {EventQueue.Count} events remaining");
                Game1.game1.parseDebugInput($"ebi {eventId}");
                IterationstoSkip = 4;
            }
        }

        private void AddEvents(GameLocation location)
        {
            try
            {
                var events = Helper.GameContent.Load<Dictionary<string,string>>($"Data/Events/{location.Name}");
                Monitor.Log($"Location {location.Name} has {events.Count} events", LogLevel.Info);
                foreach (var key in events.Keys)
                {
                    if (key.IndexOf('/') == -1)
                    {
                        Monitor.Log($"{key} is likely a fork, skipping...");
                        continue;
                    }
                    var skip = false;
                    foreach(var segment in key.Split('/'))
                    {
                        if (segment.StartsWith("x "))
                        {
                            Monitor.Log($"{key} contains an x precondition, skipping as it is an unnatural event.");
                            skip = true;
                            break;
                        }
                    }
                    if (skip) continue;
                    EventQueue.Enqueue(key);
                }
            }
            catch (ContentLoadException)
            {
                Monitor.Log($"Location {location.Name} does not have events?", LogLevel.Info);
            }
        }
    }
}
