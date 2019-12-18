using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMixedSeeds.Config
{
    /// <summary>The object that contains data for each seed</summary>
    public class Seed
    {
        /// <summary>The seed id.</summary>
        public int Id { get; set; }

        /// <summary>The seed name.</summary>
        public string Name { get; set; }

        /// <summary>The seasons that seed can be planted from mixed seeds</summary>
        public string[] Seasons { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="id">The seed id.</param>
        /// <param name="name">The seed name.</param>
        /// <param name="seasons">The seasons the crop can be found.</param>
        public Seed(int id, string name, string[] seasons)
        {
            Id = id;
            Name = name; 
            Seasons = seasons;
        }
    }
}
