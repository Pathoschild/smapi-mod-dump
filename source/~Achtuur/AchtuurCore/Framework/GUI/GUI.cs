/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.GUI;
public class GUI : IClickableMenu
{
    private Vector2 ScreenDimension => new Vector2(Game1.viewport.Size.Width, Game1.viewport.Size.Height) / 2;
    private Vector2 TopLeft => ScreenDimension - page.Size / 2;

    Page page;
    List<Button> buttons;
    List<MenuButton> menuButtons;
    Button ExitButton;

    public bool Enabled;

    public GUI() : base(500, 500, 500, 500, true)
    {
        Enabled = false;
        page = new();
        buttons = new();
        menuButtons = new();

        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += OnUpdate;
        Game1.onScreenMenus.Add(this);
    }

    private void OnUpdate(object sender, UpdateTickedEventArgs e)
    {
        
    }

    public void AddMenuButton()
    {
        menuButtons.Add(new());
    }

    public void SetPage(IEnumerable<Border> border)
    {
        this.page.SetBorders(border);
    }

    public override void draw(SpriteBatch sb)
    {
        page.Draw(sb, TopLeft);
        DrawMenuButtons(sb);
        DrawButtons(sb);
    }

    public void Draw(SpriteBatch sb)
    {
        page.Draw(sb, TopLeft);
        DrawMenuButtons(sb);
        DrawButtons(sb);
    }

    public void DrawButtons(SpriteBatch sb)
    {

    }

    public void DrawMenuButtons(SpriteBatch sb)
    {
        int tileSize = Game1.tileSize;
        Vector2 offset = new(TopLeft.X + tileSize*0.5f, TopLeft.Y - tileSize*0.77f);
        foreach(MenuButton menuButton in menuButtons)
        {

            if (ModEntry.DebugDraw)
                sb.DrawBorder(offset, menuButton.Size, Color.Red);

            menuButton.Draw(sb, offset);
            offset.X += menuButton.Width + 1; // 1 pixel margin
        }
    }
}
