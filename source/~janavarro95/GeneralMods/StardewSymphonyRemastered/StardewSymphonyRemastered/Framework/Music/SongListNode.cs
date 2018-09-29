using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// A class that keeps track of the trigger and the list of songs associated with that trigger.
    /// </summary>
    class SongListNode
    {
        /// <summary>
        /// The trigger name for the list of songs.
        /// </summary>
        public string trigger;
        /// <summary>
        /// The list of songs associated with a trigger.
        /// </summary>
        public List<Song> songList;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public SongListNode()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Trigger"></param>
        /// <param name="SongList"></param>
        public SongListNode(string Trigger, List<Song> SongList)
        {
            this.trigger = Trigger;
            this.songList = SongList;
        }

        /// <summary>
        /// Save functionality.
        /// </summary>
        /// <param name="path"></param>
        public void WriteToJson(string path)
        {
            StardewSymphony.ModHelper.WriteJsonFile(path, this);
        }
        
        /// <summary>
        /// Load functionality.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SongListNode ReadFromJson(string path)
        {
            return StardewSymphony.ModHelper.ReadJsonFile<SongListNode>(path);
        }

    }
}
