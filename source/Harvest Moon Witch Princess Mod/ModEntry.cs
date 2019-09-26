using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace WitchPrincess
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += WarpedEventArgs;
            helper.Events.Display.MenuChanged += DisplayMenuChanged;
            helper.Events.Display.MenuChanged += DisplayMenuChanged2;
            helper.Events.GameLoop.DayStarted += WitchSpouse;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WarpedEventArgs(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            if (e.NewLocation is FarmHouse && Game1.player.isMarried() && Game1.player.spouse == "Wizard" && (Game1.player.HouseUpgradeLevel == 1 || Game1.player.HouseUpgradeLevel == 2))
                this.LoadSpouseRoom();
        }

        private void WitchSpouse(object sender, DayStartedEventArgs e)
        {

            if (Game1.player.spouse == "Wizard" && Game1.currentLocation.Name == "FarmHouse" && Game1.player.HouseUpgradeLevel == 1)
                this.LoadSpouseRoom();
            else if (Game1.player.spouse == "Wizard" && Game1.currentLocation.Name == "FarmHouse" && Game1.player.HouseUpgradeLevel == 2)
                this.LoadSpouseRoom();

        }


        private void DisplayMenuChanged2(object sender, MenuChangedEventArgs e)
        {
            // check if player is married
            if (Game1.player.spouse != "Wizard")
                return;

            // get Wizard dialogue
            DialogueBox dialogue = e.NewMenu as DialogueBox;
            if (dialogue == null)
                return;
            if (Game1.currentSpeaker?.Name != "Wizard")
                return;
            
            // get dialogue text
            var dialogueStr = dialogue?.getCurrentString();

            // get new text
            int hearts = Game1.player.friendshipData["Wizard"].Points / NPC.friendshipPointsPerHeartLevel;
            string NewText2 = null;
            if (Game1.currentLocation is Farm && (!(dialogueStr.StartsWith("今天天") || dialogueStr.StartsWith("The") || dialogueStr.StartsWith("Das Wetter") || dialogueStr.StartsWith("Die firsche") || dialogueStr.StartsWith("新鲜空气很好"))))
            {
                if (hearts > 6)
                    NewText2 = Helper.Translation.Get("dialogue.outdoor");
                else
                    NewText2 = Helper.Translation.Get("dialogue.outdoor.low-friendship");
            }

            // replace dialogue
            if (NewText2 != null)
                Game1.activeClickableMenu = new DialogueBox(new Dialogue(NewText2, Game1.getCharacterFromName("Wizard")));
        }


        private bool IsFestival(string season, int day)
        {
          return Game1.CurrentEvent.FestivalName != null && Game1.currentSeason == season && Game1.dayOfMonth == day;
        }

        /// <summary>The method called after the current menu changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DisplayMenuChanged(object sender, MenuChangedEventArgs e)
        {
            //check if it is Festival
            if (Game1.CurrentEvent?.isFestival == null)
                return;

            // get Wizard dialogue
            if (Game1.currentSpeaker?.Name != "Wizard")
                return;

            DialogueBox dialogue = e.NewMenu as DialogueBox;
            if (dialogue == null)
                return;

            // check if player is married
            if (Game1.player.spouse != "Wizard")
                return;

            //get dialogue text
            var DialogueStr = dialogue?.getCurrentString();

            //get the new text
            int hearts = Game1.player.friendshipData["Wizard"].Points / NPC.friendshipPointsPerHeartLevel;
            string NewText = null;
            if (this.IsFestival("spring", 13) && (DialogueStr.StartsWith("Hmm...") || DialogueStr.StartsWith("唔……")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.egg-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.egg-festival:low-friendship");
            }

            else if (this.IsFestival("spring", 24) && (DialogueStr.StartsWith("Tanzt du heute") || DialogueStr.StartsWith("Do you dance") || DialogueStr.StartsWith("你不该来")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.flower-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.flower-festival.low-friendship");
            }

            if (this.IsFestival("summer", 11) && (DialogueStr.StartsWith("Die Meermenschen") || DialogueStr.StartsWith("The merpeople") || DialogueStr.StartsWith("鱼群们对")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.luau-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.luau-festival.low-friendship");
            }

            else if (this.IsFestival("summer", 28) && (DialogueStr.StartsWith("Wie hast du") || DialogueStr.StartsWith("How did you") || DialogueStr.StartsWith("你是怎么找")))
            {
                if (hearts > 11)
                    NewText = Helper.Translation.Get("dialogue.jelly-festival");
                else if (hearts > 8)
                    NewText = Helper.Translation.Get("dialogue.jelly-festival.medium-friendship");
                else
                    NewText = Helper.Translation.Get("dialogue.jelly-festival.low-friendship");
            }

            else if (this.IsFestival("fall", 16) && (DialogueStr.StartsWith("Welwick") || DialogueStr.StartsWith("我和维尔")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.fair-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.fair-festival.low-friendship");
            }

            else if (this.IsFestival("fall", 27) && (DialogueStr.StartsWith("Die Angelegenheiten") || DialogueStr.StartsWith("The affairs of") || DialogueStr.StartsWith("尘世间的俗事我不")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.haloween-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.haloween-festival.low-friendship");
            }

            else if (this.IsFestival("winter", 8) && (DialogueStr.StartsWith("Schleichst du dich") || DialogueStr.StartsWith("Sneaking off to") || DialogueStr.StartsWith("偷偷溜出")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.ice-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.ice-festival.low-friendship");
            }

            else if (this.IsFestival("winter", 25) && (DialogueStr.StartsWith("Ah, der mysteriöse Winterstern") || DialogueStr.StartsWith("Ah, the mysterious Winter Star") || DialogueStr.StartsWith("啊，神秘的冬日星")))
            {
                if (hearts > 6)
                    NewText = Helper.Translation.Get("dialogue.winter-festival");
                else
                    NewText = Helper.Translation.Get("dialogue.winter-festival.low-friendship");
            }




            // replace dialogue
            if (NewText != null)
                Game1.activeClickableMenu = new DialogueBox(new Dialogue(NewText, Game1.getCharacterFromName("Wizard")));



        }


            /// <summary>Add the witch princess' spouse room to the farmhouse.</summary>
            public void LoadSpouseRoom()
        {
            if (Game1.player.isMarried())
            {
                // get farmhouse
                FarmHouse farmhouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");

                // load custom map
                Map map = Helper.Content.Load<Map>(@"Content\WitchRoom.xnb");
                TileSheet room = new TileSheet(farmhouse.map, this.Helper.Content.GetActualAssetKey(@"Content\SRWitch.xnb"), map.TileSheets[0].SheetSize, map.TileSheets[0].TileSize) { Id = "ZZZ-WIZARD-SPOUSE-ROOM" };
                farmhouse.map.AddTileSheet(room);
                farmhouse.map.LoadTileSheets(Game1.mapDisplayDevice);

                // patch farmhouse
                farmhouse.map.Properties.Remove("DayTiles");
                farmhouse.map.Properties.Remove("NightTiles");
                TileSheet roomForFarmhouse = farmhouse.map.TileSheets[farmhouse.map.TileSheets.IndexOf(room)];

                int num = 0;
                Point point = new Point(num % 5 * 6, num / 5 * 9);
                Rectangle staticTile = farmhouse.upgradeLevel == 1 ? new Rectangle(29, 1, 6, 9) : new Rectangle(35, 10, 6, 9);
                for (int i = 0; i < staticTile.Width; i++)
                {
                    for (int j = 0; j < staticTile.Height; j++)
                    {
                        if (map.GetLayer("Back").Tiles[point.X + i, point.Y + j] != null)
                            farmhouse.map.GetLayer("Back").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Back"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Back").Tiles[point.X + i, point.Y + j].TileIndex);
                        if (map.GetLayer("Buildings").Tiles[point.X + i, point.Y + j] == null)
                            farmhouse.map.GetLayer("Buildings").Tiles[staticTile.X + i, staticTile.Y + j] = null;
                        else
                            farmhouse.map.GetLayer("Buildings").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Buildings"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Buildings").Tiles[point.X + i, point.Y + j].TileIndex);
                        if (j < staticTile.Height - 1 && map.GetLayer("Front").Tiles[point.X + i, point.Y + j] != null)
                            farmhouse.map.GetLayer("Front").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Front"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Front").Tiles[point.X + i, point.Y + j].TileIndex);
                        else if (j < staticTile.Height - 1)
                            farmhouse.map.GetLayer("Front").Tiles[staticTile.X + i, staticTile.Y + j] = null;
                        if (i == 4 && j == 4)
                        {
                            try
                            {
                                KeyValuePair<string, PropertyValue> prop = new KeyValuePair<string, PropertyValue>("NoFurniture", new PropertyValue("T"));

                                farmhouse.map.GetLayer("Back").Tiles[staticTile.X + i, staticTile.Y + j].Properties.Add(prop);
                            }


                            catch
                            {
                                // ignore errors
                            }
                        }
                    }
                }
            }
        }
    }
}

