/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(LoadGameMenu), "FindSaveGames")]
    class DisableOtherSaves
    {
        public static bool Prefix(ref List<Farmer> __result)
        {
            __result = new List<Farmer>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $@"assets/Saves/{Constants.SaveFile}/SaveGameInfo");
            FileStream stream = File.OpenRead(path);

            Farmer f = (Farmer)SaveGame.farmerSerializer.Deserialize(stream)!;
            SaveGame.loadDataToFarmer(f);
            f.slotName = Constants.SaveFile;
            __result.Add(f);

            stream.Close();

            return false;
        }
    }

    [HarmonyPatch(typeof(SaveGame), nameof(SaveGame.Load))]
    class DisableOtherSaves2
    {
        public static bool Prefix()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $@"assets/Saves/{Constants.SaveFile}/{Constants.SaveFile}");

            Game1.gameMode = 6;
            Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
            Game1.currentLoader = SaveGame.getLoadEnumerator(path);

            return false;
        }
    }
}
