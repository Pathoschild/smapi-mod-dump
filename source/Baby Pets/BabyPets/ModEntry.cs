/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DelphinWave/BabyPets
**
*************************************************/

using System;
using BabyPets.Framework;
using BabyPets.GenericModConfig;
using ContentPatcher;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;

namespace BabyPets
{
    internal class ModEntry : Mod
    {
        internal static IMonitor SMonitor;
        internal static IManifest SModManifest;

        public ModConfig Config;

        // ModData Constant
        public static string MOD_DATA_BDAY; // store day pet was added (born)

        // TODO: move to a token manager
        readonly List<string> CAT_TOKENS = new List<string>()
        {
            "IsBabyCatZero",
            "IsBabyCatOne",
            "IsBabyCatTwo",
            "IsBabyCatThree",
            "IsBabyCatFour"
        };

        List<TokenPetData> tokenPetData = new List<TokenPetData>();

        // Pets
        List<Pet> pets = new List<Pet>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SMonitor = Monitor;
            SModManifest = ModManifest;

            MOD_DATA_BDAY = $"{SModManifest.UniqueID}/bday";

            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;


        }


        /*********
        ** Private methods
        *********/

        /// <summary>Raised after the game is launched, right before the first update tick. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            LinkTokensToVanillaCatData();

            // Register CP tokens
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            foreach (var tokenData in tokenPetData)
            {
                api.RegisterToken(this.ModManifest, tokenData.token, () =>
                {
                    return new[] { tokenData.isBaby };
                });
            }

            // Generic Mod Config
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null)
            {
                // register mod
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                );

                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => "Pet Adult Age (Days)",
                    tooltip: () => "How many days old should your pets be when they stop being babies?",
                    getValue: () => Config.AdultPetAge,
                    setValue: value => Config.AdultPetAge = value
                    );
            }
        }


        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {

            // Get all pets & set bday
            var date = SDate.Now();
            pets = PetManager.InitializeBdayModData(date.DaysSinceStart);

            // TODO: refactor to LINQ?

            // Find pets with valid token types and set token isBaby
            foreach (var petData in tokenPetData)
            {
                foreach (var pet in pets)
                {
                    if (pet.petType.Value == petData.petType && pet.whichBreed.Value == petData.petBreed)
                    {
                        petData.SetBday(pet.modData[MOD_DATA_BDAY]);
                    }
                }
            }
        }

        // TODO: move to a token manager
        private void LinkTokensToVanillaCatData()
        {
            var adultAge = Config.AdultPetAge;
            for (int i = 0; i < CAT_TOKENS.Count; i++)
            {
                tokenPetData.Add(new TokenPetData(CAT_TOKENS[i], "Cat", $"{i}", adultAge));
            }
        }

    }
}
