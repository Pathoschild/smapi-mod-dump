/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using Gaphodil.BetterJukebox.Framework;
using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Gaphodil.BetterJukebox
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*
         * Properties
         */

        /// <summary>The mod configuration from the player.</summary>
        public ModConfig Config;

        /*
         * Public methods
         */

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /*
         * Private methods
         */

        /// <summary>
        /// Raised after loading a save.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Monitor.Log("all heard:\t" + string.Join(", ",Game1.player.songsHeard.ToArray()));
            if (Config.PermanentUnheard)
            {
                // there was "don't repeat if already done"
                // but it's gone now (because i added permanent blacklist)

                if (Config.ShowUnheardTracks)
                {
                    BetterJukeboxHelper.AddUnheardTracks(
                        Game1.player.songsHeard,
                        Config.UnheardSoundtrack,
                        Config.UnheardNamed,
                        Config.UnheardRandom,
                        Config.UnheardMisc,
                        Config.UnheardDupes,
                        Config.UnheardMusical
                    );
                }

                //Game1.player.songsHeard.Remove("title_day"); // readded at every save load, so...
                // 1.6: list to hashset means changing maintheme location unnecessary

                Monitor.Log("permanentBlacklist: " + Config.PermanentBlacklist);
                Monitor.Log("permanentBlacklist converted: " + new FilterListConfig(Config.PermanentBlacklist));
                FilterListConfig blacklist = new(Config.PermanentBlacklist);
                List<string> toRemove = blacklist.content.Distinct().ToList();
                if (toRemove.Count > 0)
                {
                    foreach (string cue in toRemove)
                    {
                        Game1.player.songsHeard.Remove(cue);
                    }
                }
            }
        }

        /// <summary>
        /// Raised after a game menu is opened, closed, or replaced. 
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // replace ChooseFromListMenu (only used for jukeboxes as of 1.4) with BetterJukeboxMenu
            if (Config.ShowMenu &&
                e.NewMenu is ChooseFromListMenu &&
                Helper.Reflection.GetField<bool>(e.NewMenu, "isJukebox").GetValue() == true)
            {
                ChooseFromListMenu.actionOnChoosingListOption action = 
                    Helper.Reflection.GetField<ChooseFromListMenu.actionOnChoosingListOption>(e.NewMenu, "chooseAction").GetValue();

                // easy 1.6 saloon bool
                bool isSaloon = !Helper.Reflection.GetField<List<string>>(e.NewMenu, "options").GetValue().Contains("random");
                
                e.NewMenu.exitThisMenuNoSound(); // is this neccessary? is there a better way?

                // create default list of songs to play
                // 1.6: list to hashset - shallow should be fine here?

                // also 1.6 but a later alpha: jukeboxes now use Utility.GetJukeboxTracks
                // which is BACK TO BEING A LIST AUGH
                HashSet<string> heardCopy = [.. Utility.GetJukeboxTracks(Game1.player, Game1.player.currentLocation)];

                // add unheard tracks
                if (Config.ShowUnheardTracks && !Config.PermanentUnheard)
                {
                    BetterJukeboxHelper.AddUnheardTracks(
                        heardCopy,
                        Config.UnheardSoundtrack,
                        Config.UnheardNamed,
                        Config.UnheardRandom,
                        Config.UnheardMisc,
                        Config.UnheardDupes,
                        Config.UnheardMusical
                    );
                }

                // convert to list here instead
                List<string> list = [.. heardCopy];

                // remove specific tracks
                BetterJukeboxHelper.FilterTracksFromList(list, Config.AmbientTracks, Config.Blacklist, Config.Whitelist);

                list.Remove("title_day"); // this one gets removed for A Good Reason, apparently

                // this is the one change that isn't true to how the game does it, because it makes me angy >:L
                int MTIndex = list.IndexOf("MainTheme");
                // if permanent: not -1
                // if not permanent: -1
                // if in either blacklist: -1
                // impossible to tell directly, can't put into filtertracks because need to check both
                bool isMTPermanentBlacklisted = new FilterListConfig(Config.PermanentBlacklist).content.Contains("MainTheme");
                bool isMTRegularBlacklisted = new FilterListConfig(Config.Blacklist).content.Contains("MainTheme");

                if (!isMTPermanentBlacklisted &&
                    !isMTRegularBlacklisted &&
                    !MTIndex.Equals(0))
                {
                    if (MTIndex.Equals(-1)) { }
                    else
                        list.RemoveAt(MTIndex);
                    list.Insert(0, "MainTheme"); 
                }

                // speculative fix for Nexus page bug report
                list.Remove("resetVariable");

                // 1.5.5 asset loading consistency (also it may not have worked off windows anyways)
                string graphicsKey = PathUtilities.NormalizeAssetName("assets/BetterJukeboxGraphics.png");

                // create and activate the menu
                Game1.activeClickableMenu = new BetterJukeboxMenu(
                    list,
                    new BetterJukeboxMenu.actionOnChoosingListOption(action),
                    Helper.ModContent.Load<Texture2D>( // 1.6 compatibility
                        graphicsKey
                    ),
                    key => Helper.Translation.Get(key),
                    Monitor,
                    Config,
                    isSaloon,
                    Game1.player.currentLocation.miniJukeboxTrack.Value
                ); 
            }
        }

        // from https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu#readme
        private void OnGameLaunched(object Sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu API (if it's installed)
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            // register mod configuration
            api.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // add some config options
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowMenu"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowMenuDescription"),
                getValue: () => Config.ShowMenu,
                setValue: value => Config.ShowMenu = value
            );
            api.AddPageLink(
                mod: ModManifest,
                pageId: Helper.Translation.Get("BetterJukebox:ListSettings"),
                text: () => Helper.Translation.Get("BetterJukebox:ListSettings"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ListSettingsDescription")
            );
            api.AddPageLink(
                mod: ModManifest,
                pageId: Helper.Translation.Get("BetterJukebox:FunctionalSettings"),
                text: () => Helper.Translation.Get("BetterJukebox:FunctionalSettings"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:FunctionalSettingsDescription")
            );
            api.AddPageLink(
                mod: ModManifest,
                pageId: Helper.Translation.Get("BetterJukebox:VisualSettings"),
                text: () => Helper.Translation.Get("BetterJukebox:VisualSettings"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:VisualSettingsDescription")
            );


            api.AddPage(
                mod: ModManifest,
                pageId: Helper.Translation.Get("BetterJukebox:ListSettings"),
                pageTitle: () => Helper.Translation.Get("BetterJukebox:ListSettings")
            );
            api.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:AmbientTracks"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:AmbientTracksDescription"),
                getValue: () => Config.AmbientTracks,
                setValue: value => Config.AmbientTracks = (int)value,
                min: 0,
                max: 2,
                interval: 1
            );
            api.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:Blacklist"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:BlacklistDescription"),
                getValue: () => Config.Blacklist,
                setValue: value => Config.Blacklist = value
            );
            api.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:Whitelist"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:WhitelistDescription"),
                getValue: () => Config.Whitelist,
                setValue: value => Config.Whitelist = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowLockedSongs"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowLockedSongsDescription"),
                getValue: () => Config.ShowLockedSongs,
                setValue: value => Config.ShowLockedSongs = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowUnheardTracks"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowUnheardTracksDescription"),
                getValue: () => Config.ShowUnheardTracks,
                setValue: value => Config.ShowUnheardTracks = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardSoundtrack"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardSoundtrackDescription"),
                getValue: () => Config.UnheardSoundtrack,
                setValue: value => Config.UnheardSoundtrack = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardNamed"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardNamedDescription"),
                getValue: () => Config.UnheardNamed,
                setValue: value => Config.UnheardNamed = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardRandom"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardRandomDescription"),
                getValue: () => Config.UnheardRandom,
                setValue: value => Config.UnheardRandom = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardMisc"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardMiscDescription"),
                getValue: () => Config.UnheardMisc,
                setValue: value => Config.UnheardMisc = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardDupes"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardDupesDescription"),
                getValue: () => Config.UnheardDupes,
                setValue: value => Config.UnheardDupes = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:UnheardMusical"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:UnheardMusicalDescription"),
                getValue: () => Config.UnheardMusical,
                setValue: value => Config.UnheardMusical = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:PermanentUnheard"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:PermanentUnheardDescription"),
                getValue: () => Config.PermanentUnheard,
                setValue: value => Config.PermanentUnheard = value
            );
            api.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:PermanentBlacklist"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:PermanentBlacklistDescription"),
                getValue: () => Config.PermanentBlacklist,
                setValue: value => Config.PermanentBlacklist = value
            );


            api.AddPage(
                mod: ModManifest,
                pageId: Helper.Translation.Get("BetterJukebox:FunctionalSettings"),
                pageTitle: () => Helper.Translation.Get("BetterJukebox:FunctionalSettings")
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:TrueRandom"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:TrueRandomDescription"),
                getValue: () => Config.TrueRandom,
                setValue: value => Config.TrueRandom = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowAlternateSorts"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowAlternateSortsDescription"),
                getValue: () => Config.ShowAlternateSorts,
                setValue: value => Config.ShowAlternateSorts = value
            );


            api.AddPage(
                mod: ModManifest, 
                pageId: Helper.Translation.Get("BetterJukebox:VisualSettings"),
                pageTitle: () => Helper.Translation.Get("BetterJukebox:VisualSettings")
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowInternalId"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowInternalIdDescription"),
                getValue: () => Config.ShowInternalId,
                setValue: value => Config.ShowInternalId = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("BetterJukebox:ShowBandcampNames"),
                tooltip: () => Helper.Translation.Get("BetterJukebox:ShowBandcampNamesDescription"),
                getValue: () => Config.ShowBandcampNames,
                setValue: value => Config.ShowBandcampNames = value
            );
        }

    }
}