/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;

namespace SocialNetwork
{
    public class SocialPostReaction
    {
        public SocialPost socialPost;
        public bool isComment;
        public NPC npc;
        public Texture2D portrait;
        public Rectangle sourceRect;
        public string text;
        public List<string> lines = new List<string>();

        public SocialPostReaction(SocialPost socialPost, string c, bool isComment)
        {
            this.socialPost = socialPost;
            this.isComment = isComment;
            string[] parts = c.Split('=');
            npc = Game1.getCharacterFromName(parts[0]);
            portrait = npc.Sprite.Texture;
            sourceRect = npc.getMugShotSourceRect();
            text = Utils.GetSocialNetworkString(parts[0], parts[1], isComment);
            lines = Utils.GetTextLines(text);
        }

        public void Refresh()
        {
            lines = Utils.GetTextLines(text);
        }
    }
}