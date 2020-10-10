/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SwizzyStudios/SV-SwizzyMeads
**
*************************************************/


using Harmony;
using StardewModdingAPI;
using System.Reflection;


namespace SwizzyMeads
{
    internal class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("SwizzyStudios.SwizzyMeads");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
