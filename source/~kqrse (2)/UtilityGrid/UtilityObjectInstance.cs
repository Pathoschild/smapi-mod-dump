/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using static UtilityGrid.ModEntry;

namespace UtilityGrid
{
    public class UtilityObjectInstance
    {
        public UtilityObjectInstance(UtilityObject template, Object obj)
        {
            Template = template;
            WorldObject = obj;
        }

        public Vector2 CurrentPowerVector { get; set; }
        public PipeGroup Group { get; set; }
        public UtilityObject Template { get; set; }
        public Object WorldObject { get; set; }
    }
}