/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdy/SDVModding
**
*************************************************/

using StardewModdingAPI;
using System;

namespace SDVFactory
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper)
        {
            Factory.FactoryGame.Initialize(helper, Monitor);
        }
    }
}
