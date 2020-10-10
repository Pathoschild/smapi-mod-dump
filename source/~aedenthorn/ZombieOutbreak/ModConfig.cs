/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace ZombieOutbreak
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float GreenAmount { get; set; } = 0.8f;
        public float DailyZombificationChance { get; set; } = 0.1f;
        public int InfectionDistance { get; set; } = 128;
        public float InfectionChancePerSecond { get; set; } = 0.05f;
    }
}