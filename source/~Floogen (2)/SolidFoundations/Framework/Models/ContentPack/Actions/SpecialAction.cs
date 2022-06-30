/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using SolidFoundations.Framework.Interfaces.Internal;
using SolidFoundations.Framework.UI;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class SpecialAction
    {
        public enum BuffType
        {
            Unknown = -1,
            Farming = 0,
            Fishing = 1,
            Mining = 2,
            Luck = 4,
            Foraging = 5,
            Stamina = 7,
            MagneticRadius = 8,
            Speed = 9,
            Defense = 10,
            Attack = 11,
            Frozen = 19,
            YobaBlessing = 21,
            Nauseous = 25,
            Darkness = 26
        }

        public enum MessageType
        {
            Achievement,
            Quest,
            Error,
            Stamina,
            Health
        }

        public enum FlagType
        {
            Temporary, // Removed on a new day start
            Permanent
        }

        public enum OperationName
        {
            Add,
            Remove
        }

        public enum StoreType
        {
            Vanilla,
            STF
        }

        public string Condition { get; set; }
        public string[] ModDataFlags { get; set; }

        public DialogueAction Dialogue { get; set; }
        public MessageAction Message { get; set; }
        public WarpAction Warp { get; set; }
        public QuestionResponseAction DialogueWithChoices { get; set; }
        public ModifyInventoryAction ModifyInventory { get; set; }
        public OpenShopAction OpenShop { get; set; }
        public List<ModifyModDataAction> ModifyFlags { get; set; }
        public List<SpecialAction> ConditionalActions { get; set; }
        public BroadcastAction Broadcast { get; set; }
        public List<ModifyMailFlagAction> ModifyMailFlags { get; set; }
        public PlaySoundAction PlaySound { get; set; }
        public ChestAction UseChest { get; set; }
        public FadeAction Fade { get; set; }
        public ModifyBuffAction ModifyBuff { get; set; }

        public void Trigger(Farmer who, GenericBuilding building, Point tile)
        {
            if (ModifyFlags is not null)
            {
                SpecialAction.HandleModifyingBuildingFlags(building, ModifyFlags);
            }
            if (ModifyMailFlags is not null)
            {
                SpecialAction.HandleModifyingMailFlags(ModifyMailFlags);
            }
            if (ConditionalActions is not null)
            {
                var validAction = ConditionalActions.Where(d => building.ValidateConditions(d.Condition, d.ModDataFlags)).FirstOrDefault();
                if (validAction is not null)
                {
                    validAction.Trigger(who, building, tile);
                }
            }
            if (UseChest is not null)
            {
                if (building.GetBuildingChest(UseChest.Name) is not null)
                {
                    building.PerformBuildingChestAction(UseChest.Name, who);
                }
            }
            if (Dialogue is not null)
            {
                var dialogues = new List<string>();
                foreach (string line in Dialogue.Text)
                {
                    dialogues.Add(HandleSpecialTextTokens(building.Model.GetTranslation(line)));
                }

                if (Dialogue.ActionAfterDialogue is not null)
                {
                    var dialogue = new SpecialActionDialogueBox(dialogues, Dialogue.ActionAfterDialogue, building, tile);
                    Game1.activeClickableMenu = dialogue;
                    dialogue.SetUp();
                }
                else
                {
                    Game1.activeClickableMenu = new DialogueBox(dialogues);
                }
            }
            if (DialogueWithChoices is not null && building.ValidateConditions(this.Condition, this.ModDataFlags))
            {
                List<Response> responses = new List<Response>();
                foreach (var response in DialogueWithChoices.Responses.Where(r => String.IsNullOrEmpty(r.Text) is false).OrderBy(r => DialogueWithChoices.ShuffleResponseOrder ? Game1.random.Next() : DialogueWithChoices.Responses.IndexOf(r)))
                {
                    responses.Add(new Response(DialogueWithChoices.Responses.IndexOf(response).ToString(), HandleSpecialTextTokens(building.Model.GetTranslation(response.Text))));
                }

                // Unable to use the vanilla method, as the afterQuestion gets cleared before the second instance of DialogueBox can call it (due to first instance closing)
                //who.currentLocation.createQuestionDialogue(HandleSpecialTextTokens(DialogueWithChoices.Question), responses.ToArray(), new GameLocation.afterQuestionBehavior((who, whichAnswer) => DialogueResponsePicked(who, building, tile, whichAnswer)));

                var dialogue = new SpecialActionDialogueBox(HandleSpecialTextTokens(building.Model.GetTranslation(DialogueWithChoices.Question)), responses, (who, whichAnswer) => DialogueResponsePicked(who, building, tile, whichAnswer));
                Game1.activeClickableMenu = dialogue;
                dialogue.SetUp();
            }
            if (Fade is not null)
            {
                Game1.globalFadeToBlack(afterFade: Fade.ActionAfterFade is null ? null : delegate { Fade.ActionAfterFade.Trigger(who, building, tile); }, fadeSpeed: Fade.Speed);
            }
            if (Message is not null)
            {
                Game1.addHUDMessage(new HUDMessage(building.Model.GetTranslation(Message.Text), (int)Message.Icon + 1));
            }
            if (ModifyInventory is not null)
            {
                var quantity = ModifyInventory.Quantity;
                if (quantity <= 0)
                {
                    if (ModifyInventory.MaxCount < ModifyInventory.MinCount)
                    {
                        ModifyInventory.MaxCount = ModifyInventory.MinCount;
                    }

                    quantity = new Random((int)((long)Game1.uniqueIDForThisGame + who.DailyLuck + Game1.stats.DaysPlayed * 500)).Next(ModifyInventory.MinCount, ModifyInventory.MaxCount + 1);
                }

                if (quantity > 0 && Toolkit.CreateItemByID(ModifyInventory.ItemId, quantity, ModifyInventory.Quality) is Item item && item is not null)
                {
                    if (ModifyInventory.Operation == OperationName.Add && item is not null)
                    {
                        if (who.couldInventoryAcceptThisItem(item))
                        {
                            who.addItemToInventoryBool(item);
                        }
                    }
                    else if (ModifyInventory.Operation == OperationName.Remove && item is not null)
                    {
                        InventoryManagement.ConsumeItemBasedOnQuantityAndQuality(who, item, quantity, ModifyInventory.Quality);
                    }
                }
            }
            if (ModifyBuff is not null && ModifyBuff.GetBuffType() is not BuffType.Unknown)
            {
                int buffType = (int)ModifyBuff.GetBuffType();
                var source = $"{building.Model.ID}_{ModifyBuff.Buff}_{ModifyBuff.Level}";

                var buff = new Buff(null, ModifyBuff.DurationInMilliseconds, source, buffType) { which = buffType, displaySource = building.Model.Name };
                if (buffType < buff.buffAttributes.Length)
                {
                    buff.buffAttributes[buffType] = ModifyBuff.Level;
                }
                buff.glow = ModifyBuff.Glow;

                // Setting source to null while using getDescription
                buff.source = null;
                var description = buff.getDescription(buffType);
                foreach (var subBuff in ModifyBuff.SubBuffs)
                {
                    int subBuffType = (int)subBuff.GetBuffType();
                    if (subBuffType < buff.buffAttributes.Length)
                    {
                        buff.buffAttributes[subBuffType] = subBuff.Level;
                        description += buff.getDescription(subBuffType);
                    }
                }
                buff.description = description + ModifyBuff.Description;
                buff.source = source;

                Game1.buffsDisplay.addOtherBuff(buff);
            }
            if (OpenShop is not null)
            {
                if (OpenShop.Type is StoreType.Vanilla)
                {
                    HandleVanillaShopMenu(OpenShop.Name, who);
                }
                else if (OpenShop.Type is StoreType.STF && SolidFoundations.apiManager.GetShopTileFrameworkApi() is not null)
                {
                    SolidFoundations.apiManager.GetShopTileFrameworkApi().OpenItemShop(OpenShop.Name);
                }
            }
            if (PlaySound is not null)
            {
                SpecialAction.HandlePlayingSound(building, PlaySound);
            }
            if (Warp is not null)
            {
                if (Warp.IsMagic)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192), Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false));
                    }
                    Game1.displayFarmer = false;
                    Game1.player.temporarilyInvincible = true;
                    Game1.player.temporaryInvincibilityTimer = -2000;
                    Game1.player.freezePause = 1000;
                    Game1.flashAlpha = 1f;
                    DelayedAction.fadeAfterDelay(delegate
                    {
                        Game1.warpFarmer(Warp.Map, Warp.DestinationTile.X, Warp.DestinationTile.Y, Warp.FacingDirection == -1 ? who.FacingDirection : Warp.FacingDirection);
                        Game1.fadeToBlackAlpha = 0.99f;
                        Game1.screenGlow = false;
                        Game1.player.temporarilyInvincible = false;
                        Game1.player.temporaryInvincibilityTimer = 0;
                        Game1.displayFarmer = true;
                    }, 1000);
                    new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
                    int j = 0;
                    for (int x = who.getTileX() + 8; x >= who.getTileX() - 8; x--)
                    {
                        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(x, who.getTileY()) * 64f, Color.White, 8, flipped: false, 50f)
                        {
                            layerDepth = 1f,
                            delayBeforeAnimationStart = j * 25,
                            motion = new Vector2(-0.25f, 0f)
                        });
                        j++;
                    }
                }
                else
                {
                    Game1.warpFarmer(Warp.Map, Warp.DestinationTile.X, Warp.DestinationTile.Y, Warp.FacingDirection == -1 ? who.FacingDirection : Warp.FacingDirection);
                }
            }
            if (Broadcast is not null)
            {
                var triggeredArgs = new IApi.BroadcastEventArgs()
                {
                    BuildingId = building.Id,
                    Building = building,
                    Farmer = who,
                    TriggerTile = tile,
                    Message = Broadcast.Message
                };
                SolidFoundations.api.OnSpecialActionTriggered(triggeredArgs);
            }
        }

        private void DialogueResponsePicked(Farmer who, GenericBuilding building, Point tile, string answerTextIndex)
        {
            int answerIndex = -1;

            if (!int.TryParse(answerTextIndex, out answerIndex) || DialogueWithChoices is null)
            {
                return;
            }

            ResponseAction response = DialogueWithChoices.Responses[answerIndex];
            if (response.SpecialAction is null)
            {
                return;
            }

            response.SpecialAction.Trigger(who, building, tile);
        }

        private string HandleSpecialTextTokens(string text)
        {
            var dialogueText = TextParser.ParseText(text);
            dialogueText = SolidFoundations.modHelper.Reflection.GetMethod(new Dialogue(dialogueText, null), "checkForSpecialCharacters").Invoke<string>(dialogueText);

            return dialogueText;
        }

        // Vanilla shop related
        private void HandleVanillaShopMenu(string shopName, Farmer who)
        {
            switch (shopName.ToLower())
            {
                case "clintshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithStock(), 0, "Clint");
                    return;
                case "deserttrader":
                    Game1.activeClickableMenu = new ShopMenu(Desert.getDesertMerchantTradeStock(who), 0, "DesertTrade", onDesertTraderPurchase);
                    return;
                case "dwarfshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getDwarfShopStock(), 0, "Dwarf");
                    return;
                case "geodes":
                    Game1.activeClickableMenu = new GeodeMenu();
                    return;
                case "gusshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getSaloonStock(), 0, "Gus", (item, farmer, amount) => onGenericPurchase(SynchronizedShopStock.SynchedShop.Saloon, item, farmer, amount));
                    return;
                case "harveyshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getHospitalStock());
                    return;
                case "itemrecovery":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getAdventureRecoveryStock(), 0, "Marlon_Recovery");
                    return;
                case "krobusshop":
                    Game1.activeClickableMenu = new ShopMenu((Game1.getLocationFromName("Sewer") as Sewer).getShadowShopStock(), 0, "KrobusGone", null);
                    return;
                case "marlonshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getAdventureShopStock(), 0, "Marlon");
                    return;
                case "marnieshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock(), 0, "Marnie");
                    return;
                case "pierreshop":
                    Game1.activeClickableMenu = new ShopMenu(new SeedShop().shopStock(), 0, "Pierre");
                    return;
                case "qishop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getQiShopStock(), 2);
                    return;
                case "sandyshop":
                    Game1.activeClickableMenu = new ShopMenu(SolidFoundations.modHelper.Reflection.GetMethod(Game1.currentLocation, "sandyShopStock").Invoke<Dictionary<ISalable, int[]>>(), 0, "Sandy", (item, farmer, amount) => onGenericPurchase(SynchronizedShopStock.SynchedShop.Sandy, item, farmer, amount));
                    return;
                case "robinshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                    return;
                case "travelingmerchant":
                case "travellingmerchant":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), 0, "Traveler", Utility.onTravelingMerchantShopPurchase);
                    return;
                case "toolupgrades":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithUpgradeStock(who), 0, "ClintUpgrade");
                    return;
                case "willyshop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(who), 0, "Willy");
                    return;
            }
        }
        private bool onDesertTraderPurchase(ISalable item, Farmer who, int amount)
        {
            if (item.Name == "Magic Rock Candy")
            {
                Desert.boughtMagicRockCandy = true;
            }
            return false;
        }

        private bool onGenericPurchase(SynchronizedShopStock.SynchedShop synchedShop, ISalable item, Farmer who, int amount)
        {
            who.team.synchronizedShopStock.OnItemPurchased(synchedShop, item, amount);
            return false;
        }

        internal static void HandleModifyingBuildingFlags(GenericBuilding building, List<ModifyModDataAction> modifyFlags)
        {
            foreach (var modifyFlag in modifyFlags)
            {
                // If FlagType is Temporary, then remove it from the building's modData before saving
                var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", modifyFlag.Name.ToLower());
                if (modifyFlag.Operation is OperationName.Add)
                {
                    building.modData[flagKey] = modifyFlag.Type.ToString();
                }
                else if (modifyFlag.Operation is OperationName.Remove && building.modData.ContainsKey(flagKey))
                {
                    building.modData.Remove(flagKey);
                }
            }
        }

        internal static void HandleModifyingMailFlags(List<ModifyMailFlagAction> modifyMailFlags)
        {
            foreach (var mailFlag in modifyMailFlags)
            {
                if (mailFlag.Operation is OperationName.Add)
                {
                    Game1.addMail(mailFlag.Name, true, false);
                }
                else if (mailFlag.Operation is OperationName.Remove)
                {
                    Game1.player.RemoveMail(mailFlag.Name, false);
                }
            }
        }

        internal static void HandlePlayingSound(GenericBuilding building, PlaySoundAction playSound)
        {
            if (playSound.IsValid())
            {
                if (Game1.soundBank == null)
                {
                    return;
                }

                int actualPitch = 1200;
                if (playSound.Pitch != -1)
                {
                    actualPitch = playSound.Pitch + playSound.GetPitchRandomized();
                }

                try
                {
                    ICue cue = Game1.soundBank.GetCue(playSound.Sound);
                    cue.SetVariable("Pitch", actualPitch);

                    var actualVolume = playSound.Volume;
                    if (playSound.AmbientSettings is not null && playSound.AmbientSettings.MaxDistance > 0)
                    {
                        float distance = Vector2.Distance(new Vector2(building.tileX.Value + playSound.AmbientSettings.Source.X, building.tileY.Value + playSound.AmbientSettings.Source.Y) * 64f, Game1.player.getStandingPosition());
                        actualVolume = Math.Min(1f, 1f - distance / playSound.AmbientSettings.MaxDistance) * playSound.Volume * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel);
                    }
                    cue.Volume = actualVolume;

                    cue.Play();
                    try
                    {
                        if (!cue.IsPitchBeingControlledByRPC)
                        {
                            cue.Pitch = Utility.Lerp(-1f, 1f, actualPitch / 2400f);
                        }
                    }
                    catch (Exception ex)
                    {
                        SolidFoundations.monitor.LogOnce($"Failed to play ({playSound.Sound}) given for {building.Id}: {ex}", StardewModdingAPI.LogLevel.Warn);
                    }
                }
                catch (Exception ex2)
                {
                    SolidFoundations.monitor.LogOnce($"Failed to play ({playSound.Sound}) given for {building.Id}: {ex2}", StardewModdingAPI.LogLevel.Warn);
                }
            }
            else
            {
                SolidFoundations.monitor.LogOnce($"Invalid sound ({playSound.Sound}) given for {building.Id}", StardewModdingAPI.LogLevel.Warn);
            }
        }
    }
}
