/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;

namespace Omegasis.Revitalize.Framework.Utilities.Extensions
{
    public static class NetFieldsExtension
    {

        public static void AddFields(this NetFields netFields, List<INetSerializable> netSerializables)
        {
            foreach (INetSerializable netSerializable in netSerializables)
                netFields.AddField(netSerializable);
        }
    }
}
