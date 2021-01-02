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

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;

using xTile.Layers;
using xTile.Tiles;

namespace Jojas_Hauntings
{
    public class ModEntry : Mod
    {
        public GameLocation jojaMart;
        public GameLocation GhostDungeon;

        public Warp DungeonWarp;
        public Warp ReturnWarp;

        public Ghost ghost;

        public Item RewardItem;

        public string ScaryMessage1 = "In between the sugar-free lollipops appears to be a piece of torn paper, it reads \"Cows are such delightfull creatures are they not?\"";
        public string ScaryMessage2 = "You think those sugar-free lollipops are safe?";
        public string ScaryMessage3 = "You ever wonder what this hoisin sauce is made out of";
        public string ScaryMessage4 = "I always found Take-Me-To-The-Emergency-Room sauce to have such a fitting name";
        public string ScaryMessage5 = "White fungus soda can't be healthy";
        public string ScaryMessage6 = "This powdered butter makes such a mess";

        public string OrigTile1 = $"Message {"JojaMart.8"}"; 
        public string OrigTile2 = $"Message {"JojaMart.1"}";
        public string OrigTile3 = $"Message {"JojaMart.46"}";
        public string OrigTile4 = $"Message {"JojaMart.38"}";
        public string OrigTile5 = $"Message {"JojaMart.66"}";
        public string OrigTile6 = $"Message {"JojaMart.61"}";

        public string GhostCluber = "The Ghosts came, fought, and went. The last one dropped a key, maybe this one works on the chest.";
        public string TreasureKey = "You shove the key in the chest preparing for the worst, instead it opens to reveal a shiny object.";

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (Game1.currentSeason == "fall" && Game1.Date.DayOfMonth == 26)
            {
                Game1.activeClickableMenu = new DialogueBox("A whisper in your dreams told you about strange things happening in joja");
                jojaMart.setTileProperty(3, 16, "Buildings", "Action", "Are");
                jojaMart.map.GetLayer("Back").Tiles[2, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);

                string mapAssetForMod = Helper.Content.GetActualAssetKey("assets/spoilers/HiddenMap.tmx", ContentSource.ModFolder);

                GhostDungeon = new GameLocation(mapAssetForMod, "Ghost Dungeon") { IsFarm = false, IsGreenhouse = false, IsOutdoors = false };

                Game1.locations.Add(GhostDungeon);

                DungeonWarp = new Warp(Game1.player.getTileX(), Game1.player.getTileY(), "Ghost Dungeon", 13, 17, false);
                ReturnWarp = new Warp(Game1.player.getTileX(), Game1.player.getTileY(), "JojaMart", 20, 5, false);
            }
            else if (Game1.currentSeason == "fall" && Game1.Date.DayOfMonth == 27)
            {
                jojaMart.setTileProperty(3, 16, "Buildings", "Action", OrigTile6);
                jojaMart.setTileProperty(15, 16, "Buildings", "Action", OrigTile5);
                jojaMart.setTileProperty(28, 13, "Buildings", "Action", OrigTile4);
                jojaMart.setTileProperty(19, 14, "Buildings", "Action", OrigTile3);
                jojaMart.setTileProperty(4, 10, "Buildings", "Action", OrigTile2);
                jojaMart.setTileProperty(11, 11, "Buildings", "Action", OrigTile1);
                jojaMart.removeTileProperty(20, 4, "Buildings", "Action");
                jojaMart.removeTileProperty(20, 3, "Buildings", "Action");

                Game1.locations.Remove(GhostDungeon);
                GhostDungeon = null;
            }
            else return;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (e.Button != SButton.MouseRight)
                return;
            Vector2 tile = e.Cursor.GrabTile;
            string property = Game1.currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
            if (property == null)
                return;
            else if(property == "Are")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage6);
                jojaMart.setTileProperty(15, 16, "Buildings", "Action", "You");
                jojaMart.map.GetLayer("Back").Tiles[2, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                jojaMart.map.GetLayer("Back").Tiles[14, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
            }
            else if(property == "You")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage5);
                jojaMart.setTileProperty(28, 13, "Buildings", "Action", "Affraid");
                jojaMart.map.GetLayer("Back").Tiles[14, 16] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                jojaMart.map.GetLayer("Back").Tiles[27, 13] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
            }
            else if(property == "Affraid")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage4);
                jojaMart.setTileProperty(19, 14, "Buildings", "Action", "Of");
                jojaMart.map.GetLayer("Back").Tiles[27, 13] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                jojaMart.map.GetLayer("Back").Tiles[18, 14] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
            }
            else if(property == "Of")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage3);
                jojaMart.setTileProperty(4, 10, "Buildings", "Action", "The");
                jojaMart.map.GetLayer("Back").Tiles[18, 14] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);
                jojaMart.map.GetLayer("Back").Tiles[5, 10] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 0);
            }
            else if(property == "The")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage2);
                jojaMart.setTileProperty(11, 11, "Buildings", "Action", "Dark");
                jojaMart.map.GetLayer("Back").Tiles[5, 10] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1889);
                jojaMart.map.GetLayer("Back").Tiles[10, 11] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("z_mineAdditions"), BlendMode.Alpha, 1);
            }
            else if (property == "Dark")
            {
                Game1.activeClickableMenu = new DialogueBox(ScaryMessage1);
                jojaMart.setTileProperty(20, 4, "Buildings", "Action", "Boo!");
                jojaMart.setTileProperty(20, 3, "Buildings", "Action", "Boo!");
                jojaMart.map.GetLayer("Back").Tiles[10, 11] = new StaticTile(jojaMart.map.GetLayer("Back"), jojaMart.map.GetTileSheet("untitled tile sheet"), BlendMode.Alpha, 1984);

            }
            else if (property == "Boo!")
            {
                var time = Game1.currentGameTime;
                Game1.activeClickableMenu = new DialogueBox("As you reach for the milk to look for clues a hand pulls you throught the door");
                int X = 19;
                int Y = 3;
                jojaMart.removeTile(X, Y, "Buildings");
                Layer layer = jojaMart.map.GetLayer("Buildings");
                TileSheet sheet = jojaMart.map.GetTileSheet("z_CustomJojaAnima");
                setAnimatedTile(X, Y, layer, sheet);
                WarpFarmerAfterXTime(2250);
            }
            else if(property == "StartWave")
            {
                int randomNum = Game1.random.Next(15, 25); //UB3R-BOT Has Spoken
                for (int i = 0; i < randomNum; i++)
                {
                    int vecRandomX = Game1.random.Next(4, 19);
                    int vecRandomY = Game1.random.Next(11, 16);
                    Vector2 MonserLoc = new Vector2(vecRandomX, vecRandomY);
                    ghost = new Ghost(MonserLoc);
                    var loc = Game1.currentLocation as MineShaft;
                    this.tryToAddGhosts(ghost, vecRandomX, vecRandomY);
                }
                Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            }
            else if(property == "ClaimPrize")
            {
                var itemNum = Game1.random.Next(1, 8);
                RewardItem = getRandomItem(itemNum);
                Game1.activeClickableMenu = new DialogueBox(TreasureKey);
                Game1.player.addItemToInventory(RewardItem);
                Game1.player.warpFarmer(ReturnWarp);
                GhostDungeon.setTileProperty(9, 6, "Buildings", "Action", "StartWave");

                //Reset Joja TileData
                jojaMart.setTileProperty(3, 16, "Buildings", "Action", OrigTile6);
                jojaMart.setTileProperty(15, 16, "Buildings", "Action", OrigTile5);
                jojaMart.setTileProperty(28, 13, "Buildings", "Action", OrigTile4);
                jojaMart.setTileProperty(19, 14, "Buildings", "Action", OrigTile3);
                jojaMart.setTileProperty(4, 10, "Buildings", "Action", OrigTile2);
                jojaMart.setTileProperty(11, 11, "Buildings", "Action", OrigTile1);
                jojaMart.removeTileProperty(20, 4, "Buildings", "Action");
                jojaMart.removeTileProperty(20, 3, "Buildings", "Action");
            }
            else return;
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            string tileSheetForAnima = Helper.Content.GetActualAssetKey("assets/spoilers/Anima.png", ContentSource.ModFolder);
            string tileSheetForGrime = Helper.Content.GetActualAssetKey("assets/spoilers/mine.png", ContentSource.ModFolder);

            jojaMart = Game1.getLocationFromName("JojaMart");

            TileSheet tile = new TileSheet("z_CustomJojaAnima", jojaMart.map, tileSheetForAnima, new xTile.Dimensions.Size(80, 16), new xTile.Dimensions.Size(16, 16));
            TileSheet tile2 = new TileSheet("z_mineAdditions", jojaMart.map, tileSheetForGrime, new xTile.Dimensions.Size(256, 288), new xTile.Dimensions.Size(16, 16));

            jojaMart.map.AddTileSheet(tile);
            jojaMart.map.AddTileSheet(tile2);
            jojaMart.map.LoadTileSheets(Game1.mapDisplayDevice);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if (Game1.player.currentLocation != Game1.getLocationFromName("Ghost Dungeon"))
                return;
            var loc = Game1.currentLocation;
            if (loc.characters.Count == 0)
            {
                int x = 9;
                int y = 6;
                loc.removeTileProperty(x, y, "Buildings", "Action");
                loc.setTileProperty(x, y, "Buildings", "Action", "ClaimPrize");
                Game1.activeClickableMenu = new DialogueBox(GhostCluber);
                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            }
            else return;
        }

        public void setAnimatedTile(int tileX, int tileY, Layer mapLayer, TileSheet mapSheet)
        {
            mapLayer.Tiles[tileX, tileY] = new AnimatedTile(mapLayer, new StaticTile[]
            {
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 0),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 1),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 2),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 3),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 4),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 3),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 2),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 1),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 0),
                new StaticTile(mapLayer, mapSheet, BlendMode.Alpha, 5)
            }, 250);
        }

        public DelayedAction WarpFarmerAfterXTime(int Time)
        {
            DelayedAction action = new DelayedAction(Time);
            action.behavior = new DelayedAction.delayedBehavior(this.WarpAndResetTile);
            Game1.delayedActions.Add(action);
            return action;
        }
        public void WarpAndResetTile()
        {
            int X = 19;
            int Y = 3;
            Layer layer = jojaMart.map.GetLayer("Buildings");
            TileSheet sheet = jojaMart.map.GetTileSheet("z_CustomJojaAnima");
            Game1.player.warpFarmer(DungeonWarp);
            layer.Tiles[X, Y] = new StaticTile(layer, sheet, BlendMode.Alpha, 5);
        }

        public void tryToAddGhosts(Monster m, int X, int Y)
        {
            m.setTilePosition(X, Y);
            m.displayName = "Dungeon Ghost";
            GhostDungeon.addCharacter((NPC) m);
        }

        public Item getRandomItem(int i)
        {
            switch (i)
            {
                case 1:
                    RewardItem = (Item)new Object(74, 1);
                    break;
                case 2:
                    RewardItem = (Item)new Object(107, 1);
                    break;
                case 3:
                    RewardItem = (Item)new Object(166, 1);
                    break;
                default:
                case 4:
                    RewardItem = (Item)new Object(275, 5);
                    break;
                case 5:
                    RewardItem = (Item)new Object(337, 3);
                    break;
                case 6:
                    RewardItem = (Item)new Object(373, 1);
                    break;
                case 7:
                    RewardItem = (Item)new Object(797, 1);
                    break;
            }
            return RewardItem;
        }
    }
}
