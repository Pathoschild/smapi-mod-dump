/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace EconomyMod.Model
{
    public class LotValue
    {
        Dictionary<string, Func<int>> Values = new Dictionary<string, Func<int>>();

        public ModConfig Config { get; }
        public int Sum
        {
            get
            {
                if (!Context.IsWorldReady) return 0;
                return Values.Select(c => c.Value()).Where(c => c > 0).Sum();
            }
        }

        public void Add(string Identifier, Func<int> value)
        {
            Values.Add(Identifier, value);
        }


        public void AddDefaultLotValue()
        {

            Add("GreenHouse",() =>
            {
                if (Util.Config.IncludeGreenhouseOnLotValue)
                    return Game1.getFarm().IsGreenhouse ? Util.Config.GreenhouseValue : -Util.Config.GreenhouseValue;
                return 0;
            });

            Add("LotValue",() =>
            {
                return Util.Config.LotValue;
            });

        }
    }
}
