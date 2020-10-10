/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Denifia.Stardew.SendItemsFunctions
{
    public class Mail : TableEntity
    {
        public const string EntityPartitionKey = "mail";

        public Mail()
        {

        }

        public Mail(string id)
        {
            RowKey = id;
            PartitionKey = EntityPartitionKey;
        }

        public string ToFarmerId { get; set; }
        public string FromFarmerId { get; set; }
        public string Text { get; set; }
        public DateTime ClientCreatedDate { get; set; }
        public DateTime ServerCreatedDate { get; set; }
    }
}
