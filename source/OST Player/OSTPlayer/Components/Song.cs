/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/


namespace OSTPlayer
{
    public class Song : IComparable<Song>
    {
        public string Id{get;set;}
        public string Name{get; set;}
        public bool isPlaying;
        public bool HasName;

        public Song(string id, string name, bool isPlaying = false){
            Id = id;
            Name = name ?? id;
            HasName = name != null;
            this.isPlaying = isPlaying;
        }

        public int CompareTo(Song other){
            return LogicUtils.CompareSongsByPos(this, other);
        }
    }
}
