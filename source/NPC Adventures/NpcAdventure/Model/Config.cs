using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Model
{
    class Config
    {
        public SButton ChangeBuffButton { get; set; } = SButton.G;
        public int HeartThreshold { get; set; } = 5;
        public int HeartSuggestThreshold { get; set; } = 7;
        public bool ShowHUD { get; set; } = true;
        public bool EnableDebug { get; set; } = false;
        public bool AdventureMode { get; set; } = true;
        public bool AvoidSayHiToMonsters { get; set; } = true;
        public bool RequestsWithShift { get; set; } = false;
        public SButton RequestsShiftButton { get; set; } = SButton.LeftShift;
        public ExperimentalFeatures Experimental { get; set; } = new ExperimentalFeatures();

        public class ExperimentalFeatures
        {
            // From version 0.13.0 or 0.14.0 as stable option (enabled by default)
            public bool FightThruCompanion { get; set; } = false;
            // From version 0.13.0 or 0.14.0 as stable option (enabled by default), 
            // from version 0.16.0 will be removed and enabled hard
            public bool UseCheckForEventsPatch { get; set; } = false;
        }
    }
}
