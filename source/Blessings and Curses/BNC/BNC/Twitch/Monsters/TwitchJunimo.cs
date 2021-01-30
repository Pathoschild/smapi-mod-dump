/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using System.Xml.Serialization;

namespace BNC
{
    class TwitchJunimo : Junimo, ITwitchMonster
    {
        public TwitchJunimo() { }

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
            this.Name = username;
            this.displayName = username;
        }
    }
}
