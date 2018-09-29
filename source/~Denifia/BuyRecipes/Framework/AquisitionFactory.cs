using Denifia.Stardew.BuyRecipes.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denifia.Stardew.BuyRecipes.Framework
{
    public static class AquisitionFactory
    {

        public static IRecipeAquisitionConditions GetConditions(string conditions)
        {
            return new DefaultRecipeAquisition();
        }

    }
}
