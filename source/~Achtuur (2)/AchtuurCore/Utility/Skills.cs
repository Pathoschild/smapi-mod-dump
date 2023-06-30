/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

namespace AchtuurCore.Utility;

public static class Skills
{
    public static string GetSkillNameFromId(int skill_id)
    {
        switch (skill_id)
        {
            case 0: return "Farming";
            case 1: return "Fishing";
            case 2: return "Foraging";
            case 3: return "Mining";
            case 4: return "Combat";
            case 5: return "Luck";
            default: return "InvalidID";
        }
    }

    public static int GetSkillIdFromName(string skill_name)
    {
        switch (skill_name)
        {
            case "Farming": return 0;
            case "Fishing": return 1;
            case "Foraging": return 2;
            case "Mining": return 3;
            case "Combat": return 4;
            case "Luck": return 5;
            default: return -1;
        }
    }
}
