/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdy/SDVModding
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;

namespace MinecartPatcher
{
    internal class DialogueBox : StardewValley.Menus.DialogueBox
    {
        private ModEntry Mod;
		private MethodInfo tryOutro;
		private MinecartInstance activeCart;
        public DialogueBox(MinecartInstance mc, ModEntry mod, List<Response> responses) : base(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), responses)
        {
			activeCart = mc;
            Mod = mod;
			tryOutro = typeof(StardewValley.Menus.DialogueBox).GetMethod("tryOutro", BindingFlags.NonPublic | BindingFlags.Instance);
        }

		public override void receiveKeyPress(Keys key)
		{
			if (transitioning)
			{
				return;
			}
			if (!Game1.options.gamepadControls && Game1.options.doesInputListContain(Game1.options.actionButton, key))
			{
				receiveLeftClick(0, 0);
			}
			else if (!Game1.eventUp)
			{
				if (responses != null)
				{
					foreach (Response response2 in responses)
					{
						if (response2.hotkey == key && Mod.onDialogueSelect(activeCart, response2.responseKey))
						{
							Game1.playSound("smallSelect");
							selectedResponse = -1;
							tryOutro.Invoke(this, null);
							return;
						}
					}
				}
				if (Game1.options.doesInputListContain(Game1.options.menuButton, key) || key == Keys.N)
				{
					if (responses != null && responses.Count > 0 && Mod.onDialogueSelect(activeCart, responses[responses.Count - 1].responseKey))
					{
						Game1.playSound("smallSelect");
					}
					selectedResponse = -1;
					tryOutro.Invoke(this, null);
				}
				else if (Game1.options.SnappyMenus)
				{
					safetyTimer = 0;
					base.receiveKeyPress(key);
				}
			}
			else if (Game1.options.SnappyMenus && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
			{
				base.receiveKeyPress(key);
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (transitioning)
			{
				return;
			}
			if (characterIndexInDialogue < getCurrentString().Length - 1)
			{
				characterIndexInDialogue = getCurrentString().Length - 1;
			}
			else
			{
				if (safetyTimer > 0)
				{
					return;
				}
				if (selectedResponse == -1)
				{
					return;
				}
				questionFinishPauseTimer = (Game1.eventUp ? 600 : 200);
				transitioning = true;
				transitionInitialized = false;
				transitioningBigger = true;
				Game1.dialogueUp = false;
				if (Mod.onDialogueSelect(activeCart, responses[selectedResponse].responseKey))
				{
					Game1.playSound("smallSelect");
				}
				selectedResponse = -1;
				tryOutro.Invoke(this, null);
				return;
			}
		}
	}
}
