/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace CustomEmotes
{
    public interface ICustomEmotesApi
    {
        void DoEmote(Character character, string emoteName);
        void DoEmote(string characterName, string emoteName);
        Dictionary<string, int> GetEmoteMap();
    }
}