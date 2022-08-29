/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace MobilePhone.Api
{
    /// <summary>
    /// !Updated Code. <br />
    /// One big change: API functionality updated to latest stability ('Blackhole') version. <br />
    /// All unused functions has been removed, to increase compatibility with next versions.
    /// </summary>
    public interface INpcAdventureModApi
    {
        bool CanRecruitCompanions();
        bool IsPossibleCompanion(NPC npc);
        bool CanRecruit(Farmer farmer, NPC npc);
        bool IsRecruited(NPC npc);
        bool RecruitCompanion(Farmer farmer, NPC npc);
    }
}