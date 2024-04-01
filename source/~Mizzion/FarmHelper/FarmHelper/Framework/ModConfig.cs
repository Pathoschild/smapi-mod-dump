/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace FarmHelper.Framework
{
    internal class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string ActivationKey { get; set; } = "Q";
        public string UseToolKey { get; set; } = "Z";
        public string ClearLocationKey { get; set; } = "R";
        public string GatherForageKey { get; set; } = "X";
        public string SingleUseKey { get; set; } = "V";
        public bool AutomaticMode { get; set; } = true;
        public bool EnablePetting { get; set; } = true;
        public bool EnableNotification { get; set; } = true;
        public bool EnableCost { get; set; } = true;
        public int HelperCost { get; set; } = 50;
        public bool AddItemsToInventory { get; set; } = true;
        public bool HarvestAnimalProducts { get; set; } = true;
        public Vector2 ChestLocation { get; set; } = new Vector2(68, 12);
    }
}
