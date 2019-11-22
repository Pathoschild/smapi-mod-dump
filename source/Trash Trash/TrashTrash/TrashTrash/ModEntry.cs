using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TrashTrash
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private Config _config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<Config>();
            InputEvents.ButtonPressed += PressButton;
        }


        private void PressButton(object sender, EventArgsInput e)
        {
            if (e.Button.ToString() != _config.TrashKey) return;
            var removeList = new List<int>();

            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                var item = Game1.player.Items[i];
                if (item != null && item.salePrice() == 0 && item.canBeTrashed() && item.Category == -20)
                {
                    removeList.Add(i);
                }
            }

            removeList.ForEach(i => Utility.removeItemFromInventory(i, Game1.player.items));
        }
    }
}