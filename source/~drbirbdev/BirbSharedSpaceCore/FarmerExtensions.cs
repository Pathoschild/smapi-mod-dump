/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewValley;
using static SpaceCore.Skills.Skill;

namespace BirbShared
{
    static class FarmerExtensions
    {
        public static bool HasProfession(this Farmer player, string profession, bool checkPrestiged = false)
        {
            Profession p = BirbSkill.KeyedProfessions?[profession];
            if (p is null)
            {
                return false;
            }
            return player.professions.Contains(p.GetVanillaId() + (checkPrestiged ? 100 : 0));
        }
    }
}
