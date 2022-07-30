/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;

namespace WhyAreMacs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var val in Enum.GetValues<Environment.SpecialFolder>())
            {
                Console.WriteLine($"For specialfolder {val}, got {Environment.GetFolderPath(val)}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
