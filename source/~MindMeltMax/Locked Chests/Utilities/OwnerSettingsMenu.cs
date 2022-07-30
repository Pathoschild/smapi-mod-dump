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
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Utilities
{
    internal class OwnerSettingsMenu : IClickableMenu
    {
        private static ITranslationHelper ITranslations => ModEntry.ITranslations;

        private const int CloseButtonId = 50;
        private const int DropdownBaseId = 100;
        private const int OwnerButtonsBaseId = 500;
        private const int AccessButtonsBaseId = 800;

        private Chest current;
        private Dropdown<Lock> lockDropdown;
        private List<ClickableTextureComponent> ownerButtons;
        private List<ClickableTextureComponent> accessButtons;
        private string hoverText = "";
        private bool finishedIntializing = false;

        private readonly Rectangle buttonBackgroundRect = new Rectangle(119, 469, 16, 16);
        private readonly Dictionary<Lock, string> dropdownOptions = new Dictionary<Lock, string>()
        {
            { Lock.Open, ITranslations.Get("Open") },
            { Lock.ReadOnly, ITranslations.Get("ViewOnly") },
            { Lock.Locked, ITranslations.Get("Locked") }

        };

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

        public int FarmerCount => Game1.getAllFarmers().Count();

        public OwnerSettingsMenu(Chest c) : base(0, 0, 0, 0, true)
        {
            current = c;
            if (!current.modData.ContainsKey("LockedChests.Owner") || !current.modData.ContainsKey("LockedChests.Owner") || !current.modData.ContainsKey("LockedChests.Owner"))
            {
                current.modData.Add("LockedChests.Owner", $"{Game1.player.UniqueMultiplayerID}");
                current.modData.Add("LockedChests.Lock", $"{Lock.Open}");
                current.modData.Add("LockedChests.AccessIds", $"[{Game1.player.UniqueMultiplayerID}]");
            }
            updateLayout();
        }

        public void updateLayout()
        {
            ownerButtons = new List<ClickableTextureComponent>();
            accessButtons = new List<ClickableTextureComponent>();
            X = Game1.uiViewport.Width / 2 - ((400 + borderWidth * 2) / 2);
            Y = Game1.uiViewport.Height / 2 - ((250 + borderWidth * 2) / 2) - 100;
            Width = 600 + borderWidth * 2;
            Height = 450 + borderWidth * 2;

            initializeUpperRightCloseButton();
            upperRightCloseButton.bounds = new Rectangle(X + Width - 40, Y + 56, 32, 32);
            upperRightCloseButton.myID = CloseButtonId;
            upperRightCloseButton.leftNeighborID = upperRightCloseButton.downNeighborID = OwnerButtonsBaseId;
            upperRightCloseButton.rightNeighborID = upperRightCloseButton.upNeighborID = -7777;

            loadButtons();

            lockDropdown = new Dropdown<Lock>(Enum.Parse<Lock>(current.modData["LockedChests.Lock"]), dropdownOptions, X + 48, Y + 288, 188, 44, DropdownBaseId);
            lockDropdown.downNeighborID = lockDropdown.rightNeighborID = AccessButtonsBaseId;
            lockDropdown.leftNeighborID = lockDropdown.upNeighborID = OwnerButtonsBaseId;
            lockDropdown.SelectionChanged += (s, e) => current.modData["LockedChests.Lock"] = $"{lockDropdown.Selected}";
        }

        private void loadButtons()
        {
            int x = X + 48;
            for (int i = 0; i < FarmerCount; i++)
            {
                x += 80 * i;
                Farmer f = Game1.getAllFarmers().ElementAt(i);

                var ctcO = new ClickableTextureComponent(new Rectangle(x, Y + 154, 64, 64), Game1.mouseCursors, buttonBackgroundRect, 4f, false)
                {
                    myID = OwnerButtonsBaseId + i,
                    rightNeighborID = (i == FarmerCount - 1 ? DropdownBaseId : OwnerButtonsBaseId + i + 1),
                    leftNeighborID = (i == 0 ? CloseButtonId : OwnerButtonsBaseId + i - 1),
                    downNeighborID = DropdownBaseId,
                    upNeighborID = CloseButtonId,
                    name = $"{f.UniqueMultiplayerID}",
                    hoverText = f.Name + (Convert.ToInt64(current.modData["LockedChests.Owner"]) == f.UniqueMultiplayerID ? ITranslations.Get("Is_Owner") : "")
                };
                ownerButtons.Add(ctcO);

                var ctcA = new ClickableTextureComponent(new Rectangle(x, Y + 404, 64, 64), Game1.mouseCursors, buttonBackgroundRect, 4f, false)
                {
                    myID = AccessButtonsBaseId + i,
                    rightNeighborID = (i == FarmerCount - 1 ? -7777 : AccessButtonsBaseId + i + 1),
                    leftNeighborID = (i == 0 ? DropdownBaseId : AccessButtonsBaseId + i - 1),
                    downNeighborID = -7777,
                    upNeighborID = DropdownBaseId,
                    name = $"{f.UniqueMultiplayerID}",
                    hoverText = f.Name + (Json.Read<List<long>>(current.modData["LockedChests.AccessIds"]) is not null and List<long> l && l.Any(x => x == f.UniqueMultiplayerID) ? ITranslations.Get("Has_Access") : ITranslations.Get("No_Access"))
                };
                if (Convert.ToInt64(current.modData["LockedChests.Owner"]) == f.UniqueMultiplayerID)
                    ctcA.hoverText = f.Name + ITranslations.Get("Is_Owner");
                accessButtons.Add(ctcA);
            }
        }

        private void getClickableComponentList()
        {
            allClickableComponents = new List<ClickableComponent>();
            for (int i = 0; i < ownerButtons.Count; i++)
                allClickableComponents.Add(ownerButtons[i]);
            for (int i = 0; i < accessButtons.Count; i++)
                allClickableComponents.Add(accessButtons[i]);
            allClickableComponents.Add(lockDropdown);
            allClickableComponents.Add(upperRightCloseButton);
        }

        private ClickableComponent? getComponentWithId(int id)
        {
            getClickableComponentList();
            for (int i = 0; i < allClickableComponents.Count; i++)
                if (allClickableComponents[i].myID == id || allClickableComponents[i].myAlternateID == id)
                    return allClickableComponents[i];
            return null;
        }

        private void exitMenu()
        {
            Game1.playSound("bigDeSelect");
            exitThisMenu();
        }

        public override void snapToDefaultClickableComponent() => currentlySnappedComponent = getComponentWithId(ownerButtons.Count > 0 ? OwnerButtonsBaseId : CloseButtonId);

        public override void setCurrentlySnappedComponentTo(int id)
        {
            if (id == -7777) return;
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

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            if (lockDropdown.tryHover(x, y)) return;
            for (int i = 0; i < ownerButtons.Count; i++)
            {
                if (ownerButtons[i].containsPoint(x, y))
                    hoverText = ownerButtons[i].hoverText;
            }
            for (int i = 0; i < accessButtons.Count; i++)
            {
                if (accessButtons[i].containsPoint(x, y))
                    hoverText = accessButtons[i].hoverText;
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (!finishedIntializing) return;

            switch (b)
            {
                case Buttons.Back:
                case Buttons.B:
                case Buttons.Y:
                    exitMenu();
                    return;
                default:
                    base.receiveGamePadButton(b);
                    return;
            }
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent == null) 
                snapToDefaultClickableComponent();
            if (lockDropdown.IsOpen)
            {
                if (direction == 0)
                {
                    var newSelected = lockDropdown.Options.ElementAtOrDefault(lockDropdown.SelectedIndex - 1);
                    if (newSelected.Value is null)
                        lockDropdown.Selected = lockDropdown.Options.ElementAt(lockDropdown.Options.Count - 1).Key;
                    else
                        lockDropdown.Selected = newSelected.Key;
                    Game1.playSound("smallSelect");
                }
                else if (direction == 2)
                {
                    var newSelected = lockDropdown.Options.ElementAtOrDefault(lockDropdown.SelectedIndex + 1);
                    if (newSelected.Value is null)
                        lockDropdown.Selected = lockDropdown.Options.ElementAt(0).Key;
                    else
                        lockDropdown.Selected = newSelected.Key;
                    Game1.playSound("smallSelect");
                }
                return;
            }
            switch (direction)
            {
                case 0:
                    if (currentlySnappedComponent.upNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 1:
                    if (currentlySnappedComponent.rightNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.rightNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2:
                    if (currentlySnappedComponent.downNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 3:
                    if (currentlySnappedComponent.leftNeighborID < 0) break;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.leftNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                default:
                    base.applyMovementKey(direction);
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!finishedIntializing) return;

            base.receiveLeftClick(x, y, playSound);
            if (lockDropdown.tryLeftClick(x, y, playSound)) return;
            for (int i = 0; i < ownerButtons.Count; i++)
            {
                if (ownerButtons[i].containsPoint(x, y))
                {
                    var owner = Convert.ToInt64(current.modData["LockedChests.Owner"]);
                    var id = Convert.ToInt64(ownerButtons[i].name);
                    var player = Game1.getFarmerMaybeOffline(id);
                    if (id == owner || player is null) return;
                    Game1.activeClickableMenu = new ConfirmationDialog(string.Format(ITranslations.Get("Transfer_Dialogue_Title"), player.Name), (x) => { ModEntry.TransferOwner(player, current); Game1.activeClickableMenu.exitThisMenu(); } );
                    return;
                }
            }
            for (int i = 0; i < accessButtons.Count; i++)
            {
                if (accessButtons[i].containsPoint(x, y))
                {
                    var accessIds = Json.Read<List<long>>(current.modData["LockedChests.AccessIds"]) ?? new List<long>();
                    var owner = Convert.ToInt64(current.modData["LockedChests.Owner"]);
                    var id = Convert.ToInt64(accessButtons[i].name);
                    if (accessIds.Contains(id) && id != owner)
                    {
                        accessIds.Remove(id);
                        current.modData["LockedChests.AccessIds"] = Json.Write(accessIds);
                        accessButtons[i].hoverText = Game1.getFarmerMaybeOffline(id).Name + ITranslations.Get("No_Access");
                        Game1.playSound("smallSelect");
                        return;
                    }
                    else if (id == owner)
                    {
                        Game1.activeClickableMenu = new DialogueBox(ITranslations.Get("No_Revoke_Owner"));
                        return;
                    }
                    else if (!accessIds.Contains(id))
                    {
                        accessIds.Add(id);
                        current.modData["LockedChests.AccessIds"] = Json.Write(accessIds);
                        accessButtons[i].hoverText = Game1.getFarmerMaybeOffline(id).Name + ITranslations.Get("Has_Access");
                        Game1.playSound("smallSelect");
                        return;
                    }
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            updateLayout();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            Game1.drawDialogueBox(X, Y, Width, Height, false, true);
            upperRightCloseButton.draw(b);

            if (ownerButtons.Count > 0)
                b.DrawString(Game1.dialogueFont, ITranslations.Get("Owner_Header"), new Vector2(ownerButtons[0].bounds.X, ownerButtons[0].bounds.Y - 48), Game1.textColor);
            for (int i = 0; i < ownerButtons.Count; i++)
            {
                bool isCurrentOwner = Convert.ToInt64(ownerButtons[i].name) == Game1.player.UniqueMultiplayerID;
                if (isCurrentOwner) ownerButtons[i].draw(b, Color.LightGreen, 0f);
                else ownerButtons[i].draw(b);
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(113, 479, 4, 4), ownerButtons[i].bounds.X + 8, ownerButtons[i].bounds.Y + 8, 48, 48, isCurrentOwner ? Color.LightGreen : Color.White, 4f, false, 0.1f);
                if (Game1.getAllFarmers().ElementAtOrDefault(i) is not null and Farmer f)
                    f.FarmerRenderer.drawMiniPortrat(b, new Vector2(ownerButtons[i].bounds.X + 8, ownerButtons[i].bounds.Y + 8), 0.2f, 3f, 2, f);
            }

            if (accessButtons.Count > 0)
                b.DrawString(Game1.dialogueFont, ITranslations.Get("Access_Header"), new Vector2(accessButtons[0].bounds.X, accessButtons[0].bounds.Y - 48), Game1.textColor);
            for (int i = 0; i < accessButtons.Count; i++)
            {
                bool hasAccess = Json.Read<List<long>>(current.modData["LockedChests.AccessIds"])?.Contains(Convert.ToInt64(accessButtons[i].name)) ?? false;
                if (hasAccess) accessButtons[i].draw(b, Color.LightGreen, 0f);
                else accessButtons[i].draw(b);
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(113, 479, 4, 4), accessButtons[i].bounds.X + 8, accessButtons[i].bounds.Y + 8, 48, 48, hasAccess ? Color.LightGreen : Color.White, 4f, false, 0.1f);
                if (Game1.getAllFarmers().ElementAtOrDefault(i) is not null and Farmer f)
                    f.FarmerRenderer.drawMiniPortrat(b, new Vector2(accessButtons[i].bounds.X + 8, accessButtons[i].bounds.Y + 8), 0.2f, 3f, 2, f);
            }

            b.DrawString(Game1.dialogueFont, ITranslations.Get("Lock_Header"), new Vector2(lockDropdown.X, lockDropdown.Y - 48), Game1.textColor);
            lockDropdown.Draw(b);

            if (!string.IsNullOrWhiteSpace(hoverText))
                drawHoverText(b, hoverText, Game1.smallFont);

            drawMouse(b);
            if (!finishedIntializing) finishedIntializing = true;
        }
    }
}
