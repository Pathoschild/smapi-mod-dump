/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Linq;

namespace TransparentObjects
{
    public class ModEntry : Mod
    {
        
        public static ModConfig Config;
        public static IMonitor SMonitor;
        public static IModHelper SHelper;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;

            SMonitor = Monitor;

            SHelper = helper;

            ObjectPatches.Initialize(Monitor, helper, Config);

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
               prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Object_draw_Prefix))
            );

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == Config.ToggleButton && !Config.RequireButtonDown)
            {
                Config.MakeTransparent = !Config.MakeTransparent;
                Helper.WriteConfig(Config);
                Monitor.Log($"Toggled transparency to {Config.MakeTransparent}");
            }
        }

        public static bool IsOff()
        {
            return (Config.RequireButtonDown && !SHelper.Input.IsDown(Config.ToggleButton)) || (!Config.RequireButtonDown && !Config.MakeTransparent);
        }
        public static bool IsAllowed(string name)
        {
            return (Config.Exceptions.Length > 0 && !Config.Exceptions.Contains(name)) || (Config.Allowed.Length > 0 && Config.Allowed.Contains(name));
        }
    }
}
