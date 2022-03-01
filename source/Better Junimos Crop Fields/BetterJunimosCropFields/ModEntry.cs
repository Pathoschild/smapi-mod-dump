/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/micfort/betterjunimoscropfields
**
*************************************************/

using BetterJunimos;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterJunimosCropFields
{
    public class ModEntry: Mod
    {
        private CropFieldsAbility? _cropFieldsAbility;

        public override void Entry(IModHelper helper)
        {
            _cropFieldsAbility = new CropFieldsAbility(Monitor);
            helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.GameLoop.UpdateTicking += _cropFieldsAbility.UpdateTick;
        }
        
        private void OnLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.GetApi("hawkfalcon.BetterJunimos") is not BetterJunimosApi bjApi)
            {
                Monitor.Log($"Better Junimos is not installed", LogLevel.Error);
                return;
            }
            bjApi.RegisterJunimoAbility(_cropFieldsAbility);
        }
    }
}