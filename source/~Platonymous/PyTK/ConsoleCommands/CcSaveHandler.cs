/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using PyTK.CustomElementHandler;
using PyTK.Types;

namespace PyTK.ConsoleCommands
{
    public static class CcSaveHandler
    {
        public static ConsoleCommand cleanup()
        {
            return new ConsoleCommand("pytk_cleanup", "Removes all custom element leftovers", (s, p) => SaveHandler.Cleanup());
        }

        public static ConsoleCommand savecheck()
        {
            return new ConsoleCommand("pytk_savecheck", "Checks all savefiles for XML errors", (s, p) => PyUtils.checkAllSaves());
        }
    }
}
