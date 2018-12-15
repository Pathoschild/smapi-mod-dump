using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


/*BREAKDOWN OF MOD:
 *	- Game1.game1.InactiveSleepTime = 0 : no fps throttling when window inactive
 *	
 *	- XNA Keyboard/GamePad/Mouse.GetState is overwritten to pass only one device in
 *	- XNA SetMouse is also overwritten to set a fake mouse (Stardew Valley sets the mouse when using a gamepad to mimick the mouse cursor moving)
 *	
 *	- Raw input for keyboard is determined using library: https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
 *	- Mouse is also overwritten by mouse obtained from a slightly modified (I removed a Console.WriteLine) RawInputSharp: http://jstookey.com/arcade/rawmouse/ 
 *	- The OS mouse is locked in place by System.Windows.Forms.Cursor.Clip and an embedded autohotkey script (see MouseDisabler)
 */

namespace SplitScreen
{
	public class ModEntry : Mod
	{
		private ModConfig config;
		private Keys menuKey;
		
		private PlayerIndexController playerIndexController;

		private Keyboards.MultipleKeyboardManager kbManager;
		private Mice.MultipleMiceManager miceManager;
		
		public override void Entry(IModHelper helper)
		{
			//Setup Montior
			new SplitScreen.Monitor(Monitor);

			//Get player index if it is set in launch options, e.g. StardewModdingAPI.exe --log-path "third.txt" --player-index 3
			this.playerIndexController = new PlayerIndexController(Environment.GetCommandLineArgs());

			//Stardew Valley uses this as the target controller for rumble(and nothing else it seems)
			Game1.playerOneIndex = PlayerIndexController._PlayerIndex.GetValueOrDefault();

			//Removes FPS throttle when window inactive
			Game1.game1.InactiveSleepTime = new TimeSpan(0);
			
			var harmony = HarmonyInstance.Create("me.ilyaki.splitscreen");
			harmony.PatchAll(Assembly.GetExecutingAssembly());


			Helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;


			Helper.Events.GameLoop.UpdateTicked += delegate { Game1.game1.SetPrivateFieldValue("isActive", true); };//only do when in-game?

			//Keyboards/Mice Managers
			kbManager = new Keyboards.MultipleKeyboardManager(harmony);
			Helper.Events.GameLoop.UpdateTicked += kbManager.OnUpdate;

			Helper.Events.GameLoop.ReturnedToTitle += kbManager.OnAfterReturnToTitle;
			
			miceManager = new Mice.MultipleMiceManager();

			Helper.Events.Specialised.UnvalidatedUpdateTicked += miceManager.OnUpdateTick;
			Helper.Events.GameLoop.ReturnedToTitle += miceManager.OnAfterReturnToTitle;

			//Set the affinity for all SMAPI processes. Adjustments are handled by AffinityButtonMenu
			var processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
			int processorCount = Environment.ProcessorCount;
			for (int i = 0; i < processes.Count(); i++)
			{
				int designatedCPU = i % processorCount;
				designatedCPU = processorCount - designatedCPU - 1;//Start backwards (Use the last core for the first instance)
				try { processes[i].ProcessorAffinity = (IntPtr)(1 << designatedCPU); }
				catch (Exception e) { Monitor.Log($"Could not set affinity for {i}: {e.ToString()}", LogLevel.Debug); }
			}

			//Load the config
			this.config = Helper.ReadConfig<ModConfig>();
			if (!Enum.TryParse(this.config.MenuKey, out menuKey)) menuKey = Keys.N;
		}

		private void InputEvents_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
		{
			if ((Keys)e.Button == menuKey && Game1.activeClickableMenu == null)
				Game1.activeClickableMenu = new Menu.InputDeviceMenu(kbManager, miceManager);
		}
	}
}
