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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void CollectionChangedEventHandler(object sender, CollectionChangedEventArgs e);

    public class CollectionChangedEventArgs(IEnumerable? added = null, IEnumerable? removed = null) : EventArgs
    {
        public IEnumerable? Added { get; } = added;

        public IEnumerable? Removed { get; } = removed;
    }
}
