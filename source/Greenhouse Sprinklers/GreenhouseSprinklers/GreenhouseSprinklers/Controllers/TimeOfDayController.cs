/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        internal void OnDayStart(object sender, DayStartedEventArgs e)
        {   
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            Monitor.Log($"OnDayStart hit. Greenhouse Level {GetUpgradeLevel(gh)}");
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
            Monitor.Log($"OnDayEnding hit. Greenhouse Level {GetUpgradeLevel(gh)}");
            if(!(Game1.player.hasOrWillReceiveMail("ccPantry") || Game1.player.hasOrWillReceiveMail("jojaPantry")))
            {
                Monitor.Log("Player has not unlocked the Greenhouse, further checks skipped");
                return;
            }
            AddLetterIfNeeded(GetUpgradeLevel(gh));

            if (gh.buildingType.Value.StartsWith("GreenhouseSprinklers"))
            {
                gh.buildingType.Set("Greenhouse");
            }

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
            Monitor.Log($"Checking if need to add letter, level {curLevel}");
            if (curLevel >= Config.MaxNumberOfUpgrades)
            {
                Monitor.Log($"Upgraded as far as the user wants to go. No more upgrades.");
                return; //We've upgraded as far as the user wants to go
            }
            bool canReceiveMail = true;
            bool jojaMember = Game1.player.hasOrWillReceiveMail("jojaMember");
            //Check if has forsaken the Junimos
            if (jojaMember)
            {
                Monitor.Log($"Player has Joja membership. Checking additional pre-requisites");
                canReceiveMail = Game1.getFarm().buildings.Any(x => x is JunimoHut);
                Monitor.Log($"Junimo Hut found: {canReceiveMail}");
            }

            if (canReceiveMail)
            {
                var requirements = Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
                if(!Game1.player.friendshipData.TryGetValue("Wizard", out var wizard))
                {
                    Monitor.Log($"Player has not talked to the wizard. Can't get first mail.");
                    return; //Haven't ever talked to the Wizard, thus can't send us mail
                }
                switch(curLevel)
                {
                    case 0: 
                        if(wizard.Points >= 250 * requirements.FirstUpgrade.Hearts && !(Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1") || (Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1b")) && Config.MaxNumberOfUpgrades >= 1))
                        {
                            Monitor.Log($"Player meets requirements for first upgrade, sending letter.");
                            if (jojaMember) Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1b");
                            else Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard1");
                        } else Monitor.Log($"Player fails requirements for first upgrade, not sending letter.");
                        break;
                    case 1:
                        if (wizard.Points >= 250 * requirements.SecondUpgrade.Hearts && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard2") && Config.MaxNumberOfUpgrades >= 2)
                        {
                            Monitor.Log($"Player meets requirements for second upgrade, sending letter.");
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard2");
                        } else Monitor.Log($"Player fails requirements for second upgrade, not sending letter.");
                        break;
                    case 2:
                        if (wizard.Points >= 250 * requirements.FinalUpgrade.Hearts && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard3") && Config.MaxNumberOfUpgrades >= 3)
                        {
                            Monitor.Log($"Player meets requirements for final upgrade, sending letter.");
                            Game1.addMailForTomorrow("Bpendragon.GreenhouseSprinklers.Wizard3");
                        } else Monitor.Log($"Player fails requirements for final upgrade, not sending letter.");
                        break;
                }
            }
        }
    }
}
