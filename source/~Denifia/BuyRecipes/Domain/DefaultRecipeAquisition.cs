using System.Text;
using System.Threading.Tasks;

namespace Denifia.Stardew.BuyRecipes.Domain
{
    public class DefaultRecipeAquisition : BaseRecipeAquisition, IRecipeAquisitionConditions
    {
        public int Cost => 1000;

        public DefaultRecipeAquisition() { }

        public DefaultRecipeAquisition(string data) : base(data) { }

        public bool AcceptsConditions(string condition)
        {
            return true;
        }
    }
}
