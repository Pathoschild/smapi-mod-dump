/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;
using System.Linq;
using Bpendragon.GreenhouseSprinklers.Data;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            if (GetUpgradeLevel(gh) >= 1)
            {
                Monitor.Log("Watering the Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if(GetUpgradeLevel(gh) >= 3)
            {
                Monitor.Log("Watering entire farm", LogLevel.Info);
                WaterFarm();
            }
        }

        internal void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();

            AddLetterIfNeeded(GetUpgradeLevel(gh));

            Monitor.Log("Day ending");
            if (GetUpgradeLevel(gh) >= 2) //run these checks before we check for upgrades
            {
                Monitor.Log("Watering the Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if (GetUpgradeLevel(gh) >= 3)
            {
                Monitor.Log("Watering entire farm", LogLevel.Info);
                WaterFarm();
            }
        }  


        private void AddLetterIfNeeded(int curLevel)
        {
            if (curLevel >= Config.MaxNumberOfUpgrades) return; //We've upgraded as far as the user wants to go
            bool canRecieveMail = true;
            bool jojaMember = Game1.player.hasOrWillReceiveMail("jojaMember");
            //Check if has forsaken the Junimos
            if (jojaMember)
            {
                canRecieveMail = Game1.getFarm().buildings.Any(x => x is JunimoHut);
            } 

            if(canRecieveMail)
            {
                var requirements = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                if(!Game1.player.friendshipData.TryGetValue("Wizard", out var wizard))
                {
                    return; //Haven't ever talked to the Wizard, thus can't send us mail
                }
                switch(curLevel)
                {
                    case 0: if(wizard.Points >= 250 * requirements.FirstUpgrade.Hearts && !(Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1") || (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1b")) && Config.MaxNumberOfUpgrades >= 1))
                        {
                            if (jojaMember) Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1b");
                            else Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1");
                        }
                        break;
                    case 1:
                        if (wizard.Points >= 250 * requirements.SecondUpgrade.Hearts && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard2") && Config.MaxNumberOfUpgrades >= 2)
                        {
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard2");
                        }
                        break;
                    case 2:
                        if (wizard.Points >= 250 * requirements.FinalUpgrade.Hearts && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard3") && Config.MaxNumberOfUpgrades >= 3)
                        {
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard3");
                        }
                        break;
                }
            }
        }
    }
}
