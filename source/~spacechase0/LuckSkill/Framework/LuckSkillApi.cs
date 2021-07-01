/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace LuckSkill.Framework
{
    public class LuckSkillApi : ILuckSkillApi
    {
        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public int FortunateProfessionId => Mod.FortunateProfessionId;

        /// <inheritdoc />
        public int PopularHelperProfessionId => Mod.PopularHelperProfessionId;

        /// <inheritdoc />
        public int LuckyProfessionId => Mod.LuckyProfessionId;

        /// <inheritdoc />
        public int UnUnluckyProfessionId => Mod.UnUnluckyProfessionId;

        /// <inheritdoc />
        public int ShootingStarProfessionId => Mod.ShootingStarProfessionId;

        /// <inheritdoc />
        public int SpiritChildProfessionId => Mod.SpiritChildProfessionId;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public IDictionary<int, IProfession> GetProfessions()
        {
            return Mod.Instance.GetProfessions();
        }
    }
}
