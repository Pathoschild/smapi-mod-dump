using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace CustomKissingMod
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ModConfig ModConfig;
        public static readonly ModConfig DefaultConfig = new ModConfig();

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
        }
    }
}
