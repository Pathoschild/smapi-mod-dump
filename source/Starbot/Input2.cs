using StardewValley;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Starbot
{
    public class Input2 : IInputSimulator
    {
        public static bool Installed { get; set;
        }
        public Input2()
        {

        }

        public void Update()
        {
        }

        public void InstallSimulator()
        {
            Mod.instance.Monitor.Log("Input simulator installed.", StardewModdingAPI.LogLevel.Info);
            Installed = true;
            typeof(Game1).GetField("inputSimulator", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);
        }

        public void UninstallSimulator()
        {
            Mod.instance.Monitor.Log("Input simulator uninstalled.", StardewModdingAPI.LogLevel.Info);
            Installed = false;
            typeof(Game1).GetField("inputSimulator", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
        }

        public void StartActionButton() { ActionButtonPressed = true; }
        public void StopActionButton() { ActionButtonPressed = false; }

        public void StartSwitchTool() { SwitchToolButtonPressed = true; }
        public void StopSwitchTool() { SwitchToolButtonPressed = false; }

        public void StartUseTool() { UseToolButtonPressed = true; UseToolButtonReleased = false; }
        public void StopUseTool() { UseToolButtonPressed = false; UseToolButtonReleased = true; }

        public void StartMoveLeft(){MoveLeftHeld = true;}
        public void StopMoveLeft(){MoveLeftHeld = false;}

        public void StartMoveRight(){MoveRightHeld = true;}
        public void StopMoveRight(){MoveRightHeld = false;}

        public void StartMoveUp() { MoveUpHeld = true; }
        public void StopMoveUp() { MoveUpHeld = false; }

        public void StartMoveDown() { MoveDownHeld = true; }
        public void StopMoveDown() { MoveDownHeld = false; }

        private bool ActionButtonPressed = false;
        private bool SwitchToolButtonPressed = false;
        private bool UseToolButtonPressed = false;
        private bool UseToolButtonReleased = false;
        private bool AddItemToInventoryButtonPressed = false;
        private bool CancelButtonPressed = false;
        private bool MoveDownHeld = false;
        private bool MoveUpHeld = false;
        private bool MoveRightHeld = false;
        private bool MoveLeftHeld = false;

        public void SimulateInput(ref bool actionButtonPressed, ref bool switchToolButtonPressed, ref bool useToolButtonPressed, ref bool useToolButtonReleased, ref bool addItemToInventoryButtonPressed, ref bool cancelButtonPressed, ref bool moveUpPressed, ref bool moveRightPressed, ref bool moveLeftPressed, ref bool moveDownPressed, ref bool moveUpReleased, ref bool moveRightReleased, ref bool moveLeftReleased, ref bool moveDownReleased, ref bool moveUpHeld, ref bool moveRightHeld, ref bool moveLeftHeld, ref bool moveDownHeld)
        {
            actionButtonPressed = ActionButtonPressed;
            switchToolButtonPressed = SwitchToolButtonPressed;
            useToolButtonPressed = UseToolButtonPressed;
            useToolButtonReleased = UseToolButtonReleased;
            addItemToInventoryButtonPressed = AddItemToInventoryButtonPressed;
            cancelButtonPressed = CancelButtonPressed;
            moveUpPressed = false;// MoveUpPressed;
            moveRightPressed = false;// MoveRightPressed;
            moveLeftPressed = false;// MoveLeftPressed;
            moveDownPressed = false;// MoveDownPressed;
            moveUpReleased = false;// MoveUpReleased;
            moveRightReleased = false;// MoveRightReleased;
            moveLeftReleased = false;// MoveLeftReleased;
            moveDownReleased = false;// MoveDownReleased;
            moveUpHeld = MoveUpHeld;
            moveRightHeld = MoveRightHeld;
            moveLeftHeld = MoveLeftHeld;
            moveDownHeld = MoveDownHeld;
        }
    }
}
