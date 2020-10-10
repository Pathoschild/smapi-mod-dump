/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class TeleporterList
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public List<TeleporterDestination> Destinations;
#pragma warning restore CS0649

        public string ListName;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"TeleporterList({this.ListName}) => {{{string.Join(",", this.Destinations)}{'}'}";
        }
    }
}
