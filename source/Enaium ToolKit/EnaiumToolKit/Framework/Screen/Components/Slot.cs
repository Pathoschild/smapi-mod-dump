/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Components;

public class Slot<T> : Component where T : Slot<T>.Entry
{
    public List<T> Entries;


    public int SlotHeight;

    public T SelectedEntry;
    private bool _mouseClicked;
    private int _index;
    private int _maxEntry;

    public Slot(string title, string description, int x, int y, int width, int height, int slotHeight) : base(
        title, description, x, y, width, height)
    {
        Entries = new List<T>();
        SlotHeight = slotHeight;
        _mouseClicked = false;
        _index = 0;
        _maxEntry = Height / slotHeight;
    }

    public override void Render(SpriteBatch b)
    {
        Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), X, Y, Width, Height);

        for (int i = _index, j = 0;
             j < (Entries.Count >= _maxEntry ? _maxEntry : Entries.Count);
             i++, j++)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), X,
                Y + j * SlotHeight, Width, SlotHeight, Color.Wheat, 4f, false);
            Entries[i].Width = Width;
            Entries[i].Height = SlotHeight;
            var slotY = Y + j * SlotHeight;
            Entries[i].Render(b, X, slotY);
            if (Entries[i].Hovered)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), X, slotY, Width,
                    SlotHeight,
                    Color.Red, 4f, false);
            }

            if (Entries[i].Equals(SelectedEntry))
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), X, slotY, Width,
                    SlotHeight,
                    Color.Red, 4f, false);
            }

            if (Entries[i].Hovered && _mouseClicked)
            {
                SelectedEntry = Entries[i];
            }
        }
    }

    public override void MouseLeftClicked(int x, int y)
    {
        _mouseClicked = true;
        base.MouseLeftClicked(x, y);
    }

    public override void MouseLeftReleased(int x, int y)
    {
        _mouseClicked = false;
        base.MouseLeftReleased(x, y);
    }

    public override void MouseScrollWheelAction(int direction)
    {
        if (!Hovered) return;

        if (direction > 0 && _index > 0)
        {
            _index--;
        }

        else if (direction < 0 &&
                 _index + (Entries.Count >= _maxEntry ? _maxEntry : Entries.Count) <
                 Entries.Count)
        {
            _index++;
        }

        base.MouseScrollWheelAction(direction);
    }

    public void AddEntry(T entry)
    {
        Entries.Add(entry);
    }

    public void AddEntryRange(params T[] entry)
    {
        Entries.AddRange(entry);
    }

    public void RemoveEntry(T entry)
    {
        Entries.Remove(entry);
    }

    public void RemoveEntryRange(params T[] entry)
    {
        foreach (var variable in entry)
        {
            Entries.Remove(variable);
        }
    }

    public abstract class Entry
    {
        public bool Hovered;

        public int Width;
        public int Height;

        protected Entry()
        {
            Hovered = false;
        }

        public abstract void Render(SpriteBatch b, int x, int y);
    }
}