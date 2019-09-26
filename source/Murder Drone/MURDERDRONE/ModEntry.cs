using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace MURDERDRONE
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.Saving += GameLoop_SaveCreated;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Player.Warped += PlayerEvents_Warped;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Sidekick/Drone");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Sidekick/Drone"))
                return Helper.Content.Load<T>("Assets/drone_sprite_robot.png", ContentSource.ModFolder);

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        /*********
        ** Private methods
        *********/
        private void GameLoop_SaveCreated(object sender, SavingEventArgs e)
        {
            RemoveDrone();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || Game1.currentMinigame != null)
                return;

            if (e.Button == (SButton)Enum.Parse(typeof(SButton), Config.KeyboardShortcut, true))
            {
                if (Config.Active)
                {
                    RemoveDrone();
                    Config.Active = false;
                    Game1.showRedMessage("Drone deactivated.");
                }
                else
                {
                    AddDrone();
                    Config.Active = true;
                    Game1.addHUDMessage(new HUDMessage("Drone activated.", 4));
                }

                Helper.WriteConfig(Config);
            }
        }

        /// <summary>
        /// The method called when the player warps.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerEvents_Warped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer || Game1.CurrentEvent != null || !Config.Active)
                return;

            AddDrone();
        }

        private void RemoveDrone()
        {
            if (Game1.getCharacterFromName("Drone") is NPC drone)
            {
                Game1.removeThisCharacterFromAllLocations(drone);
            }
        }

        private void AddDrone()
        {
            if (Game1.currentLocation is DecoratableLocation)
                return;

            if (Game1.getCharacterFromName("Drone") is NPC == false)
                Game1.currentLocation.addCharacter(new Drone(Config.RotationSpeed, Config.Damage, (float)Config.ProjectileVelocity, Helper));
            else
                Game1.warpCharacter(Game1.getCharacterFromName("Drone"), Game1.currentLocation, Game1.player.Position);
        }
    }
} 