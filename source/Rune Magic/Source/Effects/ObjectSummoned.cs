/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Effects
{
    public class ObjectSummoned : SpellEffect
    {
        public Object SummonedObject { get; set; }
        public Vector2 ObjectPosition { get; set; }

        public ObjectSummoned(Spell spell, Object summonedObject) : base(spell, Duration.Short)
        {
            SummonedObject = summonedObject;
            Start();
        }

        public override void Start()
        {
            base.Start();
            //create a Runic Anvil at the cursor location
            ObjectPosition = Game1.currentCursorTile;
            var summonedObject = SummonedObject;
            Game1.currentLocation.Objects.Add(ObjectPosition, summonedObject);
        }

        public override void End()
        {
            //remove the object
            Game1.currentLocation.Objects.Remove(ObjectPosition);
            base.End();
        }
    }
}