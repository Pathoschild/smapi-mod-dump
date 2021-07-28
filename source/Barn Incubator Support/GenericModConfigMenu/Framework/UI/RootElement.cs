/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace GenericModConfigMenu.Framework.UI
{
    internal class RootElement : Container
    {
        public bool Obscured { get; set; } = false;

        public override int Width => 0;
        public override int Height => 0;

        public override void Update(bool hidden = false)
        {
            base.Update(hidden || this.Obscured);
            if (!hidden)
            {
                foreach (var child in this.Children)
                    child.Update(hidden);
            }
        }

        internal override RootElement GetRootImpl()
        {
            return this;
        }
    }
}
