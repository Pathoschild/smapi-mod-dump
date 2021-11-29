/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace HelpForHire.Chores
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Services;
    using StardewValley;
    using StardewValley.Locations;

    internal class RepairFences : GenericChore
    {
        public RepairFences(ServiceManager serviceManager)
            : base("repair-fences", serviceManager)
        {
        }

        protected override bool DoChore()
        {
            var fencesRepaired = false;

            foreach (var fence in RepairFences.GetFences())
            {
                fence.repair();
                fencesRepaired = true;
            }

            return fencesRepaired;
        }

        protected override bool TestChore()
        {
            return RepairFences.GetFences().Any();
        }

        private static IEnumerable<Fence> GetFences()
        {
            var locations = Game1.locations.AsEnumerable();

            locations = locations.Concat(
                from location in Game1.locations.OfType<BuildableGameLocation>()
                from building in location.buildings
                where building.indoors.Value is not null
                select building.indoors.Value
            );

            return
                from location in locations
                from fence in location.Objects.Values.OfType<Fence>()
                where !(fence.getHealth() >= fence.maxHealth.Value)
                select fence;
        }
    }
}