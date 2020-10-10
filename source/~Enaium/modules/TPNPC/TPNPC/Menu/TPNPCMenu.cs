/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Security.Cryptography;
using AutoSave.Framework.Models;
using EiTK.Gui;
using EiTK.Gui.Option;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NPCTPHere.TP;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace Teleport
{
    public class TPNPCMenu : GuiMenu
    {
        private int width;
        private int height;

        private List<NPCData> npcDatas;
        
        public TPNPCMenu(IModHelper helper)
        {
            this.width = 835;
            this.height = 595;
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0);
            this.xPositionOnScreen = (int) centeringOnScreen.X;
            this.yPositionOnScreen = (int) centeringOnScreen.Y + 32;
            init();

            List<GuiOptionsElements> guiOptionsElementses;
            guiOptionsElementses = new List<GuiOptionsElements>();

            foreach (var VARIABLE in npcDatas)
            {
                guiOptionsElementses.Add(new GuiOptionButton(VARIABLE.npc.displayName,"TP", 670,
                    () =>
                {
                    TP(VARIABLE.locationName,VARIABLE.tileX,VARIABLE.tileY);
                }));
            }
            
            this.optionLists.Add(new GuiOptionList(this.xPositionOnScreen + 20,this.yPositionOnScreen + 20,10)
            {
                guiOptionsElementses = guiOptionsElementses
            });

        }

        private void init()
        {
            npcDatas = new List<NPCData>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                CharacterType? type = this.GetCharacterType(npc);
                if (type == null || npc?.currentLocation == null)
                    continue;
                Point tile = npc.getTileLocationPoint();
                npcDatas.Add(new NPCData(npc,npc.currentLocation.Name,tile.X,tile.Y));
            }

        }
        
        private CharacterType? GetCharacterType(NPC npc)
        {
            if (npc is Monster)
                return null;
            if (npc is Horse)
                return CharacterType.Horse;
            if (npc is Pet)
                return CharacterType.Pet;
            return CharacterType.Villager;
        }
        

        private void TP(string locationName, int tileX, int tileY)
        {
            Game1.exitActiveMenu();
            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

            // warp
            Game1.warpFarmer(locationName, tileX, tileY, false);
        }
        

        public override void draw(SpriteBatch b)
        {
            GuiHelper.drawBox(b,this.xPositionOnScreen,
                this.yPositionOnScreen,835,595,Color.White);
            base.draw(b);
            drawMouse(b);
        }
    }
}