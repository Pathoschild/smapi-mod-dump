/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void FocusChangedEventHandler(object sender, FocusChangedEventArgs e);

    public class FocusChangedEventArgs(IFocusable? focused = null) : EventArgs
    {
        /// <summary>
        /// The value of the currently focused <see cref="IFocusable"/>
        /// </summary>
        public IFocusable? Focused { get; } = focused;
    }
}
