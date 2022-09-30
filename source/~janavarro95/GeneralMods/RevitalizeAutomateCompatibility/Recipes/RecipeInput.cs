/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;

namespace Omegasis.RevitalizeAutomateCompatibility.Recipes
{
    public class RecipeInput
    {

        public Enums.SDVObject sdvItem;
        public Enums.SDVBigCraftable sdvBigCraftable;
        public string revitalizeItemId;

        public int amountRequired;

        public RecipeInput()
        {

        }

        public RecipeInput(Enums.SDVObject obj, int AmountRequired)
        {
            this.sdvItem = obj;
            this.amountRequired = AmountRequired;
        }

        public RecipeInput(Enums.SDVBigCraftable obj, int AmountRequired)
        {
            this.sdvBigCraftable = obj;
            this.amountRequired = AmountRequired;
        }

        public RecipeInput(string ObjectId, int AmountRequired)
        {
            this.revitalizeItemId = ObjectId;
            this.amountRequired = AmountRequired;
        }

    }
}
