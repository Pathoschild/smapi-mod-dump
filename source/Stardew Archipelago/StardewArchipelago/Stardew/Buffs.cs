/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewArchipelago.Stardew
{
    public class ConsumableBuff
    {
        public const int FARMING = 0;
        public const int FISHING = 1;
        public const int MINING = 2;
        public const int DIGGING = 3;
        public const int LUCK = 4;
        public const int FORAGING = 5;
        public const int CRAFTING = 6;
        public const int MAX_ENERGY = 7;
        public const int MAGNETISM = 8;
        public const int SPEED = 9;
        public const int DEFENSE = 10;
        public const int ATTACK = 11;
    }

    public enum Buffs
    {
        Farming = 0,
        Fishing = 1,
        Mining = 2,
        Luck = 4,
        Foraging = 5,
        Crafting = 6,
        MaxStamina = 7,
        MagneticRadius = 8,
        Speed = 9,
        Defense = 10,
        Attack = 11,
        GoblinsCurse = 12,
        Slimed = 13,
        EvilEye = 14,
        ChickenedOut = 15,
        Tipsy = 17,
        Fear = 18,
        Frozen = 19,
        WarriorEnergy = 20,
        YobaBlessing = 21,
        AdrenalineRush = 22,
        AvoidMonsters = 23,
        SpawnMonsters = 24,
        Nauseous = 25,
        Darkness = 26,
        Weakness = 27,
        SquidInkRavioli = 28,
        Full = 6,
        Quenched = 7,
        TotalNumberOfBuffableAttriutes = 12,
    }
}
