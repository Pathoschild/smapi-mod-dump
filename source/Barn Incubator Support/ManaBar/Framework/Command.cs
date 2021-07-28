/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using SpaceShared;

namespace ManaBar.Framework
{
    internal static class Command
    {
        /*********
        ** Fields
        *********/
        private static readonly Dictionary<string, Action<string[]>> Commands = new();


        /*********
        ** Public methods
        *********/
        public static void Register(string name, Action<string[]> callback)
        {
            // TODO: Load documentation from a file.
            Mod.Instance.Helper.ConsoleCommands.Add(name, "TO BE IMPLEMENTED", Command.HandleCommand);
            Command.Commands.Add(name, callback);
        }


        /*********
        ** Private methods
        *********/
        private static void HandleCommand(string cmd, string[] args)
        {
            try
            {
                Command.Commands[cmd].Invoke(args);
            }
            catch (Exception e)
            {
                Log.Error("Error running command.");
                Log.Debug("Exception: " + e);
            }
        }
    }
}
