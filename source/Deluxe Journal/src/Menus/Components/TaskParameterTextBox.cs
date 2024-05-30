/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using DeluxeJournal.Task;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A TextBox that parses input for a TaskParameter.</summary>
    public class TaskParameterTextBox : SideScrollingTextBox
    {
        public TaskParameter TaskParameter { get; set; }

        public TaskParser TaskParser { get; }

        public string Label { get; set; } = string.Empty;

        public TaskParameterTextBox(TaskParameter parameter, Task.TaskFactory factory, Texture2D? textBoxTexture, Texture2D? caretTexture, SpriteFont font, Color textColor, ITranslationHelper translation)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
            TaskParameter = parameter;
            TaskParser = new TaskParser(translation, new()
            {
                EnableFuzzySearch = true,
                IgnoreItems = !parameter.Attribute.Tag.Equals(TaskParameterTag.ItemList),
                IgnoreNpcs = !parameter.Attribute.Tag.Equals(TaskParameterTag.NpcName),
                IgnoreBuildings = !parameter.Attribute.Tag.Equals(TaskParameterTag.Building),
                IgnoreFarmAnimals = !parameter.Attribute.Tag.Equals(TaskParameterTag.FarmAnimalList),
                SetItemCategoryObject = parameter.Attribute.Constraints.HasFlag(Constraint.SObject),
                SetItemCategoryBigCraftable = parameter.Attribute.HasAnyConstraint(Constraint.SObject ^ Constraint.Craftable),
                SetItemCategoryCraftable = parameter.Attribute.Constraints.HasFlag(Constraint.Craftable),
                SetItemCategoryTool = parameter.Attribute.Constraints.HasFlag(Constraint.Upgradable)
            })
            {
                Factory = factory
            };

            TaskParser.ApplyParameterValue(parameter);
            FillWithParsedText(true);
        }

        /// <summary>Fill the text field with text and parse it.</summary>
        /// <param name="text">Text to parse.</param>
        /// <inheritdoc cref="FillWithParsedText(bool)"/>
        public bool FillWithParsedText(string text, bool caseSensitive = false)
        {
            Text = text;
            return FillWithParsedText(caseSensitive);
        }

        /// <summary>Fill the text field with the display name provided by the parser.</summary>
        /// <param name="caseSensitive">Include letter case when checking if the text was changed.</param>
        /// <returns><c>true</c> if the text was changed and <c>false</c> if it remained the same.</returns>
        public bool FillWithParsedText(bool caseSensitive = false)
        {
            string previous = Text;

            Text = TaskParameter.Attribute.Tag switch
            {
                TaskParameterTag.ItemList => TaskParser.ProxyItemDisplayName,
                TaskParameterTag.NpcName => TaskParser.NpcDisplayName,
                TaskParameterTag.Building => TaskParser.BuildingDisplayName,
                TaskParameterTag.FarmAnimalList => TaskParser.FarmAnimalDisplayName,
                TaskParameterTag.Count => TaskParser.Count.ToString(),
                _ => string.Empty
            };

            if (!caseSensitive)
            {
                return previous.ToLower() != Text.ToLower();
            }

            return previous != Text;
        }

        public override void RecieveTextInput(char inputChar)
        {
            base.RecieveTextInput(inputChar);
            UpdateParameter();
        }

        public override void RecieveTextInput(string text)
        {
            base.RecieveTextInput(text);
            UpdateParameter();
        }

        public override void RecieveCommandInput(char command)
        {
            base.RecieveCommandInput(command);

            switch (command)
            {
                case '\b':
                    UpdateParameter();
                    break;
            }
        }

        private void UpdateParameter()
        {
            if (string.IsNullOrEmpty(Text))
            {
                ClearParameterValue();
            }
            else if (!TaskParser.Parse(Text, TaskParser.ParseMode.UpdateFactory))
            {
                TaskParameter.Value = null;
            }
        }

        private void ClearParameterValue()
        {
            TaskParameter.Value = null;
            TaskParser.ApplyParameterValue(TaskParameter);
        }
    }
}
