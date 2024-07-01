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
using StardewValley;

namespace StardewArchipelago.Items.Mail
{
    public class MailKey
    {
        private const string AP_PREFIX = "AP";
        private const string AP_DELIMITER = "|";
        private static Random _random = new Random((int)Game1.uniqueIDForThisGame);
        private string ItemName { get; set; }
        private string PlayerName { get; set; }
        private string LocationName { get; set; }
        public string LetterOpenedAction { get; set; }
        public string ActionParameter { get; set; }
        private string UniqueId { get; set; }
        public bool IsEmpty { get; set; }

        public MailKey(string itemName, string playerName, string locationName, string uniqueId, bool isEmpty)
            : this(itemName, playerName, locationName, "", "", uniqueId, isEmpty)
        {
        }

        public MailKey(string itemName, string playerName, string locationName, string letterOpenedAction, string actionParameter, string uniqueId, bool isEmpty)
        {
            ItemName = Sanitize(itemName);
            PlayerName = Sanitize(playerName);
            LocationName = Sanitize(locationName);
            LetterOpenedAction = Sanitize(letterOpenedAction);
            ActionParameter = Sanitize(actionParameter);
            UniqueId = Sanitize(uniqueId);
            IsEmpty = isEmpty;
        }

        private static string Sanitize(string input)
        {
            return input.Replace(AP_DELIMITER, "");
        }

        public override string ToString()
        {
            var key = $"{AP_PREFIX}{AP_DELIMITER}{ItemName}{AP_DELIMITER}{PlayerName}{AP_DELIMITER}{LocationName}{AP_DELIMITER}{LetterOpenedAction}{AP_DELIMITER}{ActionParameter}{AP_DELIMITER}{UniqueId}{AP_DELIMITER}{IsEmpty}";
            var trimmedKey = key.Replace(" ", "_");
            return trimmedKey;
        }

        public static bool TryParse(string key, out MailKey mailKey)
        {
            mailKey = null;
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (!key.StartsWith(AP_PREFIX))
            {
                return false;
            }

            var splitKey = key.Split(AP_DELIMITER);
            if (splitKey.Length < 7)
            {
                return false;
            }

            var itemName = splitKey[1];
            var playerName = splitKey[2];
            var locationName = splitKey[3];
            var letterOpenedAction = splitKey[4];
            var actionParameter = splitKey[5];
            var uniqueId = splitKey[6];
            if (splitKey.Length <= 7 || !bool.TryParse(splitKey[7], out var isEmpty))
            {
                isEmpty = false;
            }

            mailKey = new MailKey(itemName, playerName, locationName, letterOpenedAction, actionParameter, uniqueId, isEmpty);
            return true;
        }

        public static string GetBeginningOfKeyForItem(string itemName)
        {
            return $"{AP_PREFIX}{AP_DELIMITER}{Sanitize(itemName)}".Replace(" ", "_");
        }
    }
}
