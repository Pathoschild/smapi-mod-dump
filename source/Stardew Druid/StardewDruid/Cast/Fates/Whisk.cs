/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast.Earth;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using static StardewValley.Menus.CharacterCustomization;

namespace StardewDruid.Cast.Fates
{
    internal class Whisk : CastHandle
    {

        Vector2 destination;

        public Whisk(Vector2 target, Rite rite, Vector2 Destination)
            : base(target, rite)
        {

            destination = Destination;

        }

        public override void CastEffect()
        {

            ModUtility.AnimateFateTarget(targetLocation, targetVector*64, destination*64);

            Event.World.Whisk whiskEvent = new(targetVector, riteData, destination);

            whiskEvent.EventTrigger();

            castFire = true;

        }

    }

}
