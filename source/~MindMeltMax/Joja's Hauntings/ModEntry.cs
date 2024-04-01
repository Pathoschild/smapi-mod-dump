/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using System.Reflection;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace HauntedJoja
{
    public class ModEntry : Mod
    {
        public JojaMart jojaMart;
        public GameLocation GhostDungeon;

        public Warp DungeonWarp;
        public Warp ReturnWarp;

        public Ghost ghost;

        public Item RewardItem;

        public ITranslationHelper i18n;

        public string OrigTile1 = "Message \"JojaMart.8\"";
        public string OrigTile2 = "Message \"JojaMart.1\"";
        public string OrigTile3 = "Message \"JojaMart.46\"";
        public string OrigTile4 = "Message \"JojaMart.38\"";
        public string OrigTile5 = "Message \"JojaMart.66\"";
        public string OrigTile6 = "Message \"JojaMart.61\"";

        public override void Entry(IModHelper helper)
        {
            i18n = Helper.Translation;

            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            helper.Events.Content.AssetRequested += onAssetRequested;
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("HauntedJoja/Map"))
                e.LoadFromModFile<Map>("assets/spoilers/HiddenMap.tmx", AssetLoadPriority.Exclusive);
            if (e.NameWithoutLocale.IsEquivalentTo("HauntedJoja/Animation"))
                e.LoadFromModFile<Texture2D>("assets/spoilers/Anima.png", AssetLoadPriority.Exclusive);
            if (e.NameWithoutLocale.IsEquivalentTo("HauntedJoja/MineTiles"))
                e.LoadFromModFile<Texture2D>("assets/spoilers/mine.png", AssetLoadPriority.Exclusive);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.currentSeason != "fall") 
                return;
            if (Game1.Date.DayOfMonth == 26)
            {
                Game1.activeClickableMenu = new DialogueBox(i18n.Get("Message0"));
                doMapUpdates(0);

                GhostDungeon = new("HauntedJoja/Map", "Ghost Dungeon") { IsFarm = false, IsGreenhouse = false, IsOutdoors = false };
                Game1.locations.Add(GhostDungeon);

                DungeonWarp = new((int)Game1.player.Tile.X, (int)Game1.player.Tile.Y, "Ghost Dungeon", 13, 17, false);
                ReturnWarp = new((int)Game1.player.Tile.X, (int)Game1.player.Tile.Y, "JojaMart", 20, 5, false);
            }
            else if (Game1.Date.DayOfMonth == 27)
            {
                resetMapData();

                Game1.locations.Remove(GhostDungeon);
                GhostDungeon = null;
            }
            else return;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove || !e.Button.IsActionButton())
                return;
            Vector2 tile = e.Cursor.GrabTile;
            string property = Game1.currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
            int id = -1;
            switch (property)
            {
                default:
                    return;
                case "Are":
                    id = 1;
                    break;
                case "You":
                    id = 2;
                    break;
                case "Affraid":
                    id = 3;
                    break;
                case "Of":
                    id = 4;
                    break;
                case "The":
                    id = 5;
                    break;
                case "Dark":
                    id = 6;
                    break;
                case "Boo!":
                    Game1.activeClickableMenu = new DialogueBox(i18n.Get("Message7"));
                    jojaMart.removeTile(19, 3, "Buildings");
                    Layer layer = jojaMart.map.GetLayer("Buildings");
                    TileSheet sheet = jojaMart.map.GetTileSheet("z_CustomJojaAnima");
                    setAnimatedTile(19, 3, layer, sheet);
                    WarpFarmerAfterXTime(2250);
                    return;
                case "StartWave":
                    int randomNum = Game1.random.Next(15, 25); //UB3R-BOT Has Spoken
                    for (int i = 0; i < randomNum; i++)
                    {
                        int vecRandomX = Game1.random.Next(4, 19);
                        int vecRandomY = Game1.random.Next(11, 16);
                        Vector2 MonserLoc = new(vecRandomX, vecRandomY);
                        ghost = new(MonserLoc);
                        tryToAddGhosts(ghost, vecRandomX, vecRandomY);
                    }
                    Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
                    return;
                case "ClaimPrize":
                    var itemNum = Game1.random.Next(1, 8);
                    RewardItem = getRandomItem(itemNum);
                    Game1.player.addItemToInventory(RewardItem);
                    Game1.activeClickableMenu = new DialogueBox(i18n.Get("Treasure"));
                    Game1.player.warpFarmer(ReturnWarp);
                    resetMapData();
                    return;
            }
            Game1.activeClickableMenu = new DialogueBox(i18n.Get($"Message{id}"));
            doMapUpdates(id);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            jojaMart = Game1.RequireLocation<JojaMart>("JojaMart");

            TileSheet tile = new("z_CustomJojaAnima", jojaMart.map, "HauntedJoja/Animation", new xTile.Dimensions.Size(80, 16), new xTile.Dimensions.Size(16, 16));
            TileSheet tile2 = new("z_mineAdditions", jojaMart.map, "HauntedJoja/MineTiles", new xTile.Dimensions.Size(256, 288), new xTile.Dimensions.Size(16, 16));

            jojaMart.map.AddTileSheet(tile);
            jojaMart.map.AddTileSheet(tile2);
            jojaMart.map.LoadTileSheets(Game1.mapDisplayDevice);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.CanPlayerMove || Game1.player.currentLocation != Game1.getLocationFromName("Ghost Dungeon"))
                return;

            var loc = Game1.currentLocation;
            if (loc.characters.Count > 0) 
                return;

            int x = 9;
            int y = 6;
            loc.removeTileProperty(x, y, "Buildings", "Action");
            loc.setTileProperty(x, y, "Buildings", "Action", "ClaimPrize");
            Game1.activeClickableMenu = new DialogueBox(i18n.Get("Finale"));
            Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
        }

        public void setAnimatedTile(int tileX, int tileY, Layer mapLayer, TileSheet mapSheet)
        {
            mapLayer.Tiles[tileX, tileY] = new AnimatedTile(mapLayer,
            [
                new(mapLayer, mapSheet, BlendMode.Alpha, 0),
                new(mapLayer, mapSheet, BlendMode.Alpha, 1),
                new(mapLayer, mapSheet, BlendMode.Alpha, 2),
                new(mapLayer, mapSheet, BlendMode.Alpha, 3),
                new(mapLayer, mapSheet, BlendMode.Alpha, 4),
                new(mapLayer, mapSheet, BlendMode.Alpha, 3),
                new(mapLayer, mapSheet, BlendMode.Alpha, 2),
                new(mapLayer, mapSheet, BlendMode.Alpha, 1),
                new(mapLayer, mapSheet, BlendMode.Alpha, 0),
                new(mapLayer, mapSheet, BlendMode.Alpha, 5)
            ], 250);
        }

        public DelayedAction WarpFarmerAfterXTime(int Time)
        {
            DelayedAction action = new(Time) { behavior = WarpAndResetTile };
            Game1.delayedActions.Add(action);
            return action;
        }
        public void WarpAndResetTile()
        {
            Layer layer = jojaMart.map.GetLayer("Buildings");
            TileSheet sheet = jojaMart.map.GetTileSheet("z_CustomJojaAnima");
            Game1.player.warpFarmer(DungeonWarp);
            layer.Tiles[19, 3] = new StaticTile(layer, sheet, BlendMode.Alpha, 5);
        }

        public void tryToAddGhosts(Monster m, int X, int Y)
        {
            m.setTilePosition(X, Y);
            m.displayName = "Dungeon Ghost";
            GhostDungeon.addCharacter(m);
        }

        public Item getRandomItem(int i)
        {
            RewardItem = i switch
            {
                1 => new Object("74", 1),
                2 => new Object("107", 1),
                3 => new Object("166", 1),
                5 => new Object("337", 3),
                6 => new Object("373", 1),
                7 => new Object("797", 1),
                _ => new Object("275", 5),
            };
            return RewardItem;
        }

        public void doMapUpdates(int index)
        {
            switch (index)
            {
                case 0:
                    jojaMart.setTileProperty(3, 16, "Buildings", "Action", "Are");
                    jojaMart.map.GetLayer("Back").Tiles[2, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
                    break;
                case 1:
                    jojaMart.setTileProperty(15, 16, "Buildings", "Action", "You");
                    jojaMart.map.GetLayer("Back").Tiles[2, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                    jojaMart.map.GetLayer("Back").Tiles[14, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
                    break;
                case 2:
                    jojaMart.setTileProperty(28, 13, "Buildings", "Action", "Affraid");
                    jojaMart.map.GetLayer("Back").Tiles[14, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                    jojaMart.map.GetLayer("Back").Tiles[27, 13] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
                    break;
                case 3:
                    jojaMart.setTileProperty(19, 14, "Buildings", "Action", "Of");
                    jojaMart.map.GetLayer("Back").Tiles[27, 13] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                    jojaMart.map.GetLayer("Back").Tiles[18, 14] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
                    break;
                case 4:
                    jojaMart.setTileProperty(4, 10, "Buildings", "Action", "The");
                    jojaMart.map.GetLayer("Back").Tiles[18, 14] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                    jojaMart.map.GetLayer("Back").Tiles[5, 10] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 0);
                    break;
                case 5:
                    jojaMart.setTileProperty(11, 11, "Buildings", "Action", "Dark");
                    jojaMart.map.GetLayer("Back").Tiles[5, 10] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1889);
                    jojaMart.map.GetLayer("Back").Tiles[10, 11] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
                    break;
                case 6:
                    jojaMart.setTileProperty(20, 4, "Buildings", "Action", "Boo!");
                    jojaMart.setTileProperty(20, 3, "Buildings", "Action", "Boo!");
                    jojaMart.map.GetLayer("Back").Tiles[10, 11] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1985);
                    break;
            }
        }

        public void resetMapData()
        {
            jojaMart.setTileProperty(3, 16, "Buildings", "Action", OrigTile6);
            jojaMart.setTileProperty(15, 16, "Buildings", "Action", OrigTile5);
            jojaMart.setTileProperty(28, 13, "Buildings", "Action", OrigTile4);
            jojaMart.setTileProperty(19, 14, "Buildings", "Action", OrigTile3);
            jojaMart.setTileProperty(4, 10, "Buildings", "Action", OrigTile2);
            jojaMart.setTileProperty(11, 11, "Buildings", "Action", OrigTile1);
            jojaMart.removeTileProperty(20, 4, "Buildings", "Action");
            jojaMart.removeTileProperty(20, 3, "Buildings", "Action");
        }
    }
}
