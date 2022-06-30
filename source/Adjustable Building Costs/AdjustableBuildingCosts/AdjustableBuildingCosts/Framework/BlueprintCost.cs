/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MiguelLucas/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdjustableBuildingCosts.Framework
{
    internal class BlueprintCost
    {
        /// <summary>The color to use for the <see cref="Farmer.fishingSkill"/> skill.</summary>
        public int GoldCost { get; set; } = 1000;

        public List<ItemAmount> Items { get; set; } = new List<ItemAmount>();

        public int DaysToBuild { get; set; } = 2;

        public string getFormattedBlueprintCost()
        {
            string formattedCost = "";

            foreach (ItemAmount item in Items) {
                formattedCost += item.ItemID + " " + item.Amount + " ";
            }

            formattedCost.Trim();

            return formattedCost;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
