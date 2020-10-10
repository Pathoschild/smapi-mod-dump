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
