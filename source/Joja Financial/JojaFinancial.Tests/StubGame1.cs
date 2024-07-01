/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValleyMods.JojaFinancial;
using static JojaFinancial.Tests.StubModHelper;

namespace JojaFinancial.Tests
{
    public class StubGame1
        : VGame1
    {
        private readonly StubModHelper stubHelper;

        public StubGame1(StubModHelper stubHelper)
        {
            this.stubHelper = stubHelper;
        }

        public override WorldDate Date { get; } = new WorldDate(1, Season.Spring, 1);

        public override string PlayerName => "Testy";

        public override Random Random { get; } = new Random();

        public Dictionary<string, string> ModData { get; } = new Dictionary<string, string>();

        public override int PlayerMoney { get; set; }

        public void AdvanceDay()
        {
            ((StubGameLoopEvents)this.stubHelper.Events.GameLoop).RaiseEndOfDay();

            if (this.Date.DayOfMonth < WorldDate.DaysPerMonth)
            {
                this.Date.DayOfMonth += 1;
            }
            else
            {
                this.Date.DayOfMonth = 1;
                if (this.Date.Season == Season.Winter)
                {
                    this.Date.Season = Season.Spring;
                    this.Date.Year += 1;
                }
                else
                {
                    this.Date.Season = (Season)(1 + (int)this.Date.Season);
                }
            }
        }

        public void AdvanceDay(WorldDate newDate)
        {
            Assert.IsTrue(newDate >= this.Date, "Test is broken - advancing to a date that's already past.");
            while (this.Date < newDate)
            {
                this.AdvanceDay();
            }
        }

        public override string? GetPlayerModData(string modDataKey)
        {
            this.ModData.TryGetValue(modDataKey, out string? value);
            return value;
        }

        public override void SetPlayerModData(string modDataKey, string? value)
        {
            if (value is not null)
            {
                this.ModData[modDataKey] = value;
            }
            else
            {
                this.ModData.Remove(modDataKey);
            }
        }

        public const string RandoSaleObjectName = "Test Object";
        public const string RandoSaleObjectId = "Fake";
        public const int RandoSalePrice = 57;

        public override StardewValley.Object CreateObject(string itemId, int quantity)
            => new StubObject()
            {
                ItemId = RandoSaleObjectId,
                Name = RandoSaleObjectName,
                Stack = quantity,
                Price = itemId switch { "(F)1226" => 200000, "(F)1308" => 30000, _ => RandoSalePrice },
            };

        private class StubObject : StardewValley.Object
        {
            public override string DisplayName => this.Price switch { 200000 => "Furniture Catalog", 30000 => "Wallpaper Catalog", _ => RandoSaleObjectName };
        }
    }
}
