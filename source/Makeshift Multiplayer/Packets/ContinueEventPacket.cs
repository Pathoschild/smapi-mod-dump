/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley;
using System.IO;
using System.Linq;

namespace StardewValleyMP.Packets
{
    // Client <-> Server
    // Make the festival continue for everyone
    public class ContinueEventPacket : Packet
    {
        public ContinueEventPacket()
            : base(ID.ContinueEvent)
        {
        }

        protected override void read(BinaryReader reader)
        {
        }

        protected override void write(BinaryWriter writer)
        {
        }

        public override void process(Client client)
        {
            process();
        }

        public override void process(Server server, Server.Client client)
        {
            process();
            server.broadcast(this, client.id);
        }

        private void process()
        {
            if (Game1.currentLocation != null && Game1.currentLocation.currentEvent != null)
            {
                Event @event = Game1.currentLocation.currentEvent;
                @event.forceFestivalContinue();
                Events.prevCommand = @event.currentCommand;
                Events.prevCommandCount = @event.eventCommands.Count();
            }
        }
    }
}
