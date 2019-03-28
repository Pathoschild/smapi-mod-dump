using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMonsters.Framework;
using StardewModdingAPI;

namespace CustomMonsters
{
    public class CustomMonsters : Mod
    {
        private IList<MonsterData> monsters;
        /// <summary>
        /// Entry method called after the mod is loaded
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            //Events here

            //Lets load up the Content Packs for this mod.
            Monitor.Log("Loading Content Packs.", LogLevel.Trace);
            foreach (IContentPack cp in Helper.ContentPacks.GetOwned())
                LoadMonsters(cp);
        }


        /*
         * Private Methods
         */

        private void LoadMonsters(IContentPack contentPack)
        {
            Monitor.Log($"Loading Content Pack: {contentPack.Manifest.Name}{contentPack.Manifest.Version} by {contentPack.Manifest.Author}");

            //Start loading custom monsters.
            DirectoryInfo monInfo = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Monsters"));

            if (monInfo.Exists)
            {
                foreach (var dir in monInfo.EnumerateDirectories())
                {
                    string relPath = $"Monsters/{dir.Name}";

                    //Load the data up.
                }
            }
        }
    }
}
