/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using StardewModdingAPI;

namespace MoonShared.Command
{
    public class CommandClassParser
    {
        private readonly ICommandHelper CommandHelper;
        private readonly object Command;
        private readonly CommandClass CommandClass;

        public CommandClassParser(ICommandHelper commandHelper, object command)
        {
            this.CommandHelper = commandHelper;
            this.Command = command;

            this.CommandClass = this.ParseClass();
        }

        private CommandClass ParseClass()
        {
            foreach (object attr in this.Command.GetType().GetCustomAttributes(false))
            {
                if (attr is CommandClass commandClass)
                {
                    return commandClass;
                }
            }
            Log.Error("Mod is attempting to parse command file, but is not using CommandClass attribute.  Please reach out to mod author to enable SMAPI commands.");
            return null;
        }

        public bool IsEnabled()
        {
            return this.CommandClass != null;
        }

        public bool ParseCommands()
        {
            if (!this.IsEnabled())
            {
                return false;
            }

            this.ParseAllMethods();

            return true;
        }

        private void ParseAllMethods()
        {
            foreach(MethodInfo methodInfo in this.Command.GetType().GetMethods())
            {
                foreach (Attribute attr in methodInfo.GetCustomAttributes(false))
                {
                    if (attr is CommandMethod commandMethod)
                    {
                        this.AddMethodCommand(methodInfo, commandMethod);
                    }
                }
            }
        }

        private void AddMethodCommand(MethodInfo methodInfo, CommandMethod commandMethod)
        {
            this.CommandHelper.Add(
                name: this.GetCommandName(methodInfo),
                documentation: commandMethod.Docs,
                callback: (s, args) => this.InvokeCommand(s, args, methodInfo)
            );
        }

        private string GetCommandName(MethodInfo methodInfo)
        {
            return this.CommandClass.Prefix + methodInfo.Name.ToLowerInvariant();
        }

        private void InvokeCommand(string command, string[] args, MethodInfo methodInfo)
        {
            List<object> commandArgs = new();
            if (args.Length > methodInfo.GetParameters().Length)
            {
                Log.Error($"{command} called with too many arguments: '{string.Join(' ', args)}'");
            }
            for (int i = 0; i < methodInfo.GetParameters().Length; i++)
            {
                ParameterInfo parameterInfo = methodInfo.GetParameters()[i];

                if (i < args.Length)
                {
                    string arg = args[i];

                    try
                    {
                        if (parameterInfo.ParameterType == typeof(string))
                        {
                            commandArgs.Add(arg);
                        }
                        else if (parameterInfo.ParameterType == typeof(int))
                        {
                            commandArgs.Add(int.Parse(arg));
                        }
                        else if (parameterInfo.ParameterType == typeof(double))
                        {
                            commandArgs.Add(double.Parse(arg));
                        }
                        else if (parameterInfo.ParameterType == typeof(bool))
                        {
                            commandArgs.Add(bool.Parse(arg));
                        }
                        else
                        {
                            Log.Error($"{command} cannot parse argument of type: {parameterInfo.ParameterType}.  Ask mod author about using a different type in command method signiture.");
                            return;
                        }
                    }
                    catch (FormatException)
                    {
                        Log.Error($"{command} expected argument {i} of type {parameterInfo.ParameterType}, but was '{arg}'");
                        return;
                    }
                }
                else
                {
                    if (!parameterInfo.IsOptional)
                    {
                        Log.Error($"{command} called with too few arguments: '{string.Join(' ', args)}'");
                        return;
                    }
                    else
                    {
                        commandArgs.Add(Type.Missing);
                    }
                    break;
                }
            }

            methodInfo.Invoke(this.Command, commandArgs.ToArray());
        }
    }


}
