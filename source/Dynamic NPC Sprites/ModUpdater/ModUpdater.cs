using System;
using System.IO;
using System.Net;
using StardewModdingAPI;
using ICSharpCode.SharpZipLib.Zip;

namespace ModUpdater
{
    public class ModUpdater : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var saveData = "Test";
            this.Helper.WriteJsonFile($"aoeu.json", saveData);

            string URL = "https://github.com/miketweaver/DailyNews/releases/download/v1.1/DailyNews-v1.1.zip";
            string DestinationPath = Path.Combine(this.Helper.DirectoryPath, "DailyNews.zip");

            System.Net.WebClient Client = new WebClient();
            Client.DownloadFile(URL, DestinationPath);



			var zipFileName = DestinationPath;
			var targetDir = Path.Combine(this.Helper.DirectoryPath, "unzip");
			FastZip fastZip = new FastZip();
			string fileFilter = null;

			// Will always overwrite if target filenames already exist
			fastZip.ExtractZip(zipFileName, targetDir, fileFilter);

        }
    }
}
