/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using System;
using System.Reflection;
using Harmony;
using JojaOnline.JojaOnline;
using JojaOnline.JojaOnline.Mailing;
using JojaOnline.JojaOnline.Mobile;
using JojaOnline.JojaOnline.Items;
using JojaOnline.JojaOnline.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace JojaOnline
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            // PyTK (required for Custom Furniture) has compatibility issue with SpaceCore, must be v1.4.1 and below until SpaceCore or PyTK make the required changes
            if (Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore") && Helper.ModRegistry.IsLoaded("Platonymous.CustomFurniture"))
            {
                if (Helper.ModRegistry.Get("spacechase0.SpaceCore").Manifest.Version.IsNewerThan("1.4.1") && !Helper.ModRegistry.Get("Platonymous.CustomFurniture").Manifest.Version.IsNewerThan("0.11.2"))
                {
                    throw new InvalidOperationException("JojaOnline is only compatible with SpaceCore v1.4.1 and below when used with Custom Furniture v0.11.2 due to a compatibility issue with PyTK. " +
                        "SpaceCore v1.4.1 works with Stardew Valley v1.5, so please use that if you wish to use this mod. " +
                        "You can also optionally not use Custom Furniture and instead use the Mobile Phone mod.");
                }
            }

            // Load our Harmony patches
            try
            {
                harmonyPatch();
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
            }

            // Load the config
            this.config = helper.ReadConfig<ModConfig>();

            // Load the monitor
            JojaResources.LoadMonitor(this.Monitor);

            // Get the image resources needed for the mod
            JojaResources.LoadTextures(helper);

            // Hook into the game launch
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Hook into the game's daily events
            helper.Events.GameLoop.DayStarted += this.OnDayStarting;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
        }

        public void harmonyPatch()
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            JojaMail.ProcessPlayerMailbox();
            this.Monitor.Log($"Processed player's mailbox to check for any scheduled JojaMail orders.");
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            JojaMail.ProcessPlayerMailbox();
            this.Monitor.Log($"Processed player's mailbox to check for any scheduled JojaMail orders.");
        }

        private void OnDayStarting(object sender, DayStartedEventArgs e)
        {
            // Check if the player should get the Joja Membership discount via config file
            JojaSite.SetMembershipStatus(this.config.giveJojaMemberDiscount);

            // Check if player should get the Joja Prime free shipping via config
            JojaSite.SetPrimeShippingStatus(this.config.giveJojaPrimeShipping);

            // Modify JojaStock to include all year seed stock (if past year 1) & other items
            JojaResources.SetJojaOnlineStock(this.config.itemNameToPriceOverrides, this.config.areAllSeedsAvailableBeforeYearOne, this.config.doCopyPiereeSeedStock);

            JojaSite.PickRandomItemForDiscount(this.config.minSalePercentage, this.config.maxSalePercentage);
            this.Monitor.Log($"Picked a random item for discount at JojaOnline store.", LogLevel.Debug);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Check if aedenthorn's Mobile Phone is in the current mod list
            if (Helper.ModRegistry.IsLoaded("aedenthorn.MobilePhone"))
            {
                // Attempt to load the JojaOnline app into the Mobile Phone
                Monitor.Log("Attempting to hook into aedenthorn.MobilePhone.", LogLevel.Debug);
                JojaMobile.LoadApp(Helper);
            }

            // Check if spacechase0's JsonAssets is in the current mod list
            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                // Attempt to load the JojaOnline app into the Mobile Phone
                Monitor.Log("Attempting to hook into spacechase0.JsonAssets.", LogLevel.Debug);
                JojaItems.HookIntoApi(Helper);
            }
        }
    }
}
