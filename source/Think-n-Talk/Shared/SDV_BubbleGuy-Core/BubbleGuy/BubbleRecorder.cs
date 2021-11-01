/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/


using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using StardewModdingAPI;


namespace SDV_Speaker.Speaker
{
    public class BubbleRecorder
    {
        public enum RecorderStatus
        {
            Stopped,
            Playing,
            Recording,
            Paused
        }
        public List<SpeakerItem> CurrentRecording;
        public Dictionary<string, List<SpeakerItem>> Recordings;
        public string DataDirectory;
        
        public BubbleRecorder(string sDir)
        {
            DataDirectory = sDir;
            CurrentRecording = new List<SpeakerItem> { };
            Recordings = new Dictionary<string, List<SpeakerItem>> { };
            Status = RecorderStatus.Stopped;
        }
        public RecorderStatus Status { get; private set; }
        public void Play()
        {
            Status = RecorderStatus.Playing;
        }
        public void Stop()
        {
            Status = RecorderStatus.Stopped;
        }
        public void Record()
        {
            Status = RecorderStatus.Recording;
        }
        public void Pause()
        {
            Status = RecorderStatus.Paused;
        }
        public void DeleteItemFromCurrentRecording(SpeakerItem oDelete)
        {
            foreach (SpeakerItem oSpeaker in CurrentRecording)
            {
                if (oSpeaker.Equals(oDelete))
                {
                    CurrentRecording.Remove(oSpeaker);
                    break;
                }
            }
        }
        public void UpdateItemInCurrentRecording(SpeakerItem oOldValue, SpeakerItem oNewVal)
        {
            for (int iPtr = 0; iPtr < CurrentRecording.Count; iPtr++)
            {
                if (CurrentRecording[iPtr].Equals(oOldValue))
                {
                    CurrentRecording[iPtr] = oNewVal;
                    break;
                }
            }
        }
        public bool SaveRecording(string sRecordingName)
        {
            if (Recordings.ContainsKey(sRecordingName))
            {
                Recordings.Remove(sRecordingName);
            }

            Recordings.Add(sRecordingName, CurrentRecording);
            return true;
        }
        public void LoadRecordings()
        {
            string sFilename = Path.Combine(DataDirectory, "recordings.json");

            if (File.Exists(sFilename))
            {
                string sContent = File.ReadAllText(sFilename);

                Recordings = JsonConvert.DeserializeObject<Dictionary<string, List<SpeakerItem>>>(sContent);
            }
        }
        public void SaveRecordings()
        {
            File.WriteAllText(Path.Combine(DataDirectory, "recordings.json"), JsonConvert.SerializeObject(Recordings));
        }
    }
}
