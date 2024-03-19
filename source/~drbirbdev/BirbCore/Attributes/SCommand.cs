/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BirbCore.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace BirbCore.Attributes;

/// <summary>
/// A collection of commands in the SMAPI console.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SCommand(string name, string help = "") : ClassHandler(2)
{
    private readonly Dictionary<string, Action<string[]>> _commands = new();
    private readonly Dictionary<string, string> _helps = new();

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        base.Handle(type, instance, mod, [this._commands, this._helps, name]);

        mod.Helper.ConsoleCommands.Add(
            name: name,
            documentation: this.GetHelp(),
            callback: (s, commandArgs) => this.CallCommand(commandArgs)
        );
    }

    private string GetHelp(string? subCommand = null)
    {
        if (subCommand is not null)
        {
            return this._helps[subCommand];
        }

        StringBuilder sb = new();
        sb.Append($"{name}: {help}\n");
        foreach (string sub in this._helps.Keys)
        {
            string helpText = this._helps[sub];
            sb.Append($"\t{helpText}\n\n");
        }

        return sb.ToString();
    }

    private void CallCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Log.Info(this.GetHelp());
            return;
        }

        if (args[0].Equals("help", StringComparison.InvariantCultureIgnoreCase)
            || args[0].Equals("-h", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length > 1 && this._helps.ContainsKey(args[1]))
            {
                Log.Info(this.GetHelp(args[1]));
            }
            else
            {
                Log.Info(this.GetHelp());
            }

            return;
        }

        try
        {
            this._commands[args[0]].Invoke(args[1..]);
        }
        catch (Exception e)
        {
            Log.Info(this.GetHelp(args[0]));
            Log.Trace($"Args are:{string.Join(" ", args)}");
            Log.Trace(e.ToString());
        }
    }


    /// <summary>
    /// A single command. This property converts the method into a command if possible. Typed arguments
    /// will be converted from strings as best as possible. Supports optional numbered parameters, and
    /// variadic arguments (params parameter), and will deal with having more or fewer command line arguments
    /// compared to method arguments. Optionally includes help text.
    /// This attribute, combined with the SCommand attribute, creates a two-command structure to separate mod
    /// commands.
    /// <code>
    /// [SCommand(Name="birb")]
    /// public class BirbCommand {
    ///
    ///     [Command(Subname="one")]
    ///     public static void CommandOne(string arg0, string arg1) {
    ///
    ///     }
    /// }
    /// </code>
    /// The above will create a command like the following
    /// <code>
    /// birb one arg0 arg1
    /// </code>
    /// </summary>
    public class Command(string help = "") : MethodHandler
    {
        public override void Handle(MethodInfo method, object? instance, IMod mod, object[]? args = null)
        {
            if (instance is null)
            {
                Log.Error("SCommand class may be static? Cannot parse subcommands.");
                return;
            }

            if (args is null)
            {
                Log.Error("SCommand class didn't pass args");
                return;
            }

            Dictionary<string, Action<string[]>> commands = (Dictionary<string, Action<string[]>>)args[0];
            Dictionary<string, string> helps = (Dictionary<string, string>)args[1];
            string command = (string)args[2];


            string subCommand = method.Name.ToSnakeCase();

            commands.Add(subCommand, commandArgs =>
            {
                List<object> commandArgsList = [];

                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parameter = method.GetParameters()[i];
                    string? arg = commandArgs?.Length > i ? commandArgs[i] : null;


                    commandArgsList.Add(ParseArg(arg, parameter));
                    if (parameter.GetCustomAttribute(typeof(ParamArrayAttribute), false) is null)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < (commandArgs?.Length ?? 0); j++)
                    {
                        commandArgsList.Add(ParseArg(commandArgs?[j], parameter));
                    }
                }

                method.Invoke(instance, commandArgsList.ToArray());
            });

            StringBuilder help1 = new();
            help1.Append($"{command} {subCommand}");
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                string dotDotDot = parameter.GetCustomAttribute<ParamArrayAttribute>() is not null ? "..." : "";
                if (parameter.IsOptional)
                {
                    help1.Append($" [{parameter.Name}{dotDotDot}]");
                }
                else
                {
                    help1.Append($" <{parameter.Name}{dotDotDot}>");
                }
            }

            help1.Append($"\n\t\t{help}");

            helps.Add(subCommand, help1.ToString());
        }

        private static object ParseArg(string? arg, ParameterInfo parameter)
        {
            if (arg == null)
            {
                return Type.Missing;
            }

            if (parameter.ParameterType == typeof(string))
            {
                return arg;
            }

            if (parameter.ParameterType == typeof(int))
            {
                return int.Parse(arg);
            }

            if (parameter.ParameterType == typeof(double) || parameter.ParameterType == typeof(float))
            {
                return float.Parse(arg);
            }

            if (parameter.ParameterType == typeof(bool))
            {
                return bool.Parse(arg);
            }

            if (parameter.ParameterType == typeof(GameLocation))
            {
                return Utility.fuzzyLocationSearch(arg);
            }

            if (parameter.ParameterType == typeof(NPC))
            {
                return Utility.fuzzyCharacterSearch(arg);
            }

            if (parameter.ParameterType == typeof(FarmAnimal))
            {
                return Utility.fuzzyAnimalSearch(arg);
            }

            if (parameter.ParameterType == typeof(Farmer))
            {
                if (long.TryParse(arg, out long playerId))
                {
                    return Game1.getFarmerMaybeOffline(playerId);
                }

                return arg.Equals("host", StringComparison.InvariantCultureIgnoreCase)
                    ? Game1.MasterPlayer
                    : Game1.player;
            }

            return parameter.ParameterType == typeof(Item) ? Utility.fuzzyItemSearch(arg) : Type.Missing;
        }
    }
}
