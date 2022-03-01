/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace CustomEmotes.Model
{
    internal class Emote
    {
        public string name;
        public Texture2D texture;
        public int index;

        public Emote(string name, Texture2D texture, int index)
        {
            this.name = name;
            this.texture = texture;
            this.index = index;
        }
    }
}