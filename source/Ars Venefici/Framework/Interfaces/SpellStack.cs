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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Interfaces
{
    public class SpellStack
    {

        public List<ISpellPart> Parts;
        public List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> PartsWithModifiers;

        public static readonly SpellStack Empty = Of(new List<ISpellPart>());

        public SpellStack(List<ISpellPart> parts, List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> PartsWithModifiers)
        {
            this.Parts = parts;
            this.PartsWithModifiers = PartsWithModifiers;
        }

        public static SpellStack Of(params ISpellPart[] parts)
        {
            return Of(parts.ToList());
        }

        public static SpellStack Of(List<ISpellPart> parts)
        {
            List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> partsWithModifiers = new List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>>();
            List<ISpellModifier> globalMods = new List<ISpellModifier>();
            List<ISpellModifier> currentMods = null;

            partsWithModifiers.Add(new MutableKeyValuePair<ISpellPart, List<ISpellModifier>>(null, ref globalMods));

            bool locked = false;

            foreach (ISpellPart part in parts)
            {
                if (part is ISpellComponent component) 
                {
                    currentMods = new List<ISpellModifier>();
                    partsWithModifiers.Add(new MutableKeyValuePair<ISpellPart, List<ISpellModifier>>(component, ref currentMods));
                }

                if (part is ISpellShape shape) 
                {
                    if (currentMods != null)
                        throw new MalformedSpellStackException("A shape cannot come after a component!", parts);
                    if (locked)
                        throw new MalformedSpellStackException("A shape cannot come after an end shape!", parts);

                    currentMods = new List<ISpellModifier>();
                    partsWithModifiers.Add(new MutableKeyValuePair<ISpellPart, List<ISpellModifier>>(shape, ref currentMods));
                    
                    if (shape.IsEndShape())
                        locked = true;
                }

                if (part is ISpellModifier modifier)
                {
                    //if (currentMods == null && globalMods == null)
                    //    continue;

                    (currentMods ?? globalMods).Add(modifier);
                }
            }

            return new SpellStack(parts, partsWithModifiers);
        }

        public bool IsEmpty() => Parts.Count == 0;

        public virtual bool Equals(SpellStack obj) 
        {
            return obj is SpellStack other && (IsEmpty() && other.IsEmpty() || Parts.SequenceEqual(other.Parts));
        }

        public override int GetHashCode()
        {
            return Parts.GetHashCode();
        }

        public class MalformedSpellStackException : Exception
        {
            public List<ISpellPart> Parts { get; }

            public MalformedSpellStackException(string message, List<ISpellPart> parts) : base(message)
            {
                Parts = parts;
            }
        }
    }
}
