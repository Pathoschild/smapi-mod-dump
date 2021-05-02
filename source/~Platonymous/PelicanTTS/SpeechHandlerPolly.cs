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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Amazon.Polly;
using Amazon.Polly.Model;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using OggSharp;
using System.Threading.Tasks;

namespace PelicanTTS
{
    internal class SpeechHandlerPolly
    {
        internal static string lastText;
        internal static string lastSay;
        private static string lastDialog;
        private static string lastLetter;
        private static string lastHud;
        private static string lastChat;
        public static Queue<string> chats = new Queue<string>();
        public static string currentText;
        private static Thread speechThread;
        private static bool runSpeech;
        private static IModHelper Helper;


        internal static AmazonPollyClient pc;
        private static VoiceId currentVoice;
        private static string currentVoiceString;
        private static string tmppath;
        public static bool speak;
        private static string speakerName;
        private static SoundEffectInstance currentSpeech;

        public static IMonitor Monitor;

        public SpeechHandlerPolly()
        {

        }

        internal static string getLanguageCode(bool ignoreConfig = false)
        {

            if (!ignoreConfig && PelicanTTSMod.config.LanguageCode.Length >= 2)
                return PelicanTTSMod.config.LanguageCode;

            switch (LocalizedContentManager.CurrentLanguageCode)
            {
                case LocalizedContentManager.LanguageCode.de: return "de-DE";
                case LocalizedContentManager.LanguageCode.en: return "en-US";
                case LocalizedContentManager.LanguageCode.es: return "es-ES";
                case LocalizedContentManager.LanguageCode.fr: return "fr-FR";
                case LocalizedContentManager.LanguageCode.hu: return "hu-HU";
                case LocalizedContentManager.LanguageCode.it: return "it-IT";
                case LocalizedContentManager.LanguageCode.ja: return "ja-JA";
                case LocalizedContentManager.LanguageCode.ko: return "ko-KR";
                case LocalizedContentManager.LanguageCode.pt: return "pt-PT";
                case LocalizedContentManager.LanguageCode.ru: return "ru-RU";
                case LocalizedContentManager.LanguageCode.th: return "th-TH";
                case LocalizedContentManager.LanguageCode.tr: return "tr-TR";
                case LocalizedContentManager.LanguageCode.zh: return "cmn-CN";
            }

            return "en-US";
        }

        internal static void configSay(string name, string voice, string text, int rate = -1, float pitch = -1, float volume = -1)
        {
            Task.Run(() =>
           {
               currentVoice = VoiceId.FindValue(voice);
               tmppath = Path.Combine(PelicanTTSMod._helper.DirectoryPath, "TTS");

               if (pc == null)
                   pc = AWSHandler.getPollyClient();

               bool mumbling = PelicanTTSMod.config.MumbleDialogues;
               string language1 = "<lang xml:lang=\""+getLanguageCode()+"\">";
               string language2 = "</lang>";

               text = text.Replace("0g","0 gold").Replace("< ", " ").Replace("` ", "  ").Replace("> ", " ").Replace('^', ' ').Replace(Environment.NewLine, " ").Replace("$s", "").Replace("$h", "").Replace("$g", "").Replace("$e", "").Replace("$u", "").Replace("$b", "").Replace("$8", "").Replace("$l", "").Replace("$q", "").Replace("$9", "").Replace("$a", "").Replace("$7", "").Replace("<", "").Replace("$r", "").Replace("[", "<").Replace("]", ">");
               text = language1 + text + language2;

               bool neural = shouldUseNeuralEngine(voice, out string v);

               if (!neural && voice != v)
                   currentVoice = VoiceId.FindValue(v);

               bool useNeuralEngine = !mumbling && neural;

               var amzeffectIn = mumbling ? "<amazon:effect phonation='soft'><amazon:effect vocal-tract-length='-20%'>" : "<amazon:auto-breaths><amazon:effect phonation='soft'>";
               var amzeffectOut = mumbling ? "</amazon:effect></amazon:effect>" : "</amazon:effect></amazon:auto-breaths>";
               
               if (mumbling)
                   text = @"<speak>" + (useNeuralEngine ? "" : amzeffectIn) + Dialogue.convertToDwarvish(text) + (useNeuralEngine ? "" : amzeffectOut) + @"</speak>";
               else
                   text = @"<speak>" + (useNeuralEngine ? "" : amzeffectIn) + "<prosody rate='" + (rate == -1 ? PelicanTTSMod.config.Rate : rate) + "%'>" + text + @"</prosody>"+ (useNeuralEngine ? "" : amzeffectOut) + "</speak>";

               int hash = (text + (useNeuralEngine ? "-neural" : "")).GetHashCode();
               if (!Directory.Exists(Path.Combine(tmppath, name)))
                   Directory.CreateDirectory(Path.Combine(tmppath, name));

               string file = Path.Combine(Path.Combine(tmppath, name), "speech_" + currentVoice.Value + (mumbling ? "_mumble_" : "_") + hash + ".wav");
               SoundEffect nextSpeech = null;

               if (!File.Exists(file))
               {
                   SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                   sreq.Text = text;
                   sreq.TextType = TextType.Ssml;
                   sreq.OutputFormat = OutputFormat.Ogg_vorbis;
                   sreq.Engine = useNeuralEngine ? Engine.Neural : Engine.Standard;
                   sreq.VoiceId = currentVoice;
                   SynthesizeSpeechResponse sres = pc.SynthesizeSpeech(sreq);
                   using (var memStream = new MemoryStream())
                   {
                       sres.AudioStream.CopyTo(memStream);
                       nextSpeech = Convert(memStream, file);
                   }
               }
               using (FileStream stream = new FileStream(file, FileMode.Open))
                   nextSpeech = SoundEffect.FromStream(stream);

               if (currentSpeech != null)
                   currentSpeech.Stop();

               currentSpeech = nextSpeech.CreateInstance();

               speak = false;
               currentSpeech.Pitch =  (mumbling ? 0.5f : pitch == -1 ? PelicanTTSMod.config.Voices[name].Pitch : pitch);
               currentSpeech.Volume = volume == -1 ? PelicanTTSMod.config.Volume : volume;
               currentSpeech.Play();
           });
        }

        public static void start(IModHelper h)
        {
            Helper = h;
            currentText = "";
            tmppath = Path.Combine(Helper.DirectoryPath,"TTS");

            if (!Directory.Exists(tmppath))
                Directory.CreateDirectory(tmppath);

            if (pc == null)
                pc = AWSHandler.getPollyClient();

            currentVoice = VoiceId.Salli;
            lastText = "";
            lastDialog = "";
            lastHud = "";
            speak = false;
            runSpeech = true;

            setVoice("default");

            speechThread = new Thread(t2sOut);
            speechThread.Start();
            h.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            h.Events.Display.MenuChanged += OnMenuChanged;
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null)
            {
                lastText = "";
                lastDialog = "";
                lastLetter = "";
                currentText = "";
                currentSpeech?.Stop();
                setVoice("default");
            }
        }

        public static void stop()
        {
            Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            Helper.Events.Display.MenuChanged -= OnMenuChanged;
            runSpeech = false;
        }

        public static void setVoice(string name, bool female = true)
        {
            speakerName = name;

            string t = PelicanTTSMod.i18n.Get(name);

            if (name == "default" && PelicanTTSMod.config.Voices.ContainsKey("Default"))
                t = PelicanTTSMod.config.Voices["Default"].Voice;
            else if (PelicanTTSMod.config.Voices.ContainsKey(name))
                t = PelicanTTSMod.config.Voices[name].Voice;

            if (t.ToString() == "" || t.Contains("no translation"))
                t = PelicanTTSMod.i18n.Get("default_" + (female ? "female" : "male"));

            setVoiceById(t);
        }

        public static void setVoiceById(string id)
        {
            currentVoiceString = id;
            if (VoiceId.FindValue(id) is VoiceId vId1)
                currentVoice = vId1;
            else if (VoiceId.FindValue(PelicanTTSMod.i18n.Get("default")) is VoiceId vId2)
            {
                currentVoice = vId2;
                currentVoiceString = PelicanTTSMod.i18n.Get("default");
            }
            else
            {
                speakerName = "default";
                currentVoice = VoiceId.Salli;
                currentVoiceString = "Salli";
            }
        }

        public static string getVoice(string name, bool female = true)
        {
            speakerName = name;

            string t = PelicanTTSMod.i18n.Get(name);
            if (t.ToString() != "" && !t.Contains("no translation"))
                return t;

            return PelicanTTSMod.i18n.Get("default_" + (female ? "female" : "male"));
        }


        public static bool HasMethod(object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null;
        }

        private static void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(30))
            {
                if (Game1.activeClickableMenu is DialogueBox)
                {
                    DialogueBox dialogueBox = (DialogueBox)Game1.activeClickableMenu;

                    if (!dialogueBox.isPortraitBox() && !PelicanTTSMod.config.ReadNonCharacterMessages)
                        return;

                    if (dialogueBox.isPortraitBox() && !PelicanTTSMod.config.ReadDialogues)
                        return;

                    if (dialogueBox.isPortraitBox() && Game1.currentSpeaker != null)
                        setVoice(Game1.currentSpeaker.Name, Game1.currentSpeaker.Gender != 0);
                    else
                        setVoice("default");

                    if (dialogueBox.getCurrentString() != lastDialog)
                    {
                        currentText = dialogueBox.getCurrentString();
                        lastDialog = dialogueBox.getCurrentString();
                    }

                }
                else if (Game1.activeClickableMenu is LetterViewerMenu lvm && !PelicanTTSMod.config.MumbleDialogues && PelicanTTSMod.config.ReadLetters)
                {
                    setVoice("default");
                    List<string> mailMessage = Helper.Reflection.GetField<List<string>>(lvm, "mailMessage").GetValue();
                    string letter = mailMessage[Helper.Reflection.GetField<int>(lvm, "page").GetValue()];
                    if (letter != lastLetter)
                    {
                        currentText = letter;
                        lastLetter = letter;
                    }
                }
                else if (Game1.hudMessages.Count > 0 && !PelicanTTSMod.config.MumbleDialogues && PelicanTTSMod.config.ReadHudMessages)
                {
                    if (Game1.hudMessages[Game1.hudMessages.Count - 1].Message != lastHud)
                    {
                        setVoice("default");
                        currentText = Game1.hudMessages[Game1.hudMessages.Count - 1].Message;
                        lastHud = Game1.hudMessages[Game1.hudMessages.Count - 1].Message;
                    }
                }
                /*else if(chats.Count > 0 && PelicanTTSMod.config.ReadChatMessages)
                {
                    string chat = chats.Dequeue();
                    if (lastChat != chat)
                    {
                        setVoice("default");
                        currentText = chat;
                        lastChat = chat;
                        speak = true;
                    }
                }*/
            }
        }

        internal static bool shouldUseNeuralEngine(string voice, out string replacement)
        {
            replacement = voice;

            if (!PelicanTTSMod.config.UseNeuralVoices)
            {
                replacement = PelicanTTSMod.neuralReplacements.TryGetValue(voice, out string v) ? v : voice;
                return false;
            }

            return PelicanTTSMod.neural.Contains(voice);
        }

        internal static bool canUseNews(string voice)
        {
            return PelicanTTSMod.neural.Contains(voice);
        }


        internal static void t2sOut()
        {
            while (runSpeech)
            {
                try
                {
                    if (currentText == lastText) { continue; }

                    bool mumbling = PelicanTTSMod.config.MumbleDialogues && (currentText != lastSay);

                    if (currentText.StartsWith("+"))
                        continue;
                    currentText = currentText.Replace("0g","0 gold").Replace("< ", " ").Replace("` ", "  ").Replace("> ", " ").Replace('^', ' ').Replace(Environment.NewLine, " ").Replace("$s", "").Replace("$h", "").Replace("$g", "").Replace("$e", "").Replace("$u", "").Replace("$b", "").Replace("$8", "").Replace("$l", "").Replace("$q", "").Replace("$9", "").Replace("$a", "").Replace("$7", "").Replace("<", "").Replace("$r", "").Replace("[", "<").Replace("]", ">");

                    string language1 = "<lang xml:lang=\"" + getLanguageCode() + "\">";
                    string language2 = "</lang>";

                    currentText = language1 + currentText + language2;
                    
                    bool neural = shouldUseNeuralEngine(currentVoiceString, out string v);

                    if (!neural && currentVoiceString != v)
                        currentVoice = VoiceId.FindValue(v);

                    bool useNeuralEngine = !mumbling && neural;

                    bool news = false;

                    var style = news ? "<amazon:domain name=\"news\">" : "";
                    var style2 = news ? "</amazon:domain>" : "";


                    var amzeffectIn = mumbling ? "<amazon:effect phonation='soft'><amazon:effect vocal-tract-length='-20%'>" : "<amazon:auto-breaths><amazon:effect phonation='soft'>";
                    var amzeffectOut = mumbling ? "</amazon:effect></amazon:effect>" : "</amazon:effect></amazon:auto-breaths>";

                    if (mumbling)
                        currentText = @"<speak>" + (useNeuralEngine ? "" : amzeffectIn) + Dialogue.convertToDwarvish(currentText) + (useNeuralEngine ? "" : amzeffectOut) + @"</speak>";
                    else
                        currentText = @"<speak>" + style + (useNeuralEngine ? "" : amzeffectIn) + "<prosody rate='" + (PelicanTTSMod.config.Rate) + "%'>" + currentText + @"</prosody>" + (useNeuralEngine ? "" : amzeffectOut) + style2 + "</speak>";

                    int hash = (currentText + (useNeuralEngine ? "-neural" : "")).GetHashCode();
                    if (!Directory.Exists(Path.Combine(tmppath, speakerName)))
                        Directory.CreateDirectory(Path.Combine(tmppath, speakerName));

                    string file = Path.Combine(Path.Combine(tmppath, speakerName), "speech_" + currentVoice.Value + (mumbling ? "_mumble_" : "_") + hash + ".wav");
                    SoundEffect nextSpeech = null;

                    if (!File.Exists(file))
                    {
                        SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                        sreq.Text = currentText;
                        sreq.TextType = TextType.Ssml;
                        sreq.OutputFormat = OutputFormat.Ogg_vorbis;
                        sreq.VoiceId = currentVoice;
                        sreq.Engine = useNeuralEngine ? Engine.Neural : Engine.Standard;
                        SynthesizeSpeechResponse sres = pc.SynthesizeSpeech(sreq);
                        using (var memStream = new MemoryStream())
                        {
                            sres.AudioStream.CopyTo(memStream);
                            nextSpeech = Convert(memStream, file);
                        }
                    }
                    using (FileStream stream = new FileStream(file, FileMode.Open))
                        nextSpeech = SoundEffect.FromStream(stream);

                    if (currentSpeech != null)
                        currentSpeech.Stop();

                    currentSpeech = nextSpeech.CreateInstance();

                    if (Game1.activeClickableMenu is LetterViewerMenu || Game1.activeClickableMenu is DialogueBox || Game1.hudMessages.Count > 0 || speak)
                    {
                        speak = false;
                        currentSpeech.Pitch = (mumbling ? 0.5f : PelicanTTSMod.config.Pitch);
                        currentSpeech.Volume = PelicanTTSMod.config.Volume;

                        if (PelicanTTSMod.config.Voices.ContainsKey(speakerName))
                            currentSpeech.Pitch = (mumbling ? 0.5f : PelicanTTSMod.config.Voices[speakerName].Pitch);

                        currentSpeech.Play();
                    }
                    lastText = currentText;
                }
                catch
                {
                    lastText = currentText;
                }

                Thread.Sleep(500);
            }
        }

        public static SoundEffect Convert(Stream inputStream, string wavPath)
        {
            SoundEffect soundEffect = null;

            using (FileStream stream = new FileStream(wavPath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                OggDecoder decoder = new OggDecoder();
                decoder.Initialize(inputStream);
                byte[] data = decoder.SelectMany(chunk => chunk.Bytes.Take(chunk.Length)).ToArray();
                WriteWave(writer, decoder.Stereo ? 2 : 1, decoder.SampleRate, data);
            }

            using (FileStream stream = new FileStream(wavPath, FileMode.Open))
                soundEffect = SoundEffect.FromStream(stream);

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
    }
}
