/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/holy-the-sea/LightSwitch
**
*************************************************/

using GenericModConfigMenu;
using ToolbarIcons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LightSwitch
{
    /// <summary>The mod entry point</summary>
    public class ModEntry : Mod
    {
        /*********
         ** Fields
         *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config; // set in ModEntry

        /// <summary>Cache values</summary>
        private bool lastAmbientFog;
        private float lastFogAlpha;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            Config = helper.ReadConfig<ModConfig>();

            // hook events
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        /*********
        ** Private methods
        *********/
        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Mods/LightSwitch/ToolButton"))
            {
                e.LoadFromModFile<Texture2D>("assets/icon.png", AssetLoadPriority.Low);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // add GMCM
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Light",
                getValue: () => Config.EnableLight,
                setValue: value => Config.EnableLight = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Fog Removal",
                getValue: () => Config.ToggleMineFog,
                setValue: value => Config.ToggleMineFog = value
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Keybind",
                getValue: () => Config.Keybind,
                setValue: value => Config.Keybind = value
            );

            // add Toolbar Icons
            var toolbarIconsMenu = Helper.ModRegistry.GetApi<IToolbarIconsApi>("furyx639.ToolbarIcons");
            if (toolbarIconsMenu is null)
                return;

            toolbarIconsMenu.AddToolbarIcon(
                "holythesea.LightSwitch.Icon", "Mods/LightSwitch/ToolButton", new Rectangle(0, 0, 16, 16), "Toggle Light Switch"
                );
            toolbarIconsMenu.ToolbarIconPressed += (o, s) =>
            {
                if (s.Equals("holythesea.LightSwitch.Icon")) ToggleLight();
            };
        }

        /// <summary>Just make all the light daytime</summary>
        private void ToggleLight()
        {
            if (!Config.EnableLight) // switch on
            {
                // cache old values

                if (Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    IReflectedProperty<bool> ambientFog = Helper.Reflection.GetProperty<bool>(Game1.currentLocation, "ambientFog");
                    IReflectedField<float> fogAlpha = Helper.Reflection.GetField<float>(Game1.currentLocation, "fogAlpha");

                    lastAmbientFog = ambientFog.GetValue();
                    lastFogAlpha = fogAlpha.GetValue();
                }

                Config.EnableLight = true;
            }

            else // switch off
            {
                Config.EnableLight = false;

                // return values to vanilla game settings
                Helper.Reflection.GetMethod(Game1.currentLocation, "_updateAmbientLighting").Invoke();
                if (Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                {
                    IReflectedProperty<bool> ambientFog = Helper.Reflection.GetProperty<bool>(Game1.currentLocation, "ambientFog");
                    IReflectedField<float> fogAlpha = Helper.Reflection.GetField<float>(Game1.currentLocation, "fogAlpha");

                    ambientFog.SetValue(lastAmbientFog);
                    fogAlpha.SetValue(lastFogAlpha);
                }
            }
        }

        /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (e.Button == Config.Keybind)
            {
                ToggleLight();
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;

            IReflectedField<Color> ambientLight = Helper.Reflection.GetField<Color>(typeof(Game1), "ambientLight");

            if (Config.EnableLight)
            {

                // makes outdoors/indoors bright
                ambientLight.SetValue(Color.Transparent);

                // handle mine levels
                // removes darkening of the mine floors (and probably other things idk)
                Game1.drawLighting = false;
                // remove fog
                if (Config.ToggleMineFog)
                {
                    if (Game1.currentLocation.Name.StartsWith("UndergroundMine"))
                    {
                        IReflectedProperty<bool> ambientFog = Helper.Reflection.GetProperty<bool>(Game1.currentLocation, "ambientFog");
                        IReflectedField<float> fogAlpha = Helper.Reflection.GetField<float>(Game1.currentLocation, "fogAlpha");

                        // yeetus-deleetus the fog layer
                        ambientFog.SetValue(false);
                        fogAlpha.SetValue(0f);
                    }
                }
            }
        }
    }

    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        public bool EnableLight { get; set; } = true;
        public bool ToggleMineFog { get; set; } = true;
        public SButton Keybind { get; set; } = SButton.K;
    }
}
