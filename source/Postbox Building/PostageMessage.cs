/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;

namespace PostBoxMod
{
    class PostageMessage
    {
        public int itemId { set; get; }
        public string receiver { set; get; }
        public long senderId { set; get; }
        public PostageMessage(int itemId, String receiver, long senderId)
        {
            this.itemId = itemId;
            this.receiver = receiver;
            this.senderId = senderId;
        }
    }
}
