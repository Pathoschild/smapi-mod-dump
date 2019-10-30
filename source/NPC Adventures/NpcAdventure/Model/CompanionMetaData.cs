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

            if (parts.Length != 7)
            {
                throw new ArgumentOutOfRangeException("Companion disposition metadata arguments is invalid!");
            }

            this.Recruitable = parts[0];
            this.Profession = parts[1];
            this.Temperament = parts[2];
            this.Availability = parts[3];
            this.MinimumHearts = int.Parse(parts[4]);
            this.Price = int.Parse(parts[5]);
            this.Sword = int.Parse(parts[6]);
        }

        public string Recruitable { get; private set; }
        public string Profession { get; private set; }
        public string Temperament { get; private set; }
        public string Availability { get; private set; }
        public int MinimumHearts { get; private set; }
        public int Price { get; private set; }
        public int Sword { get; private set; }
    }
}
