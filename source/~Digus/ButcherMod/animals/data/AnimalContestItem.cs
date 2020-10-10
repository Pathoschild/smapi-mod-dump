/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalContestItem
    {
        public int EventId;
        [JsonProperty]
        private string _date;
        public List<string> Contenders;
        public string VincentAnimal;
        public string MarnieAnimal;
        public string PlayerAnimal;
        public string Winner;
        public long? ParticipantId;
        public AnimalContestScore FarmAnimalScore;

        [JsonIgnore]
        public SDate Date
        {
            get => _date == null ? null : new SDate(Convert.ToInt32(_date.Split(' ')[0]), _date.Split(' ')[1], Convert.ToInt32(_date.Split(' ')[2].Replace("Y", "")));
            set => _date = value?.ToString();
        }

        public AnimalContestItem(int eventId, SDate date, List<string> contenders, string vincentAnimal, string marnieAnimal)
        {
            EventId = eventId;
            Date = date;
            Contenders = contenders;
            VincentAnimal = vincentAnimal;
            MarnieAnimal = marnieAnimal;
        }
    }
}
