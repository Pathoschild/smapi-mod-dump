using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AutomatedDoors
{

    public class AutomatedDoors : Mod
    {
        private AutomatedDoorsConfig _config;

        private bool openDoorsEventFired;
        private bool closeDoorsEventFired;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<AutomatedDoorsConfig>();
            TimeEvents.AfterDayStarted += Events_NewDay;
            GameEvents.OneSecondTick += Events_OneSecondTick;
        }


        public void Events_NewDay(object sender, EventArgs e)
        {
            openDoorsEventFired = false;
            closeDoorsEventFired = false; 
        }

        public void Events_OneSecondTick(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
            {
                return;
            }

            if (!_config.Buildings.ContainsKey(Game1.player.name))
            {
                _config.Buildings[Game1.player.name] = new Dictionary<string, bool>();
            }
            
            if (!openDoorsEventFired && Game1.timeOfDay == _config.TimeDoorsOpen && Game1.IsWinter == _config.OpenInWinter)
            {
                    if (_config.OpenOnRainyDays == true)
                    {
                        OpenBuildingDoors();
                    }
                    else if (Game1.isRaining == false && Game1.isLightning == false)
                    {
                        OpenBuildingDoors();
                    }
                }
             if (!closeDoorsEventFired && Game1.timeOfDay >= _config.TimeDoorsClose)
                {
                    CloseBuildingDoors();
                }
            
            
        }

        public void OpenBuildingDoors()
        {
            using (var enumerator = Game1.getFarm().buildings.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Building current = enumerator.Current;
                    if ( !_config.Buildings[Game1.player.Name].ContainsKey(current.nameOfIndoors) )
                    {
                        _config.Buildings[Game1.player.name].Add(current.nameOfIndoors, true);

                        if (current.animalDoorOpen == false)
                        {
                            current.doAction(new Vector2(current.animalDoor.X + current.tileX, current.animalDoor.Y + current.tileY), Game1.player);
                            openDoorsEventFired = true;
                        }
                    }
                    else if (_config.Buildings[Game1.player.name].ContainsKey(current.nameOfIndoors))
                    {
                        if (current.animalDoorOpen == false && _config.Buildings[Game1.player.name][current.nameOfIndoors] == true )
                        {
                            current.doAction(new Vector2(current.animalDoor.X + current.tileX, current.animalDoor.Y + current.tileY), Game1.player);
                            openDoorsEventFired = true;
                        }
                    }
                }
            }

            this.Helper.WriteConfig(_config);
        }

        public void CloseBuildingDoors()
        {
            using (var enumerator = Game1.getFarm().buildings.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Building current = enumerator.Current;
                    if (current.animalDoorOpen)
                    {
                        current.doAction(new Vector2(current.animalDoor.X + current.tileX, current.animalDoor.Y + current.tileY), Game1.player);
                        closeDoorsEventFired = true;
                    }
                }
            }
        }
    }
}