using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.FishingOverhaul.Configs {
    public class ConfigStrings {

        public string StreakDisplay { get; set; } = "Streak: {0}";
        public string UnawareFish { get; set; } = "The fish is unaware of your presence. ({0:P} easier catch)";

        public string PossibleFish { get; set; } = "Fish you can catch right now: {0}";
        public string NoPossibleFish { get; set; } = "You can't catch any fish there right now!";

        public string LostStreak { get; set; } = "You lost your perfect fishing streak of {0}.";
        public string WarnStreak { get; set; } = "Catch the treasure and the fish to keep your streak of {0}!";
        public string KeptStreak { get; set; } = "You kept your perfect fishing streak!";
    }
}
