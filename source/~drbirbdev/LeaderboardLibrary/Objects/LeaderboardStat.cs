/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using BirbShared;

namespace LeaderboardLibrary
{
    [DynamoDBTable(TABLE_NAME)]
    public class LeaderboardStat : IComparable
    {
        internal const string TABLE_NAME = "StardewStats";
        internal const string TOP_SCORES_INDEX_NAME = "TopScores";

        [DynamoDBHashKey]
        public string Stat { get; set; }

        [DynamoDBRangeKey]
        public string UserUUID { get; set; }

        [DynamoDBLocalSecondaryIndexRangeKey(TOP_SCORES_INDEX_NAME)]
        public double Score { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Farm { get; set; }

        [DynamoDBProperty]
        public string Secret { get; set; }

        [DynamoDBProperty]
        public double DateTime { get; set; }

        public int CompareTo(object obj)
        {
            LeaderboardStat l = (LeaderboardStat)obj;
            return this.Score.CompareTo(l.Score);
        }

        public static LeaderboardStat FromDdbShape(Dictionary<string, AttributeValue> ddb)
        {
            if (!int.TryParse(ddb.GetValueOrDefault("Score", new AttributeValue()).N, out int score)) {
                Log.Warn("Failed to parse score from DDB document");
                score = 0;
            }
            if (!int.TryParse(ddb.GetValueOrDefault("DateTime", new AttributeValue()).S, out int time))
            {
                Log.Warn("Failed to parse time from DDB document");
                time = 0;
            }

            return new LeaderboardStat
            {
                Stat = ddb.GetValueOrDefault("Stat", new AttributeValue()).S,
                UserUUID = ddb.GetValueOrDefault("UserUUID", new AttributeValue()).S,
                Score = score,
                Name = ddb.GetValueOrDefault("Name", new AttributeValue()).S,
                Farm = ddb.GetValueOrDefault("Farm", new AttributeValue()).S,
                DateTime = time,
            };
        }

        public static List<LeaderboardStat> FromDdbList(List<Dictionary<string, AttributeValue>> ddbList)
        {
            List<LeaderboardStat> result = new();
            foreach (Dictionary<string, AttributeValue> ddbItem in ddbList)
            {
                result.Add(FromDdbShape(ddbItem));
            }
            return result;
        }

        public Dictionary<string, string> ToApiShape()
        {
            return new Dictionary<string, string>
            {
                {"Stat", this.Stat.Split(":")[1] },
                {"Name", this.Name },
                {"Farm", this.Farm },
                {"Score", this.Score.ToString() },
                {"DateTime", this.DateTime.ToString() },
                {"UserUUID", this.UserUUID },
            };
        }

        public static List<Dictionary<string, string>> ToApiList(List<LeaderboardStat> leaderboardStats)
        {
            List<Dictionary<string, string>> result = new();
            foreach (LeaderboardStat leaderboard in leaderboardStats)
            {
                result.Add(leaderboard.ToApiShape());
            }
            return result;
        }
    }
}
