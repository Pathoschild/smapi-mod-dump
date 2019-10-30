using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMixedSeeds.Config
{
    public class Seed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Seasons { get; set; }

        public Seed(int id, string name, string[] seasons)
        {
            Id = id;
            Name = name; 
            Seasons = seasons;
        }
    }
}
