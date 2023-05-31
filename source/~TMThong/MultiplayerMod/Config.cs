/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using StardewModdingAPI;

namespace MultiplayerMod
{
    internal class Config
    {
        public int Port { get; set; } = 12011;
        public string LastIP { get; set; }
    }
}
