/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using SpaceShared;
using System;
using System.Collections.Generic;

namespace SpaceCore
{
    public static class Command
    {
        private static readonly Dictionary<string, Action<string[]>> commands = new Dictionary< string, Action< string[] > >();

        internal static void register( string name, Action< string[] > callback )
        {
            // TODO: Load documentation from a file.
            SpaceCore.instance.Helper.ConsoleCommands.Add(name, "TO BE IMPLEMENTED", doCommand);
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
