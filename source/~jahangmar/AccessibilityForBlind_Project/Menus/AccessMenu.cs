/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace AccessibilityForBlind.Menus
{
    public class MenuItem
    {
        public string Label;
        public string TextOnAction;
        public Action clickAction;
        public Action speakOnClickAction;
        //public Action Action;
        public MenuItem prev, next;
        public Rectangle Bounds;
        public string Description = "";
        //public bool ClickOnSelect;
        protected ClickableComponent StardewComponent;
        protected IClickableMenu menu;

        protected MenuItem()
        {

        }

        protected MenuItem(string label, Action action, Rectangle bounds, IClickableMenu menu)
        {
            Label = label;
            this.menu = menu;
            //Action = action;
            Bounds = bounds;
            //ClickOnSelect = clickOnSelect;
        }

        public static MenuItem MenuItemFromComponent(StardewValley.Menus.ClickableComponent component, IClickableMenu menu, string label = null)
        {
            MenuItem item = new MenuItem(label, null, component.bounds, menu);
            item.StardewComponent = component;
            item.SetActionFromComponent(component, label);
            return item;
        }

        protected virtual void SetActionFromComponent(StardewValley.Menus.ClickableComponent component, string label =null)
        {
            if (label == null)
                label = component.name;
            this.Label = label;
            this.Bounds = component.bounds;
            this.clickAction += Click;
            this.speakOnClickAction += DefaultSpeakOnClickAction;
        }

        public virtual void DefaultSpeakOnClickAction()
        {
            if (TextOnAction != null)
                TextToSpeech.Speak(this.TextOnAction);
        }

        public virtual void SpeakOnSelect()
        {
            if (Label != null)
                TextToSpeech.Speak(Label);
        }

        public void Click()
        {
            menu?.receiveLeftClick(Bounds.X + 10, Bounds.Y + 10);
        }

        public virtual void Select()
        {
            Microsoft.Xna.Framework.Input.Mouse.SetPosition(Bounds.X + 10, Bounds.Y + 10);
        }

        public void Activate()
        {
            //action first because speak action might need changed values from click
            if (clickAction != null)
                clickAction.Invoke();
            if (speakOnClickAction != null)
                speakOnClickAction.Invoke();
        }
    }

    public class MenuTextBox : MenuItem
    {
        protected TextBox StardewTextBox;

        protected MenuTextBox()
        {

        }

        public MenuTextBox(string label, Action action, Rectangle bounds, IClickableMenu menu) : base(label, action, bounds, menu)
        {
        }

        protected override void SetActionFromComponent(ClickableComponent component, string label = null)
        {
            base.SetActionFromComponent(component, label);
            //this.clickAction += Click;
            //this.speakOnClickAction += DefaultSpeakOnClickAction;
        }

        public static MenuTextBox MenuTextBoxFromComponent(ClickableComponent component, TextBox textBox, IClickableMenu menu, string label = null)
        {
            MenuTextBox menuTextBox = new MenuTextBox();
            menuTextBox.menu = menu;
            menuTextBox.StardewTextBox = textBox;
            menuTextBox.SetActionFromComponent(component, label);
            return menuTextBox;
        }

        public override void DefaultSpeakOnClickAction()
        {
            TextToSpeech.Speak(this.Label + " is " + StardewTextBox.Text + " " + TextOnAction);
        }

        public override void SpeakOnSelect()
        {
            TextToSpeech.Speak("type " + Label);
        }

        public override void Select()
        {
            base.Select();
            StardewTextBox.SelectMe();
        }

        public void Unselect()
        {
            StardewTextBox.Selected = false;
        }
    }

    public abstract class AccessMenu
    {
        private List<MenuItem> items;
        protected MenuItem current;
        protected StardewValley.Menus.IClickableMenu stardewMenu;

        protected AccessMenu(StardewValley.Menus.IClickableMenu menu)
        {
            items = new List<MenuItem>();
            Point point = DefaultMouse();
            //Microsoft.Xna.Framework.Input.Mouse.SetPosition(point.X, point.Y);
            stardewMenu = menu;
        }

        protected void AddItem(MenuItem item)
        {
            if (items.Count > 0)
            {
                MenuItem last = items[items.Count - 1];
                last.next = item;
                item.prev = last; 
            }
            items.Add(item);
            MenuItem first = items[0];
            item.next = first;
            first.prev = item;
        }

        public void UnselectAll()
        {
            foreach (MenuItem menuItem in items)
                if (menuItem is MenuTextBox menuTextBox)
                    menuTextBox.Unselect();
        }

        public StardewValley.Menus.IClickableMenu GetStardewMenu() => stardewMenu;

        public abstract string GetTitle();

        public Point DefaultMouse() => new Point(0, 0);

        public void ClearItems()
        {
            items.Clear();
            current = null;
        }

        public virtual void NextItem()
        {
            if (items.Count == 0)
                return;

            if (current == null)
                current = items[0];
            else
                current = current.next;

            UnselectAll();
            current.Select();
            current.SpeakOnSelect();
        }

        public virtual void PrevItem()
        {
            if (items.Count == 0)
                return;

            if (current == null)
                current = items[0];
            else
                current = current.prev;

            UnselectAll();
            current.Select();
            current.SpeakOnSelect();
        }

        public void ActivateItem()
        {
            if (current != null)
                current.Activate();
        }

        public virtual void ButtonPressed(StardewModdingAPI.SButton button)
        {
            if (Inputs.IsMenuNextButton(button))
            {
                NextItem();
            }
            else if (Inputs.IsMenuPrevButton(button))
            {
                PrevItem();
            }
            else if (Inputs.IsMenuActivateButton(button))
            {
                ActivateItem();
            }
            else if (Inputs.IsTTSInfoButton(button))
            {
                if (current != null)
                {
                    if (current is MenuTextBox)
                        current.Activate(); //speak text box content
                    else
                        TextToSpeech.Speak(current.Label + (current.Description.Length > 0 ? ": " + current.Description : ""));
                }
            }
            else if (Inputs.IsTTSRepeatButton(button))
            {
                TextToSpeech.Repeat();
            }
            else if (Inputs.IsTTSStopButton(button))
            {
                TextToSpeech.Stop();
            }
        }
    }
}
