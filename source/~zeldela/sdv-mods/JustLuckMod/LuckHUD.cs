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
using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Collections;

namespace JustLuckMod
{
    internal class LuckHUD      
    {
        private IModHelper helper;

        public LuckHUD(IModHelper helper)
        {
            this.helper = helper;
        }

        internal string GetFortuneMessage(Farmer who)
        {
            StardewValley.Objects.TV tv = new StardewValley.Objects.TV();
            string fortuneTV = tv.getFortuneForecast(who);
            string[] delimiterChars = { "! ", "!", ". ", "." };
            string[] words = fortuneTV.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
            string fortune = "";

            foreach (var word in words)
            {
                fortune = $"{fortune}{word}.\n";
            }
            fortune = $"{fortune}\n";
            return fortune;
        }

        internal string GetFortune(double fortuneScore)
        {
            double fortuneLuck = Math.Truncate((fortuneScore * 100 * 100)) / 100;

            if (fortuneScore < 0)
            {
                return $"{fortuneLuck}%";
            }
            else
            {
                return $"+{fortuneLuck}%";
            }
        }

        internal Color GetFortuneColor(double fortuneScore)
        {
            switch (fortuneScore)
            {
                case < -0.07:
                    return new Color(204, 102, 102, 255);
                case < -0.02:
                    return new Color(222, 161, 144, 255);
                case > 0.07:
                    return new Color(130, 190, 145, 255);
                case > 0.02:
                    return new Color(188, 220, 196, 255);
                default:
                    return Color.White;
            }

        }

        internal double GetFortuneScore(Farmer who)
        {
            double dailyLuck = who.DailyLuck;
            return dailyLuck;
        }

        internal int GetFortuneBuff(Farmer who)
        {
            int fortuneBuff = who.LuckLevel;
            return fortuneBuff;
        }

        internal string GetBuffString(int fortuneBuff)
        {
            if (fortuneBuff < 1)
                return "";
            return $" (+{fortuneBuff})";
        }

        internal Point GetIconCoords(ModConfig config)
        {
            int x;
            int y;
            int zoom = (Game1.pixelZoom / 4) * 3;
            switch (config.Location)
            {
                case "Stamina Bar":
                    float num = 0.625f;
                    x = Game1.uiViewport.Width - 8 - (50/2) - ((10 * zoom)/2);
                    y = Game1.uiViewport.Height - 224 - 16 - (int)((float)(Game1.player.MaxStamina - 270) * num) - (10 * zoom) - 8;
                    return new Point(x, y);
                default:
                    x = Game1.uiViewport.Width - 300;
                    y = 165;
                    return new Point(x, y);
            }
        }

        internal ClickableTextureComponent GetLuckIcon(Point coords)
        {
            int zoom = (Game1.pixelZoom / 4) * 3;
            int x = coords.X;
            int y = coords.Y;
            ClickableTextureComponent luckIcon = new ClickableTextureComponent(
                    new Rectangle(x, y, 10 * zoom, 10 * zoom),
                    Game1.mouseCursors,
                    new Rectangle(381, 361, 10, 10),
                    zoom);
            return luckIcon;
        }

    }
}

