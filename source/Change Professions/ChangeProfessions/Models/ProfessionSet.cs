/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aleksanderwaagr/Stardew-ChangeProfessions
**
*************************************************/

namespace ChangeProfessions
{
    public class ProfessionSet
    {
        public int[] Ids { get; set; }
        public int? ParentId { get; set; }
        public bool IsPrimaryProfession => ParentId == null;
    }
}