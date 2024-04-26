/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace Fishnets.Automate
{
    internal class ModEntry : Mod
    {
        internal const string FishNetsId = "MindMeltMax.Fishnets";
        internal const string AutomateId = "Pathoschild.Automate";

        internal static string FishnetId;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                var fishNetsApi = Helper.ModRegistry.GetApi<IFishnetApi>(FishNetsId);
                if (fishNetsApi is null)
                {
                    Monitor.Log($"Unable to load api for Fish Nets, mod will not work", LogLevel.Error);
                    return;
                }
                var automatApi = Helper.ModRegistry.GetApi<IAutomateApi>(AutomateId);
                if (automatApi is null)
                {
                    Monitor.Log($"Unable to load api for Automate, mod will not work", LogLevel.Error);
                    return;
                }

                FishnetId = fishNetsApi.GetId();
                automatApi.AddFactory(new FishNetFactory());
                Monitor.Log($"Successfully connected Fish Nets with Automate");
            };
        }
    }
}
