/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/StardewMods
**
*************************************************/

using System;

namespace FoodCravings
{
    public sealed class ModConfig
    {
        public bool useHangryMode { get; set; } = false;
        public int attackBuff { get; set; } = 2;
        public int defenseBuff { get; set; } = 2;
        public int speedBuff { get; set; } = 1;
        public int attackDebuff { get; set; } = -2;
        public int defenseDebuff { get; set; } = 0;
        public int speedDebuff { get; set; } = 0;
        public string[] recipeBlacklist { get; set; } = { };
        public bool seededRandom { get; set; } = false;
    }
}
