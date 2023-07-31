/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared.APIs;
using StardewValley.Menus;
using HarmonyLib;


namespace NovoMundo
{
    public class ModEntry : Mod
    {

        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal static Harmony harmony;
        private Menu_Patches Menu_Patches = new();
        private Tile_Framework Tile_Framework = new();
        

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            harmony = new(helper.ModRegistry.ModID);
            helper.Events.GameLoop.GameLaunched += (s, e) => Menu_Patches.Apply_Harmony(harmony);
            helper.Events.GameLoop.UpdateTicking += Tile_Framework.OnGameLoopUpdateTicking;
            helper.Events.Display.MenuChanged += Tile_Framework.OnDisplayMenuChanged;
            helper.Events.Input.ButtonPressed += Tile_Framework.OnButtonPressed;




        }


    }
}
