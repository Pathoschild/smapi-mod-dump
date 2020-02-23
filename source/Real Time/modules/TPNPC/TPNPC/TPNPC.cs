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