/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using SpaceCore.UI;

namespace UITestbed;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        RootElement root = new RootElement();
        Label label = new Label();
        label.Font = Game1.dialogueFont;
        label.String = "AAAAAAAAA";
        label.LocalPosition = new Vector2(0, 0);
        root.AddChild(label);

        helper.Events.Display.RenderedHud += (sender, args) =>
        {
            // root.Update();
            // root.Draw(args.SpriteBatch);
        };

        helper.Events.Input.ButtonPressed += (sender, args) =>
        {

        };
    }
}
