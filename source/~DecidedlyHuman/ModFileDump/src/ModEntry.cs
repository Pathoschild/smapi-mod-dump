/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using StardewModdingAPI;

namespace ModFileDump;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        string modPath = helper.DirectoryPath;
        DirectoryInfo gameModPath = new DirectoryInfo(modPath).Parent;
        DirectoryInfo gameContentPath = gameModPath.Parent.Parent;

        if (Directory.Exists(gameModPath.FullName))
        {
            FileInfo[] files = gameModPath.GetFiles("*", SearchOption.AllDirectories);
            string destinationDirectorySuffix =
                $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}-{DateTime.Now.Millisecond}";
            string destinationDirectory =
                Path.Combine(gameModPath.Parent.FullName, $"Mod Dump - {destinationDirectorySuffix}");

            this.CopyFolder(gameModPath.FullName, destinationDirectory);
        }
    }

    private void CopyFolder(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }

        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            string dest = Path.Combine(destFolder, name);
            this.CopyFolder(folder, dest);
        }
    }
}
