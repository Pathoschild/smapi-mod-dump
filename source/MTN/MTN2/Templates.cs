using MTN2.MapData;
using MTNWarp = MTN2.MapData.Warp;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    public class Templates {
        public void CreateTemplate(string type, IModHelper helper, IMonitor monitor) {
            switch (type) {
                case "Farm":
                    CreateFarmTemplate(helper);
                    return;
                case "GreenHouse":
                    CreateGreenhouseTemplate(helper);
                    return;
                default:
                    monitor.Log("Error. Invalid input.");
                    return;
            }
        }

        private void CreateGreenhouseTemplate(IModHelper helper) {
            CustomGreenHouse template = new CustomGreenHouse();
            template.Name = "Example";
            template.Folder = "Example";
            template.Version = 2.0f;
            template.GreenhouseMap = new MapFile("GreenHouse_Example");
            template.Enterance = new Structure(new Placement(), new Interaction(10, 23)) {
                Coordinates = null
            };
            helper.Data.WriteJsonFile("greenHouseType.json", template);
        }

        private void CreateFarmTemplate(IModHelper helper) {
            CustomFarm template = new CustomFarm();
            template.ID = 25;
            template.Name = "Example";
            template.DescriptionName = "Example Farm";
            template.DescriptionDetails = "A description that appears when the player hovers over the farm icon, as they are creating a new game.";
            template.Folder = "Example";
            template.Icon = "fileNameOfIcon.png";
            template.Version = 2.0f;
            template.CabinCapacity = 3;
            template.AllowClose = true;
            template.AllowSeperate = true;
            template.FarmMap = new MapFile("Farm_Example");
            //template.AdditionalMaps
            template.FarmHouse = new Structure(new Placement(3712.00f, 520.00f), new Interaction(64, 14));
            template.GreenHouse = new Structure(new Placement(1600.00f, 384.00f), new Interaction(28, 15));
            template.FarmCave = new Structure(new Placement(), new Interaction(34, 5)) {
                Coordinates = null
            };
            template.ShippingBin = new Structure(new Placement(), new Interaction(71, 14)) {
                Coordinates = null
            };
            template.MailBox = new Structure(new Placement(), new Interaction(68, 16)) {
                Coordinates = null
            };
            template.GrandpaShrine = new Structure(new Placement(), new Interaction(8, 7)) {
                Coordinates = null
            };
            template.RabbitShrine = new Structure(new Placement(), new Interaction(48, 6)) {
                Coordinates = null
            };
            template.PetWaterBowl = new Structure(new Placement(), new Interaction(54, 7)) {
                Coordinates = null
            };
            template.Neighbors = new List<Neighbor> {
                new Neighbor("Backwoods") {
                    WarpPoints = { new MTNWarp(13, 40, 117, 0) }
                },
                new Neighbor("BusStop") {
                    WarpPoints = { new MTNWarp(-1, 22, 155, 24), new MTNWarp(-1, 23, 155, 25) }
                },
                new Neighbor("Forest") {
                    WarpPoints = { new MTNWarp(67, -1, 116, 154) }
                }
            };
            template.ResourceClumps = new LargeDebris() {
                ResourceList = new List<Spawn> {
                    new Spawn() {

                    }
                }
            };
            template.Foraging = new Forage() {
                ResourceList = new List<Spawn> {
                    new Spawn() {

                    }
                }
            };
            template.Ores = new Ore() {
                ResourceList = new List<Spawn> {
                    new Spawn {

                    }
                }
            };
            helper.Data.WriteJsonFile("farmType.json", template);
        }
    }
}
