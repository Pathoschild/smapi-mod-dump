/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Mizzion.Stardew.Common.Integrations
{
    internal interface IModIntegration
    {
        /*********
         ** Accessors
         *********/
        /// <summary>A human-readable name for the mod.</summary>
        string Label { get; }

        /// <summary>Whether the mod is available.</summary>
        bool IsLoaded { get; }
    }
}
