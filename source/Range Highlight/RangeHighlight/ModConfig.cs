/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRangeHighlight
**
*************************************************/

// Copyright 2020 Jamie Taylor
using System;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace RangeHighlight {
    internal class ModConfig {
        public bool ShowJunimoRange { get; set; } = true;
        public bool ShowSprinklerRange { get; set; } = true;
        public bool ShowScarecrowRange { get; set; } = true;
        public bool ShowBeehouseRange { get; set; } = true;
        public bool ShowBombRange { get; set; } = true;

        public bool ShowOtherSprinklersWhenHoldingSprinkler { get; set; } = true;
        public bool ShowOtherScarecrowsWhenHoldingScarecrow { get; set; } = true;
        public bool ShowOtherBeehousesWhenHoldingBeehouse { get; set; } = false;

        public bool showHeldBombRange { get; set; } = true;
        public bool showPlacedBombRange { get; set; } = true;
        public bool showBombInnerRange { get; set; } = false;
        public bool showBombOuterRange { get; set; } = true;
        public KeybindList ShowAllRangesKey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);
        public KeybindList ShowSprinklerRangeKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public KeybindList ShowScarecrowRangeKey { get; set; } = KeybindList.ForSingle(SButton.O);
        public KeybindList ShowBeehouseRangeKey { get; set; } = KeybindList.ForSingle(SButton.H);
        public KeybindList ShowJunimoRangeKey { get; set; } = KeybindList.ForSingle(SButton.J);
        public bool hotkeysToggle { get; set; } = false;

        public Color JunimoRangeTint { get; set; } = Color.White * 0.7f;
        public Color SprinklerRangeTint { get; set; } = new Color(0.6f, 0.6f, 0.9f, 0.7f);
        public Color ScarecrowRangeTint { get; set; } = new Color(0.6f, 1.0f, 0.6f, 0.7f);
        public Color BeehouseRangeTint { get; set; } = new Color(1.0f, 1.0f, 0.6f, 0.7f);
        public Color BombRangeTint { get; set; } = new Color(1.0f, 0.5f, 0.5f, 0.6f);
        public Color BombInnerRangeTint { get; set; } = new Color(8.0f, 0.7f, 0.5f, 0.1f);
        public Color BombOuterRangeTint { get; set; } = new Color(9.0f, 0.7f, 0.5f, 0.8f);
    }
}
