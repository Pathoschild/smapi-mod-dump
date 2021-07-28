/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatoTK.Content
{
    public interface IConditionsProvider
    {
        string Id { get; }
        bool CanHandleConditions(string conditions);
        bool CheckConditions(string conditions, object caller);
    }
}
