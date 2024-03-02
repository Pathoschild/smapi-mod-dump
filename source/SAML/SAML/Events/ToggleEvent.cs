/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void ToggleEventHandler(object sender, ToggleEventArgs e);

    public class ToggleEventArgs(bool isToggled) : EventArgs
    {
        /// <summary>
        /// Whether the toggle was set to on or off
        /// </summary>
        public bool IsToggled { get; } = isToggled;
    }
}
