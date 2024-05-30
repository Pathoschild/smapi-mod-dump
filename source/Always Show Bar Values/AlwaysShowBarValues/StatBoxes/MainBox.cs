/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using AlwaysShowBarValues.UIElements;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AlwaysShowBarValues.StatBoxes
{
    internal class MainBox : StatBox
    {
        public MainBox() : base(KeybindList.Parse("H"), "main") {
            this.TopValue = new PlayerStat("Health", new Microsoft.Xna.Framework.Rectangle(16, 411, 16, 16), new Microsoft.Xna.Framework.Vector2(0, 0));
            this.BottomValue = new PlayerStat("Stamina", new Microsoft.Xna.Framework.Rectangle(0, 411, 16, 16), new Microsoft.Xna.Framework.Vector2(0, 0));
            IsValid = true;
        }

        public override bool UpdateCurrentStats()
        {
            TopValue.CurrentValue = (float)Game1.player.health;
            TopValue.MaxValue = (float)Game1.player.maxHealth;
            BottomValue.CurrentValue = (float)Game1.player.Stamina;
            BottomValue.MaxValue = (float)Game1.player.MaxStamina;
            return true;
        }
    }
}
