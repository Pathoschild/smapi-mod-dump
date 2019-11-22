using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Model
{
    class CompanionMetaData
    {
        public CompanionMetaData(string rawMetadata)
        {
            string[] parts = rawMetadata.Split('/');

            if (parts.Length < 6)
            {
                throw new ArgumentOutOfRangeException("Companion disposition metadata arguments is invalid!");
            }

            this.Recruitable = parts[0];
            this.PersonalSkills = new List<string>(parts[1].Split(' '));
            this.Availability = parts[2];
            this.MinimumHearts = int.Parse(parts[3]);
            this.Price = int.Parse(parts[4]);
            this.Sword = int.Parse(parts[5]);
        }

        public string Recruitable { get; private set; }
        public List<string> PersonalSkills { get; private set; }
        public string Availability { get; private set; }
        public int MinimumHearts { get; private set; }
        public int Price { get; private set; }
        public int Sword { get; private set; }
    }
}
