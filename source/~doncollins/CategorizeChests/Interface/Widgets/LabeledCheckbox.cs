using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A checkbox with a label next to it, like so: [x] Foo
    /// </summary>
    class LabeledCheckbox : Widget
    {
        public event Action<bool> OnChange;
        public bool Checked { get; set; } = false;

        private readonly Widget CheckedBox;
        private readonly Widget UncheckedBox;
        private readonly Label Label;

        public LabeledCheckbox(string labelText)
        {
            CheckedBox = AddChild(new Stamp(Sprites.FilledCheckbox));
            UncheckedBox = AddChild(new Stamp(Sprites.EmptyCheckbox));

            Label = AddChild(new Label(labelText, Color.Black));
            var padding = (int) Label.Font.MeasureString(" ").X;

            Height = Math.Max(CheckedBox.Height, Label.Height);
            CheckedBox.CenterVertically();
            UncheckedBox.CenterVertically();
            Label.CenterVertically();
            Label.X = CheckedBox.X + CheckedBox.Width + padding;
            Width = Label.X + Label.Width;
        }

        public override bool ReceiveLeftClick(Point point)
        {
            Checked = !Checked;
            OnChange?.Invoke(Checked);
            return true;
        }

        public override void Draw(SpriteBatch batch)
        {
            var box = Checked ? CheckedBox : UncheckedBox;
            box.Draw(batch);
            Label.Draw(batch);
        }
    }
}