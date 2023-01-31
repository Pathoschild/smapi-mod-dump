/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Integration;
using AeroCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Runtime.CompilerServices;
using xTile;

namespace AeroCore
{
    public class ModEntry : Mod
    {
        internal static ITranslationHelper i18n;
        internal static IMonitor monitor;
        internal static IModHelper helper;
        internal static Harmony harmony;
        internal static string ModID;
        internal static API.API api;
        internal static IDGAAPI DGA;
        internal static IJsonAssetsAPI JA;
        internal static Config Config;

        private IReflectedField<Multiplayer> mp;

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Hello and welcome to the Enrichment Center!", LogLevel.Debug);

            monitor = Monitor;
            ModEntry.helper = Helper;
            harmony = new(ModManifest.UniqueID);
            ModID = ModManifest.UniqueID;
            api = new();
            i18n = helper.Translation;
            Config = helper.ReadConfig<Config>();

            mp = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");

            helper.Events.GameLoop.SaveLoaded += EnteredWorld;
            helper.Events.GameLoop.GameLaunched += Init;
            helper.Events.Content.AssetRequested += LoadAssets;

            helper.ConsoleCommands.Add("reload_location", "Force-reloads the current location", (s, a) => Utils.Maps.ReloadCurrentLocation(true));
            helper.ConsoleCommands.Add("unstuck_placed", "Force-breaks placed items in a location. If no location is specified, uses the current location.", 
                (s, a) => ItemWrapper.DropAllWrapped(a.Length > 0 ? Game1.getLocationFromName(a[0]) : null)
            );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Init(object _, GameLaunchedEventArgs ev)
        {
            if (helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
                DGA = helper.ModRegistry.GetApi<IDGAAPI>("spacechase0.DynamicGameAssets");
            if (helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                JA = helper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");
            api.RegisterGMCMConfig(ModManifest, Helper, Config, ConfigUpdated);
            api.InitAll();
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        private void EnteredWorld(object _, SaveLoadedEventArgs ev)
        {
            Utils.Reflection.mp = mp.GetValue();
        }
        public override object GetApi() => api;
        public static API.API GetStaticApi() => api;
        private static void ConfigUpdated()
        {
            if (Config.CursorLightHold)
                User.isLightActive = false;
        }
        private static void LoadAssets(object _, AssetRequestedEventArgs ev)
        {
            if (ev.NameWithoutLocale.IsEquivalentTo("Maps/EventVoid"))
                ev.LoadFromModFile<Map>("assets/eventvoid.tbin", AssetLoadPriority.High);
        }
    }
}
