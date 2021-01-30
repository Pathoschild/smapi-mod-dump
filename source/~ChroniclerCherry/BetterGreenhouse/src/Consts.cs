/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Harmony;

namespace GreenhouseUpgrades
{
    public static class Consts
    {
        public static string ModUniqueID;
        public static HarmonyInstance Harmony;

        public const string GreenHouseSource = "assets\\Greenhouse.tibn";
        public const string GreenhouseMapPath = "Maps\\Greenhouse";

        public const string SaveDataKey = "SaveData.Version.1";

        public const string MultiplayerLoadKey = "Multiplayer.Load";
        public const string MultiplayerUpdate = "Multiplayer.Update";
        public const string MultiplayerJunimopointsKey = "Multiplayer.JunimoPoints";

    }
}
