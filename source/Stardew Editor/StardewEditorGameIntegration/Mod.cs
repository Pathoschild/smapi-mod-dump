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
