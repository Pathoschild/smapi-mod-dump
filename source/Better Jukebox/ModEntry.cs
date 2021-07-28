/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using System;
using Gaphodil.BetterJukebox.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using GenericModConfigMenu;

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
                int MTIndex = Game1.player.songsHeard.IndexOf("MainTheme");
                if (MTIndex.Equals(0) || MTIndex.Equals(-1)) { }
                else
                {
                    Game1.player.songsHeard.RemoveAt(MTIndex);
                    Game1.player.songsHeard.Insert(0, "MainTheme");
                }

                Monitor.Log("permanentBlacklist: " + Config.PermanentBlacklist);
                Monitor.Log("permanentBlacklist converted: " + new FilterListConfig(Config.PermanentBlacklist));
                FilterListConfig blacklist = new FilterListConfig(Config.PermanentBlacklist);
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
                
                e.NewMenu.exitThisMenuNoSound(); // is this neccessary? is there a better way?

                // create default list of songs to play - apparently this is how CA hard-copied the list
                List<string> list = Game1.player.songsHeard.Distinct().ToList();

                // add unheard tracks
                if (Config.ShowUnheardTracks && !Config.PermanentUnheard)
                {
                    BetterJukeboxHelper.AddUnheardTracks(
                        list,
                        Config.UnheardSoundtrack,
                        Config.UnheardNamed,
                        Config.UnheardRandom,
                        Config.UnheardMisc,
                        Config.UnheardDupes,
                        Config.UnheardMusical
                    );
                }

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


                // create and activate the menu
                Game1.activeClickableMenu = new BetterJukeboxMenu(
                    list,
                    new BetterJukeboxMenu.actionOnChoosingListOption(action),
                    Helper.Content.Load<Texture2D>(
                        "assets/BetterJukeboxGraphics.png",
                        ContentSource.ModFolder
                    ),
                    key => Helper.Translation.Get(key),
                    Monitor,
                    Config,
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
            api.RegisterModConfig(
                mod: ModManifest,
                revertToDefault: () => Config = new ModConfig(),
                saveToFile: () => Helper.WriteConfig(Config)
            );

            // let players configure your mod in-game (instead of just from the title screen)
            api.SetDefaultIngameOptinValue(ModManifest, true);

            // add some config options
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowMenu"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowMenuDescription"),
                optionGet: () => Config.ShowMenu,
                optionSet: value => Config.ShowMenu = value
            );
            api.RegisterPageLabel(
                ModManifest,
                Helper.Translation.Get("BetterJukebox:ListSettings"),
                Helper.Translation.Get("BetterJukebox:ListSettingsDescription"),
                Helper.Translation.Get("BetterJukebox:ListSettings")
            );
            api.RegisterPageLabel(
                ModManifest,
                Helper.Translation.Get("BetterJukebox:FunctionalSettings"),
                Helper.Translation.Get("BetterJukebox:FunctionalSettingsDescription"),
                Helper.Translation.Get("BetterJukebox:FunctionalSettings")
            );
            api.RegisterPageLabel(
                ModManifest,
                Helper.Translation.Get("BetterJukebox:VisualSettings"),
                Helper.Translation.Get("BetterJukebox:VisualSettingsDescription"),
                Helper.Translation.Get("BetterJukebox:VisualSettings")
            );

            api.StartNewPage(ModManifest, Helper.Translation.Get("BetterJukebox:ListSettings"));
            api.RegisterClampedOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:AmbientTracks"),
                optionDesc: Helper.Translation.Get("BetterJukebox:AmbientTracksDescription"),
                optionGet: () => Config.AmbientTracks,
                optionSet: value => Config.AmbientTracks = value,
                min: 0,
                max: 2,
                interval: 1
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:Blacklist"),
                optionDesc: Helper.Translation.Get("BetterJukebox:BlacklistDescription"),
                optionGet: () => Config.Blacklist,
                optionSet: value => Config.Blacklist = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:Whitelist"),
                optionDesc: Helper.Translation.Get("BetterJukebox:WhitelistDescription"),
                optionGet: () => Config.Whitelist,
                optionSet: value => Config.Whitelist = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowLockedSongs"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowLockedSongsDescription"),
                optionGet: () => Config.ShowLockedSongs,
                optionSet: value => Config.ShowLockedSongs = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowUnheardTracks"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowUnheardTracksDescription"),
                optionGet: () => Config.ShowUnheardTracks,
                optionSet: value => Config.ShowUnheardTracks = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardSoundtrack"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardSoundtrackDescription"),
                optionGet: () => Config.UnheardSoundtrack,
                optionSet: value => Config.UnheardSoundtrack = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardNamed"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardNamedDescription"),
                optionGet: () => Config.UnheardNamed,
                optionSet: value => Config.UnheardNamed = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardRandom"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardRandomDescription"),
                optionGet: () => Config.UnheardRandom,
                optionSet: value => Config.UnheardRandom = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardMisc"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardMiscDescription"),
                optionGet: () => Config.UnheardMisc,
                optionSet: value => Config.UnheardMisc = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardDupes"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardDupesDescription"),
                optionGet: () => Config.UnheardDupes,
                optionSet: value => Config.UnheardDupes = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:UnheardMusical"),
                optionDesc: Helper.Translation.Get("BetterJukebox:UnheardMusicalDescription"),
                optionGet: () => Config.UnheardMusical,
                optionSet: value => Config.UnheardMusical = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:PermanentUnheard"),
                optionDesc: Helper.Translation.Get("BetterJukebox:PermanentUnheardDescription"),
                optionGet: () => Config.PermanentUnheard,
                optionSet: value => Config.PermanentUnheard = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:PermanentBlacklist"),
                optionDesc: Helper.Translation.Get("BetterJukebox:PermanentBlacklistDescription"),
                optionGet: () => Config.PermanentBlacklist,
                optionSet: value => Config.PermanentBlacklist = value
            );

            api.StartNewPage(ModManifest, Helper.Translation.Get("BetterJukebox:FunctionalSettings"));
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:TrueRandom"),
                optionDesc: Helper.Translation.Get("BetterJukebox:TrueRandomDescription"),
                optionGet: () => Config.TrueRandom,
                optionSet: value => Config.TrueRandom = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowAlternateSorts"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowAlternateSortsDescription"),
                optionGet: () => Config.ShowAlternateSorts,
                optionSet: value => Config.ShowAlternateSorts = value
            );

            api.StartNewPage(ModManifest, Helper.Translation.Get("BetterJukebox:VisualSettings"));
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowInternalId"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowInternalIdDescription"),
                optionGet: () => Config.ShowInternalId,
                optionSet: value => Config.ShowInternalId = value
            );
            api.RegisterSimpleOption(
                mod: ModManifest,
                optionName: Helper.Translation.Get("BetterJukebox:ShowBandcampNames"),
                optionDesc: Helper.Translation.Get("BetterJukebox:ShowBandcampNamesDescription"),
                optionGet: () => Config.ShowBandcampNames,
                optionSet: value => Config.ShowBandcampNames = value
            );
        }

    }
}