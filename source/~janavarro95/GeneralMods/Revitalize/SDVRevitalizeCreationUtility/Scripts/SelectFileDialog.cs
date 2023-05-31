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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdvRevitalizeCreationUtility.Scripts
{
    public partial class SelectFileDialog:FileDialog
    {
        public TextEdit textEdit;
        string textToSet;

        public override void _Ready()
        {
            this.Connect("file_selected",new Callable(this,"_on_SelectFileDialog_file_selected"));
            this.Connect("dir_selected",new Callable(this,"_on_SelectFileDialog_dir_selected"));
            this.Connect("confirmed",new Callable(this,"_on_SelectFileDialog_confirmed"));

            this.GetLineEdit().TextChanged += this.SelectFileDialog_TextChanged;
            this.GetLineEdit().TextSubmitted += this.SelectFileDialog_TextSubmitted;
        }

        private void SelectFileDialog_TextSubmitted(string newText)
        {
            this.textEdit.Text = Path.Combine(this.CurrentPath, this.GetLineEdit().Text);
        }

        private void SelectFileDialog_TextChanged(string newText)
        {
            this.textToSet= newText;
        }

        public virtual void _on_SelectFileDialog_dir_selected(string input)
        {
            if (this.Filters.Length == 1)
            {
                string extension = System.IO.Path.GetExtension(input);
                if (string.IsNullOrEmpty(extension))
                {
                    this.textToSet = input + ".json";
                }

                if (!System.IO.Path.GetExtension(input).Equals(this.Filters[0].Replace("*","")))
                {
                    throw new Exception("Bad file path extension!");
                }
            }
            this.textEdit.Text = Path.Combine(this.CurrentPath, input);
        }

        public virtual void _on_SelectFileDialog_file_selected(string input)
        {
            this.textEdit.Text = Path.Combine(this.CurrentPath, input);
        }

        public virtual void _on_SelectFileDialog_confirmed()
        {
            this.textEdit.Text = this.GetLineEdit().Text;
        }
    }
}
