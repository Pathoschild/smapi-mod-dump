using MoreMultiplayerInfo.Helpers;
using StardewModdingAPI;

namespace MoreMultiplayerInfo
{
    public class ModEntry : Mod
    {
        private ModEntryHelper _baseHandler;

        public override void Entry(IModHelper helper)
        {
            ConfigHelper.Helper = helper;

            _baseHandler = new ModEntryHelper(Monitor, helper);
        }
    }
}
