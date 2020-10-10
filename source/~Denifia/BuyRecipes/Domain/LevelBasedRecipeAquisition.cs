/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

namespace Denifia.Stardew.BuyRecipes.Domain
{
    public class LevelBasedRecipeAquisition : BaseRecipeAquisition, IRecipeAquisitionConditions
    {
        private int _playerLevel;
        public int Cost => GetCost();

        public LevelBasedRecipeAquisition() { }

        public LevelBasedRecipeAquisition(string data) : base(data)
        {
            var dataParts = data.Split(' ');
            _playerLevel = int.Parse(dataParts[1]);
        }

        public bool AcceptsConditions(string condition)
        {
            return condition.StartsWith("l ");
        }

        private int GetCost()
        {
            if (_playerLevel == 100) return 1750;
            return _playerLevel * 75;
        }
    }
}
