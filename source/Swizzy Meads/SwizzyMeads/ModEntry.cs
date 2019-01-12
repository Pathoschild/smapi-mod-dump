
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
