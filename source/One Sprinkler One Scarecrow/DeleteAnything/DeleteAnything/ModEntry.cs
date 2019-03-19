using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DeleteAnything.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;
using StardewValley.BellsAndWhistles;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;


namespace DeleteAnything
{    
    public class ModEntry : Mod
    {
        //public List<Vector2> coords = new List<Vector2>();
        public Vector2 coords;
        public Vector2 curTile;
        //public Dictionary<Vector2, SObject> furniture;
        //private SObject @object;
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
        }

        public void GameEvents_UpdateTick(object sender, EventArgs e)
        {           
        }
        public void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if(!Context.IsWorldReady)
                return;
            if (e.KeyPressed == Keys.Delete)
            {
                GameLocation curLocation = Game1.currentLocation;
               // FarmHouse house = curLocation is FarmHouse ? curLocation as FarmHouse : null;
                //Populate furniture dictionary.
                //furniture = house.furniture;
                Dictionary<Vector2, Furniture> furniture = new Dictionary<Vector2, Furniture>();
                curTile = new Vector2((Game1.getMouseX() + Game1.viewport.X) / Game1.tileSize, (Game1.getMouseY() + Game1.viewport.Y) / Game1.tileSize);
                //coords = curTile;
                /*
                if(house != null)
                {
                    foreach(Furniture f in house.furniture)
                    {
                        furniture.Add(new Vector2(f.tileLocation.X, f.tileLocation.Y), f);

                    }
                }*/

                bool occupied = curLocation.isTileOccupied(curTile);
                bool t = curLocation is FarmHouse ? true : false;

                curLocation.Objects.TryGetValue(curTile, out SObject @object);
                curLocation.terrainFeatures.TryGetValue(curTile, out TerrainFeature @terrain);

                if (@object != null)
                {
                    List<Response> options = new List<Response>()
                    {
                        new Response("1", "Yes"),
                        new Response("2", "No")
                    };
                    curLocation.createQuestionDialogue($"Are you sure you want to delete {@object.Name}?", options.ToArray(), new GameLocation.afterQuestionBehavior(this.answer), (NPC)null);
                }
                else if(@terrain != null)
                {      
                    List<Response> options = new List<Response>()
                    {
                        new Response("3", "Yes"),
                        new Response("4", "No")
                    };
                    Game1.currentLocation.createQuestionDialogue($"Are you sure you want to delete this item?", options.ToArray(), new GameLocation.afterQuestionBehavior(this.answer), (NPC)null);                    
                }                
                this.Monitor.Log($"X:{curTile.X} Y:{curTile.Y} FarmHouse: {t} Occupied: {occupied}\n\n", LogLevel.Alert);                
            }

        }
        private void DoWork(GameLocation[] locations)
        {
            List<Response> options = new List<Response>();
            options.Add(new Response("1", "Yes"));
            options.Add(new Response("2", "No"));
            //Game1.drawObjectQuestionDialogue("Are you sure you want to delete this?", options);
            Game1.currentLocation.createQuestionDialogue("Are you sure you want to delete this?", options.ToArray(), new GameLocation.afterQuestionBehavior(this.answer), (NPC)null);
        }
        public void answer(SFarmer who, string answer)
        {
            string[] str = answer.Split(' ');
            /*
            switch (str[0])
            {
                case "1":
                    Game1.currentLocation.Objects.Remove(coords);
                    break;
                case "3":
                    Game1.currentLocation.terrainFeatures.Remove(coords);
                    break;
                case "2":
                default:
                    break;
            }*/
            if (str[0].Equals("1"))
            {
                Game1.currentLocation.Objects.Remove(curTile);
            }
            if (str[0].Equals("3"))
            {
                Game1.currentLocation.terrainFeatures.Remove(curTile);
            }
        }
    }
}