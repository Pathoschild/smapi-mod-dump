/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jangofett4/FishingTreasureSpawner
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTreasureSpawner
{
    public class ModConfig
    {
        public SButton FZ0Keymap { get; set; } = SButton.NumPad0;
        public SButton FZ1Keymap { get; set; } = SButton.NumPad1;
        public SButton FZ2Keymap { get; set; } = SButton.NumPad2;
        public SButton FZ3Keymap { get; set; } = SButton.NumPad3;
        public SButton FZ4Keymap { get; set; } = SButton.NumPad4;
        public SButton FZ5Keymap { get; set; } = SButton.NumPad5;
        public SButton FZ6Keymap { get; set; } = SButton.NumPad6;
        public SButton FZ7Keymap { get; set; } = SButton.NumPad7;
        public SButton FZ8Keymap { get; set; } = SButton.NumPad8;
        public SButton FZ9Keymap { get; set; } = SButton.NumPad9;

        public SButton BruteForceKey { get; set; } = SButton.B;

        public bool BruteForceEnabled { get; set; } = false;
        public string BruteForceItem { get; set; } = "work boots";
        public bool BruteForceAllow { get; set; } = true;
        public int BruteForceFishingZone { get; set; } = 5;
        public int BruteForceTries { get; set; } = 1;
        public string BruteForceReportFile { get; set; } = "bruteforce.csv";
    }
}
