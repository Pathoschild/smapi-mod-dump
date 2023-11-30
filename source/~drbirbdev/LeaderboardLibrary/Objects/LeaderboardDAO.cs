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
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BirbCore.Attributes;

namespace LeaderboardLibrary;

class LeaderboardDAO
{

    /// <summary>
    /// Get the local scores.  These can be cached for a variety of APIs.
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    public static async Task<List<LeaderboardStat>> GetLocalScores(string stat)
    {
        try
        {
            Log.Debug($"Getting local scores from DDB for {stat}");
            List<Dictionary<string, AttributeValue>> keys = new List<Dictionary<string, AttributeValue>>();
            foreach (string playerUuid in ModEntry.LocalModData.MultiplayerUUIDs)
            {
                keys.Add(new Dictionary<string, AttributeValue>
            {
                {"Stat", new AttributeValue { S = stat } },
                {"UserUUID", new AttributeValue { S = playerUuid } },
            });
            }

            BatchGetItemRequest request = new BatchGetItemRequest()
            {
                RequestItems = new Dictionary<string, KeysAndAttributes>
            {
                { LeaderboardStat.TABLE_NAME, new KeysAndAttributes()
                {
                    Keys = keys,
                    ProjectionExpression = "Stat, UserUUID, Score, #n, Farm, #d",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#n", "Name" },
                        {"#d", "DateTime" },
                    },
                }
                }
            }

            };
            BatchGetItemResponse response = await ModEntry.DdbClient.BatchGetItemAsync(request);

            return LeaderboardStat.FromDdbList(response.Responses[LeaderboardStat.TABLE_NAME]);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to retrieve local scores\n" +
                $"Stat : {stat}\n");
            Log.Error(e.Message);

            return null;
        }

    }

    /// <summary>
    /// Get the global top scores.  These can be cached for the GetTopN API.
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    public static async Task<List<LeaderboardStat>> GetTopScores(string stat)
    {
        try
        {
            Log.Debug($"Getting global scores from DDB for {stat}");
            QueryRequest request = new QueryRequest()
            {
                TableName = LeaderboardStat.TABLE_NAME,
                IndexName = LeaderboardStat.TOP_SCORES_INDEX_NAME,
                KeyConditionExpression = "Stat = :stat",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":stat", new AttributeValue { S = stat } },
            },
                Limit = 10,
                ProjectionExpression = "Stat, UserUUID, Score, #n, Farm, #d",
                ExpressionAttributeNames = new Dictionary<string, string>
            {
                {"#n", "Name" },
                {"#d", "DateTime" },
            },
                ScanIndexForward = false,
            };
            QueryResponse response = await ModEntry.DdbClient.QueryAsync(request);

            return LeaderboardStat.FromDdbList(response.Items);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to retrieve global scores\n" +
                $"Stat : {stat}\n");
            Log.Error(e.Message);

            return null;
        }
    }

    /// <summary>
    /// Upload a score for the current player/farm for a given stat.
    /// Upload is conditional on a provided 'secret'.  This keeps hackers from updating another players score.
    /// Scores can only be monotonically increasing.  Trying to lower a score will fail.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="score"></param>
    public static async void UploadScore(string stat, int score, string userUuid, string userName, string farmName, string secret, CachedLeaderboardAPI api)
    {
        try
        {
            Log.Debug($"Uploading score ({score}) to DDB for {stat}");
            UpdateItemRequest request = new UpdateItemRequest()
            {
                TableName = LeaderboardStat.TABLE_NAME,
                Key = new Dictionary<string, AttributeValue>
            {
                { "Stat", new AttributeValue { S = stat } },
                { "UserUUID", new AttributeValue { S = userUuid } },
            },
                // TODO: Get condition expression working with secrets
                ConditionExpression = "attribute_not_exists(Score) OR Score < :score",
                UpdateExpression = "SET Score = :score, #n = :name, Farm = :farm, Secret = :secret, #d = :datetime",
                ExpressionAttributeNames = new Dictionary<string, string>
            {
                {"#n", "Name" },
                {"#d", "DateTime" },
            },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":score", new AttributeValue { N = score.ToString() } },
                { ":name", new AttributeValue { S = userName } },
                { ":farm", new AttributeValue { S = farmName } },
                { ":secret", new AttributeValue { S = secret } },
                { ":datetime", new AttributeValue { S = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() } },
            },
                ReturnValues = ReturnValue.NONE,
            };
            await ModEntry.DdbClient.UpdateItemAsync(request);
        }
        catch (ConditionalCheckFailedException e)
        {
            // If the condition expression failed, refresh the cache!
            Log.Error($"DDB condition check failed {e}...Refreshing cache!");
            api.RefreshCache(stat);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to upload score.  Please share this error message with the mod author!\n" +
                $"Score: {score}\n" +
                $"Stat : {stat}\n" +
                $"Name : {userName}\n" +
                $"Farm : {farmName}\n" +
                $"UUID : {userUuid}\n");
            Log.Error(e.Message);
        }
    }
}
