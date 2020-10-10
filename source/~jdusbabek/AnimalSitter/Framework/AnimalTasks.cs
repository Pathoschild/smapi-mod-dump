/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewLib;

namespace AnimalSitter.Framework
{
    internal class AnimalTasks : IStats
    {
        /*********
        ** Accessors
        *********/
        public int AnimalsPet { get; set; } = 0;
        public int TrufflesHarvested { get; set; } = 0;
        public int ProductsHarvested { get; set; } = 0;
        public int Aged { get; set; } = 0;
        public int Fed { get; set; } = 0;
        public int MaxHappiness { get; set; } = 0;
        public int MaxFriendship { get; set; } = 0;

        public int NumActions { get; set; } = 0;
        public int TotalCost { get; set; } = 0;


        /*********
        ** Public methods
        *********/
        public int GetTaskCount()
        {
            return this.AnimalsPet + this.TrufflesHarvested + this.ProductsHarvested + this.Aged + this.Fed + this.MaxHappiness + this.MaxFriendship;
        }

        public bool JustGathering()
        {
            return (this.AnimalsPet + this.Aged + this.Fed + this.MaxHappiness + this.MaxFriendship) == 0 && this.GetTaskCount() > 0;
        }

        public IDictionary<string, object> GetFields()
        {
            return typeof(AnimalTasks)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(this));
        }
    }
}
