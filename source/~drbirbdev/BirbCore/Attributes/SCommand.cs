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

namespace BirbCore.Attributes;

/// <summary>
/// A collection of commands in the SMAPI console.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SCommand : ClassHandler
{
    private readonly Dictionary<string, Action<string[]>> Commands = new();
    private readonly Dictionary<string, string> Helps = new();
    public string Name;
    public string Help;

    public SCommand(string name, string help = "") : base(2)
    {
        this.Name = name;
        this.Help = help;
    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        instance = Activator.CreateInstance(type);
        base.Handle(type, instance, mod, new object[] { this.Commands, this.Helps, this.Name });

        mod.Helper.ConsoleCommands.Add(
            name: this.Name,
            documentation: this.GetHelp(),
            callback: (s, args) => this.CallCommand(args)
        );
    }

    private string GetHelp(string? subCommand = null)
    {
        if (subCommand is not null && this.Helps?[subCommand] is not null)
        {
            return this.Helps[subCommand];
        }
        if (this.Helps is null)
        {
            Log.Error("Attempting to look at null Helps dictionary");
            return "";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append($"{this.Name}: {this.Help}\n");
        foreach (string sub in this.Helps.Keys)
        {
            string help = this.Helps[sub];
            sb.Append($"\t{help}\n\n");
        }

        return sb.ToString();
    }

    private void CallCommand(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            Log.Info(this.GetHelp());
            return;
        }

        if (args[0].Equals("help", StringComparison.InvariantCultureIgnoreCase)
            || args[0].Equals("-h", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length > 1 && this.Helps.ContainsKey(args[1]))
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
            this.Commands[args[0]].Invoke(args[1..]);
        }
        catch (Exception e)
        {
            Log.Info(this.GetHelp(args[0]));
            Log.Trace($"Args are:{string.Join(" ", args)}");
            Log.Trace(e.ToString());
            return;
        }
    }




    /// <summary>
    /// A single command. This property converts the method into a command if possible. Typed arguments
    /// will be converted from strings as best as possible. Supports optional numbered parameters, and
    /// variadic arguments (params parameter), and will deal with having more or fewer command line arguments
    /// compared to method arguments. Optionally includes help text.
    /// This attribute, combined with the SCommand attribute, creates a two-command structure to seperate mod
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
    public class Command : MethodHandler
    {
        public string Help;

        public Command(string help = "")
        {
            this.Help = help;
        }

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
            Dictionary<string, Action<string[]>> Commands = (Dictionary<string, Action<string[]>>)args[0];
            Dictionary<string, string> Helps = (Dictionary<string, string>)args[1];
            string command = (string)args[2];


            string subCommand = method.Name.ToSnakeCase();

            Commands.Add(subCommand, (string[] args) =>
            {
                List<object> commandArgs = new();

                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parameter = method.GetParameters()[i];

                    if (parameter.GetCustomAttribute(typeof(ParamArrayAttribute), false) is not null)
                    {
                        for (int j = i; j < (args?.Length ?? 0); j++)
                        {
                            commandArgs.Add(ParseArg(args?[j], parameter));
                        }
                    }
                    else
                    {
                        commandArgs.Add(ParseArg(args?[i], parameter));
                    }
                }

                method.Invoke(instance, commandArgs.ToArray());

            });

            StringBuilder help = new StringBuilder();
            help.Append($"{command} {subCommand}");
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                string dotDotDot = parameter.GetCustomAttribute<ParamArrayAttribute>() is not null ? "..." : "";
                if (parameter.IsOptional)
                {
                    help.Append($" [{parameter.Name}{dotDotDot}]");
                }
                else
                {
                    help.Append($" <{parameter.Name}{dotDotDot}>");
                }
            }
            help.Append($"\n\t\t{this.Help}");

            Helps.Add(subCommand, help.ToString());
        }

        private static object ParseArg(string? arg, ParameterInfo parameter)
        {
            if (arg == null)
            {
                return Type.Missing;
            }
            else if (parameter.ParameterType == typeof(string))
            {
                return arg;
            }
            else if (parameter.ParameterType == typeof(int))
            {
                return int.Parse(arg);
            }
            else if (parameter.ParameterType == typeof(double) || parameter.ParameterType == typeof(float))
            {
                return float.Parse(arg);
            }
            else if (parameter.ParameterType == typeof(bool))
            {
                return bool.Parse(arg);
            }
            else
            {
                return Type.Missing;
            }
        }
    }
}
