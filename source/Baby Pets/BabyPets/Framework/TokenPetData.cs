/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DelphinWave/BabyPets
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPets.Framework
{
    internal class TokenPetData
    {
        public int adultAge;

        public string token;
        public string isBaby = "false";

        public int? bday;
        public string? petType;
        public string? petBreed;

        /// <summary>
        /// Constructor for Token Pet Data framework
        /// </summary>
        /// <param name="token">String name of token to be registered</param>
        /// <param name="petType">String value of Pet.petType</param>
        /// <param name="petBreed">String value of Pet.whichBreed</param>
        /// <param name="adultAge">Int value from config file</param>
        public TokenPetData(string token, string? petType, string? petBreed, int adultAge) {
            this.token = token;
            
            this.petType = petType;
            this.petBreed = petBreed;
            this.adultAge = adultAge;
        }

        public void SetBday(string bday)
        {
            var intBday = Int32.Parse(bday);
            if (this.bday == null)
            {
                this.isBaby = IsBaby(intBday).ToString();
                this.bday = intBday;
            }
            else // If more than one pet with same type and breed, use eldest pet bday
            {
                if(this.bday < intBday)
                    this.isBaby = IsBaby(this.bday.Value).ToString();
                else
                {
                    this.isBaby = IsBaby(intBday).ToString();
                }
            }
            
        }

        private bool IsBaby(int bday)
        {
            //ModEntry.SMonitor.Log($"IsBaby? {(SDate.Now().DaysSinceStart - bday) < adultAge}", LogLevel.Info);
            //ModEntry.SMonitor.Log($"Adult age: {adultAge}", LogLevel.Info);
            return (SDate.Now().DaysSinceStart - bday) < adultAge;
        }
    }
}
