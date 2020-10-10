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
using EiTK.Gui;
using EiTK.Gui.Option;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Teleport
{
    public class TPMenu : GuiMenu
    {
        private List<TPData> _tpDatas;
        private List<GuiOptionsElements> _optionButtons;
        

        public TPMenu(IModHelper helper,List<TPData> _tpDatas)
        {
            this._tpDatas = new List<TPData>();
            this._tpDatas = _tpDatas;
            this._optionButtons = new List<GuiOptionsElements>();
            foreach (var VARIABLE in this._tpDatas)
            {
                _optionButtons.Add(new GuiOptionButton(helper.Translation.Get("tp." + VARIABLE.name),"TP",700, () =>
                {
                    TP(VARIABLE.locationName,VARIABLE.tileX,VARIABLE.tileY);
                }));
            }
            
            this.optionLists.Add(new GuiOptionList(320,
                120,10)
            {
                guiOptionsElementses = _optionButtons
            });
            
        }


        public override void draw(SpriteBatch b)
        {
            GuiHelper.drawBox(b,300,
                100,835,595,Color.White);
            base.draw(b);
            drawMouse(b);
        }

        private void TP(string locationName, int tileX, int tileY)
        {
            Game1.exitActiveMenu();
            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

            // warp
            Game1.warpFarmer(locationName, tileX, tileY, false);
        }

    }
}