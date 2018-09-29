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
