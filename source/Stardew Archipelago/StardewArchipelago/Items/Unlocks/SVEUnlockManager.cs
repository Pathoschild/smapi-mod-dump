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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;

namespace StardewArchipelago.Items.Unlocks
{
    public class SVEUnlockManager : IUnlockManager
    {
        public const string DIAMOND_WAND_AP_NAME = "Diamond Wand";
        public const string MARLON_BOAT_PADDLE_AP_NAME = "Marlon's Boat Paddle";
        public const string VOID_SOUL_AP_NAME = "Void Soul";
        public const string IRIDIUM_BOMB_AP_NAME = "Iridium Bomb";
        public const string KITTYFISH_SPELL_AP_NAME = "Kittyfish Spell";
        public const string GUILD_RUNES_AP_NAME = "Nexus: Adventurer's Guild Runes";
        public const string SPRITE_RUNES_AP_NAME = "Nexus: Sprite Spring Runes";
        public const string JUNIMO_RUNES_AP_NAME = "Nexus: Junimo and Outpost Runes";
        // public const string OUTPOST_RUNES_AP_NAME = "Nexus: Outpost Runes";
        public const string FARM_RUNES_AP_NAME = "Nexus: Farm and Wizard Runes";
        // public const string WIZARD_RUNES_AP_NAME = "Nexus: Wizard Runes";
        public const string AURORA_RUNES_AP_NAME = "Nexus: Aurora Vineyard Runes";
        public const string FABLE_REEF_AP_NAME = "Fable Reef Portal";
        public const string AURORA_VINEYARD_TABLET_AP_NAME = "Aurora Vineyard Tablet";
        public const string SCARLETT_JOB_OFFER = "Scarlett's Job Offer";
        public const string MORGAN_SCHOOLING = "Morgan's Schooling";
        public const int MAIN_NEXUS_EVENT = 908071;
        public const int WIZARD_RUNES_EVENT = 908072;
        public const int GUILD_RUNES_EVENT = 908073;
        public const int FARM_RUNES_EVENT = 908074;
        public const int AURORA_RUNES_EVENT = 908075;
        public const int SPRITE_RUNES_EVENT = 908076;
        public const int JUNIMO_RUNES_EVENT = 908077;
        public const int OUTPOST_RUNES_EVENT = 908078;
        public const int LANCE_WARP_EVENT = 65360191;
        public const int KITTYFISH_EVENT = 69069247;
        public const int SCARLETT_EVENT = 3691371;
        public const int AURORA_EVENT = 658059254;
        public const int MORGAN_EVENT = 658078924;

        private Dictionary<string, Func<ReceivedItem, LetterAttachment>> _unlockables;
        
        public bool IsUnlock(string unlockName)
        {
            return _unlockables.ContainsKey(unlockName);
        }

        public LetterAttachment PerformUnlockAsLetter(ReceivedItem unlock)
        {
            return _unlockables[unlock.ItemName](unlock);
        }
        public SVEUnlockManager()
        {
            _unlockables = new Dictionary<string, Func<ReceivedItem, LetterAttachment>>();
            RegisterSVEUniqueItems();
            RegisterPlayerWarps();
            RegisterSVEPlayerImprovement();
            RegisterSVEVillagerInvitations();
        }

        private void RegisterSVEPlayerImprovement()
        {
            _unlockables.Add(MARLON_BOAT_PADDLE_AP_NAME, SendMarlonBoat);
            _unlockables.Add(VOID_SOUL_AP_NAME, SendVoidSoul);
            _unlockables.Add(IRIDIUM_BOMB_AP_NAME, SendIridiumBomb);
            _unlockables.Add(KITTYFISH_SPELL_AP_NAME, SendKittyfishSpell);
        }

        private void RegisterSVEVillagerInvitations()
        {
            _unlockables.Add(SCARLETT_JOB_OFFER, SendScarlettJobOffer);
            _unlockables.Add(AURORA_VINEYARD_TABLET_AP_NAME, SendJunimoNote);
            _unlockables.Add(MORGAN_SCHOOLING, SendAcceptanceLetter);
        }

        private void RegisterPlayerWarps()
        {
            _unlockables.Add(GUILD_RUNES_AP_NAME, SendGuildRunes);
            _unlockables.Add(SPRITE_RUNES_AP_NAME, SendSpriteSpringRunes);
            _unlockables.Add(JUNIMO_RUNES_AP_NAME, SendJunimoOutpostRunes);
            _unlockables.Add(AURORA_RUNES_AP_NAME, SendAuroraRunes);
            _unlockables.Add(FARM_RUNES_AP_NAME, SendFarmWizardRunes);
            _unlockables.Add(FABLE_REEF_AP_NAME, SendLancePortal);
        }

        private void RegisterSVEUniqueItems()
        {
            _unlockables.Add(DIAMOND_WAND_AP_NAME, SendDiamondWandLetter);
        }

        private LetterVanillaAttachment SendMarlonBoat(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "marlonsBoat", true);
        }

        private LetterVanillaAttachment SendVoidSoul(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "GaveVoidSouls", true);
        }

        private LetterVanillaAttachment SendIridiumBomb(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "RailroadBoulderRemoved", true);
        }

        private LetterEventSeenAttachment SendSpriteSpringRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, SPRITE_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendJunimoOutpostRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, JUNIMO_RUNES_EVENT, OUTPOST_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        /*private LetterEventSeenAttachment SendWizardRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, WIZARD_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }*/

        /*private LetterEventSeenAttachment SendOutpostRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, OUTPOST_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }*/

        private LetterEventSeenAttachment SendGuildRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, GUILD_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendAuroraRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, AURORA_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendFarmWizardRunes(ReceivedItem receivedItem)
        {
            var events = new List<int> {MAIN_NEXUS_EVENT, FARM_RUNES_EVENT, WIZARD_RUNES_EVENT};
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendLancePortal(ReceivedItem receivedItem)
        {
            var events = LANCE_WARP_EVENT;
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendKittyfishSpell(ReceivedItem receivedItem)
        {
            var events = KITTYFISH_EVENT;
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterActionAttachment SendDiamondWandLetter(ReceivedItem receivedItem)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.DiamondWand);
        }

        private LetterEventSeenAttachment SendScarlettJobOffer(ReceivedItem receivedItem)
        {
            var events = SCARLETT_EVENT;
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterVanillaAttachment SendJunimoNote(ReceivedItem receivedItem)
        {
            var vineyard = "apAuroraVineyard";
            return new LetterVanillaAttachment(receivedItem, vineyard, true);
        }

        private LetterVanillaAttachment SendAcceptanceLetter(ReceivedItem receivedItem)
        {
            var school = "apMorganSchooling";
            return new LetterVanillaAttachment(receivedItem, school, true);
        }
    }
}