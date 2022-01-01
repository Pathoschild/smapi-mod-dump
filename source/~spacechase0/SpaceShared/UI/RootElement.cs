/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace SpaceShared.UI
{
    internal class RootElement : Container
    {
        /*********
        ** Accessors
        *********/
        public bool Obscured { get; set; } = false;

        public override int Width => 0;
        public override int Height => 0;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen || this.Obscured);
        }

        /// <inheritdoc />
        internal override RootElement GetRootImpl()
        {
            return this;
        }
    }
}
