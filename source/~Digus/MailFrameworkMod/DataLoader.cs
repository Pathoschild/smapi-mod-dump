using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod.ContentPack;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;

namespace MailFrameworkMod
{
    public class DataLoader : IAssetEditor
    {
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (Letter letter in MailDao.GetSavedLetters())
            {
                if (letter.Title != null)
                {
                    data[letter.Id] = letter.Text + "[#]" + letter.Title;
                }
            }
            foreach (string letterId in MailDao.GetRemovedLetterIds())
            {
                if (data.ContainsKey(letterId))
                {
                    data.Remove(letterId);
                }
            }
            MailDao.CleanDataToUpdate();
        }

        public static void LoadContentPacks(object sender, EventArgs e)
        {
            foreach (IContentPack contentPack in MailFrameworkModEntry.ModHelper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, "mail.json")))
                {
                    MailFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<MailItem> mailItems = contentPack.ReadJsonFile<List<MailItem>>("mail.json");
                    foreach (MailItem mailItem in mailItems)
                    {
                        bool Condition(Letter l) => 
                            (!Game1.player.mailReceived.Contains(l.Id) || mailItem.Repeatable)
                            && (mailItem.Recipe == null || !Game1.player.cookingRecipes.ContainsKey(mailItem.Recipe))
                            && (mailItem.Date == null || SDate.Now() >= new SDate(Convert.ToInt32(mailItem.Date.Split(' ')[0]), mailItem.Date.Split(' ')[1], Convert.ToInt32(mailItem.Date.Split(' ')[2].Replace("Y", ""))))
                            && (mailItem.Days == null || mailItem.Days.Contains(SDate.Now().Day))
                            && (mailItem.Seasons == null || mailItem.Seasons.Contains(SDate.Now().Season))
                            && (mailItem.Weather == null || (Game1.isRaining && "rainy".Equals(mailItem.Weather)) || (!Game1.isRaining && "sunny".Equals(mailItem.Weather)))
                            && (mailItem.FriendshipConditions == null || mailItem.FriendshipConditions.TrueForAll(f => Game1.player.getFriendshipHeartLevelForNPC(f.NpcName) >= f.FriendshipLevel))
                            && (mailItem.SkillConditions == null || mailItem.SkillConditions.TrueForAll(s => Game1.player.getEffectiveSkillLevel((int)s.SkillName) >= s.SkillLevel))
                            && (mailItem.RandomChance == null || new Random((int)(((ulong)Game1.stats.DaysPlayed * 1000000000000000) + (((ulong)l.Id.GetHashCode()) % 1000000000 * 1000000) + Game1.uniqueIDForThisGame % 1000000)).NextDouble() < mailItem.RandomChance)
                        ;
                        

                        if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
                        {
                            List<Item> attachments = new List<Item>();
                            Dictionary<int, string> objects = null;
                            Dictionary<int, string> bigObjects = null;
                            mailItem.Attachments.ForEach(i =>
                            {
                                switch (i.Type)
                                {
                                    case ItemType.Object:
                                        if (i.Name != null)
                                        {
                                            if (objects == null) objects = MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                                            KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No object found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }
                                        
                                        if (i.Index.HasValue)
                                        {
                                            attachments.Add(new StardewValley.Object(Vector2.Zero, i.Index.Value, i.Stack ?? 1));
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach an object for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.BigObject:
                                    case ItemType.BigCraftable:
                                        if (i.Name != null)
                                        {
                                            if (bigObjects == null) bigObjects = MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", ContentSource.GameContent);
                                            KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No big craftable found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }

                                        if (i.Index.HasValue)
                                        {
                                            Item item = new StardewValley.Object(Vector2.Zero, i.Index.Value);
                                            if (i.Stack.HasValue)
                                            {
                                                item.Stack = i.Stack.Value;
                                            }
                                            attachments.Add(item);
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a big craftable for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.Tool:
                                        switch (i.Name)
                                        {
                                            case "Axe":
                                                attachments.Add(new Axe());
                                                break;
                                            case "Hoe":
                                                attachments.Add(new Hoe());
                                                break;
                                            case "Watering Can":
                                                attachments.Add(new WateringCan());
                                                break;
                                            case "Scythe":
                                                attachments.Add(new MeleeWeapon(47));
                                                break;
                                            case "Pickaxe":
                                                attachments.Add(new Pickaxe());
                                                break;
                                            default:
                                                MailFrameworkModEntry.ModMonitor.Log($"Tool with name {i.Name} not found for letter {mailItem.Id}.",LogLevel.Warn);
                                                break;
                                        }
                                        break;
                                }

                                
                            });
                            MailDao.SaveLetter(
                                new Letter(
                                    mailItem.Id
                                    , mailItem.Text
                                    , attachments
                                    , Condition
                                    , (l) => Game1.player.mailReceived.Add(l.Id)
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = mailItem.Title,
                                    GroupId = mailItem.GroupId
                                });
                        }
                        else
                        {
                            MailDao.SaveLetter(
                                new Letter(
                                    mailItem.Id
                                    , mailItem.Text
                                    , mailItem.Recipe
                                    , Condition
                                    , (l) => Game1.player.mailReceived.Add(l.Id)
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = mailItem.Title,
                                    GroupId = mailItem.GroupId
                                });
                        }
                    }
                }
                else
                {
                    MailFrameworkModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an mail.json file.", LogLevel.Warn);
                }
            }
        }
    }
}
