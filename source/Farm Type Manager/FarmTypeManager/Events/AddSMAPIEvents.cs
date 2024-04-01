/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Add this mod's event methods to SMAPI's event handlers. This should be performed once during mod entry.</summary>
        private void AddSMAPIEvents(IModHelper helper)
        {
            //spawn-related events
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.Saved += GameLoop_Saved;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
            helper.Events.GameLoop.TimeChanged += TimeChanged;

            //custom object/monster handler events
            helper.Events.Multiplayer.ModMessageReceived += ModMessageReceived;
            helper.Events.World.NpcListChanged += NpcListChanged;

            //mod compatiblity events
            helper.Events.GameLoop.GameLaunched += EnableGMCM;
            helper.Events.GameLoop.GameLaunched += EnableSaveAnywhere;
            helper.Events.GameLoop.GameLaunched += EnableEPU;
            helper.Events.GameLoop.GameLaunched += EnableDGA;
            helper.Events.GameLoop.GameLaunched += EnableContentPatcher;
            helper.Events.GameLoop.GameLaunched += EnableMTF;
            helper.Events.GameLoop.GameLaunched += EnableItemExtensions;
        }
    }
}
