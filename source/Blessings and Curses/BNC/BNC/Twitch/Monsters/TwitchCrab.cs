/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace BNC.Twitch
{
    class TwitchCrab : RockCrab, ITwitchMonster
    {
        public TwitchCrab() { }
        public TwitchCrab(Vector2 position) : base(position)
        {
        }
        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
            this.Name = username;
            this.displayName = username;
        }
    }
}
