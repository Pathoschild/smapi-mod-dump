/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship
{
    public class ArchipelagoFriend
    {
        public string StardewName { get; private set; }
        public string ArchipelagoName { get; private set; }
        public bool Bachelor { get; private set; }
        public bool Pet { get; private set; }
        public bool RequiresGingerIsland { get; private set; }
        public bool RequiresDwarfLanguage { get; private set; }

        public ArchipelagoFriend(string stardewName, string archipelagoName, bool bachelor, bool pet, bool requiresGingerIsland, bool requiresDwarfLanguage)
        {
            StardewName = stardewName;
            ArchipelagoName = archipelagoName;
            Bachelor = bachelor;
            Pet = pet;
            RequiresGingerIsland = requiresGingerIsland;
            RequiresDwarfLanguage = requiresDwarfLanguage;
        }
    }
}
