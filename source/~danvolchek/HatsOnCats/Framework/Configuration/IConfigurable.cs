/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.Collections.Generic;
using HatsOnCats.Framework.Interfaces;
using Microsoft.Xna.Framework;

namespace HatsOnCats.Framework.Configuration
{
    internal interface IConfigurable : INamed
    {
        IDictionary<Frame, Offset> Configuration { get; set; }
    }
}
