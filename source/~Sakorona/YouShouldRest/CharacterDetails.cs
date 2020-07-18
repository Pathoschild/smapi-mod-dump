using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TwilightShards.YouShouldRest
{
    public class CharacterDetails
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Manners { get; set; }
        public int SocialAnxiety { get; set; }
        public int Optimism { get; set; }
        public string HeartLevel { get; set; }
        public string HealthStatus { get; set; }
        public string StaminaStatus { get; set; }
        public int TimeOfDay { get; set; }
        public string SeasonDay { get; set; }
        public string SeasonDayFB { get; set; }

        public override string ToString()
        {
            string s = $"Talking to {Name}, with heartLevel { HeartLevel}, and StaminaStatus { StaminaStatus} and HealthStatus { HealthStatus} with Time Of Day { TimeOfDay}, seasonDay {SeasonDay}, fall back season {SeasonDayFB}.";

            s += Environment.NewLine;
            s += $"Disposition Data: Manners: {Manners}, Age: {Age}, Social Anxiety: {SocialAnxiety} and Optimism: {Optimism}";

            return s;
        }
    }
}
