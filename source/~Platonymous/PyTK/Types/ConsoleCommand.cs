/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;

namespace PyTK.Types
{
    public class ConsoleCommand
    {
        public string name;
        public string documentation;
        public Action<string, string[]> callback;

        public ConsoleCommand(string name, string documentation, Action<string,string[]> callback)
        {
            this.name = name;
            this.documentation = documentation;
            this.callback = callback;
        }

        public void trigger()
        {
            callback.Invoke(name, new string[] { });
        }

        public void register()
        {
            PyTKMod._helper.ConsoleCommands.Add(name, documentation, callback);
        }
    }
}
