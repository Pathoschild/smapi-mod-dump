/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;
using StardewValley;
using NovoMundo.Managers;
using NovoMundo.Farm1;
using NovoMundo.Farm2;
using NovoMundo.Patcher;
using NovoMundo.NPCs;

namespace NovoMundo
{
    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static Harmony harmony;
        private Patching patching = new();
        private Set_Farm1 framework1;
        private Set_Farm2 framework2;
        private readonly Asset_Editor asset_Editor = new();
        private readonly Property_Manager property_Manager = new(); 
        private readonly Schedules_Editor schedules_Editor = new();
        private readonly Call_Builders call_Builders = new();
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            harmony = new(helper.ModRegistry.ModID);
            framework1 = new Set_Farm1(helper);
            framework2 = new Set_Farm2(helper);
            helper.Events.GameLoop.GameLaunched += (s, e) => patching.Apply_Harmony(harmony);
            helper.Events.Content.AssetRequested += asset_Editor.AssetRequested;
            helper.Events.GameLoop.DayStarted += schedules_Editor.OnDayStarted;
            
            helper.Events.GameLoop.DayStarted += call_Builders.OnDayStarted;

            helper.Events.GameLoop.GameLaunched += (s, e) => framework1.Apply_Harmony(harmony);
            helper.Events.Display.MenuChanged += framework1.OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += framework1.OnSaveLoaded;
            helper.Events.GameLoop.Saving += framework1.OnSaving;
            helper.Events.GameLoop.Saved += framework1.OnSaved;
            helper.Events.GameLoop.ReturnedToTitle += framework1.OnReturnedToTitle;
            helper.Events.GameLoop.DayStarted += framework1.OnDayStarted;
            helper.Events.Display.MenuChanged += framework2.OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += framework2.OnSaveLoaded;
            helper.Events.GameLoop.Saving += framework2.OnSaving;
            helper.Events.GameLoop.Saved += framework2.OnSaved;
            helper.Events.GameLoop.ReturnedToTitle += framework2.OnReturnedToTitle;
            helper.Events.GameLoop.DayStarted += framework2.OnDayStarted;



            helper.Events.Input.ButtonPressed += property_Manager.OnButtonPressed;
            helper.Events.Display.MenuChanged += property_Manager.OnDisplayMenuChanged;
            helper.Events.GameLoop.UpdateTicking += property_Manager.OnGameLoop;


        }
    }
}
