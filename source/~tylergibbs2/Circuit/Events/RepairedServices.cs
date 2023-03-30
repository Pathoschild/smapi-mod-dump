/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewValley;

namespace Circuit.Events
{
    internal class RepairedServices : EventBase
    {
        private readonly List<string> MailFlagsToUnlock = new()
        {
            "communityUpgradeShortcuts",
            "OpenedSewer",
            "ccBoilerRoom",
            "ccCraftsRoom",
            "ccVault",
            "willyBoatTicketMachine",
            "willyBoatFixed",
            "willyBackRoomInvitation"
        };

        internal bool AlreadyHasClubCard { get; set; } = false;

        internal bool AlreadyHasRustyKey { get; set; } = false;

        internal bool AlreadyHasSkullKey { get; set; } = false;

        internal bool AlreadyUnlockedSkullDoor { get; set; } = false;

        public RepairedServices(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Repaired Services";
        }

        public override string GetChatWarningMessage()
        {
            return "There are talks of getting some construction done around town";
        }

        public override string GetChatStartMessage()
        {
            return "The construction is done! The town is looking great!";
        }

        public override string GetDescription()
        {
            return "All inaccessible areas are temporarily unlocked.";
        }

        public override void StartEvent()
        {
            foreach (string flag in MailFlagsToUnlock)
                Game1.player.mailReceived.Add(flag);

            AlreadyHasClubCard = Game1.player.hasClubCard;
            AlreadyHasRustyKey = Game1.player.hasRustyKey;
            AlreadyHasSkullKey = Game1.player.hasSkullKey;
            AlreadyUnlockedSkullDoor = Game1.player.hasUnlockedSkullDoor;

            Game1.player.hasSkullKey = true;
            Game1.player.hasUnlockedSkullDoor = true;

            Game1.player.mailReceived.OnElementChanged += OnMailReceivedChanged;
        }

        private void OnMailReceivedChanged(NetList<string, NetString> list, int index, string? oldValue, string? newValue)
        {
            if (oldValue is not null || newValue is null)
                return;

            switch (newValue)
            {
                case "TH_LumberPile":
                    AlreadyHasClubCard = true;
                    break;
            }
        }

        public override void EndEvent()
        {
            Game1.player.mailReceived.OnElementChanged -= OnMailReceivedChanged;

            foreach (string flag in MailFlagsToUnlock)
                Game1.MasterPlayer.mailReceived.Remove(flag);

            if (!AlreadyHasClubCard)
                Game1.player.hasClubCard = false;
            if (!AlreadyHasRustyKey)
                Game1.player.hasRustyKey = false;
            if (!AlreadyHasSkullKey)
                Game1.player.hasSkullKey = false;
            if (!AlreadyUnlockedSkullDoor)
                Game1.player.hasUnlockedSkullDoor = false;

            RemoveMapModifications();
        }

        private void RemoveMapModifications()
        {
            // TODO: needs fixing. doesnt remove map changes if player sleeps. warp removal is buggy

            if (!Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
            {
                // Remove Quarry bridge
                Game1.getLocationFromName("Mountain").reloadMap();
            }

            if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
            {
                // Remove town shortcuts
                GameLocation beach = Game1.getLocationFromName("Beach");
                beach.reloadMap();
                beach.warps.Filter(warp => warp.X != -1);

                Game1.getLocationFromName("Backwoods").reloadMap();

                Game1.getLocationFromName("Town").reloadMap();

                GameLocation forest = Game1.getLocationFromName("Forest");
                forest.reloadMap();
                forest.warps.Filter(warp =>
                {
                    return warp.X != 120 && (warp.Y != 35 && warp.Y != 36);
                });

                GameLocation nightMarket = Game1.getLocationFromName("BeachNightMarket");
                nightMarket.reloadMap();
                nightMarket.warps.Filter(warp => warp.X != -1);
            }
        }
    }
}
