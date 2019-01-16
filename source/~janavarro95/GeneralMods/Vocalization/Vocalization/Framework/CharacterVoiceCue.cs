using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley;

namespace Vocalization.Framework
{
    /// <summary>A class that handles all of the storage of references to the audio files for this character.</summary>
    public class CharacterVoiceCue
    {
        /// <summary>The name of the NPC.</summary>
        public string name;

        /// <summary>The name of the dialogue file to scrape from Content/Characters/Dialogue for inputting values into the dictionary of dialogueCues.</summary>
        public List<string> dialogueFileNames;

        /// <summary>The name of the files in Content/Strings to scrape for dialogue.</summary>
        public List<string> stringsFileNames;

        /// <summary>The names of the files in Content/Data to scrape for dialogue.</summary>
        public List<string> dataFileNames;

        public List<string> festivalFileNames;
        public List<string> eventFileNames;

        /// <summary>A dictionary of dialogue strings that correspond to audio files.</summary>
        public Dictionary<string, VoiceAudioOptions> dialogueCues;

        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the NPC.</param>
        public CharacterVoiceCue(string name)
        {
            this.name = name;
            this.dialogueCues = new Dictionary<string, VoiceAudioOptions>();
            this.stringsFileNames = new List<string>();
            this.dialogueFileNames = new List<string>();
            this.dataFileNames = new List<string>();

            this.festivalFileNames = new List<string>();
            this.eventFileNames = new List<string>();
        }

        /// <summary>Plays the associated dialogue file.</summary>
        /// <param name="dialogueString">The current dialogue string to play audio for.</param>
        public void speak(string dialogueString)
        {
            VoiceAudioOptions voiceFileName = new VoiceAudioOptions();
            bool exists = this.dialogueCues.TryGetValue(dialogueString, out voiceFileName);
            if (exists)
            {
                Vocalization.soundManager.stopAllSounds();
                Vocalization.soundManager.playSound(voiceFileName.getAudioClip());
            }
            else
            {
                Vocalization.ModMonitor.Log("The dialogue cue for the current dialogue could not be found. Please ensure that the dialogue is added the character's voice file and that the proper file for the voice exists.");
                return;
            }
        }

        public void addDialogue(string key, VoiceAudioOptions value)
        {
            if (!this.dialogueCues.ContainsKey(key))
                this.dialogueCues.Add(key, value);
        }


        public void initializeEnglishScrape()
        {
            switch (this.name)
            {
                case "TV":
                    this.dataFileNames.Add("CookingChannel.xnb");
                    this.dataFileNames.Add("InterviewShow.xnb");
                    this.dataFileNames.Add("TipChannel.xnb");
                    this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                    break;

                case "Shops":
                    this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                    this.addDialogue("Welcome to Pierre's! Need some supplies?", new VoiceAudioOptions());
                    break;

                case "ExtraDialogue":
                    this.dataFileNames.Add("ExtraDialogue.xnb");
                    break;

                case "LocationDialogue":
                    this.stringsFileNames.Add("Locations.xnb");
                    this.stringsFileNames.Add("StringsFromMaps.xnb");
                    break;

                case "Events":
                    this.stringsFileNames.Add("Events.xnb");
                    this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                    break;

                case "Mail":
                    this.dataFileNames.Add("mail.xnb");
                    break;

                case "Characters":
                    this.stringsFileNames.Add("Characters.xnb");
                    this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                    break;

                case "Notes":
                    this.stringsFileNames.Add("Notes.xnb");
                    this.dataFileNames.Add("SecretNotes.xnb");
                    break;

                case "Utility":
                    this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                    break;

                case "NPCGiftTastes":
                    this.dataFileNames.Add("NPCGiftTastes.xnb");
                    break;

                case "SpeechBubbles":
                    this.stringsFileNames.Add("SpeechBubbles.xnb");
                    break;

                case "Quests":
                    this.dataFileNames.Add("Quests.xnb");
                    break;

                case "Temp":
                    this.stringsFileNames.Add("Temp.xnb");
                    break;

                default:
                    {
                        this.dialogueFileNames.Add(this.name + ".xnb");
                        this.dialogueFileNames.Add("rainy.xnb");
                        this.dialogueFileNames.Add("MarriageDialogue.xnb");
                        this.dialogueFileNames.Add("MarriageDialogue" + this.name + ".xnb");

                        this.dataFileNames.Add("EngagementDialogue.xnb");

                        this.stringsFileNames.Add("StringsFromCSFiles.xnb");
                        this.stringsFileNames.Add(this.name + ".xnb");

                        this.festivalFileNames.Add("fall16.xnb");
                        this.festivalFileNames.Add("fall27.xnb");

                        this.festivalFileNames.Add("spring13.xnb");
                        this.festivalFileNames.Add("spring24.xnb");

                        this.festivalFileNames.Add("summer11.xnb");
                        this.festivalFileNames.Add("summer28.xnb");

                        this.festivalFileNames.Add("winter8.xnb");
                        this.festivalFileNames.Add("winter25.xnb");

                        string content = Game1.content.RootDirectory;
                        string dir = Path.Combine(content, "Data", "Events");
                        string[] files = Directory.GetFiles(dir);
                        foreach (string file in files)
                        {
                            string eventFileName = Path.GetFileNameWithoutExtension(file);

                            string actualName = eventFileName.Split('.').ElementAt(0) + ".xnb";

                            //Gte first position of . and split it. The 0 element will be teh actual filename.
                            if (this.eventFileNames.Contains(actualName)) continue;
                            else this.eventFileNames.Add(actualName);
                        }
                    }
                    break;
            }
        }
        
        /// <summary>Change all of the files to the ones that are appropriate for that translation version.</summary>
        /// <param name="language">The translation language name.</param>
        public void initializeForTranslation(LanguageName language)
        {
            string extension = Vocalization.config.translationInfo.getFileExtentionForTranslation(language);

            for (int i = 0; i < this.dataFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(this.dataFileNames.ElementAt(i));
                string s = this.dataFileNames.ElementAt(i);
                s = this.dataFileNames.ElementAt(i).Replace(".xnb", extension);
                this.dataFileNames[i] = s;
                Vocalization.ModMonitor.Log(this.dataFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.dialogueFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(this.dialogueFileNames.ElementAt(i));
                string s = this.dialogueFileNames.ElementAt(i);
                s = this.dialogueFileNames.ElementAt(i).Replace(".xnb", extension);
                this.dialogueFileNames[i] = s;
                Vocalization.ModMonitor.Log(this.dialogueFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.stringsFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(this.stringsFileNames.ElementAt(i));
                string s = this.stringsFileNames.ElementAt(i);
                s = this.stringsFileNames.ElementAt(i).Replace(".xnb", extension);
                this.stringsFileNames[i] = s;
                Vocalization.ModMonitor.Log(this.stringsFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.festivalFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(this.festivalFileNames.ElementAt(i));
                string s = this.festivalFileNames.ElementAt(i);
                s = this.festivalFileNames.ElementAt(i).Replace(".xnb", extension);
                this.festivalFileNames[i] = s;
                Vocalization.ModMonitor.Log(this.festivalFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.eventFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(this.eventFileNames.ElementAt(i));
                string s = this.eventFileNames.ElementAt(i);
                s = this.eventFileNames.ElementAt(i).Replace(".xnb", extension);
                this.eventFileNames[i] = s;
                Vocalization.ModMonitor.Log(this.eventFileNames.ElementAt(i));
            }
        }
    }
}
