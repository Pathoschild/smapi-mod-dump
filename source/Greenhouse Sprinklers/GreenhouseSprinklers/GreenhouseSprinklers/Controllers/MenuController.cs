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
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Bpendragon.GreenhouseSprinklers.Data;
using System.Linq;
using StardewValley.Buildings;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry
    {
        private readonly int MaxOccupantsID = -794738;
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            
            if (!Context.IsWorldReady) return; //World Hasn't Loaded yet, it's definitely not the menu we want
            if (e.NewMenu == null) return; //Menu was closed
            if (!Game1.getFarm().greenhouseUnlocked.Value) return; //Greenhouse has not been unlocked. You aren't gonna be able to add sprinklers to it. 
            if (!(e.NewMenu is CarpenterMenu)) return; //We aren't in a carpenter menu
            if (Helper.Reflection.GetField<bool>(e.NewMenu, "magicalConstruction").GetValue()) return; //We aren't in Robin's Carpenter menu
            Monitor.Log("In the Carpenter Menu, here's hoping");
            //Figure out which level of the Upgrade we already have to allow us to select the appropriate upgrade
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            int curLevel = GetUpgradeLevel(gh);
            int bluePrintLevel = GetUpgradeLevel(gh) + 1;
            if (curLevel > Config.MaxNumberOfUpgrades) return; //User decided they didn't want all the upgrades. 
            if (curLevel >= 3) return; //we've built the final upgrade, 
            
            //Don't add blueprint if we haven't recieved the letter from the wizard yet
            if (bluePrintLevel == 1 && !(Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1") || Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard1b"))) return;
            if (bluePrintLevel == 2 && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard2")) return;
            if (bluePrintLevel == 3 && !Game1.player.mailReceived.Contains("Bpendragon.GreenhouseSprinklers.Wizard3")) return;

            IList<BluePrint> blueprints = Helper.Reflection
                .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                .GetValue();

            blueprints.Add(GetBluePrint(bluePrintLevel));
            Monitor.Log("Blueprint should be added");
        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            Monitor.Log("Building list changed");
        }

        private BluePrint GetBluePrint(int level)
        {
            string desc;
            Dictionary<int, int> buildMats;
            int money;
            UpgradeCost cost =  Config.DifficultySettings.Find(x => x.Difficulty == difficulty);
            if (level == 1)
            {
                desc = I18n.CarpenterShop_FirstUpgradeDescription();
                money = cost.FirstUpgrade.Gold;
                buildMats = BuildMaterials1;

            }
            else if (level == 2)
            {
                desc = I18n.CarpenterShop_SecondUpgradeDescription();
                money = cost.SecondUpgrade.Gold;
                buildMats = BuildMaterials2;
            }
            else
            {
                desc = I18n.CarpenterShop_FinalUpgradeDescription();
                money = cost.FinalUpgrade.Gold;
                buildMats = BuildMaterials3;
            }
            return new BluePrint("Greenhouse")
            {
                displayName = I18n.CarpenterShop_BluePrintName(),
                description = desc,
                moneyRequired = money,
                nameOfBuildingToUpgrade = "Greenhouse",
                itemsRequired = buildMats,
                maxOccupants = MaxOccupantsID,
                blueprintType = "Upgrades",
                daysToConstruct = 2
            };
        }
    }
}
