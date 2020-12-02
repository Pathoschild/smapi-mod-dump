/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityForBlind
{
    public static class TextToSpeech
    {
        private static string executable;
        private static bool initialized = false;

        private const string linux_mimic_std = "/usr/bin/mimic";
        private const string linux_mimic_manual_install = "mimic/mimic";
        public static void Init()
        {
            initialized = false;
            switch (StardewModdingAPI.Constants.TargetPlatform)
            {
                case StardewModdingAPI.GamePlatform.Android:
                    ModEntry.Log("Android detected. Speech2Text is not supported.", StardewModdingAPI.LogLevel.Error);
                    break;
                case StardewModdingAPI.GamePlatform.Linux:
                    if (ModEntry.GetConfig().tts_path.Length > 0)
                    {
                        if (System.IO.File.Exists(ModEntry.GetConfig().tts_path))
                        {

                            executable = ModEntry.GetConfig().tts_path;
                            initialized = true;
                            ModEntry.Log("Using custom tts installation specified in config.json", StardewModdingAPI.LogLevel.Debug);
                        }
                        else
                        {
                            ModEntry.Log("Couldn't find specified tts installation '" + ModEntry.GetConfig().tts_path + "' from config.json", StardewModdingAPI.LogLevel.Error);
                        }
                    }
                    else if (System.IO.File.Exists(linux_mimic_std))
                    {
                        executable = linux_mimic_std;
                        initialized = true;
                        ModEntry.Log("Using system mimic installation", StardewModdingAPI.LogLevel.Debug);
                    }
                    else
                    {
                        string path = ModEntry.GetHelper().DirectoryPath + "/" + linux_mimic_manual_install;
                        if (System.IO.File.Exists(path))
                        {
                            executable = path;
                            initialized = true;
                            ModEntry.Log("Using mimic installation in mod folder", StardewModdingAPI.LogLevel.Debug);
                        }
                        else
                        {
                            ModEntry.Log("Couldn't find mimic installation at " + linux_mimic_std + " or in mod folder", StardewModdingAPI.LogLevel.Error);
                        }
                    }
                    break;
                case StardewModdingAPI.GamePlatform.Mac:
                    ModEntry.Log("OSX detected. Speech2Text is not supported.", StardewModdingAPI.LogLevel.Error);
                    break;
                case StardewModdingAPI.GamePlatform.Windows:
                    ModEntry.Log("Windows detected. Speech2Text is not supported.", StardewModdingAPI.LogLevel.Error);
                    break;
            }
            if (initialized)
                queueTask.Start();
        }

        private static Process proc;
        private static Speech last_speech;
        private static Queue<Speech> queue = new Queue<Speech>();

        public struct Speech
        {
            public string text;
            public Gender gender;
            public float speed;
            public bool overwrite;
        }

        public enum Gender {
            Male,
            Female,
            Neutral
        }

        public static bool Speaking() =>
            proc != null && !proc.HasExited;

        public static bool QueuedSpeech() => queue.Count > 0;

        public static void Stop()
        {
            if (Speaking())
                proc.Kill();

            queue.Clear();
        }

        public static void Repeat()
        {
            Speak(last_speech);
        }

        public static void SpeakQueued(Speech speech)
        {
            queue.Enqueue(speech);
            //speak(speech);

        }

        static Task queueTask = new Task(() =>
        {
            while (true)
            {
                System.Threading.Thread.Sleep(200);
                while (queue.Count > 0 && !Speaking())
                {
                    Speech q = queue.Dequeue();
                    speak(q);
                };
            }
        });

        public static void Speak(Speech speech, bool overwrite = true) =>
            Speak(speech.text, speech.gender, speech.speed);


        public static void Speak(string text, Gender gender=Gender.Neutral, float speed = 1.0f, bool overwrite = true) 
        {
            if (!initialized)
                return;

            last_speech.text = text;
            last_speech.gender = gender;
            last_speech.speed = speed;

            Speech speech = new Speech()
            {
                text = text,
                gender = gender,
                speed = speed
            };

            if (overwrite)
            {
                queue.Clear();
                Stop();
            }

            SpeakQueued(speech);
        }

        private static string GetSpeedArg()
        {
            float speed = 1 + 1 - ModEntry.GetConfig().tts_speed / 100f;
            return "--setf duration_stretch=" + speed.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string GetPitchArg()
        {
            int pitch = ModEntry.GetConfig().tts_pitch;
            return "--setf int_f0_target_mean=" + pitch;
        }

        private static void speak(Speech speech)
        {
            try
            {
                proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = executable;
                string voice()
                {
                    switch (speech.gender)
                    {
                        case Gender.Male: return "-voice rms";
                        case Gender.Female: return "-voice slt";
                        case Gender.Neutral: return "-voice ap";
                        default: return "";
                    }
                }
                proc.StartInfo.Arguments = $"{GetSpeedArg()} {GetPitchArg()} {voice()} -t " + "\"" + speech.text.Replace('"', '\'') + "\"";
                //ModEntry.Log("mimic args: " + proc.StartInfo.Arguments);
                proc.Start();
                ModEntry.Log("speaking '" + speech.text + "'");

            }
            catch (Exception e)
            {
                ModEntry.Log("Failed Text2Speech: " + e.Message, StardewModdingAPI.LogLevel.Error);
                proc = null;
            }

        }

        public static string ItemToSpeech(StardewValley.Item item)
        {
            if (item != null)
            {
                if (item.Stack > 1)
                    return item.Stack + " " + item.DisplayName;
                else
                    return item.DisplayName;
            }
            else
            {
                return "none";
            }
        }

    }

}
