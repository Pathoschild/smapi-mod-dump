using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGeodes
{
    public static class Extensions
    {
        public static int HasAdornment(this StardewValley.Farmer player, ToolType tool, int adornment)
        {
            var data = Mod.Data;
            if (player != Game1.player)
                data = Mod.instance.Helper.Data.ReadSaveData<SaveData>("spacechase0.ToolGeodes." + player.UniqueMultiplayerID) ?? new SaveData();

            int[] ids = null;
            if (tool == ToolType.Weapon) ids = Mod.Data.WeaponGeodes;
            if (tool == ToolType.Pickaxe) ids = Mod.Data.PickaxeGeodes;
            if (tool == ToolType.Axe) ids = Mod.Data.AxeGeodes;
            if (tool == ToolType.WateringCan) ids = Mod.Data.WaterCanGeodes;
            if (tool == ToolType.Hoe) ids = Mod.Data.HoeGeodes;

            int ret = 0;
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (id == adornment)
                        ++ret;
                }
            }

            return ret;
        }
    }
}
