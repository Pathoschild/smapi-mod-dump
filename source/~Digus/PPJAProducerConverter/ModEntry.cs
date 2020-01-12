using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = System.Object;

namespace PPJAProducerConverter
{
    public class ModEntry : Mod
    {
        private static Dictionary<string, string> OlderToNewMachines = new Dictionary<string, string>()
        {
            { "Artisan Valley.json.15" , "Alembic"}
            ,{ "Artisan Valley.json.24" , "Pepper Blender"}
            ,{ "Artisan Valley.json.4" , "Butter Churn"}
            ,{ "Artisan Valley.json.5" , "Dehydrator"}
            ,{ "Artisan Valley.json.6" , "Drying Rack"}
            ,{ "Artisan Valley.json.23" , "Espresso Machine"}
            ,{ "Artisan Valley.json.8" , "Extruder"}
            ,{ "Artisan Valley.json.3" , "Foreign Cask"}
            ,{ "Artisan Valley.json.2" , "Glass Jar"}
            ,{ "Artisan Valley.json.9" , "Grinder"}
            ,{ "Artisan Valley.json.14" , "Ice Cream Machine"}
            ,{ "Artisan Valley.json.10" , "Infuser"}
            ,{ "Artisan Valley.json.17" , "Juicer"}
            ,{ "Artisan Valley.json.22" , "Smoker"}
            ,{ "Artisan Valley.json.19" , "Soap Press"}
            ,{ "Artisan Valley.json.0" , "Still"}
            ,{ "Artisan Valley.json.13" , "Wax Barrel"}
            ,{ "Artisan Valley.json.12" , "Yogurt Jar"}
            ,{ "Farmer to Floris Machines Redux.json.0" , "Dryer"}
            ,{ "Farmer to Floris Machines Redux.json.1" , "Perfumery"}
            ,{ "Farmer to Floris Machines Redux.json.2" , "Soap Maker"}
            ,{ "Artisan Valley.json.7" , "Oil Maker"}
            ,{ "Artisan Valley.json.21" , "Mayonnaise Machine"}
            ,{ "Artisan Valley.json.1" , "Keg"}
            ,{ "Artisan Valley.json.11" , "Loom"}
            ,{ "Artisan Valley.json.16" , "Vinegar Cask"}
            ,{"New Machines.New_Machines.json.12", "Loom"}
            ,{"New Machines.New_Machines.json.21", "Cask"}
            ,{"New Machines.New_Machines.json.18", "Cheese Press"}
            ,{"Recovery Machine.RecoveryMachine.json.20","Recycling Machine"}
            ,{"Manure Handling Machines.Manure_Machines.json.14","Manure Grinder"}
            ,{"Manure Handling Machines.Manure_Machines.json.17","Fertilizer Mixer"}
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += ReplaceItemAnywhere;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            OlderToNewMachines = Helper.Data.ReadJsonFile<Dictionary<string, string>>("data\\MachinesFromTo.json") ?? OlderToNewMachines;
            Helper.Data.WriteJsonFile("data\\MachinesFromTo.json", OlderToNewMachines);
        }

        public void ReplaceItemAnywhere(object sender, SaveLoadedEventArgs args)
        {
            var bigObjects = Helper.Content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", ContentSource.GameContent);
            foreach (Farmer player in Game1.getAllFarmers())
            {
                for (int index1 = player.Items.Count - 1; index1 >= 0; --index1)
                {
                    if (player.Items[index1] != null)
                    {
                        KeyValuePair<string, string> machine = OlderToNewMachines.FirstOrDefault(i => player.Items[index1].Name.Contains(i.Key + "|"));
                        if (machine.Key != null)
                        {
                            KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(machine.Value + "/"));
                            if (pair.Value != null)
                            {
                                player.Items[index1] = new StardewValley.Object(Vector2.Zero, pair.Key);
                            }
                        }
                    }
                        
                }
            }
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> positionObject in location.objects.Pairs)
                {
                    if (positionObject.Value != null)
                    {
                        
                        KeyValuePair<string, string> machine = OlderToNewMachines.FirstOrDefault(i => positionObject.Value.Name.Contains(i.Key + "|"));
                        if (machine.Key != null)
                        {
                            
                            KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(machine.Value + "/"));
                            if (pair.Value != null)
                            {
                                location.objects[positionObject.Key] = new StardewValley.Object(Vector2.Zero, pair.Key);
                            }
                        }
                        else if (positionObject.Value is Chest)
                        {
                            foreach (Item obj in ((NetList<Item, NetRef<Item>>)(positionObject.Value as Chest).items).ToList())
                            {
                                if (obj != null)
                                {
                                    KeyValuePair<string, string> machine2 = OlderToNewMachines.FirstOrDefault(i => obj.Name.Contains(i.Key + "|"));
                                    if (machine2.Key != null)
                                    {
                                        KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(machine2.Value + "/"));
                                        if (pair.Value != null)
                                        {
                                            (positionObject.Value as Chest).items.Remove(obj);
                                            (positionObject.Value as Chest).items.Add(new StardewValley.Object(Vector2.Zero, pair.Key));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (location is Farm)
                {
                    foreach (Building building in (location as Farm).buildings)
                    {
                        if (building.indoors.Value != null)
                        {
                            foreach (KeyValuePair<Vector2, StardewValley.Object> locationObject in building.indoors.Value.objects.Pairs)
                            {
                                if (locationObject.Value != null)
                                {
                                    KeyValuePair<string, string> machine2 = OlderToNewMachines.FirstOrDefault(i => locationObject.Value.Name.Contains(i.Key + "|"));
                                    if (machine2.Key != null)
                                    {
                                        KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(machine2.Value + "/"));
                                        if (pair.Value != null)
                                        {
                                            building.indoors.Value.objects[locationObject.Key] = new StardewValley.Object(Vector2.Zero, pair.Key);
                                        }
                                    }
                                    else if (locationObject.Value is Chest)
                                    {
                                        foreach (Item obj in ((NetList<Item, NetRef<Item>>)(locationObject.Value as Chest).items).ToList())
                                        {
                                            if (obj != null)
                                            {
                                                KeyValuePair<string, string> machine3 = OlderToNewMachines.FirstOrDefault(i => obj.Name.Contains(i.Key + "|"));
                                                if (machine3.Key != null)
                                                {
                                                    KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(machine3.Value + "/"));
                                                    if (pair.Value != null)
                                                    {
                                                        (locationObject.Value as Chest).items.Remove(obj);
                                                        (locationObject.Value as Chest).items.Add(new StardewValley.Object(Vector2.Zero, pair.Key));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
