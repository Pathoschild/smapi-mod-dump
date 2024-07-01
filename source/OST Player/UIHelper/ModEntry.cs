/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;

namespace OSTPlayer
{
    internal sealed class ModEntry : Mod
    {

        public static ModEntry? context;

        private ModConfig config;

        public static IMobilePhoneApi? api;

        private IModHelper helper;

        public static List<Song> songs = new List<Song>();
        private bool changingSong = false;
        private int songsHeardCount = 0;

        public override void Entry(IModHelper helper)
        {
            context = this;
            this.helper = helper;

            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.WindowResized += OnWindowResized;

            if (!config.ModEnabled)
                return;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            OSTPlayer.PlayingIndexChanged += OnPlayingIndexChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if(!Context.IsWorldReady || changingSong)
                return;
            
            if(Game1.currentSong != null && Game1.currentSong.IsStopped){
                PlayNextTrack();
            }

            if(Game1.player.songsHeard.Count != songsHeardCount){
                songsHeardCount = Game1.player.songsHeard.Count;
                LoadSongs(config.ProgressiveMode);
            }
        }

        private void PlayNextTrack()
        {
            if(OSTPlayer.PlayingIndex != -1){
                int index;
                if(!config.RandomSkip){
                    index = OSTPlayer.PlayingIndex + 1;
                    if(songs.Count <= index)
                        index = 0;
                }else{
                    List<int> indexes = Enumerable.Range(0, songs.Count).Where(i => i != OSTPlayer.PlayingIndex).ToList();
                    Random randomIdx = new Random();
                    index = indexes[randomIdx.Next(indexes.Count)];
                }

                PlaySelectedOST(index);
            }
        }

        private void OnWindowResized(object? sender, WindowResizedEventArgs e)
        {
            if(Game1.activeClickableMenu is OSTPlayer)
                OpenOSTPlayer();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            changingSong = true;
            songsHeardCount = Game1.player.songsHeard.Count;
            changingSong = false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            LoadConfig();
            AddAppToMobile();
        }

        private void OnPlayingIndexChanged(int index){
            
            if(index == -1){
                PlayDefaultOST();
            }else{
                PlaySelectedOST(index);
            }
        }

        private void LoadSongs(bool progressiveMode)
        {
            songs = LogicUtils.GetAllSongs();
            if (progressiveMode)
            {
                HashSet<string> heardSongs = Game1.player.songsHeard;
                songs = songs.Where(s => heardSongs.Contains(s.Id)).ToList();
                songs.Sort();
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (config.ToggleKey.JustPressed())
                OpenOSTPlayer();
            if(config.SkipKey.JustPressed()){
                if(OSTPlayer.PlayingIndex != -1)
                    Game1.playSound("select");
                PlayNextTrack();
            
            }
            if (config.StopKey.JustPressed() && OSTPlayer.PlayingIndex != -1){
                Game1.playSound("drumkit6");
                PlayDefaultOST();
            }
            
        }

        private void LoadConfig()
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            configMenu.Register(
                ModManifest,
                () => config = new ModConfig(),
                () => Helper.WriteConfig(config)
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.ModEnabled,
                value => config.ModEnabled = value,
                () => Helper.Translation.Get("config.enabled"),
                () => Helper.Translation.Get("config.enabled.tooltip"),
                "modEnabled"
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.ToggleKey,
                value => config.ToggleKey = value,
                () => Helper.Translation.Get("config.toggleKey"),
                () => Helper.Translation.Get("config.togglekey.tooltip")
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.ProgressiveMode,
                value => config.ProgressiveMode = value,
                () => Helper.Translation.Get("config.progressivemode"),
                () => Helper.Translation.Get("config.progressivemode.tooltip"),
                "progressiveMode"
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.SkipKey,
                value => config.SkipKey = value,
                () => Helper.Translation.Get("config.skipkey"),
                () => Helper.Translation.Get("config.skipkey.tooltip")
            );

            configMenu.AddBoolOption(
                ModManifest,
                () => config.RandomSkip,
                value => config.RandomSkip = value,
                () => Helper.Translation.Get("config.randomskip"),
                () => Helper.Translation.Get("config.randomskip.tooltip")
            );

            configMenu.AddKeybindList(
                ModManifest,
                () => config.StopKey,
                value => config.StopKey = value,
                () => Helper.Translation.Get("config.stopkey"),
                () => Helper.Translation.Get("config.stopkey.tooltip")
            );

            configMenu.OnFieldChanged(ModManifest, OnOptionChanged);

        }

        private void AddAppToMobile()
        {
            api = helper.ModRegistry.GetApi<IMobilePhoneApi>("JoXW.MobilePhone");

            if (api != null)
            {
                Texture2D icon = helper.ModContent.Load<Texture2D>(Path.Combine("assets", "appIcon.png"));
                bool ok = api.AddApp(helper.ModRegistry.ModID, "OST Player", OpenOSTPlayer, icon);
                Monitor.Log(ok ? "app cargada con exito" : "app no cargada", LogLevel.Debug);
            }
        }

        private void OpenOSTPlayer()
        {
            Game1.playSound("select");
            Game1.activeClickableMenu = new OSTPlayer();

        }

        private void PlayDefaultOST()
        {
            changingSong = true;
            Game1.stopMusicTrack(MusicContext.MusicPlayer);
            if(OSTPlayer.PlayingIndex != -1){
                Game1.addHUDMessage(new OSTHUDMessage($"{Helper.Translation.Get("ost.stopped")}.", false));
                songs.ElementAt(OSTPlayer.PlayingIndex).isPlaying = false;
                OSTPlayer.PlayingIndex = -1;
            }
            changingSong = false;
        }
        private void PlaySelectedOST(int index){

            string songId = songs.ElementAt(index).Id;
            string songName = songs.ElementAt(index).Name;
            bool songHeard = Game1.player.songsHeard.Contains(songId);

            changingSong = true;
            Game1.changeMusicTrack(songId, false, MusicContext.MusicPlayer);

            songs.ElementAt(OSTPlayer.PlayingIndex).isPlaying = false;
            songs.ElementAt(index).isPlaying = true;
            OSTPlayer.PlayingIndex = index;

            if(!config.ProgressiveMode && !songHeard){
                LogicUtils.removeHeardSong(songId);
            }
            Game1.addHUDMessage(new OSTHUDMessage($"{Helper.Translation.Get("ost.playing")} {songName}.", true));
            changingSong = false;
        }

        private void OnOptionChanged(string fieldId, object newValue)
        {
            if (fieldId == "modEnabled")
            {
                PlayDefaultOST();

                bool on = (bool)newValue;

                if (on)
                {
                    helper.Events.Input.ButtonPressed += OnButtonPressed;
                    OSTPlayer.PlayingIndexChanged += OnPlayingIndexChanged;
                    helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
                }
                else
                {
                    helper.Events.Input.ButtonPressed -= OnButtonPressed;
                    OSTPlayer.PlayingIndexChanged -= OnPlayingIndexChanged;
                    helper.Events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTicked;
                }
            }
            else if (fieldId == "progressiveMode")
            {
                PlayDefaultOST();
                LoadSongs((bool)newValue);
            }
        }


    }
}
