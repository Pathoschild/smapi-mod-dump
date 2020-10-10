/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aleksanderwaagr/Stardew-ChangeProfessions
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ChangeProfessions
{
    public class ProfessionManager
    {
        private readonly List<ProfessionSet> _professionSets;

        public ProfessionManager(IModHelper modHelper)
        {
            _professionSets = modHelper.Data.ReadJsonFile<Data>("data.json").ProfessionSets;
        }

        public ProfessionSet GetProfessionSet(int professionId)
        {
            return _professionSets.Single(x => x.Ids.Contains(professionId));
        }

        public void ChangeProfession(int fromProfessionId, int toProfessionId)
        {
            RemoveProfession(fromProfessionId);
            AddProfession(toProfessionId);

            if (!IsPrimaryProfession(fromProfessionId))
                return;

            RemoveSecondaryProfessions(fromProfessionId);

            if (HasUnlockedSecondaryProfession(fromProfessionId))
            {
                AddSecondaryProfession(toProfessionId);
            }
        }

        private bool IsPrimaryProfession(int professionId)
        {
            return GetProfessionSet(professionId).IsPrimaryProfession;
        }

        private void RemoveSecondaryProfessions(int primaryId)
        {
            var oldSecondarySet = GetSecondaryProfessionSet(primaryId);
            oldSecondarySet.Ids.ToList().ForEach(RemoveProfession);
        }

        private void AddSecondaryProfession(int primaryId)
        {
            var newSecondarySet = GetSecondaryProfessionSet(primaryId);
            AddProfession(newSecondarySet.Ids.First());
        }

        private bool HasUnlockedSecondaryProfession(int professionId)
        {
            var skillLevel = GetSkillLevel(professionId);
            return skillLevel == 10;
        }

        private int GetSkillLevel(int professionId)
        {
            if (professionId < 0) return 0;
            if (professionId <= 5) return Game1.player.FarmingLevel;
            if (professionId <= 11) return Game1.player.FishingLevel;
            if (professionId <= 17) return Game1.player.ForagingLevel;
            if (professionId <= 23) return Game1.player.MiningLevel;
            if (professionId <= 29) return Game1.player.CombatLevel;
            return 0;
        }

        private ProfessionSet GetSecondaryProfessionSet(int primaryId)
        {
            return _professionSets.Single(x => x.ParentId == primaryId);
        }

        private void AddProfession(int professionId)
        {
            Game1.player.professions.Add(professionId);
        }

        private void RemoveProfession(int professionId)
        {
            Game1.player.professions.Remove(professionId);
        }

    }
}
