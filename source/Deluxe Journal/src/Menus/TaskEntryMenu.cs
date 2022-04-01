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
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Menus
{
    /// <summary>TasksPage child menu for selecting and navigating a TaskEntryComponent on a gamepad.</summary>
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
            _hoverText = "";

            checkbox = _entry.checkbox;
            checkbox.myID = 100;
            checkbox.rightNeighborID = 101;

            removeButton = _entry.removeButton;
            removeButton.myID = 102;
            removeButton.leftNeighborID = 101;

            editbox = new ClickableComponent(new Rectangle(_entry.bounds.Center.X, checkbox.bounds.Y, checkbox.bounds.Width, checkbox.bounds.Height), "")
            {
                myID = 101,
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

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            TasksPage? tasksPage = GetParentMenu() as TasksPage;

            if (checkbox.containsPoint(x, y))
            {
                if (_task.Active)
                {
                    _task.Complete = !_task.Complete;
                    _task.MarkAsViewed();
                    Game1.playSoundPitched("tinyWhip", _task.Complete ? 2000 : 1000);
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
                    Game1.playSound("trashcan");
                }

                exitThisMenuNoSound();
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!Game1.options.SnappyMenus ||
                Game1.options.doesInputListContain(Game1.options.menuButton, key) ||
                Game1.options.doesInputListContain(Game1.options.moveUpButton, key) ||
                Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
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
            _hoverText = "";

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
            _entry.Draw(b, _task, true);

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }
    }
}
