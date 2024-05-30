/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewValley;

namespace BarleyCore
{
    public interface IBarleyCoreApi
    {
        public void ModData_SetTarget(IHaveModData target);
        public TModel? ModData_Read<TModel>(string key, bool local = true) where TModel : class;
        public void ModData_Write<TModel>(string key, TModel value, bool local = true) where TModel: class;
    }
}
