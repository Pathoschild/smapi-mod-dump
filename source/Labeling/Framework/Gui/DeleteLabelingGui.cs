/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/Labeling
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;

namespace Labeling.Framework.Gui
{
    public class DeleteLabelingGui : ScreenGui
    {
        public DeleteLabelingGui()
        {
            foreach (var variable in ModEntry.GetInstance().Config.Labelings)
            {
                var deleteTitle =
                    $"{ModEntry.GetInstance().GetTranslation("labeling.deleteLabeling.button.delete.title")}:{variable.Name}";
                var delete = new Button(deleteTitle, deleteTitle);
                delete.OnLeftClicked = () =>
                {
                    ModEntry.GetInstance().Config.Labelings.Remove(variable);
                    RemoveElement(delete);
                };
                AddElement(delete);
            }
        }
    }
}