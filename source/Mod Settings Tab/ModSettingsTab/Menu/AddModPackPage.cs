using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Framework;
using ModSettingsTab.Framework.Components;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Menu
{
    public class AddModPackPage : BaseOptionsModPage
    {
        private readonly ClickableTextureComponent _button;
        private bool _clicked;
        private bool _buttonHovered;

        public AddModPackPage(int x, int y, int width, int height) : base(x, y, width, height)
        {
            Options.Add(new ModManagerAddNameTextBox());
            var modList = ModData.ModList.Keys.Where(id => id != "GilarF.ModSettingsTab");
            foreach (var uniqueId in modList)
            {
                Options.Add(new ModManagerCheckBox(uniqueId));
            }
            FilterTextBox = new FilterTextBox(this, FilterTextBox.FilterType.Options,
                xPositionOnScreen + width / 2 + 112, yPositionOnScreen + 40);
            
            _button = new ClickableTextureComponent("AddButton",
                new Rectangle(
                    xPositionOnScreen + 80,
                    yPositionOnScreen + height + 10 ,
                    64, 64),"",
                "Add",
                Game1.mouseCursors,
                new Rectangle(128, 256, 64, 64),
                1f)
            {
                myID = 12233
            };
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_button.containsPoint(x, y) && !_clicked)
            {
                _clicked = true;
                Game1.playSound("newArtifact");
                var list = Options.Where(o => o is ModManagerCheckBox opt && opt.IsChecked)
                    .Select(o => o.ModId)
                    .ToList();
                var name = ((ModManagerAddNameTextBox) Options[0]).Text;
                if (string.IsNullOrEmpty(name) || list.Count < 1)
                {
                    _clicked = false;
                    return;
                }
                ModManager.AddModPack(list,name);
                return;
            }
            
            base.receiveLeftClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (_button.containsPoint(x, y))
            {
                _button.tryHover(x, y, 0.25f);
                if (!_buttonHovered)
                    Game1.playSound("Cowboy_Footstep");
                _buttonHovered = true;
                HoverText = _button.hoverText;
            }
            else
            {
                _buttonHovered = false;
                _button.scale = 1f;
            }
        }

        public override void draw(SpriteBatch b)
        {
            _button.draw(b);
            base.draw(b);
        }
    }
}