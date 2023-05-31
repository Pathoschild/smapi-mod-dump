/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdvRevitalizeCreationUtility.Scripts
{
    /// <summary>
    /// Class to help automatically generate code for the Stardew Valley Revitalize codebase.
    /// </summary>
    public static class CodeGeneration
    {
        /// <summary>
        /// Generates a list of all code references for all of the c# item and object id files for Revitalize.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static List<string> GenerateListOfCodeReferencesForIds(params string[] relativePath)
        {
            string relativePathString = Path.Combine(relativePath);
            string absPath = Path.Combine(GetBaseIdsDirectory(), relativePathString);
            List<string> returnedJsonFiles = new List<string>();
            foreach (string folder in Directory.GetDirectories(absPath))
            {
                string folderRelativePath = Path.Combine(relativePathString, Path.GetFileName(folder));
                returnedJsonFiles.AddRange(GenerateListOfCodeReferencesForIds(folderRelativePath));
            }
            foreach (string file in Directory.GetFiles(absPath, "*.cs"))
            {
                string fileRelativePath = Path.Combine(relativePathString, Path.GetFileName(file));
                returnedJsonFiles.Add(fileRelativePath);
            }
            return returnedJsonFiles;
        }

        /// <summary>
        /// Adds the item id to a given relative file path passed in.
        /// </summary>
        /// <param name="RelativePathToFile"></param>
        /// <param name="CSharpVarName"></param>
        /// <param name="RevitalizeId"></param>
        public static void GenerateId(string RelativePathToFile, string CSharpVarName, string RevitalizeId)
        {
            GenerateIdABSPath(Path.Combine(GetBaseIdsDirectory(true), RelativePathToFile), CSharpVarName, RevitalizeId);
        }

        /// <summary>
        /// Generates an item id for the full path to the file. Note that this id will always be a public static string.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="CSharpVarName"></param>
        /// <param name="RevitalizeId"></param>
        public static void GenerateIdABSPath(string FilePath, string CSharpVarName, string RevitalizeId)
        {

            if (string.IsNullOrEmpty(CSharpVarName))
            {
                throw new Exception("Can't generate an empty variable name!");
            }
            if (string.IsNullOrEmpty(RevitalizeId))
            {
                throw new Exception("Revitalize id is not present!");
            }

            string[] lines = System.IO.File.ReadAllLines(FilePath);
            Godot.GD.Print("Number of lines in file: "+lines.Length);
            Godot.GD.Print("Reading File: " + FilePath);
            StringBuilder updateFileContent = new StringBuilder();
            bool endOfClassFound = false;
            for(int lineCounter = 0; lineCounter < lines.Length; lineCounter++)
            {
                string currentLine = lines[lineCounter];
                if (currentLine.Contains("}") && endOfClassFound==false)
                {
                    endOfClassFound = true;
                    updateFileContent.Append("\t"); //Add in a tab or two for formatting purposes.
                    updateFileContent.Append("\t");
                    updateFileContent.AppendLine(GenerateCSharpStringVariable(CSharpVarName,RevitalizeId));
                    updateFileContent.AppendLine(currentLine);
                }
                else
                {
                    updateFileContent.AppendLine(currentLine);
                }
            }
            System.IO.File.WriteAllText(FilePath, updateFileContent.ToString());
        }

        /// <summary>
        /// Formats a string to be in acceptable variable format.
        /// </summary>
        /// <param name="VariableName"></param>
        /// <param name="VariableValue"></param>
        /// <returns></returns>
        private static string GenerateCSharpStringVariable(string VariableName, string VariableValue)
        {
            return string.Format("public const string {0} = \"{1}\";",VariableName,VariableValue);
        }

        /// <summary>
        /// Gets the base directory for all of the item ids for Revitalize.
        /// </summary>
        /// <returns></returns>
        private static string GetBaseIdsDirectory(bool StripDriveLetter=true)
        {
            return System.IO.Path.Combine(GetBaseCodeDirectory(StripDriveLetter), "Constants");
        }

        /// <summary>
        /// Sanitizes a display name into a c# variable name.
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <returns></returns>
        public static string SanitizeDisplayNameForCSharpVariableName(string DisplayName)
        {
            string[] splits = DisplayName.Split(' ');
            StringBuilder varName = new StringBuilder();
            foreach(string s in splits)
            {
                varName.Append(char.ToUpper(s[0]) + s.Substring(1));
            }
            return varName.ToString();
        }

        /// <summary>
        /// Gets the base directory for the code for the Revitalize mod.
        /// </summary>
        /// <returns></returns>
        private static string GetBaseCodeDirectory(bool StripDriveLetter=true)
        {
            return System.IO.Path.Combine(Game.GetRevitalizeBaseFolder(StripDriveLetter), "Framework");
        }
    }
}
