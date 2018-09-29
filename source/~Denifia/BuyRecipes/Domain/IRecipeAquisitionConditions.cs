namespace Denifia.Stardew.BuyRecipes.Domain
{
    public interface IRecipeAquisitionConditions
    {
        bool AcceptsConditions(string condition);
        int Cost { get; }
    }
}
