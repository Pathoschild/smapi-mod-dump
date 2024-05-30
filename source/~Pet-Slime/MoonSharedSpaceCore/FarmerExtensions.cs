/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using SpaceCore;
using StardewValley;

namespace MoonShared
{
    static class FarmerExtensions
    {
        public static bool HasCustomPrestigeProfession(this Farmer player, Skills.Skill.Profession profession)
        {
            return player.professions.Contains(profession.GetVanillaId() + 100);
        }
    }
}
