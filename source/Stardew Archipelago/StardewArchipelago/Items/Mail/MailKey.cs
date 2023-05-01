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
        private static Random _random = new Random((int)Game1.uniqueIDForThisGame);
        public string ItemName { get; set; }
        public string PlayerName { get; set; }
        public string LocationName { get; set; }
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
            ItemName = itemName;
            PlayerName = playerName;
            LocationName = locationName;
            LetterOpenedAction = letterOpenedAction;
            ActionParameter = actionParameter;
            UniqueId = uniqueId;
            IsEmpty = isEmpty;
        }

        public override string ToString()
        {
            var key = $"{AP_PREFIX}|{ItemName}|{PlayerName}|{LocationName}|{LetterOpenedAction}|{ActionParameter}|{UniqueId}|{IsEmpty}";
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

            var splitKey = key.Split("|");
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
            var isEmpty = splitKey.Length > 7 && bool.Parse(splitKey[7]);

            mailKey = new MailKey(itemName, playerName, locationName, letterOpenedAction, actionParameter, uniqueId, isEmpty);
            return true;
        }

        public static string GetBeginningOfKeyForItem(string itemName)
        {
            return $"{AP_PREFIX}|{itemName}".Replace(" ", "_");
        }
    }
}
