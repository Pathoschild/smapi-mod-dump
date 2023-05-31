/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Godot;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdvRevitalizeCreationUtility.Scripts
{
    public partial class ButtonOpenFileDiaog : Button
    {

        public enum DirectoryToBeginFrom
        {
            NULL,
            DisplayStrings,
            CraftingBlueprintDisplayStrings,
            BlueprintObjects,
            CraftingPath,
        }

        [Export]
        public DirectoryToBeginFrom startingDir = DirectoryToBeginFrom.NULL;

        [Export]
        public string TextFieldToFind;

        public override void _Pressed()
        {
            base._Pressed();
            SelectFileDialog dialog = NodeExtensions.GetChild<SelectFileDialog>(Game.Self, "SelectFileDialog");
            dialog.AddFilter("*.json");

            dialog.CurrentFile = ""; ;
            dialog.FileMode = FileDialog.FileModeEnum.SaveFile;
            dialog.Access = FileDialog.AccessEnum.Filesystem;

            dialog.textEdit = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", this.TextFieldToFind);

            if (this.startingDir == DirectoryToBeginFrom.BlueprintObjects)
            {
                dialog.CurrentDir = System.IO.Path.Combine(Game.GetRevitalizeBaseFolder(), ObjectsDataPaths.CraftingBlueprintsPath);
            }
            if (this.startingDir == DirectoryToBeginFrom.DisplayStrings)
            {
                dialog.CurrentDir = System.IO.Path.Combine(Game.GetRevitalizeEnglishContentPackFolder(), StringsPaths.ObjectDisplayStrings);
            }
            if (this.startingDir == DirectoryToBeginFrom.CraftingBlueprintDisplayStrings)
            {
                dialog.CurrentDir = System.IO.Path.Combine(Game.GetRevitalizeEnglishContentPackFolder(), StringsPaths.ObjectDisplayStrings, "CraftingBlueprints");
            }
            if (this.startingDir == DirectoryToBeginFrom.CraftingPath)
            {
                dialog.CurrentDir = System.IO.Path.Combine(Game.GetRevitalizeBaseFolder(), CraftingDataPaths.CraftingPath);
            }

            dialog.Popup();
        }
    }
}
