/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Skill;
using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells.Components;
using ArsVenefici.Framework.Util;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;
using System;
using StardewValley.Buffs;

namespace ArsVenefici.Framework.Spells
{
    public class Spell : ISpell
    {
        private List<ShapeGroup> shapeGroups;
        private SpellStack spellStack;
        private int shapeGroupIndex = 0;

        private readonly Lazy<bool> continuous;
        private readonly Lazy<bool> empty;
        private readonly Lazy<bool> nonNull;
        private readonly Lazy<bool> valid;

        ModEntry modEntry;

        private string name;

        public Spell(ModEntry modEntry, List<ShapeGroup> shapeGroups, SpellStack spellStack)
        {
            this.modEntry = modEntry;
            this.shapeGroups = shapeGroups;
            this.spellStack = spellStack;

            continuous = new Lazy<bool>(() => FirstShape(CurrentShapeGroupIndex()) != null &&  FirstShape(CurrentShapeGroupIndex()).IsContinuous());

            empty = new Lazy<bool>(() => !ShapeGroups().Any() || ShapeGroups().All(g => g.IsEmpty()) && SpellStack().IsEmpty());

            nonNull = new Lazy<bool>(() => true);

            valid = new Lazy<bool>(() => Validate());
        }

        public static Spell of(ModEntry modEntry, SpellStack spellStack, params ShapeGroup[] shapeGroups)
        {
            return new Spell(modEntry, shapeGroups.ToList(), spellStack);
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public bool IsContinuous()
        {
            return continuous.Value;
        }

        public bool IsEmpty()
        {
            return empty.Value;
        }

        public bool IsNonNull()
        {
            return nonNull.Value;
        }

        public bool IsValid()
        {
            return valid.Value;
        }

        public ISpellShape FirstShape(int currentShapeGroup)
        {
            try
            {
                List<ISpellPart> list = ShapeGroup(currentShapeGroup).Parts();

                if(list.Count == 0)
                {
                    list = SpellStack().Parts;
                }
                
                ISpellShape spellShape = null;

                if (list.Any() && IsValid())
                {
                    spellShape = (ISpellShape)list.FirstOrDefault();
                }

                return spellShape;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public ShapeGroup ShapeGroup(int shapeGroup)
        {
            if (shapeGroup > ShapeGroups().Count() - 1) return null;

            return ShapeGroups()[shapeGroup];
        }

        public ShapeGroup CurrentShapeGroup()
        {
            return ShapeGroup(CurrentShapeGroupIndex()) ?? (Interfaces.Spells.ShapeGroup.EMPTY);
        }

        public int CurrentShapeGroupIndex()
        {
            return shapeGroupIndex;
        }

        public void CurrentShapeGroupIndex(int shapeGroup)
        {
            //if (shapeGroup >= ShapeGroups().Count() || shapeGroup < 0)
            //    throw new ArgumentOutOfRangeException("Invalid shape group index!");

            if(shapeGroup >= ShapeGroups().Count() || shapeGroup < 0)
                return;

            shapeGroupIndex = shapeGroup;
        }

        public SpellCastResult Cast(IEntity caster, GameLocation gameLocation, int castingTicks, bool consume, bool awardXp)
        {
            float manaValue = Mana();

            if (((Farmer)caster.entity).GetCurrentMana() < manaValue) 
                return new SpellCastResult(SpellCastResultType.NOT_ENOUGH_MANA);

            foreach (ISpellPart spellPart in SpellStack().Parts)
            {
                if (spellPart is Grow)
                {

                    if (modEntry.dailyTracker.GetDailyGrowCastCount() > modEntry.dailyTracker.GetMaxDailyGrowCastCount())
                    {
                        string message = modEntry.Helper.Translation.Get("world.max_grow_spell_cast.message.1") + "^" + modEntry.Helper.Translation.Get("world.max_grow_spell_cast.message.2") + "^" + modEntry.Helper.Translation.Get("world.max_grow_spell_cast.message.3") + "^";
                        Game1.activeClickableMenu = new DialogueBox(message);

                        return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
                    }
                    else if (Game1.player.hasBuff("HeyImAmethyst.ArsVenifici_GrowSickness"))
                    {
                        return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
                    }
                }
            }

            SpellHelper spellHelper = SpellHelper.Instance();
            SpellCastResult result = spellHelper.Invoke(modEntry, this, caster, gameLocation, null, castingTicks, 0, awardXp);

            int manaValueInt = (int)Math.Round(manaValue - (((Farmer)caster.entity).GetSpellBook().GetManaCostReductionAmount() / manaValue), 0, MidpointRounding.AwayFromZero);

            if (Game1.player.HasCustomProfession(Skill.Skill.ManaConservationProfession))
            {
                int manaCostPercentage = 75;
                int randomValueBetween0And99 = ModEntry.RandomGen.Next(100);

                if (randomValueBetween0And99 < manaCostPercentage)
                {
                    ((Farmer)caster.entity).AddMana(-manaValueInt);
                }
            }
            else
            {
                ((Farmer)caster.entity).AddMana(-manaValueInt);
            }

            if (awardXp && result.IsSuccess() && caster.entity is Farmer player)
            {
                bool continuous = IsContinuous();

                int xpTotal = 0;

                foreach (ISpellPart part in Parts())
                {
                    switch (part.GetType())
                    {
                        case SpellPartType.SHAPE:
                        case SpellPartType.MODIFIER:
                            xpTotal += 1;
                            break;
                        case SpellPartType.COMPONENT:
                            xpTotal += 1;
                            break;
                    }
                }

                int xp = xpTotal;

                if (continuous) xp /= 4;

                player.AddCustomSkillExperience(ModEntry.Skill, xp);
            }

            return result;
        }

        public List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> PartsWithModifiers()
        {
            ShapeGroup shapeGroup = ShapeGroup(CurrentShapeGroupIndex());

            //modEntry.Monitor.Log(SpellStack().PartsWithModifiers.Count.ToString(),StardewModdingAPI.LogLevel.Info);

            List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> pwm = new List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>>(SpellStack().PartsWithModifiers);
            List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>> shapesWithModifiers = new List<MutableKeyValuePair<ISpellPart, List<ISpellModifier>>>();
            
            if (shapeGroup != null)
            {
                ShapeGroup group = shapeGroup;

                List<MutableKeyValuePair<ISpellShape, List<ISpellModifier>>> li = group.ShapesWithModifiers();
                //List<LinkedListNode<KeyValuePair<ISpellPart, List<ISpellModifier>>>> lln = new List<LinkedListNode<KeyValuePair<ISpellPart, List<ISpellModifier>>>>(li.Where(c => new LinkedListNode<KeyValuePair<ISpellPart, List<ISpellModifier>>(c)));

                foreach (var item in li)
                {
                    MutableKeyValuePair<ISpellPart, List<ISpellModifier>> kp = new MutableKeyValuePair<ISpellPart, List<ISpellModifier>>(item.Key, item.Value);
                    shapesWithModifiers.Add(kp);
                }

                //shapesWithModifiers.AddRange(li);

                //var last = shapesWithModifiers.Last.Value;

                if(shapesWithModifiers.Count > 0)
                {
                    var last = shapesWithModifiers[shapesWithModifiers.Count - 1];

                    List<ISpellModifier> tmp = new List<ISpellModifier>();
                    //shapesWithModifiers.RemoveLast();
                    shapesWithModifiers.Remove(shapesWithModifiers[shapesWithModifiers.Count - 1]);

                    //shapesWithModifiers.AddLast(new KeyValuePair<ISpellPart, List<ISpellModifier>>(last.Key, tmp.ToList()));

                    tmp.AddRange(last.Value);
                    tmp.AddRange(pwm[0].Value);
                    shapesWithModifiers.Add(new MutableKeyValuePair<ISpellPart, List<ISpellModifier>>(last.Key, tmp));

                    //tmp.AddRange(last.Value);
                    //tmp.AddRange(pwm[0].Value);
                    //pwm.RemoveAt(0);
                }
            }
            //else
            //{
            //    pwm.RemoveAt(0);
            //}

            pwm.RemoveAt(0);

            //shapesWithModifiers.AddLast(pwm);
            shapesWithModifiers.AddRange(pwm);

            //modEntry.Monitor.Log(shapesWithModifiers.Count.ToString(), StardewModdingAPI.LogLevel.Info);
            return shapesWithModifiers.ToList();
        }

        public int Mana()
        {
            int cost = 0;
            int multiplier = 1;

            foreach (ISpellPart part in Parts())
            {
                switch (part.GetType())
                {
                    case SpellPartType.SHAPE:
                    case SpellPartType.MODIFIER:
                        multiplier *= part.ManaCost();
                        break;
                    case SpellPartType.COMPONENT:
                        cost += part.ManaCost();
                        break;
                }
            }

            if (multiplier == 0)
            {
                multiplier = 1;
            }

            cost *= multiplier;

            return cost;
        }

        public List<ShapeGroup> ShapeGroups()
        {
            return shapeGroups;
        }

        public SpellStack SpellStack()
        {
            return spellStack;
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            Spell spell = (Spell)o;
            return ShapeGroups().Equals(spell.ShapeGroups()) && SpellStack().Equals(spell.SpellStack()) && shapeGroupIndex.Equals(spell.shapeGroupIndex);
        }

        public override int GetHashCode()
        {
            int result = ShapeGroups().GetHashCode();
            result = 31 * result + SpellStack().GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return "Spell[shapeGroups=" + shapeGroups + ", spellStack=" + spellStack + ", shapeGroupIndex=" + shapeGroupIndex + ']';
        }

        private bool Validate()
        {
            if (IsEmpty() || !IsNonNull()) return false;

            //check spell stack
            if (SpellStack().IsEmpty()) return false;
            if (SpellStack().Parts[0].GetType() != SpellPartType.COMPONENT) return false;

            //find last non-empty shape group
            List<ShapeGroup> groups = ShapeGroups();

            if (groups.AsEnumerable().All(group => group.IsEmpty())) return false;

            int last = -1;
            for (int i = 0; i < groups.Count(); i++)
            {
                if (!groups[i].IsEmpty())
                {
                    last = i;
                }
            }

            //check for empty shape groups between other non-empty shape groups
            if (last == -1) return false;
            
            groups = groups.AsEnumerable().Where(e => !e.IsEmpty()).ToList();
            
            if (last != groups.Count() - 1) return false;
            //check shape groups themselves

            foreach (ShapeGroup group in groups)
            {
                if (group.Parts()[0].GetType() != SpellPartType.SHAPE) return false;
            }
            return true;
        }

        public List<ISpellPart> Parts()
        {
            List<ISpellPart> list = new List<ISpellPart>();
            list.AddRange(CurrentShapeGroup().Parts());
            list.AddRange(SpellStack().Parts);

            return list;
        }
    }
}
