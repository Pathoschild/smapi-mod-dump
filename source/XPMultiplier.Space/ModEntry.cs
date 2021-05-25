/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nofilenamed/XPMultiplier.Space
**
*************************************************/

using Harmony;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;

namespace XPMultiplier.Space
{
    public sealed class ModConfig
    {
        public Dictionary<string, byte> CustomSkill = new Dictionary<string, byte>()
        {
            { "spacechase0.Cooking", 1 }
        };

        public KeybindList ReloadKey { get; set; } = new KeybindList(SButton.F12);
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
                    original: AccessTools.Method(typeof(Skills), nameof(Skills.AddExperience)),
                    prefix: new HarmonyMethod(typeof(SpaceXP), nameof(SpaceXP.Prefix)));


            }
            catch (Exception e)
            {
                Monitor.Log(e.Message, LogLevel.Error);
            }

            helper.ConsoleCommands.Add("list_skills", "list all skills registered to SpaceCore.", ShowSkills);
        }

        private void ShowSkills(string command, string[] args)
        {
            Monitor.Log(Skills.GetSkillList().Join(x => Skills.GetSkill(x).Id, "\n"), LogLevel.Info);
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
            SpaceXP.Skills = Config.CustomSkill;

        }

        public ModConfig Config;

    }

    public static class SpaceXP
    {
        public static Dictionary<string, byte> Skills { get; set; } = new Dictionary<string, byte>();

        public static void Prefix(string skillName, ref int amt)
        {
            if (amt <= 0)
                return;

            if (Skills.ContainsKey(skillName))
            {
                amt *= Skills[skillName];
            }
        }
    }
}