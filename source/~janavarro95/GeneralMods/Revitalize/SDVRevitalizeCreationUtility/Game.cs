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

namespace SdvRevitalizeCreationUtility.Scripts
{

    /// <summary>
    /// TODO: Seperate out the different scenes for blueprint object creation into it's own scene that can be instantiated.
    /// TODO: Create a recipe creation scene where the field is just a TextEdit group.
    /// TODO: Create a Mail/Letter creation scene.
    /// TODO: Create a seperate scene for just Display string creation.
    /// </summary>
    public partial class Game : Control
    {
        public static Game Self;

        public override void _Ready()
        {
            Self = this;
            // OS.WindowFullscreen = true;

            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
        }

        public static string GetPathToInputFields()
        {
            return System.IO.Path.Combine("ScrollContainer", "VBoxContainer");
        }

        /// <summary>
        /// Gets the path to the executable binary.
        /// </summary>
        /// <returns></returns>
        public static string GetGameDirectory()
        {
            if (Game.IsEditor() == false)
            {
                return OS.GetExecutablePath().GetBaseDir();
            }
            else
            {
                return ProjectSettings.GlobalizePath("res://");
            }
            //return ProjectSettings.GlobalizePath("res://");
        }

        /// <summary>
        /// Gets the base project folder for Revitalize.
        /// </summary>
        /// <param name="RemoveDriveLetter">Used to strip out the drive letter from the Path3D. Set to true to be removed since Godot has a hard time navigating the path for file system dialog.</param>
        /// <returns></returns>
        public static string GetRevitalizeBaseFolder(bool RemoveDriveLetter=true)
        {
            List<string> strs = new List<string>();

            string[] splits = GetGameDirectory().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in splits)
            {
                strs.Add(s);
            }
            strs.RemoveAt(strs.Count - 1);
            if (RemoveDriveLetter)
            {
                //Gross, but strips out the beginning disk drive lettering/name and forces it into a directory.
                return (System.IO.Path.Combine(strs.ToArray()).Replace("\\", "/").Substring(2) + "/");
            }
            else
            {
                return "/" + (System.IO.Path.Combine(strs.ToArray()).Replace("\\", "/") + "/");
            }
        }

        public static string GetRevitalizeEnglishContentPackFolder()
        {
            string contentPackPath = System.IO.Path.Combine(GetRevitalizeBaseFolder(), "ContentPacks", "RevitalizeContentPack en-US" + "/").Replace("\\", "/");
            return contentPackPath;
        }

        /// <summary>
        /// Checks to see if the game is currently runnin inside the editor ui, or an editor build. Will return <code>false</code> for release builds.
        /// </summary>
        /// <returns></returns>
        public static bool IsEditor()
        {
            return OS.HasFeature("editor");
        }
    }
}
