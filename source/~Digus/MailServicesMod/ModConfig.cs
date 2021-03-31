/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailServicesMod
{
    public class ModConfig : RecoveryConfig
    {
        public bool DisableToolDeliveryService;
        public bool DisableToolShipmentService;
        public int ToolShipmentServiceFee;
        public bool DisableQuestService;
        public int QuestServiceFee;
        public bool DisableGiftService;
        public int GiftServiceFee;
        public int GiftChoicePageSize = 5;
        public bool EnableJealousyFromMailedGifts;
        public bool EnableGiftToNpcWithMaxFriendship;
        public bool ShowDialogOnItemDelivery;
        public bool DisablePerPlayerConfig;
        public Dictionary<long, PlayerRecoveryConfig> PlayerRecoveryConfig = new Dictionary<long, PlayerRecoveryConfig>();
    }

    public class RecoveryConfig
    {
        public bool DisableRecoveryConfigInGameChanges;
        public bool EnableRecoveryService;
        public bool RecoverAllItems;
        public bool RecoverForFree;
        public bool DisableClearLostItemsOnRandomRecovery;
    }

    public class PlayerRecoveryConfig : RecoveryConfig
    {
        public string PlayerName;
    }
}
