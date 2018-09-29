namespace Denifia.Stardew.BuyRecipes.Domain
{
    public class SkillBasedRecipeAquisition : BaseRecipeAquisition, IRecipeAquisitionConditions
    {
        private int _skillLevel;
        private string _skill;
        public int Cost => _skillLevel * 900;

        public SkillBasedRecipeAquisition() { }

        public SkillBasedRecipeAquisition(string data) : base(data)
        {
            var dataParts = data.Split(' ');
            _skill = dataParts[1];
            _skillLevel = int.Parse(dataParts[2]);
        }

        public bool AcceptsConditions(string condition)
        {
            return condition.StartsWith("s ");
        }
    }
}
