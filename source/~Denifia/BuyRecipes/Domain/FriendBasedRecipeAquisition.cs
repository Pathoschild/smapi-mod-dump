namespace Denifia.Stardew.BuyRecipes.Domain
{
    public class FriendBasedRecipeAquisition : BaseRecipeAquisition, IRecipeAquisitionConditions
    {
        private int _friendLevel;
        private string _friend;
        public int Cost => _friendLevel * 600;

        public FriendBasedRecipeAquisition() { }

        public FriendBasedRecipeAquisition(string data) : base(data)
        {
            var dataParts = data.Split(' ');
            _friend = dataParts[1];
            _friendLevel = int.Parse(dataParts[2]);
        }

        public bool AcceptsConditions(string condition)
        {
            return condition.StartsWith("f ");
        }
    }
}
