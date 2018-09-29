using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionAdjustments.Overrides;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ProfessionAdjustments
{
    public class ProfessionAdjustments : Mod, IAssetEditor
    {
        internal static ProfessionAdjustments instance;
        private HarmonyInstance harmony;

        public ProfessionAdjustments()
        {
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            // change 40% to 10% in Artisan description
            if (asset.AssetNameEquals("Strings/UI"))
                return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/UI"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                if (data.TryGetValue("LevelUp_ProfessionDescription_Artisan", out string str))
                {
                    data["LevelUp_ProfessionDescription_Artisan"] = str.Replace("40%", "10%");
                }
            }
        }

        public override void Entry(IModHelper helper)
        {
            instance = this;

            this.harmony = HarmonyInstance.Create("connorsk.ProfessionAdjustments");

            Type game1CompilerType = null;
            foreach (var t in typeof(Game1).Assembly.GetTypes()) {

                this.Monitor.Log(t.FullName);
                
                if (t.FullName == "StardewValley.Object")
                {
                    game1CompilerType = t;
                    this.Monitor.Log("Found Object.cs");
                }
            }

            MethodInfo sellToStorePriceMethod = null;
            foreach (var m in game1CompilerType.GetRuntimeMethods())
            {
                if (m.FullDescription().Contains("sellToStorePrice"))
                {
                    sellToStorePriceMethod = m;
                    this.Monitor.Log("Found sellToStorePrice in Object.cs");
                }
            }

            this.doTranspiler(sellToStorePriceMethod, typeof(SellToStorePriceMethodHook).GetMethod("Transpiler"));
        }

        private void doTranspiler(Type origType, string origMethod, Type newType)
        {
            doTranspiler(origType.GetMethod(origMethod), newType.GetMethod("Transpiler"));
        }
        private void doTranspiler(MethodInfo orig, MethodInfo transpiler)
        {
            try
            {
                this.Monitor.Log("Doing transpiler patch...");
                this.harmony.Patch(orig, null, null, new HarmonyMethod(transpiler));
            }
            catch (Exception e)
            {
                this.Monitor.Log("Exception doing transpiler patch");
            }
        }
    }
}