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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Characters;
using System;
using System.Collections.Generic;

namespace RememberBirthdays
{
    internal class BirthdayHandler
    { 
        private readonly IMonitor Monitor;
        private readonly ModConfig Config;
   
        List<NPC> villagers;
        public NPC birthdayNPC;
        public ClickableTextureComponent birthdayIcon;
        public bool birthdayToday = false;
        bool displayNPC = false;

        public BirthdayHandler(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;

            if (Config.NPCIcon)
            {
                this.displayNPC = true;
            }

            villagers = GetVillagers();
            foreach (var npc in villagers)
            {
                this.birthdayToday = CheckBirthday(npc);
                if (birthdayToday)
                {
                    this.birthdayNPC = npc;
                    break;
                }
            }

        }

        public static bool CheckBirthday(NPC npc)
        {
            string season = Game1.currentSeason;
            int day = Game1.dayOfMonth;
            return npc.isBirthday(season, day);
        }

        internal Point IconCoords()
        {
            int x = Game1.uiViewport.Width - 310;
            int y = (this.displayNPC) ? 200 : 205;
            return new Point(x, y);
        }

        internal ClickableTextureComponent BirthdayIcon(bool gifted)
        {
            int zoom = 2;
            Point coords = IconCoords();
            int x = coords.X;
            int y = coords.Y;
            Texture2D texture;
            Rectangle sourceRect;
            if (this.birthdayToday && !gifted)
            {
                if (this.displayNPC)
                {
                    texture = Game1.getCharacterFromName(this.birthdayNPC.Name, false).Sprite.Texture;
                    sourceRect = new Rectangle(0, 0, 16, 16);
                }
                else
                {
                    texture = Game1.mouseCursors;
                    sourceRect = new Rectangle(229, 410, 14, 14);
                }

                birthdayIcon = new ClickableTextureComponent(new Rectangle(x, y, 10 * zoom, 10 * zoom), texture, sourceRect, zoom);
                return birthdayIcon;

            }
            else
            {
                birthdayIcon = null;
                return birthdayIcon;
            }

        }

        /// Modified method from GitHub user bouhm 
        /// https://github.com/bouhm/stardew-valley-mods/blob/main/NPCMapLocations/ModEntry.cs#L676
        private List<NPC> GetVillagers()
        {
            var villagers = new List<NPC>();

            foreach (var location in Game1.locations)
            {
                foreach (var npc in location.getCharacters())
                {
                    bool shouldTrack =
                        npc != null
                        && ((npc.CanSocialize && Game1.MasterPlayer.friendshipData.ContainsKey(npc.Name))
                            || (npc.Name.Equals("Dwarf") && Game1.MasterPlayer.canUnderstandDwarves)
                            || npc.isMarried()
                            || npc is Child
                        );

                    if (shouldTrack && !villagers.Contains(npc))
                        villagers.Add(npc);
                }
            }

            return villagers;
        }

    }
}
