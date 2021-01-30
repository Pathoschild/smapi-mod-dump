/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredlll08/Trash-Trash
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TrashTrash
{
    public class ModEntry : Mod
    {
        private Config _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<Config>();
            helper.Events.Input.ButtonPressed += InputOnButtonPressed;
        }

        private void InputOnButtonPressed(object sender, ButtonPressedEventArgs e)
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