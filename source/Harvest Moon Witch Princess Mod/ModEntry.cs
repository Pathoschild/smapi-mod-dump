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
            LocationEvents.CurrentLocationChanged += this.LocationEvents_CurrentLocationChanged;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged2;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (e.NewLocation is FarmHouse && Game1.player.spouse == "Wizard")
                this.LoadSpouseRoom();
        }


        private void MenuEvents_MenuChanged2(object sender, EventArgsClickableMenuChanged e)
        {
            // check if player is married
            if (Game1.player.spouse != "Wizard")
                return;

            // get Wizard dialogue
            var dialogue = e.NewMenu as DialogueBox;
            if (dialogue == null || Game1.currentSpeaker?.name != "Wizard")
                return;

            // get dialogue text
            var dialogueStr = dialogue?.getCurrentString();


            // get new text
            int hearts = Game1.player.friendships["Wizard"][0] / NPC.friendshipPointsPerHeartLevel;
            string NewText2 = null;
            if (Game1.currentLocation is Farm && Game1.player.spouse == "Wizard" && (!dialogueStr.Contains("Wetter") && !dialogueStr.Contains("today") && !dialogueStr.Contains("weather") && !dialogueStr.Contains("天天") && !dialogueStr.Contains("dialogue") && !dialogueStr.Contains("fresh") && !dialogueStr.Contains("firsche") && !dialogueStr.Contains("空")))
            {
                if (hearts > 6)
                    NewText2 = this.Helper.Translation.Get("dialogue.outdoor");
                else
                    NewText2 = this.Helper.Translation.Get("dialogue.outdoor.low-friendship");
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
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            // check if player is married
            if (Game1.player.spouse != "Wizard")
                return;
            
            // get Wizard dialogue
            var dialogue = e.NewMenu as DialogueBox;
            if (dialogue == null || Game1.currentSpeaker?.name != "Wizard" || Game1.CurrentEvent?.FestivalName == null)
                return;

            // get dialogue text
            var dialogueStr = dialogue?.getCurrentString();


            // get new text
            int hearts = Game1.player.friendships["Wizard"][0] / NPC.friendshipPointsPerHeartLevel;
            string newText = null;
            if (this.IsFestival("summer", 11) && (dialogueStr.Contains("Las") || dialogueStr.Contains("日") || dialogueStr.Contains("Os") || dialogueStr.Contains("когда") || dialogueStr.Contains("curious") || dialogueStr.Contains("Festen") || dialogueStr.Contains("の"))) 
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.luau-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.luau-festival");
            }

            else if (this.IsFestival("summer", 28) && (dialogueStr.Contains("How") || dialogueStr.Contains("Cómo") || dialogueStr.Contains("Como") || dialogueStr.Contains("Как") || dialogueStr.Contains("么") || dialogueStr.Contains("Wie") || dialogueStr.Contains("の"))) 
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.jelly-festival.low-friendship");
                else if (hearts < 9)
                    newText = this.Helper.Translation.Get("dialogue.jelly-festival.medium-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.jelly-festival");
            }

            else if (this.IsFestival("winter", 25) && (dialogueStr.Contains("...") || dialogueStr.Contains("日") || dialogueStr.Contains("の"))) 
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.winter-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.winter-festival");
            }

            else if (this.IsFestival("winter", 8) && (dialogueStr.Contains("偷") || dialogueStr.Contains("me") || dialogueStr.Contains("Te") || dialogueStr.Contains("вот") || dialogueStr.Contains("torre") || dialogueStr.Contains("du") || dialogueStr.Contains("の"))) 
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.ice-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.ice-festival");

            }

            else if (this.IsFestival("spring", 24) && (dialogueStr.Contains("...") || dialogueStr.Contains("用") || dialogueStr.Contains("sí") || dialogueStr.Contains("の"))) 
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.flower-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.flower-festival");
            }

            else if (this.IsFestival("spring", 13) && (dialogueStr.Contains("唔") || dialogueStr.Contains("...") || dialogueStr.Contains("の")))
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.egg-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.egg-festival");
            }

            else if (this.IsFestival("fall", 27) && (dialogueStr.Contains("唔") || dialogueStr.Contains("...") || dialogueStr.Contains("の")))
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.haloween-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.haloween-festival");
            }

            else if (this.IsFestival("fall", 16) && (dialogueStr.Contains("唔") || dialogueStr.Contains("...") || dialogueStr.Contains("の")))
            {
                if (hearts < 6)
                    newText = this.Helper.Translation.Get("dialogue.fair-festival.low-friendship");
                else
                    newText = this.Helper.Translation.Get("dialogue.fair-festival");
            }

            // replace dialogue
            if (newText != null)
                Game1.activeClickableMenu = new DialogueBox(new Dialogue(newText, Game1.getCharacterFromName("Wizard")));
        }

        
        /// <summary>Add the witch princess' spouse room to the farmhouse.</summary>
        public void LoadSpouseRoom()
        {
            // get farmhouse
            FarmHouse farmhouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");

            // load custom map
            Map map = this.Helper.Content.Load<Map>(@"Content\WitchRoom.xnb");
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
