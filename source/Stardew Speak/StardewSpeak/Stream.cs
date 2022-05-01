/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSpeak
{

    public class Stream
    {
        public string Name;
        public string Id;
        public dynamic Data;
        public Stream(string name, string id, dynamic streamData) 
        {
            this.Name = name;
            this.Id = id;
            this.Data = streamData;
        }
        public static List<dynamic> MessageStreams(Dictionary<string, Stream> streams, string streamName, dynamic messageValue) 
        {
            var messages = new List<dynamic>();
            foreach (var pair in streams)
            {
                var id = pair.Key;
                var stream = pair.Value;
                if (stream.Name == streamName)
                {
                    var message = new { stream_id = id, value = messageValue };
                    messages.Add(message);
                    //this.speechEngine.SendMessage("STREAM_MESSAGE", message);
                }
            }
            return messages;
        }

    }
}
