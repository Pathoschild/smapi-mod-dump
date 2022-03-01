/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Ogg2XNA;
using HarmonyLib;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StardewValley;
using System;
using System.Reflection;
using OggSharp;
using StardewModdingAPI.Events;
using System.Threading;
using Microsoft.Xna.Framework;
using xTile;
using xTile.ObjectModel;

namespace CustomMusic
{
    public class CustomMusicMod : Mod
    {
        internal static HashSet<StoredMusic> Music = new HashSet<StoredMusic>();
        internal static HashSet<ActiveMusic> Active = new HashSet<ActiveMusic>();
        internal static IMonitor SMonitor;
        internal static IModHelper SHelper;
        internal static MethodInfo checkEventConditions = null;
        internal static Dictionary<string, string> locations = new Dictionary<string, string>();
        internal static Dictionary<string, List<SoundEmitterPlacement>> emitterPlacements = new Dictionary<string, List<SoundEmitterPlacement>>();
        public static Config config;
        internal static int loads = 0;
        internal static int simLoad = 0;
        internal static List<string> working = new List<string>();
        internal const int ti = 32;
        internal const int simuMax = 8;
        internal const int slp = 500;
        internal List<ActiveMusic> CurrentEmitters = new List<ActiveMusic>();
        public static SynthesizedSound Synth = new SynthesizedSound(form: WaveForm.Square, length: 15);
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            SMonitor = Monitor;
            SHelper = Helper;
            Thread thread = new Thread(loadContentPacks);
            thread.Start();
            var harmony = new Harmony("Platonymous.CustomMusic");
            harmony.Patch(typeof(Cue).GetMethod("Stop"), new HarmonyMethod(typeof(Overrides), "Stop"));
            harmony.Patch(typeof(Cue).GetMethod("Play"), new HarmonyMethod(typeof(Overrides), "Play"));
            harmony.Patch(typeof(Cue).GetMethod("Dispose"), new HarmonyMethod(typeof(Overrides), "Dispose"));
            harmony.Patch(typeof(Cue).GetMethod("SetVariable"), new HarmonyMethod(typeof(Overrides), "SetVariable"));
            harmony.Patch(typeof(Cue).GetProperty("IsPlaying").GetGetMethod(false), new HarmonyMethod(typeof(Overrides), "IsPlaying"));
            harmony.Patch(typeof(SoundBank).GetMethod("GetCue"), new HarmonyMethod(typeof(Overrides), "GetCue"));

            foreach(var playCue in typeof(SoundBank).GetMethods().Where(m => m.Name == "PlayCue"))
            harmony.Patch(playCue, new HarmonyMethod(typeof(Overrides), "PlayCue"));

            try
            {
                var PyUtils = Type.GetType("PyTK.PyUtils, PyTK");
                if (PyUtils != null)
                    checkEventConditions = PyUtils.GetMethod("CheckEventConditions");
            }
            catch
            {

            }

            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.ConsoleCommands.Add("playmusic", "Play a music cue. Format: playmusic [name]", (s, p) =>
              {
                  Monitor.Log("Playing: " + p[0], LogLevel.Info);
                  Game1.changeMusicTrack(p[0]);
              });

            //helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.Y)
                Synth.Play();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.currentLocation == null || !locations.ContainsKey(Game1.currentLocation.Name))
                return;

            var name = locations[Game1.currentLocation.Name];

            if (name.ToLower().StartsWith("cm:"))
            {
                string[] n = name.Split(':');
                if (n.Length <= 2 && Game1.currentSong != null && Game1.currentSong.Name is string s && s.Length > 0)
                    name += ":" + s;
            }

            DelayedAction d = new DelayedAction(500, () => Game1.changeMusicTrack(name));
            Game1.delayedActions.Add(d);
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {

            if (!(e.NewLocation is GameLocation) || !e.IsLocalPlayer)
                return;
            CurrentEmitters.ForEach(a => a.Dispose());
            CurrentEmitters.Clear();

            if (locations.TryGetValue(e.NewLocation.Name, out string name))
            {
                if (name.ToLower().StartsWith("cm:"))
                {
                    string[] n = name.Split(':');
                    if (n.Length <= 2 && Game1.currentSong != null && Game1.currentSong.Name is string s && s.Length > 0)
                        name += ":" + s;
                }
                Game1.changeMusicTrack(name);
            }

            if (emitterPlacements.TryGetValue(e.NewLocation.Name, out List<SoundEmitterPlacement> placements))
            {
                foreach(SoundEmitterPlacement emitter in placements.Where(p => CustomMusicMod.checkConditions(p.Conditions)))
                {
                    if (emitter.SoundId.ToLower().StartsWith("cm:"))
                        emitter.SoundId = emitter.SoundId.Substring(3);

                    var songs = CustomMusicMod.Music.Where(m => m.Id == emitter.SoundId && CustomMusicMod.checkConditions(m.Conditions)).ToList();
                    if (songs.FirstOrDefault() is StoredMusic music)
                    {
                        var pos = emitter.GetPosition();
                        ActiveMusic active = new ActiveMusic(emitter.SoundId + "_" + pos.X + "_" + pos.Y, music.Sound.CreateInstance(), music.Ambient, music.Loop, pos, emitter.DistanceModifier, emitter.VolumeModifier, emitter.MaxTileDistance);
                        CurrentEmitters.Add(active);
                    }
                }
            }

            if(e.NewLocation.Map is Map map && map.Properties.TryGetValue("SoundEmitters", out PropertyValue emittersValue) 
                && emittersValue != null 
                && emittersValue.ToString() is string emittersText
                && emittersText != "")
            {
                var emittersData = emittersText.Split(';');
                foreach (string data in emittersData)
                {
                    if(data.Split(' ') is string[] emitter && emitter.Length >= 3 
                        && float.TryParse(emitter[0], out float x)
                        && float.TryParse(emitter[1], out float y)
                        && emitter[2] is string aname)
                    {
                        float distance = SoundEmitterPlacement.DefaultDistanceModifier;
                        float volume = SoundEmitterPlacement.DefaultVolumeModifier;
                        int maxDistance = SoundEmitterPlacement.DefaultMaxDistance;

                        if (aname.ToLower().StartsWith("cm:"))
                            aname = aname.Substring(3);

                        if (emitter.Length >= 4)
                            float.TryParse(emitter[3], out distance);

                        if (emitter.Length >= 5)
                            float.TryParse(emitter[4], out volume);

                        if (emitter.Length >= 6)
                            int.TryParse(emitter[5], out maxDistance);

                        var songs = CustomMusicMod.Music.Where(m => m.Id == aname && CustomMusicMod.checkConditions(m.Conditions)).ToList();
                        if (songs.FirstOrDefault() is StoredMusic music)
                        {
                            ActiveMusic active = new ActiveMusic(aname + "_" + x + "_" + y, music.Sound.CreateInstance(), music.Ambient, music.Loop, new Vector2(x,y), distance, volume, maxDistance);
                            CurrentEmitters.Add(active);
                        }
                    }
                }
            }

        }

        public static Type getTypeSDV(string type)
        {
            string prefix = "StardewValley.";
            Type defaulSDV = Type.GetType(prefix + type + ", Stardew Valley");

            if (defaulSDV != null)
                return defaulSDV;
            else
                return Type.GetType(prefix + type + ", StardewValley");

        }

        private void loadContentPacks()
        {
            List<string> ids = new List<string>();
            List<string> files = new List<string>();

            foreach (IContentPack pack in Helper.ContentPacks.GetOwned())
            {
                MusicContent content = pack.ReadJsonFile<MusicContent>("content.json");
                List<MusicItem> all = new List<MusicItem>();
                all.AddRange(content.Music);
                all.AddRange(content.Sounds);
                all.AddRange(content.Emitters);
                loads += all.Count;
                foreach (MusicItem music in all)
                {
                    string path = Path.Combine(pack.DirectoryPath, music.File);
                    addMusic(path, music);
                    if (!ids.Contains(music.Id))
                        ids.Add(music.Id);

                    if (!files.Contains(Path.GetFileNameWithoutExtension(path)))
                        files.Add(Path.GetFileNameWithoutExtension(path));
                }

                foreach (LocationItem location in content.Locations)
                {
                    if (location.MusicId != null && location.MusicId != "")
                    {
                        if (locations.ContainsKey(location.Location))
                            locations.Remove(location.Location);

                        locations.Add(location.Location, location.MusicId);
                    }

                    if (location.Emitters.Count > 0)
                        if (!emitterPlacements.ContainsKey(location.Location))
                            emitterPlacements.Add(location.Location, location.Emitters);
                        else
                            emitterPlacements[location.Location].AddRange(location.Emitters);
                }
            }

            List<GMCMOption> options = new List<GMCMOption>();

            foreach(var id in ids)
            {
                List<string> choices = new List<string>() { "CM:Default", "Vanilla", "Random", "Any" };
                
                foreach(var file in files.OrderBy(f => f))
                    choices.Add(Path.GetFileNameWithoutExtension(file));

                int activeIndex = 0;

                if (config.Presets.ContainsKey(id) && config.Presets[id] != "Default" && choices.Exists(c => c == config.Presets[id]))
                    activeIndex = choices.IndexOf(config.Presets[id]);

                choices[1] = choices[1] + ":" + id;

                options.Add(new GMCMOption(id, choices, "", activeIndex));
            }

            if (options.Count == 0)
                return;

            GMCMConfig gmcmConfig = new GMCMConfig(ModManifest, (key, value) =>
            {
                if (key == "save" && value == "file")
                {
                    Helper.WriteConfig<Config>(config);
                    return;
                }

                if (value == "CM:Default")
                    value = "Default";

                if (value.StartsWith("Vanilla:"))
                    value = "Vanilla";

                    if (!config.Presets.ContainsKey(key))
                        config.Presets.Add(key, value);
                    else
                        config.Presets[key] = value;
               

            }, options, new GMCMLabel("Music Choices", "Default: Content packs. Random: Packs and vanilla mixed. Any: All mixed."));

            while (!Context.IsGameLaunched)
                Thread.Sleep(1);

            if (gmcmConfig.register(Helper))
                Monitor.Log("Added Custom Music Config to GMCM", LogLevel.Info);
        }

        private void addMusic(string path, MusicItem music)
        {
            Monitor.Log("Loading " + music.File + " ...", LogLevel.Info);
            string oPath = path;

            string orgPath = path;
           
            SoundEffect soundEffect = LoadSoundEffect(path);

            if (soundEffect != null)
            {
                Monitor.Log(music.File + " Loaded", LogLevel.Trace);
                string[] ids = music.Id.Split(',').Select(p => p.Trim()).ToArray();
                foreach (string id in ids)
                    Music.Add(new StoredMusic(id, music.Preload ? soundEffect : null, music.Ambient, music.Loop, music.Conditions, path));
            }
            else
            {
                Monitor.Log(music.File + " failed to load", LogLevel.Warn);
            }
        }

        public static SoundEffect LoadSoundEffect(string path)
        {
            SoundEffect soundEffect = null;


            if (path.EndsWith(".wav"))
            {
                    FileStream stream = new FileStream(path, FileMode.Open);
                    try
                    {
                        soundEffect = SoundEffect.FromStream(stream);
                    }
                    catch (Exception e)
                    {
                        stream.Close();
                    }
                    stream.Close();
                    stream.Dispose();
            }
            else
                soundEffect = OggLoader.Load(path);

            if(soundEffect == null)
                SMonitor?.Log("Failed to load: " + path, LogLevel.Error);

            return soundEffect;
        }
        public SoundEffect Convert(string path)
        {
            string wavPath = path.Replace(".ogg", ".wav");
            SoundEffect soundEffect = null;
            int c = 0;
            Exception le = null;
            bool done = false;
            while (!done && c < ti)
            {
                FileStream stream = new FileStream(wavPath, FileMode.Create);
                try
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        OggDecoder decoder = new OggDecoder();
                        decoder.Initialize(File.OpenRead(path));
                        byte[] data = decoder.SelectMany(chunk => chunk.Bytes.Take(chunk.Length)).ToArray();
                        WriteWave(writer, decoder.Stereo ? 2 : 1, decoder.SampleRate, data);
                    }
                }
                catch (Exception e)
                {
                    le = e;
                    c++;
                    Thread.Sleep(slp);
                }
                done = true;
                stream.Close();
                stream.Dispose();
            }


            int d = 0;
            while (soundEffect == null && d < ti)
            {
                FileStream stream = stream = new FileStream(wavPath, FileMode.Open);

                try
                {
                        soundEffect = SoundEffect.FromStream(stream);
                }
                catch (Exception e)
                {
                    le = e;
                    d++;
                    Thread.Sleep(slp);
                }

                stream.Close();
                stream.Dispose();
            }




            /*if(soundEffect == null)
                Monitor.Log(path + ":" + le.Message + ":" + le.StackTrace, LogLevel.Trace);*/


            return soundEffect;
        }

        private static void WriteWave(BinaryWriter writer, int channels, int rate, byte[] data)
        {
            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write((36 + data.Length));
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(rate);
            writer.Write((rate * ((16 * channels) / 8)));
            writer.Write((short)((16 * channels) / 8));
            writer.Write((short)16);
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(data.Length);
            writer.Write(data);
        }

        public static bool checkConditions(string condition)
        {
            if (condition == null || condition == "")
                return true;

            GameLocation location = Game1.currentLocation;
            if (location == null)
                location = Game1.getFarm();

            if (location == null)
            {
                if (condition.StartsWith("r "))
                {
                    string[] cond = condition.Split(' ');
                    return Game1.random.NextDouble() <= double.Parse(cond[1]);
                }

                return false;
            }
            else
            {
                if (checkEventConditions != null)
                    return (bool)checkEventConditions.Invoke(null, new[] { condition, null });
                else
                    return (SHelper.Reflection.GetMethod(location, "checkEventPrecondition").Invoke<int>("9999999/" + condition) != -1);
            }
        }
    }
}

