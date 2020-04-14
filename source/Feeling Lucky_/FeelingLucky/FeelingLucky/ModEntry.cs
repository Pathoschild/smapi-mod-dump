using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using static FeelingLucky.IGenericModConfigMenuAPI;

namespace FeelingLucky
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 
        /*********
        ** Fields
        *********/

        //setting up for a config, the SMAPI helper, and other uses later
        internal static ModConfig Config;
        //internal static IManifest Mod;

        private int leprechaunShoes = 14;
        private bool isWearingLeprechaunShoes = false;
        //private int purpleShorts = 15;
        //private bool isWearingPurpleShorts = false;
        //private readonly int luckyBow = 21;
        //private readonly bool isWearingLuckyBow = false;

            //TODO: Why the hell aren't the pants working

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += onUpdateTicked;
            Helper.Events.GameLoop.GameLaunched += onGameLaunched;

            Config = Helper.ReadConfig<ModConfig>();

        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var GMCMApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (GMCMApi != null)
            {
                GMCMApi.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
                GMCMApi.RegisterSimpleOption(ModManifest, "Verbose Logging", "Enable verbose logging for troubleshooting", () => Config.VerboseLogging, (bool val) => Config.VerboseLogging = val);

            }
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            
            else if (e.IsOneSecond)
            {
                this.equippedBoots();
                //this.equippedPants();
                //this.equippedHat();

                if (isWearingLeprechaunShoes == true)
                {
                    this.giveBuff();
                    if (ModEntry.Config.VerboseLogging)
                    {
                        this.Monitor.Log($"Buff applied.", LogLevel.Debug);
                    }
                }
                else
                {
                    this.buffOff();
                    if (ModEntry.Config.VerboseLogging)
                    {
                        this.doNothing();
                    }
                       
                }
            }
            
        }

        private void giveBuff()
        {

            Buff luckBuff = Game1.buffsDisplay.otherBuffs.Find(b => b.source == "Lucky");

            if (luckBuff == null)
            {

                luckBuff = new Buff(0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 10000, "Lucky", "Lucky");
                luckBuff.millisecondsDuration = 10000;
                Game1.buffsDisplay.addOtherBuff(luckBuff);
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Adding luck buff.", LogLevel.Debug);
                }
            }

            else if (luckBuff != null)
            {
                luckBuff.millisecondsDuration = 10000;
            }
            
        }
        private void buffOff()
        {

            Buff luckBuff = Game1.buffsDisplay.otherBuffs.Find(b => b.source == "Lucky");

            if (luckBuff != null)
            {

                luckBuff.removeBuff();
                Game1.buffsDisplay.otherBuffs.Remove(luckBuff);
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Removing luck buff.", LogLevel.Debug);
                }
            }

            else if (luckBuff == null)
            {
                return;
            }

        }

        private void equippedBoots()
        {
            var currentBoots = Game1.player.shoes.Value;
            if (ModEntry.Config.VerboseLogging)
            {
                Monitor.Log($"Current boots value is {currentBoots}.", LogLevel.Debug);
            }

            if (currentBoots != leprechaunShoes)
            {
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Boots are not Lucky Shoes.", LogLevel.Debug);
                }
                isWearingLeprechaunShoes = false;
                return;
            }
            else
            {
                isWearingLeprechaunShoes = true;
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Player is wearing correct boots? {isWearingLeprechaunShoes}.", LogLevel.Debug);
                }
            }
        }

        private void doNothing()
        {
            if (ModEntry.Config.VerboseLogging)
            {
                this.Monitor.Log($"We're doing nothing", LogLevel.Debug);
            }
        }

        /*private void equippedPants()
        {
            var currentPants = Game1.player.pants.Value;
            if (ModEntry.Config.VerboseLogging)
            {
                this.Monitor.Log($"Current pants value is {currentPants}.", LogLevel.Debug);
            }

        if (currentPants != purpleShorts)
            {
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Pants are not Purple Shorts.", LogLevel.Debug);
                }
                isWearingPurpleShorts = false;
                return;
            }
            else
            {
                isWearingPurpleShorts = true;
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Player is wearing correct pants? {isWearingPurpleShorts}.", LogLevel.Debug);
                }
            }
        }

        private void equippedHat()
        {
            var currentHatWearing = Game1.player.hat.ToString();
            if (ModEntry.Config.VerboseLogging)
            {
                this.Monitor.Log($"Current hat value is {currentHatWearing}.", LogLevel.Debug);
            }*/

            /*if (currentHatWearing != luckyBow)
            {
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Hat is not Lucky Bow.", LogLevel.Debug);
                }
                isWearingLuckyBow = false;
                return;
            }
            else
            {
                isWearingLuckyBow = true;
                if (ModEntry.Config.VerboseLogging)
                {
                    this.Monitor.Log($"Player is wearing correct hat? {isWearingLuckyBow}.", LogLevel.Debug);
                }
            }
        }*/

    }
}