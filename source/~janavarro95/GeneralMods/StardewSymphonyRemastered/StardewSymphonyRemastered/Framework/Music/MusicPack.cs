using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// A base class that xnb and wav packs will derive commonalities from.
    /// //Make Music Pack Meta data
    /// </summary>
    public class MusicPack 
    {
        
        public string directory;
        public string shortenedDirectory;
        public StardewSymphonyRemastered.Framework.SongSpecifics songInformation;
        public MusicPackMetaData musicPackInformation;


        public virtual void playSong(string name)
        {

        }

        public virtual void pauseSong()
        {

        }

        public virtual void stopSong()
        {

        }

        public virtual void resumeSong()
        {

        }

        public virtual void loadMusicFiles()
        {

        }

        public virtual void swapSong(string songName)
        {

        }

        public virtual string getNameOfCurrentSong()
        {
            return "";
        }

        public virtual void setModDirectoryFromFullDirectory()
        {
            
        }

        public virtual void playRandomSong()
        {

        }

        public virtual bool isPlaying()
        {
            return false;
        }

        /// <summary>
        /// Save functionality.
        /// </summary>
        public virtual void writeToJson()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Saving music for this pack:"+this.musicPackInformation.name+". Please wait.");
            string data = Path.Combine(this.directory, "data");
            if (!Directory.Exists(data))
            {
                Directory.CreateDirectory(data);
            }
            foreach (var list in this.songInformation.listOfSongsWithTriggers)
            {
                if (list.Value.Count == 0) continue;
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Saving music: " + list.Key + ". Please wait.");
                SongListNode node = new SongListNode(list.Key, list.Value);
                node.WriteToJson(Path.Combine(data, node.trigger+".json"));
            }
        }

        /// <summary>
        /// Load functionality.
        /// </summary>
        public virtual void readFromJson()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Loading music for this pack:" + this.musicPackInformation.name + ". Please wait as this will take quite some time.");
            string data = Path.Combine(this.directory, "data");
            if (!Directory.Exists(data))
            {
                Directory.CreateDirectory(data);
            }
            string[] files = Directory.GetFiles(data);
            foreach (var file in files)
            {
                SongListNode node = SongListNode.ReadFromJson(Path.Combine(data,file));
                var pair = this.songInformation.getSongList(node.trigger+".json");
                foreach (var v in node.songList)
                {
                    try
                    {
                        this.songInformation.addSongToTriggerList(node.trigger, v.name);
                    }
                    catch(Exception err)
                    {

                    }
                }
            }

        }
    }
}
