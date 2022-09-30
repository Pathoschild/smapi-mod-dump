/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;

namespace Omegasis.StardustCore.UIUtilities.MenuComponents.Delegates
{
    public class DelegatePairing
    {
        public Delegates.paramaterDelegate click;
        public List<object> paramaters;

        public DelegatePairing(Delegates.paramaterDelegate buttonDelegate, List<object> Paramaters)
        {
            this.click = buttonDelegate;
            this.paramaters = Paramaters ?? new List<object>();
        }

        public void run()
        {
            this.click?.Invoke(this.paramaters);
        }
    }
}
