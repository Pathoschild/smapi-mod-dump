/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMonsters.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace CustomMonsters
{
    public class CustomMonsters : Mod
    {
        private IList<MonsterData> monsters = new List<MonsterData>();
        private MonsterData mData;
        /// <summary>
        /// Entry method called after the mod is loaded
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            //Events here
            helper.Events.Input.ButtonPressed += Button;
            //Lets load up the Content Packs for this mod.
            Monitor.Log("Loading Content Packs.", LogLevel.Trace);
            foreach (IContentPack cp in Helper.ContentPacks.GetOwned())
                LoadMonsters(cp);
        }


        /*
         * Private Methods
         */

        private void Button(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.F8))
            {
                ICursorPosition c = Helper.Input.GetCursorPosition();
                int x = Game1.getMouseX();
                int y = Game1.getMouseY();

                //Try adding new monsters to the map.
                //BigSlime bs = new BigSlime(new Vector2(x, y), 1);
                int mineLevel = 1;
                if (Game1.player.CombatLevel >= 10)
                    mineLevel = 140;
                else if (Game1.player.CombatLevel >= 8)
                    mineLevel = 100;
                else if (Game1.player.CombatLevel >= 4)
                    mineLevel = 41;

                IList<NPC> characters = Game1.currentLocation.characters;
                MonsterData
                MetalHead metal = new MetalHead(c.Tile * 64f, mineLevel) { wildernessFarmMonster = true };
                characters.Add((NPC)metal);
            }
        }
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
                    mData = contentPack.ReadJsonFile<MonsterData>($"{relPath}/monster.json");

                    if (mData == null) continue;

                    mData.mSprite = contentPack.LoadAsset<AnimatedSprite>($"{relPath}/monster.png");
                    mData.mSpriteStr = $"{relPath}/monster.png";
                    

                    this.monsters.Add(mData);
                    //Load the data up.
                }
            }
        }
    }
}
