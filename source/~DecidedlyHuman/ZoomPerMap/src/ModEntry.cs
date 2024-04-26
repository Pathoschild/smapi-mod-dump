/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.APIs;
using DecidedlyShared.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ZoomPerMap;

public class ModEntry : Mod
{
    private Logger logger;
    private ModConfig config;

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        this.logger = new Logger(this.Monitor);
        helper.Events.Display.MenuChanged += this.DisplayOnMenuChanged;
        helper.Events.Player.Warped += this.PlayerOnWarped;
        helper.Events.GameLoop.GameLaunched += this.GameLoopOnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.GameLoopOnSaveLoaded;
    }

    private void GameLoopOnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        Game1.options.desiredBaseZoomLevel = this.GetZoomForLocation(Game1.currentLocation);
    }

    private void GameLoopOnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        IGenericModConfigMenuApi? gmcm;

        if (this.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
        {
            gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (gmcm is null)
                return;

            gmcm.Register(
                this.ModManifest,
                () => this.config = new ModConfig(),
                () => this.Helper.WriteConfig(this.config));

            gmcm.AddNumberOption(
                this.ModManifest,
                () => this.config.defaultOutdoorZoomLevel,
                (val) => this.config.defaultOutdoorZoomLevel = val,
                () => I18n.Settings_DefaultOutdoorZoom());

            gmcm.AddNumberOption(
                this.ModManifest,
                () => this.config.defaultIndoorZoomLevel,
                (val) => this.config.defaultIndoorZoomLevel = val,
                () => I18n.Settings_DefaultIndoorZoom());

            gmcm.AddNumberOption(
                this.ModManifest,
                () => this.config.defaultMineZoomLevel,
                (val) => this.config.defaultMineZoomLevel = val,
                () => I18n.Settings_DefaultMineZoom());
        }
    }

    private void PlayerOnWarped(object? sender, WarpedEventArgs e)
    {
        Game1.options.desiredBaseZoomLevel = this.GetZoomForLocation(e.NewLocation);
    }

    private void DisplayOnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is GameMenu)
        {
            bool zoomLevelsDirty = false;

            if (this.config.zoomLevels.TryGetValue(Game1.currentLocation.Name, out float zoomLevel))
            {
                if (zoomLevel != Game1.options.desiredBaseZoomLevel)
                {
                    zoomLevelsDirty = true;
                }
            }
            else
            {
                zoomLevelsDirty = true;
            }

            if (zoomLevelsDirty)
            {
                this.config.zoomLevels[Game1.currentLocation.Name] = Game1.options.zoomLevel;
                this.Helper.WriteConfig(this.config);
            }
        }
    }

    private float GetZoomForLocation(GameLocation location)
    {
        if (this.config.zoomLevels.TryGetValue(location.Name, out float zoomLevel))
        {
            return zoomLevel;
        }

        if (location is MineShaft || location is VolcanoDungeon)
            return this.config.defaultMineZoomLevel;

        return location.IsOutdoors ? this.config.defaultOutdoorZoomLevel : this.config.defaultIndoorZoomLevel;
    }
}
