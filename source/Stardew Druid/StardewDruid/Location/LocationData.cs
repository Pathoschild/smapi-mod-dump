/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;
using static StardewDruid.Data.IconData;
using static StardewValley.Minigames.CraneGame;

namespace StardewDruid.Location
{

    public class LocationTile
    {

        public IconData.tilesheets tilesheet = IconData.tilesheets.druid;

        public Microsoft.Xna.Framework.Vector2 tile;

        public Microsoft.Xna.Framework.Vector2 position;

        public Microsoft.Xna.Framework.Rectangle rectangle;

        public float layer;

        public float frame;

        public float interval;

        public bool shadow;

        public bool flip;

        public int offset;

        public LocationTile(int x, int y, int w, int h, int Offset, bool Shadow = false, IconData.tilesheets Sheet = IconData.tilesheets.druid)
        {

            tile = new Vector2(x, y);

            tilesheet = Sheet;

            position = tile * 64;

            rectangle = new(w*16,h*16,16,16);

            layer = ((((float)y + (float)offset) * 64f) / 10000f);//Game1.player.drawLayerDisambiguator;

            shadow = Shadow;

            offset = Offset;

        }

        public void Draw(SpriteBatch b)
        {

            if (Utility.isOnScreen(position, 64))
            {

                Microsoft.Xna.Framework.Vector2 drawPosition = new(position.X - (float)Game1.viewport.X, position.Y - (float)Game1.viewport.Y);

                b.Draw(Mod.instance.iconData.sheetTextures[tilesheet], drawPosition, rectangle, Microsoft.Xna.Framework.Color.White, 0f, Vector2.Zero, 4, SpriteEffects.None, layer + offset * 0.0064f);

                if (shadow)
                {

                    b.Draw(Mod.instance.iconData.sheetTextures[tilesheet], drawPosition + new Vector2(4, 8), rectangle, Microsoft.Xna.Framework.Color.Black * 0.35f, 0f, Vector2.Zero, 4, SpriteEffects.None, layer - 0.001f);

                }

            }

        }

    }

    public class WarpTile
    {

        public string location;

        public int enterX;

        public int enterY;

        public int exitX;

        public int exitY;

        public WarpTile(int x, int y, string Location, int a, int b)
        {

            enterX = x;

            enterY = y;

            location = Location;

            exitX = a;

            exitY = b;

        }

    }

    public static class LocationData
    {

        public const string druid_grove_name = "18465_Grove";

        public const string druid_atoll_name = "18465_Atoll";

        public const string druid_chapel_name = "18465_Chapel";

        public const string druid_vault_name = "18465_Treasure";

        public const string druid_court_name = "18465_Court";

        public const string druid_archaeum_name = "18465_Archaeum";

        public const string druid_tomb_name = "18465_Tomb";

        public static void DruidLocations(string map)
        {

            if (Mod.instance.locations.ContainsKey(map))
            {
                
                return;

            }

            GameLocation locale = Game1.getLocationFromName(map);

            if (locale != null)
            {

                Mod.instance.locations[map] = locale;

                return;
            
            }

            switch (map)
            {

                case druid_grove_name:

                    GameLocation grove = new Location.Grove(druid_grove_name);

                    Mod.instance.locations.Add(druid_grove_name, grove);

                    Game1.locations.Add(grove);

                    return;

                case druid_atoll_name:

                    GameLocation atoll = new Location.Atoll(druid_atoll_name);

                    Game1.locations.Add(atoll);

                    Mod.instance.locations.Add(druid_atoll_name, atoll);

                    return;

                case druid_chapel_name:

                    GameLocation chapel = new Location.Chapel(druid_chapel_name);

                    Game1.locations.Add(chapel);

                    Mod.instance.locations.Add(druid_chapel_name, chapel);

                    return;

                case druid_vault_name:

                    GameLocation vault = new Location.Vault(druid_vault_name);

                    Game1.locations.Add(vault);

                    Mod.instance.locations.Add(druid_vault_name, vault);

                    return;

                case druid_court_name:

                    GameLocation court = new Location.Court(druid_court_name);

                    Game1.locations.Add(court);

                    Mod.instance.locations.Add(druid_court_name, court);

                    return;

                case druid_archaeum_name:

                    GameLocation archaeum = new Location.Archaeum(druid_archaeum_name);

                    Game1.locations.Add(archaeum);

                    Mod.instance.locations.Add(druid_archaeum_name, archaeum);

                    return;

                case druid_tomb_name:

                    GameLocation tomb = new Location.Tomb(druid_tomb_name);

                    Game1.locations.Add(tomb);

                    Mod.instance.locations.Add(druid_tomb_name, tomb);

                    return;

            }

        }

    }


}
