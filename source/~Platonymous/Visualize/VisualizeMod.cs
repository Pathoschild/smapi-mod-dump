/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using StardewValley;

namespace Visualize
{
    public class VisualizeMod : Mod
    {
        internal static IMonitor _monitor;
        internal static Profile _activeProfile;
        internal static Config _config;
        internal static IModHelper _helper;
        internal static List<Color> palette = new List<Color>();
        internal static Effect shader = null;
        internal static Dictionary<Texture2D, List<Color>> paletteCache = new Dictionary<Texture2D, List<Color>>();
        internal static List<Profile> profiles = new List<Profile>();
        internal static List<IVisualizeHandler> _handlers = new List<IVisualizeHandler>();
        internal static Effects _handler = new Effects();
        internal static bool active = false;
        internal static int pass = 0;

        public override void Entry(IModHelper helper)
        {
            _monitor = Monitor;
            _config = Helper.ReadConfig<Config>();
            _helper = Helper;
            loadProfiles();
            setActiveProfile(new Profile());
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
            this.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            this.Helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.ConsoleCommands.Add("visrefresh", "Clears the Visualize cache.", (s, p) => emptyCache());
            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;
            harmonyFix();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            emptyCache();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu != null)
            {
                setActiveProfile();
                this.Helper.Events.Display.MenuChanged -= OnMenuChanged;
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(30)) // half-second tick
                pass = 0;
        }

        internal static Texture2D callDrawHandlers(Texture2D texture)
        {
            foreach (IVisualizeHandler handler in _handlers)
                handler.ProcessTexture(texture);

            return texture;
        }

        internal static Color callDrawHandlers(Color color)
        {
            foreach (IVisualizeHandler handler in _handlers)
                handler.ProcessColor(color);

            return color;
        }

        internal static bool callDrawHandlers(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth)
        {
            foreach (IVisualizeHandler handler in _handlers)
                if (!handler.Draw(__instance, texture, destinationRectangle, sourceRectangle, color, origin, rotation, effects, layerDepth))
                    return false;

            return true;
        }

        internal static bool callDrawHandlers(SpriteBatch __instance, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2? origin, float scale, SpriteEffects effects, float layerDepth)
        {
            foreach (IVisualizeHandler handler in _handlers)
                if (!handler.Draw(__instance, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth))
                    return false;

            return true;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == _config.next)
                switchProfile(1);

            if (e.Button == _config.previous)
                switchProfile(-1);

            if (e.Button == _config.reset)
                reset();

            if (e.Button == _config.satHigher || e.Button == _config.satLower)
            {
                if (e.Button == _config.satHigher)
                    _config.saturation = MathHelper.Min(200, _config.saturation + 10);

                if (e.Button == _config.satLower)
                    _config.saturation = MathHelper.Max(0, _config.saturation - 10);

                emptyCache();
                Helper.WriteConfig(_config);
            }
        }

        internal static void emptyCache()
        {
            Effects.textureCache = new Dictionary<Texture2D, Texture2D>();
            Effects.colorCache = new Dictionary<Color, Color>();
            Effects.tintColorCache = new Dictionary<Color, Color>();
            Effects.lastData = null;
            Effects.lastNewData = null;
        }

        public static void addHandler(IVisualizeHandler handler)
        {
            if (!_handlers.Contains(handler))
                _handlers.Add(handler);
        }

        public static int addProfile(Profile profile)
        {
            if (!profiles.Contains(profile))
                profiles.Add(profile);

            return profiles.FindIndex(p => p == profile);
        }

        public static void removeHandler(IVisualizeHandler handler)
        {
            if (_handlers.Contains(handler))
                _handlers.Remove(handler);
        }

        public static void removeProfile(Profile profile)
        {
            if (profiles.Contains(profile))
                profiles.Remove(profile);
        }

        public static Profile getActiveProfile()
        {
            return _activeProfile;
        }

        internal static void loadPalette(Profile profile)
        {
            if (profile.palette != "none")
            {
                Texture2D paletteImage = _helper.Content.Load<Texture2D>($"Profiles/{profile.palette}", ContentSource.ModFolder);

                if (paletteCache.ContainsKey(paletteImage))
                    palette = paletteCache[paletteImage];
                else
                {
                    palette = VisualizeMod._handler.loadPalette(paletteImage);
                    paletteCache.Add(paletteImage, palette);
                }
            }
            else
                palette = new List<Color>();
        }


        internal static void setActiveProfile(Profile profile = null)
        {
            active = false;

            emptyCache();

            if (profile == null)
                profile = profiles.Find(p => p.id == _config.activeProfile);

            if (profile == null && profiles.Count > 0)
                profile = profiles.Exists(p => p.id == "Platonymous.Original") ? profiles.Find(p => p.id == "Platonymous.Original") : profiles[0];

            loadPalette(profile);
            _activeProfile = profile;
            active = true;
        }

        private void harmonyFix()
        {
            var instance = new Harmony("Platonymous.Visualize");
            instance.PatchAll(Assembly.GetExecutingAssembly());
            SpritbatchFixNew.initializePatch(instance);
        }

        private void loadProfiles()
        {
            string[] files = parseDir(Path.Combine(Helper.DirectoryPath, "Profiles"), "*.json");

            foreach (string fullPath in files)
            {
                string filename = Path.GetFileName(fullPath);
                Profile profile = Helper.Data.ReadJsonFile<Profile>(Path.Combine("Profiles", filename));

                if (profile.id == "auto")
                    profile.id = profile.author + "." + profile.name;

                profiles.Add(profile);
                string author = profile.author == "none" ? "" : " by " + profile.author;
                Monitor.Log(profile.name + " " + profile.version + author, LogLevel.Info); 
            }

            Monitor.Log(files.Length + " Profiles found.");
        }

        private string[] parseDir(string path, string extension)
        {
            return Directory.GetFiles(path, extension, SearchOption.TopDirectoryOnly);
        }

        internal static void switchProfile(int i)
        {
            int cIndex = profiles.FindIndex(p => p == _activeProfile);
            int nIndex = cIndex + i;

            if (nIndex < 0)
                nIndex = profiles.Count - 1;

            if (nIndex >= profiles.Count)
                nIndex = 0;
            
            setActiveProfile(profiles[nIndex]);
            saveProfileChoice();
        }

        internal static void saveProfileChoice()
        {
            Game1.playSound("coin");
            string author = _activeProfile.author == "none" ? "" : " by " + _activeProfile.author;
            Game1.hudMessages.Clear();
            Game1.addHUDMessage(new HUDMessage("Profile: " + _activeProfile.name + author, 1));
            _config.activeProfile = _activeProfile.id;
            _helper.WriteConfig<Config>(_config);
        }

        internal static void reset()
        {
            setProfile(new Profile());
            _config.saturation = 100;
            saveProfileChoice();
        }

        public static void setProfile(Profile profile)
        {
            int i = addProfile(profile);
            setActiveProfile(profiles[i]);
        }

        public static void setProfileToVanilla()
        {
            setActiveProfile(profiles.Find(p => p.id == "Platonymous.Original"));
        }

        public static void setProfile()
        {
            setActiveProfile();
        }
    }
}
