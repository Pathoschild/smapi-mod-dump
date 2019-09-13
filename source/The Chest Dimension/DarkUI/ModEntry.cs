using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DarkUI
{
    public class ModEntry : Mod
    {
        IModHelper h;
        public override void Entry(IModHelper helper)
        {
            h = helper;
            h.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
            var harmony = HarmonyInstance.Create("zazizu.DarkUI");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void Display_RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            if (!(h.Reflection.GetField<Color>(typeof(Game1), "textColor").GetValue() == new Color(255, 255, 255)))
            {
                writeColors(h);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e) => writeColors(h);

        void writeColors(IModHelper helper)
        {
            helper.Reflection.GetField<Color>(typeof(Game1), "textColor").SetValue(new Color(255, 255, 255));
            helper.Reflection.GetField<Color>(typeof(Game1), "textShadowColor").SetValue(new Color(32, 32, 32));
        }
    }
}