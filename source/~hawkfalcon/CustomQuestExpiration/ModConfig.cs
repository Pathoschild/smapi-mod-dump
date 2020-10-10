/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

namespace CustomQuestExpiration {
    class ModConfig {
        public bool NeverExpires { get; set; } = false;
        public int DaysToExpiration { get; set; } = 3;
        public bool UsesQuestCategoryExpiration { get; set; } = true;

        public QuestCategoryExpiration CategoryExpiration { get; set; } = new QuestCategoryExpiration();

        internal class QuestCategoryExpiration {
            public int ItemDelivery { get; set; } = 3;
            public int Gathering { get; set; } = 3;
            public int Fishing { get; set; } = 2;
            public int SlayMonsters { get; set; } = 4;
        }
    }
}
