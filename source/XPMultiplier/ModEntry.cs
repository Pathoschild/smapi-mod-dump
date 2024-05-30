/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nofilenamed/XPMultiplier
**
*************************************************/

using System;
using System.Reflection;

using HarmonyLib;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;

namespace XPMultiplier
{
    public sealed class ModConfig
    {
        public float General { get; set; } = 1;

        public float Combat { get; set; } = 1;

        public float Farming { get; set; } = 1;

        public float Fishing { get; set; } = 1;

        public float Mining { get; set; } = 1;

        public float Foraging { get; set; } = 1;

        public float Luck { get; set; } = 1;

        public KeybindList ReloadKey { get; set; } = new KeybindList(SButton.F9);
    }

    public sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            LoadConfig();

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            try
            {
                var harmony = new Harmony(ModManifest.UniqueID);

                MethodInfo prefix = AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience));

                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                    prefix: new HarmonyMethod(typeof(GainXP), nameof(GainXP.Prefix)));
            }
            catch (Exception e)
            {
                Monitor.Log(e.Message, LogLevel.Error);
            }
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs arg)
        {
            if (Config.ReloadKey.JustPressed())
            {
                LoadConfig();
                Monitor.Log("Reload Config", LogLevel.Trace);
            }
        }

        private void LoadConfig()
        {
            Config = Helper.ReadConfig<ModConfig>();

        }

        internal static ModConfig Config;

    }

    public static class GainXP
    {
        public static void Prefix(Farmer __instance, int which, ref int howMuch)
        {
            if (howMuch <= 0)
                return;

            if (ModEntry.Config.General <= 1)
            {
                switch (which)
                {
                    case 0:
                        Mul(ref howMuch, ModEntry.Config.Farming);
                        break;
                    case 1:
                        Mul(ref howMuch, ModEntry.Config.Fishing);
                        break;
                    case 2:
                        Mul(ref howMuch, ModEntry.Config.Foraging);
                        break;
                    case 3:
                        Mul(ref howMuch, ModEntry.Config.Mining);
                        break;
                    case 4:
                        Mul(ref howMuch, ModEntry.Config.Combat);
                        break;
                    case 5:
                        Mul(ref howMuch, ModEntry.Config.Luck);
                        break;
                }
                return;
            }

            Mul(ref howMuch, ModEntry.Config.General);
        }

        private static void Mul(ref int howMuch, float by)
        {
            howMuch = (int)(howMuch * by);
        }
    }
}