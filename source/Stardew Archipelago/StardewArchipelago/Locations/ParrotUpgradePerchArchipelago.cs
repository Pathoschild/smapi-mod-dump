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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewValley;
using StardewValley.BellsAndWhistles;
using xTile.Dimensions;

namespace StardewArchipelago.Locations
{
    internal class ParrotUpgradePerchArchipelago : ParrotUpgradePerch
    {
        private string _scoutedItemName;
        public string ApLocationName { get; }

        public ParrotUpgradePerchArchipelago(string apLocationName, ArchipelagoClient archipelago,
            GameLocation location, Point tile_position, Microsoft.Xna.Framework.Rectangle upgrade_rectangle,
            int required_nuts, Action apply_upgrade, Func<bool> update_completion_status, string upgrade_name = "", string required_mail = "")
            : base(location, tile_position, upgrade_rectangle, required_nuts, apply_upgrade, update_completion_status, upgrade_name, required_mail)
        {
            ApLocationName = apLocationName;
            var scoutedLocation = archipelago.ScoutSingleLocation(ApLocationName);
            _scoutedItemName = scoutedLocation == null ? ScoutedLocation.GenericItemName() : scoutedLocation.GetItemName();
        }

        public bool CheckActionArchipelago(Location tileLocation)
        {
            if (!IsAtTile(tileLocation.X, tileLocation.Y) || !IsAvailable())
            {
                return false;
            }

            var upgradeMessageFormat = Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:UpgradePerch_" + upgradeName.Value);
            var gameLocation = locationRef.Value;
            if (upgradeMessageFormat != null && gameLocation != null)
            {
                var scoutedMessageFormat = ReplaceUnlockWithScoutedItem(upgradeMessageFormat);
                var upgradeMessageFormatted = string.Format(scoutedMessageFormat, requiredNuts.Value);
                costShakeTime = 0.5f;
                squawkTime = 0.5f;
                shakeTime = 0.5f;
                if (locationRef.Value == Game1.currentLocation)
                {
                    Game1.playSound("parrot_squawk");
                }

                if (Game1.netWorldState.Value.GoldenWalnuts.Value >= requiredNuts.Value)
                {
                    gameLocation.createQuestionDialogue(upgradeMessageFormatted, gameLocation.createYesNoResponses(), "UpgradePerch_" + upgradeName.Value);
                }
                else
                {
                    Game1.drawDialogueNoTyping(upgradeMessageFormatted);
                }
            }
            else if (Game1.netWorldState.Value.GoldenWalnuts.Value >= requiredNuts.Value)
            {
                AttemptConstruction();
            }
            else
            {
                ShowInsufficientNuts();
            }

            return true;
        }

        private string ReplaceUnlockWithScoutedItem(string upgradeMessageFormat)
        {
            const string ellipsis = "... ";
            var lastEllipsis = upgradeMessageFormat.LastIndexOf(ellipsis, StringComparison.InvariantCultureIgnoreCase) + ellipsis.Length;
            var messageWithoutUnlock = upgradeMessageFormat[..lastEllipsis];
            var messageWithScout = $"{messageWithoutUnlock}{_scoutedItemName}!";
            return messageWithScout;
        }
    }
}
