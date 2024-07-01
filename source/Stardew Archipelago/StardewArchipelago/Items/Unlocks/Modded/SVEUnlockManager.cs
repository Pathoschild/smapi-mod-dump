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

namespace StardewArchipelago.Items.Unlocks.Modded
{
    public class SVEUnlockManager : IUnlockManager
    {
        public const string DIAMOND_WAND_AP_NAME = "Diamond Wand";
        public const string MARLON_BOAT_PADDLE_AP_NAME = "Marlon's Boat Paddle";
        public const string VOID_SOUL_AP_NAME = "Void Spirit Peace Agreement";
        public const string IRIDIUM_BOMB_AP_NAME = "Iridium Bomb";
        public const string KITTYFISH_SPELL_AP_NAME = "Kittyfish Spell";
        public const string GUILD_RUNES_AP_NAME = "Nexus: Adventurer's Guild Runes";
        public const string SPRITE_RUNES_AP_NAME = "Nexus: Sprite Spring Runes";
        public const string JUNIMO_RUNES_AP_NAME = "Nexus: Junimo Woods Runes";
        public const string OUTPOST_RUNES_AP_NAME = "Nexus: Outpost Runes";
        public const string FARM_RUNES_AP_NAME = "Nexus: Farm Runes";
        public const string WIZARD_RUNES_AP_NAME = "Nexus: Wizard Runes";
        public const string AURORA_RUNES_AP_NAME = "Nexus: Aurora Vineyard Runes";
        public const string FABLE_REEF_AP_NAME = "Fable Reef Portal";
        public const string AURORA_VINEYARD_TABLET_AP_NAME = "Aurora Vineyard Tablet";
        public const string GRANDPA_SHED_AP_NAME = "Grandpa's Shed";
        public const string SCARLETT_JOB_OFFER = "Scarlett's Job Offer";
        public const string MORGAN_SCHOOLING = "Morgan's Schooling";
        public const string MAIN_NEXUS_EVENT = "908071";
        public const string WIZARD_RUNES_EVENT = "908072";
        public const string GUILD_RUNES_EVENT = "908073";
        public const string FARM_RUNES_EVENT = "908074";
        public const string AURORA_RUNES_EVENT = "908075";
        public const string SPRITE_RUNES_EVENT = "908076";
        public const string JUNIMO_RUNES_EVENT = "908077";
        public const string OUTPOST_RUNES_EVENT = "908078";
        public const string LANCE_WARP_EVENT = "65360191";
        public const string KITTYFISH_EVENT = "69069247";
        public const string SCARLETT_EVENT = "3691371";
        public const string AURORA_EVENT = "658059254";
        public const string MORGAN_EVENT = "658078924";

        public SVEUnlockManager()
        {
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            RegisterSVEUniqueItems(unlocks);
            RegisterPlayerWarps(unlocks);
            RegisterSVEPlayerImprovement(unlocks);
            RegisterSVEVillagerInvitations(unlocks);
        }

        private void RegisterSVEPlayerImprovement(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(MARLON_BOAT_PADDLE_AP_NAME, SendMarlonBoat);
            unlocks.Add(VOID_SOUL_AP_NAME, SendVoidSoul);
            unlocks.Add(IRIDIUM_BOMB_AP_NAME, SendIridiumBomb);
            unlocks.Add(KITTYFISH_SPELL_AP_NAME, SendKittyfishSpell);
            unlocks.Add(GRANDPA_SHED_AP_NAME, SendGrandpaShed);
        }

        private void RegisterSVEVillagerInvitations(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(SCARLETT_JOB_OFFER, SendScarlettJobOffer);
            unlocks.Add(AURORA_VINEYARD_TABLET_AP_NAME, SendJunimoNote);
            unlocks.Add(MORGAN_SCHOOLING, SendAcceptanceLetter);
        }

        private void RegisterPlayerWarps(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(GUILD_RUNES_AP_NAME, SendGuildRunes);
            unlocks.Add(SPRITE_RUNES_AP_NAME, SendSpriteSpringRunes);
            unlocks.Add(JUNIMO_RUNES_AP_NAME, SendJunimoRunes);
            unlocks.Add(WIZARD_RUNES_AP_NAME, SendWizardRunes);
            unlocks.Add(OUTPOST_RUNES_AP_NAME, SendOutpostRunes);
            unlocks.Add(AURORA_RUNES_AP_NAME, SendAuroraRunes);
            unlocks.Add(FARM_RUNES_AP_NAME, SendFarmRunes);
            unlocks.Add(FABLE_REEF_AP_NAME, SendLancePortal);
        }

        private void RegisterSVEUniqueItems(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add(DIAMOND_WAND_AP_NAME, SendDiamondWandLetter);
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

        private LetterVanillaAttachment SendGrandpaShed(ReceivedItem receivedItem)
        {
            return new LetterVanillaAttachment(receivedItem, "ShedRepaired", true);
        }

        private LetterEventSeenAttachment SendSpriteSpringRunes(ReceivedItem receivedItem)
        {
            var events = new List<string> { MAIN_NEXUS_EVENT, SPRITE_RUNES_EVENT };
            return new LetterEventSeenAttachment(receivedItem, events);
        }

        private LetterEventSeenAttachment SendJunimoRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, JUNIMO_RUNES_EVENT });
        }

        private LetterEventSeenAttachment SendOutpostRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, OUTPOST_RUNES_EVENT });
        }

        private LetterEventSeenAttachment SendGuildRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, GUILD_RUNES_EVENT });
        }

        private LetterEventSeenAttachment SendAuroraRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, AURORA_RUNES_EVENT });
        }

        private LetterEventSeenAttachment SendFarmRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, FARM_RUNES_EVENT });
        }

        private LetterEventSeenAttachment SendWizardRunes(ReceivedItem receivedItem)
        {
            return new LetterEventSeenAttachment(receivedItem, new[] { MAIN_NEXUS_EVENT, WIZARD_RUNES_EVENT });
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
