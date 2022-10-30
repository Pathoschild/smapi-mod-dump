/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zeldela/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace RememberBirthdays
{
    public class ModEntry : Mod
    {
        ClickableTextureComponent birthdayIcon = null;
        BirthdayHandler hbd;
        bool birthdayToday = false;
        bool gifted = false;
        ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            if (!Config.Disable)
            {
                helper.Events.GameLoop.DayStarted += this.OnDayStarted;
                helper.Events.Display.RenderedHud += this.OnRenderedHUD;
                helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
                helper.Events.Display.WindowResized += this.OnWindowResized;
               
            }

        }

        internal void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            hbd = new BirthdayHandler(this.Monitor, Config);
            birthdayToday = hbd.birthdayToday;
            gifted = false;
            if (birthdayToday)
            {
                Monitor.Log($"{this.hbd.birthdayNPC.Name} has a birthday today.", LogLevel.Debug);
            }
        }

        internal void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (birthdayToday)
            {
                if (Game1.player.friendshipData.ContainsKey(hbd.birthdayNPC.Name))
                {
                    if (Game1.player.friendshipData[hbd.birthdayNPC.Name].LastGiftDate == new WorldDate(Game1.Date))
                    {
                        gifted = true;
                        birthdayIcon = hbd.BirthdayIcon(gifted);
                    }
                }
            }

        }

        internal void OnRenderedHUD(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady || gifted)
                return;

            birthdayIcon = hbd.BirthdayIcon(gifted);

            if (Game1.displayHUD && birthdayIcon != null)
            {
                Point coords = hbd.IconCoords();
                birthdayIcon.bounds.X = coords.X;
                birthdayIcon.bounds.Y = coords.Y;
                birthdayIcon.draw(Game1.spriteBatch);

                if (birthdayIcon.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                { 
                    IClickableMenu.drawHoverText(Game1.spriteBatch, hbd.birthdayNPC.Name, Game1.smallFont);
                }

            }

        }

        internal void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!Context.IsWorldReady || gifted)
                return;

            birthdayIcon = hbd.BirthdayIcon(gifted);
            
        }


    }
}
