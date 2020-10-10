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
