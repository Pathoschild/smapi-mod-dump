/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System;

namespace StardewTravelSkill;

internal static class ConsoleCommands
{
    internal static void Initialize(IModHelper helper)
    {
        helper.ConsoleCommands.Add(I18n.CmdSetlvl_Name(), I18n.CmdSetlevel_Desc(), StardewTravelSkill.ConsoleCommands.setLevel);
    }
    public static void setLevel(string command, string[] args)
    {
        try
        {
            int lvl = int.Parse(args[0]);
            // Reduce xp to 0
            Game1.player.AddCustomSkillExperience(ModEntry.TravelSkill, -Game1.player.GetCustomSkillExperience(ModEntry.TravelSkill));

            // Add xp equivalent to level
            if (lvl != 0)
            {
                Game1.player.AddCustomSkillExperience(ModEntry.TravelSkill, ModEntry.TravelSkill.ExperienceCurve[lvl - 1]);
            }

            ModEntry.Instance.Monitor.Log($"{Game1.player.Name}'s Travelling skill set to level {lvl}", LogLevel.Debug);
        }
        catch (Exception e)
        {
            ModEntry.Instance.Monitor.Log(I18n.CmdSetlevel_Errormsg(args[0]), LogLevel.Error);
        }
    }
}
