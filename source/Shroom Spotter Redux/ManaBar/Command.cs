/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using SpaceShared;

namespace ManaBar
{
    public static class Command
    {
        private static Dictionary<string, Action<string[]>> commands = new Dictionary< string, Action< string[] > >();

        internal static void register( string name, Action< string[] > callback )
        {
            // TODO: Load documentation from a file.
            Mod.instance.Helper.ConsoleCommands.Add(name, "TO BE IMPLEMENTED", doCommand);
            commands.Add(name, callback);
        }

        private static void doCommand( string cmd, string[] args )
        {
            try
            {
                commands[cmd].Invoke(args);
            }
            catch (Exception e)
            {
                Log.error("Error running command.");
                Log.debug("Exception: " + e);
            }
        }
    }
}
