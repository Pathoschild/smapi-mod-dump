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
using SolidFoundations.Framework.Extensions;
using SolidFoundations.Framework.Interfaces.Internal;
using SolidFoundations.Framework.UI;
using SolidFoundations.Framework.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void Trigger(Farmer who, Building building, Point tile)
        {
            if (SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);

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
                    dialogues.Add(HandleSpecialTextTokens(extendedModel.GetTranslation(line)));
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
                    responses.Add(new Response(DialogueWithChoices.Responses.IndexOf(response).ToString(), HandleSpecialTextTokens(extendedModel.GetTranslation(response.Text))));
                }

                // Unable to use the vanilla method, as the afterQuestion gets cleared before the second instance of DialogueBox can call it (due to first instance closing)
                //who.currentLocation.createQuestionDialogue(HandleSpecialTextTokens(DialogueWithChoices.Question), responses.ToArray(), new GameLocation.afterQuestionBehavior((who, whichAnswer) => DialogueResponsePicked(who, building, tile, whichAnswer)));

                var dialogue = new SpecialActionDialogueBox(HandleSpecialTextTokens(extendedModel.GetTranslation(DialogueWithChoices.Question)), responses, (who, whichAnswer) => DialogueResponsePicked(who, building, tile, whichAnswer));
                Game1.activeClickableMenu = dialogue;
                dialogue.SetUp();
            }
            if (Fade is not null)
            {
                Game1.globalFadeToBlack(afterFade: Fade.ActionAfterFade is null ? null : delegate { Fade.ActionAfterFade.Trigger(who, building, tile); }, fadeSpeed: Fade.Speed);
            }
            if (Message is not null)
            {
                Game1.addHUDMessage(new HUDMessage(extendedModel.GetTranslation(Message.Text), (int)Message.Icon + 1));
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

                if (quantity > 0 && ItemRegistry.Create(ModifyInventory.ItemId, quantity, ModifyInventory.Quality) is Item item && item is not null)
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
                var source = $"{extendedModel.ID}_{ModifyBuff.Buff}_{ModifyBuff.Level}";

                var buff = new Buff(buffType.ToString(), source, extendedModel.Name, duration: ModifyBuff.DurationInMilliseconds, description: ModifyBuff.Description);
                buff.glow = ModifyBuff.Glow;

                Game1.player.buffs.Apply(buff);

                foreach (var subModifyBuff in ModifyBuff.SubBuffs)
                {
                    int subBuffType = (int)subModifyBuff.GetBuffType();
                    var subBuff = new Buff(subModifyBuff.ToString(), source, extendedModel.Name, duration: subModifyBuff.DurationInMilliseconds, description: subModifyBuff.Description);
                    Game1.player.buffs.Apply(subBuff);
                }
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
                    for (int x = (int)who.Tile.X + 8; x >= (int)who.Tile.X - 8; x--)
                    {
                        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(x, who.Tile.Y) * 64f, Color.White, 8, flipped: false, 50f)
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
                    BuildingId = building.buildingType.Value,
                    Building = building,
                    Farmer = who,
                    TriggerTile = tile,
                    Message = Broadcast.Message
                };
                SolidFoundations.api.OnSpecialActionTriggered(triggeredArgs);
            }
        }

        private void DialogueResponsePicked(Farmer who, Building building, Point tile, string answerTextIndex)
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
            var dialogueText = TokenParser.ParseText(text);
            dialogueText = SolidFoundations.modHelper.Reflection.GetMethod(new Dialogue(null, string.Empty, dialogueText), "checkForSpecialCharacters").Invoke<string>(dialogueText);

            return dialogueText;
        }

        // Vanilla shop related
        private void HandleVanillaShopMenu(string shopName, Farmer who, string shopOwner = null)
        {
            switch (shopName.ToLower())
            {
                case "clintshop":
                    Utility.TryOpenShopMenu(Game1.shop_blacksmith, shopOwner);
                    return;
                case "deserttrader":
                    Utility.TryOpenShopMenu(Game1.shop_desertTrader, shopOwner);
                    return;
                case "dwarfshop":
                    Utility.TryOpenShopMenu(Game1.shop_dwarf, shopOwner);
                    return;
                case "geodes":
                    Game1.activeClickableMenu = new GeodeMenu();
                    return;
                case "gusshop":
                    Utility.TryOpenShopMenu(Game1.shop_saloon, shopOwner);
                    return;
                case "harveyshop":
                    Utility.TryOpenShopMenu(Game1.shop_hospital, shopOwner);
                    return;
                case "itemrecovery":
                    Utility.TryOpenShopMenu(Game1.shop_adventurersGuildItemRecovery, shopOwner);
                    return;
                case "krobusshop":
                    Utility.TryOpenShopMenu(Game1.shop_krobus, shopOwner);
                    return;
                case "marlonshop":
                    Utility.TryOpenShopMenu(Game1.shop_adventurersGuild, shopOwner);
                    return;
                case "marnieshop":
                    Utility.TryOpenShopMenu(Game1.shop_animalSupplies, shopOwner);
                    return;
                case "pierreshop":
                    Utility.TryOpenShopMenu(Game1.shop_generalStore, shopOwner);
                    return;
                case "qishop":
                    Utility.TryOpenShopMenu(Game1.shop_qiGemShop, shopOwner);
                    return;
                case "sandyshop":
                    Utility.TryOpenShopMenu(Game1.shop_sandy, shopOwner);
                    return;
                case "robinshop":
                    Utility.TryOpenShopMenu(Game1.shop_carpenter, shopOwner);
                    return;
                case "travelingmerchant":
                case "travellingmerchant":
                    Utility.TryOpenShopMenu(Game1.shop_travelingCart, shopOwner);
                    return;
                case "toolupgrades":
                    Utility.TryOpenShopMenu(Game1.shop_blacksmithUpgrades, shopOwner);
                    return;
                case "willyshop":
                    Utility.TryOpenShopMenu(Game1.shop_fish, shopOwner);
                    return;
                default:
                    Utility.TryOpenShopMenu(shopName, shopOwner);
                    return;
            }
        }

        internal static void HandleModifyingBuildingFlags(Building building, List<ModifyModDataAction> modifyFlags)
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

        internal static void HandlePlayingSound(Building building, PlaySoundAction playSound)
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
                        SolidFoundations.monitor.LogOnce($"Failed to play ({playSound.Sound}) given for {building.buildingType.Value}: {ex}", StardewModdingAPI.LogLevel.Warn);
                    }
                }
                catch (Exception ex2)
                {
                    SolidFoundations.monitor.LogOnce($"Failed to play ({playSound.Sound}) given for {building.buildingType.Value}: {ex2}", StardewModdingAPI.LogLevel.Warn);
                }
            }
            else
            {
                SolidFoundations.monitor.LogOnce($"Invalid sound ({playSound.Sound}) given for {building.buildingType.Value}", StardewModdingAPI.LogLevel.Warn);
            }
        }
    }
}
