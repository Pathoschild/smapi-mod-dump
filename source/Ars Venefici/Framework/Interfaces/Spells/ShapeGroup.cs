/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using static HarmonyLib.Code;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    ///  A shape group is a part of a spell, which can be exchanged to change the casting, but not the effect of a spell.
    /// </summary>
    public class ShapeGroup
    {
        List<ISpellPart> parts;
        List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>> shapesWithModifiers;

        public static ShapeGroup EMPTY = Of(new List<ISpellPart>());

        public ShapeGroup(List<ISpellPart> parts, List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>> shapesWithModifiers)
        {
            this.parts = parts;
            this.shapesWithModifiers = shapesWithModifiers;
        }

        public static ShapeGroup Of(params ISpellPart[] parts)
        {
            return Of(new List<ISpellPart>(parts));
        }

        public static ShapeGroup Of(List<ISpellPart> parts)
        {
            List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>> map = new List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>>();
            List<ISpellModifier> currentMods = null;

            bool locked = false;
            bool first = true;

            foreach (ISpellPart part in parts)
            {
                if (part is ISpellComponent)
                    throw new MalformedShapeGroupException("Shape groups cannot contain components!", parts);
                if (part is ISpellShape shape)
                {
                    if (locked) throw new MalformedShapeGroupException("A shape cannot come after an end shape!", parts);

                    if (first && shape.NeedsPrecedingShape())
                        throw new MalformedShapeGroupException("A non begin shape cannot come first!", parts);

                    first = false;

                    currentMods = new List<ISpellModifier>();
                    map.Add(new MutableKeyValuePair<ISpellShape, List<ISpellModifier>>(shape, currentMods));

                    if (shape.IsEndShape())
                    {
                        locked = true;
                    }
                }

                if (part is ISpellModifier modifier) 
                {
                    if (currentMods == null)
                        throw new MalformedShapeGroupException("A modifier cannot come first in a shape group!", parts);

                    currentMods.Add(modifier);
                }
            }

            return new ShapeGroup(parts, map);
        }

        /// <returns>An list of all spell parts in this shape group.</returns>
        public List<ISpellPart> Parts()
        {
            return parts;
        }

        /// <returns>An list of all spell shapes with their associated modifiers in this shape group.</returns>
        public List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>> ShapesWithModifiers()
        {
            return shapesWithModifiers;
        }

        /// <returns>Whether this shape group is empty or not.</returns>
        public bool IsEmpty()
        {
            return !Parts().Any();
        }

        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            ShapeGroup that = (ShapeGroup)o;
            return (IsEmpty() && that.IsEmpty()) || Parts().Equals(that.Parts());
        }

        public override int GetHashCode()
        {
            return Parts().GetHashCode();
        }
    }

    public class MalformedShapeGroupException : Exception
    {
        private List<ISpellPart> parts;

        public MalformedShapeGroupException(String message, List<ISpellPart> parts) : base(message)
        {
            this.parts = parts;
        }

        /// <returns>A list of spell parts that caused the exception.</returns>
        public List<ISpellPart> GetParts()
        {
            return parts;
        }
    }
}
