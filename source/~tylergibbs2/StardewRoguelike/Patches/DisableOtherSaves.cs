/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    class DisableOtherSaves : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor()
        {
            return new(typeof(LoadGameMenu), "FindSaveGames");
        }

        public static bool Prefix(ref List<Farmer> __result)
        {
            __result = new List<Farmer>();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $@"assets/Saves/{Roguelike.SaveFile}/SaveGameInfo");
            FileStream stream = File.OpenRead(path);

            Farmer f = (Farmer)SaveGame.farmerSerializer.Deserialize(stream)!;
            SaveGame.loadDataToFarmer(f);
            f.slotName = Roguelike.SaveFile;
            __result.Add(f);

            stream.Close();

            return false;
        }
    }
    class DisableOtherSaves2 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor()
        {
            return new(typeof(SaveGame), "Load");
        }

        public static bool Prefix()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $@"assets/Saves/{Roguelike.SaveFile}/{Roguelike.SaveFile}");

            Game1.gameMode = 6;
            Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
            Game1.currentLoader = SaveGame.getLoadEnumerator(path);

            return false;
        }
    }
}
