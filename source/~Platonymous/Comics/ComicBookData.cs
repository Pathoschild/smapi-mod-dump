/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comics
{
    public class ComicBookData
    {
        string Name { get; set; } = "";

        string Description { get; set; } = "";

        string Volume { get; set; } = "";

        int Number { get; set; } = 0;

        string SmallUrl { get; set; } = "";

        string BigUrl { get; set; } = "";

        string Id { get; set; } = "";
    }
}
