/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader2.Structure.Version1
{
    class ShopItem
    {
#pragma warning disable CS0649
#pragma warning disable CS0169
        string ItemName;
        int Price;
        int Stock=int.MaxValue;
        string Conditions;
#pragma warning restore CS0649
#pragma warning restore CS0169
    }
}
