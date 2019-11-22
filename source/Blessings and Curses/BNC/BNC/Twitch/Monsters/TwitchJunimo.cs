using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using System.Xml.Serialization;

namespace BNC
{
    class TwitchJunimo : Junimo, ITwitchMonster
    {

        public TwitchJunimo(Vector2 position) : base(position, 0, true) { }

        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
        }
    }
}
