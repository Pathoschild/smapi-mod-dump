/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewValley;

namespace StardewArchipelago.GameModifications.Buildings
{
    public class FreeBlueprint : BluePrint
    {
        public FreeBlueprint(string name, string sendingPlayerName) : base(name)
        {
            itemsRequired.Clear();
            moneyRequired = 0;
            displayName = $"Free {displayName}";
            description = $"A gift from {sendingPlayerName}. {description}";
        }

        public void SetDisplayFields(string displayName, string description, string sendingPlayerName)
        {
            this.displayName = $"Free {displayName}";
            this.description = $"A gift from {sendingPlayerName}. {description}";
        }
    }
}
