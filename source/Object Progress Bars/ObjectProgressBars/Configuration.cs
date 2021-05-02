/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdeelTariq/ObjectProgressBars
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace ObjectProgressBars
{
    public class Configuration
    {
        public InputButton ToggleDisplay
        {
            get;
            set;
        } = new InputButton((Keys)80);

		public bool DisplayProgressBars
        {
            get;
            set;
		} = true;

    }
}
