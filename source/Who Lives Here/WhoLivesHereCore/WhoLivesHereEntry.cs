/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using Prism99_Core.Utilities;
using System;
using WhoLivesHere.GMCM;
using WhoLivesHereCore;
using System.IO;

namespace WhoLivesHere
{
    /// <summary>
    /// Mod entry point
    /// </summary>
    internal class WhoLivesHereEntry : Mod
    {
        public IModHelper Helper ;
        private SDVLogger logger;
        private WhoLivesHereConfig config;
        private WhoLivesHereLogic whoLogic;
        public override void Entry(IModHelper helper)
        {

            logger = new SDVLogger(Monitor, helper.DirectoryPath, helper);
            Helper=helper;
            config = helper.ReadConfig<WhoLivesHereConfig>();

            CleanUp();

            GMCM_Integration.Initialize(helper, ModManifest, config);

            whoLogic = new WhoLivesHereLogic();
            whoLogic.Initialize(helper, logger,config);
        }
        private void CleanUp()
        {
            //
            //  remove old external debug file
            //
            if(File.Exists(Path.Combine(Helper.DirectoryPath, "WhoLivesHere.pdb")))
            {
                try
                {
                    File.Delete(Path.Combine(Helper.DirectoryPath, "WhoLivesHere.pdb"));
                }
                catch { }
            }
        }
    }
}
