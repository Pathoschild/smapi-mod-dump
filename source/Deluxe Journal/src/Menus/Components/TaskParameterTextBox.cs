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
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A TextBox that parses input for a TaskParameter.</summary>
    public class TaskParameterTextBox : SideScrollingTextBox
    {
        public TaskParameter TaskParameter { get; set; }

        public TaskParser TaskParser { get; }

        public string Label { get; set; }

        public string TextWithParse
        {
            get
            {
                return Text;
            }

            set
            {
                Text = value;
                UpdateParameter();
            }
        }

        public TaskParameterTextBox(TaskParameter parameter, Texture2D? textBoxTexture, Texture2D? caretTexture, SpriteFont font, Color textColor, ITranslationHelper translation) :
            base(textBoxTexture, caretTexture, font, textColor)
        {
            TaskParameter = parameter;
            TaskParser = new TaskParser(translation);
            Label = string.Empty;
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

            if (command == '\b')
            {
                UpdateParameter();
            }
        }

        private void UpdateParameter()
        {
            TaskParser.Parse(Text);
            
            if (!TaskParser.SetParameterValue(TaskParameter))
            {
                if (TaskParameter.Type == typeof(string))
                {
                    TaskParameter.Value = Text.Trim();
                }
                else if (TaskParameter.Type == typeof(int) && int.TryParse(Text.Trim(), out int num))
                {
                    TaskParameter.Value = num;
                }
                else
                {
                    TaskParameter.Value = null;
                }
            }
        }
    }
}
