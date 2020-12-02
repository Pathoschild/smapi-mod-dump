/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using BCC.Utilities;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewModdingAPI;
using System;

namespace BCC.Menus
{
    class PantryDonationSheet : LetterViewerMenu
    {
        public string[] MailAsSplitString;
        public string MailAsOneString;
        public IMonitor Monitor;
        public IModHelper Helper;
        /*public Texture2D VillagerIcon;

        public int IconWidth = 0;
        public int IconHeight = 0;
        public int IndexCount = 0;
        public int ChosenIndex = 0;*/

        public PantryDonationSheet(string Lines, string Title, IMonitor monitor, IModHelper helper) : base(Lines, Title)
        {
            Helper = helper;
            Monitor = monitor;

            //VillagerIcon = Helper.Content.Load<Texture2D>("Assets/Textures/VillagerIcons.png", ContentSource.ModFolder);
            //getIconIndexCount();

            MailAsSplitString = Lines.Split(',');
            foreach (string item in MailAsSplitString)
            {
                item.Trim(',');
                /*for(int i=0; i<IndexCount; i++)
                {
                    if (item.Contains($"$VIndex{i}"))
                    {
                        item.Remove(item.IndexOf('$'), 7);
                        ChosenIndex = i;
                    }
                }*/
                MailAsOneString += item;
            }
            LetterViewerMenu menu = new LetterViewerMenu(MailAsOneString, Title);
            Game1.activeClickableMenu = menu;
        }

        /*public void getIconIndexCount()
        {
            Texture2D emojis = VillagerIcon;
            int width = emojis.Width / 9;
            int height = emojis.Height / 9;
            int indexCount = width * height;

            IconWidth = width;
            IconHeight = height;
            IndexCount = indexCount;
            Monitor.Log($"{IconWidth}-{IconHeight}-{IndexCount}", LogLevel.Debug);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }*/
    }
}
