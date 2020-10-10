/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using NPCTPHere;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Teleport;

namespace TPNPC
{
    public class TPNPC : Mod
    {
        
        private Config Config;
        
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();
            helper.Events.Input.ButtonPressed += onButton;
        }


        private void onButton(object sneder, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Context.IsPlayerFree)
                return;
            if (e.Button != Config.npcTPHere)
                return;
            Game1.activeClickableMenu = new TPNPCMenu(Helper);
        }
        
        
    }
}