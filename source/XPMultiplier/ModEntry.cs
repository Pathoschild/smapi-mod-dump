/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nofilenamed/XPMultiplier
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;


namespace XPMultiplier
{
    public sealed class ModConfig
    {
        public byte General { get; set; } = 1;

        public byte Combat { get; set; } = 1;

        public byte Farming { get; set; } = 1;

        public byte Fishing { get; set; } = 1;

        public byte Mining { get; set; } = 1;

        public byte Foraging { get; set; } = 1;

        public byte Luck { get; set; } = 1;

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
                HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                    prefix: new HarmonyMethod(typeof(GainXP), nameof(GainXP.Prefix)));
            }
            catch(Exception e)
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

        public static ModConfig Config;

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
                        howMuch *= ModEntry.Config.Farming;
                        break;
                    case 1:
                        howMuch *= ModEntry.Config.Fishing;
                        break;
                    case 2:
                        howMuch *= ModEntry.Config.Foraging;
                        break;
                    case 3:
                        howMuch *= ModEntry.Config.Mining;
                        break;
                    case 4:
                        howMuch *= ModEntry.Config.Combat;
                        break;
                    case 5:
                        howMuch *= ModEntry.Config.Luck;
                        break;
                }

                return;
            }

            howMuch *= ModEntry.Config.General;
        }
    }
}