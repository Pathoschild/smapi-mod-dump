/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Helpers;
using EconomyMod.Interface.PageContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod.Interface.Submenu
{
    public class ConfigurationPage : Page
    {
        public ConfigurationPage(UIFramework ui, Texture2D Icon = null, string hoverText = null) : base(ui, Icon, hoverText)
        {

            //Elements.Add(new OptionsElement(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.Constants.WholeYearDaysCount33")));
            //Elements.Add(new ContentElementText(Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.Constants.WholeYearDaysCount34")));
            Elements.Add(new ContentElementHeaderText(Util.Helper.Translation.Get("Configuration").Default("Economy Mod Configuration")));
            //Elements.Add(new ContentElementSlider("Threshold To Ask About Payment", () => Util.Config.ThresholdInPercentageToAskAboutPayment, (o) => Util.Config.SetThresholdInPercentageToAskAboutPayment(Convert.ToByte(o))));
            Elements.Add(new ContentElementCheckbox(Util.Helper.Translation.Get("ChkTaxAfterFirstYear").Default("skip tax for the first Year"), Util.Config.TaxAfterFirstYear, (newValue) =>
            {
                Util.Config.TaxAfterFirstYear = newValue;
                Util.Helper.WriteConfig(Util.Config);

            }));


            this.Draw = DrawContent;
            this.DrawHover = DrawHoverContent;
            ui.OnLeftClick += Leftclick;

        }

        private void Leftclick(object sender, Coordinate e)
        {
            if (this.active)
            {
                foreach (var el in Elements)
                {


                    if (el is ContentElementSlider slider)
                    {


                        if (InterfaceHelper.ClickOnTriggerArea(e.X, e.Y, new Rectangle(slider.clickArea.X, slider.clickArea.Y, slider.clickArea.Width, slider.clickArea.Height)))
                            slider.receiveLeftClick(e.X - slider.clickArea.X, e.Y - slider.clickArea.Y);
                    }
                    if (el is ContentElementCheckbox chk)
                    {
                        chk.receiveLeftClick(e.X, e.Y);
                    }

                }
            }

        }

        private void DrawHoverContent(int arg1, int arg2)
        {
            //throw new NotImplementedException();
        }

        private void DrawContent()
        {

            int currentItemIndex = 0;
            for (int i = 0; i < Slots.Count; ++i)
            {
                if (Slots[i].bounds.X != xPositionOnScreen + Game1.tileSize / 4)
                {
                    Slots[i].bounds = new Rectangle(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * (height - Game1.tileSize * 2) / 7, width - Game1.tileSize / 2, (height - Game1.tileSize * 2) / 7 + Game1.pixelZoom);
                }
                if (currentItemIndex >= 0 &&
                    currentItemIndex + i < Elements.Count)
                {
                    Elements[currentItemIndex + i].draw(Game1.spriteBatch, Slots[i].bounds.X, Slots[i].bounds.Y);
                    InterfaceHelper.Draw(Slots[i].bounds, InterfaceHelper.InterfaceHelperType.Cyan);

                    if (Elements[currentItemIndex + i] is ContentElementSlider slider)
                    {
                        if (slider.clickArea.IsEmpty)
                        {
                            var bounds = Slots[i].bounds;
                            var clickArea = new Rectangle(bounds.X + 32, bounds.Y + 16, slider.bounds.Width, slider.bounds.Height);
                            if (slider.bounds.X != clickArea.X || slider.bounds.Y != clickArea.Y)
                            {
                                slider.clickArea = clickArea;
                            }
                        }
                        InterfaceHelper.Draw(slider.clickArea, InterfaceHelper.InterfaceHelperType.Red);

                    }
                    if (Elements[currentItemIndex + i] is ContentElementCheckbox chk)
                    {
                        if (chk.Slot != currentItemIndex + i)
                        {
                            chk.Slot = currentItemIndex + i;
                            chk.SlotBounds = new Rectangle(Slots[i].bounds.X + 36, Slots[i].bounds.Y + 16, 36, 36);
                        }
                    }
                    //else
                    InterfaceHelper.Draw(Slots[i].bounds, InterfaceHelper.InterfaceHelperType.Cyan);
                }
            }
            //}
            //throw new NotImplementedException();
        }

    }
}
