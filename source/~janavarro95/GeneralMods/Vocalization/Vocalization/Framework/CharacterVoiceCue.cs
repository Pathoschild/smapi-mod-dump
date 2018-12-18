using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocalization.Framework
{
    /// <summary>
    /// A class that handles all of the storage of references to the audio files for this character.
    /// </summary>
    public class CharacterVoiceCue
    {
        /// <summary>
        /// The name of the NPC.
        /// </summary>
        public string name;

        /// <summary>
        /// The name of the dialogue file to scrape from Content/Characters/Dialogue for inputting values into the dictionary of dialogueCues.
        /// </summary>
        public List<string> dialogueFileNames;

        /// <summary>
        /// The name of the files in Content/Strings to scrape for dialogue.
        /// </summary>
        public List<string> stringsFileNames;

        /// <summary>
        /// The names of the files in Content/Data to scrape for dialogue.
        /// </summary>
        public List<string> dataFileNames;


        public List<string> festivalFileNames;
        public List<string> eventFileNames;



        /// <summary>
        /// A dictionary of dialogue strings that correspond to audio files.
        /// </summary>
        public Dictionary<string, VoiceAudioOptions> dialogueCues;




        /// <summary>
        /// Constructor.
        /// </summary>
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

        /// <summary>
        /// Plays the associated dialogue file.
        /// </summary>
        /// <param name="dialogueString">The current dialogue string to play audio for.</param>
        public void speak(string dialogueString)
        {
            VoiceAudioOptions voiceFileName =new VoiceAudioOptions();
            bool exists = dialogueCues.TryGetValue(dialogueString, out voiceFileName);
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
            if (dialogueCues.ContainsKey(key))
            {
                return;
            }
            else
            {
                this.dialogueCues.Add(key, value);
            }
        }


        public void initializeEnglishScrape()
        {
            
            if (name == "TV")
            {
                dataFileNames.Add("CookingChannel.xnb");
                dataFileNames.Add("InterviewShow.xnb");
                dataFileNames.Add("TipChannel.xnb");
                stringsFileNames.Add("StringsFromCSFiles.xnb");

            }
            else if (name == "Shops")
            {
                stringsFileNames.Add("StringsFromCSFiles.xnb");
                this.addDialogue("Welcome to Pierre's! Need some supplies?", new VoiceAudioOptions());
            }
            else if (name == "ExtraDialogue")
            {
                dataFileNames.Add("ExtraDialogue.xnb");
            }
            else if (name == "LocationDialogue")
            {
                stringsFileNames.Add("Locations.xnb");
                stringsFileNames.Add("StringsFromMaps.xnb");
            }
            else if (name == "Events")
            {
                stringsFileNames.Add("Events.xnb");
                stringsFileNames.Add("StringsFromCSFiles.xnb");

            }
            else if (name == "Mail")
            {
                dataFileNames.Add("mail.xnb");
            }
            else if (name == "Characters")
            {
                stringsFileNames.Add("Characters.xnb");
                stringsFileNames.Add("StringsFromCSFiles.xnb");
            }
            else if (name == "Notes")
            {
                stringsFileNames.Add("Notes.xnb");
                dataFileNames.Add("SecretNotes.xnb");
            }
            else if (name == "Utility")
            {
                stringsFileNames.Add("StringsFromCSFiles.xnb");
            }

            else if (name == "NPCGiftTastes")
            {
                dataFileNames.Add("NPCGiftTastes.xnb");
            }

            else if (name == "SpeechBubbles")
            {
                stringsFileNames.Add("SpeechBubbles.xnb");
            }

            else if (name == "Quests")
            {
                dataFileNames.Add("Quests.xnb");
            }

            else if (name == "Temp")
            {
                stringsFileNames.Add("Temp.xnb");
            }

            else
            {
                dialogueFileNames.Add(name + ".xnb");
                dialogueFileNames.Add("rainy.xnb");
                dialogueFileNames.Add("MarriageDialogue.xnb");
                dialogueFileNames.Add("MarriageDialogue" + name + ".xnb");

                dataFileNames.Add("EngagementDialogue.xnb");

                stringsFileNames.Add("StringsFromCSFiles.xnb");
                stringsFileNames.Add(name + ".xnb");

                festivalFileNames.Add("fall16.xnb");
                festivalFileNames.Add("fall27.xnb");

                festivalFileNames.Add("spring13.xnb");
                festivalFileNames.Add("spring24.xnb");

                festivalFileNames.Add("summer11.xnb");
                festivalFileNames.Add("summer28.xnb");

                festivalFileNames.Add("winter8.xnb");
                festivalFileNames.Add("winter25.xnb");

                string content = Game1.content.RootDirectory;
                string dir = Path.Combine(content, "Data", "Events");
                string[] files = Directory.GetFiles(dir);
                foreach(var file in files)
                {
                    string eventFileName = Path.GetFileNameWithoutExtension(file);

                    string actualName = eventFileName.Split('.').ElementAt(0)+".xnb";

                    //Gte first position of . and split it. The 0 element will be teh actual filename.
                    if (eventFileNames.Contains(actualName)) continue;
                    else eventFileNames.Add(actualName);
                }
            }
        }


        /// <summary>
        /// Change all of the files to the ones that are appropriate for that translation version.
        /// </summary>
        /// <param name="translation"></param>
        public void initializeForTranslation(string translation)
        {
            for (int i = 0; i < this.dataFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(dataFileNames.ElementAt(i));
                string s = dataFileNames.ElementAt(i);
                s=dataFileNames.ElementAt(i).Replace(".xnb", Vocalization.config.translationInfo.getFileExtentionForTranslation(translation));
                dataFileNames[i] = s;
                Vocalization.ModMonitor.Log(dataFileNames.ElementAt(i));

            }

            for (int i = 0; i < this.dialogueFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(dialogueFileNames.ElementAt(i));
                string s = dialogueFileNames.ElementAt(i);
                s=dialogueFileNames.ElementAt(i).Replace(".xnb", Vocalization.config.translationInfo.getFileExtentionForTranslation(translation));
                dialogueFileNames[i] = s;
                Vocalization.ModMonitor.Log(dialogueFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.stringsFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(stringsFileNames.ElementAt(i));
                string s = stringsFileNames.ElementAt(i);
                s=stringsFileNames.ElementAt(i).Replace(".xnb", Vocalization.config.translationInfo.getFileExtentionForTranslation(translation));
                stringsFileNames[i] = s;
                Vocalization.ModMonitor.Log(stringsFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.festivalFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(festivalFileNames.ElementAt(i));
                string s = festivalFileNames.ElementAt(i);
                s = festivalFileNames.ElementAt(i).Replace(".xnb", Vocalization.config.translationInfo.getFileExtentionForTranslation(translation));
                festivalFileNames[i] = s;
                Vocalization.ModMonitor.Log(festivalFileNames.ElementAt(i));
            }

            for (int i = 0; i < this.eventFileNames.Count; i++)
            {
                Vocalization.ModMonitor.Log(eventFileNames.ElementAt(i));
                string s = eventFileNames.ElementAt(i);
                s = eventFileNames.ElementAt(i).Replace(".xnb", Vocalization.config.translationInfo.getFileExtentionForTranslation(translation));
                eventFileNames[i] = s;
                Vocalization.ModMonitor.Log(eventFileNames.ElementAt(i));
            }
        }



    }
}
