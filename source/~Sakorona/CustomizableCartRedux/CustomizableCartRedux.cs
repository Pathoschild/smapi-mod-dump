/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using TwilightShards.Common;
using Harmony;
using System.Reflection;
using CustomizableTravelingCart.Patches;
using System.Collections.Generic;

namespace CustomizableTravelingCart
{
    public class CustomizableCartRedux : Mod
    {
        public static Mod instance;
        public static IMonitor Logger;
        public static CartConfig OurConfig;
        public MersenneTwister Dice;
        internal static Dictionary<StardewValley.Object, int[]> APIItemsToBeAdded;
        private ICustomizableCart API;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Dice = new MersenneTwister();
            OurConfig = helper.ReadConfig<CartConfig>();
            Logger = Monitor;
            APIItemsToBeAdded = new Dictionary<StardewValley.Object, int[]>();

            var harmony = HarmonyInstance.Create("koihimenakamura.customizablecart");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MethodInfo CheckAction = AccessTools.Method(typeof(Forest), "checkAction");
            HarmonyMethod CATranspiler = new HarmonyMethod(AccessTools.Method(typeof(ForestPatches), "CheckActionTranspiler"));
            Monitor.Log($"Patching {CheckAction} with Transpiler: {CATranspiler}", LogLevel.Trace); ;
            harmony.Patch(CheckAction, transpiler: CATranspiler);

            MethodInfo ForestDraw = AccessTools.Method(typeof(Forest), "draw");
            HarmonyMethod DrawTranspiler = new HarmonyMethod(AccessTools.Method(typeof(ForestPatches), "DrawTranspiler"));
            Monitor.Log($"Patching {ForestDraw} with Transpiler: {DrawTranspiler}", LogLevel.Trace); ;
            harmony.Patch(ForestDraw, transpiler: DrawTranspiler);

            MethodInfo GenerateTMS = AccessTools.Method(typeof(Utility), "getTravelingMerchantStock");
            HarmonyMethod LTMPrefix = new HarmonyMethod(AccessTools.Method(typeof(UtilityPatches), "getTravelingMerchantStockPrefix"));
            Monitor.Log($"Patching {GenerateTMS} with Prefix: {LTMPrefix}", LogLevel.Trace); ;
            harmony.Patch(GenerateTMS, prefix: LTMPrefix);
             
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLanuched;
        }

        private void OnGameLanuched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<Integrations.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(ModManifest, () => OurConfig = new CartConfig(), () => Helper.WriteConfig(OurConfig));
                api.RegisterClampedOption(ModManifest, "Monday Apparence", "The chance for the cart to appear on Monday", () => (float)OurConfig.MondayChance, (float val) => OurConfig.MondayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Tuesday Apparence", "The chance for the cart to appear on Tuesday", () => (float)OurConfig.TuesdayChance, (float val) => OurConfig.TuesdayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Wednesday Apparence", "The chance for the cart to appear on Wednesday", () => (float)OurConfig.WednesdayChance, (float val) => OurConfig.WednesdayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Thursday Apparence", "The chance for the cart to appear on Thursday", () => (float)OurConfig.ThursdayChance, (float val) => OurConfig.ThursdayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Friday Apparence", "The chance for the cart to appear on Friday", () => (float)OurConfig.FridayChance, (float val) => OurConfig.FridayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Saturday Apparence", "The chance for the cart to appear on Saturday", () => (float)OurConfig.SaturdayChance, (float val) => OurConfig.SaturdayChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Sunday Apparence", "The chance for the cart to appear on Sunday", () => (float)OurConfig.SundayChance, (float val) => OurConfig.SundayChance = val, 0f, 1f);
                api.RegisterSimpleOption(ModManifest, "Appear Only At Start Of Season", "If selected, the cart only appears at the beginning of the season", () => OurConfig.AppearOnlyAtStartOfSeason, (bool val) => OurConfig.AppearOnlyAtStartOfSeason = val);
                api.RegisterSimpleOption(ModManifest, "Appear Only At End Of Season", "If selected, the cart only appears at the end of the season", () => OurConfig.AppearOnlyAtEndOfSeason, (bool val) => OurConfig.AppearOnlyAtEndOfSeason = val);
                api.RegisterSimpleOption(ModManifest, "Appear Only At Start and End Of Season", "If selected, the cart only appears at the beginning and end of the season", () => OurConfig.AppearOnlyAtStartAndEndOfSeason, (bool val) => OurConfig.AppearOnlyAtStartAndEndOfSeason = val);
                api.RegisterSimpleOption(ModManifest, "Appear Only Every Other Week", "If selected, the cart only appears every other week", () => OurConfig.AppearOnlyEveryOtherWeek, (bool val) => OurConfig.AppearOnlyEveryOtherWeek = val);
                api.RegisterSimpleOption(ModManifest, "Use Vanilla Max", "The game defaults to a max of 790. Turning this off allows PPJA assets to appear in the cart.", () => OurConfig.UseVanillaMax, (bool val) => OurConfig.UseVanillaMax = val);
                api.RegisterClampedOption(ModManifest, "Amount of Items", "The amount of items the cart contains.", () => OurConfig.AmountOfItems, (int val) => OurConfig.AmountOfItems = val, 3, 100);
                api.RegisterSimpleOption(ModManifest, "Use Cheaper Pricing", "Toggling this to true allows for cheaper pricing.", () => OurConfig.UseCheaperPricing, (bool val) => OurConfig.UseCheaperPricing = val);
                api.RegisterClampedOption(ModManifest, "Opening Time", "The time the cart opens. Please select a 10-minute time.", () => OurConfig.OpeningTime, (int val) => OurConfig.OpeningTime = val, 600, 2600);
                api.RegisterClampedOption(ModManifest, "Closing Time", "The time the cart closes for the night. Please select a 10-minute time. You don't have to go home, but you can't stay here.", () => OurConfig.ClosingTime, (int val) => OurConfig.ClosingTime = val, 600, 2600);
            }
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="M:StardewModdingAPI.Mod.Entry(StardewModdingAPI.IModHelper)" />.</summary>
        public override object GetApi()
        {
            return API ?? (API = new CustomizableCartAPI(Helper.Reflection));
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            Random r = new Random();
            double randChance = r.NextDouble();

            if (!(Game1.getLocationFromName("Forest") is Forest f))
                throw new Exception("The Forest is not loaded. Please verify your game is properly installed.");
            
            //get the day
            DayOfWeek day = GetDayOfWeek(SDate.Now());
            double dayChance;
            switch (day)
            {
                case DayOfWeek.Monday:
                    dayChance = OurConfig.MondayChance;
                    break;
                case DayOfWeek.Tuesday:
                    dayChance = OurConfig.TuesdayChance;
                    break;
                case DayOfWeek.Wednesday:
                    dayChance = OurConfig.WednesdayChance;
                    break;
                case DayOfWeek.Thursday:
                    dayChance = OurConfig.ThursdayChance;
                    break;
                case DayOfWeek.Friday:
                    dayChance = OurConfig.FridayChance;
                    break;
                case DayOfWeek.Saturday:
                    dayChance = OurConfig.SaturdayChance;
                    break;
                case DayOfWeek.Sunday:
                    dayChance = OurConfig.SundayChance;
                    break;
                default:
                    dayChance = 0;
                    break;
            }

            /* Start of the Season - Day 1. End of the Season - Day 28. Both is obviously day 1 and 28 
               Every other week is only on days 8-14 and 22-28) */

            bool setCartToOn = false;
            if (OurConfig.AppearOnlyAtEndOfSeason)
            {
                if (Game1.dayOfMonth == 28)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartAndEndOfSeason)
            {
                if (Game1.dayOfMonth == 28 || Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartAndEndOfSeason)
            {
                if (Game1.dayOfMonth == 28 || Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyAtStartOfSeason)
            {
                if (Game1.dayOfMonth == 1)
                    setCartToOn = true;
            }

            else if (OurConfig.AppearOnlyEveryOtherWeek)
            {
                if ((Game1.dayOfMonth >= 8 && Game1.dayOfMonth <= 14) || (Game1.dayOfMonth >= 22 && Game1.dayOfMonth <= 28))
                {
                    if (dayChance > randChance)
                    {
                        setCartToOn = true;
                    }
                }
            }

            else
            {
                if (dayChance > randChance)
                {
                    setCartToOn = true;
                }
            }

            if (setCartToOn)
            {
                f.travelingMerchantDay = true;
                f.travelingMerchantBounds.Add(new Rectangle(1472, 640, 492, 116));
                f.travelingMerchantBounds.Add(new Rectangle(1652, 744, 76, 48));
                f.travelingMerchantBounds.Add(new Rectangle(1812, 744, 104, 48));
                foreach (Rectangle travelingMerchantBound in f.travelingMerchantBounds)
                    Utility.clearObjectsInArea(travelingMerchantBound, f);       

                ((CustomizableCartAPI)API).InvokeCartProcessingComplete();
            }
            else
            {
                //clear other values
                f.travelingMerchantBounds.Clear();
                f.travelingMerchantDay = false;
                
            }
        }

        private DayOfWeek GetDayOfWeek(SDate Target)
        {
            switch (Target.Day % 7)
            {
                case 0:
                    return DayOfWeek.Sunday;
                case 1:
                    return DayOfWeek.Monday;
                case 2:
                    return DayOfWeek.Tuesday;
                case 3:
                    return DayOfWeek.Wednesday;
                case 4:
                    return DayOfWeek.Thursday;
                case 5:
                    return DayOfWeek.Friday;
                case 6:
                    return DayOfWeek.Saturday;
                default:
                    return 0;
            }
        }

        public static bool IsValidHours()
        {
            int TimeOfDay = Game1.timeOfDay;
            if (TimeOfDay >= OurConfig.OpeningTime && TimeOfDay <= OurConfig.ClosingTime)
                return true;

            return false;
        }
    }
}   
