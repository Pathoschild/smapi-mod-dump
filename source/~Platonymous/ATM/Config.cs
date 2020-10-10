/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace ATM
{
    class Config
    {
        public string Map { get; set; } = "Town";
        public int[] Position { get; set; } = new int[] { 32, 55 };
        public bool Credit { get; set; } = true;
        public string CreditLine { get; set; } = "250 + (value / 2)";
        public float CreditInterest { get; set; } = 0.05f;
        public float GainInterest { get; set; } = 0.01f;
        public string[] CreditAdjustment { get; set; } = new[] { "spring", "summer","fall","winter" };
    }
}
