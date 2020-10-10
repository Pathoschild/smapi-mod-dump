/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewLib;

namespace AnimalSitter.Framework
{
    internal class ModConfig : IConfig
    {
        public string KeyBind { get; set; } = "O";
        public bool GrowUpEnabled { get; set; }
        public bool MaxHappinessEnabled { get; set; }
        public bool MaxFullnessEnabled { get; set; }
        public bool HarvestEnabled { get; set; } = true;
        public bool PettingEnabled { get; set; } = true;
        public bool MaxFriendshipEnabled { get; set; }
        public int CostPerAction { get; set; } = 25;
        public string WhoChecks { get; set; } = "spouse";
        public bool EnableMessages { get; set; } = true;
        public bool TakeTrufflesFromPigs { get; set; } = true;
        public bool BypassInventory { get; set; }
        public Vector2 ChestCoords { get; set; } = new Vector2(73, 14);
        public string ChestDefs { get; set; } = "";
    }
}
