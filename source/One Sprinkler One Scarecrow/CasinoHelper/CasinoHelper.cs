using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CasinoHelper.Injectors;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;

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
