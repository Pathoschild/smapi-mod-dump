using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using StardewModdingAPI.Events;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace SeparateGreenhouse
{
    public class ModConfig
    {
        public SButton UpdateGreenhouses { get; set; } = SButton.F6;
    }
    class SeparateGreenhouseMod : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = (ModConfig)helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.SaveCreating += SaveCreating;
            Helper.Events.GameLoop.SaveLoaded += this.Loaded;
            Helper.Events.World.BuildingListChanged += this.Built; ;
            Helper.Events.Player.Warped += this.Warped;
            Helper.Events.Input.ButtonPressed += this.ButtonPressed;
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if(e.Button == this.Config.UpdateGreenhouses && Context.IsMainPlayer)
            {
                NetCollection<Building> ToBuild = new NetCollection<Building>();
                foreach (Building building in Game1.getFarm().buildings)
                {
                    bool create = true;
                    if (building.buildingType.Value.Contains("Cabin"))
                    {
                        foreach(Building greenhouse in Game1.getFarm().buildings)
                        {
                            if(greenhouse.buildingType.Value.Equals("Greenhouse") && greenhouse.owner.Value == (building.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID)
                            {
                                create = false;
                                break;
                            }
                        }
                        if (create)
                        {
                            ToBuild.Add(CreateGreenhouse(building));
                        }
                    }
                }
                foreach (Building building in ToBuild)
                {
                    Game1.getFarm().buildings.Add(building);
                }
                Game1.chatBox.addMessage("Greanhouses Created: " + ToBuild.Count.ToString(), Color.White);
                NetCollection<Building> ToDestroy = new NetCollection<Building>();
                foreach (Building building in Game1.getFarm().buildings)
                {
                    bool destroy = true;
                    if (building.buildingType.Value.Equals("Greenhouse"))
                    {
                        foreach (Building cabin in Game1.getFarm().buildings)
                        {
                            if (cabin.buildingType.Value.Contains("Cabin") && building.owner.Value == (cabin.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID)
                            {
                                destroy = false;
                                break;
                            }
                        }
                        if (destroy)
                        {
                            ToDestroy.Add(building);
                        }
                    }
                }
                foreach (Building building in ToDestroy)
                {
                    Game1.getFarm().buildings.Remove(building);
                }
                Game1.chatBox.addMessage("Greanhouses Removed: " + ToBuild.Count.ToString(), Color.White);
            }
        }

        private void SaveCreating(object sender, SaveCreatingEventArgs e)
        {
            NetCollection<Building> ToBuild = new NetCollection<Building>();
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.buildingType.Value.Contains("Cabin"))
                {
                    ToBuild.Add(CreateGreenhouse(building));
                }
            }
            foreach(Building building in ToBuild)
            {
                Game1.getFarm().buildings.Add(building);
            }
            Game1.chatBox.addMessage("Greanhouses Created: " + ToBuild.Count.ToString(), Color.White);
        }

        private void Built(object sender, BuildingListChangedEventArgs e)
        {
            if (Context.IsMainPlayer && e.Location.IsFarm)
            {
                int count = Game1.getFarm().buildings.Count;
                foreach (Building building in e.Added)
                {
                    if (building.buildingType.Value.Contains("Cabin"))
                    {
                        Game1.getFarm().buildings.Add(CreateGreenhouse(building));
                    }
                }
                count = Game1.getFarm().buildings.Count - count;
                if (count != 0)
                {
                    Game1.chatBox.addMessage("Greanhouses Created: " + count.ToString(), Color.White);
                }
                count = Game1.getFarm().buildings.Count;
                foreach (Building building in e.Removed)
                {
                    if (building.buildingType.Value.Contains("Cabin"))
                    {
                        foreach(Building greenhouse in Game1.getFarm().buildings)
                        {
                            if (greenhouse.buildingType.Value.Equals("Greenhouse") && greenhouse.owner.Value == (building.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID)
                            {
                                Game1.getFarm().buildings.Remove(greenhouse);
                                break;
                            }
                        }

                    }
                }
                count = count - Game1.getFarm().buildings.Count;
                if (count != 0)
                {
                    Game1.chatBox.addMessage("Greanhouses Removed: " + count.ToString(), Color.White);
                }
            }
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsMainPlayer && e.NewLocation.Name.Equals("Greenhouse") && e.OldLocation.Name.Equals("Farm"))
            {
                foreach(Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value.Equals("Greenhouse") && building.owner.Value == Game1.player.UniqueMultiplayerID)
                    {
                        Game1.warpFarmer(building.nameOfIndoors, 10, 23, false);
                    }
                }
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                foreach(Building building in Game1.getFarm().buildings)
                {
                    if (building.buildingType.Value.Equals("Greenhouse"))
                    {
                        foreach(StardewValley.Object obj in building.indoors.Value.objects.Values)
                        {
                            obj.boundingBox.Value = new Rectangle((int)obj.TileLocation.X, (int)obj.TileLocation.Y, obj.boundingBox.Value.Width, obj.boundingBox.Value.Height);
                        }
                        building.indoors.Value.warps[0] = Game1.getLocationFromName("Greenhouse").warps[0];
                    }
                }
            }
        }

        private Building CreateGreenhouse(Building cabin)
        {
            Building Greenhouse = new Building(new BluePrint("Greenhouse"), new Vector2(-10000f, 0.0f));
            Greenhouse.owner.Value = (cabin.indoors.Value as Cabin).getFarmhand().Value.UniqueMultiplayerID;
            Greenhouse.indoors.Value.warps[0] = Game1.getLocationFromName("Greenhouse").warps[0];
            return Greenhouse;
        }
    }
}