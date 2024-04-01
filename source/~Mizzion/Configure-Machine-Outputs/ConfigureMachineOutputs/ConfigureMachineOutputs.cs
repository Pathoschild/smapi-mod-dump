/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using ConfigureMachineOutputs.Framework.Configs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConfigureMachineOutputs
{
    public class ConfigureMachineOutputs : Mod
    {
        private CmoConfig _config;
        //public static readonly Type[] PatchedTypes = { typeof(Furniture), typeof(Wallpaper) };
        private bool _debugging = true;

        
        public override void Entry(IModHelper helper)
        {
           //toDo _config = helper.ReadConfig<CmoConfig>();

            //Events
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.GameLaunched += GameLaunched;

            //Make sure Customized Crystalarium mod isn't installed.
            if (helper.ModRegistry.IsLoaded("DIGUS.CustomCrystalariumMod"))
            {
                Monitor.Log("Due to incompatability issues with Customizable Crystalarium, the Crystalarium has been turned off for this mod. This way you can use both at the same time.", LogLevel.Info);
                //_config.Machines.Crystalarium.Enabled = false;
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {

        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.NumPad8) && _debugging)
            {
                var locations = getLocations();

                foreach (var loc in locations)
                {
                    foreach (var obj in loc.objects.Values)
                    {

                        if (obj.heldObject.Value != null)
                        {
                            obj.MinutesUntilReady = 10;
                            Monitor.Log("Set all objects to 10 minutes.");
                        }
                    }
                }
                locations.Clear();
            }

            if (e.IsDown(SButton.NumPad7) && _debugging)
            {
                var objs = DataLoader.Objects(Game1.content);
                
                

                foreach(var o in objs)
                {
                    Monitor.Log($"(O){o.Key} {o.Value.Name}");
                    
                }
            }

            if (e.IsDown(SButton.NumPad9) && _debugging)
            {
                var machines = DataLoader.Machines(Game1.content);
                var bigCraftables = Game1.bigCraftableData;

                foreach (var machine in machines)
                {
                    var machineIdSplitter = machine.Key.Split(')');
                    var machineId = bigCraftables[machineIdSplitter[1]];
                    Monitor.Log($"Key: {machine.Key} Machine Name: {machineId.Name}");

                    
                    
                    switch (machine.Key)
                    {
                        case "(BC)9":

                            break;
                        case "(BC)10":

                            break;
                        case "(BC)12":

                            break;
                        case "(BC)13":

                            break;
                        case "(BC)15":

                            break;
                        case "(BC)16":

                            break;
                        case "(BC)17":

                            break;
                        case "(BC)19":

                            break;
                        case "(BC)20":
                            Monitor.Log("recycle edit should have happened. But its not coded lol");
                            break;
                        case "(BC)21":

                            break;
                        case "(BC)24":

                            break;
                        case "(BC)25":

                            break;
                        case "(BC)90":

                            break;
                        case "(BC)101":

                            break;
                        case "(BC)105":

                            break;
                        case "(BC)114":

                            break;
                        case "(BC)117":

                            break;
                        case "(BC)127":

                            break;
                        case "(BC)128":

                            break;
                        case "(BC)154":

                            break;
                        case "(BC)156":

                            break;
                        case "(BC)158":

                            break;
                        case "(BC)160":

                            break;
                        case "(BC)163":

                            break;
                        case "(BC)182":

                            break;
                        case "(BC)211":

                            break;
                        case "(BC)231":

                            break;
                        case "(BC)246":

                            break;
                        case "(BC)254":

                            break;
                        case "(BC)264":

                            break;
                        case "(BC)265":

                            break;
                        case "(BC)280":

                            break;
                        default:
                            Monitor.Log("Machine edits failed");
                            break;

                    }

                        
                }

                
            }

            if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<CmoConfig>();
                Monitor.Log("The Config file was reloaded.", LogLevel.Info);
            }
        }


        private void EditMachine(string QualityId)
        {
            var machines = DataLoader.Machines(Game1.content);
            
            foreach (var machine in machines)
            {
                if (machine.Key != QualityId)
                    continue;

                foreach (var outPutRules in machine.Value.OutputRules)
                {
                    foreach (var triggers in outPutRules.Triggers)
                    {
                        //triggers.RequiredCount
                    }
                }
            }
        }

        private List<GameLocation> getLocations()
        {
            var location = new List<GameLocation>();
            foreach (var loc in Game1.locations)
            {
                if(!location.Contains(loc))
                    location.Add(loc);

                foreach (var building in loc.buildings)
                {
                    if (!loc.IsBuildableLocation() || building.indoors.Value != null)
                        continue;

                    location.Add(building.indoors.Value);
                }
            }
            return location;
        }

        
    }
}
