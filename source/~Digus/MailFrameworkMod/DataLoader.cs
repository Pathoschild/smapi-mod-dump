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
using MailFrameworkMod.integrations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace MailFrameworkMod
{
    public class DataLoader
    {
        internal const string MailAssetName = "Data/mail";
        public static Dictionary<Tuple<string,string>, Texture2D> _contentPackAssets = new Dictionary<Tuple<string, string>, Texture2D>();

        private static readonly List<string> NoUpgradeLevelTools = new List<string>() {"Scythe", "Shears", "Milk Pail", "Fishing Rod", "Golden Scythe", "Pan", "Return Scepter" };
        private static readonly List<int> SlingshotIndexes = new List<int>() {32, 33, 34};

        public static IModHelper Helper;
        public static IDynamicGameAssetsApi DgaApi;
        public static IConditionsChecker ConditionsCheckerApi;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            Helper.Events.Content.AssetRequested += this.Edit;

            DgaApi = MailFrameworkModEntry.ModHelper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            ConditionsCheckerApi = Helper.ModRegistry.GetApi<IConditionsChecker>("Cherry.ExpandedPreconditionsUtility");
            ConditionsCheckerApi?.Initialize(false, MailFrameworkModEntry.Manifest.UniqueID);
        }
        
        public void Edit(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(MailAssetName))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (Letter letter in MailDao.GetSavedLetters())
                    {
                        if (letter.Title != null && !letter.AutoOpen)
                        {
                            data[letter.Id] = letter.TranslatedText + "[#]" + letter.TranslatedTitle;
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
                });
            }
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

                        //Populate all Indexes based on the given name. Ignore the letter otherwise.
                        if (mailItem.CollectionConditions != null && mailItem.CollectionConditions.Any(c =>
                        {
                            if (c.Name != null && c.Collection != Collection.Crafting)
                            {
                                objects ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/ObjectInformation"));
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
                        })) continue;

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
                                    || (c.Collection == Collection.Crafting && Game1.player.craftingRecipes.ContainsKey(c.Name) && Game1.player.craftingRecipes[c.Name] >= c.Amount)
                                    )))
                            && (mailItem.RandomChance == null || new Random((int)(((ulong)Game1.stats.DaysPlayed * 1000000000000000) + (((ulong)l.Id.GetHashCode()) % 1000000000 * 1000000) + Game1.uniqueIDForThisGame % 1000000)).NextDouble() < mailItem.RandomChance)
                            && (mailItem.Buildings == null || (mailItem.RequireAllBuildings ? mailItem.Buildings.TrueForAll(b=> Game1.getFarm().isBuildingConstructed(b)) : mailItem.Buildings.Any(b => Game1.getFarm().isBuildingConstructed(b))))
                            && (mailItem.MailReceived == null || (mailItem.RequireAllMailReceived ? !mailItem.MailReceived.Except(Game1.player.mailReceived).Any() : mailItem.MailReceived.Intersect(Game1.player.mailReceived).Any()))
                            && (mailItem.MailNotReceived == null ||  !mailItem.MailNotReceived.Intersect(Game1.player.mailReceived).Any())
                            && (mailItem.EventsSeen == null || (mailItem.RequireAllEventsSeen ? !mailItem.EventsSeen.Except(Game1.player.eventsSeen).Any() : mailItem.EventsSeen.Intersect(Game1.player.eventsSeen).Any()))
                            && (mailItem.EventsNotSeen == null ||  !mailItem.EventsNotSeen.Intersect(Game1.player.eventsSeen).Any())
                            && (mailItem.RecipeKnown == null || (mailItem.RequireAllRecipeKnown ? mailItem.RecipeKnown.All(r=> Game1.player.knowsRecipe(r)) : mailItem.RecipeKnown.Any(r => Game1.player.knowsRecipe(r))))
                            && (mailItem.RecipeNotKnown == null || mailItem.RecipeNotKnown.All(r=>!Game1.player.knowsRecipe(r)))
                            && (mailItem.ExpandedPrecondition == null || (ConditionsCheckerApi != null && ConditionsCheckerApi.CheckConditions(mailItem.ExpandedPrecondition)))
                            && (mailItem.ExpandedPreconditions == null || (ConditionsCheckerApi != null && ConditionsCheckerApi.CheckConditions(mailItem.ExpandedPreconditions)))
                            && (mailItem.HouseUpgradeLevel == null || mailItem.HouseUpgradeLevel <= Game1.player.HouseUpgradeLevel)
                            && (mailItem.DeepestMineLevel == null || mailItem.DeepestMineLevel <= Game1.player.deepestMineLevel)
                            && (mailItem.CurrentMoney == null || mailItem.CurrentMoney <= Game1.player.Money)
                            && (mailItem.TotalMoneyEarned == null || mailItem.TotalMoneyEarned <= Game1.player.totalMoneyEarned)
                        ;

                        if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
                        {
                            List<Item> attachments = new List<Item>();
                            mailItem.Attachments.ForEach(i =>
                            {
                                if (i == null) return;
                                switch (i.Type)
                                {
                                    case ItemType.Object:
                                        if (i.Name != null)
                                        {
                                            objects ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/ObjectInformation"));
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
                                            bigObjects ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/BigCraftablesInformation"));
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
                                            case "Golden Scythe":
                                                tool = new MeleeWeapon(53);
                                                break;
                                            case "Pickaxe":
                                                tool = new Pickaxe();
                                                break;
                                            case "Milk Pail":
                                                tool = new MilkPail();
                                                break;
                                            case "Shears":
                                                tool = new Shears();
                                                break;
                                            case "Fishing Rod":
                                                tool = new FishingRod(i.UpgradeLevel);
                                                break;
                                            case "Pan":
                                                tool = new Pan();
                                                break;
                                            case "Return Scepter":
                                                tool = new Wand();
                                                break;
                                            default:
                                                MailFrameworkModEntry.ModMonitor.Log($"Tool with name {i.Name} not found for letter {mailItem.Id}.",LogLevel.Warn);
                                                break;
                                        }
                                        if (tool != null)
                                        {
                                            if (!NoUpgradeLevelTools.Contains(i.Name))
                                            {
                                                tool.UpgradeLevel = i.UpgradeLevel;
                                            }
                                            attachments.Add(tool);
                                        }
                                        break;
                                    case ItemType.Ring:
                                        objects ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/ObjectInformation"));
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
                                            furnitures ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Furniture"));
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
                                            weapons ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Weapons"));
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
                                            int index = i.Index.Value;
                                            attachments.Add(SlingshotIndexes.Contains(index) ? (Item)new Slingshot(index) : (Item)new MeleeWeapon(index));
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"An index value is required to attach a weapon for letter {mailItem.Id}.", LogLevel.Warn);
                                        }
                                        break;
                                    case ItemType.Boots:
                                        if (i.Name != null)
                                        {
                                            boots ??= MailFrameworkModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Boots"));
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
                                    case ItemType.DGA:
                                        if (DgaApi != null)
                                        {
                                            try
                                            {
                                                object dgaObject = DgaApi.SpawnDGAItem(i.Name);
                                                if (dgaObject is StardewValley.Item dgaItem)
                                                {
                                                    if (dgaItem is StardewValley.Object)
                                                    {
                                                        dgaItem.Stack = i.Stack ?? 1;
                                                    }
                                                    else
                                                    {
                                                        dgaItem.Stack = 1;
                                                    }

                                                    attachments.Add(dgaItem);
                                                }
                                                else
                                                {
                                                    MailFrameworkModEntry.ModMonitor.Log($"No DGA item found with the ID {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                MailFrameworkModEntry.ModMonitor.Log($"Error trying to create item with the DGA ID {i.Name} for letter {mailItem.Id}.", LogLevel.Warn);
                                                MailFrameworkModEntry.ModMonitor.Log(ex.Message, LogLevel.Trace);
                                            }
                                        }
                                        else
                                        {
                                            MailFrameworkModEntry.ModMonitor.Log($"No DGA API found, so item with the ID {i.Name} for letter {mailItem.Id} will be ignored.", LogLevel.Warn);
                                        }
                                        break;
                                    default:
                                        MailFrameworkModEntry.ModMonitor.Log($"Invalid attachment type '{i.Type}' found in letter {mailItem.Id}.", LogLevel.Warn);
                                        break;
                                }
                            });
                            MailDao.SaveLetter(
                                new Letter(
                                    mailItem.Id
                                    , mailItem.Text
                                    , attachments
                                    , Condition
                                    , (l) =>
                                    {
                                        Game1.player.mailReceived.Add(l.Id);
                                        if (mailItem.AdditionalMailReceived != null) Game1.player.mailReceived.AddRange(mailItem.AdditionalMailReceived);
                                    }
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = mailItem.Title,
                                    GroupId = mailItem.GroupId,
                                    LetterTexture = mailItem.LetterBG != null ? GetTextureAsset(contentPack, mailItem.LetterBG) : null,
                                    UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null ? GetTextureAsset(contentPack, mailItem.UpperRightCloseButton) : null,
                                    AutoOpen = mailItem.AutoOpen,
                                    I18N = hasTranslation ? contentPack.Translation : null
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
                                    , (l) =>
                                    {
                                        Game1.player.mailReceived.Add(l.Id);
                                        if (mailItem.AdditionalMailReceived != null) Game1.player.mailReceived.AddRange(mailItem.AdditionalMailReceived);
                                    }
                                    , mailItem.WhichBG
                                )
                                {
                                    TextColor = mailItem.TextColor,
                                    Title = mailItem.Title,
                                    GroupId = mailItem.GroupId,
                                    LetterTexture = mailItem.LetterBG != null ? GetTextureAsset(contentPack, mailItem.LetterBG) : null,
                                    UpperRightCloseButtonTexture = mailItem.UpperRightCloseButton != null ? GetTextureAsset(contentPack, mailItem.UpperRightCloseButton) : null,
                                    AutoOpen = mailItem.AutoOpen,
                                    I18N = hasTranslation ? contentPack.Translation : null
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

        public static Texture2D GetTextureAsset(IContentPack contentPack, string textureName)
        {
            var key = new Tuple<string, string>(contentPack.Manifest.UniqueID, textureName);
            if (!_contentPackAssets.ContainsKey(key))
            {
                _contentPackAssets[key] = contentPack.ModContent.Load<Texture2D>(textureName);
            }
            return _contentPackAssets[key];
        }
    }
}
