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
