/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System.Collections;

namespace CustomTokens
{
    public class PlayerDataToWrite
    {
        public int DeathCountMarried { get; set; }
        public int DeathCountMarriedOld { get; set; }
        public int PassOutCount { get; set; }

        public ArrayList AdditionalQuestsCompleted = new ArrayList();
    }
}
