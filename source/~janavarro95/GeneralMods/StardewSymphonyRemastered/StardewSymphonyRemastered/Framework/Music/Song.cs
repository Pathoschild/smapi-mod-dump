using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// The class to be used to manage individual songs.
    /// </summary>
    public class Song
    {
        /// <summary>
        /// The path to the song. In the case of XACT songs this points to the .xwb file. For WAV files this points directly to the WAV file itself.
        /// </summary>
        private string pathToSong;

        /// <summary>
        /// The name of the song itself.
        /// </summary>
        public string name;

        /// <summary>
        /// The relative path to the song.
        /// </summary>
        private string relativePath;

        /// <summary>
        /// Blank Constructor;
        /// </summary>
        public Song()
        {

        }

        /// <summary>
        /// Constructor to be used for WAV files.
        /// </summary>
        /// <param name="PathToSong"></param>
        public Song(string PathToSong)
        {
            this.pathToSong=PathToSong;
            this.name = getNameFromPath(this.pathToSong);
            this.relativePath = this.getRelativePathFromFullPath();
        }

        /// <summary>
        /// Constructor to be used for XACT music packs.
        /// </summary>
        /// <param name="PathToSong"></param>
        /// <param name="Name"></param>
        public Song(string PathToSong,string Name)
        {
            this.pathToSong = PathToSong;
            this.name = Name;
            this.relativePath = this.getRelativePathFromFullPath();
        }

        /// <summary>
        /// Parse the name of the file from the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string getNameFromPath(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Get the relative path to this song from the full path on disk provided.
        /// </summary>
        /// <returns></returns>
        public string getRelativePathFromFullPath()
        {
            string[] spliter = this.pathToSong.Split(Path.DirectorySeparatorChar);
            string relative = "";
            int index = 0;
            foreach(var str in spliter) //iterate across all of the strings until Content is found.
            {
                
                if (str == "Content")
                {
                    //Once content is found add it to the relative string and append a newline character.
                    for(int i = index; i < spliter.Length; i++)
                    {
                        relative += spliter[i];
                        if (i != spliter.Length - 1)
                        {
                            relative += Path.DirectorySeparatorChar;
                        }
                    }
                }
                index++;
            }
            return relative; //Return the relative path string
        }

        /// <summary>
        /// Read the info from a .json file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Song ReadFromJson(string path)
        {
            return StardewSymphony.ModHelper.ReadJsonFile<Song>(path);
        }

        /// <summary>
        /// Write the information of the instance to a .json file.
        /// </summary>
        /// <param name="path">The path to which the json file is written to.</param>
        public void writeToJson(string path)
        {
            StardewSymphony.ModHelper.WriteJsonFile<Song>(path, this);
        }

        public string getRelativePath()
        {
            return this.relativePath;
        }

        public string getPathToSong()
        {
            return this.pathToSong;
        }
    }
}
