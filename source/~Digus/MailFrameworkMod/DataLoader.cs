/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod.ContentPack;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
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
                    bool hasTranslation = contentPack.Translation.GetTranslations().Any();

                    MailFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<MailItem> mailItems = contentPack.ReadJsonFile<List<MailItem>>("mail.json");
                    foreach (MailItem mailItem in mailItems)
                    {
                        Dictionary<int, string> objects = null;
                        Dictionary<int, string> bigObjects = null;
                        Dictionary<int, string> furnitures = null;
                        Dictionary<int, string> weapons = null;
                        Dictionary<int, string> boots = null;

                        //Populate all Indexs based on the given name. Ignore the letter otherwise.
                        if (mailItem.CollectionConditions != null && mailItem.CollectionConditions.Any(c =>
                        {
                            if (c.Name != null)
                            {
                                objects = objects ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(c.Name + "/"));
                                if (pair.Value != null)
                                {
                                    c.Index = pair.Key;
                                }
                                else
                                {
                                    MailFrameworkModEntry.ModMonitor.Log($"No object found with the name '{c.Name}' for a condition for letter '{mailItem.Id}'.\n This letter will be ignored.", LogLevel.Warn);
                                    MailDao.RemoveLetter(new Letter(mailItem.Id,null,null));
                                    return true;
                                }
                            }
                            return false;
                        })) break;

                        bool Condition(Letter l) => 
                            (!Game1.player.mailReceived.Contains(l.Id) || mailItem.Repeatable)
                            && (mailItem.Recipe == null || !Game1.player.cookingRecipes.ContainsKey(mailItem.Recipe))
                            && (mailItem.Date == null || SDate.Now() >= new SDate(Convert.ToInt32(mailItem.Date.Split(' ')[0]), mailItem.Date.Split(' ')[1], Convert.ToInt32(mailItem.Date.Split(' ')[2].Replace("Y", ""))))
                            && (mailItem.Days == null || mailItem.Days.Contains(SDate.Now().Day))
                            && (mailItem.Seasons == null || mailItem.Seasons.Contains(SDate.Now().Season))
                            && (mailItem.Weather == null || (Game1.isRaining && "rainy".Equals(mailItem.Weather)) || (!Game1.isRaining && "sunny".Equals(mailItem.Weather)))
                            && (mailItem.FriendshipConditions == null || (mailItem.FriendshipConditions.TrueForAll(f => Game1.player.getFriendshipHeartLevelForNPC(f.NpcName) >= f.FriendshipLevel)) 
                                && mailItem.FriendshipConditions.TrueForAll(f => f.FriendshipStatus == null || (Game1.player.friendshipData.ContainsKey(f.NpcName) && f.FriendshipStatus.Any(s => s == Game1.player.friendshipData[f.NpcName].Status))))
                            && (mailItem.SkillConditions == null || mailItem.SkillConditions.TrueForAll(s => Game1.player.getEffectiveSkillLevel((int)s.SkillName) >= s.SkillLevel))
                            && (mailItem.StatsConditions == null || (mailItem.StatsConditions.TrueForAll(s => s.StatsLabel == null || Game1.player.stats.getStat(s.StatsLabel) >= s.Amount) && mailItem.StatsConditions.TrueForAll(s => s.StatsName == null || MailFrameworkModEntry.ModHelper.Reflection.GetProperty<uint>(Game1.player.stats,s.StatsName.ToString()).GetValue() >= s.Amount)))
                            && (mailItem.CollectionConditions == null || (mailItem.CollectionConditions.TrueForAll(c => 
                                    (c.Collection == Collection.Shipped && Game1.player.basicShipped.ContainsKey(c.Index) && Game1.player.basicShipped[c.Index] >= c.Amount)
                                    || (c.Collection == Collection.Fish && Game1.player.fishCaught.ContainsKey(c.Index) && Game1.player.fishCaught[c.Index][0] >= c.Amount)
                                    || (c.Collection == Collection.Artifacts && Game1.player.archaeologyFound.ContainsKey(c.Index) && Game1.player.archaeologyFound[c.Index][0] >= c.Amount)
                                    || (c.Collection == Collection.Minerals && Game1.player.mineralsFound.ContainsKey(c.Index) && Game1.player.mineralsFound[c.Index] >= c.Amount)
                                    || (c.Collection == Collection.Cooking && Game1.player.recipesCooked.ContainsKey(c.Index) && Game1.player.recipesCooked[c.Index] >= c.Amount)
                                    )))
                            && (mailItem.RandomChance == null || new Random((int)(((ulong)Game1.stats.DaysPlayed * 1000000000000000) + (((ulong)l.Id.GetHashCode()) % 1000000000 * 1000000) + Game1.uniqueIDForThisGame % 1000000)).NextDouble() < mailItem.RandomChance)
                            && (mailItem.Buildings == null || (mailItem.RequireAllBuildings ? mailItem.Buildings.TrueForAll(b=> Game1.getFarm().isBuildingConstructed(b)) : mailItem.Buildings.Any(b => Game1.getFarm().isBuildingConstructed(b))))
                            && (mailItem.MailReceived == null || (mailItem.RequireAllMailReceived ? !mailItem.MailReceived.Except(Game1.player.mailReceived).Any() : mailItem.MailReceived.Intersect(Game1.player.mailReceived).Any()))
                            && (mailItem.MailNotReceived == null ||  !mailItem.MailNotReceived.Intersect(Game1.player.mailReceived).Any())
                            && (mailItem.EventsSeen == null || (mailItem.RequireAllEventsSeen ? !mailItem.EventsSeen.Except(Game1.player.eventsSeen).Any() : mailItem.EventsSeen.Intersect(Game1.player.eventsSeen).Any()))
                            && (mailItem.EventsNotSeen == null ||  !mailItem.EventsNotSeen.Intersect(Game1.player.eventsSeen).Any())
                            && (mailItem.RecipeKnown == null || (mailItem.RequireAllRecipeKnown ? mailItem.RecipeKnown.All(r=> Game1.player.knowsRecipe(r)) : mailItem.RecipeKnown.Any(r => Game1.player.knowsRecipe(r))))
                            && (mailItem.RecipeNotKnown == null || mailItem.RecipeNotKnown.All(r=>!Game1.player.knowsRecipe(r)))
                        ;

                        if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
                        {
                            List<Item> attachments = new List<Item>();
                            mailItem.Attachments.ForEach(i =>
                            {
                                switch (i.Type)
                                {
                                    case ItemType.Object:
                                        if (i.Name != null)
                                        {
                                            objects = objects ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
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
                                            bigObjects = bigObjects ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", ContentSource.GameContent);
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
                                        Tool tool = null;
                                        switch (i.Name)
                                        {
                                            case "Axe":
                                                tool = new Axe();
                                                break;
                                            case "Hoe":
                                                tool = new Hoe();
                                                break;
                                            case "Watering Can":
                                                tool = new WateringCan();
                                                break;
                                            case "Scythe":
                                                tool = new MeleeWeapon(47);
                                                break;
                                            case "Pickaxe":
                                                tool = new Pickaxe();
                                                break;
                                            default:
                                                MailFrameworkModEntry.ModMonitor.Log($"Tool with name {i.Name} not found for letter {mailItem.Id}.",LogLevel.Warn);
                                                break;
                                        }
                                        if (tool != null)
                                        {
                                            if (i.Name != "Scythe")
                                            {
                                                tool.UpgradeLevel = i.UpgradeLevel;
                                            }
                                            attachments.Add(tool);
                                        }
                                        break;
                                    case ItemType.Ring:
                                        objects = objects ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                                        if (i.Name != null)
                                        {
                                            KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No ring found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }
                                        if (i.Index.HasValue)
                                        {
                                            if (objects[i.Index.Value].Split('/')[3] == "Ring")
                                            {
                                                attachments.Add(new Ring(i.Index.Value));
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"A valid ring is required to attach an ring for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach an ring for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.Furniture:
                                        if (i.Name != null)
                                        {
                                            furnitures = furnitures ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\Furniture", ContentSource.GameContent);
                                            KeyValuePair<int, string> pair = furnitures.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No furniture found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }

                                        if (i.Index.HasValue)
                                        {
                                            attachments.Add(Furniture.GetFurnitureInstance(i.Index.Value));
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a furniture for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.Weapon:
                                        if (i.Name != null)
                                        {
                                            weapons = weapons ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\Weapons", ContentSource.GameContent);
                                            KeyValuePair<int, string> pair = weapons.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No weapon found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }

                                        if (i.Index.HasValue)
                                        {
                                            attachments.Add(new MeleeWeapon(i.Index.Value));
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a weapon for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.Boots:
                                        if (i.Name != null)
                                        {
                                            boots = boots ?? MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<int, string>>("Data\\Boots", ContentSource.GameContent);
                                            KeyValuePair<int, string> pair = boots.FirstOrDefault(o => o.Value.StartsWith(i.Name + "/"));
                                            if (pair.Value != null)
                                            {
                                                i.Index = pair.Key;
                                            }
                                            else
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"No boots found with the name {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                            }
                                        }

                                        if (i.Index.HasValue)
                                        {
                                            attachments.Add(new Boots(i.Index.Value));
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a boots for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                }
                            });
                            MailDao.SaveLetter(
                                new Letter(
                                    mailItem.Id
                                    , hasTranslation? contentPack.Translation.Get(mailItem.Text) : mailItem.Text
                                    , attachments
                                    , Condition
                                    , (l) => Game1.player.mailReceived.Add(l.Id)
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = hasTranslation && mailItem.Title != null ? contentPack.Translation.Get(mailItem.Title) : mailItem.Title,
                                    GroupId = mailItem.GroupId,
                                    LetterTexture = mailItem.LetterBG != null ? contentPack.LoadAsset<Texture2D>(mailItem.LetterBG) : null,
                                    UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null ? contentPack.LoadAsset<Texture2D>(mailItem.UpperRightCloseButton) : null,
                                });
                        }
                        else
                        {
                            MailDao.SaveLetter(
                                new Letter(
                                    mailItem.Id
                                    , hasTranslation ? contentPack.Translation.Get(mailItem.Text) : mailItem.Text
                                    , mailItem.Recipe
                                    , Condition
                                    , (l) => Game1.player.mailReceived.Add(l.Id)
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = hasTranslation && mailItem.Title != null ? contentPack.Translation.Get(mailItem.Title) : mailItem.Title,
                                    GroupId = mailItem.GroupId,
                                    LetterTexture = mailItem.LetterBG != null ? contentPack.LoadAsset<Texture2D>(mailItem.LetterBG) : null,
                                    UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null ? contentPack.LoadAsset<Texture2D>(mailItem.UpperRightCloseButton) : null,
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
