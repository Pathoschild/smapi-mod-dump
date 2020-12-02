/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

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
        public bool AllowGainFriendship { get; set; } = true;
        public bool FightThruCompanion { get; set; } = true;
        // from version 0.16.0 will be removed and enabled hard
        public bool UseCheckForEventsPatch { get; set; } = true;
        public bool AllowEntryLockedCompanionHouse { get; set; } = true;
        public bool UseAsk2FollowCursor { get; set; } = true;
        public bool AllowLegacyContentPacks { get; set; } = false;

        public class ExperimentalFeatures
        {    
            // From version ??? as stable option (enabled by default)
            public bool UseSwimsuits { get; set; } = false;
        }
    }
}
