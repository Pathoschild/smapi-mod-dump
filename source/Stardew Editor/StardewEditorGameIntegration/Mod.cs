/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewEditor
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.Net.Sockets;

namespace StardewEditorGameIntegration
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        internal Connection conn;

        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);
            instance = this;

            Command.register("editor_connect", startConnection);
        }

        private void startConnection( string[] args )
        {
            try
            {
                conn = new Connection();
            }
            catch (Exception e)
            {
                Log.error("Failed to connect to editor! " + e);
            }
        }
    }
}
