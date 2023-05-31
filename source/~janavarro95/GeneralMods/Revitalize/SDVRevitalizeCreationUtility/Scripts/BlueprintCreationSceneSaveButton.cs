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
    public partial class BlueprintCreationSceneSaveButton : Button
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

        }

        public override void _Pressed()
        {
            base._Pressed();

            //Display strings
            string blueprintDisplayName = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "DisplayNameText").Text;
            string blueprintDescription = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "DescriptionText").Text;
            string blueprintCategory = "StardewValley.Crafting";

            string newItemDisplayName = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "NewItemDisplayNameText").Text;
            string newItemDescription = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "NewItemDescriptionText").Text;
            string newItemCategory = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "NewItemCategoryText").Text;

            //Blueprint template params.
            string newItemId = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "ItemToDrawIdText").Text;
            string blueprintObjectId = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "BlueprintObjectIdText").Text;

            //Recipe related information.
            string recipesToUnlock = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipesToUnlockText").Text;
            string recipeTabId = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipeTabIdText").Text;
            string[] recipeIdSplitArray = recipesToUnlock.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            string recipeId = recipeIdSplitArray[recipeIdSplitArray.Length - 1].Replace("\"", "").Replace(" ", "").Replace("}","").Replace("\n",""); //Get the recipe id and remove all unnecessary whitespace.
            string recipeInputs= NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipeInputsText").Text;
            string recipeOutputs = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipeOutputsText").Text;
            string recipeOutputFilePath= NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipeFilePathText").Text;


            //All the other paths for new files go here.
            string blueprintOutputPath = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "BlueprintObjectFilePathText").Text;
            string displayStringOutputPath = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "BlueprintDisplayStringFilePathText").Text;
            string newItemDisplayStringsOutputPath = NodeExtensions.GetChild<TextEdit>(Game.Self, "ScrollContainer", "VBoxContainer", "NewItemDisplayStringFilePathText").Text;

            string codeGenerationBlueprintRelativePath= NodeExtensions.GetChild<IdFileSelectionButton>(Game.Self, "ScrollContainer", "VBoxContainer", "BlueprintIdGenerationSelectionList").Text;
            string codeGenerationItemRelativePath = NodeExtensions.GetChild<IdFileSelectionButton>(Game.Self, "ScrollContainer", "VBoxContainer", "ItemIdGenerationSelectionList").Text;
            string codeGenerationRecipeRelativePath = NodeExtensions.GetChild<IdFileSelectionButton>(Game.Self, "ScrollContainer", "VBoxContainer", "RecipeIdGenerationSelectionList").Text;

            //Write them to new json files.
            TemplateTransformerScript.WriteCraftingBlueprintFile(blueprintOutputPath, blueprintObjectId, recipesToUnlock, newItemId);
            TemplateTransformerScript.WriteDisplayStringsFile(displayStringOutputPath, blueprintObjectId, blueprintDisplayName, blueprintDescription, blueprintCategory);
            TemplateTransformerScript.WriteRecipeFileForBlueprintObject(recipeOutputFilePath, recipeTabId, recipeId, recipeInputs, recipeOutputs);
            TemplateTransformerScript.WriteDisplayStringsFile(newItemDisplayStringsOutputPath, newItemId, newItemDisplayName, newItemDescription, newItemCategory);

            //Generate new variable names for blueprints and items.
            string sanitizedBlueprintDisplayName = CodeGeneration.SanitizeDisplayNameForCSharpVariableName(blueprintDisplayName);

            CodeGeneration.GenerateId(codeGenerationBlueprintRelativePath,sanitizedBlueprintDisplayName, blueprintObjectId);
            CodeGeneration.GenerateId(codeGenerationItemRelativePath, CodeGeneration.SanitizeDisplayNameForCSharpVariableName(newItemDisplayName), newItemId);
            CodeGeneration.GenerateId(codeGenerationRecipeRelativePath, sanitizedBlueprintDisplayName, recipeId);
        }
    }
}
