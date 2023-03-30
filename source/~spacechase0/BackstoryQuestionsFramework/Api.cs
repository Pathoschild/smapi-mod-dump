/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace BackstoryQuestionsFramework
{
    public interface IApi
    {
        List<string> GetQuestionsAskedFor(Farmer perspective, NPC npc);
    }
    public class Api : IApi
    {
        public List<string> GetQuestionsAskedFor(Farmer perspective, NPC npc)
        {
            return perspective.friendshipData[npc.Name].get_questionsAsked().ToList();
        }
    }
}
