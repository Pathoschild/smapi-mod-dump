using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using PyTK.Extensions;
using PyTK.Types;
using PyTK.CustomElementHandler;
using PyTK.CustomTV;
using PyTK;
using StardewValley.Network;
using System.Collections.Generic;
using System.Linq;

namespace ServerCode
{
    //todo
    // add warning in smapi if you have wrong mods installed
    // set up wrong mods kick
    // use "Mymod.Myaddress" to make sure peopel stay up to date



    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private int numPlayers = 0; //stores number of players
        private int gameClockTicks; //stores in game clock change
        private int secondTicks = 0;
        bool sendID;


        // private bool shipTicker = false;
        // private int shipTicks = 0;


        public override void Entry(IModHelper helper)
        {
            // MultiplayerEvents.BeforeMainSync += MultiplayerEvents_BeforeMainSync; 
            //  SaveEvents.BeforeSave += this.Shipping_Menu; // Shipping Menu handler
            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged; // Time of day change handler
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;

            bool consoleCommandsIsLoaded = this.Helper.ModRegistry.IsLoaded("SMAPI.ConsoleCommands");
            bool cJBCheatsMenuIsLoaded = this.Helper.ModRegistry.IsLoaded("CJBok.CheatsMenu");
            bool bcmpincMovementSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("bcmpinc.MovementSpeed");
            bool bcmpincHarvestWithScytheIsLoaded = this.Helper.ModRegistry.IsLoaded("bcmpinc.HarvestWithScythe");
            bool dewModsStardewValleyModsPhoneVillagersIsLoaded = this.Helper.ModRegistry.IsLoaded("DewMods.StardewValleyMods.PhoneVillagers");
            bool vylusPelicanPostalServiceIsLoaded = this.Helper.ModRegistry.IsLoaded("Vylus.PelicanPostalService");
            bool crazywigtoolpowerselectIsLoaded = this.Helper.ModRegistry.IsLoaded("crazywig.toolpowerselect");
            bool defenTheNationTimeMultiplierIsLoaded = this.Helper.ModRegistry.IsLoaded("DefenTheNation.TimeMultiplier");
            bool scythHarvestIsLoaded = this.Helper.ModRegistry.IsLoaded("965169fd-e1ed-47d0-9f12-b104535fb4bc");
            bool omegasisAutoSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("Omegasis.AutoSpeed");
            bool cantorsdustTimeSpeedIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.TimeSpeed");
            bool cJBokItemSpawnerIsLoaded = this.Helper.ModRegistry.IsLoaded("CJBok.ItemSpawner");
            bool pathoschildChestsAnywhereIsLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere");
            bool dewModsStardewValleyModsSkipFishingMinigameIsLoaded = this.Helper.ModRegistry.IsLoaded("DewMods.StardewValleyMods.SkipFishingMinigame");
            bool shalankwaWarpToFriendsIsLoaded = this.Helper.ModRegistry.IsLoaded("Shalankwa.WarpToFriends");
            bool punyoPassableObjectsIsLoaded = this.Helper.ModRegistry.IsLoaded("punyo.PassableObjects");
            bool defenTheNationBackpackResizerdIsLoaded = this.Helper.ModRegistry.IsLoaded("DefenTheNation.BackpackResizer");
            bool dWhiteMindAFIsLoaded = this.Helper.ModRegistry.IsLoaded("WhiteMind.AF");
            bool mucchanPrairieKingMadeEasyIsLoaded = this.Helper.ModRegistry.IsLoaded("Mucchan.PrairieKingMadeEasy");
            bool jwdredAnimalSitterIsLoaded = this.Helper.ModRegistry.IsLoaded("jwdred.AnimalSitter");
            bool cantorsdustAllProfessionsIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.AllProfessions");
            bool mod451IsLoaded = this.Helper.ModRegistry.IsLoaded("451");
            bool cantorsdustAllCropsAllSeasonsIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.AllCropsAllSeasons");
            bool drynwynnFishingAutomatonIsLoaded = this.Helper.ModRegistry.IsLoaded("Drynwynn.FishingAutomaton");
            bool cantorsdustRecatchLegendaryFishIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.RecatchLegendaryFish");
            bool kathrynHazukaFasterRunIsLoaded = this.Helper.ModRegistry.IsLoaded("KathrynHazuka.FasterRun");
            bool bitwiseJonModsInstantBuildingsIsLoaded = this.Helper.ModRegistry.IsLoaded("BitwiseJonMods.InstantBuildings");
            bool jotserAutoGrabberModIsLoaded = this.Helper.ModRegistry.IsLoaded("Jotser.AutoGrabberMod");
            bool dcantorsdustInstantGrowTreesIsLoaded = this.Helper.ModRegistry.IsLoaded("cantorsdust.InstantGrowTrees");
            bool jwdredPointAndPlantIsLoaded = this.Helper.ModRegistry.IsLoaded("jwdred.PointAndPlant");
            bool magicIsLoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.Magic");
            bool fastTravelIsLoaded = this.Helper.ModRegistry.IsLoaded("DeathGameDev.FastTravel");
            bool scytheHarvesting2IsLoaded = this.Helper.ModRegistry.IsLoaded("mmanlapat.ScytheHarvesting");
            bool SkullCavernElevatorIsLoaded = this.Helper.ModRegistry.IsLoaded("SkullCavernElevator");
            bool horseWhistleIsLoaded = this.Helper.ModRegistry.IsLoaded("icepuente.HorseWhistle");
            bool fasterHorseIsLoaded = this.Helper.ModRegistry.IsLoaded("kibbe.faster_horse");
            bool cropTransplantIsLoaded = this.Helper.ModRegistry.IsLoaded("DIGUS.CropTransplantMod");
            bool automateIsLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
   

            if (consoleCommandsIsLoaded == true || cJBCheatsMenuIsLoaded == true || bcmpincMovementSpeedIsLoaded == true || bcmpincHarvestWithScytheIsLoaded == true 
                || dewModsStardewValleyModsPhoneVillagersIsLoaded == true || vylusPelicanPostalServiceIsLoaded == true || crazywigtoolpowerselectIsLoaded == true
                || defenTheNationTimeMultiplierIsLoaded == true || scythHarvestIsLoaded == true || omegasisAutoSpeedIsLoaded == true || cantorsdustTimeSpeedIsLoaded == true 
                || cJBokItemSpawnerIsLoaded == true || pathoschildChestsAnywhereIsLoaded == true || dewModsStardewValleyModsSkipFishingMinigameIsLoaded == true
                || shalankwaWarpToFriendsIsLoaded == true || punyoPassableObjectsIsLoaded == true || defenTheNationBackpackResizerdIsLoaded == true 
                || dWhiteMindAFIsLoaded == true || mucchanPrairieKingMadeEasyIsLoaded == true || jwdredAnimalSitterIsLoaded == true
                || cantorsdustAllCropsAllSeasonsIsLoaded == true || mod451IsLoaded == true || drynwynnFishingAutomatonIsLoaded == true
                || cantorsdustRecatchLegendaryFishIsLoaded == true || kathrynHazukaFasterRunIsLoaded == true || bitwiseJonModsInstantBuildingsIsLoaded == true
                || jotserAutoGrabberModIsLoaded == true || dcantorsdustInstantGrowTreesIsLoaded == true || jwdredPointAndPlantIsLoaded == true || magicIsLoaded == true
                || fastTravelIsLoaded == true || scytheHarvesting2IsLoaded == true || SkullCavernElevatorIsLoaded == true
                || horseWhistleIsLoaded == true || fasterHorseIsLoaded == true || cropTransplantIsLoaded == true || automateIsLoaded == true)
            {
                sendID = false;
            }
            else
            {
                sendID = true;
            }
        }


        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {

 
            if (!Context.IsWorldReady)
                return;

            secondTicks += 1;
            if (secondTicks >= 1)
            {
                if (sendID == true)
                {
                    var playerID = Game1.player.UniqueMultiplayerID;

                    //send Messages
                    PyNet.sendMessage("anticheat.2", playerID);
                }
                else if (sendID == false)
                {
                    Game1.chatBox.addInfoMessage("Cheat Mods Detected!");
                    Game1.displayHUD = true;
                    Game1.addHUDMessage(new HUDMessage("Cheat Mods Detected!", ""));
                }

                secondTicks = 0;
            }

        }






        











        /* doesn't work for now, no timer in the ship menu, maybe figure out later////////////////////////////////////////////////////////////////////////////////////////////////////
                // timer for shipping ok click delay
                private void MultiplayerEvents_BeforeMainSync(object sender, EventArgs e)
                {
                    if (!Context.IsMultiplayer) return;

                    if (shipTicker == true)
                    {
                        shipTicks += 1;

                    }

                    if (shipTicker == false)
                    {
                        shipTicks = 0;
                    }

                }



                // shipping menu"OK" click through code
                private void Shipping_Menu(object sender, EventArgs e)
                {
                    shipTicker = true;

                    if (shipTicks == 1800)
                    {
                        this.Monitor.Log("This is the Shipping Menu");
                        this.Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();
                        shipTicker = false;
                    }

                }


        *//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        // Holiday code
        private void TimeEvents_TimeOfDayChanged(object sender, EventArgs e)
        {






            Game1.player.hasRustyKey = true;  //wallet addition


            this.numPlayers = Game1.otherFarmers.Count;

            gameClockTicks += 1;


            if (gameClockTicks >= 1)
            {
                var currentTime = Game1.timeOfDay;
                var currentDate = SDate.Now();
                var eggFestival = new SDate(13, "spring");
                var dayAfterEggFestival = new SDate(14, "spring");
                var flowerDance = new SDate(24, "spring");
                var luau = new SDate(11, "summer");
                var danceOfJellies = new SDate(28, "summer");
                var stardewValleyFair = new SDate(16, "fall");
                var spiritsEve = new SDate(27, "fall");
                var festivalOfIce = new SDate(8, "winter");
                var feastOfWinterStar = new SDate(25, "winter");



                if (currentDate == eggFestival && numPlayers >= 1)   //set back to 1 after testing~!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    EggFestival();
                }


                //flower dance changed to disabled bc it causes crashes
                else if (currentDate == flowerDance && numPlayers >= 1)
                {
                    // FlowerDance();
                }

                else if (currentDate == luau && numPlayers >= 1)
                {
                    Luau();
                }

                else if (currentDate == danceOfJellies && numPlayers >= 1)
                {
                    DanceOfTheMoonlightJellies();
                }

                else if (currentDate == stardewValleyFair && numPlayers >= 1)
                {
                    StardewValleyFair();
                }

                else if (currentDate == spiritsEve && numPlayers >= 1)
                {
                    SpiritsEve();
                }

                else if (currentDate == festivalOfIce && numPlayers >= 1)
                {
                    FestivalOfIce();
                }

                else if (currentDate == feastOfWinterStar && numPlayers >= 1)
                {
                    FeastOfWinterStar();
                }



                gameClockTicks = 0;   // never reaches rest of code bc gameClockTicks is reset to 0, these methods below are called higher up.




                void EggFestival()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {



                        //teleports to egg festival
                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                // flower dance turned off causes game crashes
                /*void FlowerDance()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {

                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Forest", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        flowerDanceAvailable = true;

                    }

                }*/

                void Luau()
                {

                    if (currentTime >= 900 && currentTime <= 1400)
                    {

                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Beach", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void DanceOfTheMoonlightJellies()
                {



                    if (currentTime >= 2200 && currentTime <= 2400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Beach", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void StardewValleyFair()
                {
                    if (currentTime >= 900 && currentTime <= 1500)
                    {



                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void SpiritsEve()
                {


                    if (currentTime >= 2200 && currentTime <= 2350)
                    {



                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void FestivalOfIce()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Forest", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

                void FeastOfWinterStar()
                {
                    if (currentTime >= 900 && currentTime <= 1400)
                    {


                        Game1.player.team.SetLocalReady("festivalStart", true);
                        Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior)(who =>
                        {
                            Game1.exitActiveMenu();
                            Game1.warpFarmer("Town", 1, 20, 1);
                        }), (ConfirmationDialog.behavior)null);

                        

                    }

                }

            }


        }
        




    }
}