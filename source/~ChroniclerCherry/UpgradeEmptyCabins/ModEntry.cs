/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using UpgradeEmptyCabins.Framework;

namespace UpgradeEmptyCabins
{
    public class UpgradeCabins : Mod
    {
        private Config _config;
        public override void Entry(IModHelper h)
        {
            this._config = this.Helper.ReadConfig<Config>();

            this.Helper.ConsoleCommands.Add("upgrade_cabin", "If Robin is free, brings up the menu to upgrade cabins.", this.UpgradeCabinsCommand);
            this.Helper.ConsoleCommands.Add("remove_seed_boxes", "Removes seed boxes from all unclaimed cabins.", this.RemoveSeedBoxesCommand);
            this.Helper.ConsoleCommands.Add("remove_cabin_beds", "Removes beds from all unclaimed cabins.", this.RemoveCabinBedsCommand);
            this.Helper.ConsoleCommands.Add("renovate_cabins", "Removes cribs and adds all the extra rooms to all unclaimed cabins.", this.RenovateCabinsCommand);
            this.Helper.ConsoleCommands.Add("toggle_renovate", "Toggles a renovation for an unclaimed cabin.", this.ToggleRenovateCommand);
            this.Helper.ConsoleCommands.Add("set_crib_style", "Sets the crib style for an unclaimed cabin.", this.SetCribStyleCommand);

            this.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            this.Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = this.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
                return;

            api.RegisterModConfig(this.ModManifest, () => this._config = new Config(), () => this.Helper.WriteConfig(this._config));
            api.RegisterSimpleOption(this.ModManifest, "Instance Build", "Whether cabins are instantly upgraded", () => this._config.InstantBuild, val => this._config.InstantBuild = val);
        }

        private void SetCribStyleCommand(string arg1, string[] arg2)
        {
            string cabin = arg2[0] + " Cabin"; //"Plank","Stone","Log"
            int style = int.Parse(arg2[1]);

            foreach (var cab in ModUtility.GetCabins())
            {
                if (((Cabin)cab.indoors.Value).owner.Name != "")
                    continue;
                if (cab.buildingType.ToString() == cabin)
                {
                    ((Cabin)cab.indoors.Value).cribStyle.Set(style);
                    this.Monitor.Log("Cabin: " + cab.GetIndoorsName(), LogLevel.Info);
                    this.Monitor.Log("Cabin Type: " + cab.buildingType.Value, LogLevel.Info);
                    this.Monitor.Log("cribStyle: " + ((Cabin)cab.indoors.Value).cribStyle.Value, LogLevel.Info);
                }
            }
        }

        private void ToggleRenovateCommand(string arg1, string[] arg2)
        {
            string cabin = arg2[0] + " Cabin"; //"Plank","Stone","Log"
            string reno = arg2[1]; //"renovation_bedroom_open","renovation_southern_open","renovation_corner_open"

            foreach (var cab in ModUtility.GetCabins())
            {
                if (((Cabin)cab.indoors.Value).owner.Name != "")
                    continue;
                if (cab.buildingType.ToString() == cabin)
                {
                    var mail = ((Cabin)cab.indoors.Value).owner.mailReceived;
                    if (mail.Contains(reno))
                        mail.Remove(reno);
                    else
                        mail.Add(reno);
                    this.Monitor.Log("Cabin: " + cab.GetIndoorsName(), LogLevel.Info);
                    this.Monitor.Log("Cabin Type: " + cab.buildingType.Value, LogLevel.Info);
                    this.Monitor.Log("Flags: " + mail, LogLevel.Info);
                }
            }
        }

        private void RenovateCabinsCommand(string arg1, string[] arg2)
        {
            string[] renos = { "renovation_bedroom_open", "renovation_southern_open", "renovation_corner_open" };
            foreach (var cab in ModUtility.GetCabins())
            {
                if (((Cabin)cab.indoors.Value).owner.Name != "")
                    continue;
                var mail = ((Cabin)cab.indoors.Value).owner.mailReceived;
                foreach (string reno in renos)
                {
                    if (mail.Contains(reno))
                        this.Monitor.Log("Renovation already done: " + reno + " " + cab.GetIndoorsName(), LogLevel.Info);
                    else
                        mail.Add(reno);
                }
                ((Cabin)cab.indoors.Value).cribStyle.Set(0);
                this.Monitor.Log("Cabin: " + cab.GetIndoorsName(), LogLevel.Info);
                this.Monitor.Log("   Type: " + cab.buildingType.Value, LogLevel.Info);
                this.Monitor.Log("   cribStyle:  " + ((Cabin)cab.indoors.Value).cribStyle.Value, LogLevel.Info);
                this.Monitor.Log("   flags: " + mail, LogLevel.Info);
            }
        }

        private void RemoveCabinBedsCommand(string arg1, string[] arg2)
        {
            foreach (var cab in ModUtility.GetCabins())
            {
                BedFurniture bed = null;
                if (((Cabin)cab.indoors.Value).owner.Name != "")
                    continue;
                foreach (var furniture in ((Cabin)cab.indoors.Value).furniture)
                {
                    if (furniture is BedFurniture b)
                    {
                        bed = b;
                        break;
                    }
                }

                if (bed != null)
                {
                    ((Cabin)cab.indoors.Value).furniture.Remove(bed);
                    this.Monitor.Log("Bed removed from " + cab.GetIndoorsName(), LogLevel.Info);
                }
            }
        }

        private void RemoveSeedBoxesCommand(string arg1, string[] arg2)
        {
            foreach (var cab in ModUtility.GetCabins())
            {
                if (((Cabin)cab.indoors.Value).owner.Name != "")
                    continue;
                foreach (var obj in
                    ((Cabin)cab.indoors.Value).Objects.SelectMany(objs =>
                    objs.Where(obj => obj.Value is Chest).Select(obj => obj)))
                {
                    Chest chest = (Chest)obj.Value;
                    if (!chest.giftbox.Value || chest.bigCraftable.Value)
                    {
                        continue;
                    }

                    ((Cabin)cab.indoors.Value).Objects.Remove(obj.Key);
                    this.Monitor.Log("Seed box removed from " + cab.GetIndoorsName(), LogLevel.Info);
                }
            }
        }

        private void UpgradeCabinsCommand(string arg1, string[] arg2)
        {
            this.AskForUpgrade();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;

            if (Game1.currentLocation.Name != "ScienceHouse")
                return;

            if (this.Helper.Input.GetCursorPosition().GrabTile != new Vector2(6, 19))
                return;

            this.AskForUpgrade();

        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            //so this doesn't end up happening for every single player online....
            if (!Context.IsMainPlayer)
                return;

            foreach (var cabin in ModUtility.GetCabins())
            {
                if (cabin.daysUntilUpgrade.Value == 1)
                {
                    FinalUpgrade(cabin);
                }
            }
        }

        private static void FinalUpgrade(Building cabin)
        {
            var cabinIndoors = ((Cabin)cabin.indoors.Value);
            cabin.daysUntilUpgrade.Value = -1;
            cabinIndoors.moveObjectsForHouseUpgrade(cabinIndoors.upgradeLevel);
            cabinIndoors.setMapForUpgradeLevel(cabinIndoors.upgradeLevel);
            cabinIndoors.upgradeLevel++;
        }

        internal void AskForUpgrade()
        {
            if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
                Game1.drawObjectDialogue(this.Helper.Translation.Get("robin.busy"));
                return;
            }

            List<Response> cabinNames = new List<Response>();
            foreach (var cabin in ModUtility.GetCabins())
            {
                string displayInfo = null;
                var cabinIndoors = ((Cabin)cabin.indoors.Value);

                //if the cabin is occupied, we ignore it
                if (cabinIndoors.owner.Name != "")
                    continue;

                switch (cabinIndoors.upgradeLevel)
                {
                    case 0:
                        displayInfo = $"{cabin.buildingType.Value} {this.Helper.Translation.Get("robin.hu1_materials")}";
                        break;
                    case 1:
                        displayInfo = $"{cabin.buildingType.Value} {this.Helper.Translation.Get("robin.hu2_materials")}";
                        break;
                    case 2:
                        displayInfo = $"{cabin.buildingType.Value} {this.Helper.Translation.Get("robin.hu3_materials")}";
                        break;
                }
                if (displayInfo != null)
                    cabinNames.Add(new Response(cabin.GetIndoorsName(), displayInfo));
            }

            if (cabinNames.Count > 0)
            {
                cabinNames.Add(new Response("Cancel", this.Helper.Translation.Get("menu.cancel_option")));
                //Game1.activeClickableMenu = new CabinQuestionsBox("Which Cabin would you like to upgrade?", cabinNames);
                Game1.currentLocation.createQuestionDialogue(this.Helper.Translation.Get("robin.whichcabin_question"),
                                cabinNames.ToArray(),
                                delegate (Farmer who, string answer)
                                {
                                    Game1.activeClickableMenu = null;
                                    this.HouseUpgradeAccept(ModUtility.GetCabin(answer));
                                }
                                );
            }
        }

        internal void HouseUpgradeAccept(Building cab)
        {
            Game1.activeClickableMenu = null;
            Game1.player.canMove = true;
            if (cab == null)
            {
                Game1.playSound("smallSelect");
                return;
            }

            if (this._config.InstantBuild)
            {
                FinalUpgrade(cab);
                Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                return;
            }

            var cabin = ((Cabin)cab.indoors.Value);


            switch (cabin.upgradeLevel)
            {
                case 0:
                    if (Game1.player.Money >= 10000 && Game1.player.Items.ContainsId("(O)388", 450))
                    {
                        cab.daysUntilUpgrade.Value = 3;
                        Game1.player.Money -= 10000;
                        Game1.player.Items.ReduceId("(O)388", 450);
                        Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                        break;
                    }
                    if (Game1.player.Money < 10000)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        break;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood1"));
                    break;
                case 1:
                    if (Game1.player.Money >= 50000 && Game1.player.Items.ContainsId("(O)709", 150))
                    {
                        cab.daysUntilUpgrade.Value = 3;
                        Game1.player.Money -= 50000;
                        Game1.player.Items.ReduceId("(O)709", 150);
                        Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                        break;
                    }
                    if (Game1.player.Money < 50000)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        break;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood2"));
                    break;
                case 2:
                    if (Game1.player.Money >= 100000)
                    {
                        cab.daysUntilUpgrade.Value = 3;
                        Game1.player.Money -= 100000;
                        Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                        break;
                    }
                    if (Game1.player.Money >= 100000)
                        break;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                    break;
            }
        }
    }
}
