/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trading.Utilities
{
    internal class TradeMenu : IClickableMenu
    {
        private const int BaseIdSenderItems = 41000;
        private const int BaseIdReceiverItems = 42000;
        private const int BaseIdSenderInventory = 43000;
        private const int IdSenderGold = 50001;
        private const int IdSendOfferButton = 50002;
        private const int IdConfirmOfferButton = 50003;
        private const int IdCloseButton = 50004;

        private Farmer _sender;
        private Farmer _receiver;
        private bool _pending;
        private List<Item> _receiverItems;
        private List<Item> _senderItems;
        private float _receiverGold;
        private float _senderGold;
        private bool _sentOffer;
        private bool _receivedOffer;
        private bool _acceptedOffer;
        private bool _receivedConfirmation;
        private bool _confirmedOffer;

        private IModHelper helper => ModEntry.IHelper;
        private IMonitor monitor => ModEntry.IMonitor;
        private readonly string PendingText = ModEntry.ITranslations.PendingResponse;

        private Item hoverItem;
        private InventoryMenu senderInventory;
        private List<ClickableComponent> senderSlots;
        private List<ClickableComponent> receiverSlots;
        private ClickableComponent senderGoldComponent;
        private TextBox senderGold;
        private bool lastTextBoxState;
        private string hoverText;
        private ClickableComponent sendOfferButton;
        private ClickableComponent confirmOfferButton;
        private int playerPanelIndex_R;
        private int playerPanelTimer_R;
        private Rectangle playerPanel_R;
        private int playerPanelIndex_S;
        private int playerPanelTimer_S;
        private Rectangle playerPanel_S;
        private int[] playerPanelFrames = new int[4]
        {
          0,
          1,
          0,
          2
        };

        public Farmer Sender
        {
            get => _sender;
            set => _sender = value;
        }
        public Farmer Receiver
        {
            get => _receiver;
            set => _receiver = value;
        }
        public bool Pending
        {
            get => _pending;
            set => _pending = value;
        }
        public List<Item> ReceiverItems
        {
            get => _receiverItems;
            set 
            {
                _receiverItems = value;
                reloadItems();
            }
        }
        public List<Item> SenderItems
        {
            get => _senderItems;
            set
            {
                _senderItems = value;
                reloadItems();
            }
        }
        public float ReceiverGold
        {
            get => _receiverGold;
            set => _receiverGold = value < 0 ? 0 : value;
        }
        public float SenderGold
        {
            get => _senderGold;
            set
            {
                _senderGold = value < 0 ? 0 : value;
                if (Pending || senderGold is null) return;
                senderGold.Text = $"{_senderGold:0}";
            }
        }

        internal bool SentOffer
        {
            get => _sentOffer;
            set => _sentOffer = value;
        }
        internal bool ReceivedOffer
        {
            get => _receivedOffer;
            set => _receivedOffer = value;
        }
        internal bool AcceptedOffer
        {
            get => _acceptedOffer;
            set
            {
                _acceptedOffer = value;
                assignIds();
            }
        }
        internal bool ReceivedConfirmation
        {
            get => _receivedConfirmation;
            set => _receivedConfirmation = value;
        }
        internal bool ConfirmedOffer
        {
            get => _confirmedOffer;
            set => _confirmedOffer = value;
        }

        public int X
        {
            get => xPositionOnScreen;
            set => xPositionOnScreen = value;
        }
        public int Y
        {
            get => yPositionOnScreen;
            set => yPositionOnScreen = value;
        }
        public int Width
        {
            get => width;
            set => width = value;
        }
        public int Height
        {
            get => height;
            set => height = value;
        }

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public TradeMenu(Farmer sender, Farmer receiver, bool isPending, List<Item>? receiverItems = null, float? receiverGold = null, List<Item>? senderItems = null, float? senderGold = null) : base(0, 0, 0, 0, true)
        {
            Sender = sender;
            Receiver = receiver;
            Pending = isPending;
            ReceiverItems = receiverItems ?? new List<Item>();
            SenderItems = senderItems ?? new List<Item>();
            ReceiverGold = receiverGold ?? 0;
            SenderGold = senderGold ?? 0;

            PendingText = string.Format(PendingText, receiver.Name);

            senderSlots = new List<ClickableComponent>();
            receiverSlots = new List<ClickableComponent>();

            loadViewComponents();
        }

        private void loadViewComponents()
        {
            if (Pending)
            {
                Width = 400 + borderWidth * 2;
                Height = 250 + borderWidth * 2;

                X = Game1.uiViewport.Width / 2 - (Width / 2);
                Y = Game1.uiViewport.Height / 2 - (Height / 2);

                initializeUpperRightCloseButton();
                upperRightCloseButton.bounds.X = X + Width - 40;
                upperRightCloseButton.bounds.Y = Y + 60;
                upperRightCloseButton.myID = IdCloseButton;
                return;
            }

            Width = 1000 + borderWidth * 2;
            Height = 650 + borderWidth * 2;

            X = Game1.uiViewport.Width / 2 - (Width / 2);
            Y = Game1.uiViewport.Height / 2 - (Height / 2);

            initializeUpperRightCloseButton();
            upperRightCloseButton.bounds.X = X + Width - 40;
            upperRightCloseButton.bounds.Y = Y + 60;

            playerPanel_R = new Rectangle(X + Width - 176, Y + borderWidth + spaceToClearTopBorder, 128, 192);
            playerPanel_S = new Rectangle(X + 64 - 12, Y + borderWidth + spaceToClearTopBorder, 128, 192);

            senderInventory = new(X + 64, Y + Height - 150, false, highlightMethod:(i) => i is not Tool || i is MeleeWeapon || i is Slingshot);
            senderInventory = new(X + Width / 2 - senderInventory.width / 2, Y + Height - senderInventory.height - 28, false, highlightMethod:(i) => i is not Tool || i is MeleeWeapon || i is Slingshot);
            senderInventory.showGrayedOutSlots = true;

            for (int i = 0; i < 16; i++)
            {
                senderSlots.Add(new ClickableComponent(new Rectangle(playerPanel_S.Right + ((i % 4) * 64) + senderInventory.horizontalGap * (i % 4) + 8, playerPanel_S.Top + i / 4 * 64 + senderInventory.verticalGap + ((i / 4) - 1) * 4, 64, 64), $"SenderSlot_{i}"));
                receiverSlots.Add(new ClickableComponent(new Rectangle(playerPanel_R.Left - (64 * 3 - ((i % 4) * 64)) - senderInventory.horizontalGap * (i % 4) - 64 - 8, playerPanel_S.Top + i / 4 * 64 + senderInventory.verticalGap + ((i / 4) - 1) * 4, 64, 64), $"ReceiverSlot_{i}"));
            }

            senderGold = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textbox"), null, Game1.smallFont, Game1.textColor) { X = playerPanel_S.Left + 8, Y = playerPanel_S.Bottom + 12, Width = 120, Height = 56, numbersOnly = true, Text = $"{SenderGold}" };
            senderGold.OnEnterPressed += onAttemptSendGold;
            senderGoldComponent = new ClickableComponent(new Rectangle(playerPanel_S.Left + 8, playerPanel_S.Bottom + 12, 120, 56), "");

            for (int i = 0; i < ReceiverItems.Count && i < 16; i++)
                receiverSlots[i].item = ReceiverItems[i];
            for (int i = 0; i < SenderItems.Count && i < 16; i++)
                senderSlots[i].item = SenderItems[i];

            sendOfferButton = new ClickableComponent(new Rectangle(senderSlots[3].bounds.X + 72, senderSlots[3].bounds.Y + 32, 180, 50), "");
            confirmOfferButton = new ClickableComponent(new Rectangle(senderSlots[11].bounds.X + 72, senderSlots[11].bounds.Y + 32, 180, 50), "");
            assignIds();
        }

        private void assignIds()
        {
            upperRightCloseButton.myID = IdCloseButton;
            sendOfferButton.myID = IdSendOfferButton;
            confirmOfferButton.myID = IdConfirmOfferButton;
            senderGoldComponent.myID = IdSenderGold;

            const int rowLength = 12;
            int colLength = senderInventory.inventory.Count / rowLength;

            for (int i = 0; i < colLength; i++) 
            {
                for (int j = 0; j < rowLength; j++) 
                {
                    int index = j + (rowLength * i);
                    senderInventory.inventory[index].myID = BaseIdSenderInventory + index;

                    senderInventory.inventory[index].upNeighborID = i != 0 ? BaseIdSenderInventory + index - rowLength : -1;
                    if (index < 4) 
                        senderInventory.inventory[index].upNeighborID = BaseIdSenderItems + 12 + index;
                    if (index >= 8 && index < 12) 
                        senderInventory.inventory[index].upNeighborID = BaseIdReceiverItems + 12 + ((index % 4));
                    senderInventory.inventory[index].downNeighborID = i != (colLength - 1) ? BaseIdSenderInventory + index + rowLength : -1;
                    senderInventory.inventory[index].leftNeighborID = j != 0 ? BaseIdSenderInventory + index - 1 : (i > 0 ? BaseIdSenderInventory + index - 1 : -1);
                    senderInventory.inventory[index].rightNeighborID = j != rowLength - 1 ? BaseIdSenderInventory + index + 1 : (i < colLength - 1 ? BaseIdSenderInventory + rowLength * (i + 1) : -1);
                }
            }

            for (int i = 0; i < senderSlots.Count; i++)
            {
                senderSlots[i].myID = BaseIdSenderItems + i;
                senderSlots[i].upNeighborID = i / 4 == 0 ? -1 : BaseIdSenderItems + i - 4;
                senderSlots[i].downNeighborID = i / 4 == 3 ? BaseIdSenderInventory + (i % 4) : BaseIdSenderItems + i + 4;
                senderSlots[i].rightNeighborID = i % 4 == 3 ? (!AcceptedOffer ? sendOfferButton.myID : confirmOfferButton.myID) : BaseIdSenderItems + i + 1;
                senderSlots[i].leftNeighborID = i % 4 == 0 ? -1 : BaseIdSenderItems + i - 1;
                if (i == 12) senderSlots[i].leftNeighborID = senderGoldComponent.myID;
            }
            for (int i = 0; i < receiverSlots.Count; i++)
            {
                receiverSlots[i].myID = BaseIdReceiverItems + i;
                receiverSlots[i].upNeighborID = i / 4 == 0 ? -1 : BaseIdReceiverItems + i - 4;
                receiverSlots[i].downNeighborID = i / 4 == 3 ? BaseIdSenderInventory + rowLength - 4 + (i % 4) : BaseIdReceiverItems + i + 4;
                receiverSlots[i].rightNeighborID = i % 4 == 3 ? IdCloseButton : BaseIdReceiverItems + i + 1;
                receiverSlots[i].leftNeighborID = i % 4 == 0 ? (!AcceptedOffer ? sendOfferButton.myID : confirmOfferButton.myID) : BaseIdReceiverItems + i - 1;
            }
            upperRightCloseButton.downNeighborID = receiverSlots[3].myID;
            upperRightCloseButton.leftNeighborID = receiverSlots[3].myID;
            senderGoldComponent.rightNeighborID = senderSlots[12].myID;
            senderGoldComponent.downNeighborID = BaseIdSenderInventory;
            sendOfferButton.leftNeighborID = senderSlots[3].myID;
            sendOfferButton.rightNeighborID = receiverSlots[0].myID;
            confirmOfferButton.leftNeighborID = senderSlots[11].myID;
            confirmOfferButton.rightNeighborID = receiverSlots[8].myID;
        }

        private void getClickableComponentList()
        {
            allClickableComponents = new List<ClickableComponent>();
            if (!Pending)
            {
                for (int i = 0; i < senderSlots.Count; i++)
                    allClickableComponents.Add(senderSlots[i]);
                for (int i = 0; i < receiverSlots.Count; i++)
                    allClickableComponents.Add(receiverSlots[i]);
                for (int i = 0; i < senderInventory.inventory.Count; i++)
                    allClickableComponents.Add(senderInventory.inventory[i]);
                allClickableComponents.Add(upperRightCloseButton);
                allClickableComponents.Add(sendOfferButton);
                allClickableComponents.Add(confirmOfferButton);
                allClickableComponents.Add(senderGoldComponent);
            }
            allClickableComponents.Add(upperRightCloseButton);
        }

        private ClickableComponent getComponentWithId(int id)
        {
            getClickableComponentList();
            for (int i = 0; i < allClickableComponents.Count; i++)
                if (allClickableComponents[i].myID == id || allClickableComponents[i].myAlternateID == id)
                    return allClickableComponents[i];
            return null;
        }

        private void drawSenderPortrait(SpriteBatch b)
        {
            int num1 = X + 64 - 12;
            int num2 = Y + borderWidth + spaceToClearTopBorder;
            b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2(num1, num2), Color.White);
            FarmerRenderer.isDrawingForUI = true;
            Sender.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(Sender.bathingClothes.Value ? 108 : playerPanelFrames[playerPanelIndex_S], 0, false, false), Sender.bathingClothes.Value ? 108 : playerPanelFrames[playerPanelIndex_S], new Rectangle(playerPanelFrames[playerPanelIndex_S] * 16, Sender.bathingClothes.Value ? 576 : 0, 16, 32), new Vector2(num1 + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Sender);
            if (Game1.timeOfDay >= 1900)
                Sender.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(playerPanelFrames[playerPanelIndex_S], 0, false, false), playerPanelFrames[playerPanelIndex_S], new Rectangle(playerPanelFrames[playerPanelIndex_S] * 16, 0, 16, 32), new Vector2(num1 + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Sender);
            FarmerRenderer.isDrawingForUI = false;
        }

        private void drawReceiverPortrait(SpriteBatch b)
        {
            int num1 = X + Width - 176;
            int num2 = Y + borderWidth + spaceToClearTopBorder;
            b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, new Vector2(num1, num2), Color.White);
            FarmerRenderer.isDrawingForUI = true;
            Receiver.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(Receiver.bathingClothes.Value ? 108 : playerPanelFrames[playerPanelIndex_R], 0, false, false), Receiver.bathingClothes.Value ? 108 : playerPanelFrames[playerPanelIndex_R], new Rectangle(playerPanelFrames[playerPanelIndex_R] * 16, Receiver.bathingClothes.Value ? 576 : 0, 16, 32), new Vector2(num1 + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, Receiver);
            if (Game1.timeOfDay >= 1900)
                Receiver.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(playerPanelFrames[playerPanelIndex_R], 0, false, false), playerPanelFrames[playerPanelIndex_R], new Rectangle(playerPanelFrames[playerPanelIndex_R] * 16, 0, 16, 32), new Vector2(num1 + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0.0f, 1f, Receiver);
            FarmerRenderer.isDrawingForUI = false;
        }

        private void onAttemptSendGold(TextBox sender)
        {
            resetOfferStatus();
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                SenderGold = 0;
                sender.Text = "0";
                sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
                return;
            }
            float value = Convert.ToSingle(sender.Text);
            if (value > Sender.Money)
                value = Sender.Money;
            SenderGold = value;
            sender.Text = $"{value:0}";
            sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
        }

        private NetworkInventory getParsableInventory() => new(Game1.player.UniqueMultiplayerID, SenderGold, Utilites.ParseItems(SenderItems));

        private void sendNetworkMessage<T>(string message, T data) => helper.Multiplayer.SendMessage(data, message, ModEntry.ModId, new[] { Receiver.UniqueMultiplayerID });

        private void reloadItems()
        {
            if (Pending) return;
            if (ReceiverItems is not null && receiverSlots is not null)
                for (int i = 0; i < 16; i++)
                    receiverSlots[i].item = ReceiverItems.Count > i ? ReceiverItems[i] : null;
            if (SenderItems is not null && senderSlots is not null)
                for (int i = 0; i < 16; i++)
                    senderSlots[i].item = SenderItems.Count > i ? SenderItems[i] : null;
        }

        private Item takeItemFromInventory(Item obj)
        {
            bool isShift = Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldKBState.IsKeyDown(Keys.RightShift);
            bool isCtrl = Game1.oldKBState.IsKeyDown(Keys.LeftControl) || Game1.oldKBState.IsKeyDown(Keys.RightControl);
            int stack = 1;
            if (isShift) stack = obj.Stack >= 5 ? 5 : obj.Stack;
            if (isCtrl && isShift) stack = obj.Stack >= 25 ? 25 : obj.Stack;
            Item newObj = obj.getOne();
            newObj.Stack = stack;
            if (obj.Stack - stack <= 0) Sender.removeItemFromInventory(obj);
            else obj.Stack -= stack;
            return newObj;
        }

        private Item takeItemFromTrade(Item obj)
        {
            bool isShift = Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldKBState.IsKeyDown(Keys.RightShift);
            bool isCtrl = Game1.oldKBState.IsKeyDown(Keys.LeftControl) || Game1.oldKBState.IsKeyDown(Keys.RightControl);
            int stack = 1;
            if (isShift) stack = obj.Stack >= 5 ? 5 : obj.Stack;
            if (isCtrl && isShift) stack = obj.Stack >= 25 ? 25 : obj.Stack;
            Item newObj = obj.getOne();
            newObj.Stack = stack;
            if (obj.Stack - stack <= 0)
            {
                senderSlots[SenderItems.IndexOf(obj)].item = null;
                SenderItems.Remove(obj);
            }
            else obj.Stack -= stack;
            reloadItems();
            return newObj;
        }
        
        private void addItemToTrade(Item obj)
        {
            if (SenderItems.FirstOrDefault(x => x.canStackWith(obj)) is not null and Item i)
                SenderItems[SenderItems.IndexOf(i)].addToStack(obj);
            else
                SenderItems.Add(obj);
        }

        private void resetOfferStatus()
        {
            if (SentOffer || ReceivedOffer || AcceptedOffer || ReceivedConfirmation || ConfirmedOffer)
            {
                sendNetworkMessage(Utilites.MSG_DeclineOffer, (NetworkPlayer)Game1.player);
                AcceptedOffer = false;
                SentOffer = false;
                ReceivedOffer = false;
                ReceivedConfirmation = false;
                ConfirmedOffer = false;
            }
        }

        private void drawButtonWithText(SpriteBatch b, string text, Vector2 position, Color color, int textOffset = 0)
        {
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), (int)position.X, (int)position.Y, 180, 50, color, 2, false);
            b.DrawString(Game1.smallFont, text, new Vector2(position.X + textOffset, position.Y + 8), Game1.textColor);
        }

        public void exit(bool sendExitMessage = false, bool playSound = true)
        {
            for (int i = 0; i < SenderItems.Count; i++)
                if (!Sender.addItemToInventoryBool(SenderItems[i]))
                    Game1.createItemDebris(SenderItems[i], Sender.getStandingPosition(), Sender.FacingDirection);

            if (sendExitMessage)
                sendNetworkMessage(Utilites.MSG_ExitTrade, (NetworkPlayer)Game1.player);
            exitThisMenu(playSound);
        }

        public override void snapToDefaultClickableComponent()
        {
            if (Pending)
            {
                currentlySnappedComponent = getComponentWithId(IdCloseButton);
                return;
            }
            currentlySnappedComponent = getComponentWithId(senderInventory.actualInventory.Count > 0 ? BaseIdSenderInventory : BaseIdSenderItems);
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            currentlySnappedComponent = getComponentWithId(id);
            if (currentlySnappedComponent == null)
            {
                snapToDefaultClickableComponent();
                ModEntry.IMonitor.Log($"Couldn't snap to component with id : {id}, Snapping to default", LogLevel.Warn);
            }
            Game1.playSound("smallSelect");
        }

        public override void setUpForGamePadMode()
        {
            snapToDefaultClickableComponent();
            snapCursorToCurrentSnappedComponent();
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent == null) snapToDefaultClickableComponent();
            switch (direction)
            {
                case 0: //Up
                    if (currentlySnappedComponent.upNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 1: //Right
                    if (currentlySnappedComponent.rightNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.rightNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2: //Down
                    if (currentlySnappedComponent.downNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 3: //Left
                    if (currentlySnappedComponent.leftNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.leftNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                default:
                    base.applyMovementKey(direction);
                    break;
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.Back:
                case Buttons.B:
                case Buttons.Y:
                    if (senderGold?.Selected ?? false)
                        senderGold.Selected = false;
                    else
                        exit(true);
                    return;
                case Buttons.A:
                    if (currentlySnappedComponent != null)
                    {
                        if (currentlySnappedComponent.myID == IdCloseButton) goto case Buttons.B;
                        else if (currentlySnappedComponent.myID == IdSenderGold) senderGold.SelectMe();
                    }
                    break;
            }
            base.receiveGamePadButton(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
            {
                exit(true, playSound: playSound);
                return;
            }

            if (Pending) return;
            if (senderGoldComponent.bounds.Contains(x, y))
            {
                senderGold.SelectMe();
                return;
            }
            else senderGold.Selected = false;

            if (sendOfferButton.bounds.Contains(x, y))
            {
                if (!SentOffer && !ReceivedOffer)
                {
                    resetOfferStatus();
                    sendNetworkMessage(Utilites.MSG_SendTradeOffer, (NetworkPlayer)Game1.player);
                    SentOffer = true;
                    return;
                }
                if (SentOffer)
                {
                    resetOfferStatus();
                    return;
                }
                if (ReceivedOffer)
                {
                    sendNetworkMessage(Utilites.MSG_SendTradeOffer, (NetworkPlayer)Game1.player);
                    AcceptedOffer = true;
                    return;
                }
            }

            if (confirmOfferButton.bounds.Contains(x, y))
            {
                if (!AcceptedOffer) return;
                if (!ReceivedConfirmation)
                {
                    sendNetworkMessage(Utilites.MSG_ConfirmTrade, (NetworkPlayer)Game1.player);
                    ConfirmedOffer = true;
                    return;
                }
                sendNetworkMessage(Utilites.MSG_ConfirmTrade, (NetworkPlayer)Game1.player);
                SenderItems.Clear();
                for (int i = 0; i < ReceiverItems.Count; i++)
                    if (!Sender.addItemToInventoryBool(ReceiverItems[i]))
                        Game1.createItemDebris(ReceiverItems[i], Sender.getStandingPosition(), Sender.FacingDirection, Sender.currentLocation);
                Sender._money -= (int)SenderGold;
                Sender._money += (int)ReceiverGold;
                exit(playSound: playSound);
                return;
            }

            if (senderInventory.inventory.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent invC)
            {
                int index = Convert.ToInt32(invC.name);
                if (senderInventory.actualInventory.Count > index && senderInventory.actualInventory[index] is not null and Item i && (i is not Tool || i is MeleeWeapon || i is Slingshot))
                {
                    resetOfferStatus();
                    addItemToTrade(i);
                    reloadItems();
                    Sender.removeItemFromInventory(i);
                    Game1.playSound("dwop");
                    sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
                    return;
                }
            }
            if (senderSlots.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent slotC)
            {
                if (slotC.item is not null and Item i)
                {
                    resetOfferStatus();
                    SenderItems.Remove(i);
                    slotC.item = null;
                    Sender.addItemToInventory(i);
                    reloadItems();
                    Game1.playSound("dwop");
                    sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
                    return;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (Pending) return;
            if (senderInventory.inventory.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent invC)
            {
                int index = Convert.ToInt32(invC.name);
                if (senderInventory.actualInventory.Count > index && senderInventory.actualInventory[index] is not null and Item i && (i is not Tool || i is MeleeWeapon || i is Slingshot))
                {
                    resetOfferStatus();
                    var newObj = takeItemFromInventory(i);
                    addItemToTrade(newObj);
                    reloadItems();
                    Game1.playSound("dwop");
                    sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
                    return;
                }
            }
            if (senderSlots.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent slotC)
            {
                if (slotC.item is not null and Item i)
                {
                    var newObj = takeItemFromTrade(i);
                    if (!Sender.addItemToInventoryBool(newObj))
                    {
                        addItemToTrade(newObj);
                        return;
                    }
                    resetOfferStatus();
                    reloadItems();
                    Game1.playSound("dwop");
                    var inv = getParsableInventory();
                    sendNetworkMessage(Utilites.MSG_UpdateTradeInventory, getParsableInventory());
                    return;
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (Pending) return;
            if (senderGold.Selected != lastTextBoxState)
                onAttemptSendGold(senderGold);
            lastTextBoxState = senderGold.Selected;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Pending) return;
            if (key == Keys.Escape)
            {
                if (senderGold.Selected)
                    senderGold.Selected = false;
                else
                {
                    exit(true);
                    return;
                }
            }
            else
                base.receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y)
        {
            upperRightCloseButton.tryHover(x, y);
            if (Pending) 
                return;
            hoverText = "";
            hoverItem = null;
            if (playerPanel_R.Contains(x, y))
            {
                hoverText = Receiver.Name;
                playerPanelTimer_R -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                if (playerPanelTimer_R > 0)
                    return;
                playerPanelIndex_R = (playerPanelIndex_R + 1) % 4;
                playerPanelTimer_R = 150;
            }
            else
                playerPanelIndex_R = 0;
            if (playerPanel_S.Contains(x, y))
            {
                hoverText = Sender.Name;
                playerPanelTimer_S -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                if (playerPanelTimer_S > 0)
                    return;
                playerPanelIndex_S = (playerPanelIndex_S + 1) % 4;
                playerPanelTimer_S = 150;
            }
            else
                playerPanelIndex_S = 0;
            hoverItem = senderInventory.hover(x, y, null);
            if (senderSlots.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent sc && sc.item is not null)
                hoverItem = sc.item;
            if (receiverSlots.FirstOrDefault(c => c.containsPoint(x, y)) is not null and ClickableComponent rc && rc.item is not null)
                hoverItem = rc.item;
            senderGold.Hover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            Game1.drawDialogueBox(X, Y, Width, Height, false, true);
            upperRightCloseButton.draw(b);

            if (Pending)
            {
                string text = Utilites.WrapText(Game1.smallFont, PendingText, Width - 50 - borderWidth * 2);
                b.DrawString(Game1.smallFont, text, new Vector2(Bounds.Center.X - (Game1.smallFont.MeasureString(text).X / 2), Bounds.Center.Y), Game1.textColor);
            }
            else
            {
                drawSenderPortrait(b);
                drawReceiverPortrait(b);

                senderInventory.draw(b);

                foreach (var slot in senderSlots)
                    b.Draw(Game1.menuTexture, new Vector2(slot.bounds.X, slot.bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                foreach (var slot in senderSlots)
                {
                    if (slot.item is null) break;
                    slot.item.drawInMenu(b, new Vector2(slot.bounds.X, slot.bounds.Y), 1f);
                }
                foreach (var slot in receiverSlots)
                {
                    b.Draw(Game1.menuTexture, new Vector2(slot.bounds.X, slot.bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                    b.Draw(Game1.menuTexture, new Vector2(slot.bounds.X, slot.bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57)), Color.White * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                }
                foreach (var slot in receiverSlots)
                {
                    if (slot.item is null) break;
                    slot.item.drawInMenu(b, new Vector2(slot.bounds.X, slot.bounds.Y), 1f, .7f, .88f);
                }

                b.Draw(Game1.debrisSpriteSheet, new Vector2(playerPanel_S.Left - 4, playerPanel_S.Bottom + 36), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16)), Color.White, 0.0f, new Vector2(8f), 3f, SpriteEffects.None, .95f);
                senderGold.Draw(b, false);
                b.Draw(Game1.debrisSpriteSheet, new Vector2(playerPanel_R.Left + 8, playerPanel_R.Bottom + 36), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16)), Color.White, 0.0f, new Vector2(8f), 3f, SpriteEffects.None, .95f);
                b.DrawString(Game1.smallFont, $"{ReceiverGold:0}", new Vector2(playerPanel_R.Left + 20, playerPanel_R.Bottom + 22), Game1.textColor);

                if (!AcceptedOffer)
                    drawButtonWithText(b, ReceivedOffer ? ModEntry.ITranslations.AcceptOffer : ModEntry.ITranslations.SendOffer, new Vector2(sendOfferButton.bounds.X, sendOfferButton.bounds.Y), SentOffer ? Color.Green : (ReceivedOffer ? Color.Yellow : Color.White), ReceivedOffer ? 16 : 22);
                else
                    drawButtonWithText(b, ModEntry.ITranslations.ConfirmOffer, new Vector2(confirmOfferButton.bounds.X, confirmOfferButton.bounds.Y), ConfirmedOffer ? Color.Green : (ReceivedConfirmation ? Color.Yellow : Color.White), 40);

                if (hoverItem is not null)
                    drawToolTip(b, hoverItem.getDescription(), hoverItem.DisplayName, hoverItem);
            }

            drawHoverText(b, hoverText, Game1.smallFont);
            drawMouse(b);
        }
    }
}
