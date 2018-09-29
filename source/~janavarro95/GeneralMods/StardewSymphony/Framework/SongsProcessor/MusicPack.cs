using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.StardewSymphony.Framework.SongsProcessor
{
    public class MusicPack
    {
        public MusicPackMetaData musicPackInformation;
        public List<Song> listOfAllSongs;

       
        public MusicPack(string Name,string pathToFiles)
        {
            string extentionInformation = Path.GetExtension(pathToFiles);


            if (extentionInformation==".xwb") {
                this.musicPackInformation = new MusicPackMetaData(Name,pathToFiles);
            }
            else
            {
                this.musicPackInformation = new MusicPackMetaData(Name);
                this.getAllWavFileFromDirectory();
            }
        }

       public void getAllWavFileFromDirectory()
        {
            string[] files = System.IO.Directory.GetFiles(musicPackInformation.fileLocation, "*.wav");

            foreach(var s in files)
            {
                Song song = new Song(Path.GetFileName(musicPackInformation.fileLocation), s, false);
                this.listOfAllSongs.Add(song);
            }
        }

        public void playSong(string name)
        {
            Song song = returnSong(name);
            if (song != null)
            {
                song.play();
            }
        }

        public void stopSong(string name)
        {
            Song song = returnSong(name);
            if (song != null)
            {
                song.stop();
            }
        }

        public void resumeSong(string name)
        {
            Song song = returnSong(name);
            if (song != null)
            {
                song.resume();
            }
        }

        public void pauseSong(string name)
        {
            Song song = returnSong(name);
            if (song != null)
            {
                song.pause();
            }
        }

        public void changeVolume(string name,float amount)
        {
            Song song = returnSong(name);
            if (song != null)
            {
                song.changeVolume(amount);
            }
        }

        /// <summary>
        /// Get's the song from the list.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Song returnSong(string Name)
        {
            return listOfAllSongs.Find(item => item.name == Name);
        }


    }
}
