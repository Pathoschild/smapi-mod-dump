/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using MiniBars.Framework.Rendering;
using StardewModdingAPI;
using System.Threading;

namespace MiniBars.Framework
{
    public class Commands
    {
        private static IMonitor Monitor = ModEntry.instance.Monitor;

        public static void Theme(string command, string[] args)
        {
            if (args.Length <= 0) return;
            if (args[0] == "1") ModEntry.config.Bars_Theme = 1;
            else if (args[0] == "2") ModEntry.config.Bars_Theme = 2;
            else
            {
                Monitor.Log("Choose the theme between 1 and 2.", LogLevel.Info);
                return;
            }
            Textures.LoadTextures();
            ModEntry.instance.Helper.WriteConfig(ModEntry.config);
            Monitor.Log("Theme changed.", LogLevel.Info);
        }

        public static void ShowFullLife(string command, string[] args)
        {
            if (args.Length <= 0) return;
            if (args[0] == "true") ModEntry.config.Show_Full_Life = true;
            else if (args[0] == "false") ModEntry.config.Show_Full_Life = false;
            ModEntry.instance.Helper.WriteConfig(ModEntry.config);
            Monitor.Log("Show healthbars when enemy full life changed.", LogLevel.Info);
        }

        public static void RangeVerification(string command, string[] args)
        {
            if (args.Length <= 0) return;
            if (args[0] == "true") ModEntry.config.Range_Verification = true;
            else if (args[0] == "false") ModEntry.config.Range_Verification = false;
            ModEntry.instance.Helper.WriteConfig(ModEntry.config);
            Monitor.Log("Range verification changed.", LogLevel.Info);
        }
    }
}
