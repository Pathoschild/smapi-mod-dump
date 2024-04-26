/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using OrnithologistsGuild.Content;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;

namespace OrnithologistsGuild.Models
{
    public class LifeList: Dictionary<string, LifeListEntry>
    {
        public int IdentifiedCount { get
            {
                return this.Values.Where(v => v.Identified).Count();
            } }

        public LifeListEntry GetOrAddEntry(BirdieDef birdieDef, out int? newAttribute)
        {
            newAttribute = null;

            if (!this.ContainsKey(birdieDef.UniqueID))
            {
               this.Add(birdieDef.UniqueID, new Models.LifeListEntry());
            }

            var lifeListEntry = this[birdieDef.UniqueID];
            if (lifeListEntry.Identified) return lifeListEntry; // Already added

            var attributes = Enumerable.Range(1, birdieDef.Attributes);
            var undiscoveredAttributes = attributes.Except(lifeListEntry.Sightings.Select(logEntry => logEntry.Attribute)).ToList();
            if (undiscoveredAttributes.Count == 1)
            {
                lifeListEntry.Identified = true;
            }

            newAttribute = Game1.random.ChooseFrom(undiscoveredAttributes);
            lifeListEntry.AddSighting(newAttribute.Value);

            SaveDataManager.Save();

            return lifeListEntry;
        }
    }

    public class LifeListEntry
    {
        public string ID;

        public bool Identified;
        public List<LifeListSighting> Sightings;

        public LifeListEntry()
        {
            this.Sightings = new List<LifeListSighting>();
        }

        public void AddSighting(int attribute)
        {
            this.Sightings.Add(new LifeListSighting()
            {
                DaysSinceStart = SDate.From(Game1.Date).DaysSinceStart,
                TimeOfDay = Game1.timeOfDay,

                LocationName = Game1.player.currentLocation.Name,

                Attribute = attribute
            });
        }
    }

    public class LifeListSighting
    {
        public int DaysSinceStart;
        public int TimeOfDay;

        public string LocationName;

        public int Attribute;
    }

}
