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

namespace PlatoTK
{
    internal abstract class InnerHelper
    {
        protected readonly IPlatoHelper Plato;

        public InnerHelper(IPlatoHelper helper)
        {
            Plato = helper;
        }
    }
}
