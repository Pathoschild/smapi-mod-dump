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
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;

namespace StardewArchipelago.Items.Unlocks
{
    public class MagicUnlockManager : IUnlockManager
    {
        public const string SPELL_CLEARDEBRIS_AP = "Spell: Clear Debris";
        public const string SPELL_TILL_AP = "Spell: Till";
        public const string SPELL_WATER_AP = "Spell: Water";
        public const string SPELL_BLINK_AP = "Spell: Blink";
        public const string SPELL_EVAC_AP = "Spell: Evac";
        public const string SPELL_HASTE_AP = "Spell: Haste";
        public const string SPELL_HEAL_AP = "Spell: Heal";
        public const string SPELL_BUFF_AP = "Spell: Buff";
        public const string SPELL_SHOCKWAVE_AP = "Spell: Shockwave";
        public const string SPELL_FIREBALL_AP = "Spell: Fireball";
        public const string SPELL_FROSTBITE_AP = "Spell: Frostbite";
        public const string SPELL_TELEPORT_AP = "Spell: Teleport";
        public const string SPELL_LANTERN_AP = "Spell: Lantern";
        public const string SPELL_TENDRILS_AP = "Spell: Tendrils";
        public const string SPELL_PHOTOSYNTHESIS_AP = "Spell: Photosynthesis";
        public const string SPELL_DESCEND_AP = "Spell: Descend";
        public const string SPELL_METEOR_AP = "Spell: Meteor";
        public const string SPELL_BLOODMANA_AP = "Spell: Bloodmana";
        public const string SPELL_LUCKSTEAL_AP = "Spell: Lucksteal";
        public const string SPELL_SPIRIT_AP = "Spell: Spirit";
        public const string SPELL_REWIND_AP = "Spell: Rewind";

        private Dictionary<string, Func<ReceivedItem, LetterAttachment>> _unlockables;

        public MagicUnlockManager()
        {
            _unlockables = new Dictionary<string, Func<ReceivedItem, LetterAttachment>>();
            _unlockables.Add(SPELL_CLEARDEBRIS_AP, SendClearDebrisSpell);
            _unlockables.Add(SPELL_TILL_AP, SendTillSpell);
            _unlockables.Add(SPELL_WATER_AP, SendWaterSpell);
            _unlockables.Add(SPELL_BLINK_AP, SendBlinkSpell);
            _unlockables.Add(SPELL_EVAC_AP, SendEvacSpell);
            _unlockables.Add(SPELL_HEAL_AP, SendHealSpell);
            _unlockables.Add(SPELL_HASTE_AP, SendHasteSpell);
            _unlockables.Add(SPELL_BUFF_AP, SendBuffSpell);
            _unlockables.Add(SPELL_DESCEND_AP, SendDescendSpell);
            _unlockables.Add(SPELL_FROSTBITE_AP, SendFrostbiteSpell);
            _unlockables.Add(SPELL_FIREBALL_AP, SendFireballSpell);
            _unlockables.Add(SPELL_TELEPORT_AP, SendTeleportSpell);
            _unlockables.Add(SPELL_SHOCKWAVE_AP, SendShockwaveSpell);
            _unlockables.Add(SPELL_TENDRILS_AP, SendTendrilsSpell);
            _unlockables.Add(SPELL_LANTERN_AP, SendLanternSpell);
            _unlockables.Add(SPELL_PHOTOSYNTHESIS_AP, SendPhotosynthesisSpell);
            _unlockables.Add(SPELL_METEOR_AP, SendMeteorSpell);
            _unlockables.Add(SPELL_BLOODMANA_AP, SendBloodmanaSpell);
            _unlockables.Add(SPELL_LUCKSTEAL_AP, SendLuckstealSpell);
            _unlockables.Add(SPELL_SPIRIT_AP, SendSpiritSpell);
            _unlockables.Add(SPELL_REWIND_AP, SendRewindSpell);
        }

        public bool IsUnlock(string unlockName)
        {
            return _unlockables.ContainsKey(unlockName);
        }

        public LetterAttachment PerformUnlockAsLetter(ReceivedItem unlock)
        {
            return _unlockables[unlock.ItemName](unlock);
        }

        private LetterAttachment SendClearDebrisSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("toil:cleardebris");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendTillSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("toil:till");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendWaterSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("toil:water");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendBlinkSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("toil:blink");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendEvacSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("life:evac");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendHealSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("life:heal");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendHasteSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("life:haste");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendBuffSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("life:buff");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendDescendSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("elemental:descend");
            return new LetterInformationAttachment(receivedItem);
        }       

        private LetterAttachment SendFireballSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("elemental:fireball");
            return new LetterInformationAttachment(receivedItem);
        }   

        private LetterAttachment SendFrostbiteSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("elemental:frostbite");
            return new LetterInformationAttachment(receivedItem);
        }   

        private LetterAttachment SendTeleportSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("elemental:teleport");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendShockwaveSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("nature:shockwave");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendTendrilsSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("nature:tendrils");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendLanternSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("nature:lantern");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendPhotosynthesisSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("nature:photosynthesis");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendMeteorSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("eldritch:meteor");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendBloodmanaSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("eldritch:bloodmana");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendLuckstealSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("eldritch:lucksteal");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendSpiritSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("eldritch:spirit");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendRewindSpell(ReceivedItem receivedItem)
        {
            ReceiveSpell("arcane:rewind");
            return new LetterInformationAttachment(receivedItem);
        }

        private void ReceiveSpell(string spellName)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                var magicType = AccessTools.TypeByName("Magic");
                var getSpellBookMethod = ModEntry.Instance.Helper.Reflection.GetMethod(magicType, "GetSpellBook");
                object[] getSpellBookArgs = { farmer };
                var spellBook = getSpellBookMethod.Invoke<object>(getSpellBookArgs);
                var learnSpellArgTypes = new Type[] { typeof(string), typeof(int), typeof(bool) };
                var spellBookType = AccessTools.TypeByName("SpellBook");
                var learnSpellMethod = AccessTools.Method(spellBookType, "LearnSpell", learnSpellArgTypes);
                object[] learnSpellArgs = { spellName, 0, false };
                learnSpellMethod.Invoke(spellBook, learnSpellArgs);
            }
        }
    }
}