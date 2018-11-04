using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace FunnySnek.AntiCheat.Client
{
    //todo
    // add warning in smapi if you have wrong mods installed
    // set up wrong mods kick
    // use "Mymod.Myaddress" to make sure peopel stay up to date

    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        private int HalfSecondTicks;
        private int CheatClockTicks;
        private readonly List<string> BlockedModNames = new List<string>();
        private bool IsCheater;
        private bool IsMessageSent;
        private bool IsPreMessageDeleteMessageSent;
        private string CurrentPassword = "SCAZ"; //current code password 4 characters only
        private string IntroMessage = "ServerCode7.3 Activated";

        /// <summary>The mod names to prohibit indexed by mod ID.</summary>
        private readonly IDictionary<string, string> ProhibitedMods = new Dictionary<string, string>
        {
            ["A Tapper's Dream"] = "ddde5195-8f85-4061-90cc-0d4fd5459358",
            ["Adjustable Price Hikes"] = "Pokeytax.AdjustablePriceHikes",
            ["All Crops All Seasons"] = "cantorsdust.AllCropsAllSeasons",
            ["All Professions"] = "cantorsdust.AllProfessions",
            ["Almighty Farming Tool"] = "439",
            ["Auto Animal Doors"] = "AaronTaggart.AutoAnimalDoors",
            ["Auto Grabber"] = "Jotser.AutoGrabberMod",
            ["AutoEat"] = "Permamiss.AutoEat",
            ["AutoFish"] = "WhiteMind.AF",
            ["Automate"] = "Pathoschild.Automate",
            ["AutoSpeed"] = "Omegasis.AutoSpeed",
            ["AutoWater"] = "Whisk.AutoWater",
            ["Backpack Resizer"] = "DefenTheNation.BackpackResizer",
            ["Basic Sprinkler Improved"] = "lrsk_sdvm_bsi.0117171308",
            ["Better Hay"] = "cat.betterhay",
            ["Better Junimos"] = "hawkfalcon.BetterJunimos",
            ["Better Sprinklers"] = "Speeder.BetterSprinklers",
            ["Better Transmutation"] = "f4iTh.BetterTransmutation",
            ["Big Silos"] = "lperkins2.BigSilo",
            ["BJS No Clip"] = "BunnyJumps.BJSNoClip",
            ["BJS Stop Grass"] = "BunnyJumps.BJSStopGrass",
            ["BJS Time Skipper"] = "BunnyJumps.BJSTimeSkipper",
            ["Bregs Fish"] = "Bregodon.BregsFish",
            ["Build Endurance"] = "Omegasis.BuildEndurance",
            ["Build Health"] = "Omegasis.BuildHealth",
            ["Casks Anywhere"] = "CasksAnywhere",
            ["Chefs Closet"] = "Duder.ChefsCloset",
            ["Chests Anywhere"] = "Pathoschild.ChestsAnywhere",
            ["CJB Cheats Menu"] = "CJBok.CheatsMenu",
            ["CJB Item Spawner"] = "CJBok.ItemSpawner",
            ["Configurable Machines"] = "21da6619-dc03-4660-9794-8e5b498f5b97",
            ["Console Commands"] = "SMAPI.ConsoleCommands",
            ["Crop Transplant"] = "DIGUS.CropTransplantMod",
            ["Custom Quest Expiration"] = "hawkfalcon.CustomQuestExpiration",
            ["Custom Warp Locations"] = "cat.customwarplocations",
            ["Customizable Cart Redux"] = "KoihimeNakamura.CCR",
            ["Customizable Death Penalty"] = "cat.customizabledeathpenalty",
            ["Daily Quest Anywhere"] = "Omegasis.DailyQuestAnywhere",
            ["Debug Mode"] = "Pathoschild.DebugMode",
            ["Dynamic Machines"] = "DynamicMachines",
            ["Equivalent Exchange"] = "MercuriusXeno.EquivalentExchange",
            ["Everlasting Baits and Unbreakable Tackles"] = "DIGUS.EverlastingBaitsAndUnbreakableTacklesMod",
            ["Expanded Fridge"] = "Uwazouri.ExpandedFridge",
            ["Extended Reach"] = "spacechase0.ExtendedReach",
            ["Extreme Fishing Overhaul"] = "DevinLematty.ExtremeFishingOverhaul",
            ["EZ Legendary Fish"] = "misscoriel.EZLegendaryFish",
            ["Fast Travel"] = "DeathGameDev.FastTravel",
            ["Faster Horse"] = "kibbe.faster_horse",
            ["Faster Run"] = "KathrynHazuka.FasterRun",
            ["Fishing Adjust"] = "shuaiz.FishingAdjustMod",
            ["Fishing Automaton"] = "Drynwynn.FishingAutomaton",
            ["Foxyfficiency"] = "Fokson.Foxyfficiency",
            ["God Mode"] = "treyh0.GodMode",
            ["Harvest With Scythe by bcmpinc"] = "bcmpinc.HarvestWithScythe",
            ["Harvest With Scythe by ThatNorthernMonkey"] = "965169fd-e1ed-47d0-9f12-b104535fb4bc",
            ["Horse Whistle"] = "icepuente.HorseWhistle",
            ["Idle Timer"] = "LordAndreios.IdleTimer",
            ["Improved Quality of Life"] = "Demiacle.ImprovedQualityOfLife",
            ["Infinite Inventory"] = "DevinLematty.InfiniteInventory",
            ["Infinite Junimo Cart Lives"] = "renny.infinitejunimocartlives",
            ["Instant Buildings"] = "BitwiseJonMods.InstantBuildings",
            ["Instant Grow Trees"] = "cantorsdust.InstantGrowTrees",
            ["Kisekae"] = "Kabigon.kisekae",
            ["Line Sprinklers (Json Assets)"] = "hootless.JASprinklers",
            ["Line Sprinklers (SMAPI)"] = "hootless.LineSprinklers",
            ["Longer Lasting Lures"] = "caraxian.LongerLastingLures",
            ["Longevity"] = "UnlockedRecipes.pseudohub.de",
            ["Luck Skill"] = "spacechase0.LuckSkill",
            ["Magic"] = "spacechase0.Magic",
            ["Mining With Explosives"] = "MiningWithExplosives",
            ["Mood Guard"] = "YonKuma.MoodGuard",
            ["More Artifact Spots"] = "451",
            ["More Map Warps"] = "rc.maps",
            ["More Mine Ladders"] = "JadeTheavas.MoreMineLadders",
            ["More Rain"] = "Omegasis.MoreRain",
            ["More Silo Storage"] = "OrneryWalrus.MoreSiloStorage",
            ["Move Faster"] = "shuaiz.MoveFasterMod",
            ["Move Though Object"] = "ylsama.MoveThoughObject",
            ["Movement Speed"] = "bcmpinc.MovementSpeed",
            ["MultiTool"] = "miome.MultiToolMod",
            ["No Crows"] = "cat.nocrows",
            ["No More Random Mine Flyers"] = "Drynwynn.NoAddedFlyingMineMonsters",
            ["No Soil Decay Redux"] = "Platonymous.NoSoilDecayRedux",
            ["One Click Shed Reloader"] = "BitwiseJonMods.OneClickShedReloader",
            ["Parsnips Absolutely Everywhere"] = "SolomonsWorkshop.ParsnipsAbsolutelyEverywhere",
            ["Part of the Community"] = "SB_PotC",
            ["Passable Objects"] = "punyo.PassableObjects",
            ["Pelican Postal Service"] = "Vylus.PelicanPostalService",
            ["Phone Villagers"] = "DewMods.StardewValleyMods.PhoneVillagers",
            ["Plantable Mushroom Trees"] = "f4iTh.PMT",
            ["Point And Plant"] = "jwdred.PointAndPlant",
            ["Prairie King Made Easy"] = "Mucchan.PrairieKingMadeEasy",
            ["Price Drops"] = "skuldomg.priceDrops",
            ["Quest Delay"] = "BadNetCode.QuestDelay",
            ["Quick Start"] = "WuestMan.QuickStart",
            ["Realistic Fishing"] = "KevinConnors.RealisticFishing",
            ["Recatch Legendary Fish"] = "cantorsdust.RecatchLegendaryFish",
            ["Rename"] = "Remmie.Rename",
            ["Rented Tools"] = "JarvieK.RentedTools",
            ["Replanter"] = "jwdred.Replanter",
            ["Rocs Reseed"] = "Roc.Reseed",
            ["Safe Lightning"] = "cat.safelightning",
            ["Scythe Harvesting"] = "mmanlapat.ScytheHarvesting",
            ["Seasonal Items"] = "midoriarmstrong.seasonalitems",
            ["Seed Catalogue"] = "spacechase0.SeedCatalogue",
            ["Self Service Shops"] = "GuiNoya.SelfServiceShop",
            ["Self Service"] = "JarvieK.SelfService",
            ["Skip Fishing Minigame"] = "DewMods.StardewValleyMods.SkipFishingMinigame",
            ["Skull Cavern Elevator"] = "SkullCavernElevator",
            ["Sprint and Dash Redux"] = "littleraskol.SprintAndDashRedux",
            ["Sprint and Dash"] = "SPDSprintAndDash",
            ["Starting Money"] = "mmanlapat.StartingMoney",
            ["Tehs Fishing Overhaul"] = "TehPers.FishingOverhaul",
            ["Tillable Ground"] = "hawkfalcon.TillableGround",
            ["Time Freeze"] = "Omegasis.TimeFreeze",
            ["Time Multiplier"] = "DefenTheNation.TimeMultiplier",
            ["TimeSpeed"] = "cantorsdust.TimeSpeed",
            ["Tool Power Select"] = "crazywig.toolpowerselect",
            ["Tree Transplant"] = "TreeTransplant",
            ["Ultimate Gold"] = "Shadowfoxss.UltimateGoldStardew",
            ["Unlocked Cooking Recipes"] = "RTGOAT.Longevity",
            ["Warp To Friends"] = "Shalankwa.WarpToFriends",
            ["wHats Up"] = "wHatsUp",
            ["Winter Grass"] = "cat.wintergrass"
        };


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // find prohibited mods
            foreach (var pair in this.ProhibitedMods)
            {
                string modID = pair.Value;
                string name = pair.Key;

                if (helper.ModRegistry.IsLoaded(modID))
                {
                    this.BlockedModNames.Add(name);
                    this.IsCheater = true;
                }
                this.BlockedModNames.Sort();
            }

            // disable interaction if prohibited mod found
            if (this.IsCheater)
            {
                InputEvents.ButtonPressed += this.OnButtonPressed;
            }

            TimeEvents.TimeOfDayChanged += this.OnTimeOfDayChanged;
            GameEvents.HalfSecondTick += this.OnHalfSecondTick;
            GameEvents.FourthUpdateTick += this.OnFourthUpdateTick;
            SaveEvents.BeforeSave += this.OnBeforeSave; // Shipping Menu handler
            GraphicsEvents.OnPreRenderGuiEvent += this.OnPreRenderGui; // for fixing social menu


        }


        /*********
        ** Private methods
        *********/
        /// <summary>An event handler called when the player presses a button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, EventArgs e)
        {
            if (this.IsCheater)
            {
                Game1.activeClickableMenu.exitThisMenu();
                this.Monitor.Log($"Blocked mods detected: {string.Join(", ", this.BlockedModNames)}", LogLevel.Warn);
            }
        }

        /// <summary>An event handler called twice per second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnHalfSecondTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu)
                this.IsMessageSent = false;

            if (!Context.IsWorldReady)
                return;

            this.HalfSecondTicks++;
            if (this.HalfSecondTicks >= 30 && !this.IsMessageSent && !this.IsCheater)
            {
                var playerID = Game1.player.UniqueMultiplayerID;

                Game1.chatBox.addInfoMessage($"{this.IntroMessage}");
                Game1.displayHUD = true;
                Game1.addHUDMessage(new HUDMessage($"{this.IntroMessage}", ""));

                string myStringMessage = $"{this.CurrentPassword}{playerID}";
                Game1.client.sendMessage(18, myStringMessage);

                this.IsMessageSent = true;
            }
        }

        /// <summary>An event handler called fifteen times per second.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnFourthUpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // send anti-cheat code
            if (!this.IsPreMessageDeleteMessageSent)
            {
                var playerID = Game1.player.UniqueMultiplayerID;
                this.SendChatMessage("/color");
                this.SendChatMessage("Sending anti-cheat code.");
                Game1.client.sendMessage(Multiplayer.serverToClientsMessage, $"{this.CurrentPassword}{playerID}");
                this.IsPreMessageDeleteMessageSent = true;
            }
        }

        /// <summary>An event handler called before drawing a menu to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPreRenderGui(object sender, EventArgs e)
        {
            // fix social tab
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.currentTab == GameMenu.socialTab)
            {
                List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
                SocialPage socialPage = (SocialPage)tabs[gameMenu.currentTab];
                IReflectedField<int> numFarmers = this.Helper.Reflection.GetField<int>(socialPage, "numFarmers");
                numFarmers.SetValue(1);
            }
        }

        /// <summary>An event handler called before the game saves.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBeforeSave(object sender, EventArgs e)
        {
            // click "OK" in shipping menu
            if (Game1.activeClickableMenu is ShippingMenu)
                this.Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();
        }

        /// <summary>An event handler called when the in-game clock changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeOfDayChanged(object sender, EventArgs e)
        {
            // exit if cheat mods detected
            if (this.IsCheater)
            {
                this.CheatClockTicks++;
                Game1.chatBox.addInfoMessage("Blocked mods detected!");
                Game1.displayHUD = true;
                Game1.addHUDMessage(new HUDMessage("Blocked mods detected!", ""));
                if (this.CheatClockTicks == 6)
                {
                    Game1.exitToTitle = true;
                    this.CheatClockTicks = 0;
                    return;
                }
            }

            // join today's festival if it started
            if (Game1.otherFarmers.Count > 1)
            {
                var date = SDate.Now();
                switch ($"{date.Season} {date.Day}")
                {
                    // egg festival
                    case "spring 13":
                        this.StartFestival(minTime: 900, maxTime: 1400, location: "Town", x: 1, y: 20);
                        break;

                    // flower dance
                    case "spring 24":
                        this.StartFestival(minTime: 900, maxTime: 1400, location: "Forest", x: 1, y: 20);
                        break;

                    // Luau
                    case "summer 11":
                        this.StartFestival(minTime: 900, maxTime: 1400, location: "Beach", x: 1, y: 20);
                        break;

                    // Dance of the Moonlight Jellies
                    case "summer 28":
                        this.StartFestival(minTime: 2200, maxTime: 2400, location: "Beach", x: 1, y: 20);
                        break;

                    // Stardew Valley Fair
                    case "fall 16":
                        this.StartFestival(minTime: 900, maxTime: 1500, location: "Town", x: 1, y: 20);
                        break;

                    // Spirit's Eve
                    case "fall 27":
                        this.StartFestival(minTime: 2200, maxTime: 2350, location: "Town", x: 1, y: 20);
                        break;

                    // Festival of Ice
                    case "winter 8":
                        this.StartFestival(minTime: 900, maxTime: 1400, location: "Forest", x: 1, y: 20);
                        break;

                    // Feast of the Winter Star
                    case "winter 25":
                        this.StartFestival(minTime: 900, maxTime: 1400, location: "Town", x: 1, y: 20);
                        break;
                }
            }
        }

        /// <summary>Join today's festival if it started.</summary>
        /// <param name="minTime">The in-game time when the festival starts.</param>
        /// <param name="maxTime">The in-game time when the festival ends.</param>
        /// <param name="location">The location name where the festival takes place.</param>
        /// <param name="x">The X position at which to place the player when they warp to the festival location.</param>
        /// <param name="y">The Y position at which to place the player when they warp to the festival location.</param>
        private void StartFestival(int minTime, int maxTime, string location, int x, int y)
        {
            int currentTime = Game1.timeOfDay;
            if (currentTime >= minTime && currentTime <= maxTime)
            {
                Game1.player.team.SetLocalReady("festivalStart", true);
                Game1.activeClickableMenu = new ReadyCheckDialog("festivalStart", true, who =>
                {
                    Game1.exitActiveMenu();
                    Game1.warpFarmer(location, x, y, 1);
                });
            }
        }

        /// <summary>Send a chat message to all players.</summary>
        /// <param name="text">The chat text to send.</param>
        private void SendChatMessage(string text)
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText(text);
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }
    }
}
