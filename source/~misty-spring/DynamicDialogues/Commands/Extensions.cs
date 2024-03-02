/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Linq;
using StardewValley;

namespace DynamicDialogues.Commands;

internal static class Extensions
{
    /// <summary>
    /// Add if/else conditions to event-making. 
    /// Format: if \"QUERY\"#action#alternative
    /// </summary>
    /// <param name="event">Event</param>
    /// <param name="args">Parameters to use.</param>
    /// <param name="context">Event context.</param>
    public static void IfElse(Event @event, string[] args, EventContext context)
    {
        var fullArg = "";
        foreach (var part in args)
        {
            fullArg += part;
            if (args[^1] != part)
                fullArg += " ";
        }

        var rawArgs = fullArg.Replace("if ", "").Split("##");
        var condition = rawArgs[0];

        if (GameStateQuery.CheckConditions(condition))
        {
            //make rawArgs[1] the next command
            @event.InsertNextCommand(rawArgs[1]);
        }
        else if (rawArgs.Length >= 2)
        {
            //same as above, but for rawArgs[2]
            @event.InsertNextCommand(rawArgs[2]);
        }
        @event.CurrentCommand++;
    }


    /// <summary>
    /// Append another event to the current one (like forks, but less hard to understand).
    /// </summary>
    /// <param name="event">Event</param>
    /// <param name="args">Parameters to use.</param>
    /// <param name="context">Event context.</param>
    public static void Append(Event @event, string[] args, EventContext context)
    {
        if (args.Length < 2)
        {
            @event.LogCommandErrorAndSkip(args, "append must state an event string (e.g 'append myEvent')");
            return;
        }

        //get event to append
        var subEvent = args[1];

        // get specific string. used to get it with @event.exitLocation.Name but that might cause bugs
        var path = $"Data/Events/{Game1.currentLocation.Name}:{subEvent}";

        var events = Game1.content.LoadString(path);

        if (events == path)
        {
            @event.LogCommandErrorAndSkip(args, "Found no event with that key. Skipping...");
            return;
        }

        var commandParsed = events.Split('/');//.split('\\');

        //if theres a single command, append
        if (commandParsed.Length == 1)
        {
            @event.InsertNextCommand(commandParsed[0]);
            @event.CurrentCommand++;
            return;
        }

        // based off Event's InsertNextCommand
        var eventCommands = ModEntry.Help.Reflection.GetField<string[]>(@event, "eventCommands");
        var index = ModEntry.Help.Reflection.GetField<int>(@event, "currentCommand").GetValue();
        var commands = eventCommands.GetValue().ToList();

        foreach (var subcommand in commandParsed)
        {
            index++;
            if (index <= commands.Count)
            {
                commands.Insert(index, subcommand);
            }
            else
            {
                commands.Add(subcommand);
            }
        }
        eventCommands.SetValue(commands.ToArray());
        @event.CurrentCommand++;
    }

}
