/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Messages;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace FashionSense.Framework.Managers
{
    internal class ColorManager
    {
        private IMonitor _monitor;
        private Dictionary<Farmer, Dictionary<string, Color>> _farmerToColorIdToColorValue;

        public ColorManager(IMonitor monitor)
        {
            _monitor = monitor;
            _farmerToColorIdToColorValue = new Dictionary<Farmer, Dictionary<string, Color>>();
        }

        internal void BroadcastColorChange(Farmer who, string colorKey, Color colorValue)
        {
            var colorChangeMessage = new ColorChangeMessage(who.UniqueMultiplayerID, colorKey, colorValue);
            FashionSense.modHelper.Multiplayer.SendMessage(colorChangeMessage, "ColorChangeMessage", modIDs: new[] { FashionSense.modManifest.UniqueID });
        }

        internal Color GetColor(Farmer who, string colorKey)
        {
            Color colorValue = who.hairstyleColor.Value;
            if (_farmerToColorIdToColorValue.ContainsKey(who) is true && _farmerToColorIdToColorValue[who].ContainsKey(colorKey) is true)
            {
                colorValue = _farmerToColorIdToColorValue[who][colorKey];
            }
            else if (who.modData.ContainsKey(colorKey))
            {
                SetColor(who, colorKey, who.modData[colorKey]);
                return GetColor(who, colorKey);
            }

            return colorValue;
        }

        internal void SetColor(Farmer who, string colorKey, Color colorValue)
        {
            if (_farmerToColorIdToColorValue.ContainsKey(who) is false)
            {
                _farmerToColorIdToColorValue[who] = new Dictionary<string, Color>();
            }

            who.modData[colorKey] = colorValue.PackedValue.ToString();
            _farmerToColorIdToColorValue[who][colorKey] = colorValue;

            // Send out color change message to all other players
            if (who.IsLocalPlayer)
            {
                BroadcastColorChange(who, colorKey, colorValue);
            }
        }

        internal void SetColor(Farmer who, string colorKey, string rawColorValue)
        {
            Color actualColor = who.hairstyleColor.Value;
            if (uint.TryParse(rawColorValue, out var parsedColorValue) is true)
            {
                actualColor = new Color() { PackedValue = parsedColorValue };
            }

            SetColor(who, colorKey, actualColor);
        }

        public void CopyColors(Farmer sourceFarmer, Farmer destinationFarmer)
        {
            if (_farmerToColorIdToColorValue.ContainsKey(sourceFarmer) is false)
            {
                _farmerToColorIdToColorValue[sourceFarmer] = new Dictionary<string, Color>();
            }

            foreach (var keyToColor in _farmerToColorIdToColorValue[sourceFarmer])
            {
                SetColor(destinationFarmer, keyToColor.Key, keyToColor.Value);
            }
        }
    }
}
