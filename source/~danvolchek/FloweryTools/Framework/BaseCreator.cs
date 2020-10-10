/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using FloweryTools.Framework.Flowerers;

namespace FloweryTools.Framework
{
    internal abstract class BaseCreator
    {
        protected FlowerHelper helper;

        public BaseCreator(FlowerHelper helper)
        {
            this.helper = helper;
        }
    }
}
