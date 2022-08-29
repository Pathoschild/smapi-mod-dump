/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Commands;

#region using directives

using Attributes;
using Extensions.Collections;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Handles mod-provided console commands.</summary>
internal class CommandHandler
{
    /// <summary>Cache of handled <see cref="IConsoleCommand"/> instances.</summary>
    private readonly Dictionary<string, IConsoleCommand> _HandledCommands = new();

    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _CommandHelper;

    /// <summary>The <see cref="string"/> used as entry for all handled commands.</summary>
    public string EntryCommand = null!;

    /// <summary>Human-readable name of the mod providing commands.</summary>
    public string Mod = null!;

    /// <summary>Construct an instance.</summary>
    /// <param name="helper">Provides an API for managing console commands.</param>
    internal CommandHandler(ICommandHelper helper)
    {
        _CommandHelper = helper;

        Log.D("[CommandHandler]: Gathering commands...");
        var commandTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IConsoleCommand)))
            .Where(t => t.IsAssignableTo(typeof(IConsoleCommand)) && !t.IsAbstract)
            .ToArray();

        Log.D($"[CommandHandler]: Found {commandTypes.Length} command classes. Initializing commands...");
        foreach (var c in commandTypes)
        {
            try
            {
#if RELEASE
                var debugOnlyAttribute =
                    (DebugOnlyAttribute?)c.GetCustomAttributes(typeof(DebugOnlyAttribute), false).FirstOrDefault();
                if (debugOnlyAttribute is not null) continue;
#endif

                var command = (IConsoleCommand)c
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { GetType() }, null)!
                    .Invoke(new object?[] { this });
                foreach (var trigger in command.Triggers)
                    _HandledCommands.Add(trigger, command);

                Log.D($"[CommandHandler]: Handling {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.E($"[CommandHandler]: Failed to handle {c.Name}.\n{ex}");
            }
        }

        Log.D("[CommandHandler] Command initialization completed.");
    }

    /// <summary>Register the entry command and name for this module.</summary>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled commands.</param>
    /// <param name="mod">Human-readable name of the mod providing commands.</param>
    internal void Register(string entry, string mod)
    {
        EntryCommand = entry;
        Mod = mod;
        var documentation =
            $"The entry point for all {mod} console commands. Type `{entry} help` to list available commands.";
        _CommandHelper.Add(entry, documentation, Entry);
    }

    /// <summary>Handles the entry command for this module, delegating to the appropriate <see cref="IConsoleCommand"/>.</summary>
    /// <param name="command">The entry command.</param>
    /// <param name="args">The supplied arguments.</param>
    internal void Entry(string command, string[] args)
    {
        if (args.Length <= 0)
        {
            Log.I(
                $"This is the entry point for all {Mod} console commands. Use it by specifying a command to be executed. " +
                $"For example, typing `{command} help` will invoke the `help` command, which lists all available commands.");
            return;
        }

        if (string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var result = "Available commands:";
            _HandledCommands.Values.Distinct().ForEach(c =>
            {
                result +=
                    $"\n\t-{command} {c.Triggers.First()}";
            });
            Log.I(result);
            return;
        }

        if (!_HandledCommands.TryGetValue(args[0].ToLowerInvariant(), out var handled))
        {
            Log.W($"{args[0]} is not a valid command. Use `{command} help` to see available sub-commands.");
            return;
        }

        if (args.Length > 1 && (string.Equals(args[1], "help", StringComparison.InvariantCultureIgnoreCase) ||
                                string.Equals(args[1], "doc", StringComparison.InvariantCultureIgnoreCase)))
        {
            Log.I(
                $"{handled.Documentation}\n\nAliases: {string.Join(',', handled.Triggers.Skip(1).Select(t => "`" + t + "`"))}");
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save before running this command.");
            return;
        }

        handled.Callback(args.Skip(1).ToArray());
    }
}