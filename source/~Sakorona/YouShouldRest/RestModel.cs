/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System.Collections.Generic;

namespace TwilightShards.YouShouldRest
{
    public class RestModel
    {
        public Dictionary<string, string> Keys;

        public RestModel()
        {

        }

        public RestModel(Dictionary<string,string> otherKeys)
        {
            foreach (var k in otherKeys)
            {
                Keys.Add(k.Key, k.Value);
            }
        }
    }
}
