/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace GiftRejection
{
    public partial class ModEntry
    {
        private static void ThrowObject(Object o, NPC npc, float throwDistance)
        {
            var debris = Game1.createItemDebris(o.getOne(), npc.getStandingPosition(), npc.FacingDirection, null, -1);
            debris.chunksMoveTowardPlayer = false;
            for(int i = 0; i < debris.Chunks.Count; i++)
            {
                debris.Chunks[i].xVelocity.Value *= throwDistance;
                debris.Chunks[i].yVelocity.Value *= throwDistance;
            }
        }
    }
}