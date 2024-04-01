/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SpennyLite
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public int startingSpin = -1;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Display.MenuChanged += this.Display_MenuChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu == null && e.NewMenu is DialogueBox db && db.characterDialogue?.speaker?.Name == "Penny")
            {
                startingSpin = db.characterDialogue.speaker.facingDirection.Value;
                db.characterDialogue.speaker.jump();
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(8) && startingSpin != -1)
            {
                NPC penny = Game1.getCharacterFromName("Penny");
                if (penny == null)
                    return;

                penny.faceDirection((penny.FacingDirection + 1) % 4);

                if (penny.FacingDirection == startingSpin)
                    startingSpin = -1;
            }
        }
    }
}
