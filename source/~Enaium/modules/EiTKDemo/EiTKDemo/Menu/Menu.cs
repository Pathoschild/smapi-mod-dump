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
using EiTK.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EiTKDemo.Menu
{
    public class Menu : GuiMenu
    {
        public Menu()
        {


            List<GuiOptionsElements> optionLabels;
            optionLabels = new List<GuiOptionsElements>();


            optionLabels.Add(new GuiOptionLabel("----------------1"));
            optionLabels.Add(new GuiOptionLabel("----------------2"));
            optionLabels.Add(new GuiOptionLabel("----------------3"));
            optionLabels.Add(new GuiOptionLabel("----------------4"));
            optionLabels.Add(new GuiOptionLabel("----------------5"));
            optionLabels.Add(new GuiOptionLabel("----------------6"));
            optionLabels.Add(new GuiOptionButton("----------------7","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------7"));
            }));
            optionLabels.Add(new GuiOptionButton("----------------8","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------8"));
            }));            
            optionLabels.Add(new GuiOptionButton("----------------9","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------9"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------10"));
            optionLabels.Add(new GuiOptionLabel("----------------11"));
            optionLabels.Add(new GuiOptionLabel("----------------12"));
            optionLabels.Add(new GuiOptionLabel("----------------13"));
            optionLabels.Add(new GuiOptionLabel("----------------14"));
            optionLabels.Add(new GuiOptionLabel("----------------15"));
            optionLabels.Add(new GuiOptionButton("----------------16","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------16"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------17"));
            optionLabels.Add(new GuiOptionButton("----------------18","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------18"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------19"));
            optionLabels.Add(new GuiOptionButton("----------------20","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------20"));
            }));
            optionLabels.Add(new GuiOptionLabel("----------------21"));
            optionLabels.Add(new GuiOptionButton("----------------22","open",700, () =>
            {
                Game1.addHUDMessage(new HUDMessage("----------------22"));
            }));
            
            this.optionLists.Add(new GuiOptionList(50,50,10)
            {
                guiOptionsElementses = optionLabels
            });
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            drawMouse(b);
        }
    }
}