/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Utils;
using System;
using System.Collections.Generic;

namespace NpcAdventure.Model
{
    class CompanionMetaData
    {
        public CompanionMetaData(string rawMetadata)
        {
            string[] parts = rawMetadata.Split('/');

            if (parts.Length < 5)
            {
                throw new ArgumentOutOfRangeException("Companion disposition metadata arguments is invalid!");
            }

            this.Recruitable = parts[0];
            this.PersonalSkills = new List<string>(parts[1].Split(' '));
            this.Availability = parts[2];
            this.MinimumHearts = int.Parse(parts[3]);
            this.Price = int.Parse(parts[4]);

            if (parts.Length >= 6)
                this.Sword = Helper.GetSwordId(parts[5]);
        }

        public string Recruitable { get; private set; }
        public List<string> PersonalSkills { get; private set; }
        public string Availability { get; private set; }
        public int MinimumHearts { get; private set; }
        public int Price { get; private set; }

        [Obsolete("Deprecated. Will be removed in version 1.0")]
        public int Sword { get; private set; } = -2;
    }
}
