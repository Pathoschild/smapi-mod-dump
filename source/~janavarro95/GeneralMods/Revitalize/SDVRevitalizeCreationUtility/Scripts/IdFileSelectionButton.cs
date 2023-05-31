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
using System;
namespace SdvRevitalizeCreationUtility.Scripts
{
    /// <summary>
    /// Class to populate a dropdown button with the options for potential code files to generate ids for.
    /// </summary>
    public partial class IdFileSelectionButton : OptionButton
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        [Export]
        public string relativePathForFileGeneration = "";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            foreach (string option in string.IsNullOrEmpty(this.relativePathForFileGeneration) ? CodeGeneration.GenerateListOfCodeReferencesForIds() : CodeGeneration.GenerateListOfCodeReferencesForIds(this.relativePathForFileGeneration))
            {
                this.AddItem(option);
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
