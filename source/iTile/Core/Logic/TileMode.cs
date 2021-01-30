/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Client.UI.Impl;
using iTile.Core.Logic.Action;
using StardewModdingAPI;
using StardewValley;
using xTile.Tiles;

namespace iTile.Core.Logic
{
    public class TileMode : IInitializable
    {
        private string onControlMsgPrefix = "Tile mode turned ";
        private string onActionControlMsgPrefix = "Current action: ";
        public TileOutline outline;
        private ActionManager actionManager;

        public TileMode()
        {
            Init();
        }

        public Tile CurrentTile { get; set; }
        public bool State { get; set; }
        public ActionManager.Action CurrentAction { get; set; } = ActionManager.Action.Copy;

        public ActionManager ActionManager
            => actionManager ?? (actionManager = new ActionManager());

        public void Init()
        {
            outline = new TileOutline(this);
            SubscribeEvents();
        }

        public void OnTileModeControl(bool state)
        {
            CoreManager.ShowNotification(CoreManager.defaultNotificationTime, GetOnControlMsg(state));
            State = state;
        }

        public void OnTileModeActionControl(ControlPanel cp)
        {
            if (CurrentAction == ActionManager.Action.Copy)
            {
                CurrentAction = ActionManager.Action.Paste;
                cp.actionBtnIcon.texture = cp.pasteAction;
            }
            else if (CurrentAction == ActionManager.Action.Paste)
            {
                CurrentAction = ActionManager.Action.Delete;
                cp.actionBtnIcon.texture = cp.deleteAction;
            }
            else if (CurrentAction == ActionManager.Action.Delete)
            {
                CurrentAction = ActionManager.Action.Restore;
                cp.actionBtnIcon.texture = cp.restoreAction;
            }
            else if (CurrentAction == ActionManager.Action.Restore)
            {
                CurrentAction = ActionManager.Action.Copy;
                cp.actionBtnIcon.texture = cp.copyAction;
            }
            CoreManager.ShowNotification(CoreManager.defaultNotificationTime, GetOnActionControlMsg());
        }

        private string GetOnControlMsg(bool state)
        {
            return onControlMsgPrefix + (state ? "on" : "off");
        }

        private string GetOnActionControlMsg()
        {
            return onActionControlMsgPrefix + CurrentAction.ToString();
        }

        private void SubscribeEvents()
        {
            iTile._Helper.Events.Display.RenderingHud += outline.Draw;
            iTile._Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnAction()
        {
            CurrentTile = ActionManager.PerformAction(CurrentAction, Game1.currentCursorTile, CurrentTile) ?? CurrentTile;
        }

        private void OnButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (GetClicked(e.Button))
                OnAction();
        }

        private bool GetClicked(SButton button)
        {
            return Context.IsWorldReady && Context.IsPlayerFree && State && button == CoreManager.Instance.config.ActionKey;
        }
    }
}