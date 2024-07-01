/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Task;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary><see cref="IPage"/> child menu for selecting and navigating a <see cref="TaskEntryComponent"/> on a gamepad.</summary>
    public class TaskEntryMenu : IClickableMenu
    {
        public readonly ClickableComponent checkbox;
        public readonly ClickableComponent editbox;
        public readonly ClickableComponent removeButton;

        private readonly ITranslationHelper _translation;
        private readonly TaskEntryComponent _entry;
        private readonly ITask _task;
        private string _hoverText;

        public TaskEntryMenu(TaskEntryComponent entry, ITask task, ITranslationHelper translation) : base()
        {
            xPositionOnScreen = entry.bounds.X;
            yPositionOnScreen = entry.bounds.Y;
            width = entry.bounds.Width;
            height = entry.bounds.Height;

            _translation = translation;
            _entry = entry;
            _task = task;
            _hoverText = string.Empty;

            checkbox = _entry.checkbox;
            checkbox.myID = 100;
            checkbox.upNeighborID = CUSTOM_SNAP_BEHAVIOR;
            checkbox.downNeighborID = CUSTOM_SNAP_BEHAVIOR;
            checkbox.leftNeighborID = CUSTOM_SNAP_BEHAVIOR;
            checkbox.rightNeighborID = 101;

            removeButton = _entry.removeButton;
            removeButton.myID = 102;
            removeButton.upNeighborID = CUSTOM_SNAP_BEHAVIOR;
            removeButton.downNeighborID = CUSTOM_SNAP_BEHAVIOR;
            removeButton.leftNeighborID = 101;
            removeButton.rightNeighborID = CUSTOM_SNAP_BEHAVIOR;

            editbox = new ClickableComponent(new Rectangle(_entry.bounds.Center.X, checkbox.bounds.Y, checkbox.bounds.Width, checkbox.bounds.Height), "")
            {
                myID = 101,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = 102,
                leftNeighborID = 100
            };

            Game1.playSound("smallSelect");
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = editbox;
            snapCursorToCurrentSnappedComponent();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (GetParentMenu() != null)
            {
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            exitThisMenuNoSound();

            // forward snap movement back to TasksPage
            Game1.activeClickableMenu?.applyMovementKey(direction switch
            {
                Game1.up => Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton),
                Game1.down => Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton),
                Game1.left => Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton),
                Game1.right => Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton),
                _ => Keys.None
            });
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            TasksPage? tasksPage = GetParentMenu() as TasksPage;

            if (checkbox.containsPoint(x, y))
            {
                if (_task.Active)
                {
                    _task.Complete = !_task.Complete;
                    _task.MarkAsViewed();
                    Game1.playSound("tinyWhip", _task.Complete ? 2000 : 1000);
                }
                else
                {
                    _task.Active = true;
                    Game1.playSound("newRecipe");
                }
            }
            else
            {
                if (editbox.containsPoint(x, y) && tasksPage != null)
                {
                    tasksPage.OpenTaskOptionsMenu(_task);
                    return;
                }
                else if (removeButton.containsPoint(x, y))
                {
                    tasksPage?.RemoveTask(_task);
                    Game1.playSound("woodyStep");
                }

                exitThisMenuNoSound();
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!Game1.options.SnappyMenus
                || Game1.options.doesInputListContain(Game1.options.menuButton, key)
                || Game1.options.doesInputListContain(Game1.options.journalButton, key))
            {
                exitThisMenuNoSound();

                // forward key press back to TasksPage
                Game1.activeClickableMenu?.receiveKeyPress(key);
            }
            else
            {
                applyMovementKey(key);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = string.Empty;

            if (editbox.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.editbox.hover");
            }
            else if (removeButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.removebutton.hover");
            }
        }

        public override void draw(SpriteBatch b)
        {
            int colorIndex = _task.ColorIndex > 0 || _task.GroupColorIndex < 0 ? _task.ColorIndex : _task.GroupColorIndex;

            _entry.Draw(b, _task, DeluxeJournalMod.ColorSchemas[colorIndex < DeluxeJournalMod.ColorSchemas.Count ? colorIndex : 0], true);

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }
    }
}
