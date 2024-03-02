/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    public interface INotifyPropertyChanged
    {
        /// <summary>
        /// An event which fires when the value of a property has changed
        /// </summary>
        event PropertyChangedEventHandler? PropertyChanged;
    }
}
