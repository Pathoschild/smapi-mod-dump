using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class WaterTheSlimesChore : BaseChore
    {
        private readonly List<SlimeHutch> _slimeHutches = new List<SlimeHutch>();
        private int _slimesWatered;

        public WaterTheSlimesChore(ChoreData choreData) : base(choreData) { }

        public override bool CanDoIt(bool today = true)
        {
            _slimesWatered = 0;
            _slimeHutches.Clear();

            _slimeHutches.AddRange(
                from building in Game1.getFarm().buildings
                where building.daysOfConstructionLeft.Value <= 0
                      && building.indoors.Value is SlimeHutch
                select building.indoors.Value as SlimeHutch);

            return _slimeHutches.Any();
        }

        public override bool DoIt()
        {
            foreach (var slimeHutch in _slimeHutches)
            {
                for (var index = 0; index < slimeHutch.waterSpots.Count; ++index)
                {
                    if (slimeHutch.waterSpots[index])
                        continue;

                    slimeHutch.waterSpots[index] = true;
                    ++_slimesWatered;
                }
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("SlimesWatered", GetSlimesWatered);
            tokens.Add("WorkDone", GetSlimesWatered);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        private string GetSlimesWatered() =>
            _slimesWatered.ToString(CultureInfo.InvariantCulture);

        private string GetWorkNeeded() =>
            _slimeHutches?.Sum(slimeHutch => slimeHutch.waterSpots.Count).ToString(CultureInfo.InvariantCulture);
    }
}
