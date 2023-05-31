/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Weapons;
using Archery.Framework.Utilities;
using Object = StardewValley.Object;

namespace Archery.Framework.Objects.Items
{
    internal class Arrow : InstancedObject
    {
        private const int ARROW_BASE_ID = 590;

        public static Object CreateInstance(AmmoModel ammoModel, int stackCount = 1)
        {
            var arrow = new Object(ARROW_BASE_ID, stackCount);
            arrow.modData[ModDataKeys.AMMO_FLAG] = ammoModel.Id;

            return arrow;
        }

        public static Object CreateRecipe(AmmoModel ammoModel)
        {
            var recipe = CreateInstance(ammoModel);
            recipe.modData[ModDataKeys.RECIPE_FLAG] = true.ToString();

            return recipe;
        }
    }
}
