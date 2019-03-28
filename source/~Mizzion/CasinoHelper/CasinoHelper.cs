using Harmony;
using System.Reflection;
using StardewModdingAPI;

namespace CasinoHelper
{
    public class CasinoHelper: Mod
    {
        //Config
        public ModConfig config;
        internal static IModHelper Modhelper;
        internal static IMonitor monitor;
        //CalicoJack Variables


        //Slots Variables
        //Entry
        public override void Entry(IModHelper helper)
        {
            this.config = Helper.ReadConfig<ModConfig>();
            Modhelper = helper;
            monitor = Monitor;
            //Set up harmony
            var harmony = HarmonyInstance.Create("mizzion.casinohelper");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
