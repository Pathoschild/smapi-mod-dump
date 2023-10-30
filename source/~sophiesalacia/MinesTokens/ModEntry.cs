/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Reflection;
using ContentPatcher;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MinesTokens
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private IContentPatcherAPI Api;
        private PropertyInfo IsQuarryArea;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                Api = helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

                IsQuarryArea = typeof(MineShaft).GetProperty("isQuarryArea", BindingFlags.NonPublic | BindingFlags.Instance);

                if (Api is null)
                {
                    Monitor.Log("Failed to register Content Patcher API, aborting token registration.", LogLevel.Error);
                }
                else
                {
                    RegisterMinesToken();
                    RegisterQuarryToken();
                    RegisterSkullCavernToken();
                    RegisterMineLevelToken();
                    RegisterHardModeToken();
                }
            };
        }

        private void RegisterMinesToken()
        {
            Api.RegisterToken(ModManifest, "IsMines", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return ms.mineLevel < 121 ? new[] { true.ToString() } : new[] { false.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return null;
            });

            Monitor.Log("Registered sophie.MinesTokens/IsMines successfully.", LogLevel.Info);
        }

        private void RegisterQuarryToken()
        {
            Api.RegisterToken(ModManifest, "IsQuarry", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    bool? quarryArea = (bool?)IsQuarryArea.GetValue(ms);

                    return quarryArea is null ? null : new[] { quarryArea.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return null;
            });

            Monitor.Log("Registered sophie.MinesTokens/IsQuarry successfully.", LogLevel.Info);
        }

        private void RegisterSkullCavernToken()
        {
            Api.RegisterToken(ModManifest, "IsSkullCavern", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return ms.mineLevel >= 121 ? new[] { true.ToString() } : new[] { false.ToString() };
                }

                // save is loaded but we are not in a mineshaft
                if (Context.IsWorldReady)
                {
                    return new[] { false.ToString() };
                }

                // no save loaded (e.g. on the title screen)
                return null;
            });

            Monitor.Log("Registered sophie.MinesTokens/IsSkullCavern successfully.", LogLevel.Info);
        }

        private void RegisterMineLevelToken()
        {
            Api.RegisterToken(ModManifest, "MineLevel", () =>
            {
                // save is loaded and we are in a mineshaft
                if (Context.IsWorldReady && Game1.currentLocation is MineShaft ms)
                {
                    return new[] { ms.mineLevel.ToString() };
                }

                // no save loaded (e.g. on the title screen) or we are not in a mineshaft
                return null;
            });

            Monitor.Log("Registered sophie.MinesTokens/MineLevel successfully.", LogLevel.Info);
        }

        private void RegisterHardModeToken()
        {
            Api.RegisterToken(ModManifest, "IsHardMode", () =>
            {

                // no save loaded (e.g. on the title screen)
                if (!Context.IsWorldReady)
                    return null;

                // save is loaded
                return new[] { (Game1.netWorldState.Value.MinesDifficulty > 0).ToString() };
            });

            Monitor.Log("Registered sophie.MinesTokens/IsHardMode successfully.", LogLevel.Info);
        }
    }
}
