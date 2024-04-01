/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace ObjectTriggers
{
    public class ObjectTriggerData
    {
        public string objectID;
        public string triggerType;
        public string tripperType;
        public string triggerEffectType;
        public string tripSound;
        public float tripChance = 1;
        public Action<GameLocation, Vector2, string, object> triggerEffectAction;
        public Action<GameLocation, Vector2, string, object> resetEffectAction;
        public string triggerEffectName;
        public bool targetTripper;
        public int radius;
        public float effectAmountMin;
        public float effectAmountMax;
        public float interval;
    }
}